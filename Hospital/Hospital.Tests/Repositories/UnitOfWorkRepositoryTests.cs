using FluentAssertions;
using Hospital.Db;
using Hospital.Db.Entities;
using Hospital.Repositories.UnitOfWorkRepository;
using Hospital.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Tests.Repositories
{
    public class UnitOfWorkRepositoryTests
    {
        private readonly HospitalContext _context;
        private readonly UnitOfWorkRepository _repository;

        public UnitOfWorkRepositoryTests()
        {
            _context = TestDbContextFactory.Create();
            _repository = new UnitOfWorkRepository(_context);
        }

        [Fact]
        public async Task BeginTransactionAsync_ShouldThrowInvalidOperationException_WhenUsingInMemoryDatabase()
        {
            var action = async () => await _repository.BeginTransactionAsync();

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldSaveChangesToDb()
        {
            var specialty = new Specialty
            {
                Name = "Therapy",
                Price = 100m
            };

            await _context.Specialties.AddAsync(specialty);
            await _repository.SaveChangesAsync();

            var result = await _context.Specialties.FirstOrDefaultAsync();

            result.Should().NotBeNull();
            result.Name.Should().Be(specialty.Name);
            result.Price.Should().Be(specialty.Price);
        }
    }
}
