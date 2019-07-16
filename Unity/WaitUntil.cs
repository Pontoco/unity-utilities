using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ASG.Utilites.Unity
{
    /// <summary>
    ///     Suspends the coroutine execution, with a timeout, until the supplied delegate evaluates to
    ///     true.
    /// </summary>
    public sealed class WaitUntilWithTimeout : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                if (Time.time - startTime > m_timeout)
                {
                    throw new TimeoutException(m_message);
                }
                return !m_Predicate();
            }
        }

        private readonly Func<bool> m_Predicate;
        private readonly float m_timeout;
        private readonly string m_message;
        private float startTime;

        /// <summary>Suspends the coroutine execution until delegate evaluates to true</summary>
        /// <throws>TimeOutException() after the specified time</throws>
        public WaitUntilWithTimeout(float timeout, Func<bool> predicate, string message)
        {
            m_timeout = timeout;
            m_Predicate = predicate;
            m_message = message;
            startTime = Time.time;
        }
    }

    /// <summary>Suspends the coroutine execution, until the scene is fully loaded</summary>
    public sealed class LoadSceneAndWaitUntilReady : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get { return sceneloading.MoveNext(); }
        }

        private readonly IEnumerator sceneloading;

        /// <summary>Suspends the coroutine execution, until the scene is fully loaded</summary>
        public LoadSceneAndWaitUntilReady(string scenename, float timeout = 10f)
        {
            sceneloading = LoadScene(scenename, timeout);
        }

        [UsedImplicitly]
        public LoadSceneAndWaitUntilReady(string scenename, float timeout, Dictionary<string, int> resolution)
        {
            sceneloading = LoadSceneWithResolution(scenename, timeout, resolution);
        }

        private IEnumerator LoadScene(string m_scenename, float m_timeout)
        {
            var currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(m_scenename);

            var wait = new WaitUntilWithTimeout(m_timeout, () => SceneManager.GetActiveScene() != currentScene,
                "Scene " + m_scenename + " timed out on loading");
            while (wait.keepWaiting)
            {
                yield return null;
            }
            yield return null;
            yield return null;
        }

        private IEnumerator LoadSceneWithResolution(string m_scenename, float m_timeout,
                                                    Dictionary<string, int> resolution)
        {
            var currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(m_scenename);

            Screen.SetResolution(resolution["x"], resolution["y"], false);

            var wait = new WaitUntilWithTimeout(m_timeout, () => SceneManager.GetActiveScene() != currentScene,
                "Scene " + m_scenename + " timed out on loading");
            while (wait.keepWaiting)
            {
                yield return null;
            }
            yield return null;
            yield return null;
        }
    }

    public sealed class WaitUntilSceneReady : CustomYieldInstruction
    {
        private IEnumerator sceneloading;
        private Scene currentScene;

        public WaitUntilSceneReady(string scenename, float timeout)
        {
            sceneloading = WaitForScene(scenename, timeout);
            currentScene = SceneManager.GetActiveScene();
        }


        private IEnumerator WaitForScene(string m_scenename, float m_timeout)
        {
            var wait = new WaitUntilWithTimeout(m_timeout, () => SceneManager.GetActiveScene() != currentScene,
                "Scene " + m_scenename + " timed out on loading");
            while (wait.keepWaiting)
            {
                yield return null;
            }
            yield return null;
            yield return null;
        }

        /// <inheritdoc />
        public override bool keepWaiting
        {
            get { return sceneloading.MoveNext(); }
        }
    }
}