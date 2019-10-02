using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Unity.Layout
{
    /// <summary>
    ///     This is a custom <see cref="VerticalLayoutGroup" /> that is not an <see cref="ILayoutElement" />. See
    ///     <see cref="LayoutGroupPlus" /> for more information.
    ///     <para>Layout child layout elements vertically.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class VerticalLayoutGroupPlus : HorizontalOrVerticalLayoutGroupPlus
    {
        /// <summary>
        ///     <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, true);
        }

        /// <summary>
        ///     <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, true);
        }
    }
}
