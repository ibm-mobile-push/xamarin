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
using IBMMobilePush.Droid;
using IBMMobilePush.Forms;
using Android.Content;
using Android.App;
using Newtonsoft.Json.Linq;

[assembly: Dependency(typeof(Sample.Droid.SnoozeActionImpl))]
namespace Sample.Droid
{
	public class SnoozeActionImpl : ISnoozeAction
	{
		public SnoozeActionImpl ()
		{
		}


		public void Snooze(JObject payload, int minutes, int id, string attribution)
		{
			Logging.Verbose ("snooze payload: " + payload);
			var context = Xamarin.Forms.Forms.Context;

			AlarmManager mgr = (AlarmManager) context.GetSystemService(Android.Content.Context.AlarmService);
			Intent intent =  new Intent(context, typeof(SnoozeIntentService));
			intent.PutExtra("payload", payload.ToString());
			intent.PutExtra("attribution", attribution);
			intent.PutExtra("id", id);
			PendingIntent pi = PendingIntent.GetService(context, 0, intent, 0);
			(new SnoozeIntentService()).ScheduleSnooze(mgr, pi, minutes);
			NotificationManager notificationManager = (NotificationManager) context.GetSystemService("notification");
			notificationManager.Cancel(id);

		}
	}
}

