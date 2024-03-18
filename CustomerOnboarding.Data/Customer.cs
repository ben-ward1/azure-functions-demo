using CustomerOnboarding.Core.Enums;

namespace CustomerOnboarding.Data
{
    public class Customer
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public ShirtSize ShirtSize { get; init; }

        public static Customer New(Core.Models.CustomerPostRequest customer)
        {
            return new Customer
            {
                Id = default,
                Name = customer.Name,
                ShirtSize = customer.ShirtSize,
            };
        }

        public Core.Models.Customer ToDto()
        {
            return new Core.Models.Customer
            {
                Id = this.Id,
                Name = this.Name,
                ShirtSize = this.ShirtSize
            };
        }
    }
}
