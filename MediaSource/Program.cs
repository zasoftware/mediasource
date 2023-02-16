using System;
using System.Threading;
using Media.MediaSource;

public class Program
{    
    static MediaSource mediaSource = new MediaSource();
    static SourceBuffer sourceBuffer = null;

    public static void Main(string[] args)
    {
        CountingHours();
        Console.ReadLine();
    }

    static void CountingHours()
    {
        var FirstDate = new DateTime(2022, 09, 25, 11, 47, 00);
        var secondDate = DateTime.Now;
        var Diff = secondDate - FirstDate;

        string format = string.Empty;

        if (Diff.TotalSeconds < 60)
        {
            format = string.Format("{0} {1}", Math.Ceiling(Diff.TotalSeconds), Math.Ceiling(Diff.TotalSeconds) > 1 ? "seconds ago" : "minute ago");
        }

        if (Diff.TotalMinutes < 60 && Diff.TotalSeconds > 60)
        {
            format = string.Format("{0} {1}", Math.Ceiling(Diff.TotalMinutes), Math.Ceiling(Diff.TotalMinutes) > 1 ? "minutes ago" : "minute ago");
        }

        if (Diff.TotalHours <= 24 && Diff.TotalMinutes > 60)
        {
            format = string.Format("{0} {1}", Math.Ceiling(Diff.TotalHours), Math.Ceiling(Diff.TotalHours) > 1 ? "hours ago" : "hour ago"); 
        }
        if (Diff.TotalDays <= 7 && Diff.TotalHours > 24)
        {
            format = string.Format("{0} {1}", Diff.TotalDays, Diff.TotalDays > 1 ? "days ago" : "day ago");
        }
        if (Diff.TotalDays > 7 && Diff.TotalDays < 31)
        {
            format = string.Format("{0} {1}", Math.Ceiling(Diff.TotalDays / 7), Math.Ceiling(Diff.TotalDays / 7) > 1 ? "weeks ago" : "weeks ago");
        }
        if (Diff.TotalDays > 31 && Diff.TotalDays <= 365)
        {
            format = string.Format("{0} {1}", Math.Ceiling(Diff.TotalDays / 31), Math.Ceiling(Diff.TotalDays / 31) > 1 ? "months ago" : "month ago");
        }

        if (Diff.TotalDays > 365)
        {
            format = string.Format("{0} {1}", Math.Ceiling(Diff.TotalDays / 365), Math.Ceiling(Diff.TotalDays / 365) > 1 ? "years ago" : "year ago");
        }

        var days = Math.Ceiling(Diff.TotalDays);
        Console.WriteLine(format);
    }

    static void DelegatesExample()
    {
        Console.WriteLine("***** Delegates as event enablers *****\n");

        // First, make a Car object.
        Car c1 = new Car("SlugBug", 100, 10);

        // Now, tell the car which method to call
        // when it wants to send us messages.
        c1.RegisterWithCarEngine(new Car.CarEngineHandler(OnCarEngineEvent));

        // Speed up (this will trigger the events).
        Console.WriteLine("***** Speeding up *****");
        for (int i = 0; i < 6; i++)
        {
            c1.Accelerate(20);
        }       
    }

    static void MediaSourceDelegate()
    {
        Console.WriteLine("Source Will Open!");

        VideoElement videoElement = new VideoElement();
        videoElement.SetSrc(mediaSource);        
        
        mediaSource.AddEventListener("SourceOpen", new EventHandler(SourceOpened));
        AudioTrackList audioTrackList = videoElement.AudioTrackList();

        Console.WriteLine("AudioTracks In the List: ");
        foreach (var item in audioTrackList)
        {
            Console.WriteLine("AudioTracks In the List: ");
        }

        do
        {
            Console.WriteLine("Waiting for MediaSource to Open!");
            Thread.Sleep(1000);
        } while (!mediaSource.IsReady);
    }

    private static void OnCarEngineEvent(string msgForCaller)
    {
        Console.WriteLine("\n***** Message From Car Object *****");
        Console.WriteLine("=> {0}", msgForCaller);
        Console.WriteLine("**************************************\n");
    }

    /// <summary>
    /// This is the method is called when the source is opened.
    /// </summary>
    /// <param name="sender">The information about the object</param>
    /// <param name="e">the information about the generated event</param>

    public static async void SourceOpened(object sender, EventArgs e)
    {
        sourceBuffer = mediaSource.AddSourceBuffer("video/mp4; codecs=avc3.64001F, mp4a.40.2");
        byte[] testingBuffer = new byte[1000];

        AudioTrack audioTrack = new AudioTrack();
        
        sourceBuffer.AudioTracks.AddAudioTrack(audioTrack);

        Console.WriteLine("Number Of AudioTracks = {0}, VideoTracks = {1}, TextTracks = {2}", 
            sourceBuffer.AudioTracks.Length(), sourceBuffer.VideoTracks.Length(), sourceBuffer.TextTracks.Length());


        for (int i = 0; i < testingBuffer.Length; i++)
        {
            testingBuffer[i] = (byte)(new Random().Next());
        }

        // Register the Buffer Events to listen to.
        EventHandler bufferUpdating = new EventHandler(BufferUpdating);        
        sourceBuffer.OnUpdate += bufferUpdating;

        await sourceBuffer.AppendBuffer(testingBuffer);

        // Media Source Opened
        Console.WriteLine("The Source is Opened!");
    }

    private static void BufferUpdating(object sender, EventArgs e)
    {
        Console.WriteLine("***** Fun with Events *****");
        Console.WriteLine("The Buffer has updated!");
    }
}

public class Car
{
    // Internal state data.
    public int CurrentSpeed { get; set; }
    public int MaxSpeed { get; set; }
    public string PetName { get; set; }

    // Is the car alive or dead?
    private bool carIsDead;

    // Class constructors.
    public Car() { MaxSpeed = 100; }
    public Car(string name, int maxSp, int currSp)
    {
        CurrentSpeed = currSp;
        MaxSpeed = maxSp;
        PetName = name;
    }

    public delegate void CarEngineHandler(string msgForCaller);

    private CarEngineHandler listOfHandlers;
    public void RegisterWithCarEngine(CarEngineHandler methodToCall)
    {
        listOfHandlers = methodToCall;
    }

    public void Accelerate(int delta)
    {
        // if this is "dead" send dead message.
        if (carIsDead)
        {
            listOfHandlers?.Invoke("Sorry, this car is dead...");
        }
        else
        {
            CurrentSpeed += delta;

            // Is this car "almost dead"?
            if (10 == (MaxSpeed - CurrentSpeed) && listOfHandlers != null)
            {
                listOfHandlers("Careful buddy! Gonna blow!");
            }

            if (CurrentSpeed >= MaxSpeed)
            {
                carIsDead = true;
            }
            else
            {
                Console.WriteLine("CurrentSpeed = {0}", CurrentSpeed);
            }
        }
    }
}