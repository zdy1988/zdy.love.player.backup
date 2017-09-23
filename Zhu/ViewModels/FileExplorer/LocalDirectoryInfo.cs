using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.ViewModels.FileExplorer
{
    public class LocalDirectoryInfo : ViewModelBase
    {
        public DirectoryInfo Directory { get; private set; }
        public ObservableCollection<LocalDirectoryInfo> DirectoryList { get; private set; }
        public ObservableCollection<LocalFileInfo> FileList { get; set; }

        public LocalDirectoryInfo(DirectoryInfo directory)
        {
            this.Directory = directory;
            this.DirectoryList = new ObservableCollection<LocalDirectoryInfo>();
            this.FileList = new ObservableCollection<LocalFileInfo>();
        }
    }
}
