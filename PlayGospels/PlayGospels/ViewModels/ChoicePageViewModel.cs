using PlayGospels.Common;
using PlayGospels.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayGospels.ViewModels {
    public class ChoicePageViewModel : ViewModelBase {
        public ChoicePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService, eventAggregator) {

            AllChaptersCommand = new DelegateCommand(() => CurrentBook?.SetChaptersIsSelected(true));
            NoneChaptersCommand = new DelegateCommand(() => CurrentBook?.SetChaptersIsSelected(false));
            ToMediaCommand = new DelegateCommand(ToMedia);
        }
        public DelegateCommand AllChaptersCommand { get; private set; }
        public DelegateCommand NoneChaptersCommand { get; private set; }
        public DelegateCommand ToMediaCommand { get; private set; }

        private IEnumerable<Book> books;
        public IEnumerable<Book> Books {
            get => books;
            set => SetProperty(ref books, value);
        }

        private Book currentBook;
        public Book CurrentBook {
            get => currentBook;
            set => SetProperty(ref currentBook, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters) {
            base.OnNavigatedTo(parameters);
            if (parameters.TryGetValue(Given.LangSoundParameterName, out string lang)) {
                foreach (var book in Books) book.CheckLang(lang);
            }

            if (parameters.TryGetValue(Given.RecalcDurationParameterName, out bool recalcDuration)) {
                if (recalcDuration) foreach (var book in Books) book.RecalcDuration();
            }

            if (parameters.TryGetValue(Given.EndedChaptersParameterName, out List<Chapter> endedChapters)) {
                if (endedChapters != null && CurrentBook != null) {
                    foreach (var chapter in CurrentBook.Chapters) {
                        if (endedChapters.Contains(chapter)) {
                            chapter.IsSelected = false;
                        }
                    }
                }
            }
        }

        private async void ToMedia() {
            if (CurrentBook == null) {
                return;
            }

            var selected = CurrentBook.Chapters.Where(n => n.IsSelected).ToArray();
            if (selected == null || selected.Length == 0) {
                selected = CurrentBook.Chapters.Take(1).ToArray();
            }

            var parameters = new NavigationParameters {
                { Given.BookFolderParameterName, CurrentBook.Folder },
                { Given.ChaptersParameterName, selected }
            };

            await NavigationService.NavigateAsync(Given.MediaViewName, parameters);
        }

        protected override void RestoreSettings() {
            base.RestoreSettings();
            var lang = Settings.Instance.LangSound;
            if (string.IsNullOrWhiteSpace(lang)) {
                lang = Given.Langs.ElementAt(1);
            }

            Books = Given.Books.Select(n => new Book(lang, n.Folder, n.CountChapters)).ToList();
            CurrentBook = Books?.FirstOrDefault(n => n.Folder == Settings.Instance.Book);
        }
        protected override void SaveSettings() {
            base.SaveSettings();
            Settings.Instance.Book = CurrentBook?.Folder;
            foreach (var book in Books) {
                book.SaveSettings();
            }
        }
    }
}
