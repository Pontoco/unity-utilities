using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Unity.Layout
{
    /// <summary>
    ///     This is a custom <see cref="HorizontalOrVerticalLayoutGroup" /> that is not an
    ///     <see cref="ILayoutElement" />. See <see cref="LayoutGroupPlus" /> for more information. It also
    ///     contains a few bugs fixes, here and there.
    ///     <para>Abstract base class for HorizontalLayoutGroupPlus and VerticalLayoutGroupPlus.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public abstract class HorizontalOrVerticalLayoutGroupPlus : LayoutGroupPlus
    {
        private struct Sizes
        {
            public float Min;
            public float Preferred;
            public float Flexible;
        }

        /// <summary>
        ///     <para>The spacing to use between layout elements in the layout group.</para>
        /// </summary>
        public float spacing
        {
            get { return m_Spacing; }
            set { SetProperty(ref m_Spacing, value); }
        }

        /// <summary>
        ///     <para>Whether to force the children to expand to fill additional available horizontal space.</para>
        /// </summary>
        public bool childForceExpandWidth
        {
            get { return m_ChildForceExpandWidth; }
            set { SetProperty(ref m_ChildForceExpandWidth, value); }
        }

        /// <summary>
        ///     <para>Whether to force the children to expand to fill additional available vertical space.</para>
        /// </summary>
        public bool childForceExpandHeight
        {
            get { return m_ChildForceExpandHeight; }
            set { SetProperty(ref m_ChildForceExpandHeight, value); }
        }

        /// <summary>
        ///     <para>
        ///         Returns true if the Layout Group controls the widths of its children. Returns false if
        ///         children control their own widths.
        ///     </para>
        /// </summary>
        public bool controlsChildWidths
        {
            get { return m_ChildControlWidth; }
            set { SetProperty(ref m_ChildControlWidth, value); }
        }

        /// <summary>
        ///     <para>
        ///         Returns true if the Layout Group controls the heights of its children. Returns false if
        ///         children control their own heights.
        ///     </para>
        /// </summary>
        public bool controlsChildHeights
        {
            get { return m_ChildControlHeight; }
            set { SetProperty(ref m_ChildControlHeight, value); }
        }

        [SerializeField]
        protected float m_Spacing;

        [SerializeField]
        protected bool m_ChildForceExpandWidth = true;

        [SerializeField]
        protected bool m_ChildForceExpandHeight = true;

        [SerializeField]
        protected bool m_ChildControlWidth = true;

        [SerializeField]
        protected bool m_ChildControlHeight = true;

        /// <summary>
        ///     <para>Set the positions and sizes of the child layout elements for the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
        /// <param name="isVertical">Is this group a vertical group?</param>
        protected void SetChildrenAlongAxis(int axis, bool isVertical)
        {
            float parentSize = rectTransform.rect.size[axis];
            Debug.Log(gameObject.name + " parent size: " + axis + " : " + parentSize);
            bool controlSize = axis != 0 ? m_ChildControlHeight : m_ChildControlWidth;
            bool childForceExpand = axis != 0 ? childForceExpandHeight : childForceExpandWidth;
            float alignmentOnAxis = GetAlignmentOnAxis(axis);
            if (isVertical ^ (axis == 1))
            {
                float internalParentSize = parentSize - (axis != 0 ? padding.vertical : padding.horizontal);
                for (int index = 0; index < rectChildren.Count; ++index)
                {
                    RectTransform rectChild = rectChildren[index];
                    float min;
                    float preferred;
                    float flexible;
                    GetChildSizes(rectChild, axis, controlSize, childForceExpand, out min, out preferred, out flexible);
                    float requiredSpaceWithoutPadding = Mathf.Clamp(internalParentSize, min,
                        (double) flexible <= 0.0 ? preferred : parentSize);
                    float startOffset =
                        GetStartOffset(axis, requiredSpaceWithoutPadding); // starting offset of elements inside parent
                    if (controlSize)
                    {
                        SetChildAlongAxis(rectChild, axis, startOffset, requiredSpaceWithoutPadding);
                    }
                    else
                    {
                        float position = (requiredSpaceWithoutPadding - rectChild.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxis(rectChild, axis, startOffset + position);
                    }
                }
            }
            else
            {
                var sizes = CalcAlongAxis(axis, isVertical);
                float pos = axis != 0 ? padding.top : padding.left;
                if (sizes.Flexible == 0.0 && sizes.Preferred < (double) parentSize)
                {
                    pos = GetStartOffset(axis,
                        sizes.Preferred - (axis != 0 ? padding.vertical : padding.horizontal));
                }
                float t = 0.0f;
                if (sizes.Min != (double) sizes.Preferred)
                {
                    t = Mathf.Clamp01((float) ((parentSize - (double) sizes.Min) /
                                               (sizes.Preferred - (double) sizes.Min)));
                }
                float flexibleExcess = 0.0f;
                if (parentSize > (double) sizes.Preferred && sizes.Flexible > 0.0)
                {
                    flexibleExcess = (parentSize - sizes.Preferred) / sizes.Flexible;
                }
                for (int index = 0; index < rectChildren.Count; ++index)
                {
                    RectTransform rectChild = rectChildren[index];
                    float min;
                    float preferred;
                    float flexible;
                    GetChildSizes(rectChild, axis, controlSize, childForceExpand, out min, out preferred, out flexible);
                    float size = Mathf.Lerp(min, preferred, t) + flexible * flexibleExcess;
                    if (controlSize)
                    {
                        SetChildAlongAxis(rectChild, axis, pos, size);
                    }
                    else
                    {
                        float num3 = (size - rectChild.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxis(rectChild, axis, pos + num3);
                    }
                    pos += size + spacing;
                }
            }
        }

        private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
                                   out float min, out float preferred, out float flexible)
        {
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0.0f;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }
            if (!childForceExpand)
            {
                return;
            }
            flexible = Mathf.Max(flexible, 1f);
        }

        /// <summary>
        ///     <para>Calculate the layout element properties for this layout element along the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis to calculate for. 0 is horizontal and 1 is vertical.</param>
        /// <param name="isVertical">Is this group a vertical group?</param>
        private Sizes CalcAlongAxis(int axis, bool isVertical)
        {
            float parentPadding = axis != 0 ? padding.vertical : padding.horizontal;
            bool controlSize = axis != 0 ? m_ChildControlHeight : m_ChildControlWidth;
            bool childForceExpand = axis != 0 ? childForceExpandHeight : childForceExpandWidth;
            float totalMin = parentPadding;
            float b = parentPadding;
            float totalFlexible = 0.0f;
            bool flag = isVertical ^ (axis == 1);
            foreach (RectTransform t in rectChildren)
            {
                float min;
                float preferred;
                float flexible;
                GetChildSizes(t, axis, controlSize, childForceExpand, out min,
                    out preferred, out flexible);
                if (flag)
                {
                    totalMin = Mathf.Max(min + parentPadding, totalMin);
                    b = Mathf.Max(preferred + parentPadding, b);
                    totalFlexible = Mathf.Max(flexible, totalFlexible);
                }
                else
                {
                    totalMin += min + spacing;
                    b += preferred + spacing;
                    totalFlexible += flexible;
                }
            }
            if (!flag && rectChildren.Count > 0)
            {
                totalMin -= spacing;
                b -= spacing;
            }
            float totalPreferred = Mathf.Max(totalMin, b);

            return new Sizes {Min = totalMin, Preferred = totalPreferred, Flexible = totalFlexible};
        }

        protected override void Reset()
        {
            base.Reset();
            m_ChildControlWidth = false;
            m_ChildControlHeight = false;
        }
    }
}
