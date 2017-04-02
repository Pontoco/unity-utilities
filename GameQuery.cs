using System;
using UnityEngine;

namespace Utilities {
    public static class GameQuery {

        /// <summary>
        /// Finds a <see cref="GameObject"/> in the scene by the matching
        /// <paramref name="name"/> or throws an exception if not found.
        /// </summary>
        /// <param name="name">Name of <see cref="GameObject"/></param>
        /// <param name="find_non_active">
        /// Whether to search for non-active objects as well. Note this may be
        /// slower.
        /// </param>
        public static GameObject FindOrThrow(string name, bool find_non_active = false) {
            if (find_non_active) {
                // To find a non active object we have to use a separate, but most likely slower process:
                foreach (GameObject g in Resources.FindObjectsOfTypeAll(typeof (GameObject))) {
                    if (g.name == name) {
                        return g;
                    }
                }

                throw new ArgumentException("No game object with name: " + name);
            } else {
                var obj = GameObject.Find(name);
                if (obj == null) {
                    throw new ArgumentException("No game object with name: " + name);
                }
                return obj;
            }
        }
    }
}
