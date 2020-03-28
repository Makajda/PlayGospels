using PlayGospels.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlayGospels.ViewModels {
    public class DownloadPageViewModel : BindableBase, INavigationAware {
        private INavigationService navigationService;
        private CancellationTokenSource cancellationTokenSource;
        private int langValue;
        private readonly int langMaximum;
        private int bookValue;
        private int bookMaximum;

        public DownloadPageViewModel(INavigationService navigationService) {
            this.navigationService = navigationService;
            CancelCommand = new DelegateCommand(Cancel);
            langMaximum = Given.Books.Sum(n => n.CountChapters);
        }

        public DelegateCommand CancelCommand { get; private set; }

        private string lang;
        public string Lang {
            get => lang;
            set => SetProperty(ref lang, value);
        }
        private string book;
        public string Book {
            get => book;
            set => SetProperty(ref book, value, () => BookTitle = Given.GetBookTitle(Lang, Book));
        }
        private string chapter;
        public string Chapter {
            get => chapter;
            set => SetProperty(ref chapter, value);
        }
        private string bookTitle;
        public string BookTitle {
            get => bookTitle;
            set => SetProperty(ref bookTitle, value);
        }

        private double bookProgress;
        public double BookProgress {
            get => bookProgress;
            set => SetProperty(ref bookProgress, value);
        }

        private double langProgress;
        public double LangProgress {
            get => langProgress;
            set => SetProperty(ref langProgress, value);
        }

        private string error;
        public string Error {
            get { return error; }
            set { SetProperty(ref error, value); }
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public async void OnNavigatedTo(INavigationParameters parameters) {
            if (parameters.TryGetValue(Given.LangSoundParameterName, out string lang)) {
                Lang = lang;
            }

            if (parameters.TryGetValue(Given.BookFolderParameterName, out string book)) {
                Book = book;
            }

            if (parameters.TryGetValue(Given.ChapterParameterName, out string chapter)) {
                Chapter = chapter;
            }

            if (parameters.TryGetValue(Given.WhatDownloadParameterName, out string what)) {
                await LoadSound(what);
            }
        }

        private async Task LoadSound(string what) {
            bookMaximum = Given.Books.FirstOrDefault(n => n.Folder == Book).CountChapters;
            langValue = 0;
            try {
                foreach (var folder in Given.Books.Select(n => n.Folder)) {
                    var path = Path.Combine(Given.PathToData, Lang, folder);
                    Directory.CreateDirectory(Path.Combine(path));
                    langValue += Directory.GetFiles(path).Count();
                }

                bookValue = Directory.GetFiles(Path.Combine(Given.PathToData, Lang, Book)).Count();
            }
            catch (Exception) { }

            CalcProgress();
            Error = null;
            cancellationTokenSource = new CancellationTokenSource();
            switch (what) {
                case "1":
                    await LoadSoundOne();
                    break;
                case "2":
                    await LoadSoundBook();
                    break;
                case "3":
                    await LoadSoundLang();
                    break;
            }

            await navigationService.GoBackAsync();
        }

        private async Task LoadSoundLang() {
            foreach (var book in Given.Books) {
                Book = book.Folder;
                bookMaximum = book.CountChapters;
                try {
                    bookValue = Directory.GetFiles(Path.Combine(Given.PathToData, Lang, Book)).Count();
                }
                catch (Exception) { }

                CalcProgress();
                await LoadSoundBook();
            }
        }

        private async Task LoadSoundBook() {
            for (int i = 0; i < bookMaximum; i++) {
                var chapter = (i + 1).ToString();
                var file = Given.GetFileSoundFullName(Lang, Book, chapter);
                var exist = true;
                try {
                    exist = File.Exists(file);
                }
                catch (Exception) { }

                if (!exist) {
                    Chapter = chapter;
                    await LoadSoundOne();
                }
            }
        }

        private async Task LoadSoundOne() {
            var web = Given.GetWebFullName(Lang, Book, Chapter);
            var file = Given.GetFileSoundFullName(Lang, Book, Chapter);
            try {
                await Utilities.LoadToFile(web, file, cancellationTokenSource.Token);
                langValue++;
                bookValue++;
                CalcProgress();
            }
            catch (Exception e) {
                Error += e.Message;
                try {
                    File.Delete(file);
                }
                catch (Exception) { }
            }
        }

        private void CalcProgress() {
            BookProgress = bookMaximum == 0 ? 0 : (double)bookValue / bookMaximum;
            LangProgress = langMaximum == 0 ? 0 : (double)langValue / langMaximum;
        }

        private async void Cancel() {
            cancellationTokenSource?.Cancel();
            await navigationService.GoBackAsync();
        }
    }
}
