using ZDY.LovePlayer.Foundation;
using Unosquare.FFME.Platform;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using ZDY.LovePlayer.Messaging;
using ZDY.LovePlayer.Models;

namespace ZDY.LovePlayer.UserControls.Player.Controls
{
    /// <summary>
    /// Interaction logic for PlaylistPanelControl.xaml
    /// </summary>
    public partial class PlaylistPanelControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistPanelControl"/> class.
        /// </summary>
        public PlaylistPanelControl()
        {
            InitializeComponent();

            // Prevent binding to the events
            if (GuiContext.Current.IsInDesignTime)
                return;

            // Bind the Enter key to the command
            OpenFileTextBox.KeyDown += (s, e) =>
            {
                if (e.Key != Key.Enter) return;
                Messenger.Default.Send(new OpenMediaMessage(new Media
                {
                    MediaSource = OpenFileTextBox.Text
                }));
                e.Handled = true;
            };

            SearchTextBox.IsEnabledChanged += (s, e) =>
            {
                if ((bool)e.OldValue == false && (bool)e.NewValue == true)
                    FocusSearchBox();

                if ((bool)e.OldValue == true && (bool)e.NewValue == false)
                    FocusFileBox();
            };

            IsVisibleChanged += (s, e) =>
            {
                if (SearchTextBox.IsEnabled)
                    FocusSearchBox();
                else
                    FocusFileBox();
            };
        }

        private void FocusTextBox(TextBox textBox)
        {
            DeferredAction deferredAction = null;
            deferredAction = DeferredAction.Create(() =>
            {
                if (textBox == null || App.Current == null || App.Current.MainWindow == null)
                    return;

                textBox.Focus();
                textBox.SelectAll();
                FocusManager.SetFocusedElement(App.Current.MainWindow, textBox);
                Keyboard.Focus(textBox);

                if (textBox.IsVisible == false || textBox.IsKeyboardFocused)
                    deferredAction.Dispose();
                else
                    deferredAction.Defer(TimeSpan.FromSeconds(0.25));
            });

            deferredAction.Defer(TimeSpan.FromSeconds(0.25));
        }

        /// <summary>
        /// Focuses the search box.
        /// </summary>
        private void FocusSearchBox()
        {
            FocusTextBox(SearchTextBox);
        }

        /// <summary>
        /// Focuses the file box.
        /// </summary>
        private void FocusFileBox()
        {
            FocusTextBox(OpenFileTextBox);
        }
    }
}
