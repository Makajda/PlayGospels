using PlayGospels.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace PlayGospels.Models {
    public class Chapter : BindableBase {
        private string lang;
        private string book;

        public Chapter(int num, string lang, string book) {
            Name = num.ToString();
            this.lang = lang;
            this.book = book;
        }

        public string Name { get; private set; }

        private IEnumerable<Verse> verses;
        public IEnumerable<Verse> Verses {
            get => verses;
            set => SetProperty(ref verses, value);
        }
        private double duration = -1d;
        public double Duration {
            get {
                if (duration < 0) duration = Given.GetDuration(lang, book, Name);
                return duration;
            }
            private set => SetProperty(ref duration, value);
        }

        private bool isSelected;
        public bool IsSelected {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public void ResetDuration(string lang, string book) {
            this.lang = lang;
            this.book = book;
            Duration = -1;
        }
    }
}
