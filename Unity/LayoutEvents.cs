using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Unity
{
    /// <summary>
    ///     Convenient event methods for Unity layout events. This lets us only run code if the layout
    ///     has started, etc.
    /// </summary>
    public sealed class LayoutEvents
    {
        /// A fake canvas element that sits in the registry and listens for events.
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

        public static bool HasFirstLayoutStarted { get; private set; }

        public static readonly Subject<Unit> LayoutComplete = new Subject<Unit>();
        public static readonly Subject<Unit> GraphicUpdateComplete = new Subject<Unit>();
        public static readonly Subject<CanvasUpdate> Rebuild = new Subject<CanvasUpdate>();

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(new LayoutEventReceiver());
        }
    }
}
