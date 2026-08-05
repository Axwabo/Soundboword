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

1. Download the archive for your OS from the [releases page](https://github.com/Axwabo/Soundboword/releases)
2. Extract the archive into a directory of your choosing
3. Run the application
    - Linux: `./Soundboword`
    - Windows: `Soundboword.exe`
   
> [!NOTE]
> `.deb` or `.rpm` packages won't be provided yet, as I have no desire to learn allat :333

# Usage

Click the `Add Sound` button to add a sound.

Press 🔊 to play the sound.

Click ⚙️ to configure settings, including changing [trigger modes](#trigger-modes)

Click 🟦 to stop the sound.

Click ⚡ to assign a shortcut using any of the active [input methods](#input-methods)

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

# Input Methods

These are basically shortcuts, allowing you to trigger a sound or an action using any of the available devices.

## Global Shortcuts

[//]: # (TODO: is this real?)
On Windows, global shortcuts work out of the box. 

On Linux, you need the `xdg-desktop-portal` to be able to assign global shortcuts.

> [!NOTE]
> The portal will **not** prompt the user for subsequent requests to bind the same action.
> If the dialog is closed, the request will be ignored even if no shortcut was bound.
> Additionally, you must go to your System Settings to remove or change global shortcuts.

## Launchpad Mini

This is primarily for my personal use.

If you also have a Novation Launchpad Mini MKII, you're in luck!