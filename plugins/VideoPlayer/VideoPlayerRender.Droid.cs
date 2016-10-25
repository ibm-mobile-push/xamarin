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
using Android;
using Android.Media;
using Android.Views;
using Android.Graphics;

[assembly:ExportRenderer(typeof(Sample.VideoPlayer), typeof(Sample.Droid.VideoPlayerRender))]
namespace Sample.Droid
{
	public class VideoPlayerRender : ViewRenderer<VideoPlayer, Android.Views.View>
	{
		Android.Widget.VideoView videoView;
		Android.Widget.ImageView playPause;
		public VideoPlayerRender ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<VideoPlayer> e)
		{
			base.OnElementChanged (e);

			if (Control == null) {
				string name = Android.Content.Context.LayoutInflaterService;
				var inflater = Xamarin.Forms.Forms.Context.GetSystemService(name) as LayoutInflater;
				var view = inflater.Inflate (Resource.Layout.VideoViewRenderer, null);
				videoView = view.FindViewById (Resource.Id.videoView) as Android.Widget.VideoView;
				var buttonView = view.FindViewById(Resource.Id.buttonView) as Android.Widget.Button;
				buttonView.SetBackgroundColor(Android.Graphics.Color.Transparent);
				buttonView.Click += (object sender, EventArgs ev) => { 
					Element.OnClick();
				};
				playPause = view.FindViewById (Resource.Id.playPause) as Android.Widget.ImageView;
				SetNativeControl (view);
			}

			videoView.SetVideoURI (null);

			if (e.OldElement != null) {
				// Unsubscribe
				e.OldElement.PlayEvent -= Play;
				e.OldElement.PauseEvent -= Pause;
			}

			if (e.NewElement != null) {
				// Subscribe
				e.NewElement.PlayEvent += Play;
				e.NewElement.PauseEvent += Pause;
			}

			LoadVideo ();
		}

		public void Play(object sender, EventArgs e)
		{
			if(Element != null)
				Element.Playing = true;
			if(videoView != null)
				videoView.Start ();
			if(playPause != null)
				playPause.Alpha = 0;
		}

		void Pause(object sender, EventArgs args)
		{
			if (Element != null)
				Element.Playing = false;
			if (videoView != null)
				videoView.Pause();
			if (playPause != null)
				playPause.Alpha = 1;
		}

		void LoadVideo()
		{
			if (Element.Url != null) {
				videoView.SetVideoURI (Android.Net.Uri.Parse (Element.Url));

				videoView.RequestFocus();
				videoView.Completion += (object sender, EventArgs e) => {
					Element.Complete();
				};
				videoView.Prepared += (object sender, EventArgs ev) => {
					var m = sender as MediaPlayer;
					m.VideoSizeChanged += (object s, MediaPlayer.VideoSizeChangedEventArgs e) => {
						var metrics = Xamarin.Forms.Forms.Context.Resources.DisplayMetrics;
						var scale = ((float) videoView.Width / (float) e.Width) / metrics.Density;
						Element.ContentSize = new Size(e.Width * scale, e.Height * scale);
					};
						
					if (Element.Autoplay == true) 
					{
						m.SetVolume(1f, 1f);
						Play(null, EventArgs.Empty);
					}
					else
					{
						playPause.Alpha = 1;
						m.SetVolume(0f, 0f);
						videoView.Start ();
						videoView.Pause ();
						m.SetVolume(1f, 1f);
					}
				};
			}		
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (Element != null && Control != null) {
				if (e.PropertyName == VideoPlayer.UrlProperty.PropertyName) {
					Pause (null, EventArgs.Empty);
					LoadVideo ();
				}
			}
		}

	}
}

