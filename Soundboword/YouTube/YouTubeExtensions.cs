using YoutubeExplode.Videos;

namespace Soundboword.YouTube;

public static class YouTubeExtensions
{

    extension(IVideo video)
    {

        public string? Description => video switch
        {
            Video {Description: var description} => description,
            YouTubeVideo {Description: var description} => description,
            _ => null
        };

    }

}
