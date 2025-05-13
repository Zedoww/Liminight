# Background Audio System Setup Guide

## Overview
This guide explains how to set up background music in your game using the `BackgroundAudioManager`.

## Setup Steps

### Option 1: Setup through GameManager (Recommended)
The GameManager automatically creates a BackgroundAudioManager instance during startup if one doesn't exist. No additional setup is required, but you'll need to configure audio clips in the inspector.

1. When running the game, find the generated `BackgroundAudioManager` GameObject in the hierarchy
2. Assign audio clips in the inspector:
   - Set your main background track in `Default Background Track`
   - Add any alternative tracks to the `Alternative Background Tracks` list
3. Adjust volume and fade settings as needed

### Option 2: Manual Setup
If you prefer to set up the audio manager manually:

1. Create an empty GameObject in your scene and name it "BackgroundAudioManager"
2. Add the `BackgroundAudioManager` component to this GameObject
3. Configure the audio settings in the inspector
4. Make sure to check "Play On Awake" if you want music to start automatically

## Usage in Code

### Toggle Music On/Off
```csharp
// Using GameManager (recommended)
GameManager.Instance.ToggleBackgroundMusic();

// Or directly
BackgroundAudioManager.Instance.ToggleAudio();
```

### Change Background Music
```csharp
// Using GameManager (recommended)
// Play default track
GameManager.Instance.ChangeBackgroundMusic(-1);
// Play alternative track (by index)
GameManager.Instance.ChangeBackgroundMusic(0); // First alternative track

// Or directly
BackgroundAudioManager.Instance.PlayDefaultTrack();
BackgroundAudioManager.Instance.PlayTrackByIndex(0); // First alternative track
```

### Change Volume
```csharp
BackgroundAudioManager.Instance.SetVolume(0.75f); // 0.0f to 1.0f
```

### Enable/Disable Music
```csharp
BackgroundAudioManager.Instance.SetEnabled(false); // Disable music
BackgroundAudioManager.Instance.SetEnabled(true);  // Enable music
```

### Control Fade Durations
```csharp
// Set fade durations for smoother transitions (in seconds)
BackgroundAudioManager.Instance.SetFadeDurations(5f, 4f); // 5s fade in, 4s fade out

// Get current fade durations
Vector2 fadeTimes = BackgroundAudioManager.Instance.GetFadeDurations();
float fadeIn = fadeTimes.x;
float fadeOut = fadeTimes.y;
```

## Example Use Cases

1. **Game Start**: Default background music plays automatically
2. **Dangerous Area**: Switch to tense music when player enters a specific zone
   ```csharp
   // In your trigger zone script
   private void OnTriggerEnter(Collider other) {
       if (other.CompareTag("Player")) {
           GameManager.Instance.ChangeBackgroundMusic(1); // Play tense music
       }
   }
   ```

3. **Boss Fight**: Switch to battle music during intense moments
   ```csharp
   // In your boss encounter script
   public void StartBossFight() {
       // Make the transition to boss music more dramatic with longer fade
       BackgroundAudioManager.Instance.SetFadeDurations(3f, 6f);
       GameManager.Instance.ChangeBackgroundMusic(2); // Play boss music
   }
   ```

4. **Silent Areas**: Disable music in specific zones
   ```csharp
   // In your silent zone trigger script
   private void OnTriggerEnter(Collider other) {
       if (other.CompareTag("Player")) {
           BackgroundAudioManager.Instance.SetEnabled(false);
       }
   }
   
   private void OnTriggerExit(Collider other) {
       if (other.CompareTag("Player")) {
           BackgroundAudioManager.Instance.SetEnabled(true);
       }
   }
   ``` 