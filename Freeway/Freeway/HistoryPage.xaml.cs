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
    public partial class HistoryPage : PhoneApplicationPage
    {
        public HistoryPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
            {
                VerticalSearch.Visibility = Visibility.Collapsed;
                HorizontalSearch.Visibility = Visibility.Visible;
            }
            else 
            {
                VerticalSearch.Visibility = Visibility.Visible;
                HorizontalSearch.Visibility = Visibility.Collapsed;
            }
        }
        private void Navigate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        { 
        
        }

        private void HorizontalResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void VerticalResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
        
        }
    }
}