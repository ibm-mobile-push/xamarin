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
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Sample
{
	// Email can't be sent directly from a Xamarin.Forms app, it must go through the native implementations
	public interface IEmailAction
	{
		void SendEmail(string subject, string body, string recipient);
	}

	public class EmailAction : PushAction
	{
		IEmailAction emailSender;
		public EmailAction ()
		{
			emailSender = DependencyService.Get<IEmailAction> ();
		}

		public override void HandleAction (JObject action, JObject payload, string attribution, string mailingId, int id)
		{
			JToken values = null;
			string subject = null;
			string body = null;
			string recipient = null;
			if (action.TryGetValue("value", out values))
			{
				subject = values["subject"].ToString();
				body = values["body"].ToString();
				recipient = values["recipient"].ToString();
			}
			else
			{
				subject = action["subject"].ToString();
				body = action["body"].ToString();
				recipient = action["recipient"].ToString();
			}

			if (subject != null && body != null && recipient != null)
				emailSender.SendEmail(subject, body, recipient);
			else
				Logging.Error("Email Action can not complete, can't find subject, body or recipient in payload.");
		}
	}
}

