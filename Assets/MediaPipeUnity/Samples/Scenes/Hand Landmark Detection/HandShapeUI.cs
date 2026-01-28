// This file is no longer used - HandShapeRecognizer now directly updates TextMeshPro
// You can safely delete this file if you want
// 
// The HandShapeRecognizer component now has a direct reference to a TextMeshProUGUI object
// Just drag your TextMeshPro text component to the "Syllable Display Text" field in the inspector

using UnityEngine;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
    [System.Obsolete("This class is deprecated. Use HandShapeRecognizer's syllableDisplayText field instead.")]
    public class HandShapeUI : MonoBehaviour
    {
        // This class is no longer needed
        // HandShapeRecognizer now directly updates a TextMeshProUGUI component
    }
}

