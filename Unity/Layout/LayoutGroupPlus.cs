using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Utilities.Unity.Layout
{
    /// <summary>
    ///     This is a decompiled version of <see cref="LayoutGroup" />. It contains minor
    ///     modifications, particularly that this LayoutGroup is not an <see cref="ILayoutElement" />,
    ///     meaning that it does not set its own sizes from its children. This is beneficial when nesting
    ///     lots of LayoutGroups, to keep the one-way dependence on sizes (parent to child) from creating a
    ///     reverse dependency.
    ///     <para>Abstract base class to use for layout groups.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public abstract class LayoutGroupPlus : UIBehaviour, ILayoutGroup 
    {
        /// <summary>
        ///     <para>The padding to add around the child layout elements.</para>
        /// </summary>
        public RectOffset padding
        {
            get { return m_Padding; }
            set { SetProperty(ref m_Padding, value); }
        }

        /// <summary>
        ///     <para>The alignment to use for the child layout elements in the layout group.</para>
        /// </summary>
        public TextAnchor childAlignment
        {
            get { return m_ChildAlignment; }
            set { SetProperty(ref m_ChildAlignment, value); }
        }

        public bool UpdateDisabledIfRootLayoutGroup = false;

        protected RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                {
                    m_Rect = GetComponent<RectTransform>();
                }
                return m_Rect;
            }
        }

        protected List<RectTransform> rectChildren
        {
            get { return m_RectChildren; }
        }

        private bool isRootLayoutGroup
        {
            get
            {
                if (transform.parent == null)
                {
                    return true;
                }
                return transform.parent.GetComponent(typeof(ILayoutGroup)) == null;
            }
        }

        [SerializeField]
        protected RectOffset m_Padding = new RectOffset();

        [FormerlySerializedAs("m_Alignment")]
        [SerializeField]
        protected TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;

        protected DrivenRectTransformTracker m_Tracker;

        [NonSerialized]
        private readonly List<RectTransform> m_RectChildren = new List<RectTransform>();

        [NonSerialized]
        private RectTransform m_Rect;

        protected LayoutGroupPlus()
        {
            if (m_Padding != null)
            {
                return;
            }
            m_Padding = new RectOffset();
        }

        /// <summary>
        ///     <para>Called by the layout system to set the children's positions.</para>
        /// </summary>
        public abstract void SetLayoutHorizontal();

        /// <summary>
        ///     <para>Called by the layout system to set the children's positions.</para>
        /// </summary>
        public abstract void SetLayoutVertical();

        /// <summary>
        ///     <para>Called by the layout system.</para>
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        {
            m_RectChildren.Clear();
            List<Component> componentList = new List<Component>(); // TODO: Convert to pooling.

            for (int index1 = 0; index1 < rectTransform.childCount; ++index1)
            {
                RectTransform child = rectTransform.GetChild(index1) as RectTransform;
                if (!(child == null) && child.gameObject.activeInHierarchy)
                {
                    child.GetComponents(typeof(ILayoutIgnorer), componentList);
                    if (componentList.Count == 0)
                    {
                        m_RectChildren.Add(child);
                    }
                    else
                    {
                        for (int index2 = 0; index2 < componentList.Count; ++index2)
                        {
                            if (!((ILayoutIgnorer) componentList[index2]).ignoreLayout)
                            {
                                m_RectChildren.Add(child);
                                break;
                            }
                        }
                    }
                }
            }
            m_Tracker.Clear();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        /// <summary>
        ///     <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        /// <summary>
        ///     <para>Callback for when properties have been changed by animation.</para>
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        /// <summary>
        ///     <para>Returns the calculated position of the first child layout element along the given axis.</para>
        /// </summary>
        /// <param name="axis">The axis index. 0 is horizontal and 1 is vertical.</param>
        /// <param name="requiredSpaceWithoutPadding">
        ///     The total space required on the given axis for all the
        ///     layout elements including spacing and excluding padding.
        /// </param>
        /// <returns>
        ///     <para>The position of the first child along the given axis.</para>
        /// </returns>
        protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float num1 = requiredSpaceWithoutPadding + (axis != 0 ? padding.vertical : padding.horizontal);
            float num2 = rectTransform.rect.size[axis] - num1;
            float alignmentOnAxis = GetAlignmentOnAxis(axis);
            return (axis != 0 ? padding.top : padding.left) + num2 * alignmentOnAxis;
        }

        /// <summary>
        ///     <para>
        ///         Returns the alignment on the specified axis as a fraction where 0 is lefttop, 0.5 is
        ///         middle, and 1 is rightbottom.
        ///     </para>
        /// </summary>
        /// <param name="axis">The axis to get alignment along. 0 is horizontal and 1 is vertical.</param>
        /// <returns>
        ///     <para>The alignment as a fraction where 0 is lefttop, 0.5 is middle, and 1 is rightbottom.</para>
        /// </returns>
        protected float GetAlignmentOnAxis(int axis)
        {
            if (axis == 0)
            {
                return (int) childAlignment % 3 * 0.5f;
            }
            return (int) childAlignment / 3 * 0.5f;
        }

        protected void SetChildAlongAxis(RectTransform rect, int axis, float pos)
        {
            if (rect == null)
            {
                return;
            }
            m_Tracker.Add(this, rect, (DrivenTransformProperties) (3840 | (axis != 0 ? 4 : 2)));
            rect.SetInsetAndSizeFromParentEdge(axis != 0 ? RectTransform.Edge.Top : RectTransform.Edge.Left, pos,
                rect.sizeDelta[axis]);
        }

        /// <summary>
        ///     <para>Set the position and size of a child layout element along the given axis.</para>
        /// </summary>
        /// <param name="rect">The RectTransform of the child layout element.</param>
        /// <param name="axis">The axis to set the position and size along. 0 is horizontal and 1 is vertical.</param>
        /// <param name="pos">The position from the left side or top.</param>
        /// <param name="size">The size.</param>
        protected void SetChildAlongAxis(RectTransform rect, int axis, float pos, float size)
        {
            if (rect == null)
            {
                return;
            }
            m_Tracker.Add(this, rect, (DrivenTransformProperties) (3840 | (axis != 0 ? 8196 : 4098)));
            rect.SetInsetAndSizeFromParentEdge(axis != 0 ? RectTransform.Edge.Top : RectTransform.Edge.Left, pos, size);
        }
        
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            Debug.Log("transform parent changed!");
            if (!isRootLayoutGroup && UpdateDisabledIfRootLayoutGroup)
            {
                return;
            }
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!isRootLayoutGroup && UpdateDisabledIfRootLayoutGroup)
            {
                return;
            }
            SetDirty();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            SetDirty();
        }

        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if (currentValue == null && newValue == null || currentValue != null && currentValue.Equals(newValue))
            {
                return;
            }
            currentValue = newValue;
            SetDirty();
        }

        /// <summary>
        ///     <para>Mark the LayoutGroup as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (!IsActive())
            {
                return;
            }
            if (!CanvasUpdateRegistry.IsRebuildingLayout())
            {
                LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            }
            else
            {
                StartCoroutine(DelayedSetDirty(rectTransform));
            }
        }

        [DebuggerHidden]
        private IEnumerator DelayedSetDirty(RectTransform rect)
        {
            LayoutRebuilder.MarkLayoutForRebuild(rect);
            yield return null;
        }

        protected override void OnValidate()
        {
            SetDirty();
        }
    }
}
