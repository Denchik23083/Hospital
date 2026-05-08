using Hospital.Core.Exceptions;
using Hospital.Core.Models.Response;
using Hospital.Repositories.NotificationRepository;
using Hospital.Repositories.UnitOfWorkRepository;
using Microsoft.Extensions.Logging;

namespace Hospital.Services.NotificationService
{
    public class NotificationService(INotificationRepository repository,
            ILogger<NotificationService> logger,
            IUnitOfWorkRepository unitOfWorkRepository) : INotificationService
    {
        private readonly INotificationRepository _repository = repository;
        private readonly ILogger<NotificationService> _logger = logger;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository = unitOfWorkRepository;

        public async Task<IEnumerable<NotificationResponce>> GetAllNotificationsAsync(int userId)
        {
            return await _repository.GetAllNotificationsAsync(userId);
        }

        public async Task DeleteNotificationAsync(int id, int userId)
        {
            var notificationToDelete = await _repository.GetNotificationAsync(id, userId);

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
