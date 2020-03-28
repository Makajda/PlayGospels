using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGospels.Common {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutView : ContentView {
        public AboutView() {
            InitializeComponent();
        }

        private async void ButtonWord_Click(object sender, EventArgs e) {
            var x= await Launcher.TryOpenAsync(new Uri("https://www.wordproject.org/"));
        }

        private async void ButtonGit_Click(object sender, EventArgs e) {
            await Launcher.TryOpenAsync(new Uri("https://github.com/makayda/PlayGospels"));
        }
    }
}