namespace Media.MediaSource
{
    public class TrackBuffer
    {
        private string TrackId;
        private object TrackDescription;
        private double DecodeTimeStamp;
        private double FrameDuration;
        private double EndTimeStamp;
        private bool NeedRAP = true;
        private TimeRanges TimeRanges;

        public TrackBuffer() { }
        public TrackBuffer(string trackId, string trackDes, double DTS)
        {
            TrackId = trackId;
            TrackDescription = trackDes;
            DecodeTimeStamp = DTS;
        }

        public string GetTrackId { get; set; }
        public object GetTrackDec { get; set; }
        public double GetDecodeTS { get; set; }
        public double GetFrameDuration { get; set; }
        public double GetEndTimeStamp { get; set; }
        public bool GetNeedRAP { get; set; }
        public TimeRanges GetTimeRanges { get; set; }
    }
}