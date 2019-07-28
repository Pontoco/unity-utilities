using System.Collections;
using NUnit.Framework;
using Optional;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ASG.Utilities.Unity.Testing
{
    public abstract class UnityTestWithSetup
    {
        private static Option<Scene> currentTestScene = Option<Scene>.None();

        /// <summary>The path of the test scene to load when running the test.</summary>
        protected readonly string testSceneName;

        /// <param name="testSceneName">The path of the test scene to load when running this test.</param>
        protected UnityTestWithSetup(string testSceneName)
        {
            this.testSceneName = testSceneName;
        }

        /// <summary>This function is called just before the test scene is loaded so that you can do pre-setup.</summary>
        /// <remarks>
        ///     <see cref="SetUpAttribute" /> functions are run at the very start of the test, but because of other issues,
        ///     it's not guaranteed that the last test is properly cleaned up yet. This function runs, instead, after we know the
        ///     last test is totally gone.
        /// </remarks>
        protected virtual void BeforeTestSceneLoad()
        {
        }

        /// <summary>Ran before immediately before every test case. We can't do much here because we can't perform async tasks.</summary>
        [SetUp]
        public void Setup()
        {
            Debug.Log($"Starting test [{testSceneName}]");
        }

        /// <summary>
        ///     Ran immediately after every test case. This is the only function I know of that is guaranteed to be run even
        ///     if the test fails.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Debug.Log($"Tearing down test [{testSceneName}]...");
            SceneManager.UnloadSceneAsync(currentTestScene.Value).completed += _ =>
            {
                Debug.Log($"Finished tearing down test [{testSceneName}]");
                currentTestScene = Option<Scene>.None();
            };
        }

        // Note: Can't be a [Setup] method because this needs to be a Coroutine
        /// <summary>Adds the Test Harness prefab that contains the game harness and some testing scripts.
        ///     <para>This function should be yielded at the start of every test. </para>
        /// </summary>
        public IEnumerator SetupTestHarness()
        {
            // Wait for the previous test to unload.
            if (currentTestScene.HasValue)
            {
                Debug.Log($"Waiting for previous test scene [{currentTestScene.Value.name}] to clean up.");
            }

            yield return new WaitUntil(() => !currentTestScene.HasValue);

            Debug.Log("===============");
            Debug.Log($"Beginning test: {TestContext.CurrentContext.Test.Name}");
            Debug.Log("===============");

            BeforeTestSceneLoad();

            Debug.Log($"Loading test scene: [{testSceneName}]");
            yield return SceneManager.LoadSceneAsync(testSceneName, new LoadSceneParameters(LoadSceneMode.Additive));

            currentTestScene = SceneManager.GetSceneByName(testSceneName).Some();
            SceneManager.SetActiveScene(currentTestScene.Value);

            // Waits for the game harness / input system to have the first frame.
            Debug.Log("Waiting for the first fixed update to finish.");
            yield return new WaitForFixedUpdate();
        }
    }
}
