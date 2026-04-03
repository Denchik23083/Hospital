using Hospital.Db;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static HospitalContext Create()
        {
            var options = new DbContextOptionsBuilder<HospitalContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new HospitalContext(options);
        }
    }
}