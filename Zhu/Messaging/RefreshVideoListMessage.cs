using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Messaging
{
    public class RefreshVideoListMessage : MessageBase
    {
        public bool IsRefresh { get; set; }

        public RefreshVideoListMessage(bool isRefresh = true)
        {
            IsRefresh = isRefresh;
        }
    }
}
