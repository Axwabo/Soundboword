namespace Soundboword.Linux;

public static class ShortcutActionExtensions
{

    extension(ShortcutAction action)
    {

        public string Description => action switch
        {
            StopAllSoundsAction => "Stop All Sounds",
            TriggerSoundAction sound => $"Trigger {sound.Model.Name}",
            _ => action.ToString()
        };

    }

}
