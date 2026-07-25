namespace Soundboword.Models;

public interface IPlaybackSuspender
{

    public static IPlaybackSuspender User { get; } = new UserPlaybackSuspender();

}

file sealed class UserPlaybackSuspender : IPlaybackSuspender;
