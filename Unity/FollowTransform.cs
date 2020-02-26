using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    ///     A component that causes the object to lerp towards a specific target Transform. The speed of the lerp and a
    ///     relative position to the target can be set.
    /// </summary>
    public class FollowTransform : MonoBehaviour
    {
        [Tooltip("The target to follow.")]
        public Transform Target;

        [Tooltip("Do the follow update in the FixedUpdate.")]
        public bool UseFixedUpdate = true;

        [Tooltip("Whether to follow target rotation as well.")]
        public bool FollowRotation;

        [Tooltip("Whether to ease towards the target transform.")]
        public bool UseEasing;

        [ShowIf("UseEasing")]
        [Tooltip("The speed at which this object changes it's position.")]
        public float PositionLerpSpeed = .05f;

        [Tooltip("The relative location from the target that this object should be placed.  In world space.")]
        public Vector3 RestPosition;

        private void FixedUpdate()
        {
            if (!UseFixedUpdate)
            {
                return;
            }

            UpdateTransform();
        }

        private void Update()
        {
            if (UseFixedUpdate)
            {
                return;
            }

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (Target != null)
            {
                float speed = UseEasing ? Time.deltaTime * PositionLerpSpeed : 1;

                Vector3 posTo = Target.position + Target.forward * RestPosition.z +
                                Target.right * RestPosition.x + Target.up * RestPosition.y;
                transform.position = Vector3.LerpUnclamped(transform.position, posTo, speed);

                if (FollowRotation)
                {
                    transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Target.rotation, speed);
                }
            }
        }
    }
}
