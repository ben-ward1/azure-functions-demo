using CustomerOnboarding.Core.Models;
using CustomerOnboarding.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;
using CustomerOnboarding.FunctionApp.Activities;
using CustomerOnboarding.Core.Enums;
using Customer = CustomerOnboarding.Core.Models.Customer;

namespace CustomerOnboarding.FunctionApp
{
    public class NewCustomerFunction(ILoggerFactory _loggerFactory, CustomerOnboardingContext _db)
    {
        private readonly ILogger<NewCustomerFunction> _logger = _loggerFactory.CreateLogger<NewCustomerFunction>();

        [Function(nameof(PostCustomer))]
        public async Task<IActionResult> PostCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req,
            [FromBody] CustomerPostRequest customerRequest,
            FunctionContext executionContext)
        {
            // save customer to db
            var customer = Data.Customer.New(customerRequest);
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            // log and respond
            _logger.LogWarning("[Created Customer] " + JsonConvert.SerializeObject(customer, Formatting.Indented));
            return new CreatedAtRouteResult("customer", new { id = customer.Id }, customer);
        }

        [Function(nameof(CustomerTableSqlTrigger))]
        public async Task CustomerTableSqlTrigger(
            [SqlTrigger("[dbo].[Customers]", "CustomerOnboardingDbConnection")] IReadOnlyList<SqlChange<Data.Customer>> changes,
            [DurableClient] DurableTaskClient client,
            FunctionContext context)
        {
            var newCustomers = changes
                .Where(c => c.Operation == SqlChangeOperation.Insert)
                .Select(c => c.Item);

            foreach (var customer in newCustomers)
            {
                await client.ScheduleNewOrchestrationInstanceAsync(nameof(NewCustomerOrchestration), customer.ToDto());
            }
        }

        [Function(nameof(NewCustomerOrchestration))]
        public async Task NewCustomerOrchestration([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var customer = (context.GetInput<Customer>()) ?? throw new Exception();
            
            var accountManagerTask = context.CallActivityAsync<string>(nameof(AssignAccountManagerActivity.Assign), customer.Id);
            var credentialTask = context.CallActivityAsync<Credentials>(nameof(GenerateCredentialsActivity.Generate), customer.Id);
            var orderSwagTask = context.CallActivityAsync<ShirtSize>(nameof(OrderSwagActivity.Order), customer);

            await Task.WhenAll([
                accountManagerTask,
                credentialTask,
                orderSwagTask
                ]);

            var managerName = accountManagerTask.Result;
            var credentials = credentialTask.Result;
            var shirtSize = orderSwagTask.Result;

            var outcome = new OnboardingCompleteOutcome(customer, shirtSize, managerName, credentials);

            var sentEmail = await context.CallActivityAsync<bool>(nameof(EmailActivity.WelcomeCustomer), outcome);
            await context.CallActivityAsync(nameof(EmailActivity.NotifyManager), (outcome, sentEmail));
        }
    }
}
