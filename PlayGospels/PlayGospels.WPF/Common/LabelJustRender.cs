using PlayGospels.Common;
using PlayGospels.WPF.Common;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using TextAlignment = System.Windows.TextAlignment;

[assembly: ExportRenderer(typeof(LabelJust), typeof(LabelJustRender))]

namespace PlayGospels.WPF.Common {
    public class LabelJustRender : LabelRenderer {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e) {
            base.OnElementChanged(e);

            if (Control != null) {
                Control.TextAlignment = TextAlignment.Justify;
            }
        }
    }
}
