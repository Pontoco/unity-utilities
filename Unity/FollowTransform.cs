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

        [Tooltip("The relative location from the target that this object should be placed.  In world space.")]
        public Vector3 RestPosition;

        [Tooltip("The speed at which this object changes it's position.")]
        public float PositionLerpSpeed = .05f;

        private void Update()
        {
            if (Target != null)
            {
                float posSpeed = Time.deltaTime * PositionLerpSpeed;
                Vector3 posTo = Target.position + Target.forward * RestPosition.z +
                                Target.right * RestPosition.x + Target.up * RestPosition.y;
                transform.position = Vector3.LerpUnclamped(transform.position, posTo, posSpeed);
            }
        }
    }
}
