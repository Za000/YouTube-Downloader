using System.ComponentModel;
using System.Runtime.CompilerServices;
using YouTube_Downloader.ViewModels;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace YouTube_Downloader
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            DownloadYT();
        }

        private async void DownloadYT()
        {
            /*System.IProgress<double> ProgressChange = new System.Progress<double>(s => CounterBtn.Text = s.ToString());
            var youtube = new YoutubeClient();
            var videoUrl = "https://www.youtube.com/watch?v=v587yNSTPTo";
            await youtube.Videos.DownloadAsync(videoUrl, "E:\\YTDownloader\\video.mp4", o => o.SetFFmpegPath("E:\\YTDownloader\\YouTube Downloader\\YouTube Downloader\\lib\\ffmpeg"), ProgressChange);*/
        }
    }

}
