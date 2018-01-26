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
using Android.Net;
using Java.Util;

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
            if (interactive)
            {
                Intent it = new Intent(Intent.ActionCloseSystemDialogs);
                Xamarin.Forms.Forms.Context.SendBroadcast(it);
                Intent intent = new Intent(Intent.ActionInsert);
                intent.SetType("vnd.android.cursor.item/event");


                intent.AddFlags(ActivityFlags.ClearTask);
                intent.PutExtra(Android.Provider.CalendarContract.ExtraEventBeginTime, Utilities.ConvertDate(startDate, new Java.Util.Date()).Time);
                intent.PutExtra(Android.Provider.CalendarContract.ExtraEventEndTime, Utilities.ConvertDate(endDate, new Java.Util.Date()).Time);
                intent.PutExtra(Android.Provider.CalendarContract.EventsColumns.Title, title);
                intent.PutExtra(Android.Provider.CalendarContract.EventsColumns.Description, description);

                Xamarin.Forms.Forms.Context.StartActivity(intent);
            }
            else
            {
                var eventUriString = "content://com.android.calendar/events";
                var eventValues = new ContentValues();
                eventValues.Put("calendar_id", 1);
                eventValues.Put("title", title);
                eventValues.Put("description", description);
                eventValues.Put("dtstart", Utilities.ConvertDate(startDate, new Java.Util.Date()).Time);
                eventValues.Put("dtend", Utilities.ConvertDate(endDate, new Java.Util.Date()).Time);
                eventValues.Put("eventTimezone", Java.Util.TimeZone.Default.ID);
                Xamarin.Forms.Forms.Context.ContentResolver.Insert(Android.Net.Uri.Parse(eventUriString), eventValues);
            }
		}
	}
}

