using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ASG.Utilities.Unity.Testing
{
    /// <summary>
    /// This is an object that gets added to the scene of every test, so we can listen for log messages and do basic setup.
    /// </summary>
    public class TestHarness : MonoBehaviour
    {
        private static int harnessesEnabled = 0;
        private void OnEnable()
        {
            // Listens to all log messages in the console and reports which scenes any errors come from.
            if (harnessesEnabled == 0)
            {
                Application.logMessageReceived += OnApplicationOnLogMessageReceived;
            }
            harnessesEnabled++;
        }

        private void OnDisable()
        {
            harnessesEnabled--;
            if (harnessesEnabled == 0)
            {
                Application.logMessageReceived -= OnApplicationOnLogMessageReceived;
            }
        }

        private static void OnApplicationOnLogMessageReceived(string c, string t, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                Debug.LogWarning("Got error from test harness. Current scene: " + SceneManager.GetActiveScene().path);
            }
        }
    }
}
