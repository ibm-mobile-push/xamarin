/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using IBMMobilePush.Forms;
using IBMMobilePush.Forms.Droid;
using Xamarin.Forms;
using System;
using Android;
using Android.Content;

[assembly: Dependency(typeof(Sample.Droid.CalendarActionImpl))]
namespace Sample.Droid
{
	public class CalendarActionImpl : ICalendarAction
	{
		public CalendarActionImpl ()
		{
		}

		public async void AddEvent(string title, string description, DateTimeOffset startDate, DateTimeOffset endDate, bool interactive)
		{
			Intent it = new Intent(Intent.ActionCloseSystemDialogs);
			Xamarin.Forms.Forms.Context.SendBroadcast(it);
			Intent intent = new Intent(Intent.ActionInsert);
			intent.SetType("vnd.android.cursor.item/event");


			intent.AddFlags(ActivityFlags.ClearTask);
			intent.PutExtra(Android.Provider.CalendarContract.ExtraEventBeginTime, IBMMobilePushImpl.ConvertDate(startDate, new Java.Util.Date()).Time );
			intent.PutExtra(Android.Provider.CalendarContract.ExtraEventEndTime, IBMMobilePushImpl.ConvertDate(endDate, new Java.Util.Date()).Time );
			intent.PutExtra(Android.Provider.CalendarContract.EventsColumns.Title, title );
			intent.PutExtra(Android.Provider.CalendarContract.EventsColumns.Description, description );

			Xamarin.Forms.Forms.Context.StartActivity (intent);
		}
	}
}

