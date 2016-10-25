/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using Xamarin.Forms;
using IBMMobilePush.Forms;

namespace Sample
{
	public class DefaultInboxTemplate : IInboxTemplate
	{
		public DefaultInboxTemplate ()
		{
		}

		public bool ShouldDisplayInboxMessage (InboxMessage message)
		{
			return true;
		}

		public ViewCell MessageCell (InboxMessage message, RichContent content)
		{
			return new DefaultInboxTemplateCell (message, content);
		}

		public View MessageView (InboxMessage message, RichContent content)
		{
			return new DefaultInboxTemplateView (message, content);
		}
	}
}

