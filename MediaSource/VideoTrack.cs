using System;
using System.Collections;
using System.Collections.Generic;

namespace Media.MediaSource
{
    interface IVideoTrackList : IEventTarget
    {
        ulong Length();
        VideoTrack GetVideoTrack(ulong Index);
        VideoTrack GetTrackById(string Id);

        event EventHandler OnChange;
        event EventHandler OnAddTrack;
        event EventHandler OnRemoveTrack;
    }

    public class VideoTrackList : IVideoTrackList, IEnumerable
    {
        private List<VideoTrack> videoTracks;
        public VideoTrackList()
        {
            videoTracks = new List<VideoTrack>();
        }

        public void AddVideoTrack(VideoTrack videoTrack)
        {
            videoTracks.Add(videoTrack);
        }

        public event EventHandler OnChange;
        public event EventHandler OnAddTrack;
        public event EventHandler OnRemoveTrack;

        public void AddEventListener(string name, EventHandler eventHandler)
        {
            throw new NotImplementedException();
        }

        public void DispatchEvent(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public VideoTrack GetTrackById(string Id)
        {
            throw new NotImplementedException();
        }

        public VideoTrack GetVideoTrack(ulong Index)
        {
            throw new NotImplementedException();
        }

        public ulong Length()
        {
            return (ulong)videoTracks.ToArray().LongLength;
        }

        public void RemoveEventListener(string name, EventHandler eventHandler)
        {
            throw new NotImplementedException();
        }
    }

    
    internal interface IVideoTrack
    {
        string Id { get; set; }
        string Kind { get; set; }
        string Label { get; set; }
        string Language { get; set; }
        bool Selected { get; set; }
    }

    public class VideoTrack : IVideoTrack
    {
        VideoTrack() { }

        VideoTrack(string trackId, string trackKind, string trackLable, string trackLanguage, bool isSelected)
        {
            Id = trackId;
            Kind = trackKind;
            Label = trackLable;
            Language = trackLanguage;
            Selected = isSelected;
        }

        public string Id { get; set; }
        public string Kind { get; set; }
        public string Label { get; set; }
        public string Language { get; set; }
        public bool Selected { get; set; }
    }
}