using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ZdyLovePlayer.UserControls.Layout
{
    public class DialogPane : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
            typeof(string), typeof(DialogPane));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

        public static readonly DependencyProperty InnerContentProperty = DependencyProperty.Register("InnerContent",
            typeof(object), typeof(DialogPane));
        public object InnerContent { get { return (object)GetValue(InnerContentProperty); } set { SetValue(InnerContentProperty, value); } }

        public static readonly DependencyProperty FooterContentProperty = DependencyProperty.Register("FooterContent",
            typeof(object), typeof(DialogPane));
        public object FooterContent { get { return (object)GetValue(FooterContentProperty); } set { SetValue(FooterContentProperty, value); } }
    }
}
