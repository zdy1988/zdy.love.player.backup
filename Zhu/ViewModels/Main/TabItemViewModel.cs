using GalaSoft.MvvmLight;

namespace Zhu.ViewModels.Main
{
    public class TabItemViewModel : ViewModelBase
    {
        private string _title;
        private object _content;
        private string _icon;

        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        public string Icon
        {
            get { return _icon; }
            set { Set(() => Icon, ref _icon, value); }
        }

        public object Content
        {
            get { return _content; }
            set { Set(() => Content, ref _content, value); }
        }
    }
}
