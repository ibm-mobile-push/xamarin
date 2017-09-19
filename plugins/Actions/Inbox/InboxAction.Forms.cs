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
		IOpenAppAction openApp;
		public InboxAction()
		{
			openApp = DependencyService.Get<IOpenAppAction>();
		}

		public override void HandleAction (JObject action, JObject payload, string attribution, string mailingId, int id)
		{
            var inboxMessageId = action["inboxMessageId"];
            if(inboxMessageId == null)
            {
				var richContentId = action["value"];
				SDK.Instance.FetchInboxMessageWithRichContentId(richContentId.ToString(), (message) => {
                    if (message != null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var page = new InboxMessagePage(message, null);
                            Application.Current.MainPage.Navigation.PushModalAsync(page);
                        });
                    }
				});
			}
            else
            {
				SDK.Instance.FetchInboxMessage(inboxMessageId.ToString(), (message) => {
                    if (message != null)
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
	}
}

