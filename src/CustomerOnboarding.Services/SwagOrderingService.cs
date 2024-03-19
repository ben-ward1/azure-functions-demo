using CustomerOnboarding.Core.Enums;

namespace CustomerOnboarding.Services
{
    public class SwagOrderingService
    {
        public async Task<ShirtSize> OrderShirt(Guid customerId, ShirtSize shirtSize)
        {
            await Task.Delay(500);

            var firstChar = customerId.ToString()[0];
            var inStock = Convert.ToInt16(firstChar) % 5 != 0;

            return inStock ? shirtSize : shirtSize + 1;
        }
    }
}
