using System.Threading;
using System.Threading.Tasks;

namespace Media.MediaSource
{
    public class AudioElement : MediaElement
    {

    }

    public class VideoElement : MediaElement
    {
        MediaSource source = new MediaSource();

        public AudioTrackList audioTrackList { get { return source.sourceBuffers[0].AudioTracks; } }
        public VideoTrackList videoTrackList { get { return source.sourceBuffers[0].VideoTracks; } }        
        
        public void SetSrc(MediaSource source)
        {
            Task.Run(() =>
            {
                while (!source.isSet())
                {
                    Thread.Sleep(1000);
                }
            });
        }
    }
}