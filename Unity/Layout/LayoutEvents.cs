using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities.Unity.Layout
{
    /// <summary>
    /// Provides hooks on a MonoBehavior to listen for its RectTransform updates.
    /// </summary>
    [ExecuteInEditMode]
    public class LayoutEvents : UIBehaviour
    {
        public readonly Subject<Unit> RectTransformDimensionsChange = new Subject<Unit>();

        /// <inheritdoc />
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            RectTransformDimensionsChange.OnNext(Unit.Default);
        }

        /// <summary>
        /// <see cref="LayoutRebuilder.MarkLayoutForRebuild"/> only recurses into children that have ILayoutController
        /// components. However, they didn't consider that you might have a child further down with an ILayoutController,
        /// even if the direct descendent of the marked layout doesn't. (there's a non-layout-controller in between).
        /// This properly handles this case. 
        /// </summary>
        public static void MarkLayoutForRebuildRecursive(RectTransform root)
        {
            // Queue lets us traverse in breadth-first manner
            Queue<RectTransform> childrenToVisit = new Queue<RectTransform>();
            childrenToVisit.Enqueue(root);

            while (childrenToVisit.Count > 0)
            {
                var rect = childrenToVisit.Dequeue();
                foreach (var child in rect.GetChildren())
                {
                    childrenToVisit.Enqueue(child);
                }

                // If the direct parent has an ILayoutController, then this controller will already be updated from that one.
                if (rect.parent.HasComponent<ILayoutController>() && rect != root)
                {
                    continue;
                }

                if (rect.HasComponent<ILayoutController>())
                {
                    LayoutRebuilder.MarkLayoutForRebuild(rect);
                }
            }
        }
    }
}
