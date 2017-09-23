using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using Zhu.Controls;
using Zhu.Messaging;
using Zhu.Models;
using Zhu.Models.ApplicationState;
using Zhu.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zhu.ViewModels.Pages
{
    public class NetTVListViewModel : MediaListViewModel<NetTV>
    {
        private INetTVService NetTVServices;

        public NetTVListViewModel(IApplicationState applicationState, INetTVService netTVServices)
            : base(applicationState, netTVServices)
        {
            NetTVServices = netTVServices;

            SortFields = new Dictionary<string, string>();
            SortFields.Add("加入顺序", "ID");
            SortFields.Add("星级评分", "Rating");

            RegisterCommands();
        }

        public Dictionary<string, string> SortFields { get; set; }

        private void RegisterCommands()
        {
            PlayMediaCommand = new RelayCommand<Media>(async (media) =>
            {
                var view = new SampleProgressDialog();
                await DialogHost.Show(view, "RootDialog", (object sender, DialogOpenedEventArgs eventArgs) =>
                {
                    Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(async(t) =>
                    {
                        var netTV = await SimpleIoc.Default.GetInstance<INetTVService>().GetEntityAsync(e => e.ID == media.ID);
                        Messenger.Default.Send(new LoadMediaMessage(netTV));
                        eventArgs.Session.Close(false);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }, (object sender, DialogClosingEventArgs eventArgs) => { });
            });
        }
    }
}
