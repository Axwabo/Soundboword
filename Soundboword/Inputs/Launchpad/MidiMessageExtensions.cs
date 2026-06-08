using SoundFlow.Midi.Structs;

namespace Soundboword.Inputs.Launchpad;

public static class MidiMessageExtensions
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

}
