using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.NetworkInformation;

using Windows.UI.Xaml;

using AppStudio.Services;
using AppStudio.Data;

namespace AppStudio.ViewModels
{
    public class MainViewModel : BindableBase
    {
       private TimesOfIndiaViewModel _timesOfIndiaModel;
       private TheHinduViewModel _theHinduModel;
       private HindustanTimesViewModel _hindustanTimesModel;
        private PrivacyViewModel _privacyModel;

        private ViewModelBase _selectedItem = null;

        public MainViewModel()
        {
            _selectedItem = TimesOfIndiaModel;
            _privacyModel = new PrivacyViewModel();

        }
 
        public TimesOfIndiaViewModel TimesOfIndiaModel
        {
            get { return _timesOfIndiaModel ?? (_timesOfIndiaModel = new TimesOfIndiaViewModel()); }
        }
 
        public TheHinduViewModel TheHinduModel
        {
            get { return _theHinduModel ?? (_theHinduModel = new TheHinduViewModel()); }
        }
 
        public HindustanTimesViewModel HindustanTimesModel
        {
            get { return _hindustanTimesModel ?? (_hindustanTimesModel = new HindustanTimesViewModel()); }
        }

        public void SetViewType(ViewTypes viewType)
        {
            TimesOfIndiaModel.ViewType = viewType;
            TheHinduModel.ViewType = viewType;
            HindustanTimesModel.ViewType = viewType;
        }

        public ViewModelBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                UpdateAppBar();
            }
        }

        public Visibility AppBarVisibility
        {
            get
            {
                return SelectedItem == null ? AboutVisibility : SelectedItem.AppBarVisibility;
            }
        }

        public Visibility AboutVisibility
        {

         get { return Visibility.Visible; }
        }

        public void UpdateAppBar()
        {
            OnPropertyChanged("AppBarVisibility");
            OnPropertyChanged("AboutVisibility");
        }

        /// <summary>
        /// Load ViewModel items asynchronous
        /// </summary>
        public async Task LoadData(bool isNetworkAvailable)
        {
            var loadTasks = new Task[]
            { 
                TimesOfIndiaModel.LoadItems(isNetworkAvailable),
                TheHinduModel.LoadItems(isNetworkAvailable),
                HindustanTimesModel.LoadItems(isNetworkAvailable),
            };
            await Task.WhenAll(loadTasks);
        }

        /// <summary>
        /// Refresh ViewModel items asynchronous
        /// </summary>
        public async Task RefreshData(bool isNetworkAvailable)
        {
            var refreshTasks = new Task[]
            { 
                TimesOfIndiaModel.RefreshItems(isNetworkAvailable),
                TheHinduModel.RefreshItems(isNetworkAvailable),
                HindustanTimesModel.RefreshItems(isNetworkAvailable),
            };
            await Task.WhenAll(refreshTasks);
        }

        //
        //  ViewModel command implementation
        //
        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await RefreshData(NetworkInterface.GetIsNetworkAvailable());
                });
            }
        }

        public ICommand AboutCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    NavigationServices.NavigateToPage("AboutThisAppPage");
                });
            }
        }

        public ICommand PrivacyCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    NavigationServices.NavigateTo(_privacyModel.Url);
                });
            }
        }
    }
}
