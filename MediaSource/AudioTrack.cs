using System;
using System.Collections;
using System.Collections.Generic;

namespace Media.MediaSource
{
    interface IAudioTrackList : IEventTarget
    {
        ulong Length();
        AudioTrack GetAudioTrack(ulong Index);
        AudioTrack GetTrackById(string Id);

        event EventHandler OnChange;
        event EventHandler OnAddTrack;
        event EventHandler OnRemoveTrack;
    }

    public class AudioTrackList : IAudioTrackList, IEnumerable
    {
        private List<AudioTrack> audioTracks;

        public void AddAudioTrack(AudioTrack audioTrack)
        {
            audioTracks.Add(audioTrack);
        }

        public AudioTrackList()
        {
            audioTracks = new List<AudioTrack>();
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

        public AudioTrack GetAudioTrack(ulong Index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return audioTracks.GetEnumerator();
        }

        public AudioTrack GetTrackById(string Id)
        {
            throw new NotImplementedException();
        }

        public ulong Length()
        {
            return (ulong)audioTracks.ToArray().LongLength;
        }

        public void RemoveEventListener(string name, EventHandler eventHandler)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IAudioTrack
    {
        string Id { get; set; }
        string Kind { get; set; }
        string Label { get; set; }
        string Language { get; set; }
        bool Enabled { get; set; }
    }

    public class AudioTrack : IAudioTrack
    {
        public AudioTrack() { }
        public AudioTrack(string TrackId, string TrackKind, string TrackLabel, string TrackLanguage, bool IsEnabled)
        {
            Id = TrackId;
            Kind = TrackKind;
            Label = TrackLabel;
            Language = TrackLanguage;
            Enabled = IsEnabled;
        }
        public string Id { get; set; }
        public string Kind { get; set; }
        public string Label { get; set; }
        public string Language { get; set; }
        public bool Enabled { get; set; }
    }
}