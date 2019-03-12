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
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZdyLovePlayer.Messaging;
using ZdyLovePlayer.Models;
using ZdyLovePlayer.Services;
using ZdyLovePlayer.Untils;
using ZdyLovePlayer.ViewModels.Reused;

namespace ZdyLovePlayer.ViewModels.Dialogs
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

        private bool _isHasData = false;
        public bool IsHasData
        {
            get { return _isHasData; }
            set { Set(() => IsHasData, ref _isHasData, value); }
        }

        private bool _isImportToMovie = false;
        public bool IsImportToMovie
        {
            get { return _isImportToMovie; }
            set { Set(() => IsImportToMovie, ref _isImportToMovie, value); }
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

                    DirectorySecurity security = new DirectorySecurity(foldPath, AccessControlSections.Access);
                    if (security.AreAccessRulesProtected)
                    {
                        Messenger.Default.Send(new ManageExceptionMessage(new Exception($" {foldPath} 没有访问权限！")));
                        return;
                    }

                    TransitionerIndex = 1;
                    IsImportToMovie = false;

                    Medias = await Task.Run(async () =>
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => ScaningTootips = "开始分析目录...");

                        await Task.Delay(1000);

                        List<FileInfo> fileInfos = new List<FileInfo>();
                        List<string> filePaths = new List<string>();
                        DictionaryHelper.GetFiles(foldPath, filePaths);

                        foreach (var filePath in filePaths)
                        {
                            try
                            {
                                DirectorySecurity directorySecurity = new DirectorySecurity(filePath, AccessControlSections.Access);
                                if (!directorySecurity.AreAccessRulesProtected)
                                {
                                    FileInfo fileInfo = new FileInfo(filePath);
                                    fileInfos.Add(fileInfo);
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                            }
                        }

                        DispatcherHelper.CheckBeginInvokeOnUI(() => ScaningTootips = "开始分析文件...");

                        await Task.Delay(1000);

                        List<Tuple<Media, ListItemIsSelectViewModel>> medias = new List<Tuple<Media, ListItemIsSelectViewModel>>();

                        //遍历文件
                        foreach (FileInfo file in fileInfos)
                        {
                            if (Untils.Constants.IsValidVedioFormat(file.Extension))
                            {
                                await Task.Delay(100);

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

                                DispatcherHelper.CheckBeginInvokeOnUI(() => ScaningTootips = file.FullName);
                            }
                        }

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

                    int count = 0;

                    await Task.Run(async () =>
                    {
                        foreach (var media in Medias.Where(t => t.Item2.IsSelected))
                        {
                            //重复的文件不插入
                            if (await _mediaService.GetEntitiesCountAsync(t => t.MD5 == media.Item1.MD5) <= 0)
                            {
                                if (IsImportToMovie)
                                {
                                    media.Item1.MediaType = (int)PubilcEnum.MediaType.Movie;
                                }

                                count++;
                                await _mediaService.InsertAsync(media.Item1);
                            }
                        }
                    });

                    _applicationState.HideLoadingDialog();

                    Messenger.Default.Send(new ManageExceptionMessage(new Exception($"排除重复项后成功保存 {count} 个本地视频数据！")));

                    Messenger.Default.Send(new RefreshVideoListMessage());
                }
            });
        }
    }
}
