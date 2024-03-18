using CustomerOnboarding.Core.Enums;

namespace CustomerOnboarding.Core.Models
{
    public record OnboardingCompleteOutcome(Customer Customer, ShirtSize ShirtOrdered, string ManagerName, Credentials Credentials);
}
