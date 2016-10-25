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
using Xamarin.Forms;
using IBMMobilePush.Forms;
using System.Linq;

namespace Sample
{
	public partial class InboxPage : ContentPage
	{
		public delegate void Completed();
		public ObservableCollection<InboxMessage> Messages = new ObservableCollection<InboxMessage> ();

		public void SyncMessages(Completed completed)
		{
			SDK.Instance.FetchInboxMessages ((newMessages) => {
				if(Messages == null)
					return;

				for(var index=0; index<newMessages.Length; index++)
				{
					var newMessage = newMessages[index];
					var existingMessage = Messages.Where(m => m.InboxMessageId.Equals(newMessage.InboxMessageId)).Select(m=>m).FirstOrDefault();

					if(existingMessage == null)
					{
						// new message isn't yet in Messages collection, insert it
						Messages.Insert(index, newMessage);
					}
					else
					{
						// new message is in Messages collection, update it
						var i = Messages.IndexOf(existingMessage);
						Messages[i] = newMessage;
					}
				}

				// Delete message if not in newMessages
				var delete = new List<InboxMessage>();
				foreach(var existingMessage in Messages)
				{
					var newMessage = newMessages.Where(m=>m.InboxMessageId.Equals(existingMessage.InboxMessageId)).Select(m=>m).FirstOrDefault();
					if(newMessage == null)
					{
						delete.Add(existingMessage);
					}
				}
				foreach(var existingMessage in delete)
					Messages.Remove(existingMessage);

				if(completed != null)
					completed();
				
			}, false);
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			TranslationX = 1000;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing ();
			TranslationX = 0;
		}

		public InboxPage()
		{
			InitializeComponent ();

			InboxMessages.HasUnevenRows = true;
			InboxMessages.ItemsSource = Messages;
			InboxMessages.ItemTemplate = new DataTemplate (typeof(InboxDataTemplate));
			InboxMessages.ItemTapped += (object sender, ItemTappedEventArgs e) => {
				var message = e.Item as InboxMessage;

				// Update Message List read status
				message.IsRead=true;
				var index = Messages.IndexOf(message);
				Messages[index]=message;

				Navigation.PushAsync( new InboxMessagePage(message, this) );

				InboxMessages.SelectedItem=null;
			};

			SDK.Instance.InboxMessagesUpdate += () => {
				SyncMessages(null);
			};

			// Get initial list of messages
			SDK.Instance.FetchInboxMessages ((newMessages) => {
				SyncMessages( () => {
					SDK.Instance.SyncInboxMessages();
				});

			}, true);

			SyncButton.Clicked += (object sender, EventArgs e) => {
				SDK.Instance.SyncInboxMessages();
			};
		}
	}
}