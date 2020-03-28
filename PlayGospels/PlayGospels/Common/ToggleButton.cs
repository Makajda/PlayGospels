using Xamarin.Forms;

namespace PlayGospels.Common {
    public class ToggleButton : Label {
        private Color originColor;
        public ToggleButton() {
            HorizontalTextAlignment = TextAlignment.Center;
            VerticalTextAlignment = TextAlignment.Center;

            var tapRecognizer = new TapGestureRecognizer() {
                Command = new Command(() => this.IsToggled = !this.IsToggled)
            };

            GestureRecognizers.Add(tapRecognizer);
        }

        public static readonly BindableProperty ToggleColorProperty = BindableProperty.Create(nameof(ToggleColor), typeof(Color), typeof(ToggleButton));
        public Color ToggleColor {
            set { SetValue(ToggleColorProperty, value); }
            get { return (Color)GetValue(ToggleColorProperty); }
        }

        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled),
            typeof(bool), typeof(ToggleButton), false, BindingMode.TwoWay, null, OnIsToggledChanged);
        public bool IsToggled {
            set { SetValue(IsToggledProperty, value); }
            get { return (bool)GetValue(IsToggledProperty); }
        }
        private static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue) {
            if (newValue is bool value) {
                if (bindable is ToggleButton toggleView) {
                    if (value) {
                        toggleView.originColor = toggleView.BackgroundColor;
                        toggleView.BackgroundColor = toggleView.ToggleColor;
                    }
                    else {
                        toggleView.BackgroundColor = toggleView.originColor;
                    }
                }
            }
        }
    }
}