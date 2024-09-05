using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using YouTube_Downloader.Models;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace YouTube_Downloader.Controllers;

public interface IQueueDownloadController
{
    Task EnqueueDownloadAsync(YoutubeExplode.Videos.Video video);
    List<QueueDownloadModel>? GetDownloaded();
    ObservableCollection<QueueDownloadModel> GetQueue();
    event Action DownloadCompleted;
    event Action ProgressChanged;
}

public class QueueDownloadController : IQueueDownloadController
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private readonly ConcurrentQueue<QueueDownloadModel> _downloadQueue = new 
        ConcurrentQueue<QueueDownloadModel>();
    private readonly List<QueueDownloadModel> downloaded = new List<QueueDownloadModel>();
    private readonly HttpClient _httpClient = new HttpClient();

    public event Action DownloadCompleted;
    public event Action ProgressChanged;

    public ObservableCollection<QueueDownloadModel> DownloadQueue { get; private set; }

    public QueueDownloadController()
    {
        DownloadQueue = new ObservableCollection<QueueDownloadModel>();
    }

    public List<QueueDownloadModel>? GetDownloaded() => downloaded;
    public ObservableCollection<QueueDownloadModel> GetQueue() => DownloadQueue;

    public async Task EnqueueDownloadAsync(YoutubeExplode.Videos.Video video)
    {
        if (_downloadQueue.Where(r => r.Vid.Id == video.Id).Any())
        {
            return;
        }

        QueueDownloadModel currentVideo = new QueueDownloadModel()
        {
            completed = false,
            progress = 0,
            Vid = video
        };
        _downloadQueue.Enqueue(currentVideo);
        DownloadQueue.Add(currentVideo);
        await ProcessQueueAsync();
    }


    private async Task ProcessQueueAsync()
    {
        while (_downloadQueue.TryDequeue(out var url))
        {
            await _semaphore.WaitAsync();

            try
            {
                await DownloadMusicAsync(url);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    private async Task DownloadMusicAsync(QueueDownloadModel vid)
    {
        var youtube = new YoutubeClient();

        try
        {
            await youtube.Videos.DownloadAsync(vid.Vid.Url,
                $"E:\\YTDownloader\\YouTube Downloader\\YouTube Downloader\\downloads\\{vid.Vid.Title}.mp3", o => o
                    .SetContainer("mp3")
                    .SetPreset(ConversionPreset.UltraFast) 
                    .SetFFmpegPath(
                        "E:\\YTDownloader\\YouTube Downloader\\YouTube Downloader\\lib\\ffmpeg.exe")
            , new Progress<double>(value =>
                {
                    vid.progress = value;
                    ProgressChanged?.Invoke();
                }));

            vid.completed = true;
            downloaded.Add(vid);
            DownloadQueue.Remove(vid);
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert(
                "Błąd pobierania",
                $"Nie udało się pobrać muzyki {vid.Vid.Title}",
                "OK");

            DownloadQueue.Remove(vid);
        }

        DownloadCompleted?.Invoke();

    }
}