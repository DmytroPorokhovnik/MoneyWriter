namespace Common.Extensions
{
    public static class TaskExtensions
    {
        public static async void RunSafe(this Task task, Action<Exception> exceotionHandler)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                exceotionHandler?.Invoke(e);
            }
        }
    }
}
