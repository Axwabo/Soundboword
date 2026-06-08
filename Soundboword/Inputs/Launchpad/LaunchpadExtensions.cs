using SoundFlow.Midi.Structs;

namespace Soundboword.Inputs.Launchpad;

public static class LaunchpadExtensions
{

    private const byte TopRowStatusByte = 176;
    private const int TopRowValueStart = 104;
    private const int TopRowEnumValue = 8 * 16;

    extension(MidiMessage message)
    {

        public LaunchpadKey LaunchpadKey => message.StatusByte == TopRowStatusByte
            ? (LaunchpadKey) (TopRowValueStart - message.NoteNumber + TopRowEnumValue)
            : (LaunchpadKey) message.NoteNumber;

    }

    extension(LaunchpadKey key)
    {

        public string FriendlyName => key switch
        {
            LaunchpadKey.Top1 => "🔝1",
            LaunchpadKey.Top2 => "🔝2",
            LaunchpadKey.Top3 => "🔝3",
            LaunchpadKey.Top4 => "🔝4",
            LaunchpadKey.Top5 => "🔝5",
            LaunchpadKey.Top6 => "🔝6",
            LaunchpadKey.Top7 => "🔝7",
            LaunchpadKey.Top8 => "🔝8",
            _ => key.ToString()
        };

    }

}
