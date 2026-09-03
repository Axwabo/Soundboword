using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;

namespace Soundboword.Editor;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public sealed class EditorAudioEngine : IDisposable
{

    private readonly MiniAudioEngine _engine;
    private readonly AudioPlaybackDevice _playback;

    public EditorAudioEngine(MiniAudioEngine engine)
    {
        _engine = engine;
        
    }

    public void Dispose()
    {
        _engine.Dispose();
    }

}
