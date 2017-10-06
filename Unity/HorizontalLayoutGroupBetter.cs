using UnityEngine;

namespace Utilities.Unity
{
    /// <summary>
    ///     <para>Layout child layout elements horizontally.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalLayoutGroupBetter : HorizontalOrVerticalLayoutGroupBetter
    {
        /// <summary>
        ///     <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();
            SetChildrenAlongAxis(0, false);
        }

        /// <summary>
        ///     <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();
            SetChildrenAlongAxis(1, false);
        }
    }
}
