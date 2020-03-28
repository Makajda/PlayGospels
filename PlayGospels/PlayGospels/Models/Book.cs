using PlayGospels.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PlayGospels.Models {
    public class Book : BindableBase {
        private readonly Lazy<IEnumerable<Chapter>> chapters;
        private string lang;
        public Book(string lang, string folder, int countChapters) {
            this.lang = lang;
            Folder = folder;
            Title = Given.GetBookTitle(lang, folder);

            chapters = new Lazy<IEnumerable<Chapter>>(() => {
                var cs = new Chapter[countChapters];
                for (int i = 0; i < countChapters; i++) {
                    cs[i] = new Chapter(i + 1, this.lang, Folder);
                }

                RestoreSettings(cs);
                foreach (var chapter in cs) {
                    chapter.PropertyChanged += Chapter_PropertyChanged;
                }

                return cs;
            });
        }

        public readonly string Folder;
        public IEnumerable<Chapter> Chapters { get => chapters.Value; }

        private string title;
        public string Title {
            get => title;
            private set => SetProperty(ref title, value);
        }

        private TimeSpan duration;
        public TimeSpan Duration {
            get => duration;
            set => SetProperty(ref duration, value);
        }

        private bool isDuration;
        public bool IsDuration {
            get => isDuration;
            set => SetProperty(ref isDuration, value);
        }

        public void SetChaptersIsSelected(bool isSelected) {
            foreach (var chapter in Chapters) {
                chapter.PropertyChanged -= Chapter_PropertyChanged;
                chapter.IsSelected = isSelected;
                chapter.PropertyChanged += Chapter_PropertyChanged;
            }

            RecalcDuration();
        }

        public void CheckLang(string lang) {
            if (this.lang != lang) {
                this.lang = lang;
                Title = Given.GetBookTitle(lang, Folder);
                if (chapters.IsValueCreated) {
                    foreach (var chapter in Chapters) {
                        chapter.ResetDuration(this.lang, Folder);
                    }
                }
            }
        }

        private void Chapter_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Chapter.IsSelected)) {
                RecalcDuration();
            };
        }

        public void RecalcDuration() {
            var total = 0d;
            if (chapters.IsValueCreated) {
                var selected = Chapters.Where(n => n.IsSelected);
                foreach (var chapter in selected) {
                    if (chapter.Duration > 0d) {
                        total += chapter.Duration;
                    }
                    else {
                        total = 0d;
                        break;
                    }
                }
            }

            IsDuration = total > double.Epsilon;
            Duration = TimeSpan.FromSeconds(total);
        }

        private void RestoreSettings(IEnumerable<Chapter> chapters) {
            if (Settings.Instance.ChaptersSelected != null) {
                if (Settings.Instance.ChaptersSelected.TryGetValue(Folder, out string[] names)) {
                    if (names != null) {
                        foreach (var chapter in chapters) {
                            chapter.IsSelected = names.Contains(chapter.Name);
                        }
                    }
                }
            }
        }

        public void SaveSettings() {
            if (chapters.IsValueCreated) {
                var selected = Chapters.Where(n => n.IsSelected).Select(n => n.Name).ToArray();
                if (selected.Any()) {
                    if (Settings.Instance.ChaptersSelected == null) {
                        Settings.Instance.ChaptersSelected = new Dictionary<string, string[]>();
                    }

                    Settings.Instance.ChaptersSelected[Folder] = selected;
                }
            }
        }
    }
}
