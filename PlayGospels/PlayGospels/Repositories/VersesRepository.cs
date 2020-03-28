using PlayGospels.Common;
using PlayGospels.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using GetVerseProp = System.Func<PlayGospels.Models.Verse, string>;
using Sdict = System.Collections.Generic.SortedList<int, PlayGospels.Models.Verse>;
using SetVerseProp = System.Action<PlayGospels.Models.Verse, string>;

namespace PlayGospels.Repositories {
    public class VersesRepository {
        private string lastBook;
        private string lastChapter;
        private SetVerseProp SetLeftProp => (l, t) => l.Left = t;
        private SetVerseProp SetRightProp => (l, t) => l.Right = t;

        public Sdict GetItems(string langLeft, string langRight, string book, string chapter) {
            lastBook = book;
            lastChapter = chapter;
            var verses = new Sdict();
            FillVerses(verses, langLeft, SetLeftProp);
            FillVerses(verses, langRight, SetRightProp);
            return verses;
        }

        public Sdict ChangeLangLeft(Sdict verses, string lang) => RecreateVerses(verses, lang, SetLeftProp, l => l.Right);

        public Sdict ChangeLangRight(Sdict verses, string lang) => RecreateVerses(verses, lang, SetRightProp, l => l.Left);

        private Sdict RecreateVerses(Sdict verses, string lang, SetVerseProp setVerseProp, GetVerseProp getVerseProp) {
            var newVerses = new Sdict();
            if (verses != null) {
                foreach (var p in verses) {
                    if (!string.IsNullOrWhiteSpace(getVerseProp(p.Value))) {
                        setVerseProp(p.Value, null);
                        newVerses.Add(p.Key, p.Value);
                    }
                }
            }

            FillVerses(newVerses, lang, setVerseProp);
            return newVerses;
        }

        private void FillVerses(Sdict verses, string lang, SetVerseProp setVerseProp) {
            if (string.IsNullOrEmpty(lastChapter)) {
                return;
            }

            string text = null;

            try {
                var stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream(Given.GetResourceTextFullName(lang, lastBook));
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read)) {
                    var entry = archive.GetEntry(Given.GetEntryName(lastChapter));
                    using (var entryStream = entry.Open()) {
                        using (var streamReader = new StreamReader(entryStream)) {
                            text = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e) {
                text = $"0 {e.Message}";
            }

            if (string.IsNullOrWhiteSpace(text)) {
                return;
            }

            var ls = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in ls) {
                var numString = string.Concat<Char>(l.TakeWhile(n => Char.IsDigit(n)));
                if (int.TryParse(numString, out int num)) {
                    var t = l.Substring(numString.Length).TrimStart();
                    if (verses.TryGetValue(num, out Verse verse)) {
                        setVerseProp(verse, t);
                    }
                    else {
                        verse = new Verse();
                        setVerseProp(verse, t);
                        verses.Add(num, verse);
                    }
                }
            }
        }
    }
}
