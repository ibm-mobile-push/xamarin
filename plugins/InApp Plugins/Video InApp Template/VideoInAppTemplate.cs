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
using FormsVideoLibrary;

namespace Sample
{
    public class VideoInAppTemplate : MediaInAppTemplate
    {
		VideoPlayer ContentVideo;
        TapGestureRecognizer TapGestureRecognizer = new TapGestureRecognizer();
        
		public VideoInAppTemplate () : base()
        {
            // Android
            TapGestureRecognizer.Tapped += Tapped;
        }

		void ContentVideo_UpdateStatus(object sender, EventArgs e)
		{
			if(ContentVideo.Status == VideoStatus.Paused) {
				ContentVideo.UpdateStatus -= ContentVideo_UpdateStatus;
				Dismiss(null, EventArgs.Empty);
			}
		}

        void Tapped(object sender, EventArgs e)
        {
            SDK.Instance.ExecuteInAppAction(Message.Content["action"], Message.MailingId, Message.Attribution);
            SDK.Instance.DeleteInAppMessage(Message);
            ContentVideo.Pause();
        }

        public async override Task<bool> Show() {
			ContentVideo.GestureRecognizers.Add(TapGestureRecognizer);
			ContentVideo.UpdateStatus += ContentVideo_UpdateStatus;

            // iOS 11
            ContentVideo.ClickEvent += Tapped;
			return await base.Show();
        }
        
        public async override Task<bool> Hide()
        {
            ContentVideo.Stop();
			ContentVideo.GestureRecognizers.Remove(TapGestureRecognizer);

            // iOS 11
            ContentVideo.ClickEvent -= Tapped;
            return await base.Hide();
        }

        public async override Task<bool> Wait()
        {
            return false;
        }

        public override void Configure () 
        {
            if (Message.Content ["video"] != null) {
				ContentVideo = new VideoPlayer() { 
					AutoPlay = true,
					Source = new UriVideoSource() { Uri = Message.Content["video"].ToString() },
					AreTransportControlsEnabled = false
				};
				ContentVideo.GestureRecognizers.Add(TapGestureRecognizer);
            }
			base.Configure();
        }

        protected override View Content { get { 
				return ContentVideo; 
			} }
    }
}