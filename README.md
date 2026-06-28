# Soundboword

Are you tired of other soundboard apps limiting the amount of sounds you can use at once?

Meet Soundboword, which is a desktop app that solves this issue.
You can add as many sounds as you'd like!

# Multi-platform

Linux[^1] receives primary support.
To be able to use global keyboard shortcuts, you'll need a desktop environment
that uses `xdg-desktop-portal`. Additional functionality includes pipewire manipulation.

Windows support is partial, but it's expected to work.

MacOS support is not provided. You can compile the app yourself, and while it might work,
functionality will be limited (no shortcuts).

[^1]: Tested on Fedora KDE 43 

# Installation

[//]: # (TODO: Linux installation steps)

1. Make sure to have .NET 10 installed
    - On windows, the Desktop Runtime is sufficient
2. Download the binary for your OS from the [releases page](https://github.com/Axwabo/Soundboword/releases)
3. Run the application

# Usage

Click the `Add Sound` button to add a sound.

Press 🔊 to play the sound.

Click ⚙️ to configure settings, including changing [trigger modes](#trigger-modes)

Click 🟦 to stop the sound.

Click 🔁 to toggle looping.

[//]: # (TODO: sound setup guide)

## Trigger Modes

### Start 🠊 Stop

When you press the trigger, the sound will start playing.

If you trigger it again, it'll stop playing.

### Start 🠊 Restart

When you press the trigger, the sound will start playing.

If you trigger it again, it'll restart from the beginning, unpausing playback.

### Play 🠊 Pause

When you press the trigger, the sound will start playing.

If you trigger it again, it'll pause or unpause playback.

### Duplicate

Each time you press the trigger, a new sound instance will be played.