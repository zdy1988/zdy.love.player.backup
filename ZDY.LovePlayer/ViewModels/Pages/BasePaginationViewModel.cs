using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Infrastructure.SearchModel.Model;
using NLog;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;
using ZDY.LovePlayer.Services;
using ZDY.LovePlayer.Untils;
using ZDY.LovePlayer.Untils.Extension;
using ZDY.LovePlayer.ViewModels.Reused;

namespace ZDY.LovePlayer.ViewModels.Pages
{
    public abstract class BasePaginationViewModel<T> : ViewModelBasic
    {
        protected QueryModel SearchQueryModel = new QueryModel();

        #region Constructor

        public BasePaginationViewModel(IApplicationState applicationState)
            : base(applicationState)
        {

        }

        #endregion

        #region Properties

        private ObservableCollection<Tuple<T, ListItemIsSelectViewModel>> _medias = new ObservableCollection<Tuple<T, ListItemIsSelectViewModel>>();
        public ObservableCollection<Tuple<T, ListItemIsSelectViewModel>> Medias
        {
            get { return _medias; }
            set { Set(() => Medias, ref _medias, value); }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set { Set(() => SearchQuery, ref _searchQuery, value); }
        }

        private int _pageSize = Constants.LoadDataPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => Set(ref _pageSize, value);
        }

        private int _pageIndex;
        protected int PageIndex
        {
            get { return _pageIndex; }
            set { Set(() => PageIndex, ref _pageIndex, value); }
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

        private bool _isLoadingFailed;
        public bool IsLoadingFailed
        {
            get { return _isLoadingFailed; }
            set { Set(() => IsLoadingFailed, ref _isLoadingFailed, value); }
        }

        private bool _isDataFound = false;
        public bool IsDataFound
        {
            get { return _isDataFound; }
            set { Set(() => IsDataFound, ref _isDataFound, value); }
        }

        private bool? _isSelectedAll;

        public bool? IsSelectedAll
        {
            get { return _isSelectedAll; }
            set
            {
                Set(() => IsSelectedAll, ref _isSelectedAll, value);

                if (_isSelectedAll.HasValue)
                {
                    ExecuteSelectAll(_isSelectedAll.Value, _medias);
                }
            }
        }

        #endregion

        #region Methods

        public abstract void ExecuteLoadMedias(bool isRefresh = false);

        public virtual void ExecuteLoadMedias(Func<Tuple<List<T>, int>> action, bool isRefresh = false)
        {
            if (isRefresh)
            {
                PageIndex = 0;
                ExecuteClearMedias();
            }

            ExecuteActionAndWatchPageTurning(() => ExecuteActionWithProgress(async () =>
            {
                var medias = await ExecuteLoadDataWithDelay<T>(() => action.Invoke());

                var list = new List<Tuple<T, ListItemIsSelectViewModel>>();
                for (var i = 0; i < medias.Item1.Count(); i++)
                {
                    var item = medias.Item1[i];
                    var isSelect = new ListItemIsSelectViewModel { IsSelected = false };
                    list.Add(new Tuple<T, ListItemIsSelectViewModel>(item, isSelect));
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Medias.AddRange(list);
                    IsDataFound = Medias.Any();
                    CurrentNumberOfData = Medias.Count;
                    MaxNumberOfData = medias.Item2;
                });
            }));
        }

        public virtual void ExecuteClearMedias()
        {
            Medias.Clear();
        }

        public void ExecuteActionAndWatchPageTurning(Action action)
        {
            ExecuteActionWithWatching($"Load Page {PageIndex}",
                () =>
                {
                    PageIndex++;
                    IsLoadingFailed = false;

                    action.Invoke();
                },
                () =>
                {
                    PageIndex--;
                    IsLoadingFailed = true;
                },
                () => { });
        }

        private static void ExecuteSelectAll(bool select, IEnumerable<Tuple<T, ListItemIsSelectViewModel>> models)
        {
            foreach (var model in models)
            {
                model.Item2.IsSelected = select;
            }
        }

        #endregion

        #region Commands

        public RelayCommand RefreshMediaListCommand => new Lazy<RelayCommand>(() => new RelayCommand(() => ExecuteLoadMedias(true))).Value;

        #endregion
    }
}
