using GalaSoft.MvvmLight.Messaging;

namespace ZDY.LovePlayer.Messaging
{
    public class SwitchPageMessage : MessageBase
    {
        public object Content { get; set; }
    }
}
