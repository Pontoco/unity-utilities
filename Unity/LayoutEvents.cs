using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities.Unity
{
    /// <summary>
    ///     Convenient event methods for Unity layout events. This lets us only run code if the layout
    ///     has started, etc. Add this to an object to register events for that objects layout.
    /// </summary>
    [ExecuteInEditMode]
    public class LayoutEvents : UIBehaviour
    {
        /// A fake canvas element that sits in the registry and listens for events.
        private class LayoutEventReceiver : ICanvasElement
        {
            public void Rebuild(CanvasUpdate executing)
            {
                HasFirstLayoutStarted = true;
                Debug.Log("Rebuild");
                LayoutEvents.Rebuild.OnNext(executing);
            }

            public void LayoutComplete()
            {
                LayoutEvents.LayoutComplete.OnNext(Unit.Default);
            }

            public void GraphicUpdateComplete()
            {
                LayoutEvents.GraphicUpdateComplete.OnNext(Unit.Default);
            }

            public bool IsDestroyed()
            {
                // Don't destroy this object.
                return false;
            }

            public Transform transform { get; set; }
        }

        public static readonly Subject<Unit> LayoutComplete = new Subject<Unit>();
        public static readonly Subject<Unit> GraphicUpdateComplete = new Subject<Unit>();
        public static readonly Subject<CanvasUpdate> Rebuild = new Subject<CanvasUpdate>();

        public static bool HasFirstLayoutStarted { get; private set; }

        public readonly Subject<Unit> RectTransformDimensionsChange = new Subject<Unit>();

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeStaticEvents()
        {
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(new LayoutEventReceiver());
        }

        /// <inheritdoc />
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            RectTransformDimensionsChange.OnNext(Unit.Default);
        }
    }
}
