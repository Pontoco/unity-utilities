using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Unity
{
    /// <summary>Contains a smattering of general utilties for Unity projects.</summary>
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

        /// <summary>Returns the first child with the given name.</summary>
        public static Transform FirstChildOrDefault(this Transform parent, string name)
        {
            if (parent.childCount == 0)
            {
                return null;
            }

            Transform result = null;
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }

                result = FirstChildOrDefault(child, name);
            }

            return result;
        }

        /// <summary>Returns the full path to this transform including parents and the name of this gameobject.</summary>
        /// <returns>A string in the form "rootobject/someobject/otherobject/thisobject"</returns>
        public static string GetPath(this Transform transform)
        {
            if (transform.parent == null)
            {
                return "/" + transform.name;
            }

            return transform.parent.GetPath() + "/" + transform.name;
        }
    }
}
