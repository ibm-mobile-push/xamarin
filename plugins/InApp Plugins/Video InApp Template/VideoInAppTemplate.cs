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
	public class VideoInAppTemplate : MediaInAppTemplate
	{
		public VideoInAppTemplate ()
		{
		}

		public async override Task<bool> Wait()
		{
			return false;
		}

		public override void Configure () 
		{
			base.Configure();
			if (Message.Content ["video"] != null) {
				ContentVideo.Url = Message.Content ["video"].ToString ();
				ContentVideo.Play ();
			}
		}

		protected override View Content { get { return ContentVideo; } }

		VideoPlayer _ContentVideo;
		VideoPlayer ContentVideo {
			get {
				if (_ContentVideo == null) {
					_ContentVideo = new VideoPlayer () {
						Autoplay = true
					};

					_ContentVideo.CompleteEvent += (object sender, EventArgs e) => {
						Dismiss(sender, e);
					};

					_ContentVideo.ClickEvent += (object sender, EventArgs e) => {
						SDK.Instance.ExecuteInAppAction (Message.Content ["action"], Message.Attribution);
						SDK.Instance.DeleteInAppMessage (Message);
						Dismiss (null, EventArgs.Empty);
					};
				}
				return _ContentVideo;
			}
		}
	}
}