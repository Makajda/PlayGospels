using System;
using System.Collections;
using Xamarin.Forms;

namespace PlayGospels.Common {
    class ItemsControl : ScrollView {
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            "ItemTemplate", typeof(DataTemplate), typeof(ItemsControl), null, propertyChanged: (bindable, value, newValue) => Populate(bindable));

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            "ItemsSource", typeof(IEnumerable), typeof(ItemsControl), null, BindingMode.OneWay,
            propertyChanged: (bindable, value, newValue) => Populate(bindable));

        public IEnumerable ItemsSource {
            get => (IEnumerable)this.GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate {
            get => (DataTemplate)this.GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        private static void Populate(BindableObject bindable) {
            var repeater = (ItemsControl)bindable;

            // Clean
            repeater.Content = null;

            // Only populate once both properties are recieved
            if (repeater.ItemsSource == null || repeater.ItemTemplate == null) {
                return;
            }

            // Create a stack to populate with items
            var list = new StackLayout();

            foreach (var viewModel in repeater.ItemsSource) {
                var content = repeater.ItemTemplate.CreateContent();
                if (!(content is View) && !(content is ViewCell)) {
                    throw new Exception($"Invalid visual object {nameof(content)}");
                }

                var view = content is View ? content as View : ((ViewCell)content).View;
                view.BindingContext = viewModel;

                list.Children.Add(view);
            }

            // Set stack as conent to this ScrollView
            repeater.Content = list;
        }
    }
}