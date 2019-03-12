using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdyLovePlayer.Messaging
{
    public class RefreshNetTVListMessage : MessageBase
    {
        public bool IsRefresh { get; set; }

        public RefreshNetTVListMessage(bool isRefresh = true)
        {
            IsRefresh = isRefresh;
        }
    }
}
