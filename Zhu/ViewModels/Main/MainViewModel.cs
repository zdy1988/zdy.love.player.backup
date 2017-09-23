using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using NLog;
using Zhu.Controls;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Models.ApplicationState;
using Zhu.Services;
using Zhu.UserControls.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhu.ViewModels.Pages;

namespace Zhu.ViewModels.Main
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IApplicationState _applicationState;
        public IApplicationState ApplicationState
        {
            get { return _applicationState; }
            set { Set(() => ApplicationState, ref _applicationState, value); }
        }

        private INetTVService NetTVServices;
        private IMovieService MovieServices;

        public MainViewModel(IApplicationState applicationState,
            IMovieService movieServices,
            INetTVService netTVServices)
        {
            ApplicationState = applicationState;
            MovieServices = movieServices;
            NetTVServices = netTVServices;

            RegisterMessages();
            RegisterCommands();

            Tabs.Add(new NetTVListViewModel(ApplicationState, NetTVServices));
            Tabs.Add(new MovieListViewModel(ApplicationState, MovieServices));
            SelectedTab = Tabs[0];
        }



        private ObservableCollection<object> _tabs = new ObservableCollection<object>();

        public ObservableCollection<object> Tabs
        {
            get { return _tabs; }
            set { Set(() => Tabs, ref _tabs, value); }
        }

        private object _selectedTab;

        public object SelectedTab
        {
            get => _selectedTab;
            set
            {
                Set(() => SelectedTab, ref _selectedTab, value);
            }
        }

        private bool _isMovieFlyoutOpen;

        public bool IsMovieFlyoutOpen
        {
            get { return _isMovieFlyoutOpen; }
            set { Set(() => IsMovieFlyoutOpen, ref _isMovieFlyoutOpen, value); }
        }

        public event EventHandler<string> MessageNotice;

        private void RegisterMessages()
        {
            Messenger.Default.Register<MediaFlyoutOpenMessage>(this, e => IsMovieFlyoutOpen = true);
            Messenger.Default.Register<MediaFlyoutCloseMessage>(this, e => IsMovieFlyoutOpen = false);
            Messenger.Default.Register<ManageExceptionMessage>(this, e =>
            {
                MessageNotice?.Invoke(this, e.UnHandledException.Message);
            });
            Messenger.Default.Register<SelectMediaFileDialogOpenMessage>(this, async (e) =>
            {
                var view = new SelectMediaFileDialog
                {
                    DataContext = new SelectMediaFileDialogViewModel()
                };

                await DialogHost.Show(view, "RootDialog", (object sender, DialogOpenedEventArgs eventArgs) => { }, async (object sender, DialogClosingEventArgs eventArgs) =>
                 {
                     if ((bool)eventArgs.Parameter == false) return;
                     eventArgs.Cancel();
                     eventArgs.Session.UpdateContent(new SampleProgressDialog());
                     await Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(t =>
                     {
                         var vm = view.DataContext as SelectMediaFileDialogViewModel;
                         if (vm != null)
                         {
                             string path = string.IsNullOrEmpty(vm.FilePath) ? vm.StreamNetworkAddress : vm.FilePath;
                             Messenger.Default.Send(new LoadMediaMessage(new Media
                             {
                                 Path = path
                             }));
                         }
                         eventArgs.Session.Close(false);
                     }, TaskScheduler.FromCurrentSynchronizationContext());
                 });
            });
            Messenger.Default.Register<ImportNetworkMediaDialogOpenMessage>(this, async (e) =>
            {
                var view = new ImportNetworkMediaDialog
                {
                    DataContext = new ImportNetworkMediaDialogViewModel()
                };

                await DialogHost.Show(view, "RootDialog", (object sender, DialogOpenedEventArgs eventArgs) => { }, (object sender, DialogClosingEventArgs eventArgs) =>
               {
                   if ((bool)eventArgs.Parameter == false) return;
                   eventArgs.Cancel();
                   eventArgs.Session.UpdateContent(new SampleProgressDialog());
                   Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(async (t) =>
                   {
                       await view.Dispatcher.Invoke(async () =>
                       {
                           var vm = view.DataContext as ImportNetworkMediaDialogViewModel;
                           if (vm != null)
                           {
                               if (!File.Exists(vm.FilePath))
                               {
                                   Messenger.Default.Send(new ManageExceptionMessage(new Exception("未找到导入文件！")));
                               }
                               else
                               {
                                   List<NetTV> netTVList = new List<NetTV>();
                                   StreamReader sr = new StreamReader(vm.FilePath, Encoding.Default);
                                   string line;
                                   while ((line = await sr.ReadLineAsync()) != null)
                                   {
                                       if (line.IndexOf(",") != -1)
                                       {
                                           string[] item = line.Split(',');
                                           netTVList.Add(new NetTV
                                           {
                                               Title = item[0],
                                               StreamNetworkAddress = item[1],
                                               IsFavorite = false,
                                               EnterDate = DateTime.Now,
                                               UpdateDate = DateTime.Now
                                           });
                                       }
                                   }

                                   await NetTVServices.InsertAsync(netTVList);

                                   Messenger.Default.Send(new ManageExceptionMessage(new Exception(
                                       $"已导入{netTVList.Count}条！")
                                       ));
                               }
                               eventArgs.Session.Close(false);
                           }
                       });
                   }, TaskScheduler.FromCurrentSynchronizationContext());
               });
            });
        }

        public RelayCommand ToggleMoviePalyerCommand { get; private set; }

        public RelayCommand SelectMediaDialogOpenCommand { get; private set; }

        public RelayCommand ImportNetworkMediaDialogOpenCommand { get; private set; }

        private void RegisterCommands()
        {
            ToggleMoviePalyerCommand = new RelayCommand(() =>
            {
                this.IsMovieFlyoutOpen = !this.IsMovieFlyoutOpen;
            });

            SelectMediaDialogOpenCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new SelectMediaFileDialogOpenMessage());
            });

            ImportNetworkMediaDialogOpenCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new ImportNetworkMediaDialogOpenMessage());
            });
        }
    }
}