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
using System.Collections.Generic;

namespace Sample
{
	public class PostInboxTemplate : IInboxTemplate
	{
		public PostInboxTemplate ()
		{
		}

		public bool ShouldDisplayInboxMessage (InboxMessage message)
		{
			return true;
		}

		public ViewCell MessageCell (InboxMessage message)
		{
			return new PostInboxViewCell (message);
		}

		public View MessageView (InboxMessage message)
		{
			return new ScrollView () { Content = new PostInboxTemplateView (message, null) };
		}
	}

	public class PostInboxViewCell : InboxTemplateCell
	{
		PostInboxTemplateView PostView;

		public PostInboxViewCell(InboxMessage message)
		{
			PostView = new PostInboxTemplateView (message, this);
			View = PostView;
		}

		public override void CellDisappearing()
		{
			PostView.Disappearing ();
		}
	}
}

