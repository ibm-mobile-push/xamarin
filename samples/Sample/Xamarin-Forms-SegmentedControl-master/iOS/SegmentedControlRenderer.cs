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
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using UIKit;

[assembly:ExportRenderer(typeof(Sample.SegmentedControl), typeof(Sample.iOS.SegmentedControlRenderer))]
namespace Sample.iOS
{
	public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, UISegmentedControl>
	{
		public SegmentedControlRenderer ()
		{
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e) 
		{
			base.OnElementPropertyChanged (sender, e);

			if (this.Element == null || this.Control == null)
				return;

			if (e.PropertyName == SegmentedControl.SelectedValueProperty.PropertyName) {
				for (var i = 0; i < Control.NumberOfSegments; i++) {
					if (Control.TitleAt (i).Equals (Element.SelectedValue)) {
						Control.SelectedSegment = i;
					}
				}
			}
		}

		protected override void OnElementChanged (ElementChangedEventArgs<SegmentedControl> e)
		{
			base.OnElementChanged (e);

			var newControl = new UISegmentedControl ();
            if (e.NewElement != null)
            {
                for (var i = 0; i < e.NewElement.Children.Count; i++)
                {
                    newControl.InsertSegment(e.NewElement.Children[i].Text, i, false);
                    if (e.NewElement.Children[i].Text.Equals(Element.SelectedValue))
                    {
                        newControl.SelectedSegment = i;
                    }
                }

                newControl.ValueChanged += (sender, eventArgs) =>
                {
                    e.NewElement.SelectedValue = newControl.TitleAt(newControl.SelectedSegment);
                };

                SetNativeControl(newControl);
            }
		}
	}
}