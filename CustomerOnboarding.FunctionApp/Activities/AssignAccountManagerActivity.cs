using CustomerOnboarding.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CustomerOnboarding.FunctionApp.Activities
{
    public class AssignAccountManagerActivity(ManagerAssignerService _service, ILogger<AssignAccountManagerActivity> _logger)
    {
        [Function(nameof(Assign))]
        public async Task<string> Assign([ActivityTrigger] Guid customerId)
        {
            _logger.LogWarning("[Activity Started] Assigning account manager for customer " +  customerId);
            var managerName = await _service.AssignAccountManager(customerId);
            _logger.LogWarning("[Activity Complete] Assigned manager for customer " + customerId);
            return managerName;
        }
    }
}
