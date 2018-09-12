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
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Graphics;
using Android.Views;
using Android.Graphics.Drawables;
using Android;
using System.ComponentModel;

[assembly:ExportRenderer (typeof(Sample.SegmentedControl), typeof(Sample.Droid.SegmentedControlRenderer))]
namespace Sample.Droid
{
	public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, RadioGroup>
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
				for (var i = 0; i < Control.ChildCount; i++) {
					RadioButton button = (RadioButton)Control.GetChildAt (i);
					if (button.Text.Equals (Element.SelectedValue)) {
						button.Checked = true;
					}
				}
			}
		}

		protected override void OnElementChanged (ElementChangedEventArgs<SegmentedControl> e)
		{
			base.OnElementChanged (e);

			var layoutInflater = (LayoutInflater)Context.GetSystemService (Context.LayoutInflaterService);

			var g = new RadioGroup (Context);
			g.Orientation = Orientation.Horizontal;
			g.CheckedChange += (sender, eventArgs) => {
				var rg = (RadioGroup)sender;
				if (rg.CheckedRadioButtonId != -1) {
					var id = rg.CheckedRadioButtonId;
					var radioButton = rg.FindViewById (id);
					var radioId = rg.IndexOfChild (radioButton);
					var btn = (RadioButton)rg.GetChildAt (radioId);
					var selection = (String)btn.Text;
					e.NewElement.SelectedValue = selection;

					for(var i = 0; i < rg.ChildCount; i++)
					{
						var r = (RadioButton)rg.GetChildAt(i);
                        if(r.Id == rg.CheckedRadioButtonId) {
                            r.Checked = true;
                        } else {
                            r.Checked = false;
                        }
					}
				}
			};

			for (var i = 0; i < e.NewElement.Children.Count; i++) {
				var o = e.NewElement.Children [i];
				var v = (SegmentedControlButton)layoutInflater.Inflate (Resource.Layout.SegmentedControl, null);
				v.Text = o.Text;

				if (v.Text.Equals (Element.SelectedValue)) {
					v.Checked = true;
				}

				if (i == 0)
					v.SetBackgroundResource (Resource.Drawable.segmented_control_first_background);
				else if (i == e.NewElement.Children.Count - 1)
					v.SetBackgroundResource (Resource.Drawable.segmented_control_last_background);
				g.AddView (v);
			}

			SetNativeControl (g);
		}
	}

	public class SegmentedControlButton : RadioButton
	{
		private int lineHeightSelected;
		private int lineHeightUnselected;

		private Paint linePaint;

		public SegmentedControlButton (Context context) : this (context, null)
		{
		}

		public SegmentedControlButton (Context context, IAttributeSet attributes) : this (context, attributes, Resource.Attribute.segmentedControlOptionStyle)
		{
		}

		public SegmentedControlButton (Context context, IAttributeSet attributes, int defStyle) : base (context, attributes, defStyle)
		{
			Initialize (attributes, defStyle);
		}

		private void Initialize (IAttributeSet attributes, int defStyle)
		{
			var a = this.Context.ObtainStyledAttributes (attributes, Resource.Styleable.SegmentedControlOption, defStyle, Resource.Style.SegmentedControlOption);

			var lineColor = a.GetColor (Resource.Styleable.SegmentedControlOption_lineColor, 0);
			linePaint = new Paint ();
			linePaint.Color = lineColor;

			lineHeightUnselected = a.GetDimensionPixelSize (Resource.Styleable.SegmentedControlOption_lineHeightUnselected, 0);
			lineHeightSelected = a.GetDimensionPixelSize (Resource.Styleable.SegmentedControlOption_lineHeightSelected, 0);

			a.Recycle ();
		}
			
		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			if (linePaint.Color != 0 && (lineHeightSelected > 0 || lineHeightUnselected > 0)) {
				var lineHeight = Checked ? lineHeightSelected : lineHeightUnselected;

				if (lineHeight > 0) {
					var rect = new Rect (0, Height - lineHeight, Width, Height);
					canvas.DrawRect (rect, linePaint);
				}
			}
		}
	}
}