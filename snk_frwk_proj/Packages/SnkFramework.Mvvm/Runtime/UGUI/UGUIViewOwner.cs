using SnkFramework.Mvvm.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SnkFramework.Mvvm.Runtime
{
    namespace UGUI
    {
        [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
        public class UGUIViewOwner : UIBehaviour, IUGUIViewOwner
        {
            private ISnkMvvmSettings _setting;
            protected ISnkMvvmSettings mSetting => _setting ??= SnkIoCProvider.Instance.Resolve<ISnkMvvmSettings>();
                
              //  if (IoC.Resolve<ISnkMvvmSettings>().mUseBlocksRaycastsInsteadOfInteractable)

                
            private Canvas _canvas;
            public Canvas mCanvas => this._canvas ??= this.GetComponent<Canvas>();

            private CanvasGroup _canvasGroup;
            public CanvasGroup mCanvasGroup => this._canvasGroup ??= this.GetComponent<CanvasGroup>();

            public virtual void Dispose()
            {
                this.onDisposeBegin();
                if (!this.IsDestroyed() && this.gameObject != null)
                    GameObject.Destroy(this.gameObject);
                this.onDisposeEnd();
            }

            protected virtual void onDisposeBegin()
            {

            }

            protected virtual void onDisposeEnd()
            {

            }

            public bool mInteractable
            {
                get
                {
                    if (this.gameObject == null)
                        return false;
                    if (mSetting.mUseBlocksRaycastsInsteadOfInteractable)
                        return this.mCanvasGroup.blocksRaycasts;
                    return this.mCanvasGroup.interactable;
                }
                set
                {
                    if (this.gameObject == null)
                        return;
                    if (mSetting.mUseBlocksRaycastsInsteadOfInteractable)
                        this.mCanvasGroup.blocksRaycasts = value;
                    else
                        this.mCanvasGroup.interactable = value;
                }
            }
        }
    }
}