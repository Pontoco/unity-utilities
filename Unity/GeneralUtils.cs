using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Unity
{
    /// <summary>
    /// Contains a smattering of general utilties for Unity projects.
    /// </summary>
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

        /// <summary>Returns the children of this transform as an iterable.</summary>
        public static IEnumerable<Transform> GetChildren(this Transform root)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                yield return root.GetChild(i);
            }
        }
    }
}
