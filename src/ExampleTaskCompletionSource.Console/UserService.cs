using System.Threading.Tasks;

namespace ExampleTaskCompletionSource
{
    public class UserService
    {
        private TaskCompletionSource<bool> _tcs;

        public Task<bool> WaitForUserConfirmation()
        {
            _tcs = new TaskCompletionSource<bool>();
            return _tcs.Task;
        }

        public void OnUserActionCompleted(bool isConfirmed)
        {
            _tcs.SetResult(isConfirmed);
        }
    }
}