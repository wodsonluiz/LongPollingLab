using System.Threading.Tasks;

namespace ExampleTaskCompletionSource
{
    public class Application
    {
        private UserService _userService;
        public async Task Run()
        {
            var confirmationTask = _userService.WaitForUserConfirmation();

            var confirmed = await confirmationTask;

            if (confirmed)
            {
                System.Console.WriteLine("User confirmed");
            }
            else
            {
                System.Console.WriteLine("User didn't confirm");
            }
        }

        public void OnUserAction(bool isConfirmed)
        {
            _userService.OnUserActionCompleted(isConfirmed);
        }
    }
}