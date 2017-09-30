﻿#pragma checksum "..\..\..\Windows\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2B706F1E3FE3CFC0228F9BC52321A751"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Zhu.AttachedProperties;
using Zhu.Controls;
using Zhu.Converters;
using Zhu.UserControls;
using Zhu.UserControls.Home;
using Zhu.UserControls.Home.Media;
using Zhu.ViewModels.Main;
using Zhu.ViewModels.Pages;


namespace Zhu.Windows {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 57 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.DialogHost PlayerMain;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.ColorZone TopBar;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Menu;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button WindowMinimized;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button WindowFullScreen;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button WindowClose;
        
        #line default
        #line hidden
        
        
        #line 193 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox CatgoryItemsListBox;
        
        #line default
        #line hidden
        
        
        #line 275 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.Snackbar MainSnackbar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Zhu;component/windows/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 33 "..\..\..\Windows\MainWindow.xaml"
            ((Zhu.Windows.MainWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.PlayerMain = ((MaterialDesignThemes.Wpf.DialogHost)(target));
            
            #line 63 "..\..\..\Windows\MainWindow.xaml"
            this.PlayerMain.DragEnter += new System.Windows.DragEventHandler(this.PlayerMain_DragEnter);
            
            #line default
            #line hidden
            
            #line 64 "..\..\..\Windows\MainWindow.xaml"
            this.PlayerMain.Drop += new System.Windows.DragEventHandler(this.PlayerMain_Drop);
            
            #line default
            #line hidden
            return;
            case 3:
            this.TopBar = ((MaterialDesignThemes.Wpf.ColorZone)(target));
            
            #line 73 "..\..\..\Windows\MainWindow.xaml"
            this.TopBar.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.TopBar_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 74 "..\..\..\Windows\MainWindow.xaml"
            this.TopBar.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.TopBar_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Menu = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.WindowMinimized = ((System.Windows.Controls.Button)(target));
            
            #line 90 "..\..\..\Windows\MainWindow.xaml"
            this.WindowMinimized.Click += new System.Windows.RoutedEventHandler(this.WindowMinimized_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.WindowFullScreen = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\..\Windows\MainWindow.xaml"
            this.WindowFullScreen.Click += new System.Windows.RoutedEventHandler(this.WindowFullScreen_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.WindowClose = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\..\Windows\MainWindow.xaml"
            this.WindowClose.Click += new System.Windows.RoutedEventHandler(this.WindowClose_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.CatgoryItemsListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 9:
            this.MainSnackbar = ((MaterialDesignThemes.Wpf.Snackbar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

