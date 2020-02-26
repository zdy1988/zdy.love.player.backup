using GalaSoft.MvvmLight.Messaging;
using ZDY.LovePlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDY.LovePlayer.Messaging
{
    public class OpenMediaMessage : MessageBase
    {
        public IMedia Media { get; set; }

        public OpenMediaMessage(IMedia media)
        {
            Media = media;
        }
    }
}
