using CustomerOnboarding.Core.Models;

namespace CustomerOnboarding.Services
{
    public class CustomerCredentialService
    {
        public async Task<Credentials> CreateCredentials(Guid customerId)
        {
            await Task.Delay(2000);
            return new Credentials("foo", "bar");
        }
    }
}
