using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ThatConference_Freeway.Resources;
using Windows.Phone.Speech.Synthesis;

namespace ThatConference_Freeway
{
    public partial class MainPage : PhoneApplicationPage
    {
        SpeechSynthesizer MyVoice;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            MyVoice = new SpeechSynthesizer();
        }
        

        private void btnSpeak_Click(object sender, RoutedEventArgs e)
        {
            speak(this.txtSpeechText.Text);
        }
        private async void speak(string speechtext)
        {
            try
            {
                await MyVoice.SpeakTextAsync(speechtext);
            }
            catch
            {
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            mainMap.Center = new System.Device.Location.GeoCoordinate(0, 0);
            mainMap.ZoomLevel = 2;
        }
        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}