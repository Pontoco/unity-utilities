using System;
using Conditions;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    ///     This is a utility that allows you to store constants for a script as an Asset in Unity.
    ///     This lets you do a few things: - Access script constants under a static, unique path - Update
    ///     constants while the game is playing and have the changes keep. - No manual connection of
    ///     configuration assets to prefabs/scripts. See <see cref="FlightPaths" /> for an example of how
    ///     to use it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ScriptParams<T> : ScriptableObject where T : ScriptableObject
    {
        private static T cachedInstance;

        /// <summary>
        ///     Retrieves the singleton asset for this ScriptParams type. If none or more than one asset
        ///     exists for the type, an error is thrown.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (cachedInstance == null)
                {
                    // Generally just load all resources.
                    Resources.LoadAll<T>("ScriptParams");

                    var paramsObjects = Resources.FindObjectsOfTypeAll<T>();
                    paramsObjects.Length.Requires()
                                 .IsNotEqualTo(0,
                                     "Couldn't find an asset of type " + typeof(T).FullName +
                                     " in any Resources directory. Did you create one?");
                    paramsObjects.Length.Requires()
                                 .IsEqualTo(1,
                                     "Found more than one asset of type " + typeof(T).FullName +
                                     " in Resources directories. There should only be one.");

                    cachedInstance = paramsObjects[0];
                }

                return cachedInstance;
            }
        }
    }
}
