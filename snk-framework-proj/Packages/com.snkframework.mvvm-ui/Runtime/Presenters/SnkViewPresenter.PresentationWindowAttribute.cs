using System;
using System.Threading.Tasks;
using SnkFramework.Mvvm.Runtime.Presenters.Attributes;
using SnkFramework.Mvvm.Runtime.View;
using SnkFramework.Mvvm.Runtime.ViewModel;

namespace SnkFramework.Mvvm.Runtime
{
    namespace Presenters
    {
        public partial class SnkViewPresenter
        {
            protected virtual async Task<bool> OpenWindow(ISnkPresentationAttribute attribute, SnkViewModelRequest request)
            {
                SnkWindow window = await this._viewLoader.CreateView(request);
                var windowAttribute = attribute as SnkPresentationWindowAttribute;
                if (windowAttribute == null)
                    throw new ArgumentNullException(nameof(windowAttribute) + " is null");
                var layer = _layerContainer.GetLayer(windowAttribute.LayerType);
                window.Create(layer, null);
                await layer.Open(window);
                return true;
            }

            protected virtual async Task<bool> CloseWindow(ISnkViewModel viewModel, ISnkPresentationAttribute attribute)
            {
                var windowAttribute = attribute as SnkPresentationWindowAttribute;
                if (windowAttribute == null)
                    throw new ArgumentNullException(nameof(windowAttribute) + " is null");
                var layer = _layerContainer.GetLayer(windowAttribute.LayerType);
                SnkWindow window = layer.GetChild(0);
                await layer.Close(window);
                this._viewLoader.UnloadView(window);
                return true;
            }
        }
    }
}