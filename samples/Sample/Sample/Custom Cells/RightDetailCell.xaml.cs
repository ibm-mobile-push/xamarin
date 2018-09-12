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
	public partial class RightDetailCell : ViewCell
	{
		string text;
		public string Text { get { return text; } set { text = value; TextLabel.Text = text; } }

		string detail;
		public string Detail { 
			get { 
				return detail; 
			} 
			set { 
				detail = value; 
				Device.BeginInvokeOnMainThread (() => {
					DetailLabel.Text = detail; 
				});
			} 
		}

		public RightDetailCell ()
		{
			InitializeComponent ();
		}
	}
}

