using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Unity
{
    public static class GeneralUtils
    {
        /// <summary>Returns the children of this rect transform as an iterable.</summary>
        public static IEnumerable<RectTransform> GetChildren(this RectTransform root)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                yield return root.GetChild(i) as RectTransform;
            }
        }
    }
}
