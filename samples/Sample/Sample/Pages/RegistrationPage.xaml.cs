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

namespace Sample
{
	public partial class RegistrationPage : ContentPage
	{
		public RegistrationPage ()
		{
			InitializeComponent ();
			UpdateRegistration ();
		}

		protected override void OnAppearing()
		{
			SDK.Instance.RegistrationUpdated += UpdateRegistration;

		}
		protected override void OnDisappearing ()
		{
			SDK.Instance.RegistrationUpdated -= UpdateRegistration;
		}


		public void UpdateRegistration()
		{
			Device.BeginInvokeOnMainThread(() => {
				UserId.Detail = SDK.Instance.UserId ();
				ChannelId.Detail = SDK.Instance.ChannelId ();
				AppKey.Detail = SDK.Instance.AppKey ();
			});
		}
	}
}

