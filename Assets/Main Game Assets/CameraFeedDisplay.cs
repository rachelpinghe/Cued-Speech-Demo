using UnityEngine;
using UnityEngine.UI;

namespace MainGame
{
    /// <summary>
    /// Displays the camera RenderTexture in a UI element.
    /// Use RawImage for best performance with RenderTextures.
    /// </summary>
    public class CameraFeedDisplay : MonoBehaviour
    {
        [Header("Render Texture Settings")]
        [Tooltip("The RenderTexture from the Hand Landmark Detection camera")]
        [SerializeField] private RenderTexture cameraRenderTexture;
        
        [Header("UI Element")]
        [Tooltip("RawImage component to display the camera feed")]
        [SerializeField] private RawImage displayImage;

        [Header("Display Settings")]
        [Tooltip("Position the display in the top-right corner")]
        [SerializeField] private bool positionTopRight = true;
        
        [Tooltip("Maintain aspect ratio of the RenderTexture")]
        [SerializeField] private bool maintainAspectRatio = true;
        
        [Tooltip("Width of the camera display")]
        [SerializeField] private float displayWidth = 320f;
        
        [Tooltip("Height of the camera display (auto-calculated if maintaining aspect ratio)")]
        [SerializeField] private float displayHeight = 240f;
        
        [Tooltip("Margin from the screen edges")]
        [SerializeField] private float margin = 20f;

        private void Start()
        {
            SetupDisplay();
        }

        private void SetupDisplay()
        {
            // Get or add RawImage component
            if (displayImage == null)
            {
                displayImage = GetComponent<RawImage>();
            }

            if (displayImage == null)
            {
                Debug.LogError("RawImage component not found! Please assign a RawImage to this script.");
                return;
            }

            // Assign the render texture
            if (cameraRenderTexture != null)
            {
                displayImage.texture = cameraRenderTexture;
                
                // Calculate proper aspect ratio
                if (maintainAspectRatio)
                {
                    AdjustAspectRatio();
                }
            }
            else
            {
                Debug.LogWarning("Camera RenderTexture not assigned. Please assign it in the Inspector.");
            }

            // Position in top-right if requested
            if (positionTopRight)
            {
                PositionTopRight();
            }
        }

        /// <summary>
        /// Adjusts the RawImage size to match the RenderTexture aspect ratio
        /// </summary>
        private void AdjustAspectRatio()
        {
            if (cameraRenderTexture == null) return;

            float textureAspect = (float)cameraRenderTexture.width / cameraRenderTexture.height;
            
            // Calculate height based on width and aspect ratio
            displayHeight = displayWidth / textureAspect;
            
            Debug.Log($"RenderTexture: {cameraRenderTexture.width}x{cameraRenderTexture.height}, Aspect: {textureAspect:F2}, Display: {displayWidth}x{displayHeight:F1}");
        }

        /// <summary>
        /// Positions the camera feed display in the top-right corner
        /// </summary>
        private void PositionTopRight()
        {
            RectTransform rectTransform = displayImage.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            // Anchor to top-right
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);

            // Set size
            rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);

            // Position with margin
            rectTransform.anchoredPosition = new Vector2(-margin, -margin);
        }

        /// <summary>
        /// Updates the RenderTexture at runtime if needed
        /// </summary>
        /// <param name="newRenderTexture">New RenderTexture to display</param>
        public void UpdateRenderTexture(RenderTexture newRenderTexture)
        {
            cameraRenderTexture = newRenderTexture;
            if (displayImage != null)
            {
                displayImage.texture = cameraRenderTexture;
            }
        }
    }
}
