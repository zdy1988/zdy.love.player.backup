using GalaSoft.MvvmLight.Messaging;
using Zhu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Messaging
{
    public class LoadMediaMessage : MessageBase
    {
        public IMedia Media { get; set; }

        public LoadMediaMessage(IMedia media)
        {
            Media = media;
        }
    }
}
