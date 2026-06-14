using Avalonia.Threading;

namespace Soundboword;

public static class DispatcherExtensions
{

    extension(Dispatcher dispatcher)
    {

        public void InvokeOrPost(Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Post(action);
        }

    }

}
