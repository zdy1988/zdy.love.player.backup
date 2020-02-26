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
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;
using ZDY.LovePlayer.Services;
using ZDY.LovePlayer.Untils;
using ZDY.LovePlayer.ViewModels.Reused;

namespace ZDY.LovePlayer.ViewModels.Dialogs
{
    public class ImportNetworkStreamDialogViewModel : ViewModelBase
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

        private List<Tuple<Media, ListItemIsSelectViewModel>> _medias = new List<Tuple<Media, ListItemIsSelectViewModel>>();
        public List<Tuple<Media, ListItemIsSelectViewModel>> Medias
        {
            get { return _medias; }
            set { Set(() => Medias, ref _medias, value); }
        }

        public ImportNetworkStreamDialogViewModel(IApplicationState applicationState,
            IMediaService mediaService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;

            RegisterCommands();
        }

        public RelayCommand ReadMediaSourceFileCommand { get; private set; }

        public RelayCommand ImportNetworkStreamCommand { get; private set; }

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
                        List<Tuple<Media, ListItemIsSelectViewModel>> medias = new List<Tuple<Media, ListItemIsSelectViewModel>>();

                        StreamReader sr = new StreamReader(filePath, Encoding.Default);
                        string line;
                        while ((line = await sr.ReadLineAsync()) != null)
                        {
                            if (line.IndexOf("|") != -1)
                            {
                                string[] item = line.Split('|');

                                await Task.Delay(100);

                                var md5 = Convert.ToBase64String(Encoding.UTF8.GetBytes(item[1].ToLower()));

                                var media = new Media
                                {
                                    MD5 = md5,
                                    Title = item[0],
                                    MediaType = (int)PubilcEnum.MediaKind.NetTV,
                                    MediaSource = item[1],
                                    MediaSourceType = (int)PubilcEnum.MediaSourceKind.NetworkStream,
                                    IsFavorite = false,
                                    EnterDate = DateTime.Now,
                                    UpdateDate = DateTime.Now
                                };

                                medias.Add(new Tuple<Media, ListItemIsSelectViewModel>(media, new ListItemIsSelectViewModel { IsSelected = true }));

                                DispatcherHelper.CheckBeginInvokeOnUI(() => ScaningTootips = item[0]);
                            }
                        }

                        return medias;
                    }).ConfigureAwait(false);

                    ScaningTootips = "";
                    TransitionerIndex = 2;

                    IsHasData = Medias.Any();
                }
            });

            ImportNetworkStreamCommand = new RelayCommand(async () =>
            {
                if (Medias.Count() > 0)
                {
                    _applicationState.ShowLoadingDialog();

                    int count = 0;

                    await Task.Run(async () =>
                    {
                        foreach (var media in Medias)
                        {
                            if (await _mediaService.GetEntitiesCountAsync(t => t.MD5 == media.Item1.MD5) <= 0)
                            {
                                count++;
                                await _mediaService.InsertAsync(media.Item1);
                            }
                        }
                    });

                    _applicationState.HideLoadingDialog();

                    Messenger.Default.Send(new ManageExceptionMessage(new Exception($"排除重复项后成功导入 {count} 条网络视数据！")));

                    Messenger.Default.Send(new RefreshNetTVListMessage());
                }
            });
        }
    }
}
