using System.Collections;
using UnityEngine;

public class PrintLogToScreen : MonoBehaviour {
  private readonly Queue my_log_queue = new Queue();
  private string log;

  private void Start() {
    Debug.Log("Log now printing to screen...");
  }

  private void Update() {
  }

  private void OnEnable() {
    Application.logMessageReceived += HandleLog;
  }

  private void OnDisable() {
    Application.logMessageReceived -= HandleLog;
  }

  private void HandleLog(string log_string, string stack_trace, LogType type) {
    string line = "[" + type + "] : " + log + "\n";
    log += line;

    if (type == LogType.Exception) {
      line = stack_trace + "\n";
      log += line;
    }
  }

  private void OnGUI() {
    GUILayout.Label(log);
  }
}
