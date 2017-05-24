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

namespace Sample
{
	public partial class NormalEntryCell : ViewCell
	{
		public NormalEntryCell ()
		{
			InitializeComponent ();
			TextEntry.TextChanged += (sender, e) => {
				Text = ((Entry) sender).Text;
				if(TextChanged != null)
				{
					TextChanged(sender, e);
				}
			};
			View.SizeChanged += (object sender, EventArgs e) => {

				SizeRequest sizeRequest = TextLabel.GetSizeRequest(Layout.Width, Double.PositiveInfinity);
				TextLabel.WidthRequest = sizeRequest.Request.Width;
				TextEntry.WidthRequest = View.Width - sizeRequest.Request.Width - Layout.Padding.Left - Layout.Padding.Right - 10;
			};
		}
			
		public delegate void ChangedDelegate(object sender, EventArgs e);
		public ChangedDelegate TextChanged { get; set; }

		string text;
		public string Text { get { return text; } set { text = value; TextEntry.Text = text; } }

		string label;
		public string Label { get { return label; } set { label = value; TextLabel.Text = label; } }
	}
}

