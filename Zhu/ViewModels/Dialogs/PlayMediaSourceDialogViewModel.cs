using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Services;

namespace Zhu.ViewModels.Dialogs
{
    public class PlayMediaSourceDialogViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { Set(() => FilePath, ref _filePath, value); }
        }

        private string _networkAddress;
        public string NetworkAddress
        {
            get { return _networkAddress; }
            set { Set(() => NetworkAddress, ref _networkAddress, value); }
        }

        public PlayMediaSourceDialogViewModel()
        {
            RegisterCommands();
        }

        private string MediaSource
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) ? NetworkAddress : FilePath;
            }
        }

        private string MediaTitle
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) ? NetworkAddress : FilePath.Substring(FilePath.LastIndexOf("\\") + 1);
            }
        }

        public PlayMediaSourceDialogViewModel(IApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        public RelayCommand PlayMediaSourceCommand { get; private set; }

        private void RegisterCommands()
        {
            PlayMediaSourceCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(MediaSource))
                {
                    Messenger.Default.Send(new OpenMediaMessage(new Media
                    {
                        ID = 0,
                        Title = MediaTitle,
                        MediaSource = MediaSource
                    }));
                }
            });
        }
    }
}
