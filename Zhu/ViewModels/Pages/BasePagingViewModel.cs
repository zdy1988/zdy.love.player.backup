using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public abstract class BasePagingViewModel : ViewModelBase
    {
        public BasePagingViewModel()
        {
            RegisterCommands();
        }

        protected QueryModel SearchQueryModel = new QueryModel();

        protected int PageSize { get; set; } = Constants.LoadDataPageSize;

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

        private string _orderField;
        public string OrderField
        {
            get { return _orderField; }
            set { Set(() => OrderField, ref _orderField, value); }
        }

        public abstract Task LoadMediasAsync(bool isRefresh = false);

        public RelayCommand RefreshMediaListCommand { get; set; }

        private void RegisterCommands()
        {
            RefreshMediaListCommand = new RelayCommand(async () =>
            {
                await LoadMediasAsync(true).ConfigureAwait(false);
            });
        }
    }
}
