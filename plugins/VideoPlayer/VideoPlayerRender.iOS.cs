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
using UIKit;
using AVFoundation;
using Foundation;
using CoreFoundation;
using CoreMedia;
using CoreAnimation;

[assembly:ExportRenderer(typeof(Sample.VideoPlayer), typeof(Sample.iOS.VideoPlayerRender))]
namespace Sample.iOS
{
	public class VideoPlayerRender : ViewRenderer<VideoPlayer, UIView>
	{
		AVPlayerLayer PlayerLayer;
		AVPlayer Player;
		AVPlayerItem PlayerItem;
		UITapGestureRecognizer recongnizer;
		CALayer Cover;
		CALayer PlayButton;

		public VideoPlayerRender ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<VideoPlayer> e)
		{
			base.OnElementChanged (e);

			if (Control == null) {
				UIView view = new UIView ();
				SetNativeControl (view);
				recongnizer = new UITapGestureRecognizer ( () => {
					Element.OnClick();
				});

				Player = new AVPlayer ();
				PlayerLayer = AVPlayerLayer.FromPlayer (Player);
				view.Layer.AddSublayer (PlayerLayer);

				Cover = new CALayer ();
				Cover.Bounds = view.Bounds;
				Cover.BackgroundColor = UIColor.Black.ColorWithAlpha ((nfloat)0.3).CGColor;
				view.Layer.AddSublayer (Cover);

				PlayButton = new CALayer ();
				PlayButton.Contents = UIImage.FromBundle ("play").CGImage;
				PlayButton.Frame = new CoreGraphics.CGRect (0, 0, 44, 44);
				view.Layer.AddSublayer (PlayButton);
			}

			Player.Pause ();

			if (e.OldElement != null) {
				// Unsubscribe
				e.OldElement.PlayEvent -= Play;
				e.OldElement.PauseEvent -= Pause;
				Control.RemoveGestureRecognizer (recongnizer);

				Player.RemoveObserver (this, "status");
				Player.RemoveObserver (this, "currentItem.presentationSize");
			}

			if (e.NewElement != null) {
				// Subscribe
				e.NewElement.PlayEvent += Play;
				e.NewElement.PauseEvent += Pause;
				Control.AddGestureRecognizer (recongnizer);

				Player.AddObserver (this, "status", NSKeyValueObservingOptions.OldNew, IntPtr.Zero);
				Player.AddObserver (this, "currentItem.presentationSize", NSKeyValueObservingOptions.OldNew, IntPtr.Zero);
			}

			Element.SizeChanged += (object sender, EventArgs sizeElement) => {
				UpdateFrame();
			};

			LoadVideo ();
		}

		public void Play(object sender, EventArgs e)
		{
			Element.Playing = true;
			Cover.RemoveFromSuperLayer();
			PlayButton.RemoveFromSuperLayer();
			Player.Play();
		}

		public void Pause(object sender, EventArgs e)
		{
			Element.Playing = false;
			Control.Layer.AddSublayer(Cover);
			Control.Layer.AddSublayer(PlayButton);
			Player.Pause();
		}

		// Sizing step 3: Once the video loads some of the stream and we know the size of the Xamarin Element, we can calculate the actual content size
		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr ctx)
		{
			if (keyPath.Equals (new NSString ("status"))) {
				if (Player.Status == AVPlayerStatus.ReadyToPlay && Element.Autoplay) {
					Play (null, EventArgs.Empty);
				}
			}
			else if(keyPath.Equals(new NSString("currentItem.presentationSize")))
			{
				var size = Player.CurrentItem.PresentationSize;
				if (size.Width <= 0 || size.Height <= 0)
					return;
				
				var bounds = PlayerLayer.Bounds;
				var scale = bounds.Width / size.Width;
				Element.ContentSize = new Size(size.Width * scale, size.Height * scale);
			}
		}

		void LoadVideo()
		{
			if (Element.Url == null)
				return;

			var url = new NSUrl (Element.Url);
			PlayerItem = new AVPlayerItem (url);
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("AVPlayerItemDidPlayToEndTimeNotification"), (note) => {
				if(Element != null)
					Element.Complete();
			});

			Player.ReplaceCurrentItemWithPlayerItem (PlayerItem);
		}

		// Sizing step 2: Once we have the size of the element, resize the video sublayer
		void UpdateFrame()
		{
			var bounds = PlayerLayer.Bounds;
			bounds.Width = (nfloat) Element.PlayerSize.Width;
			bounds.Height = (nfloat) Element.PlayerSize.Height;
			PlayerLayer.Frame = bounds;
			Cover.Frame = bounds;
			PlayButton.Frame = new CoreGraphics.CGRect (bounds.Width / 2 - 22, bounds.Height/2 - 22, 44, 44);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (Element != null && Control != null) {
				if (e.PropertyName == VideoPlayer.UrlProperty.PropertyName) {
					Pause (null, EventArgs.Empty);
					LoadVideo ();

				} else if (e.PropertyName == VideoPlayer.PlayerSizeProperty.PropertyName) {
					UpdateFrame ();
				}

			}
		}
	}
}

