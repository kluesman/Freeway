﻿#pragma checksum "C:\Version Control\Freeway\Freeway\Freeway\SearchPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "21EE0AB7FC6055DE9FB0460EB9A6F7BE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Freeway {
    
    
    public partial class SearchPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel stackSearch;
        
        internal System.Windows.Controls.TextBox txtSearch;
        
        internal System.Windows.Controls.Image imgSearch;
        
        internal System.Windows.Controls.ProgressBar progressBar;
        
        internal System.Windows.Controls.Grid VerticalSearch;
        
        internal Microsoft.Phone.Maps.Controls.Map VerticalMap;
        
        internal System.Windows.Controls.ListBox VerticalResult;
        
        internal System.Windows.Controls.Grid HorizontalSearch;
        
        internal Microsoft.Phone.Maps.Controls.Map HorizontalMap;
        
        internal System.Windows.Controls.ListBox HorizontalResult;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Freeway;component/SearchPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.stackSearch = ((System.Windows.Controls.StackPanel)(this.FindName("stackSearch")));
            this.txtSearch = ((System.Windows.Controls.TextBox)(this.FindName("txtSearch")));
            this.imgSearch = ((System.Windows.Controls.Image)(this.FindName("imgSearch")));
            this.progressBar = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBar")));
            this.VerticalSearch = ((System.Windows.Controls.Grid)(this.FindName("VerticalSearch")));
            this.VerticalMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("VerticalMap")));
            this.VerticalResult = ((System.Windows.Controls.ListBox)(this.FindName("VerticalResult")));
            this.HorizontalSearch = ((System.Windows.Controls.Grid)(this.FindName("HorizontalSearch")));
            this.HorizontalMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("HorizontalMap")));
            this.HorizontalResult = ((System.Windows.Controls.ListBox)(this.FindName("HorizontalResult")));
        }
    }
}

