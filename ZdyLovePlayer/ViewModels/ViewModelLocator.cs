/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Player"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ZdyLovePlayer.Services;
using ZdyLovePlayer.ViewModels.Main;
using ZdyLovePlayer.ViewModels.Player;
using ZdyLovePlayer.ViewModels.Pages;
using CommonServiceLocator;

namespace ZdyLovePlayer.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IApplicationState, ApplicationState>();

            // DataServices
            SimpleIoc.Default.Register<ITagService, TagService>();
            SimpleIoc.Default.Register<IMediaService, MediaService>();
            SimpleIoc.Default.Register<ISeenService, SeenService>();
            SimpleIoc.Default.Register<IGroupService, GroupService>();
            SimpleIoc.Default.Register<IGroupMemberService, GroupMemberService>();

            // ViewModels
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<MediaPlayerViewModel>();
            SimpleIoc.Default.Register<MovieListViewModel>();
            SimpleIoc.Default.Register<NetTVListViewModel>();
            SimpleIoc.Default.Register<VideoListViewModel>();
            SimpleIoc.Default.Register<SeenListViewModel>();
            SimpleIoc.Default.Register<FavoriteListViewModel>();
            SimpleIoc.Default.Register<MediaGroupViewModel>();
            SimpleIoc.Default.Register<MediaGroupMembersViewModel>();
        }

        public MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
        public MediaPlayerViewModel MediaPlayer => ServiceLocator.Current.GetInstance<MediaPlayerViewModel>();
        public MovieListViewModel MovieList => ServiceLocator.Current.GetInstance<MovieListViewModel>();
        public NetTVListViewModel NetTVList => ServiceLocator.Current.GetInstance<NetTVListViewModel>();
        public VideoListViewModel VideoList => ServiceLocator.Current.GetInstance<VideoListViewModel>();
        public SeenListViewModel SeenList => ServiceLocator.Current.GetInstance<SeenListViewModel>();
        public FavoriteListViewModel FavoriteList => ServiceLocator.Current.GetInstance<FavoriteListViewModel>();
        public MediaGroupViewModel MediaGroup => ServiceLocator.Current.GetInstance<MediaGroupViewModel>();
        public MediaGroupMembersViewModel MediaGroupMembers => ServiceLocator.Current.GetInstance<MediaGroupMembersViewModel>();
    }
}