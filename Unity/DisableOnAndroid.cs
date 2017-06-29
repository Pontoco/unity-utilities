using UnityEngine;

public class DisableOnAndroid : MonoBehaviour {
  private void Start() {
    if (Application.platform == RuntimePlatform.Android) {
      gameObject.SetActive(false);
    }
  }
}
