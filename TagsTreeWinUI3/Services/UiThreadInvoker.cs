namespace TagsTreeWinUI3.Services
{
	public static class UiThreadInvoker
	{
		public static bool Invoke(Microsoft.UI.Dispatching.DispatcherQueueHandler dispatcherQueueHandler) => App.Window.DispatcherQueue.TryEnqueue(dispatcherQueueHandler);
	}
}