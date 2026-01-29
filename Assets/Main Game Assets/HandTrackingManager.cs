using UnityEngine;

namespace MainGame
{
    /// <summary>
    /// Singleton manager to access hand tracking data from the Main Game scene.
    /// Place this in the Hand Landmark Detection scene on the HandLandmarkerRunner GameObject.
    /// </summary>
    public class HandTrackingManager : MonoBehaviour
    {
        private static HandTrackingManager _instance;
        public static HandTrackingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<HandTrackingManager>();
                }
                return _instance;
            }
        }

        [Header("Hand Tracking Status")]
        [SerializeField] private bool handsDetected = false;
        [SerializeField] private int numberOfHands = 0;

        // Public properties to access from other scenes
        public bool HandsDetected => handsDetected;
        public int NumberOfHands => numberOfHands;

        private void Awake()
        {
            // Ensure only one instance exists
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            
            // Optional: Make this persist across scene loads
            // DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Call this method when hand detection status changes
        /// </summary>
        public void UpdateHandDetection(bool detected, int handCount)
        {
            handsDetected = detected;
            numberOfHands = handCount;
        }

        /// <summary>
        /// Example method to get hand position data
        /// Extend this based on your needs
        /// </summary>
        public Vector3 GetHandPosition(int handIndex)
        {
            // TODO: Implement actual hand position retrieval from MediaPipe
            // This is a placeholder for you to extend
            return Vector3.zero;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
