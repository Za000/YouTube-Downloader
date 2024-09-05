using System.ComponentModel;
using Plugin.Maui.Audio;
using YouTube_Downloader.Controllers;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Windows.Input;
using YoutubeExplode.Videos;

namespace YouTube_Downloader.Views;

public partial class YouTubeVideoTile : ContentView, INotifyPropertyChanged
{
    public static readonly BindableProperty ItemProperty = BindableProperty.Create(
        nameof(Item),
        typeof(YoutubeExplode.Videos.Video),
        typeof(YouTubeVideoTile),
        null,
        propertyChanged: OnThisPropertyChanged);

    public static readonly BindableProperty DownloadProperty = BindableProperty.Create(
        nameof(Download),
        typeof(ICommand),
        typeof(YouTubeVideoTile),
        null,
        propertyChanged: OnThisPropertyChanged);

    private System.Timers.Timer AudioPlayInterval;

    public YoutubeExplode.Videos.Video Item
    {
        get
        {
            if ((YoutubeExplode.Videos.Video)GetValue(YouTubeVideoTile.ItemProperty) != null)
            {
                if (AudioPlayer == null)
                {
                    InitializePlayback(((YoutubeExplode.Videos.Video)GetValue(YouTubeVideoTile.ItemProperty)).Id);
                }
            }
            return (YoutubeExplode.Videos.Video)GetValue(YouTubeVideoTile.ItemProperty);
        }
        set => SetValue(YouTubeVideoCollection.ItemsProperty, value);
    }

    public ICommand Download
    {
        get
        {
            return (ICommand)GetValue(YouTubeVideoTile.DownloadProperty);
        }
        set => SetValue(YouTubeVideoCollection.DownloadProperty, value);
    }

    private string YouTubeVideoURL { get; set; }
    private IAudioPlayerController AudioPlayer;
    private string PlaybackDuration { get; set; } 
    private double AudioCurrentPosition { get; set; }
    private IAudioManager audioManager;
    private double AudioProgress { get; set; }
    private bool IsAudioLoading { get; set; }

    public string ObYouTubeVideoURL
    {
        get
        {
            return YouTubeVideoURL;
        }
        set
        {
            YouTubeVideoURL = value;
            OnPropertyChanged(nameof(ObYouTubeVideoURL));
        }
    }

    public string ObPlaybackDuration
    {
        get
        {
            return PlaybackDuration;
        }
        set
        {
            PlaybackDuration = value;
            OnPropertyChanged(nameof(ObPlaybackDuration));
        }
    }

    public double ObAudioCurrentPosition
    {
        get
        {
            return AudioCurrentPosition;
        }
        set
        {
            AudioCurrentPosition = value;
            ObAudioProgress = value;
            OnPropertyChanged(nameof(ObAudioCurrentPosition));
        }
    }

    public double ObAudioProgress
    {
        get
        {
            return AudioProgress;
        }
        set
        {
            value = Math.Round((value / Item.Duration.Value.TotalSeconds), 2);
            AudioProgress = value;
            OnPropertyChanged(nameof(ObAudioProgress));
        }
    }

    public bool ObIsAudioLoading
    {
        get
        {
            return IsAudioLoading;
        }

        set
        {
            IsAudioLoading = value;
            OnPropertyChanged(nameof(ObIsAudioLoading));
        }
    }

    protected IQueueDownloadController QueueDownload { get; }


    public YouTubeVideoTile()
	{
		InitializeComponent();
        QueueDownload = App.Services.GetService<IQueueDownloadController>();
        ObPlaybackDuration = "00:00 / 00:00";
        audioManager = new AudioManager();
        AudioPlayInterval = new System.Timers.Timer(100);
        AudioPlayInterval.Elapsed += AudioInterval;
        AudioPlayInterval.Start();
        AudioPlayInterval.Enabled = false;
    }

    private ICommand _enqueueDownloadCommand;
    public ICommand EnqueueDownloadCommand => _enqueueDownloadCommand ??= new Command<Video>(async (Video vid) => await EnqueueDownload(vid));

    private async Task EnqueueDownload(Video vid)
    {
        await QueueDownload.EnqueueDownloadAsync(vid);
        OnPropertyChanged(nameof(EnqueueDownloadCommand));
    }

    private async Task<string> GetYouTubeVideoUrl(string videoId)
    {
        var YTClient = new YoutubeClient();
        var streamManifest = await YTClient.Videos.Streams.GetManifestAsync(videoId);
        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        if (streamInfo != null)
        {
            var stream = await YTClient.Videos.Streams.GetAsync(streamInfo);
            var source = streamInfo.Url;

            return source;
        }

        return string.Empty;
    }

    private async void InitializePlayback(string vID)
    {
        ObIsAudioLoading = true;
        string VideoURL = await GetYouTubeVideoUrl(vID);

        if (string.IsNullOrEmpty(VideoURL))
            return;

        ObYouTubeVideoURL = VideoURL;

        
        this.AudioPlayer = new AudioPlayerController(audioManager);
        await this.AudioPlayer.CreatePlayer(GetStreamFromUrl(VideoURL));

        TimeSpan CurrentDuration = TimeSpan.FromMinutes(AudioPlayer.GetCurrentPositon());
        TimeSpan TotalDuration = Item.Duration.Value;
        ObPlaybackDuration =
            $"{CurrentDuration.ToString(@"mm\:ss")} / {TotalDuration.ToString(@"mm\:ss")}";
        ObAudioCurrentPosition = AudioPlayer.GetCurrentPositon();
        ObIsAudioLoading = false;
    }

    private static Stream GetStreamFromUrl(string url)
    {
        byte[] data = null;

        using (var wc = new System.Net.WebClient())
            data = wc.DownloadData(url);

        return new MemoryStream(data);
    }

    private async void Play_OnClicked(object? sender, EventArgs e)
    {
        AudioPlayer.PlayAudio(ObAudioCurrentPosition);
        AudioPlayInterval.Enabled = true;
    }

    private void Pause_OnClicked(object? sender, EventArgs e)
    {
        AudioPlayer.PauseAudio();
        AudioPlayInterval.Enabled = false;
    }

    void AudioInterval(object sender, System.Timers.ElapsedEventArgs e)
    {
        ObAudioCurrentPosition = AudioPlayer.GetCurrentPositon();
        TimeSpan CurrentDuration = TimeSpan.FromSeconds(ObAudioCurrentPosition);
        TimeSpan TotalDuration = Item.Duration.Value;

        ObPlaybackDuration =
            $"{CurrentDuration.ToString(@"mm\:ss")} / {TotalDuration.ToString(@"mm\:ss")}";
    }


    private static void OnThisPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var myCard = bindable as YouTubeVideoTile;
        myCard.OnPropertyChanged();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Button butt = sender as Button;
        butt.Background = Brush.Gray;
        butt.Text = "Pobieranie...";
    }
}