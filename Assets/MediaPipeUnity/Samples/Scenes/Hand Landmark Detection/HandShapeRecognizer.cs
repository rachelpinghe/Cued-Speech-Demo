// Handshape Recognition System for Cued Speech
// Recognizes 8 different handshapes and outputs corresponding syllables

using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Tasks.Components.Containers;
using TMPro;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
    public class HandShapeRecognizer : MonoBehaviour
    {
        // Define the 8 handshapes and their corresponding syllables
        public enum HandShape
        {
            None,
            Shape1,
            Shape2,
            Shape3,
            Shape4,
            Shape5,
            Shape6,
            Shape7,
            Shape8
        }

        // Map each handshape to a list of 3 syllables
        private Dictionary<HandShape, string[]> handShapeSyllables = new Dictionary<HandShape, string[]>()
        {
            { HandShape.None, new string[0] },
            { HandShape.Shape1, new string[] { "p", "d", "zh" } },      // Example: Fist (all fingers closed)
            { HandShape.Shape2, new string[] { "k", "q", "z" } },      // Example: Index finger extended
            { HandShape.Shape3, new string[] { "s", "r", "h" } },      // Example: Index and middle extended (V-shape)
            { HandShape.Shape4, new string[] { "b", "n", "yu" } },      // Example: Index, middle, ring extended
            { HandShape.Shape5, new string[] { "m", "t", "f" } },      // Example: All fingers extended (open hand)
            { HandShape.Shape6, new string[] { "l", "x", "w" } },      // Example: Thumb extended only
            { HandShape.Shape7, new string[] { "g", "j", "ch" } },      // Example: Thumb and index extended (L-shape)
            { HandShape.Shape8, new string[] { "y", "c", "sh" } }       // Example: Pinky extended only
        };

        // Confidence threshold for recognition
        [SerializeField] private float recognitionConfidence = 0.7f;
        
        // Debounce to avoid multiple detections
        [SerializeField] private float detectionCooldown = 0.5f;
        private float lastDetectionTime = 0f;
        
        // Time to reset the previous shape (allows same shape to be detected again)
        [SerializeField] private float shapeResetTime = 0.5f;
        
        // UI reference - Direct TextMeshPro object for displaying syllables
        [SerializeField] private TextMeshProUGUI syllableDisplayText;
        
        private HandShape currentHandShape = HandShape.None;
        private HandShape previousHandShape = HandShape.None;
        
        // For thread-safe time tracking
        private System.DateTime lastDetectionDateTime = System.DateTime.MinValue;
        private System.DateTime lastShapeChangeDateTime = System.DateTime.MinValue;
        
        // For thread-safe UI updates
        private bool needsUIUpdate = false;
        private string pendingSyllableText = "";

        // MediaPipe hand landmark indices
        private const int WRIST = 0;
        private const int THUMB_CMC = 1;
        private const int THUMB_MCP = 2;
        private const int THUMB_IP = 3;
        private const int THUMB_TIP = 4;
        private const int INDEX_MCP = 5;
        private const int INDEX_PIP = 6;
        private const int INDEX_DIP = 7;
        private const int INDEX_TIP = 8;
        private const int MIDDLE_MCP = 9;
        private const int MIDDLE_PIP = 10;
        private const int MIDDLE_DIP = 11;
        private const int MIDDLE_TIP = 12;
        private const int RING_MCP = 13;
        private const int RING_PIP = 14;
        private const int RING_DIP = 15;
        private const int RING_TIP = 16;
        private const int PINKY_MCP = 17;
        private const int PINKY_PIP = 18;
        private const int PINKY_DIP = 19;
        private const int PINKY_TIP = 20;

        /// <summary>
        /// Main method to process hand landmarks and recognize handshape
        /// </summary>
        public void ProcessHandLandmarks(HandLandmarkerResult result)
        {
            if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            {
                currentHandShape = HandShape.None;
                return;
            }

            // Process the first detected hand
            var landmarks = result.handLandmarks[0];
            
            // Recognize the handshape
            HandShape detectedShape = RecognizeHandShape(landmarks);
            
            // Update current handshape
            currentHandShape = detectedShape;
            
            var now = System.DateTime.Now;
            var timeSinceLastDetection = (now - lastDetectionDateTime).TotalSeconds;
            var timeSinceLastShapeChange = (now - lastShapeChangeDateTime).TotalSeconds;
            
            // Reset previous shape after shapeResetTime to allow same shape detection
            if (timeSinceLastShapeChange > shapeResetTime)
            {
                previousHandShape = HandShape.None;
            }
            
            // Output syllable if handshape changed and cooldown passed
            if (timeSinceLastDetection > detectionCooldown)
            {
                if (currentHandShape != HandShape.None && currentHandShape != previousHandShape)
                {
                    OutputSyllable(currentHandShape);
                    previousHandShape = currentHandShape;
                    lastDetectionDateTime = now;
                    lastShapeChangeDateTime = now;
                }
            }
        }

        /// <summary>
        /// Recognizes handshape based on finger states
        /// </summary>
        private HandShape RecognizeHandShape(NormalizedLandmarks landmarks)
        {
            // Calculate which fingers are extended
            bool thumbExtended = IsThumbExtended(landmarks);
            bool indexExtended = IsFingerExtended(landmarks, INDEX_MCP, INDEX_PIP, INDEX_DIP, INDEX_TIP);
            bool middleExtended = IsFingerExtended(landmarks, MIDDLE_MCP, MIDDLE_PIP, MIDDLE_DIP, MIDDLE_TIP);
            bool ringExtended = IsFingerExtended(landmarks, RING_MCP, RING_PIP, RING_DIP, RING_TIP);
            bool pinkyExtended = IsFingerExtended(landmarks, PINKY_MCP, PINKY_PIP, PINKY_DIP, PINKY_TIP);

            // Recognize handshape based on finger combination
            // You can customize these patterns based on your specific cued speech handshapes
            
            // Shape 1: [p,d,zh]
            if (!thumbExtended && indexExtended && !middleExtended && !ringExtended && !pinkyExtended)
            {
                return HandShape.Shape1;
            }
            
            // Shape 2: [k,q,z]
            if (!thumbExtended && indexExtended && middleExtended && !ringExtended && !pinkyExtended)
            {
                return HandShape.Shape2;
            }
            
            // Shape 3: [s,r,h]
            if (!thumbExtended && !indexExtended && middleExtended && ringExtended && pinkyExtended)
            {
                return HandShape.Shape3;
            }
            
            // Shape 4: [b,n,yu]
            if (!thumbExtended && indexExtended && middleExtended && ringExtended && pinkyExtended)
            {
                return HandShape.Shape4;
            }
            
            // Shape 5: [m,t,f]
            if (thumbExtended && indexExtended && middleExtended && ringExtended && pinkyExtended)
            {
                return HandShape.Shape5;
            }
            
            // Shape 6: [l,x,w]
            if (thumbExtended && indexExtended && !middleExtended && !ringExtended && !pinkyExtended)
            {
                return HandShape.Shape6;
            }
            
            // Shape 7: [g,j,ch]
            if (thumbExtended && indexExtended && middleExtended && !ringExtended && !pinkyExtended)
            {
                return HandShape.Shape7;
            }
            
            // Shape 8: [y,c,sh]
        if (!thumbExtended && indexExtended && middleExtended && !ringExtended && !pinkyExtended)
            {
                return HandShape.Shape8;
            }

            return HandShape.None;
        }

        /// <summary>
        /// Checks if a finger is extended based on landmark positions
        /// </summary>
        private bool IsFingerExtended(NormalizedLandmarks landmarks, int mcp, int pip, int dip, int tip)
        {
            var landmarkList = landmarks.landmarks;
            
            // Calculate distance from MCP to TIP
            float tipDistance = Vector3.Distance(
                new Vector3(landmarkList[mcp].x, landmarkList[mcp].y, landmarkList[mcp].z),
                new Vector3(landmarkList[tip].x, landmarkList[tip].y, landmarkList[tip].z)
            );
            
            // Calculate distance from MCP to PIP
            float pipDistance = Vector3.Distance(
                new Vector3(landmarkList[mcp].x, landmarkList[mcp].y, landmarkList[mcp].z),
                new Vector3(landmarkList[pip].x, landmarkList[pip].y, landmarkList[pip].z)
            );
            
            // If tip is significantly farther from MCP than PIP, finger is extended
            // Threshold can be adjusted based on testing
            return tipDistance > pipDistance * 1.5f;
        }

        /// <summary>
        /// Checks if thumb is extended (uses different logic due to thumb anatomy)
        /// </summary>
        private bool IsThumbExtended(NormalizedLandmarks landmarks)
        {
            var landmarkList = landmarks.landmarks;
            
            // Calculate distance from wrist to thumb tip
            float thumbTipDistance = Vector3.Distance(
                new Vector3(landmarkList[WRIST].x, landmarkList[WRIST].y, landmarkList[WRIST].z),
                new Vector3(landmarkList[THUMB_TIP].x, landmarkList[THUMB_TIP].y, landmarkList[THUMB_TIP].z)
            );
            
            // Calculate distance from wrist to thumb MCP
            float thumbMcpDistance = Vector3.Distance(
                new Vector3(landmarkList[WRIST].x, landmarkList[WRIST].y, landmarkList[WRIST].z),
                new Vector3(landmarkList[THUMB_MCP].x, landmarkList[THUMB_MCP].y, landmarkList[THUMB_MCP].z)
            );
            
            // Thumb is extended if tip is significantly farther from wrist than MCP
            return thumbTipDistance > thumbMcpDistance * 1.8f;
        }

        /// <summary>
        /// Outputs the syllable for the detected handshape
        /// </summary>
        private void OutputSyllable(HandShape shape)
        {
            if (handShapeSyllables.TryGetValue(shape, out string[] syllableList))
            {
                string syllablesText = string.Join(" / ", syllableList);
                Debug.Log($"[HandShape Recognition] Detected: {shape} → Syllables: {syllablesText}");
                
                // Queue UI update (will be applied on main thread in Update)
                pendingSyllableText = syllablesText;
                needsUIUpdate = true;
                
                // Call event or method for further processing
                OnSyllableDetected(syllableList, shape);
            }
        }
        
        /// <summary>
        /// Update method to handle UI updates on main thread
        /// </summary>
        private void Update()
        {
            if (needsUIUpdate && syllableDisplayText != null)
            {
                syllableDisplayText.text = pendingSyllableText;
                needsUIUpdate = false;
            }
        }

        /// <summary>
        /// Event method called when a syllable is detected
        /// Override or subscribe to this for custom behavior
        /// </summary>
        protected virtual void OnSyllableDetected(string[] syllables, HandShape shape)
        {
            // This is where you can add custom behavior:
            // - Display syllable on UI
            // - Play audio
            // - Build words from syllables
            // - Trigger game events
            // etc.
        }

        /// <summary>
        /// Get the currently detected handshape
        /// </summary>
        public HandShape GetCurrentHandShape()
        {
            return currentHandShape;
        }

        /// <summary>
        /// Get the syllable for the current handshape
        /// </summary>
        public string[] GetCurrentSyllables()
        {
            if (handShapeSyllables.TryGetValue(currentHandShape, out string[] syllables))
            {
                return syllables;
            }
            return new string[0];
        }

        /// <summary>
        /// Get the syllables as a formatted string
        /// </summary>
        public string GetCurrentSyllablesText()
        {
            var syllables = GetCurrentSyllables();
            return syllables.Length > 0 ? string.Join(" / ", syllables) : "";
        }

        /// <summary>
        /// Customize syllable mappings
        /// </summary>
        public void SetSyllableMapping(HandShape shape, string[] syllables)
        {
            if (handShapeSyllables.ContainsKey(shape))
            {
                handShapeSyllables[shape] = syllables;
            }
        }
    }
}
