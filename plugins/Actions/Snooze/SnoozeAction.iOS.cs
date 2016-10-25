/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using UIKit;
using Foundation;
using IBMMobilePush.iOS;
using IBMMobilePush.Forms;

[assembly: Dependency(typeof(Sample.iOS.SnoozeActionImpl))]
namespace Sample.iOS
{
	public class SnoozeActionImpl : ISnoozeAction
	{
		public SnoozeActionImpl ()
		{
		}

		public void Snooze(JObject payload, int minutes, int id, string attribution)
		{
			var notification = new UILocalNotification ();

			var jsonString = payload.ToString ();
			var jsonData = NSData.FromString (jsonString);
			NSError error = null;
			var userInfo = (NSDictionary) NSJsonSerialization.Deserialize (jsonData, 0, out error);
			if (error != null) {
				Logging.Error ("Could not deserialize json data for snooze");
				return;
			}
			notification.UserInfo = userInfo;

			var aps = payload ["aps"];
			if (aps == null) {
				Logging.Error ("Could not find aps in snooze payload");
				return;
			}

			if (aps ["category"] != null) {
				notification.Category = aps ["category"].ToString ();
			}

			if (aps ["sound"] != null) {
				notification.SoundName = aps ["sound"].ToString ();
			}

			if (aps ["badge"] != null) {
				notification.ApplicationIconBadgeNumber = int.Parse(aps["badge"].ToString());
			}

			if (aps ["alert"] != null && aps["alert"].Type == JTokenType.Object && aps ["alert"] ["action-loc-key"] != null) {
				notification.AlertAction = aps["alert"] ["action-loc-key"].ToString();
				notification.HasAction = true;
			}

			notification.AlertBody = MCESdk.SharedInstance ().ExtractAlert ((NSDictionary) userInfo [ new NSString( "aps" )]);
			notification.FireDate = NSDate.FromTimeIntervalSinceNow (minutes * 60);
			UIApplication.SharedApplication.ScheduleLocalNotification (notification);
		}
	}
}
