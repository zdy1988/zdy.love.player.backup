using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Services;

namespace Zhu.ViewModels.Dialogs
{
    public class ScanMediaFileDialogViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IApplicationState _applicationState { get; set; }

        public ScanMediaFileDialogViewModel(IApplicationState applicationState)
        {
            _applicationState = applicationState;
        }
    }
}
