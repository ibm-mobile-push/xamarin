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
			var richContentId = action["value"].ToString();
			SDK.Instance.FetchInboxMessageWithRichContentId (richContentId, (message) => {
				Device.BeginInvokeOnMainThread(() =>
				{
					Application.Current.MainPage.Navigation.PushModalAsync(new InboxMessagePage(message, null));
				});
			});
		}
	}
}

