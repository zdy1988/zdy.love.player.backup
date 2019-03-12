using GalaSoft.MvvmLight;

namespace ZdyLovePlayer.ViewModels.Main
{
    public class NavItem : ViewModelBase
    {
        private string _title;
        private string _icon;
        private string _tag;

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

        public string Tag
        {
            get { return _tag; }
            set { Set(() => Tag, ref _tag, value); }
        }
    }
}
