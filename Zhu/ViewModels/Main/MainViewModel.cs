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
using Zhu.UserControls.Reused;
using GalaSoft.MvvmLight.Threading;

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

        private IMediaService _mediaService;

        public MainViewModel(IApplicationState applicationState,
            IMediaService mediaService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;

            RegisterMessages();
            RegisterCommands();
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
            Messenger.Default.Register<MediaFlyoutOpenMessage>(this, e =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsMovieFlyoutOpen = true;
                });
            });
            Messenger.Default.Register<MediaFlyoutCloseMessage>(this, e =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsMovieFlyoutOpen = false;
                });
            });
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
                    eventArgs.Session.UpdateContent(new SampleLoading());
                    await Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(t =>
                    {
                        var vm = view.DataContext as SelectMediaFileDialogViewModel;
                        if (vm != null)
                        {
                            string mediaSource = string.IsNullOrEmpty(vm.FilePath) ? vm.NetworkAddress : vm.FilePath;
                            string mediaTitle = string.IsNullOrEmpty(vm.FilePath) ? vm.NetworkAddress : vm.FilePath.Substring(vm.FilePath.LastIndexOf("\\") + 1);
                            Messenger.Default.Send(new LoadMediaMessage(new Media
                            {
                                ID = 0,
                                Title = mediaTitle,
                                MediaSource = mediaSource
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
                    eventArgs.Session.UpdateContent(new SampleLoading());
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
                                    StreamReader sr = new StreamReader(vm.FilePath, Encoding.Default);
                                    int count = 0;
                                    string line;
                                    while ((line = await sr.ReadLineAsync()) != null)
                                    {
                                        if (line.IndexOf(",") != -1)
                                        {
                                            string[] item = line.Split(',');
                                            var media = new Media
                                            {
                                                Title = item[0],
                                                MediaType = (int)PubilcEnum.MediaType.NetTV,
                                                MediaSource = item[1],
                                                MediaSourceType = (int)PubilcEnum.MediaSourceType.NetworkAddress,
                                                IsFavorite = false,
                                                EnterDate = DateTime.Now,
                                                UpdateDate = DateTime.Now
                                            };
                                            await _mediaService.InsertAsync(media);
                                            count++;
                                        }
                                    }

                                    Messenger.Default.Send(new ManageExceptionMessage(new Exception($"已导入{count}条！")));
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
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.IsMovieFlyoutOpen = !this.IsMovieFlyoutOpen;
                });
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