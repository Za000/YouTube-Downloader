using System.ComponentModel;
using System.Runtime.CompilerServices;
using YouTube_Downloader.Controllers;
using System.Windows.Input;
using YouTube_Downloader.Models;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace YouTube_Downloader.ViewModels;

public class YoutubeQueueDownloadViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected IQueueDownloadController QueueDownload { get; }

    public ObservableCollection<QueueDownloadModel> DownloadQue { get; set; }
    public ObservableCollection<QueueDownloadModel> DownloadedList { get; set; }


    public YoutubeQueueDownloadViewModel()
    {
        QueueDownload = App.Services.GetService<IQueueDownloadController>();
        DownloadQue = new ObservableCollection<QueueDownloadModel>(QueueDownload.GetQueue());
        DownloadedList = new ObservableCollection<QueueDownloadModel>(QueueDownload.GetDownloaded().ToList());
        QueueDownload.DownloadCompleted += OnDownloadCompleted;
        QueueDownload.ProgressChanged += OnProgressChanged;
    }

    private void OnDownloadCompleted() => RefreshData();
    private void OnProgressChanged() => RefreshData();

    public void RefreshData()
    {
        foreach (var item in QueueDownload.GetQueue())
        {
            if (DownloadQue.Where(r => r.Vid.Id == item.Vid.Id).Any())
            {
                DownloadQue.Where(r => r.Vid.Id == item.Vid.Id).First().progress = item.progress;
            }
            else
            {
                DownloadQue.Add(item);
            }
            
        }

        foreach (var item in QueueDownload.GetDownloaded())
        {
            if (DownloadQue.Where(r => r.Vid.Id == item.Vid.Id).Any())
            {
                DownloadQue.Remove(item);
            }

            if (!DownloadedList.Where(r => r.Vid.Id == item.Vid.Id).Any())
            {
                DownloadedList.Add(item);
            }
        }

    }

    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}