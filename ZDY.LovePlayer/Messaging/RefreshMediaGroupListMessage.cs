using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDY.LovePlayer.Messaging
{
    public class RefreshMediaGroupListMessage : MessageBase
    {
        public bool IsRefresh { get; set; }

        public RefreshMediaGroupListMessage(bool isRefresh = true)
        {
            IsRefresh = isRefresh;
        }
    }
}
