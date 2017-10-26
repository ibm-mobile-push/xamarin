/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using IBMMobilePush.Forms;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;

namespace Sample
{
	public interface IOpenAppAction
	{
		void OpenApp();
	}

	public class InboxAction : PushAction
	{
        String inboxMessageId = null;
		IOpenAppAction openApp;
		public InboxAction()
		{
			openApp = DependencyService.Get<IOpenAppAction>();
            SDK.Instance.InboxMessagesUpdate += InboxMessagesUpdate;;
        }

		public override void HandleAction (JObject action, JObject payload, string attribution, string mailingId, int id)
		{
            var inboxMessageId = action["inboxMessageId"];
            if(inboxMessageId != null)
            {
				SDK.Instance.FetchInboxMessage(inboxMessageId.ToString(), (message) => {
                    if (message == null)
                    {
                        this.inboxMessageId = inboxMessageId.ToString();
                        SDK.Instance.SyncInboxMessages();
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var page = new InboxMessagePage(message, null);
                            Application.Current.MainPage.Navigation.PushModalAsync(page);
                        });
                    }
				});
			}
		}

        void InboxMessagesUpdate()
        {
            if (this.inboxMessageId != null)
            {
                SDK.Instance.FetchInboxMessage(this.inboxMessageId, (message) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var page = new InboxMessagePage(message, null);
                        Application.Current.MainPage.Navigation.PushModalAsync(page);
                    });

                    this.inboxMessageId = null;
                });
            }
		}
    }
}

