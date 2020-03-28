using PlayGospels.Repositories;
using Prism.Commands;
using Prism.Mvvm;
using Sdict = System.Collections.Generic.SortedList<int, PlayGospels.Models.Verse>;

namespace PlayGospels.ViewModels {
    public class MediaTextViewModel : BindableBase {
        private readonly VersesRepository versesRepository;
        public MediaTextViewModel(VersesRepository versesRepository) {
            this.versesRepository = versesRepository;
            FontSizeCommand = new DelegateCommand<string>(SetFontSize);
        }

        public DelegateCommand<string> FontSizeCommand { get; private set; }

        private Sdict verses;
        public Sdict Verses {
            get => verses;
            private set => SetProperty(ref verses, value);
        }

        private double fontSize = 18d;
        public double FontSize {
            get => fontSize;
            set => SetProperty(ref fontSize, value);
        }

        private bool isLangLeft = true;
        public bool IsLangLeft {
            get => isLangLeft;
            set => SetProperty(ref isLangLeft, value, () => RaisePropertyChanged(nameof(IsAnyLang)));
        }

        private bool isLangRight = true;
        public bool IsLangRight {
            get => isLangRight;
            set => SetProperty(ref isLangRight, value, () => RaisePropertyChanged(nameof(IsAnyLang)));
        }

        public bool IsAnyLang { get => IsLangLeft || IsLangRight; }

        public void SetVerses(string bookFolder, string chapterName, string langLeft, string langRight) => Verses = versesRepository.GetItems(langLeft, langRight, bookFolder, chapterName);
        public void SetVersesLeft(string lang) => Verses = versesRepository.ChangeLangLeft(Verses, lang);
        public void SetVersesRight(string lang) => Verses = versesRepository.ChangeLangRight(Verses, lang);

        private void SetFontSize(string delta) {
            if (double.TryParse(delta, out double d)) {
                FontSize += d;
            }
        }
    }
}
