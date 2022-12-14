using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SnkFramework.Mvvm.Runtime.Presenters;
using SnkFramework.Mvvm.Runtime.View;
using UnityEngine;

namespace SnkFramework.Mvvm.Demo
{
    namespace Implements
    {
        public class DemoViewLoader : SnkViewLoader
        {
            public DemoViewLoader(ISnkViewFinder viewFinder) : base(viewFinder)
            {
            }

            public override async Task<SnkWindow> CreateView(Type viewType)
            {
                var asset = await Resources.LoadAsync<GameObject>(viewType.Name);
                GameObject inst = UnityEngine.Object.Instantiate(asset) as GameObject;
                if (inst == null)
                    return null;
                inst.name = viewType.Name;
                return inst.AddComponent(viewType) as SnkWindow;
            }
        }
    }
}