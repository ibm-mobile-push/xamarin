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
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;

namespace Sample
{
	public class ImageInAppTemplate : MediaInAppTemplate
	{
		int Duration = 5000;

		public ImageInAppTemplate ()
		{
		}

		public async override Task<bool> Wait()
		{
			if (Duration > 0) {
				await Task.Delay (Duration);
				return true;
			}
			return false;
		}

		public override void Configure () 
		{
			base.Configure();
			if(Message.Content["image"] != null)
				ContentImage.Source = ImageSource.FromUri (new Uri (Message.Content ["image"].ToString ()));

			if (Message.Content ["duration"] != null)
				Duration = (int)(1000 * float.Parse (Message.Content ["duration"].ToString (), CultureInfo.InvariantCulture));
			else
				Duration = 5000;
		}

		protected override View Content { get { return ContentImage; } }

		Image _ContentImage;
		Image ContentImage {
			get {
				if (_ContentImage == null) {
					_ContentImage = new Image ();
					var tapGesture = new TapGestureRecognizer ();
					tapGesture.Tapped += (object sender, EventArgs e) => {
						SDK.Instance.ExecuteInAppAction(Message.Content["action"], Message.Attribution);
						SDK.Instance.DeleteInAppMessage(Message);
						Dismiss(null, EventArgs.Empty);
					};
					_ContentImage.GestureRecognizers.Add (tapGesture);
				}
				return _ContentImage;
			}
		}
	}
}