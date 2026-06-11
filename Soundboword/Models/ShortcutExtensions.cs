using Soundboword.ViewModels;

namespace Soundboword.Models;

public static class ShortcutExtensions
{

    extension(Shortcut shortcut)
    {

        public void Trigger(AudioManager manager) => shortcut.Action.Trigger(manager);

        public bool IsSound(SoundViewModel sound) => shortcut.Action.IsSound(sound);

    }

    extension(ShortcutAction action)
    {

        public void Trigger(AudioManager audioManager)
        {
            switch (action)
            {
                case TriggerSoundAction {Model: var sound}:
                    audioManager.Trigger(sound);
                    break;
                case StopAllSoundsAction:
                    audioManager.StopAll();
                    break;
            }
        }

        public bool IsSound(SoundViewModel sound) => (action as TriggerSoundAction)?.Model == sound;

    }

}
