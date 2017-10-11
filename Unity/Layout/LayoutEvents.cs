using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities.Unity.Layout
{
    /// <summary>
    ///     Convenient event methods for Unity layout events.
    ///     <p />This includes static events such as "has the layout pass started", etc.
    ///     <p />This is also a component that can be added to a gameobject to register events for that
    ///     object's layout.
    /// </summary>
    [ExecuteInEditMode]
    public class LayoutEvents : UIBehaviour
    {
        /// A fake canvas element that sits in the registry and listens for events.
        /// TODO: Bug. This class needs to re-register itself for layout events in LayoutComplete.
        /// TODO: This class should be removed, as there is no single layout event, rather every frame, Unity
        /// updates layouts for any elements that marked themselves for update.
        private class LayoutEventReceiver : ICanvasElement
        {
            public void Rebuild(CanvasUpdate executing)
            {
                HasFirstLayoutStarted = true;
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
