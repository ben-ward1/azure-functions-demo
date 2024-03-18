using CustomerOnboarding.Core.Enums;

namespace CustomerOnboarding.Data
{
    public class Customer
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public ShirtSize ShirtSize { get; init; }

        public static Customer New(Core.Models.Customer customer)
        {
            return new Customer
            {
                Id = default,
                Name = customer.Name,
                ShirtSize = customer.ShirtSize,
            };
        }
    }
}
