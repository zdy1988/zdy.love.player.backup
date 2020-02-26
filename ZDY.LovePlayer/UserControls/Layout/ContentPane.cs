using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ZDY.LovePlayer.UserControls.Layout
{
    public class ContentPane : UserControl
    {
        public static readonly DependencyProperty HeadingTextProperty = DependencyProperty.Register("HeadingText",
            typeof(string), typeof(ContentPane));
        public string HeadingText { get {return (string)GetValue(HeadingTextProperty);} set {SetValue(HeadingTextProperty, value); }}

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent",
            typeof(object), typeof(ContentPane));
        public object HeaderContent { get { return (object)GetValue(HeaderContentProperty); } set { SetValue(HeaderContentProperty, value); } }

        public static readonly DependencyProperty InnerContentProperty = DependencyProperty.Register("InnerContent",
            typeof(object), typeof(ContentPane));
        public object InnerContent { get { return (object)GetValue(InnerContentProperty); } set { SetValue(InnerContentProperty, value); } }
    }
}
