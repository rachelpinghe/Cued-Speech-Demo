using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainGame
{
    /// <summary>
    /// Loads the Hand Landmark Detection scene additively when the Main Game starts.
    /// This allows the hand tracking camera to run simultaneously with the main game.
    /// </summary>
    public class AdditiveSceneLoader : MonoBehaviour
    {
        [Header("Scene Settings")]
        [Tooltip("Name of the scene to load additively")]
        [SerializeField] private string sceneToLoad = "Hand Landmark Detection";
        
        [Tooltip("Load the scene automatically on Start")]
        [SerializeField] private bool loadOnStart = true;

        private bool isSceneLoaded = false;

        private void Start()
        {
            if (loadOnStart)
            {
                LoadHandDetectionScene();
            }
        }

        /// <summary>
        /// Loads the Hand Landmark Detection scene additively
        /// </summary>
        public void LoadHandDetectionScene()
        {
            if (isSceneLoaded)
            {
                Debug.LogWarning("Hand Landmark Detection scene is already loaded.");
                return;
            }

            // Load the scene additively
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            isSceneLoaded = true;
            
            Debug.Log($"Loading {sceneToLoad} scene additively...");
        }

        /// <summary>
        /// Unloads the Hand Landmark Detection scene if needed
        /// </summary>
        public void UnloadHandDetectionScene()
        {
            if (!isSceneLoaded)
            {
                Debug.LogWarning("Hand Landmark Detection scene is not loaded.");
                return;
            }

            SceneManager.UnloadSceneAsync(sceneToLoad);
            isSceneLoaded = false;
            
            Debug.Log($"Unloading {sceneToLoad} scene...");
        }
    }
}
