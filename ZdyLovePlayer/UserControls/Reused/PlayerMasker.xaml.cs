using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZdyLovePlayer.UserControls.Reused
{
    /// <summary>
    /// PlayerMasker.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerMasker
    {
        public static readonly DependencyProperty PlayIconWidthProperty = DependencyProperty.Register("PlayIconWidth", 
            typeof(double), typeof(PlayerMasker), new PropertyMetadata(new PropertyChangedCallback(OnPlayIconWidthPropertyChangedCallback)));

        private static void OnPlayIconWidthPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlayerMasker playerMasker = (d as PlayerMasker);
            playerMasker?.PlayIconWidthRuntime();
        }

        private void PlayIconWidthRuntime()
        {
            double width = Convert.ToDouble(PlayIconWidth, CultureInfo.InvariantCulture);
            PlayIcon.Width = width;
            PlayIcon.Height = width;
        }

        public double PlayIconWidth { get { return (double)GetValue(PlayIconWidthProperty); } set { SetValue(PlayIconWidthProperty, value); } }

        public PlayerMasker()
        {
            InitializeComponent();
        }
    }
}
