using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>Follows the camera object's position, but only in the XZ plane. Has a fixed vertical offset for y.</summary>
    public class FollowCameraXZ : MonoBehaviour
    {
        public float VerticalOffset;
        public float ForwardOffset;

        private void Update()
        {
            Vector3 followPos = Camera.main.transform.position + Camera.main.transform.forward * ForwardOffset;

            Vector3 pos = transform.position;
            pos.x = followPos.x;
            pos.y = Camera.main.transform.position.y + VerticalOffset;

            pos.z = followPos.z;
            transform.position = pos;
        }
    }
}
