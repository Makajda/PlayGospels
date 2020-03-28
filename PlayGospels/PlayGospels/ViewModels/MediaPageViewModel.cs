using PlayGospels.Common;
using PlayGospels.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayGospels.ViewModels {
    public class MediaPageViewModel : ViewModelBase {
        private bool notInit;
        private string bookFolder;
        private List<Chapter> endedChapters;
        private bool recalcDuration;
        public MediaPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator,
            MediaSoundViewModel mediaSoundViewModel, MediaTextViewModel mediaTextViewModel)
            : base(navigationService, eventAggregator) {
            MediaSoundViewModel = mediaSoundViewModel;
            MediaTextViewModel = mediaTextViewModel;

            ToChoiceCommand = new DelegateCommand(async () => await NavigationService.GoBackAsync());
            RepeatCommand = new DelegateCommand<string>(Repeat);
            CtrpnlCommand = new DelegateCommand(() => IsCtrpnlHide = !IsCtrpnlHide);
            LoadSoundCommand = new DelegateCommand<string>(LoadSound);

            MediaSoundViewModel.ChapterEnded += (s, e) => ChapterEnded();
        }

        public DelegateCommand ToChoiceCommand { get; private set; }
        public DelegateCommand<string> RepeatCommand { get; private set; }
        public DelegateCommand CtrpnlCommand { get; private set; }
        public DelegateCommand<string> LoadSoundCommand { get; private set; }
        public MediaSoundViewModel MediaSoundViewModel { get; private set; }
        public MediaTextViewModel MediaTextViewModel { get; private set; }

        private string inTheBeginning;
        public string InTheBeginning {
            get => inTheBeginning;
            set => SetProperty(ref inTheBeginning, value);
        }

        private string bookTitle;
        public string BookTitle {
            get => bookTitle;
            private set => SetProperty(ref bookTitle, value);
        }

        //Selected Chapters
        private Chapter[] chapters;
        public Chapter[] Chapters {
            get => chapters;
            private set => SetProperty(ref chapters, value);
        }

        private int times = 1;
        public int Times {
            get => times;
            set => SetProperty(ref times, value);
        }

        private Chapter currentChapter;
        public Chapter CurrentChapter {
            get => currentChapter;
            set => SetProperty(ref currentChapter, value, OnCurrentChapterChanged);
        }

        private string langLeft;
        public string LangLeft {
            get => langLeft;
            set => SetProperty(ref langLeft, value, OnLangLeftChanged);
        }

        private string langRight;
        public string LangRight {
            get => langRight;
            set => SetProperty(ref langRight, value, OnLangRightChanged);
        }

        private string langSound;
        public string LangSound {
            get => langSound;
            set => SetProperty(ref langSound, value, OnLangSoundChanged);
        }

        private string langCaption;
        public string LangCaption {
            get { return langCaption; }
            private set { SetProperty(ref langCaption, value); }
        }

        private bool isLangShowedUp;
        public bool IsLangShowedUp {
            get => isLangShowedUp;
            set => SetProperty(ref isLangShowedUp, value);
        }

        private bool isCtrpnlHide;
        public bool IsCtrpnlHide {
            get => isCtrpnlHide;
            set => SetProperty(ref isCtrpnlHide, value);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters) {
            base.OnNavigatedFrom(parameters);
            MediaSoundViewModel.Stop();
            IsLangShowedUp = false;
            parameters.Add(Given.LangSoundParameterName, LangSound);
            parameters.Add(Given.EndedChaptersParameterName, endedChapters);
            parameters.Add(Given.RecalcDurationParameterName, recalcDuration);
        }
        public override void OnNavigatedTo(INavigationParameters parameters) {
            base.OnNavigatedTo(parameters);
            if (parameters.Count == 0) {//from Download
                UpdateDurationAndPlayCurrent();
            }
            else {//from Choice
                if (parameters.TryGetValue(Given.BookFolderParameterName, out string bookFolder)) {
                    this.bookFolder = bookFolder;
                }

                if (parameters.TryGetValue(Given.ChaptersParameterName, out Chapter[] chapters)) {
                    Chapters = chapters;
                }

                if (Chapters != null && Chapters.Length > 0) {
                    endedChapters = null;
                    CurrentChapter = null;
                    CurrentChapter = Chapters[0];
                }
            }
        }

        private void OnCurrentChapterChanged() {
            if (CurrentChapter != null) {
                MediaTextViewModel.SetVerses(bookFolder, CurrentChapter.Name, LangLeft, LangRight);
                MediaSoundViewModel.Start(LangSound, bookFolder, CurrentChapter.Name, CalcLeftHelper());
            }
        }

        private double CalcLeftHelper() {
            var leftHelper = Times * Chapters.Sum(n => n.Duration) - Chapters.TakeWhile(n => n != CurrentChapter).Sum(n => n.Duration) - CurrentChapter.Duration;
            return leftHelper;
        }

        private void ChapterEnded() {
            if (Times == 1) {
                if (endedChapters == null) {
                    endedChapters = new List<Chapter>();
                }

                if (Chapters.Length - endedChapters.Count > 1) {
                    endedChapters.Add(CurrentChapter);
                }
            }

            var chapterIndex = Array.IndexOf(Chapters, CurrentChapter);
            chapterIndex++;
            if (chapterIndex >= Chapters.Length) {
                Times--;
                chapterIndex = 0;
            }

            if (Times > 0) {
                CurrentChapter = Chapters[chapterIndex];
            }
            else {
                Times = 1;
                ToChoiceCommand.Execute();
            }
        }

        private void OnLangSoundChanged() {
            InTheBeginning = Given.GetInTheBeginning(LangSound);
            BookTitle = Given.GetBookTitle(LangSound, bookFolder);
            SetLangCaption();
            if (notInit) {
                UpdateDurationAndPlayCurrent();
            }
            else {
                notInit = true;
            }
        }

        private void UpdateDurationAndPlayCurrent() {
            recalcDuration = true;
            if (Chapters != null) {
                foreach (var chapter in Chapters) {
                    chapter.ResetDuration(LangSound, bookFolder);
                }
            }

            if (CurrentChapter != null) {
                MediaSoundViewModel.Start(LangSound, bookFolder, CurrentChapter.Name, CalcLeftHelper());
            }
        }

        private void Repeat(string n) {
            if (int.TryParse(n, out int k)) {
                Times += k;
                if (Times <= 0) Times = 1;
                MediaSoundViewModel.SetLeftHelper(CalcLeftHelper());
            }
        }

        private void OnLangLeftChanged() {
            MediaTextViewModel.SetVersesLeft(LangLeft);
            SetLangCaption();
        }
        private void OnLangRightChanged() {
            MediaTextViewModel.SetVersesRight(LangRight);
            SetLangCaption();
        }

        private void SetLangCaption() => LangCaption = $"{LangLeft}      \U0001D11E{LangSound}      {LangRight}";

        private async void LoadSound(string what) {
            var parameters = new NavigationParameters {
                { Given.WhatDownloadParameterName, what },
                { Given.LangSoundParameterName, LangSound },
                { Given.BookFolderParameterName, bookFolder},
                { Given.ChapterParameterName, CurrentChapter?.Name },
            };

            IsLangShowedUp = false;
            await NavigationService.NavigateAsync(Given.DownloadViewName, parameters);
        }

        protected override void RestoreSettings() {
            base.RestoreSettings();
            IsCtrpnlHide = Settings.Instance.IsCtrpnlHide;
            MediaTextViewModel.IsLangLeft = Settings.Instance.IsLangLeft;
            MediaTextViewModel.IsLangRight = Settings.Instance.IsLangRight;
            MediaTextViewModel.FontSize = Settings.Instance.TextFontSize;
            Times = Settings.Instance.Times;
            if (Times <= 0) {
                Times = 1;
            }

            var langLeft = Settings.Instance.LangLeft;
            var langRight = Settings.Instance.LangRight;
            var langSound = Settings.Instance.LangSound;

            if (string.IsNullOrWhiteSpace(langLeft)) {
                langLeft = Given.Langs.ElementAt(1);
            }

            if (string.IsNullOrWhiteSpace(langRight)) {
                langRight = Given.Langs.ElementAt(2);
            }

            if (string.IsNullOrWhiteSpace(langSound)) {
                langSound = langLeft;
            }

            this.langLeft = langLeft;
            this.langRight = langRight;
            LangSound = langSound;
        }

        protected override void SaveSettings() {
            base.SaveSettings();
            Settings.Instance.LangSound = LangSound;
            Settings.Instance.LangLeft = LangLeft;
            Settings.Instance.LangRight = LangRight;
            Settings.Instance.IsLangLeft = MediaTextViewModel.IsLangLeft;
            Settings.Instance.IsLangRight = MediaTextViewModel.IsLangRight;
            Settings.Instance.TextFontSize = MediaTextViewModel.FontSize;
            Settings.Instance.Times = Times;
            Settings.Instance.IsCtrpnlHide = IsCtrpnlHide;
        }
    }
}
