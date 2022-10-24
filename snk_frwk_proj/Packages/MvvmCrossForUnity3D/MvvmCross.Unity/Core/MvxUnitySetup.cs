using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using Microsoft.Extensions.Logging;

using MvvmCross.Core;
using MvvmCross.IoC;
using MvvmCross.Plugin;
using MvvmCross.Presenters;
using MvvmCross.Unity.Base.ResourceService;
using MvvmCross.Unity.Logging;
using MvvmCross.Unity.Presenters;
using MvvmCross.Unity.Views;
using MvvmCross.Unity.Views.UGUI;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using UnityEngine;

namespace MvvmCross.Unity.Core
{
    public abstract class MvxUnitySetup : MvxSetup, IMvxUnitySetup
    {
        private IMvxUnityViewPresenter _presenter;
        protected IMvxUnityViewPresenter Presenter => _presenter ??= CreateViewPresenter();

        private SynchronizationContext _synchronizationContext;

        public virtual void PlatformInitialize(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        protected virtual MvxUnityApplicationLifetime CreateLifetimeMonitor() => new ();

        protected virtual void InitializeLifetimeMonitor(IMvxIoCProvider iocProvider)
        {
            ValidateArguments(iocProvider);

            var lifetimeMonitor = CreateLifetimeMonitor();

            iocProvider.RegisterSingleton<IMvxUnityApplicationLifetime>(lifetimeMonitor);
            iocProvider.RegisterSingleton<IMvxLifetime>(lifetimeMonitor);
        }

        protected virtual void RegisterPresenter(IMvxIoCProvider iocProvider)
        {
            ValidateArguments(iocProvider);

            var presenter = Presenter;
            iocProvider.RegisterSingleton(presenter);
            iocProvider.RegisterSingleton<IMvxViewPresenter>(presenter);
        }

        protected virtual IMvxUnityViewPresenter CreateViewPresenter()
            => new MvxUnityViewPresenter();

        protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
        {
            ValidateArguments(iocProvider);

            InitializeLifetimeMonitor(iocProvider);
            RegisterPresenter(iocProvider);
                
            base.InitializeFirstChance(iocProvider);
        }

        protected virtual MvxUnityLayerContainer CreateUnityLayerContainer()
        {
            GameObject layerContainerGameObject = new GameObject(nameof(MvxUnityLayerContainer));
            GameObject.DontDestroyOnLoad(layerContainerGameObject);
            return layerContainerGameObject.AddComponent<MvxUnityLayerContainer>();
        }

        protected virtual MvxUnityLayerBuilder CreateUnityLayerBuilder()
        {
            MvxUnityLayerBuilder builder = new MvxUnityLayerBuilder();
            RegiestUnityLayerCreator(builder);
            return builder;
        }

        protected virtual void RegiestUnityLayerCreator(IMvxUnityLayerBuilder builder)
        {
            builder.RegiestUnityLayerBuilder<MvxUGUINormalLayer>();
            builder.RegiestUnityLayerBuilder<MvxUGUIDialogueLayer>();
            builder.RegiestUnityLayerBuilder<MvxUGUIPopupLayer>();
            builder.RegiestUnityLayerBuilder<MvxUGUIGuideLayer>();
            builder.RegiestUnityLayerBuilder<MvxUGUILoadingLayer>();
            builder.RegiestUnityLayerBuilder<MvxUGUISystemLayer>();
        }

        protected virtual void InitializeUnityLayerContainer(IMvxIoCProvider iocProvider)
        {
            var unityLayerContainer = CreateUnityLayerContainer();
            var builder = CreateUnityLayerBuilder();
            var unityLayerLookup = builder.Build();
            unityLayerContainer.AddAll(unityLayerLookup);
            iocProvider.RegisterSingleton<IMvxUnityLayerContainer>(unityLayerContainer);
        }

        protected override void RegisterDefaultSetupDependencies(IMvxIoCProvider iocProvider)
        {
            base.RegisterDefaultSetupDependencies(iocProvider);

            InitializeUnityLayerContainer(iocProvider);
        }

        protected virtual MvxUnityResourceService CreateResourceService() => new();
        
        protected override void InitializeLastChance(IMvxIoCProvider iocProvider)
        {
            base.InitializeLastChance(iocProvider);
            
            var resourceService = CreateResourceService();
            iocProvider.RegisterSingleton<IMvxUnityResourceService>(resourceService);
        }

        protected virtual void RegisterUnityViewCreator(IMvxIoCProvider iocProvider, IMvxUnityViewsContainer container)
        {
            ValidateArguments(iocProvider);
            iocProvider.RegisterSingleton<IMvxUnityViewCreator>(container);
        }

        protected virtual IMvxUnityViewsContainer CreateUnityViewsContainer()
            => new MvxUnityViewsContainer();

        protected override IMvxViewsContainer CreateViewsContainer(IMvxIoCProvider iocProvider)
        {
            var container = CreateUnityViewsContainer();
            RegisterUnityViewCreator(iocProvider, container);
            return container;
        }

        protected override IMvxViewDispatcher CreateViewDispatcher()
            => new MvxUnityThreadDispatcher(_synchronizationContext, Presenter);

        protected override ILoggerProvider? CreateLogProvider()
            => new UnityLoggerProvider();

        protected override ILoggerFactory? CreateLogFactory()
            => new UnityLoggerFactory();

        protected override IMvxNameMapping CreateViewToViewModelNaming()
            => new MvxPostfixAwareViewToViewModelNameMapping("Window", "ViewModel");

        public override void LoadPlugins(IMvxPluginManager pluginManager)
        {
            base.LoadPlugins(pluginManager);
            pluginManager.EnsurePluginLoaded<MvvmCross.Plugin.Messenger.Plugin>();
        }
    }
    
    public abstract class MvxUnitySetup<TApplication> : MvxUnitySetup
        where TApplication : class, IMvxApplication, new()
    {
        protected override IMvxApplication CreateApp(IMvxIoCProvider iocProvider) =>
            iocProvider.IoCConstruct<TApplication>();

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            return new[] { typeof(TApplication).GetTypeInfo().Assembly };
        }
    }
}