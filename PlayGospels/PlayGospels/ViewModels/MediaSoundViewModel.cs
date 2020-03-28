using PlayGospels.Common;
using Plugin.SimpleAudioPlayer;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using Xamarin.Forms;

namespace PlayGospels.ViewModels {
    public class MediaSoundViewModel : BindableBase {
        private readonly ISimpleAudioPlayer player = CrossSimpleAudioPlayer.Current;
        private double leftHelper;
        public event EventHandler ChapterEnded;
        public MediaSoundViewModel() {
            player.PlaybackEnded += Player_PlaybackEnded;

            PlayCommand = new DelegateCommand(() => IsPlay = !IsPlay);
            PositionCommand = new DelegateCommand<string>(SetPosition);
        }

        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand<string> PositionCommand { get; private set; }

        private bool isPlay;
        public bool IsPlay {
            get => isPlay;
            set => SetProperty(ref isPlay, value, OnIsPlayChanged);
        }

        private double progress;
        public double Progress {
            get => progress;
            set => SetProperty(ref progress, value);
        }

        private TimeSpan aLeft;
        public TimeSpan ALeft {
            get => aLeft;
            set => SetProperty(ref aLeft, value);
        }

        private TimeSpan tLeft;
        public TimeSpan TLeft {
            get => tLeft;
            set => SetProperty(ref tLeft, value);
        }

        private bool isThereSound;
        public bool IsThereSound {
            get => isThereSound;
            set => SetProperty(ref isThereSound, value);
        }

        public void SetLeftHelper(double leftHelper) => this.leftHelper = leftHelper;
        public void Stop() {
            IsPlay = false;
            player.Stop();
        }

        public void Start(string lang, string bookFolder, string chapterName, double leftHelper) {
            this.leftHelper = leftHelper;
            Stop();
            var fileName = Given.GetFileSoundFullName(lang, bookFolder, chapterName);
            IsThereSound = File.Exists(fileName);
            if (IsThereSound) {
                try {
                    var stream = File.OpenRead(fileName);
                    player.Load(stream);
                    IsPlay = true;
                }
                catch { }
            }
            else {
                Progress = 0;
                ALeft = TimeSpan.Zero;
            }
        }

        private void OnIsPlayChanged() {
            if (IsPlay) {
                player.Play();
                Device.StartTimer(TimeSpan.FromSeconds(1), Timer_Tick);
            }
            else {
                player.Pause();
            }
        }

        private void SetPosition(string delta) {
            if (player.CanSeek) {
                if (int.TryParse(delta, out int d)) {
                    player.Seek(player.CurrentPosition + d);
                }
            }
        }

        private bool Timer_Tick() {
            if (IsPlay) {
                var position = player.CurrentPosition;
                Progress = player.Duration > double.Epsilon ? position / player.Duration : 0d;
                var left = player.Duration - position;
                ALeft = TimeSpan.FromSeconds(left);
                TLeft = TimeSpan.FromSeconds(left + leftHelper);
                return true;
            }
            else {
                return false;
            }
        }

        private void Player_PlaybackEnded(object sender, EventArgs e) {
            Stop();
            ChapterEnded?.Invoke(sender, e);
        }
    }
}
