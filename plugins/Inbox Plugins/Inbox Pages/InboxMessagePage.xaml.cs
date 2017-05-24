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
using System.Collections.ObjectModel;
using System.Linq;

using IBMMobilePush.Forms;

using Xamarin.Forms;

namespace Sample
{
	public partial class InboxMessagePage : ContentPage
	{
		InboxMessage FindMessage(InboxMessage message, InboxPage inbox)
		{
			return inbox.Messages.Where(m => m.InboxMessageId.Equals(message.InboxMessageId)).Select(m=>m).FirstOrDefault();
		}

		public InboxMessagePage (InboxMessage message, InboxPage inbox)
		{
			InitializeComponent ();

			SDK.Instance.ReadInboxMessage(message);

			if (inbox == null) {
				DismissLayout.Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 0);
				Dismiss.Clicked += (object sender, EventArgs e) => {
					Navigation.PopModalAsync();
				};
			} else {
				Layout.Children.Clear ();
			}

			var handler = SDK.Instance.RegisteredInboxTemplate(message.TemplateName);
			if(handler != null && handler.ShouldDisplayInboxMessage(message))
			{
				var view = handler.MessageView(message);
				Layout.Children.Add (view, 0, 1);
			}

			if (inbox == null || inbox.Messages.IndexOf (message) == 0) {
				ToolbarItems.Remove (PrevButton);
			}
			if (inbox == null || inbox.Messages.IndexOf (message) == inbox.Messages.Count-1) {
				ToolbarItems.Remove (NextButton);
			}

			PrevButton.Clicked += (object sender, EventArgs e) => {
				int index = inbox.Messages.IndexOf (FindMessage(message, inbox)) - 1;
				Navigation.PushAsync(new InboxMessagePage(inbox.Messages[index], inbox));
				Navigation.RemovePage(this);
			};
			NextButton.Clicked += (object sender, EventArgs e) => {
				int index = inbox.Messages.IndexOf (FindMessage(message, inbox)) + 1;
				Navigation.PushAsync(new InboxMessagePage(inbox.Messages[index], inbox));
				Navigation.RemovePage(this);
			};
			TrashButton.Clicked += (object sender, EventArgs e) => {
				SDK.Instance.DeleteInboxMessage(message);
				if (inbox != null)
				{
					inbox.Messages.Remove(FindMessage(message, inbox));
				}
				SDK.Instance.SyncInboxMessages();
				Navigation.PopAsync();
			};

			var attributes = new Dictionary<string, object>() {
							{"richContentId", message.RichContentId},
							{"inboxMessageId", message.InboxMessageId}
						};
			SDK.Instance.QueueAddEvent("messageOpened", "inbox", DateTimeOffset.Now, message.Attribution, message.MailingId, attributes, true);
		}
	}
}

