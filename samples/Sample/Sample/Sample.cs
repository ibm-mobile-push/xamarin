/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using Xamarin.Forms;
using IBMMobilePush.Forms;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Sample
{
	public class App : Application
	{
		public App (JObject jsonAction = null, JObject jsonPayload = null, String attribution = null, int id = 0)
		{
			// The root page of your application
			MainPage = new NavigationPage( new MainPage() );

			// Custom Actions
			SDK.Instance.RegisterAction ("sendEmail", new EmailAction ());
			SDK.Instance.RegisterAction ("calendar", new CalendarAction ());
			SDK.Instance.RegisterAction ("snooze", new SnoozeAction ());
			SDK.Instance.RegisterAction ("displayWebView", new WebViewAction ());

			// Inbox Plugin
			SDK.Instance.RegisterAction ("openInboxMessage", new InboxAction ());
			SDK.Instance.RegisterInboxTemplate ("default", new DefaultInboxTemplate ());
			SDK.Instance.RegisterInboxTemplate ("post", new PostInboxTemplate ());

			// InApp Plugin
			SDK.Instance.RegisterInAppTemplate ("video", new VideoInAppTemplate ());
			SDK.Instance.RegisterInAppTemplate ("image", new ImageInAppTemplate ());
			SDK.Instance.RegisterInAppTemplate ("default", new BannerInAppTemplate ());

			if (jsonAction != null)
			{
				SDK.Instance.ExecuteAction(jsonAction, jsonPayload, attribution, id);
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

