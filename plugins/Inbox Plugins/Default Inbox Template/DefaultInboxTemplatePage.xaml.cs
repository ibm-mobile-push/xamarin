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
using Xamarin.Forms;
using IBMMobilePush.Forms;

namespace Sample
{
	public partial class DefaultInboxTemplateView : ContentView
	{
		public DefaultInboxTemplateView(InboxMessage message, RichContent content)
		{
			InitializeComponent ();

			var messagePreview = content.Content["messagePreview"];
			var messageDetails = content.Content["messageDetails"];
			var subject = messagePreview ["subject"];
			var richContent = messageDetails ["richContent"];
			var actions = content.Content ["actions"];

			Content.Source = new HtmlWebViewSource () { Html = richContent.ToString() };
			Content.Navigating += (object sender, WebNavigatingEventArgs e) => {
				var url = new Uri(e.Url);
				if(url.Scheme == "actionid")
				{
					var action = actions[url.PathAndQuery];
					if(action != null)
					{
						var attributes = new Dictionary<string, string>() { 
							{"richContentId", content.RichContentId},
							{"inboxMessageId", message.InboxMessageId}
						};
						SDK.Instance.ExecuteInboxAction(action, message.Attribution, attributes);
					}
					e.Cancel = true;
				}
			};
			Subject.Text = subject.ToString();
			Date.Text = message.SendDate.ToString("f");
		}
	}
}

