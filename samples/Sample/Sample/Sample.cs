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
using Newtonsoft.Json.Linq;

namespace Sample
{
	public class App : Application
	{
		public static double ScreenHeight;
		public static double ScreenWidth;
		
		public App ()
		{
			// The root page of your application
			MainPage = new NavigationPage( new MainPage() );

            Device.BeginInvokeOnMainThread(() =>
            {
                // Custom Actions
                SDK.Instance.RegisterAction("sendEmail", new EmailAction());
                SDK.Instance.RegisterAction("calendar", new CalendarAction());
                SDK.Instance.RegisterAction("snooze", new SnoozeAction());

                // iOS and Android use different names for this plugin
                var displayWebAction = new WebViewAction();
                SDK.Instance.RegisterAction("displayWebView", displayWebAction);
                SDK.Instance.RegisterAction("displayweb", displayWebAction);

                // Inbox Plugin
                SDK.Instance.RegisterAction("openInboxMessage", new InboxAction());
                SDK.Instance.RegisterInboxTemplate("default", new DefaultInboxTemplate());
                SDK.Instance.RegisterInboxTemplate("post", new PostInboxTemplate());

                // InApp Plugin
                SDK.Instance.RegisterInAppTemplate("video", new VideoInAppTemplate());
                SDK.Instance.RegisterInAppTemplate("image", new ImageInAppTemplate());
                SDK.Instance.RegisterInAppTemplate("default", new BannerInAppTemplate());
            });
		}
	}
}

