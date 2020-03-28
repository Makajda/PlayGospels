using PlayGospels.Common;
using PlayGospels.UWP.Common;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using TextAlignment = Windows.UI.Xaml.TextAlignment;

[assembly: ExportRenderer(typeof(LabelJust), typeof(LabelJustRender))]

namespace PlayGospels.UWP.Common {
    public class LabelJustRender : LabelRenderer {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e) {
            base.OnElementChanged(e);

            if (Control != null) {
                Control.TextAlignment = TextAlignment.Justify;
            }
        }
    }
}
