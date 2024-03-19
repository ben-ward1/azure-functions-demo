namespace CustomerOnboarding.Services
{
    public class ManagerAssignerService
    {
        public async Task<string> AssignAccountManager(Guid customerId)
        {
            await Task.Delay(500);
            return "John Doe";
        }
    }
}
