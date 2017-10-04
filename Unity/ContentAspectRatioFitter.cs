using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

namespace Utilities.Unity
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ContentAspectRatioFitter : AspectRatioFitter
    {
        protected override void OnRectTransformDimensionsChange()
        {
            var width = LayoutUtility.GetPreferredSize(GetComponent<RectTransform>(), 0);
            var height = LayoutUtility.GetPreferredSize(GetComponent<RectTransform>(), 1);

            var ratio = width / height;

            if (!float.IsNaN(ratio))
            {
                aspectRatio = Mathf.Clamp(ratio, 1f / 1000f, 1000f);
            }

            base.OnRectTransformDimensionsChange();
        }
    }
}
