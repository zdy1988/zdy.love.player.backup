using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.ViewModels.Main
{
    public class ImportNetworkMediaDialogViewModel : ViewModelBase
    {
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { Set(() => FilePath, ref _filePath, value); }
        }
    }
}
