using GalaSoft.MvvmLight;
using System;

namespace Zhu.ViewModels.Pages
{
    public class SeenListItemViewModel: ViewModelBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        private int _id;
        public int ID
        {
            get { return _id; }
            set { Set(() => ID, ref _id, value); }
        }

        private DateTime _seenDate;
        public DateTime SeenDate
        {
            get { return _seenDate; }
            set { Set(() => SeenDate, ref _seenDate, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        private string _mediaSource;
        public string MediaSource
        {
            get { return _mediaSource; }
            set { Set(() => MediaSource, ref _mediaSource, value); }
        }

        private int _mediaID;
        public int MediaID
        {
            get { return _mediaID; }
            set { Set(() => MediaID, ref _mediaID, value); }
        }
    }
}
