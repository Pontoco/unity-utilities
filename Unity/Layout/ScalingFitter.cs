using System;
using System.Collections.Generic;
using Optional;
using Optional.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities.Unity.Layout
{
    /// <summary>
    ///     <para>
    ///         Changes the localscale property on the rectTransform to fit this element within the
    ///         parent. This is in contrast to <see cref="AspectRatioFitter" /> which changes the actual
    ///         size of the fitted element. Partially decompiled from <see cref="AspectRatioFitter"/>
    ///     </para>
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ScalingFitter : UIBehaviour, ILayoutSelfController
    {
        /// <summary>
        ///     <para>Specifies a mode to use to enforce an aspect ratio.</para>
        /// </summary>
        public enum Mode
        {
            FitInParent,
            EnvelopeParent
        }

        /// <summary>
        ///     <para>The mode to use to enforce the aspect ratio.</para>
        /// </summary>
        public Mode AspectMode
        {
            get { return aspectMode; }
            set
            {
                if (!SetStruct(ref aspectMode, value))
                {
                    return;
                }
                SetDirty();
            }
        }

        private RectTransform rectTransform
        {
            get { return rect ?? (rect = GetComponent<RectTransform>()); }
        }

        [SerializeField]
        private Mode aspectMode = Mode.FitInParent;

        [NonSerialized]
        private RectTransform rect;

        private Vector3 currentScale = Vector3.one;
        private DrivenRectTransformTracker tracker;

        protected ScalingFitter()
        {
        }

        /// <summary>
        ///     <para>Method called by the layout system.</para>
        /// </summary>
        public void SetLayoutHorizontal()
        {
            UpdateRect();
        }

        /// <summary>
        ///     <para>Method called by the layout system.</para>
        /// </summary>
        public void SetLayoutVertical()
        {
            UpdateRect();
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
            tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        /// <summary>
        ///   <para>See MonoBehaviour.OnBeforeTransformParentChanged.</para>
        /// </summary>
        protected override void OnBeforeTransformParentChanged()
        {
            base.OnBeforeTransformParentChanged();
            UpdateRect();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateRect();
        }

        private void UpdateRect()
        {
            if (!IsActive())
            {
                return;
            }
            
            tracker.Clear(false);
            tracker.Add(this, rectTransform,
                DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition);
            tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
            rectTransform.anchorMin = new Vector2(.5f, .5f);
            rectTransform.anchorMax = new Vector2(.5f, .5f);
            rectTransform.anchoredPosition = Vector2.zero;

            if (rectTransform.sizeDelta.x > 0 && rectTransform.sizeDelta.y > 0)
            {
                Option<Vector2> parentSizeOpt = GetParentSize();
                if (parentSizeOpt.HasValue)
                {
                    var parentSize = parentSizeOpt.ValueOrFailure();

                    // Parent sizes will be zero if this runs at odd times
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (parentSize.x != 0.0 && parentSize.y != 0.0)
                    {
                        {
                            float scaleX = parentSize.x / rectTransform.sizeDelta.x;
                            float scaleY = parentSize.y / rectTransform.sizeDelta.y;

                            float s;
                            switch (aspectMode)
                            {
                                case Mode.FitInParent:
                                    s = Math.Min(scaleX, scaleY);
                                    break;
                                case Mode.EnvelopeParent:
                                    s = Math.Max(scaleX, scaleY);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            currentScale = new Vector3(s, s, 1);
                        }
                    }
                }
            }

            rectTransform.localScale = currentScale;
        }

        private Option<Vector2> GetParentSize()
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (!parent)
            {
                Debug.LogWarning("A ScalingFitter component exists on an object with no parent!");
                return Option.None<Vector2>();
            }
            return parent.rect.size.Some();
        }

        /// <summary>
        ///     <para>Mark the AspectRatioFitter as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            UpdateRect();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif

        private static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                return false;
            }
            currentValue = newValue;
            return true;
        }
    }
}
