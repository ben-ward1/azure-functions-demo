using CustomerOnboarding.Core.Models;
using CustomerOnboarding.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CustomerOnboarding.FunctionApp.Activities
{
    internal class GenerateCredentialsActivity(CustomerCredentialService _service, ILogger<GenerateCredentialsActivity> _logger)
    {
        [Function(nameof(Generate))]
        public async Task<Credentials> Generate([ActivityTrigger] Guid customerId)
        {
            _logger.LogWarning("[Activity Started] Generating credentials for customer " + customerId);
            var credentials = await _service.CreateCredentials(customerId);
            _logger.LogWarning("[Activity Complete] Generating credentials for customer " + customerId);
            return credentials;
        }
    }
}
