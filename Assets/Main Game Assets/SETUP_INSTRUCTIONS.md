# Hand Landmark Detection - Additive Scene Setup

## Overview
This setup allows the "Hand Landmark Detection" scene to run simultaneously with the "Main Game" scene, displaying the camera feed in a small UI box in the top-right corner.

## Files Created
1. **AdditiveSceneLoader.cs** - Loads the hand detection scene additively
2. **CameraFeedDisplay.cs** - Displays the RenderTexture in the UI

## Setup Instructions

### Step 1: Main Game Scene Setup
1. Open the **Main Game** scene
2. Create an empty GameObject (e.g., "SceneManager") and add the **AdditiveSceneLoader** component
3. In the Inspector, verify the scene name is set to "Hand Landmark Detection"
4. Ensure "Load On Start" is checked

### Step 2: UI Setup (Two Options)

#### Option A: Using RawImage (Recommended for RenderTextures)
1. In your Canvas, delete the Plane child object
2. Right-click Canvas → UI → Raw Image
3. Rename it to "CameraFeed"
4. Add the **CameraFeedDisplay** component to it
5. Assign your "Cam" RenderTexture to the "Camera Render Texture" field
6. The script will automatically position it in the top-right corner
7. Adjust Display Width, Height, and Margin as needed

#### Option B: Keep Using Material on Plane (Less Recommended)
1. Keep your existing Plane setup
2. Ensure the Material with "Cam" RenderTexture is assigned
3. Use a World Space Canvas or manually position the plane

### Step 3: Build Settings (Already Updated)
The EditorBuildSettings.asset has been updated to include both scenes:
- Main Game (index 0)
- Hand Landmark Detection (index 1)

### Step 4: Hand Landmark Detection Scene Camera
1. Open the "Hand Landmark Detection" scene
2. Find the Main Camera
3. Ensure its Target Texture is set to your "Cam" RenderTexture
4. Save the scene

### Step 5: Test
1. Open the "Main Game" scene
2. Press Play
3. Both scenes should load, and you should see the camera feed in the top-right corner
4. Hand tracking should work while the main game runs

## How It Works

### Additive Scene Loading
Unity's Scene Management allows multiple scenes to be loaded simultaneously:
- **Main Game** scene contains the game logic and UI
- **Hand Landmark Detection** scene runs in the background for hand tracking
- The camera from the hand detection scene renders to a RenderTexture
- The UI displays that RenderTexture

### RenderTexture Pipeline
```
Hand Detection Camera → RenderTexture "Cam" → UI RawImage → Display
```

## Troubleshooting

### Camera Feed Not Showing
- Verify the RenderTexture "Cam" is assigned to both:
  - Hand Detection Camera's Target Texture
  - CameraFeedDisplay script's Camera Render Texture field
- Check that the Hand Landmark Detection scene is loading (check Console logs)

### Hand Detection Not Running
- Ensure the Hand Landmark Detection scene has all required components active
- Check for errors in the Console
- Verify the webcam permissions are granted

### Performance Issues
- The RenderTexture resolution affects performance (lower = better performance)
- Consider reducing the display size in CameraFeedDisplay settings
- Check the hand detection confidence thresholds

### Both Scenes Active
- Only one scene can be "active" at a time (for lighting, etc.)
- The Main Game scene should be the active scene
- The Hand Detection scene runs additively in the background

## Advanced: DontDestroyOnLoad (Optional)
If you need objects from the Hand Detection scene to persist when loading other scenes:

```csharp
void Awake()
{
    DontDestroyOnLoad(gameObject);
}
```

Add this to critical GameObjects in the Hand Landmark Detection scene.

## Script Customization

### Change Display Position
Edit `CameraFeedDisplay.cs` and modify the `PositionTopRight()` method or set `positionTopRight = false` and manually position the RawImage.

### Load Scene Manually
Instead of auto-loading, you can call:
```csharp
GetComponent<AdditiveSceneLoader>().LoadHandDetectionScene();
```

### Unload Scene
To unload the hand detection scene:
```csharp
GetComponent<AdditiveSceneLoader>().UnloadHandDetectionScene();
```

## Notes
- Additive scene loading is perfect for modular features like hand tracking
- The hand detection scene runs independently of the main game
- Both scenes share the same game state and can communicate via static classes or singletons
- Memory usage increases with multiple scenes loaded

## Next Steps
1. Test the setup in Play mode
2. Build UI elements around the camera feed
3. Add hand gesture controls to your main game
4. Optimize RenderTexture resolution for your target platform
