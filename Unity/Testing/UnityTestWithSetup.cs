using UnityEngine;
using UnityEngine.TestTools;

namespace ASG.Utilities.Unity.Testing
{
    public class UnityTestWithSetup : IPrebuildSetup
    {
        /// <inheritdoc />
        public virtual void Setup()
        {
            // Add our canary that sits around during tests.
            Debug.Log("Setup test harness to catch logged errors.");

            Object.Instantiate(Resources.Load<GameObject>("GameHarnessTesting"));
        }
    }
}
