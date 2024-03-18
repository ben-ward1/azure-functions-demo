using CustomerOnboarding.Core.Enums;
using CustomerOnboarding.Core.Models;
using CustomerOnboarding.Data;
using CustomerOnboarding.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Customer = CustomerOnboarding.Core.Models.Customer;

namespace CustomerOnboarding.FunctionApp.Activities
{
    internal class OrderSwagActivity(SwagOrderingService _service, ILogger<OrderSwagActivity> _logger)
    {
        [Function(nameof(Order))]
        public async Task<ShirtSize> Order([ActivityTrigger] Customer customer)
        {
            _logger.LogWarning("[Activity Started] Ordering swag for customer " + customer.Id);
            var shirtSize = await _service.OrderShirt(customer.Id, customer.ShirtSize);
            _logger.LogWarning("[Activity Complete] Ordered swag for customer " + customer.Id);
            return shirtSize;
        }
    }
}
