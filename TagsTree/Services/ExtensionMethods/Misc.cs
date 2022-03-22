using System;

namespace TagsTree.Services.ExtensionMethods;

public static class Misc
{
    public static T CastThrow<T>(this object? obj) where T : notnull => (T)(obj ?? throw new InvalidCastException());
    public static bool Invoke(Microsoft.UI.Dispatching.DispatcherQueueHandler dispatcherQueueHandler) => WindowHelper.Window.DispatcherQueue.TryEnqueue(dispatcherQueueHandler);
}