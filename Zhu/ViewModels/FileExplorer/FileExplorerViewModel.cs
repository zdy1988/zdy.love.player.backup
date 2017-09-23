using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Zhu.Untils;
using Zhu.Models;
using Zhu.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Zhu.ViewModels.FileExplorer
{
    public class FileExplorerViewModel : ViewModelBase
    {
        private IMovieService MovieServices;

        public FileExplorerViewModel(IMovieService movieServices)
        {
            this.MovieServices = movieServices;

            RegisterCommands();

            InitializeFileExplorer();
        }

        private LocalDirectoryInfo _selectedItem;
        public LocalDirectoryInfo SelectedItem
        {
            get { return _selectedItem; }
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }

        public ObservableCollection<LocalDirectoryInfo> DirectoryList { get; set; }

        private LocalDirectoryInfo ListDirectory(LocalDirectoryInfo info)
        {
            info.DirectoryList.Clear();
            info.FileList.Clear();
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var path in Directory.GetDirectories(info.Directory.FullName))
                {
                    if (Directory.Exists(@path))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(path);
                        // 系统文件可见 排除临时文件，只读文件,隐藏文件
                        if (FileAttributes.Directory == directoryInfo.Attributes || directoryInfo.Attributes.ToString().Equals("ReadOnly, Directory"))
                        {
                            var childInfo = ListDirectory(new LocalDirectoryInfo(directoryInfo));
                            info.DirectoryList.Add(childInfo);
                        }
                    }
                }

                foreach (var path in Directory.GetFiles(info.Directory.FullName))
                {
                    if (File.Exists(@path))
                    {
                        FileInfo fileInfo = new FileInfo(@path);
                        if (fileInfo.Extension.ToLower().Equals(".jpg") || fileInfo.Extension.ToLower().Equals(".mp4"))
                        {
                            info.FileList.Add(new LocalFileInfo(fileInfo));
                        }
                    }
                }
            }));
            return info;
        }

        private void InitializeFileExplorer()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Untils.Constants.DictionaryRoot);
            if (directoryInfo.Exists)
            {

                var info = ListDirectory(new LocalDirectoryInfo(directoryInfo));
                DirectoryList = new ObservableCollection<LocalDirectoryInfo>
                {
                    info
                };
            }
        }

        public RelayCommand ImportFileDataCommand { get; private set; }

        private void RegisterCommands()
        {
            ImportFileDataCommand = new RelayCommand(async () =>
            {
                await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    SelectedItem.FileList?.Where(t => t.IsSelected).ToList().ForEach(async (t) =>
                    {
                        await MovieServices.InsertAsync(new Movie
                        {
                            Title = t.FileName.Replace(t.File.Extension, ""),
                            FilePath = t.File.Directory.FullName + "\\" + t.File.Name,
                            MD5 = FileHelper.MD5File(t.File.Directory.FullName + "\\" + t.File.Name),
                            IsFavorite =false,
                            EnterDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });
                    });
                });
            });
        }
    }
}
