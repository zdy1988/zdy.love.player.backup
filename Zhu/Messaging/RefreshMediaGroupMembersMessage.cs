using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhu.Models;

namespace Zhu.Messaging
{
    public class RefreshMediaGroupMembersMessage : MessageBase
    {
        public bool IsRefresh { get; set; }

        public Group Group { get; set; }

        public RefreshMediaGroupMembersMessage(Group group, bool isRefresh = true)
        {
            IsRefresh = isRefresh;
            Group = group;
        }
    }
}
