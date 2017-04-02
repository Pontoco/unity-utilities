using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintLogToScreen : MonoBehaviour {
  private string log;
  private readonly Queue my_log_queue = new Queue();

  void Start() {
    Debug.Log("Log now printing to screen...");
  }

  void Update() {
  }

  void OnEnable() {
    Application.logMessageReceived += HandleLog;
  }

  void OnDisable() {
    Application.logMessageReceived -= HandleLog;
  }

  void HandleLog(string log_string, string stack_trace, LogType type) {

    string line = "[" + type + "] : " + log + "\n";
    log += line;

    if (type == LogType.Exception) {
      line = stack_trace + "\n";
      log += line;
    }

  }

  void OnGUI() {
    GUILayout.Label(log);
  }

}
