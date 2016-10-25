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
	public partial class EventsPage : ContentPage
	{
		public EventsPage ()
		{
			InitializeComponent ();

			SendClient.Tapped += (object sender, EventArgs e) => {
				SendClient.Status = RightStatus.Sending;
				SDK.Instance.AddEvent("appOpened", "simpleNotification", DateTimeOffset.Now, "SendClient", new Dictionary<string, object>(), (success, name, type, timestamp, attribution, attributes) => {
					SendClient.Status = success ? RightStatus.Received : RightStatus.Failed;
				});
			};
			SendQueue.Tapped += (object sender, EventArgs e) => {
				SendQueue.Status = RightStatus.Sending;
				SDK.Instance.QueueAddEvent("appOpened", "simpleNotification", DateTimeOffset.Now, "SendQueue", new Dictionary<string, object>(), true);
			};
			QueueEvent.Tapped += (object sender, EventArgs e) => {
				QueueEvent.Status = RightStatus.Queued;
				SDK.Instance.QueueAddEvent("appOpened", "simpleNotification", DateTimeOffset.Now, "QueueEvent", new Dictionary<string, object>(), false);
			};
			FlushQueue.Tapped += (object sender, EventArgs e) => {
				FlushQueue.Status = RightStatus.Sending;
				SDK.Instance.QueueAddEvent("appOpened", "simpleNotification", DateTimeOffset.Now, "FlushQueue", new Dictionary<string, object>(), false);
				SDK.Instance.FlushEventQueue();
			};
		}

		public void QueueCallback(bool success, string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,object> attributes)
		{
			switch (attribution) {
			case "SendQueue":
				SendQueue.Status = success ? RightStatus.Received : RightStatus.Failed;
				break;
			case "QueueEvent":
				QueueEvent.Status = success ? RightStatus.Received : RightStatus.Failed;
				break;
			case "FlushQueue":
				FlushQueue.Status = success ? RightStatus.Received : RightStatus.Failed;
				break;
			}
		}

		protected override void OnDisappearing ()
		{
			SDK.Instance.EventQueueResults -= QueueCallback;
		}

		protected override void OnAppearing()
		{
			SDK.Instance.EventQueueResults += QueueCallback;
		}

	}
}

