using System.Collections.Generic;
using Conditions;
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
                Transform child = parent.GetChild(i);
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

        /// <summary>
        ///     Instantiates the given prefab as disabled, so that we can modify it before Start/Awake get run. Only works on
        ///     prefabs. Should not be used to clone a game object.
        ///     <para>The gameobject returned should have SetActive(true) called on it to finish setup.</para>
        /// </summary>
        /// <param name="prefab">A prefab asset.</param>
        /// <param name="worldPosition">The position of the new gameobject in world space.</param>
        /// <param name="worldRotation">The rotation of the new gameobject in world space.</param>
        /// <returns>A new gameobject that is not active.</returns>
        public static GameObject InstantiateDisabled(GameObject prefab, Vector3 worldPosition = new Vector3(),
                                                     Quaternion worldRotation = new Quaternion())
        {
            // Make sure the input is a prefab not an in scene game object.
            Condition.Requires(prefab.scene.name)
                     .IsNull("The input to InstantiateDisabled must be a prefab, not an in scene GameObject!");

            bool prevActive = prefab.activeSelf;
            prefab.SetActive(false);
            GameObject obj = Object.Instantiate(prefab, worldPosition, worldRotation);
            prefab.SetActive(prevActive);
            return obj;
        }
    }
}
