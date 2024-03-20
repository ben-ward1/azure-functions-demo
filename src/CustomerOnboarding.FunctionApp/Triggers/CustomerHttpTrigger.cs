using CustomerOnboarding.Core.Models;
using CustomerOnboarding.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace CustomerOnboarding.FunctionApp.Triggers
{
    public class CustomerHttpTrigger(CustomerOnboardingContext _db, ILoggerFactory _loggerFactory)
    {
        private readonly ILogger<CustomerHttpTrigger> _logger = _loggerFactory.CreateLogger<CustomerHttpTrigger>();

        [Function(nameof(PostCustomer))]
        public async Task<IActionResult> PostCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")][FromBody] CustomerPostRequest request)
        {
            var customer = Data.Customer.New(request);
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            _logger.LogWarning("[Created Customer] " + JsonConvert.SerializeObject(customer, Formatting.Indented));
            return new CreatedResult($"/customers/{customer.Id}", customer);
        }

        [Function(nameof(GetCustomer))]
        public async Task<IActionResult> GetCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/{id:guid}")] HttpRequestData request, Guid id)
        {
            var customer = await _db.Customers.FindAsync(id);
            return customer == null ? new NotFoundResult() : new OkObjectResult(customer);
        }
    }
}
