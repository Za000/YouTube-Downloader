using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Plugin.Maui.Audio;

namespace YouTube_Downloader.Views;

public partial class YouTubeVideoCollection : ContentView
{
    public static readonly BindableProperty ItemsProperty = BindableProperty.Create(
        nameof(Items), 
        typeof(List<YoutubeExplode.Videos.Video>), 
        typeof(YouTubeVideoCollection), 
        new List<YoutubeExplode.Videos.Video>(),
        propertyChanged:OnThisPropertyChanged);

    public static readonly BindableProperty DownloadProperty = BindableProperty.Create(
        nameof(Download),
        typeof(ICommand),
        typeof(YouTubeVideoCollection),
        null,
        propertyChanged: OnThisPropertyChanged);


    public List<YoutubeExplode.Videos.Video> Items
    {
        get => (List<YoutubeExplode.Videos.Video>)GetValue(YouTubeVideoCollection.ItemsProperty);
        set => SetValue(YouTubeVideoCollection.ItemsProperty, value);
    }

    public ICommand Download
    {
        get => (ICommand)GetValue(YouTubeVideoCollection.DownloadProperty);
        set => SetValue(YouTubeVideoCollection.DownloadProperty, value);
    }

    public YouTubeVideoCollection()
	{
        InitializeComponent();
    }


    private static void OnThisPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var myCard = bindable as YouTubeVideoCollection;
        myCard.OnPropertyChanged();
    }
}