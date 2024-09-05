using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YouTube_Downloader.Controllers;

namespace YouTube_Downloader.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private double DownloadProgress { get; set; }
    private List<YoutubeExplode.Videos.Video> VideoItems { get; set; }
    public YoutubeClient YTClient { get; private set; }
    private string YouTubeVideoURL { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public double ObDownloadProgress
    {
        get
        {
            return DownloadProgress;
        }
        set
        {
            DownloadProgress = value;
            OnPropertyChanged();
        }
    }

    public List<YoutubeExplode.Videos.Video> ObVideoItems
    {
        get
        {
            return VideoItems;
        }
        set
        {
            VideoItems = value;
            OnPropertyChanged();
        }
    }

    public string ObYouTubeVideoURL
    {
        get
        {
            return YouTubeVideoURL;
        }
        set
        {
            YouTubeVideoURL = value;
            OnPropertyChanged();
        }
    }

    public MainPageViewModel()
    {
        YTClient = new YoutubeClient();
    }


    public async void GetVideos(string query)
    {

        string url = "https://www.youtube.com/";
        string? PlaylistID = TryGetPlaylistID(query);
        string? WatchID = TryGetVideoID(query);

        if (!string.IsNullOrEmpty(WatchID))
        {
            url += $"watch?v={WatchID}";
            YoutubeExplode.Videos.Video video = await YTClient.Videos.GetAsync(url);
            ObVideoItems = new List<YoutubeExplode.Videos.Video> { video };
        }else if (!string.IsNullOrEmpty(PlaylistID))
        {
            url += $"playlist?list={PlaylistID}";
            var PlaylistVideos = await YTClient.Playlists.GetVideosAsync(PlaylistID);
            List<Video> VideoList = new List<Video>();
            ObVideoItems = VideoList;

            foreach (var PVideo in PlaylistVideos)
            {

                YoutubeExplode.Videos.Video video = new Video(
                    PVideo.Id,
                    PVideo.Title,
                    PVideo.Author,
                    new DateTimeOffset(DateTime.Now),
                    "",
                    PVideo.Duration,
                    PVideo.Thumbnails,
                    new List<string>(),
                    new Engagement(0,0,0)
                    );
                VideoList.Add(video);
            }

            ObVideoItems = VideoList;
        }
        
    }

    private string? TryGetPlaylistID(string query)
    {

        Regex r = new Regex("([^?=&]+)(=([^&]*))?");
        MatchCollection match = r.Matches(query);
        string? PlaylistID = "";

        foreach (Match m in match)
        {
            if (m.Groups.Values.Count() > 1 && m.Value.Contains("list="))
            {
                PlaylistID = m.Value.Split("=")[1];
                return PlaylistID;
            }
        }

        return string.Empty;

    }

    private string? TryGetVideoID(string query)
    {
        Regex r = new Regex("([^?=&]+)(=([^&]*))?");
        MatchCollection match = r.Matches(query);
        string? WatchID = "";

        foreach (Match m in match)
        {
            if (m.Groups.Values.Count() > 1 && m.Value.Contains("v="))
            {
                WatchID = m.Value.Split("=")[1];
                return WatchID;
            }
        }

        return string.Empty;
    }


    public ICommand PerformSearch => new Command<string>((string query) =>
    {
        GetVideos(query);
    });


    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}