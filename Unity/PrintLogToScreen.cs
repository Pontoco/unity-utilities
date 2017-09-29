using System;
using UnityEngine;

namespace Utilities.Unity
{
    public class PrintLogToScreen : MonoBehaviour
    {
        private string log;

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            string line = "[" + type + "] : " + log + "\n";
            log += line;

            switch (type)
            {
                case LogType.Exception:
                    line = stackTrace + "\n";
                    log += line;
                    break;
                case LogType.Error:
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    break;
                case LogType.Log:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label(log);
        }
    }
}
