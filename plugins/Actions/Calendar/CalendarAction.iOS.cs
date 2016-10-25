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
using EventKit;
using EventKitUI;
using UIKit;
using Foundation;
using System.Diagnostics;
using IBMMobilePush.Forms;
using IBMMobilePush.Forms.iOS;

[assembly: Dependency(typeof(Sample.iOS.CalendarActionImpl))]
namespace Sample.iOS
{ 
	public class CalendarActionImpl : ICalendarAction
	{
		public CalendarActionImpl ()
		{
		}

		public async void AddEvent(string title, string description, DateTimeOffset startDate, DateTimeOffset endDate, bool interactive)
		{
			var store = new EKEventStore ();
			var granted = await store.RequestAccessAsync (EKEntityType.Event);

			if (granted.Item2 != null) {
				Logging.Error ("Could not add to calendar " + granted.Item2.LocalizedDescription);
				return;
			}
				
			if (granted.Item1 == false) {
				Logging.Error ("Could not get access to EventKit, can't add to calendar");
				return;
			}

			var newEvent = EKEvent.FromStore (store);
			newEvent.Calendar=store.DefaultCalendarForNewEvents;
			newEvent.Title = title;
			newEvent.Notes = description;

			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( new DateTime(2001, 1, 1, 0, 0, 0) );

			newEvent.StartDate = IBMMobilePushImpl.ConvertDate (startDate, NSDate.Now);
			newEvent.EndDate = IBMMobilePushImpl.ConvertDate(endDate, NSDate.Now);

			if (interactive) {
				var controller = new EKEventEditViewController ();
				controller.Event = newEvent;
				controller.EventStore = store;
				var topViewController = Utility.FindCurrentViewController ();
				controller.Completed += (object sender, EKEventEditEventArgs e) => {
					topViewController.DismissViewController(true, null);
				};
				topViewController.PresentViewController(controller, true, null);
			} else {
				NSError error = null;
				store.SaveEvent (newEvent, EKSpan.ThisEvent, out error);
				if (error!=null) {
					Logging.Error ("Could not save event: " + error.LocalizedDescription);
				}
			}
		}
	}
}

