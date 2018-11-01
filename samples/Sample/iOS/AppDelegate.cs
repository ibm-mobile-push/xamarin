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
using System.Linq;
using Foundation;
using UIKit;
using IBMMobilePush.iOS;
using IBMMobilePush.Forms;
using IBMMobilePush.Forms.iOS;
using FFImageLoading.Forms.Touch;
using UserNotifications;
using Xamarin.Forms;
using Xamarin;

namespace Sample.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// Avoid Some Mono Crashes
			Environment.SetEnvironmentVariable("MONO_XMLSERIALIZER_THS", "no");
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            new FreshEssentials.iOS.AdvancedFrameRendereriOS();

            Forms.Init ();
			FormsMaps.Init();

			App.ScreenWidth = UIScreen.MainScreen.Bounds.Width;
			App.ScreenHeight = UIScreen.MainScreen.Bounds.Height;

			LoadApplication (new App ());

			// IBMMobilePush Integration
			MCESdk.SharedInstance ().HandleApplicationLaunch ();

			// Setup Push Message Permission
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				Logging.Verbose("iOS > 10");

				UNUserNotificationCenter.Current.Delegate = MCENotificationDelegate.SharedInstance();
				UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.CarPlay | UNAuthorizationOptions.Sound, (approved, err) =>
				{
					InvokeOnMainThread(() =>
					{
						UIApplication.SharedApplication.RegisterForRemoteNotifications();
					});
				});
			}
			else if (UIDevice.CurrentDevice.CheckSystemVersion(8,0))
			{
				Logging.Verbose ("iOS > 8");
				var settings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Sound |
					UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);

				UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			}
			else
			{
				Logging.Verbose ("iOS < 8");
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Badge |
					UIRemoteNotificationType.Sound | UIRemoteNotificationType.Alert);
			}

			return base.FinishedLaunching (app, options);
		}

		// IBMMobilePush Integration
		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			MCEEventService.SharedInstance ().SendPushEnabledEvent ();
		}

		// IBMMobilePush Integration
		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			MCEEventService.SharedInstance ().SendPushEnabledEvent ();
		}

		// IBMMobilePush Integration
		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			MCESdk.SharedInstance().RegisterDeviceToken(deviceToken);
			Logging.Verbose ("deviceToken: " + MCEApiUtil.DeviceToken (deviceToken));
			MCEEventService.SharedInstance ().SendPushEnabledEvent ();
		}

		// IBMMobilePush Integration
		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
            MCEInAppManager.SharedInstance().ProcessPayload(userInfo);
			MCESdk.SharedInstance().PresentOrPerformNotification(userInfo);
		}

		// IBMMobilePush Integration
		public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
		{
            MCEInAppManager.SharedInstance().ProcessPayload(notification.UserInfo);
			MCESdk.SharedInstance ().PresentOrPerformNotification (notification.UserInfo);
		}

		// IBMMobilePush Integration
		public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
            MCEInAppManager.SharedInstance().ProcessPayload(userInfo);
			MCESdk.SharedInstance().PresentDynamicCategoryNotification(userInfo);
		}

		// IBMMobilePush Integration
		public override void HandleAction (UIApplication application, String actionIdentifier, NSDictionary remoteNotificationInfo, Action completionHandler)
		{
			MCESdk.SharedInstance ().ProcessCategoryNotification (remoteNotificationInfo, actionIdentifier);
		}

		// IBMMobilePush Integration
		public override void HandleAction (UIApplication application, String actionIdentifier, UILocalNotification localNotification, Action completionHandler)
		{
			MCESdk.SharedInstance ().ProcessDynamicCategoryNotification (localNotification.UserInfo, actionIdentifier, null);
		}

		public override void HandleAction(UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, NSDictionary responseInfo, Action completionHandler)
		{
			
		}

		public override void HandleAction(UIApplication application, string actionIdentifier, UILocalNotification localNotification, NSDictionary responseInfo, Action completionHandler)
		{
			NSString response = null;
			if(responseInfo.ContainsKey(UIUserNotificationAction.ResponseTypedTextKey))
				response = (NSString)responseInfo.ObjectForKey(UIUserNotificationAction.ResponseTypedTextKey);
			MCESdk.SharedInstance().ProcessDynamicCategoryNotification(localNotification.UserInfo, actionIdentifier, response);
		}
	}
}

