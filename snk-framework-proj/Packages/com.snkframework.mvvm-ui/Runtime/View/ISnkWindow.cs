using SnkFramework.Mvvm.Runtime.Base;
using SnkFramework.Mvvm.Runtime.Layer;

namespace SnkFramework.Mvvm.Runtime
{
    namespace View
    {
        public interface ISnkWindow : ISnkView, ISnkUINodeUnit, ISnkNavigator, ISnkContainer<ISnkPage>
        {
            public WIN_STATE WindowState { get; }

            public ISnkLayer Layer { get; }
            public SnkTransitionOperation Show(bool animated);
            public SnkTransitionOperation Hide(bool animated);
            public SnkTransitionOperation Dismiss(bool animated);
            public void AddPage(ISnkPage page);
            public TViewModel AddPage<TViewModel>();
            public SnkTransitionOperation AddPageAsync<TViewModel>();
        }
    }
}