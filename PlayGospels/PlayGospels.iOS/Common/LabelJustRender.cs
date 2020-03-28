using PlayGospels.Common;
using PlayGospels.iOS.Common;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LabelJust), typeof(LabelJustRender))]

namespace PlayGospels.iOS.Common {
    public class LabelJustRender : LabelRenderer {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e) {
            base.OnElementChanged(e);

            if (Control != null) {
                Control.TextAlignment = UITextAlignment.Justified;
            }
        }
    }
}