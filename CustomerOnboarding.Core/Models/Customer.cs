using CustomerOnboarding.Core.Enums;

namespace CustomerOnboarding.Core.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ShirtSize ShirtSize { get; set; }
    }
}
