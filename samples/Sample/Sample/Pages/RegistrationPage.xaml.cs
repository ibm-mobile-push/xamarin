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
            InitializeComponent();
            SDK.Instance.RegistrationUpdated += UpdateRegistration;
			UpdateRegistration ();
            Registration.Tapped += (object sender, EventArgs e) => {
                if (SDK.Instance.UserId() == null && SDK.Instance.ChannelId() == null)
                {
                    Registration.Detail = "Registering";
                    SDK.Instance.ManualSdkInitialization();
                }
            };
		}

		protected override void OnDisappearing ()
		{
			SDK.Instance.RegistrationUpdated -= UpdateRegistration;
		}


		public void UpdateRegistration()
		{
			Device.BeginInvokeOnMainThread(() => {
                var userId = SDK.Instance.UserId();
                var channelId = SDK.Instance.ChannelId();
                if(userId == null && channelId == null) {
                    Registration.Detail = "Tap to Register";
                } else {
                    Registration.Detail = "Registered";
                }
                if(SDK.Instance.UserInvalidated()) {
                    Registration.Detail += " (Invalidated State)";
                }
                UserId.Detail = userId;
                ChannelId.Detail = channelId;
				AppKey.Detail = SDK.Instance.AppKey ();
			});
		}
	}
}

