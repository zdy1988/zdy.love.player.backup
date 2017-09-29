using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.ViewModels.Reused
{
    public class SampleLoadingViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _loadingMessage;
        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set { Set(() => LoadingMessage, ref _loadingMessage, value); }
        }

        public SampleLoadingViewModel()
        { }
    }
}
