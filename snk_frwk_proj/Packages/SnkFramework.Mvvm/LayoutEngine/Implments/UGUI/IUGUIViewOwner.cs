using SnkFramework.Mvvm.Core.View;
using UnityEngine;

namespace SnkFramework.Mvvm.LayoutEngine
{
    namespace UGUI
    {
        public interface IUGUIViewOwner : ISnkViewOwner
        {
            public CanvasGroup mCanvasGroup { get; }
            public Canvas mCanvas { get; }
        }
    }
}