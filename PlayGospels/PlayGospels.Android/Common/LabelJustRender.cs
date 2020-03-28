using Android.Content;
using Android.Text;
using PlayGospels.Common;
using PlayGospels.Droid.Common;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LabelJust), typeof(LabelJustRender))]

namespace PlayGospels.Droid.Common {
    public class LabelJustRender : LabelRenderer {
        public LabelJustRender(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e) {
            base.OnElementChanged(e);

            if (Control != null) {
                Control.JustificationMode = JustificationMode.InterWord;
            }
        }
    }
}