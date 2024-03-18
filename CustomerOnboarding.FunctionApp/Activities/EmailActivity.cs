using CustomerOnboarding.Core.Models;
using CustomerOnboarding.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CustomerOnboarding.FunctionApp.Activities
{
    internal class EmailActivity(EmailService _service, ILogger<EmailActivity> _logger)
    {
        [Function(nameof(WelcomeCustomer))]
        public async Task<bool> WelcomeCustomer([ActivityTrigger] OnboardingCompleteOutcome outcome)
        {
            _logger.LogWarning("[Activity Started] Sending welcome email to customer " + outcome);
            var sent = await _service.WelcomeCustomer(outcome);
            _logger.LogWarning("[Activity Complete] Sent welcome email to customer " + outcome);
            return sent;
        }

        [Function(nameof(NotifyManager))]
        public async Task<bool> NotifyManager([ActivityTrigger] (OnboardingCompleteOutcome outcome, bool successfulEmail) payload)
        {
            var (outcome, successfulEmail) = payload;
            var customerId = outcome.Customer.Id;

            _logger.LogWarning("[Activity Started] Notifying manager that onboarding has completed for customer " + customerId);
            var sent = await _service.NotifyManager(outcome, successfulEmail);
            _logger.LogWarning("[Activity Complete] Notified manager that onboarding has completed for customer" + customerId);
            return sent;
        }
    }
}
