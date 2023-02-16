using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Media.MediaSource
{
    // EventTarget interface
    public interface IEventTarget
    {
        void AddEventListener(string name, EventHandler eventHandler);
        void RemoveEventListener(string name, EventHandler eventHandler);
        void DispatchEvent(string name);        
    }

    // IMediaSource interface
    public interface IMediaSource : IEventTarget
    {
        SourceBuffer AddSourceBuffer(string type);
        void RemoveSourceBuffer(SourceBuffer sourceBuffer);
        void EndOfStream();
        void SetLiveSeekableRange(double start, double end);
    }

    // ISourceBuffer interface
    public interface ISourceBuffer : IEventTarget
    {
        Task AppendBuffer(byte[] Data);
        void Abort();
        void Remove(double start, double end);

        event EventHandler OnUpdate;
        event EventHandler OnUpdateStart;
        event EventHandler OnUpdateEnd;
        event EventHandler OnError;
        event EventHandler OnAbort;
    }

    // Custom EventArgs
    public class MediaSourceEventArgs : EventArgs
    {
        public readonly string Message;
        public MediaSourceEventArgs() { }
        public MediaSourceEventArgs(string Msg, MediaSource mediaSource)
        {
            Message = Msg;           
        }
    }

    public class SourceBufferEventArgs : EventArgs
    {
        public readonly string Message;
        public SourceBufferEventArgs() { }
        public SourceBufferEventArgs(string Msg, SourceBuffer sourceBuffer)
        {
            sourceBuffer.Updating = true;
            Message = Msg;
        }
    }

    // The State of the MediaSource
    public enum ReadyState
    {
        closed = 0,
        opened = 1
    }

    // Only For Delegate Demonstration
    public delegate void SourceHandler(object sender, EventArgs e);
    public delegate void BufferHandler(object sender, EventArgs e);

    public class MediaSource : IMediaSource
    {
        // Member for sake of testing.
        public bool IsReady = false;

        public bool isSet()
        {
            SetMediaSource();
            IsReady = true;
            return IsReady;
        }

        public void SetMediaSource()
        {
            EventArgs eventArgs = new EventArgs();
            EventHandler onSourceOpen = new EventHandler(MediaSource_OnSourceOpen);
            onSourceOpen(this, eventArgs);
        }

        // public event EventHandler OnSourceEnded;
        // public event EventHandler OnSourceClosed;

        public MediaSource()
        {
            
        }

        private void MediaSource_OnSourceOpen(object sender, EventArgs e)
        {
            MediaSourceEvent(sender, e);
        }

        // Members to maintain the sourceBuffers of this mediaSource
        public List<SourceBuffer> sourceBuffers = new List<SourceBuffer>();
        public List<SourceBuffer> activeSourceBuffers = new List<SourceBuffer>();

        // Event Handler
        private EventHandler MediaSourceEvent;

        public SourceBuffer AddSourceBuffer(string type)
        {
            SourceBuffer sourceBuffer = new SourceBuffer();
            sourceBuffers.Add(sourceBuffer);
            return sourceBuffer;
        } 

        public void RemoveSourceBuffer(SourceBuffer SourceBuffer)
        {

        }

        public void SetLiveSeekableRange(double start, double end)
        {

        }

        public void ClearLiveSeekableRange()
        {

        }

        public bool isTypeSupported(string type)
        {
            return true;
        }

        public void AddEventListener(string name, EventHandler eventHandler)
        {
            MediaSourceEvent += eventHandler;
        }

        public void RemoveEventListener(string name, EventHandler eventHandler)
        {
            throw new NotImplementedException();
        }

        public void DispatchEvent(string name)
        {
            throw new NotImplementedException();
        }

        public void EndOfStream()
        {
            throw new NotImplementedException();
        }
    }

    public class SourceBufferList : IEnumerable
    {
        private ArrayList SourceBuffers = new ArrayList();
        private IDictionary<string, SourceBuffer> Name_SourceBuffers = new Dictionary<string, SourceBuffer>();
        private IDictionary<int, SourceBuffer> Index_SourceBuffers = new Dictionary<int, SourceBuffer>();

        public SourceBuffer this[int index]
        {
            get { return (SourceBuffer)SourceBuffers[index]; }
            set { SourceBuffers.Insert(index, value); }
        }

        public SourceBuffer this[int index, string name]
        {
            get { return (SourceBuffer)SourceBuffers[index]; }
            set { SourceBuffers.Insert(index, value); }
        }

        public SourceBuffer this[string name]
        {
            get { return (SourceBuffer)Name_SourceBuffers[name]; }
            set { Name_SourceBuffers.Add(name, value); }
        }

        public IEnumerator GetEnumerator()
        {
            return SourceBuffers.GetEnumerator();
        }
    }

    public class SourceBuffer : ISourceBuffer
    {
        private AppendState AppendState { get; set; }
        private byte[] InputBuffer { get; set; }

        private bool IsBufferFull { get; set; }
        private double GroupStartTimestamp { get; set; }
        private double GroupEndTimestamp { get; set; }
        private bool GenerateTimestamp { get; set; }
        private bool FirstInitialization { get; set; }
        private double Duration;
        private bool ActiveFlag;

        private List<TrackBuffer> trackBuffers;

        public SourceBuffer()
        {
            AppendState = AppendState.WAITING_FOR_SEGMENT;

            AudioTracks = new AudioTrackList();
            VideoTracks = new VideoTrackList();
            TextTracks = new TextTrackList();
        }

        private async void SegmentParser(byte[] Data)
        {
            Updating = true;

            // Placing Data in the source Buffer Here

            // Declare resources to using
            using (MemoryStream stream = new MemoryStream(Data))
            {
                // OnUpdate(this, new SourceBufferEventArgs("Updating", this));

                // Check if InputBuffer is Empty
                if (InputBuffer.Length == 0)
                {
                    return;
                }

                // Check the bytes that violates the byte Stream format specification.
                if (!InputBuffer.Equals(Data))
                {                    
                    throw new Exception();
                }

                // Remove Unwanted Bytes from the Input Buffer


                if (AppendState == AppendState.WAITING_FOR_SEGMENT)
                {
                    // InputBuffer is Initialization
                    if (!GetInitialization(InputBuffer))
                    {
                        AppendState = AppendState.PARSING_INIT_SEGMENT;
                    }
                    else
                    {
                        AppendState = AppendState.PARSING_MEDIA_SEGMENT;
                    }
                }

                if (AppendState == AppendState.PARSING_INIT_SEGMENT)
                {
                    // Is it a Complete Initialization Segment
                    if (GetCompleteInit(InputBuffer))
                    {
                        InitReceived(InputBuffer);


                        // Jump to the Loop top
                    }
                }

                // Is it appending Media Segment?
                if (AppendState == AppendState.PARSING_MEDIA_SEGMENT)
                {

                }

                await ReadBytes(Data);

                // Place data into the Buffer here!
                // ...

                // Done placing data to be decoded into the sourceBuffer.
                // now set the Updating property to false again, signaling ready to take more data.
                Updating = false;

                OnUpdate(this, new SourceBufferEventArgs("Update Ended", this));
            }            
        }        
        
        private void InitReceived(byte[] inputBuffer)
        {
            try
            {
                trackBuffers = new List<TrackBuffer>();

                // 1. Check Duration
                if (Duration == 0)
                {
                    // Run duration change algorithm
                    DurationChange(inputBuffer);
                }
                else
                {
                    // Run Duration Change set new Duration.
                    DurationChange(GetDuration(inputBuffer));
                }

                // 2. Check if Initialization has atleast one track
                CheckInitialization(inputBuffer);

                // 3. Check the FirstInitialization is true
                if (FirstInitialization)
                {
                    // 1. Verify the following properties.

                    /// <summary>
                    /// 1. The Number of audio, video and text tracks match what was in the first initialization segment.
                    /// 2. The codecs for each track, match what was specified in the first initialization segment.
                    /// 3. If more than one track for a single type are present (e.g 2 audio tracks), then the The Track IDs match the ones in the first initialization segment.
                    /// </summary>
                    // 2. Add the appropriate track descriptions from this Initialization segment to each of the track buffers.
                    // 3. Set the need random access point flag on all track buffers to true.
                }

                // 4. Let active track flag equal false.

                // 5. If the FirstInitialization is false.
                if (!FirstInitialization)
                {
                    // 1. Check if the codec is supported.
                    bool _IsTypeSupported = IsTypeSupported(inputBuffer);

                    // 2. For each audio track in the initialization segment, run the following steps:
                    /// <summary>
                    /// 1. let audio byte stream track ID be the TrackId for the current track being processed.
                    /// 2. Let audio language be a BCP 47 language tag for the language specified in the initialization segment.
                    /// 3. if audio language equals the 'und' BCP 47 value, then assign an empty string to audio language.
                    /// 4.Let audio label be a label specified in the initialization segment for this track or an empty string  if no label info is present.
                    /// 5. Let audio kinds be a sequence of kind strings specified in the initialization segment for this track or a sequence with a single empty string element in it if no kind information is provided.
                    /// 6. For each value in audio kinds, run the following steps:
                    ///     1.Let current audio kind equal the value
                    /// </summary>
                    
                    long audioTrackCounter = GetAudioTracks(inputBuffer);

                    Func<string, Tuple<string, string, string, string[]>> GetTrackInf = new Func<string, Tuple<string, string, string, string[]>>((trackType) =>
                    {
                        var inf = Tuple.Create("trackId", "trackLanguage", "trackLabel", new string[] { "", "", ""});

                        return inf;
                    });

                    var myTuple = GetTrackInf("audio");

                    for (int i = 0; i < audioTrackCounter; i++)
                    {
                        string TrackID = myTuple.Item1;
                        string Language = myTuple.Item2;
                        string Label = myTuple.Item3;
                        string[] Kinds = myTuple.Item4;

                        if (Language.Equals("und"))
                        {
                            Language = string.Empty;
                        }
                        
                        foreach (string kind in Kinds)
                        {
                            string _Kind = kind;
                            string Id = GenerateUniqueId();

                            AudioTrack audioTrack = new AudioTrack();
                            audioTrack.Id = Id;
                            audioTrack.Kind = _Kind;
                            audioTrack.Label = Label;
                            audioTrack.Language = Language;

                            if (AudioTracks.Length() == 0)
                            {
                                audioTrack.Enabled = true;
                                ActiveFlag = true;
                            }
                            AudioTracks.AddAudioTrack(audioTrack);
                            // 9. Add new audio track to the audioTracks attribute on the HTMLMediaElement.
                            // Create a new Track Buffer to store coded frames for this track

                            // trackBuffers.Add(CreateTrackBuffer(audioTrack));
                        }
                    }
                    FirstInitialization = true;
                }
                long NumberOfTracks = GetNumberOfTracks(InputBuffer);               
            }
            catch (Exception ex)
            {

            }
        }

        private string GenerateUniqueId()
        {
            throw new NotImplementedException();
        }

        private long GetAudioTracks(byte[] inputBuffer)
        {
            return 2;
        }

        private bool IsTypeSupported(byte[] inputBuffer)
        {
            return true;
        }

        private void CheckInitialization(byte[] inputBuffer)
        {
            throw new NotImplementedException();
        }

        private double GetDuration(byte[] inputBuffer)
        {
            return 8.09;
        }

        private void DurationChange(double Duration)
        {
            this.Duration = Duration;
        }

        private void DurationChange(byte[] inputBuffer)
        {
            throw new NotImplementedException();
        }

        private TrackBuffer CreateTrackBuffer()
        {
            TrackBuffer trackBuffer = new TrackBuffer();
            trackBuffer.GetDecodeTS = 23.89;
            trackBuffer.GetNeedRAP = true;

            return trackBuffer;
        }

        private long GetNumberOfTracks(byte[] inputBuffer)
        {
            return 3;
        }

        private bool GetCompleteInit(byte[] inputBuffer)
        {
            return true;
        }

        private bool GetInitialization(byte[] inputBuffer)
        {
            using (MemoryStream memoryStream = new MemoryStream(inputBuffer))
            {
                if (FirstInitialization)
                {
                    return true;
                }
                else
                {
                    return false;
                }                
            }
        }

        public enum AppendMode
        {
            segments = 0,
            sequency = 1
        }

        public bool Updating { get; set; }
        public TimeRanges Buffered { get; set; }
        public double TimeStampOffset { get; set; }
        
        public AudioTrackList AudioTracks { get; set; }
        public VideoTrackList VideoTracks { get; set; }
        public TextTrackList TextTracks { get; set; }
        public double AppendWindowStart { get; set; }
        public double AppendWindowEnd { get; set; }
        

        public event EventHandler OnUpdate;
        public event EventHandler OnUpdateEnd;
        public event EventHandler OnError;
        public event EventHandler OnAbort;
        public event EventHandler OnUpdateStart;

        // Event handler
        private EventHandler BufferHandler;
       

        public async Task AppendBuffer(byte[] Data)
        {
            await ReadBytes(Data);
        }

        private void BufferUpdating(object sender, EventArgs e)
        {
            BufferHandler(sender, e);
        }

        private Task ReadBytes(byte[] data)
        {
            return Task.Run(() => { });
        }

        public void Abort()
        {

        }

        public void Remove(double Start, double End)
        {

        }

        public void AddEventListener(string name, EventHandler eventHandler)
        {
            BufferHandler += BufferUpdating;
        }

        public void RemoveEventListener(string name, EventHandler eventHandler)
        {
            
        }

        public void DispatchEvent(string name)
        {
            
        }
    }

    public class TimeRanges
    {
        public double Start { get; set; }
        public double End { get; set; }
    }
}

namespace Media.MediaSource
{
    enum AppendState
    {
        WAITING_FOR_SEGMENT = 0,
        PARSING_INIT_SEGMENT = 1,
        PARSING_MEDIA_SEGMENT = 2
    }
}