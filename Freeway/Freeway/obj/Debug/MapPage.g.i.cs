﻿#pragma checksum "C:\Version Control\Freeway\Freeway\Freeway\MapPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BCB4F585650131F426EE2A184BC1F345"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Freeway.Controls;
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
    
    
    public partial class MapPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Media.Animation.Storyboard FloatDown;
        
        internal System.Windows.Media.Animation.Storyboard FloatUp;
        
        internal System.Windows.Media.Animation.Storyboard LandscapeFloatUp;
        
        internal System.Windows.Media.Animation.Storyboard LandscapeFloatDown;
        
        internal System.Windows.Controls.Grid MapLayoutRoot;
        
        internal Microsoft.Phone.Maps.Controls.Map MyMap;
        
        internal Freeway.Controls.NavigationDetail NavDirections;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Freeway;component/MapPage.xaml", System.UriKind.Relative));
            this.FloatDown = ((System.Windows.Media.Animation.Storyboard)(this.FindName("FloatDown")));
            this.FloatUp = ((System.Windows.Media.Animation.Storyboard)(this.FindName("FloatUp")));
            this.LandscapeFloatUp = ((System.Windows.Media.Animation.Storyboard)(this.FindName("LandscapeFloatUp")));
            this.LandscapeFloatDown = ((System.Windows.Media.Animation.Storyboard)(this.FindName("LandscapeFloatDown")));
            this.MapLayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("MapLayoutRoot")));
            this.MyMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("MyMap")));
            this.NavDirections = ((Freeway.Controls.NavigationDetail)(this.FindName("NavDirections")));
        }
    }
}
