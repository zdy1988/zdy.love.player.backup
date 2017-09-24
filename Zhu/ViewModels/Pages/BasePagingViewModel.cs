using GalaSoft.MvvmLight;
using Infrastructure.SearchModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Untils;

namespace Zhu.ViewModels.Pages
{
    public class BasePagingViewModel : ViewModelBase
    {
        protected QueryModel SearchQueryModel = new QueryModel();

        protected int PageSize { get; set; } = Constants.LoadMoviesPageSize;

        private int _pageIndex;
        protected int PageIndex
        {
            get { return _pageIndex; }
            set { Set(() => PageIndex, ref _pageIndex, value); }
        }

        private bool _hasLoadingFailed;
        public bool HasLoadingFailed
        {
            get { return _hasLoadingFailed; }
            set { Set(() => HasLoadingFailed, ref _hasLoadingFailed, value); }
        }

        private bool _isDataLoading;
        public bool IsDataLoading
        {
            get { return _isDataLoading; }
            protected set { Set(() => IsDataLoading, ref _isDataLoading, value); }
        }

        private bool _isDataFound = true;
        public bool IsDataFound
        {
            get { return _isDataFound; }
            set { Set(() => IsDataFound, ref _isDataFound, value); }
        }

        private int _currentNumberOfData;
        public int CurrentNumberOfData
        {
            get { return _currentNumberOfData; }
            set { Set(() => CurrentNumberOfData, ref _currentNumberOfData, value); }
        }

        private int _maxNumberOfData;
        public int MaxNumberOfData
        {
            get { return _maxNumberOfData; }
            set { Set(() => MaxNumberOfData, ref _maxNumberOfData, value); }
        }

        private string _searchOrderField;
        public string SearchOrderField
        {
            get { return _searchOrderField; }
            set { Set(() => SearchOrderField, ref _searchOrderField, value); }
        }
    }
}
