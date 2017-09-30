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

        private List<Media> _medias = new List<Media>();
        public List<Media> Medias
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
                        List<Media> medias = new List<Media>();

                        //遍历文件
                        foreach (FileInfo file in dirInfo)
                        {
                            if (Untils.Constants.IsValidVedioFormat(file.Extension))
                            {
                                //var md5 = await Task.Run(() =>
                                //{
                                //    return Untils.FileHelper.MD5File(file.FullName);
                                //}).ConfigureAwait(false);

                                var media = new Media
                                {
                                    //MD5 = md5,
                                    Title = file.Name,
                                    Cover = $"{Guid.NewGuid().ToString()}.jpg",
                                    MediaType = (int)PubilcEnum.MediaType.Video,
                                    MediaSource = file.FullName,
                                    MediaSourceType = (int)PubilcEnum.MediaSourceType.LocalFile,
                                    IsFavorite = false,
                                    EnterDate = DateTime.Now,
                                    UpdateDate = DateTime.Now
                                };

                                medias.Add(media);

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
                    _applicationState.ShowLoadingDialog("请稍等...");

                    await Task.Run(async () =>
                    {
                        foreach (var media in Medias)
                        {
                            await _mediaService.InsertAsync(media);
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
