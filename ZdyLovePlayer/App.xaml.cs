using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using NLog;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Unosquare.FFME;
using Unosquare.FFME.Shared;
using Unosquare.FFME.Platform;
using ZdyLovePlayer.Windows;

namespace ZdyLovePlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Stopwatch WatchStart { get; }

        #region Constructor

        static App()
        {
            WatchStart = Stopwatch.StartNew();
            Logger.Info("Application Starting...");
            DispatcherHelper.Initialize();
        }

        public App()
        {
            HandleFFmpegSettings();

            RegisterExceptionEvent();
        }

        #endregion

        #region Handle FFmpeg

        public void HandleFFmpegSettings()
        {
            //ffmpeg
            // Change the default location of the ffmpeg binaries
            // You can get the binaries here: https://ffmpeg.zeranoe.com/builds/win32/shared/ffmpeg-4.0-win32-shared.zip
            MediaElement.FFmpegDirectory = @"c:\ffmpeg";

            // You can pick which FFmpeg binaries are loaded. See issue #28
            // Full Features is already the default.
            MediaElement.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;

            // Multithreaded video enables the creation of independent
            // dispatcher threads to render video frames.
            MediaElement.EnableWpfMultithreadedVideo = GuiContext.Current.IsInDebugMode == false;
        }

        public void HandleFFmpegLoading()
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                try
                {
                    // Force loading
                    MediaElement.LoadFFmpeg();
                }
                catch (Exception ex)
                {
                    GuiContext.Current?.EnqueueInvoke(() =>
                    {
                        MessageBox.Show(MainWindow,
                            $"Unable to Load FFmpeg Libraries from path:\r\n    {MediaElement.FFmpegDirectory}" +
                            $"\r\nMake sure the above folder contains FFmpeg shared binaries (dll files) for the " +
                            $"applicantion's architecture ({(Environment.Is64BitProcess ? "64-bit" : "32-bit")})" +
                            $"\r\nTIP: You can download builds from https://ffmpeg.zeranoe.com/builds/" +
                            $"\r\n{ex.GetType().Name}: {ex.Message}\r\n\r\nApplication will exit.",
                            "FFmpeg Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        Application.Current?.Shutdown();
                    });
                }
            });
        }

        #endregion

        #region  Handle Exception

        private void RegisterExceptionEvent()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            CurrentDomainUnhandledException(sender, new UnhandledExceptionEventArgs(e.Exception, false));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            CurrentDomainUnhandledException(sender, new UnhandledExceptionEventArgs(exception, false));
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            CurrentDomainUnhandledException(sender, new UnhandledExceptionEventArgs(e.Exception, false));
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            CurrentDomainUnhandledException(sender, new UnhandledExceptionEventArgs(e.Exception, false));
        }

        #endregion

        #region Startup

        /// <summary>
        /// Splash screen dispatcher
        /// </summary>
        private Dispatcher _splashScreenDispatcher;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var splashScreenThread = new Thread(() =>
            {
                var splashScreen = new Windows.SplashScreen();
                splashScreen.OnCompleted += SplashScreen_OnCompleted;
                _splashScreenDispatcher = splashScreen.Dispatcher;
                _splashScreenDispatcher.ShutdownStarted += (o, args) => splashScreen.Close();
                splashScreen.Show();
                Dispatcher.Run();
            });

            splashScreenThread.SetApartmentState(ApartmentState.STA);
            splashScreenThread.Start();


            MainWindow = new MainWindow();
            MainWindow.Loaded += async (sender2, e2) =>
            {
                await MainWindow.Dispatcher.InvokeAsync(() =>
                {
                    _splashScreenDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    MainWindow.Activate();
                    WatchStart.Stop();
                    var elapsedStartMs = WatchStart.ElapsedMilliseconds;
                    Logger.Info($"Application Started In {elapsedStartMs} Milliseconds.");
                });
            };

            HandleFFmpegLoading();
        }

        private void SplashScreen_OnCompleted(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => MainWindow.Show());
        }

        #endregion
    }
}
