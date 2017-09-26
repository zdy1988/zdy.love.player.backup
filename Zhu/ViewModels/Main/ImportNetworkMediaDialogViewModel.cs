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

namespace Zhu.ViewModels.Main
{
    public class ImportNetworkMediaDialogViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        public IMediaService _mediaService { get; set; }

        private bool _isImported;
        public bool IsImported
        {
            get { return _isImported; }
            set { Set(() => IsImported, ref _isImported, value); }
        }

        private bool _isHasData;
        public bool IsHasData
        {
            get { return _isHasData; }
            set { Set(() => IsHasData, ref _isHasData, value); }
        }

        private ObservableCollection<Tuple<Media, bool>> _medias = new ObservableCollection<Tuple<Media, bool>>();
        public ObservableCollection<Tuple<Media, bool>> Medias
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

                    if (!File.Exists(filePath))
                    {
                        Messenger.Default.Send(new ManageExceptionMessage(new Exception("未找到导入文件！")));
                    }
                    else
                    {
                        await Task.Run(async () =>
                        {
                            StreamReader sr = new StreamReader(filePath, Encoding.Default);
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

                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        Medias.Add(new Tuple<Media, bool>(media, false));
                                    });
                                }
                            }
                        });

                        if (Medias.Count > 0)
                        {
                            IsHasData = true;
                        }
                    }
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
                            await _mediaService.InsertAsync(media.Item1);
                        }
                    });

                    IsImported = true;

                    _applicationState.HideLoadingDialog();

                    Messenger.Default.Send(new ManageExceptionMessage(new Exception($"成功导入 {Medias.Count} 条网络视频数据！")));
                }
            });
        }
    }
}
