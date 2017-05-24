using System;
using System.Collections.Generic;
using Xamarin.Forms;
using IBMMobilePush.Forms;

namespace Sample
{
	public partial class InboxDataTemplate : ViewCell
	{
		InboxTemplateCell MessageCell;

		public InboxDataTemplate ()
		{
			InitializeComponent ();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged ();

			var inboxMessage = BindingContext as InboxMessage;
			if (inboxMessage == null)
				return;
			var template = SDK.Instance.RegisteredInboxTemplate (inboxMessage.TemplateName);
			if (template == null) {
				return;
			}
			MessageCell = template.MessageCell (inboxMessage) as InboxTemplateCell;
			View = MessageCell.View;
			Height = MessageCell.Height;
		}

		override protected void OnDisappearing()
		{
			base.OnDisappearing();
			if(MessageCell != null)
				MessageCell.CellDisappearing ();
		}
	}
}

