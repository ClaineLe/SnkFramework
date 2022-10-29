using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnkFramework.Mvvm.Runtime.Presenters.Attributes;
using SnkFramework.Mvvm.Runtime.ViewModel;

namespace SnkFramework.Mvvm.Runtime.Presenters
{
    public partial class SnkViewPresenter
    {
        protected void internalRegisterAttributeTypes<TPresentationAttribute>(Func<ISnkPresentationAttribute, SnkViewModelRequest, Task<bool>> openAction, Func<ISnkViewModel, ISnkPresentationAttribute, Task<bool>> closeAction)
            where TPresentationAttribute : SnkBasePresentationAttribute
        {
            this.AttributeTypesToActionsDictionary[typeof(TPresentationAttribute)] =
                new SnkPresentationAttributeAction { OpenAction = openAction, CloseAction = closeAction };
        }

        public override void RegisterAttributeTypes()
        {
            internalRegisterAttributeTypes<SnkPresentationWindowAttribute>(ShowWindow, CloseWindow);
        }

        
        public override SnkBasePresentationAttribute CreatePresentationAttribute(Type viewModelType, Type viewType)
            => new SnkPresentationWindowAttribute();

        public override SnkBasePresentationAttribute GetOverridePresentationAttribute(SnkViewModelRequest request, Type viewType)
            => null;

        protected virtual SnkPresentationAttributeAction GetPresentationAttributeAction(SnkViewModelRequest request, out SnkBasePresentationAttribute attribute)
        { if (request == null)
                throw new ArgumentNullException(nameof(request));

            var presentationAttribute = GetPresentationAttribute(request);
            presentationAttribute.ViewModelType = request.ViewModelType;
            var attributeType = presentationAttribute.GetType();

            attribute = presentationAttribute;

            if (AttributeTypesToActionsDictionary != null &&
                AttributeTypesToActionsDictionary.TryGetValue(attributeType,
                    out SnkPresentationAttributeAction attributeAction))
            {
                if (attributeAction.OpenAction == null)
                {
                    throw new InvalidOperationException(
                        $"attributeAction.ShowAction is null for attribute: {attributeType.Name}");
                }

                if (attributeAction.CloseAction == null)
                {
                    throw new InvalidOperationException(
                        $"attributeAction.CloseAction is null for attribute: {attributeType.Name}");
                }

                return attributeAction;
            }

            throw new KeyNotFoundException($"The type {attributeType.Name} is not configured in the presenter dictionary");
        }
        
        public override SnkBasePresentationAttribute GetPresentationAttribute(SnkViewModelRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.ViewModelType == null)
                throw new InvalidOperationException("Cannot get view types for null ViewModelType");

            //if (ViewsContainer == null)
            //    throw new InvalidOperationException($"Cannot get view types from null {nameof(ViewsContainer)}");

            //var viewType = ViewsContainer.GetViewType(request.ViewModelType);
            var viewType = request.ViewType;
            if (viewType == null)
                throw new InvalidOperationException($"Could not get View Type for ViewModel Type {request.ViewModelType}");

            var overrideAttribute = GetOverridePresentationAttribute(request, viewType);
            if (overrideAttribute != null)
                return overrideAttribute;

            var attribute = viewType
                .GetCustomAttributes(typeof(SnkBasePresentationAttribute), true)
                .FirstOrDefault();

            if (attribute is SnkBasePresentationAttribute basePresentationAttribute)
            {
                if (basePresentationAttribute.ViewType == null)
                    basePresentationAttribute.ViewType = viewType;

                if (basePresentationAttribute.ViewModelType == null)
                    basePresentationAttribute.ViewModelType = request.ViewModelType;

                return basePresentationAttribute;
            }

            return CreatePresentationAttribute(request.ViewModelType, viewType);
        }

        
    }
}