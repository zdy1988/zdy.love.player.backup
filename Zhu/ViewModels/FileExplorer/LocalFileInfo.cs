using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.ViewModels.FileExplorer
{
    public class LocalFileInfo : ViewModelBase
    {
        private bool _isSelected;

        private string _fileName;

        public FileInfo File { get; private set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        public string FileName
        {
            get { return _fileName; }
            set { Set(() => FileName, ref _fileName, value); }
        }

        public LocalFileInfo(FileInfo file)
        {
            this.File = file;
            this.FileName = file.Name;
        }
    }
}
