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

namespace Sample
{
	public class VideoPlayer : View
	{
		public event EventHandler CompleteEvent;

		public event EventHandler PauseEvent;
		public event EventHandler PlayEvent;
		public event EventHandler ContentSizeChange;
		public event EventHandler ClickEvent;

		// Sizing step 4: Once we know the size of the content, we need to resize the Xamarin Element.
		public static readonly BindableProperty ContentSizeProperty = BindableProperty.Create("ContentSize", typeof(Size), typeof(VideoPlayer), new Size(-1.0, -1.0));
		public Size ContentSize { 
			get { return (Size)GetValue (ContentSizeProperty); } 
			set { 
				SetValue (ContentSizeProperty, value); 
				if (ContentSizeChange != null)
					ContentSizeChange (this, EventArgs.Empty);
			} 
		}

		public static readonly BindableProperty PlayingProperty = BindableProperty.Create("Playing", typeof(bool), typeof(VideoPlayer), false);
		public bool Playing { get { return (bool)GetValue (PlayingProperty); } set { SetValue (PlayingProperty, value); } }

		public static readonly BindableProperty AutoplayProperty = BindableProperty.Create("Autoplay", typeof(bool), typeof(VideoPlayer), false);
		public bool Autoplay { get { return (bool)GetValue (AutoplayProperty); } set { SetValue (AutoplayProperty, value); } }

		public static readonly BindableProperty PlayerSizeProperty = BindableProperty.Create("PlayerSize", typeof(Size), typeof(VideoPlayer), new Size(-1.0, -1.0));
		public Size PlayerSize { get { return (Size)GetValue (PlayerSizeProperty); } set { SetValue (PlayerSizeProperty, value); } }

		public static readonly BindableProperty UrlProperty = BindableProperty.Create("Url", typeof(string), typeof(VideoPlayer), null);
		public string Url { 
			get { return (string)GetValue (UrlProperty); } 
			set { Pause (); SetValue (UrlProperty, value); } }

		public VideoPlayer ()
		{
		}

		public void Hide()
		{
			Pause ();
		}

		public void Complete()
		{
			if(CompleteEvent != null)
				CompleteEvent(null, EventArgs.Empty);
		}

		public void Play()
		{
			Playing = true;
			if (PlayEvent != null)
				PlayEvent (null, EventArgs.Empty);
		}

		public void Pause()
		{
			Playing = false;
			if (PauseEvent != null)
				PauseEvent (null, EventArgs.Empty);
		}

		public void OnClick()
		{
			if(ClickEvent != null)
				ClickEvent (null, EventArgs.Empty);
			
		}

		// Sizing step 1: After Xamarin lets us know the size of the Xamarin Element, let the renderer know.
		protected override void OnSizeAllocated (double width, double height)
		{
			PlayerSize = new Size(width, height);
		}
	}
}


