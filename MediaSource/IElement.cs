using System;

namespace Media.MediaSource
{
    internal interface IElement
    {

    }

    internal interface IMediaElement : IElement
    {
        // error state
        MediaError Error { get; set; }

        // networ state
        string Src { get; set; }
        MediaProvider SrcObject { get; set; }
        string CurrentSrc { get; set; }
        string CrossOrigin { get; set; }

        string Preload { get; set; }
        TimeRanges Buffered { get; set; }
        void Load();
        CanPlayTypeResult CanPlayType(string type);

        // Playback state
        void Play();
        bool Paused();
        double DefaultPlaybackRate { get; set; }
        double PlaybackRate { get; set; }
        TimeRanges Played();
        TimeRanges Seekable();
        bool Ended();
        bool AutoPlay { get; set; }
        bool Loop { get; set; }

        // Controls
        bool Controls { get; set; }
        double Volume { get; set; }
        bool Muted { get; set; }
        bool DefaultMuted { get; set; }

        // tracks
        AudioTrackList AudioTrackList();
        VideoTrackList VideoTrackList();
        TextTrackList TextTrackList();
        TextTrack AddTextTrack(TextTrackKind Kind, string Label = "", string Language = "");
    }

    public class MediaElement : IMediaElement
    {
        public const ushort NETWORK_EMPTY = 0;
        public const ushort NETWORK_IDLE = 1;
        public const ushort NETWORK_LOADING = 2;
        public const ushort NETWORK_NO_SOURCE = 3;
        public ushort NetworkState { get; set; }

        // ready state
        public const ushort HAVE_NOTHING = 0;
        public const ushort HAVE_METADATA = 1;
        public const ushort HAVE_CURRENT_DATA = 2;
        public const ushort HAVE_FUTURE_DATA = 3;
        public const ushort HAVE_ENOUGH_DATA = 4;
        public ushort ReadyState;
        public bool Seeking;

        // Playback state
        public double CurrentTime { get; set; }
        public double Duration { get; set; }
        public object GetStartDate { get; set; }
        public MediaError Error { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Src { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public MediaProvider SrcObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string CurrentSrc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string CrossOrigin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Preload { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeRanges Buffered { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double DefaultPlaybackRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PlaybackRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoPlay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Loop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Controls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Muted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DefaultMuted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public CanPlayTypeResult CanPlayType(string type)
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public bool Paused()
        {
            throw new NotImplementedException();
        }

        public TimeRanges Played()
        {
            throw new NotImplementedException();
        }

        public TimeRanges Seekable()
        {
            throw new NotImplementedException();
        }

        public bool Ended()
        {
            throw new NotImplementedException();
        }

        public AudioTrackList AudioTrackList()
        {
            throw new NotImplementedException();
        }

        public VideoTrackList VideoTrackList()
        {
            throw new NotImplementedException();
        }

        public TextTrackList TextTrackList()
        {
            throw new NotImplementedException();
        }

        public TextTrack AddTextTrack(TextTrackKind Kind, string Label = "", string Language = "")
        {
            throw new NotImplementedException();
        }

        // Controls    




    }

}

namespace Media.MediaSource
{
    public enum CanPlayTypeResult
    {

    }
}