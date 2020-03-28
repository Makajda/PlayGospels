using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace PlayGospels.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FormsApplicationPage {
        public MainWindow() {
            InitializeComponent();
            Forms.Init();
            LoadApplication(new PlayGospels.App());
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape) Close();
        }
    }
}
