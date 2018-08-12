using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;
using Zhu.Untils;

namespace Zhu.ViewModels.Dialogs
{
    public class ImportNetworkMediaDialogViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        public IMediaService _mediaService { get; set; }

        private int _transitionerIndex;
        public int TransitionerIndex
        {
            get { return _transitionerIndex; }
            set { Set(() => TransitionerIndex, ref _transitionerIndex, value); }
        }

        private string _scaningTootips;
        public string ScaningTootips
        {
            get { return _scaningTootips; }
            set { Set(() => ScaningTootips, ref _scaningTootips, value); }
        }

        private bool _isHasData;
        public bool IsHasData
        {
            get { return _isHasData; }
            set { Set(() => IsHasData, ref _isHasData, value); }
        }

        private List<Media> _medias = new List<Media>();
        public List<Media> Medias
        {
            get { return _medias; }
            set { Set(() => Medias, ref _medias, value); }
        }

        public ImportNetworkMediaDialogViewModel(IApplicationState applicationState,
            IMediaService mediaService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;

            RegisterCommands();
        }

        public RelayCommand ReadMediaSourceFileCommand { get; private set; }

        public RelayCommand ImportMediaSourceCommand { get; private set; }

        private void RegisterCommands()
        {
            ReadMediaSourceFileCommand = new RelayCommand(async () =>
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = Constants.TxtFilters;
                if (dialog.ShowDialog() == true)
                {
                    string filePath = dialog.FileName;

                    TransitionerIndex = 1;

                    Medias = await Task.Run(async () =>
                    {
                        List<Media> medias = new List<Media>();

                        StreamReader sr = new StreamReader(filePath, Encoding.Default);
                        string line;
                        while ((line = await sr.ReadLineAsync()) != null)
                        {
                            if (line.IndexOf(",") != -1)
                            {
                                string[] item = line.Split(',');

                                await Task.Delay(100);

                                var media = new Media
                                {
                                    Title = item[0],
                                    MediaType = (int)PubilcEnum.MediaType.NetTV,
                                    MediaSource = item[1],
                                    MediaSourceType = (int)PubilcEnum.MediaSourceType.NetworkStream,
                                    IsFavorite = false,
                                    EnterDate = DateTime.Now,
                                    UpdateDate = DateTime.Now
                                };

                                medias.Add(media);

                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    ScaningTootips = item[0];
                                });
                            }
                        }

                        return medias;
                    }).ConfigureAwait(false);

                    ScaningTootips = "";
                    TransitionerIndex = 2;

                    IsHasData = Medias.Any();
                }
            });

            ImportMediaSourceCommand = new RelayCommand(async () =>
            {
                if (Medias.Count() > 0)
                {
                    _applicationState.ShowLoadingDialog();

                    await Task.Run(async () =>
                    {
                        foreach (var media in Medias)
                        {
                            await _mediaService.InsertAsync(media);
                        }
                    });

                    _applicationState.HideLoadingDialog();

                    Messenger.Default.Send(new ManageExceptionMessage(new Exception($"成功导入 {Medias.Count} 条网络视频源！")));

                    Messenger.Default.Send(new RefreshNetTVListMessage());
                }
            });
        }
    }
}
