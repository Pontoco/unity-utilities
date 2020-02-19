using System;
using System.Collections.Generic;
using System.Linq;
using Conditions;
using UnityEngine;

namespace Global.Utilities.Unity
{
    /// <summary>
    ///     A convenience class for the common pattern of keeping track of which colliders/gameobjects are currently
    ///     colliding with an object. To use, call the two <see cref="StartCollision" /> and <see cref="EndCollision" />
    ///     methods in a MonoBehavior script.
    /// </summary>
    public class CollisionTracker
    {
        private Func<Collider, bool> filter;

        private readonly HashSet<Collider> currentColliding = new HashSet<Collider>();

        /// <param name="ignoreTriggers">When true, collisions with triggers are ignored.</param>
        public CollisionTracker(bool ignoreTriggers = false, Func<Collider, bool> filter = null)
        {
            if (ignoreTriggers && filter != null)
            {
                throw new Exception("ignoreTriggers overrides filter. Only one should be set.");
            }

            if (ignoreTriggers)
            {
                this.filter = c => !c.isTrigger;
            }
            else
            {
                this.filter = filter;
            }
        }

        /// <summary>Call this when a collision starts.</summary>
        public void StartCollision(Collider collider)
        {
            Condition.Requires(currentColliding.Contains(collider)).IsFalse();

            currentColliding.Add(collider);
        }

        /// <summary>Call this when a collision ends.</summary>
        /// <param name="collider"></param>
        public void EndCollision(Collider collider)
        {
            // We don't check for trigger here, because if the collider became a trigger during the collision,
            // we still want to remove it.
            currentColliding.Remove(collider);
        }

        /// <summary>Returns whether this collider is present in the "currentColliding" set, regardless of the collider's state.</summary>
        public bool Contains(Collider coll)
        {
            return currentColliding.Contains(coll);
        }

        /// <summary>Whether there is a tracked collision currently happening with the given gameobject.</summary>
        public bool IsColliding(GameObject gameObj)
        {
            return GetColliding().Any(c => c.gameObject == gameObj);
        }

        /// <returns>An iterator of the currently colliding gameobjects.</returns>
        public IEnumerable<GameObject> GetCollidingObjects()
        {
            return GetColliding().Select(c => c.gameObject).Distinct();
        }

        /// <summary>An iterator of the currently colliding Colliders.</summary>
        public IEnumerable<Collider> GetColliding()
        {
            // Remove destroyed objects (they don't ever trigger a TriggerExit).
            currentColliding.RemoveWhere(c => c == null);

            // Filter out any disabled colliders (they must have been disabled while inside the collision tracker)
            return currentColliding.Where(c => c.enabled && (filter == null || filter(c)));
        }
    }
}
