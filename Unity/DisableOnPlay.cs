using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    /// Disables the game object on awake.
    /// </summary>
    public class DisableOnPlay : MonoBehaviour {
        private void Awake () {
            gameObject.SetActive(false);
        }
    }
}
