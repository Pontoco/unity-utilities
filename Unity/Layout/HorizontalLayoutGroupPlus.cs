using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Unity.Layout
{
    /// <summary>
    ///     This is a custom <see cref="HorizontalLayoutGroup" /> that is not an
    ///     <see cref="ILayoutElement" />. See <see cref="LayoutGroupPlus" /> for more information.
    ///     <para>Layout child layout elements horizontally.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalLayoutGroupPlus : HorizontalOrVerticalLayoutGroupPlus
    {
        /// <summary>
        ///     <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, false);
        }

        /// <summary>
        ///     <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, false);
        }
    }
}
