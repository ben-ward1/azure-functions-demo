using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace CustomerOnboarding.FunctionApp.Triggers
{
    public class CustomerSqlTrigger(ILoggerFactory _loggerFactory)
    {
        private readonly ILogger<CustomerOrchetrationTrigger> _logger = _loggerFactory.CreateLogger<CustomerOrchetrationTrigger>();

        /// <summary>
        /// Leverages SQL Server's change tracking feature to trigger function.
        /// See https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-azure-sql-trigger?tabs=isolated-process%2Cportal&pivots=programming-language-csharp#set-up-change-tracking-required
        /// </summary>
        /// <param name="changes">Changes made to the Customers table</param>
        /// <param name="client">Used to invoke durable orchestrations</param>
        [Function(nameof(CustomerTableSqlTrigger))]
        public async Task CustomerTableSqlTrigger(
            [SqlTrigger("[dbo].[Customers]", "CustomerOnboardingDbConnection")] IReadOnlyList<SqlChange<Data.Customer>> changes,
            [DurableClient] DurableTaskClient client)
        {
            var newCustomers = changes
                .Where(c => c.Operation == SqlChangeOperation.Insert)
                .Select(c => c.Item);

            foreach (var customer in newCustomers)
            {
                await client.ScheduleNewOrchestrationInstanceAsync(nameof(CustomerOrchetrationTrigger.NewCustomer), customer.ToDto());
            }
        }
    }
}
