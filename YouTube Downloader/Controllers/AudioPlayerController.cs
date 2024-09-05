namespace YouTube_Downloader.Controllers;

using Plugin.Maui.Audio;

public interface IAudioPlayerController
{
    void PlayAudio(double position);
    void PauseAudio();

    double GetCurrentPositon();
    double GetAudioDuration();
    double GetVolume();
    Task CreatePlayer(Stream path);

    void SetVolume(double volume);
    void SetAudioPosition(double position);
}

public class AudioPlayerController : IAudioPlayerController
{
    private readonly IAudioManager AudioManager;
    private AsyncAudioPlayer AudioPlayer;

    public AudioPlayerController(IAudioManager audioManager)
    {
        this.AudioManager = audioManager;
    }

    public async Task CreatePlayer(Stream path)
    {
        this.AudioPlayer = this.AudioManager.CreateAsyncPlayer(path);
    }

    public async void PlayAudio(double position)
    {
        if (!this.AudioPlayer.IsPlaying)
        {
            SetAudioPosition(position);
            await this.AudioPlayer.PlayAsync(default);
        }
    }

    public void PauseAudio() => this.AudioPlayer.Stop();

    public double GetCurrentPositon() => this.AudioPlayer.CurrentPosition;
    public double GetAudioDuration() => this.AudioPlayer.Duration;
    public double GetVolume() => this.AudioPlayer.Volume;

    public void SetVolume(double volume) => this.AudioPlayer.Volume = volume;
    public void SetAudioPosition(double position) => this.AudioPlayer.Seek(position);
}