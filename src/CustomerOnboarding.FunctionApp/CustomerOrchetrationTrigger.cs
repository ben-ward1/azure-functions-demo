using CustomerOnboarding.Core.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using CustomerOnboarding.FunctionApp.Activities;
using CustomerOnboarding.Core.Enums;
using Customer = CustomerOnboarding.Core.Models.Customer;

namespace CustomerOnboarding.FunctionApp
{
    public class CustomerOrchetrationTrigger
    {
        /// <summary>
        /// Orchestration that executes three activities in parralel, followed up by two chained activities.
        /// See <a href="https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations?tabs=csharp-inproc#features-and-patterns">
        /// Durable Orchestrations documentation.</a>
        /// </summary>
        [Function(nameof(NewCustomer))]
        public async Task NewCustomer([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var customer = context.GetInput<Customer>() ?? throw new ArgumentNullException();
            
            // ============================
            // PARRALLEL ACTIVITIES EXAMPLE
            // ============================

            // Invoke three activity functions in parrallel
            var accountManagerTask = context.CallActivityAsync<string>(nameof(AssignAccountManagerActivity.Assign), customer.Id);
            var credentialTask = context.CallActivityAsync<Credentials>(nameof(GenerateCredentialsActivity.Generate), customer.Id);
            var orderSwagTask = context.CallActivityAsync<ShirtSize>(nameof(OrderSwagActivity.Order), customer);

            // Await all for completion
            await Task.WhenAll([
                accountManagerTask,
                credentialTask,
                orderSwagTask
                ]);

            // gather results
            var managerName = accountManagerTask.Result;
            var credentials = credentialTask.Result;
            var shirtSize = orderSwagTask.Result;
            var outcome = new OnboardingCompleteOutcome(customer, shirtSize, managerName, credentials);



            // ========================================
            // SEQUENTIAL (CHAINING) ACTIVITIES EXAMPLE
            // ========================================

            // Await first activity
            var sentEmail = await context.CallActivityAsync<bool>(nameof(EmailActivity.WelcomeCustomer), outcome);

            // Await second activity
            await context.CallActivityAsync(nameof(EmailActivity.NotifyManager), (outcome, sentEmail));
        }
    }
}
