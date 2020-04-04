using System.Collections.Generic;
using System.Linq;
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

        /// <summary>Returns the set of active colliders on this gameobject and children.</summary>
        public static IEnumerable<Collider> GetActiveCollidersInChildren(this GameObject gameObject)
        {
            return gameObject.GetComponentsInChildren<Collider>().Where(c => c.enabled);
        }

        /// <summary>Returns the set of active colliders on this gameobject and children.</summary>
        public static IEnumerable<Collider> GetActiveCollidersInChildren(this Component component)
        {
            return component.GetComponentsInChildren<Collider>().Where(c => c.enabled);
        }

        /// <summary>Returns the set of active colliders on this gameobject.</summary>
        public static IEnumerable<Collider> GetActiveColliders(this GameObject gameObject)
        {
            return gameObject.GetComponents<Collider>().Where(c => c.enabled);
        }

        /// <summary>Returns the set of active colliders on this gameobject.</summary>
        public static IEnumerable<Collider> GetActiveColliders(this Component component)
        {
            return component.GetComponents<Collider>().Where(c => c.enabled);
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
        /// <param name="position">The position of the new gameobject in the given space.</param>
        /// <param name="rotation">The rotation of the new gameobject in the given space.</param>
        /// <param name="coordinateSpace">What coordinate space the previous position and rotation are in.</param>
        /// <param name="parent">The parent to create this object as a child under.</param>
        /// <returns>A new gameobject that is not active.</returns>
        public static GameObject InstantiateDisabled(GameObject prefab, Vector3 position = new Vector3(),
                                                     Quaternion rotation = new Quaternion(),
                                                     Space coordinateSpace = Space.Self,
                                                     Transform parent = null)
        {
            // Make sure the input is a prefab not an in scene game object.
            Condition.Requires(prefab.scene.name)
                     .IsNull("The input to InstantiateDisabled must be a prefab, not an in scene GameObject!");

            bool prevActive = prefab.activeSelf;
            prefab.SetActive(false);
            GameObject obj = Object.Instantiate(prefab, parent);
            if (coordinateSpace == Space.World)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            else
            {
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation;
            }

            prefab.SetActive(prevActive);
            return obj;
        }

        /// <summary>
        ///     Prints out a summary of the number of each <see cref="System.Type" /> of <see cref="UnityEngine.Object" />
        ///     that currently exists in memory. Prints the count of each type on a log line.
        /// </summary>
        /// <remarks>Printing this much stuff is slow. Use for memory debugging.</remarks>
        public static void PrintUnityObjectsAllocated()
        {
            Object[] objcts = Object.FindObjectsOfType<Object>();
            Debug.Log("================================================");
            Debug.Log("================================================");
            Debug.Log("Total objects: " + objcts.Length);
            objcts.GroupBy(x => x.GetType().FullName).OrderByDescending(g => g.Count())
                  .Select(g => $"{g.Key}: {g.Count()}").PrintOnLines();
        }
    }
}
