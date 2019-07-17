using UnityEngine;
using UnityEngine.TestTools;

namespace ASG.Utilities.Unity.Testing
{
    public class UnityTestWithSetup : IPrebuildSetup
    {
        /// <inheritdoc />
        public virtual void Setup()
        {
            // Test Setup runs after the test scene is loaded, but before hitting 'play'
        }

        /// <summary>
        /// Adds the Test Harness prefab that contains the game harness and some testing scripts.
        /// </summary>
        public void SetupTestHarness() {
            // Add our canary that sits around during tests.
            Debug.Log("Setup test harness to catch logged errors.");

            Object.Instantiate(Resources.Load<GameObject>("GameHarnessTesting"));
        }
    }
}
