﻿/*
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

        InboxTemplateCell generateCell(InboxMessage inboxMessage) {
            var template = SDK.Instance.RegisteredInboxTemplate(inboxMessage.TemplateName);
            if (template == null)
            {
                return null;
            }
            var cell = template.MessageCell(inboxMessage) as InboxTemplateCell;
            cell.InboxMessage = inboxMessage;
            cell.Tapped += MessageCell_Tapped;
            return cell;
        }

		public void SyncMessages(Completed completed)
		{
			SDK.Instance.FetchInboxMessages ((newInboxMessages) => {
                var inboxMessageCells = new List<InboxTemplateCell>();

                foreach (var inboxMessage in newInboxMessages)
                {
                    var cell = generateCell(inboxMessage);
                    inboxMessageCells.Add(cell);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    InboxMessages.Clear();
                    InboxMessages.Add(inboxMessageCells);
                });

				if(completed != null)
					completed();
				
			}, false);
		}

		void MessageCell_Tapped(object sender, EventArgs e)
		{
			var cell = sender as InboxTemplateCell;
            if (cell != null)
            {
                var message = cell.InboxMessage;

                // Update Message List read status
                message.IsRead = true;

                Navigation.PushAsync(new InboxMessagePage(message, this));
            }
		}


		protected override void OnDisappearing ()
		{
            foreach(var cell in InboxMessages) {
                var templatecell = cell as InboxTemplateCell;
                if (templatecell != null)
                {
                    templatecell.CellDisappearing();
                }
            }
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

        public InboxTemplateCell FindMessage(InboxMessage message)
        {
            foreach(var cell in InboxMessages) {
                var templatecell = cell as InboxTemplateCell;
                if(templatecell != null) {
                    if(templatecell.InboxMessage.InboxMessageId == message.InboxMessageId) {
                        return templatecell;
                    }
                }
            }
            return null;
        }

        public InboxMessage MessageAtIndex(int index)
        {
            var cell = InboxMessages[index] as InboxTemplateCell;
            if(cell != null) {
                return cell.InboxMessage;
            }
            return null;
        }

        public int MessageIndex(InboxMessage message) {
            foreach (var cell in InboxMessages)
            {
                var templatecell = cell as InboxTemplateCell;
                if (templatecell != null)
                {
                    if (templatecell.InboxMessage.InboxMessageId == message.InboxMessageId)
                    {
                        return InboxMessages.IndexOf(cell);
                    }
                }
            }
            return -1;
        }

        public int MessageCount()
        {
            return InboxMessages.Count();
        }

        public void UpdateCell(int index) {
            var templatecell = InboxMessages[index] as InboxTemplateCell;
            if (templatecell != null) {
                InboxMessages[index] = generateCell(templatecell.InboxMessage);
            }

        }
	}
}