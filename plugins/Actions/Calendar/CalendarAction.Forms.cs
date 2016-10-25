/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using IBMMobilePush.Forms;
using System;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;

namespace Sample
{
	// Calendar can't be accessed directly from a Xamarin.Forms app, it must go through the native implementations
	public interface ICalendarAction
	{
		void AddEvent(string title, string description, DateTimeOffset startDate, DateTimeOffset endDate, bool interactive);
	}

	public class CalendarAction : PushAction
	{
		ICalendarAction calendar;
		public CalendarAction ()
		{
			calendar = DependencyService.Get<ICalendarAction> ();
		}

		public override void HandleAction (JObject action, JObject payload, string attribution, int id)
		{
			var title = action.GetValue ("title").ToString();
			var startDate = DateTimeOffset.Parse( action.GetValue ("startDate").ToString() );
			var endDate = DateTimeOffset.Parse( action.GetValue ("endDate").ToString () );
			var description = action.GetValue ("description").ToString ();

			var interactive = false;
			if (action.GetValue ("interactive") != null) {
				interactive = bool.Parse (action.GetValue ("interactive").ToString ());
			}

			calendar.AddEvent (title, description, startDate, endDate, interactive);
		}
	}
}

