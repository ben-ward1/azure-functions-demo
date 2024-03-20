using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CustomerOnboarding.Core.Enums;

namespace CustomerOnboarding.FunctionApp.Triggers
{
    public class PurchaseOrderQueueTrigger(ILogger<PurchaseOrderQueueTrigger> _logger)
    {
        /// <summary>
        /// Handles the queue output from OrderSwagActivity. A closer to real-life
        /// example would place this in a separate function app for individual scaling.
        /// It would run just as it does here as long as the queue connection is provided.
        /// </summary>
        /// <param name="size"></param>
        [Function(nameof(HandleOrder))]
        public void HandleOrder([QueueTrigger("purchase-order-input", Connection = "StorageAccountConnection")] ShirtSize size)
        {
            _logger.LogWarning($"[PurchaseOrderQueue] Ordering shirt size {size}");
        }
    }
}
