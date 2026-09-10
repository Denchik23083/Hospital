using AutoMapper;
using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.NotificationService
{
    public class NotificationService(INotificationRepository repository,
            ILogger<NotificationService> logger,
            IMapper mapper,
            IUnitOfWorkRepository unitOfWorkRepository) : INotificationService
    {
        private readonly INotificationRepository _repository = repository;
        private readonly ILogger<NotificationService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<NotificationResponse>> GetAllNotificationsAsync(int userId, CancellationToken ct)
        {
            var notifications = await _repository.GetAllNotificationsAsync(userId, ct);
        
            return _mapper.Map<IEnumerable<NotificationResponse>>(notifications);
        }

        public async Task DeleteNotificationAsync(int id, int userId, CancellationToken ct)
        {
            var notificationToDelete = await _repository.GetNotificationAsync(id, userId, ct);

            if (notificationToDelete is null)
            {
                _logger.LogWarning("Notification not found");
                throw new NotificationNotFoundException("Notification not found");
            }

            await _repository.DeleteNotificationAsync(notificationToDelete);
            await _unitOfWorkRepository.SaveChangesAsync();
        }
    }
}
