using PlayGospels.Common;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;

namespace PlayGospels.ViewModels {
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible {
        protected INavigationService NavigationService { get; private set; }
        protected IEventAggregator EventAggregator { get; private set; }

        public ViewModelBase(INavigationService navigationService, IEventAggregator eventAggregator) {
            NavigationService = navigationService;
            EventAggregator = eventAggregator;
        }

        public virtual void Initialize(INavigationParameters parameters) {
            EventAggregator.GetEvent<AppExitEvent>().Subscribe(SaveSettings);
            RestoreSettings();
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters) {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters) {

        }

        public virtual void Destroy() {
            SaveSettings();
            EventAggregator.GetEvent<AppExitEvent>().Unsubscribe(SaveSettings);
        }

        protected virtual void RestoreSettings() {

        }

        protected virtual void SaveSettings() {

        }
    }
}
