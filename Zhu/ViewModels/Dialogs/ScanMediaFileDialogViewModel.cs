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
using System.Windows.Forms;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;
using Zhu.ViewModels.Reused;

namespace Zhu.ViewModels.Dialogs
{
    public class ScanMediaFileDialogViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        public IMediaService _mediaService { get; set; }

        public ScanMediaFileDialogViewModel(IApplicationState applicationState,
            IMediaService mediaService)
        {
            _applicationState = applicationState;
            _mediaService = mediaService;

            RegisterCommands();
        }

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

        public RelayCommand ScanMediaFileCommand { get; private set; }

        public RelayCommand ImportMediaFileDataCommand { get; private set; }

        private void RegisterCommands()
        {
            ScanMediaFileCommand = new RelayCommand(async () =>
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string foldPath = dialog.SelectedPath;

                    TransitionerIndex = 1;

                    Medias = await Task.Run(async () =>
                    {
                        DirectoryInfo theFolder = new DirectoryInfo(foldPath);
                        FileInfo[] dirInfo = new FileInfo[] { };
                        try
                        {
                            dirInfo = theFolder.GetFiles("*", SearchOption.AllDirectories);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                        List<Tuple<Media, ListItemIsSelectViewModel>> medias = new List<Tuple<Media, ListItemIsSelectViewModel>>();

                        //遍历文件
                        foreach (FileInfo file in dirInfo)
                        {
                            if (Untils.Constants.IsValidVedioFormat(file.Extension))
                            {
                                //大文件计算MD5太慢，暂用这个代替，之后用来检测文件唯一性
                                var md5 = Convert.ToBase64String(Encoding.UTF8.GetBytes(file.FullName.ToLower()));

                                //var mediaInfo = FFmpeg.FFmpegHelper.GetMediaInfo(file.FullName);

                                var media = new Media
                                {
                                    MD5 = md5,
                                    Title = file.Name,
                                    MediaType = (int)PubilcEnum.MediaType.Video,
                                    MediaSource = file.FullName,
                                    MediaSourceType = (int)PubilcEnum.MediaSourceType.LocalFile,
                                    IsFavorite = false,
                                    EnterDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    
                                    Duration = 1000// mediaInfo.Duration
                                };

                                medias.Add(new Tuple<Media, ListItemIsSelectViewModel>(media, new ListItemIsSelectViewModel { IsSelected = true }));

                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    ScaningTootips = file.FullName;
                                });
                            }
                        }

                        await Task.Delay(1000);

                        return medias;
                    }).ConfigureAwait(false);

                    ScaningTootips = "";
                    TransitionerIndex = 2;

                    IsHasData = Medias.Any();
                }
            });

            ImportMediaFileDataCommand = new RelayCommand(async () =>
            {

                if (Medias.Count() > 0)
                {
                    _applicationState.ShowLoadingDialog();

                    await Task.Run(async () =>
                    {
                        foreach (var media in Medias.Where(t => t.Item2.IsSelected))
                        {
                            //重复的文件不插入
                            if (await _mediaService.GetEntitiesCountAsync(t => t.MD5 == media.Item1.MD5) <= 0)
                            {
                                await _mediaService.InsertAsync(media.Item1);
                            }
                        }
                    });

                    _applicationState.HideLoadingDialog();

                    Messenger.Default.Send(new ManageExceptionMessage(new Exception($"成功加入 {Medias.Count} 个本地视频数据！")));

                    Messenger.Default.Send(new RefreshVideoListMessage());
                }

            });
        }
    }
}
