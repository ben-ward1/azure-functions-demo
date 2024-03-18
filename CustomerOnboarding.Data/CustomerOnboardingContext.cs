using Microsoft.EntityFrameworkCore;

namespace CustomerOnboarding.Data
{
    public class CustomerOnboardingContext : DbContext
    {
        public CustomerOnboardingContext(DbContextOptions<CustomerOnboardingContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
    }
}
