
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YouTube_Downloader.Models
{
    public class QueueDownloadModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public YoutubeExplode.Videos.Video Vid { get; set; }
        public bool completed { get; set; }
        private double _progress { get; set; }

        public double progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
