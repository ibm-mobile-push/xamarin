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

namespace Sample
{
	public partial class DefaultInboxTemplateCell : InboxTemplateCell
	{
		public DefaultInboxTemplateCell (InboxMessage message, RichContent content)
		{
			InitializeComponent ();
			var messagePreview = content.Content["messagePreview"];
			var subject = messagePreview ["subject"];
			var previewContent = messagePreview ["previewContent"];

			Content.Text = previewContent.ToString();
			Subject.Text = subject.ToString();

			if (message.ExpirationDate.Subtract (DateTimeOffset.Now).TotalSeconds < 0) {
				Date.Text = "Expired " + message.ExpirationDate.ToString ("d");
				Date.TextColor = Color.Red;
			} else {
				Date.Text = message.SendDate.ToString("d");
				Date.TextColor = Color.Blue;
			}

			if (message.IsRead) {
				Subject.FontAttributes = FontAttributes.None;
			}
			else {
				Subject.FontAttributes = FontAttributes.Bold;
			}
		}

		public override void CellDisappearing()
		{
		}

	}
}

