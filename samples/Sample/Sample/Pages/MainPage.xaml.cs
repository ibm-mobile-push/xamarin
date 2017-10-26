/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using System.Collections.Generic;
using IBMMobilePush.Forms;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Sample
{
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();
            this.Appearing += (sender, e) => {
				VersionLabel.Text = SDK.Instance.Version();
                if (Device.RuntimePlatform == Device.Android)
				{
					Logo.HeightRequest = 100;
					Logo.WidthRequest = 245;
				}
				Logo.Source = ImageSource.FromFile("logo.png");
			};
		}

		public async void OpenPage(object sender, EventArgs e) 
		{
			var styleId = ((Cell)sender).StyleId;

			switch (styleId) {
			case "Registration":
				await Navigation.PushAsync (new RegistrationPage ());
				break;
			case "Inbox":
				await Navigation.PushAsync (new InboxPage ());
				break;
			case "InApp":
				await Navigation.PushAsync (new InAppPage());
				break;
			case "Actions":
				await Navigation.PushAsync (new ActionsPage());
				break;
			case "Events":
				await Navigation.PushAsync (new EventsPage());
				break;
			case "Attributes":
				await Navigation.PushAsync (new AttributesPage());
				break;
			case "Geofences":
				await Navigation.PushAsync(new GeofencePage());
				break;
			case "iBeacons":
				await Navigation.PushAsync(new BeaconPage());
				break;
			}
		}
	}

}
