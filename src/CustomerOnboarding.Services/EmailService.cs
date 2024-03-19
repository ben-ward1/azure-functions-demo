using CustomerOnboarding.Core.Models;
using Microsoft.Extensions.Logging;

namespace CustomerOnboarding.Services
{
    public class EmailService(ILogger<EmailService> _logger)
    {
        public async Task<bool> NotifyManager(OnboardingCompleteOutcome outcome, bool successfulWelcomeEmail)
        {
            var customerName = outcome.Customer.Name;

            var emailText = $"A new customer, {customerName}, has been assigned to you.\n" +
                $"Please greet them at your earliest convenience.";

            if (!successfulWelcomeEmail)
            {
                emailText += "\nThey had no email on file, so you may need to reach out to them directly.";
            }

            _logger.LogWarning(emailText);

            return true;
        }

        public async Task<bool> WelcomeCustomer(OnboardingCompleteOutcome outcome)
        {
            var customer = outcome.Customer;
            var shirtOrdered = outcome.ShirtOrdered;
            var managerName = outcome.ManagerName;
            var credentials = outcome.Credentials;
            var wrongSize = customer.ShirtSize != shirtOrdered;
            
            var emailText = $"Hi, {outcome.Customer.Name}. Welcome aboard!\n" +
                $"Your credentials are {credentials.Username}:{credentials.Password}.\n" +
                $"An account representative, {managerName}, has been assigned to you and will be reaching out to shortly.\n" +
                $"Be on the lookout in the mail for some cool new swag";

            if (wrongSize)
            {
                emailText += $"\nWe didn't have your size shirt in stock, but hopefully a size {shirtOrdered} will do.";
            }

            _logger.LogWarning(emailText);

            return true;
        }
    }
}
