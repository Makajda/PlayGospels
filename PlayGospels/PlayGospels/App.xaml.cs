using PlayGospels.Common;
using PlayGospels.ViewModels;
using PlayGospels.Views;
using Prism;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PlayGospels {
    public partial class App {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized() {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/ChoicePage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<ChoicePage, ChoicePageViewModel>();
            containerRegistry.RegisterForNavigation<MediaPage, MediaPageViewModel>();
            containerRegistry.RegisterForNavigation<DownloadPage, DownloadPageViewModel>();
        }

        //hack only for load WPF project
        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterInstance<IContainerExtension>(Container as IContainerExtension);
            containerRegistry.RegisterSingleton<ILoggerFacade, EmptyLogger>();
            containerRegistry.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            containerRegistry.RegisterSingleton<IApplicationStore, ApplicationStore>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<IPageDialogService, PageDialogService>();
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
            containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            containerRegistry.RegisterSingleton<IModuleCatalog, ModuleCatalog>();
            //containerRegistry.RegisterSingleton<IModuleManager, ModuleManagerDump>();
            containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.Register<INavigationService, PageNavigationService>(NavigationServiceName);
        }

        protected override void OnResume() {
            base.OnResume();
        }

        protected override void OnSleep() {
            base.OnSleep();
            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<AppExitEvent>().Publish();
            Settings.Instance.Save();
        }
    }
}
