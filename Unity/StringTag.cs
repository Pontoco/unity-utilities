using UnityEngine;

namespace Global.Utilities.Unity
{
    /// <summary>
    ///     Represents a simple string-based tag that can be set in the inspector. This has a couple of benefits over
    ///     Unity tags, or Component-typed tags, because it can be changed and queried dynamically (at runtime).
    ///     <para />
    ///     Additionally, Unity tags must be added to the global tag list, which is not ideal for tests. For tests, it can be
    ///     convenient to label a couple of objects in the test with string tags. Here performance isn't too much of a concern,
    ///     we just want to grab a couple of objects out of the scene.
    /// </summary>
    public class StringTag : MonoBehaviour
    {
        /// <summary>A label for this gameobject.</summary>
        [Tooltip("A label for this gameobject.")]
        public string Tag;
    }
}
