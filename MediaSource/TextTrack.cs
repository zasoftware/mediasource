using System;
using System.Collections;
using System.Collections.Generic;

namespace Media.MediaSource
{
    interface ITextTrackList : IEventTarget
    {
        ulong Length();
        TextTrack GetTextTrack(ulong Index);
        TextTrack GetTrackById(string Id);

        event EventHandler OnChange;
        event EventHandler OnAddTrack;
        event EventHandler OnRemoveTrack;

    }

    public class TextTrackList : ITextTrackList, IEnumerable
    {
        private List<TextTrack> textTracks;
        public TextTrackList()
        {
            textTracks = new List<TextTrack>();
        }

        public void AddTextTrack(TextTrack textTrack)
        {
            textTracks.Add(textTrack);
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
            return textTracks.GetEnumerator();
        }

        public TextTrack GetTextTrack(ulong Index)
        {
            throw new NotImplementedException();
        }

        public TextTrack GetTrackById(string Id)
        {
            throw new NotImplementedException();
        }

        public ulong Length()
        {
            return (ulong)textTracks.ToArray().LongLength;
        }

        public void RemoveEventListener(string name, EventHandler eventHandler)
        {
            throw new NotImplementedException();
        }
    }

    interface ITextTrack
    {
        string Id { get; set; }
        string Kind { get; set; }
        string Label { get; set; }
        string Language { get; set; }
    }

    public class TextTrack : ITextTrack
    {
        public TextTrack() { }

        TextTrack(string TrackId, string TrackKind, string TrackLabel, string TrackLanguage)
        {
            Id = TrackId;
            Kind = TrackKind;
            Label = TrackLabel;
            Language = TrackLanguage;
        }

        public string Id { get; set; }
        public string Kind { get; set; }
        public string Label { get; set; }
        public string Language { get; set; }
    }
}