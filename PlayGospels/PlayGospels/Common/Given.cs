using ATL;
using PlayGospels.Views;
using System;
using System.IO;
using System.Reflection;

namespace PlayGospels.Common {
    public static class Given {
        public const string BookFolderParameterName = "BookFolderParameter";
        public const string ChaptersParameterName = "ChaptersParameter";
        public const string ChapterParameterName = "ChapterParameter";
        public const string LangSoundParameterName = "LangSoundParameter";
        public const string EndedChaptersParameterName = "EndedChaptersParameter";
        public const string RecalcDurationParameterName = "RecalcDurationParameter";
        public const string WhatDownloadParameterName = "WhatDownloadParameter";

        public static readonly string ChoiceViewName = typeof(ChoicePage).Name;
        public static readonly string MediaViewName = typeof(MediaPage).Name;
        public static readonly string DownloadViewName = typeof(DownloadPage).Name;

        public static readonly string PathToData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        public static readonly string[] Langs = { "De", "En", "Es", "Fr", "La", "Pt", "Ru" };
        public static (string Folder, int CountChapters)[] Books = { ("40", 28), ("41", 16), ("42", 24), ("43", 21) };

        public static string GetFileSoundFullName(string lang, string book, string chapter) => Path.ChangeExtension(Path.Combine(PathToData, lang, book, chapter), "mp3");
        public static string GetWebFullName(string lang, string book, string chapter) => $"{Given.GetHttpAddress(lang)}{book}/{chapter}.mp3";
        public static string GetResourceTextFullName(string lang, string book) => $"{typeof(App).GetTypeInfo().Namespace}.Data.{lang}.{book}.zip";
        public static string GetEntryName(string chapter) => $"{chapter}.txt";
        public static double GetDuration(string lang, string book, string chapter) {
            var track = new Track(GetFileSoundFullName(lang, book, chapter));
            return track.Duration;
        }
        public static string GetInTheBeginning(string lang) {
            switch (lang) {
                case "De": return "In den beginne was het Woord, en het Woord was bij God, en het Woord was God.";
                case "En": return "In the beginning was the Word, and the Word was with God, and the Word was God.";
                case "Es": return "EN el principio era el Verbo, y el Verbo era con Dios, y el Verbo era Dios.";
                case "Fr": return "Au commencement était la Parole, et la Parole était avec Dieu, et la Parole était Dieu.";
                case "La": return "in principio erat Verbum et Verbum erat apud Deum et Deus erat Verbum";
                case "Pt": return "No princípio era o Verbo, e o Verbo estava com Deus, e o Verbo era Deus.";
                case "Ru": return "В начале было Слово, и Слово было у Бога, и Слово было Бог.";
                default: return null;
            };
        }
        public static string GetBookTitle(string lang, string folder) {
            switch (lang) {
                case "De":
                    switch (folder) {
                        case "40": return "Matthäus";
                        case "41": return "Markus";
                        case "42": return "Lukas";
                        case "43": return "Johannes";
                        default: return null;
                    }
                case "En":
                    switch (folder) {
                        case "40": return "Matthew";
                        case "41": return "Mark";
                        case "42": return "Luke";
                        case "43": return "John";
                        default: return null;
                    }
                case "Es":
                    switch (folder) {
                        case "40": return "Mateo";
                        case "41": return "Marcos";
                        case "42": return "Lucas";
                        case "43": return "Juan";
                        default: return null;
                    }
                case "Fr":
                    switch (folder) {
                        case "40": return "Matthieu";
                        case "41": return "Marc";
                        case "42": return "Luc";
                        case "43": return "Jean";
                        default: return null;
                    }
                case "La":
                    switch (folder) {
                        case "40": return "Matthæum";
                        case "41": return "Marcum";
                        case "42": return "Lucam";
                        case "43": return "Ioannem";
                        default: return null;
                    }
                case "Pt":
                    switch (folder) {
                        case "40": return "Mateus";
                        case "41": return "Marcos";
                        case "42": return "Lucas";
                        case "43": return "João";
                        default: return null;
                    }
                case "Ru":
                    switch (folder) {
                        case "40": return "Матфея";
                        case "41": return "Марка";
                        case "42": return "Луки";
                        case "43": return "Иоанна";
                        default: return null;
                    }
                default: return null;
            };
        }
        private static string GetHttpAddress(string lang) {
            switch (lang) {
                case "De": return "http://wpkorg.wordproject.com/bibles/app/audio/9/";
                case "En": return "http://wordfree.org/audio_kjv/1/";
                case "Es": return "http://wpanet.wordproject.com/bibles/app/audio/6/";
                case "Fr": return "http://wpanet.wordproject.com/bibles/app/audio/7/";
                case "La": return "http://wpaorg.wordproject.com/bibles/app/audio/41/";
                case "Pt": return "http://wpanet.wordproject.com/bibles/app/audio/2/";
                case "Ru": return "http://wpkorg.wordproject.com/bibles/app/audio/8/";
                default: return null;
            };
        }
    }
}
