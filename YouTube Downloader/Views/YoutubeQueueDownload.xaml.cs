using YouTube_Downloader.ViewModels;

namespace YouTube_Downloader.Views;


public partial class YoutubeQueueDownload : ContentPage
{
    private YoutubeQueueDownloadViewModel _viewModel;

    public YoutubeQueueDownload()
	{
		InitializeComponent();
        _viewModel = BindingContext as YoutubeQueueDownloadViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.RefreshData();
    }
}