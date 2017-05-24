/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using Xamarin.Forms;
using IBMMobilePush.Forms;
using Newtonsoft.Json.Linq;
using System;

namespace Sample
{
	public class WebViewAction : PushAction
	{
		public WebViewAction () 
		{
		}

		public override void HandleAction (JObject action, JObject payload, string attribution, string mailingId, int id)
		{
			var page = new ContentPage ();
			var dismiss = new Button () {
				Text = "Dismiss",
				HorizontalOptions = LayoutOptions.Start
			};
			dismiss.Clicked += (object sender, EventArgs e) => {
				page.Navigation.PopModalAsync();
			};

			page.Content = new StackLayout () {
				Padding = new Thickness (0, Device.OnPlatform (20, 0, 0), 0, 0),
				Children = {
					new StackLayout () {
						Padding = new Thickness(10,0),
						Children = {
							dismiss
						}
					},
					new BoxView() {
						HeightRequest = 1,
						Color = Color.FromHex("#ccc")
					},
					new WebView () {
						Source = new UrlWebViewSource
						{
							Url = action["value"].ToString(),
						},
						VerticalOptions = LayoutOptions.FillAndExpand
					}
				}
			};
			Application.Current.MainPage.Navigation.PushModalAsync (page);
		}	
	}
}

