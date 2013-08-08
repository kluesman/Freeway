using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Freeway
{
    public partial class POIsPage : PhoneApplicationPage
    {
        public POIsPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
            {
                wrapPanelLayout.Width = (btnMap.Width * 3);
            }
            else
                wrapPanelLayout.Width = this.ActualWidth;
        }
    }
}