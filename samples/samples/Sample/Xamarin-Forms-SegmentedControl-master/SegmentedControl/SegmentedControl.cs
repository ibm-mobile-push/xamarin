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
using System.Collections.Generic;

namespace Sample
{
	public class SegmentedControl : View, IViewContainer<SegmentedControlOption>
	{
		public event EventHandler Change;

		public static readonly BindableProperty SelectedValueProperty =
			BindableProperty.Create<SegmentedControl,string> (p => p.SelectedValue, "");

		public string SelectedValue {
			get { 
				return (string)GetValue (SelectedValueProperty); 
			}
			set { 
				SetValue (SelectedValueProperty, value); 
				if (Change != null) {
					Change (this, EventArgs.Empty);
				}
			}
		}

		public IList<SegmentedControlOption> Children { get; set; }

		public SegmentedControl ()
		{
			Children = new List<SegmentedControlOption> ();
		}

		public event ValueChangedEventHandler ValueChanged;

		public delegate void ValueChangedEventHandler (object sender, EventArgs e);
	}

	public class SegmentedControlOption:View
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create<SegmentedControlOption, string> (p => p.Text, "");

		public string Text {
			get{ return (string)GetValue (TextProperty); }
			set{ SetValue (TextProperty, value); }
		}

		public SegmentedControlOption ()
		{
		}
	}
}