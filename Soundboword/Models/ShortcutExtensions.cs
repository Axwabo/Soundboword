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
                case LinkToggleAction {Id: var id} when audioManager.DeviceToggles is { } toggles && toggles.GetCommand(id) is {IsRunning: false} command:
                    command.Execute(null);
                    break;
            }
        }

        public bool IsSound(SoundViewModel sound) => (action as TriggerSoundAction)?.Model == sound;

    }

}
