using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    ///     Causes an object to maintain a given relative position to the camera object. Movement has
    ///     a spring to it, isn't instant.
    /// </summary>
    public class SpringFollowCamera : MonoBehaviour
    {
        // The distance from the camera that this object should be placed
        // in meters.
        public float RestX;
        public float RestY;
        public float RestZ;

        // The speed at which this object changes it's position.
        public float PositionLerpSpeed = 5f;

        // The speed at which this object changes it's rotation.
        public float RotationLerpSpeed = 5f;

        // The camera this object will be in front of.
        public Camera Camera;

        /// <summary>Initializes variables and verifies that necesary components exist.</summary>
        private void Awake()
        {
            if (Camera == null)
            {
                enabled = false;
                Debug.LogError("Failed to find the Camera component.");
            }
        }

        /// <summary>
        ///     Update position and rotation of this canvas object to face the camera using lerp for
        ///     smoothness.
        /// </summary>
        private void Update()
        {
            // Move the object in front of the camera.
            float posSpeed = Time.deltaTime * PositionLerpSpeed;
            Vector3 posTo = Camera.transform.position + Camera.transform.forward * RestZ +
                            Camera.transform.right * RestX + Camera.transform.up * RestY;
            transform.position = Vector3.Slerp(transform.position, posTo, posSpeed);

            // Rotate the object to face the camera.
            float rotSpeed = Time.deltaTime * RotationLerpSpeed;
            Quaternion rotTo = Quaternion.LookRotation(transform.position - Camera.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotTo, rotSpeed);
        }
    }
}
