using GalaSoft.MvvmLight.Messaging;

namespace ZdyLovePlayer.Messaging
{
    public class SwitchPageMessage : MessageBase
    {
        public object Content { get; set; }
    }
}
