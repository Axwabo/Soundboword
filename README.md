# Soundboword

Are you tired of other soundboard apps limiting the amount of sounds you can use at once?

Meet Soundboword, which is a desktop app that solves this issue.
You can add as many sounds as you'd like!

# Multi-platform

Linux[^1] receives primary support.
To be able to use global keyboard shortcuts, you'll need a desktop environment
that uses `xdg-desktop-portal`. Additional functionality includes pipewire manipulation.

Windows support is partial, but it's expected to work.

MacOS support is not provided. You can add a project and compile the app yourself, and while it might work,
functionality will be limited (no shortcuts).

> [!TIP]
> See also: [platform-specific features](#platform-specific-features)

[^1]: Tested on Fedora KDE 43

# Installation

1. Download the archive for your OS from the [releases page](https://github.com/Axwabo/Soundboword/releases)
    - Linux: `Soundboword-linux-x64.zip`
    - Windows: `Soundboword-slopdows-x64.zip`
2. Extract the archive into a directory of your choosing
3. Run the application
    - Linux: `bin/Soundboword`
    - Windows: `bin/Soundboword.exe`

> [!NOTE]
> `.deb` or `.rpm` packages won't be provided yet, as I have no desire to learn allat :333

# Usage

> [!IMPORTANT]
> Only `mp3` and `wav` files are supported.

Click the `Add Sounds` button to add a sound (or multiple sounds).

Press 🔊 to play the sound.

Click ⚙️ to configure settings, including changing [trigger modes](#trigger-modes)

Click 🟦 to stop the sound.

Click ⚡ to assign a shortcut using any of the active [input methods](#input-methods)

## Sound Management

You can add sounds either by clicking `Add Sounds` or by using drag-and-drop into the board.

Click `Rescan` if you've mounted a drive that a not found was located on.

Click `Relink All` to locate not found sounds in different directories.
The filename must match for a sound to be located.

The `Add From YouTube` button opens a new window that lets you download audio from YouTube.

1. Type a search query or paste a YouTube link
2. Select a stream to download
3. Choose a format
4. Click `Download`
5. The sound will be cached, and it will be added (name = video title)

> [!TIP]
> You can click `Download` while streams are still loading.
> The highest quality stream will be selected in this case.

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

## Sound Interaction

You can specify the behavior for other sounds if you trigger a specific sound.
The behavior will apply to all other playbacks.

| Interaction | Behavior                                                              |
|-------------|-----------------------------------------------------------------------|
| Nothing     | Other sounds are not affected                                         |
| Stop        | Stops the playbacks of other sounds                                   |
| Pause       | Pauses other sounds, and resumes them when the triggered sound(s) end |
| Mute        | Mutes other sounds while the triggered sound(s) are playing           |

> [!NOTE]
> The `Mute` interaction is not fully implemented. Subsequently triggered sounds will not be muted yet.

## Editor

> [!NOTE]
> An improved editor (with waveform rendering) will be implemented later.

Click the ✏️ icon when configuring a sound. This will open up a simple window.

For now, you can only set the following properties with sliders:

| Property   | Effect                                                                                        |
|------------|-----------------------------------------------------------------------------------------------|
| Start Time | The time at which point the sound initially starts                                            |
| Loop Start | Jumps here when reaching the loop end[^2]                                                     |
| Loop End   | Upon reaching this point, jumps to the beginning of the sound (or to the `Loop Start` marker) |

[^2]: Only applies when looping is enabled

> [!NOTE]
> `Loop End` requires the `Loop Start` property to be specified.

> [!TIP]
> You can move the slider by 10 milliseconds by pressing the left or right arrow key.
> Use PageUp or PageDown move it by a second.


# Input Methods

These are basically shortcuts, allowing you to trigger a sound or an action using any of the available devices.

Head to the `🎛️ Inputs` tab to see and enable/disable input devices.

You can set shortcuts for global actions (e.g. `Stop All Sounds`) as follows:

1. Click the ⚙️ next to a device
2. Select an action
3. Press the ⚡ button
4. Use the selected input method to assign a shortcut

## Global Shortcuts

### Windows

On Windows, global shortcuts work out of the box.

Click the ⚡ button to assign a shortcut.
Hold down any modifier keys (i.e. `Ctrl` `Alt` `Shift`), then press and release a non-modifier key.
Modifier keys are not required, it's best to have at least one.

> [!NOTE]
> Some non-modifier keys don't have translations. Please create an issue if you encounter one.

### Linux

On Linux, you need the `xdg-desktop-portal` to be able to assign global shortcuts.
The portal requires at least one modifier key (i.e. `Ctrl` `Alt` `Shift` `Meta`) per shortcut.

> [!NOTE]
> When removing an XDG Global Shortcut, the shortcut will only be unassigned in memory,
> so it will "reappear" after restarting Soundboword.
> Visit your System Settings to remove such a global shortcut.

> [!NOTE]
> The portal will **not** prompt the user for subsequent requests to bind the same action.
> If the dialog is closed, the request will be ignored even if no shortcut was bound.
> Additionally, you must go to your System Settings to remove or change global shortcuts.

## Launchpad Mini

This is primarily for my personal use.

If you also have a Novation Launchpad Mini MKII, you're in luck!

# Platform-specific Features

## Linux

Global shortcuts require `xdg-desktop-portal` (see [this section](#global-shortcuts)).

You can manage some node links using PipeWire.

1. Go to the `🔌 PipeWire` tab, and launch the wizard to set up the `Soundboword Microphone`
2. Select an audio device in the `🔌 PipeWire` tab that will be considered as your physical microphone
3. You will be able to use the `Soundboword Microphone` in programs that capture microphone input
    - Some apps might require a restart to recognize the device

There are some toggles at the bottom right of the window.
The table below shows which nodes will be connected/disconnected.

| Icon    | Name            | Output Node          | Input Node             |
|---------|-----------------|----------------------|------------------------|
| 🔊 🠊 🎧 | Hear Sounds     | Soundboword Playback | Physical Output        |
| 🔊 🠊 📣 | Mic Sounds      | Soundboword Playback | Soundboword Microphone |
| 🎤 🠊 📣 | Mic Passthrough | Physical Microphone  | Soundboword Microphone |
| 🎤 🠊 🎧 | Hear Myself     | Physical Microphone  | Physical Output        |

You can assign shortcuts to each of these actions by configuring a device in the `🎛️ Inputs` tab.

The `Toggle Sounds & Passthrough` action toggles between `Mic Sounds` and `Mic Passthrough`
which lets you assign a separate shortcut while being able to change individual toggles separately.

> [!NOTE]
> Soundboword does **not** install a service that connects your physical microphone to the Soundboword Microphone.
> Tick the `Auto-connect Mic Passthrough` in settings and start Soundboword, or connect the nodes manually.

> [!TIP]
> Install [qpwgraph](https://github.com/rncbc/qpwgraph) to view and modify node links freely.

> [!TIP]
> Add Soundboword to the autostart list for ease of access.

## Windows

Global shortcuts work out of the box (see [this section](#global-shortcuts)).

To use Soundboword as a microphone, you can use [VB-CABLE](https://vb-audio.com/Cable/).
Select `CABLE Output` in the `🎧 Outputs` view; you won't be able to hear sounds though.

For more granular control and audio feedback, try [Voicemeeter Banana](https://vb-audio.com/Voicemeeter/banana.htm).
Voicemeeter installation and configuration instructions will not be provided.
