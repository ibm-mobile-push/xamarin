using System;
using System.Collections.Generic;
using Xamarin.Forms;
using IBMMobilePush.Forms;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace Sample
{
	public partial class PostInboxTemplateView : ContentView
	{
		static Dictionary<string, double> VideoHeightCache = new Dictionary<string, double>();
		static Dictionary<string, Size> ImageHeightCache = new Dictionary<string, Size>();
		InboxMessage InboxMessage;
		RichContent RichContent;

		public PostInboxTemplateView (InboxMessage message, RichContent content, ViewCell viewCell)
		{
			InboxMessage = message;
			RichContent = content;
			InitializeComponent ();

			var headerImage = RichContent.Content ["headerImage"];
			var header = RichContent.Content ["header"];
			var subHeader = RichContent.Content ["subHeader"];
			var contentText = RichContent.Content ["contentText"];
			var actions = RichContent.Content ["actions"];
			var contentVideo = RichContent.Content ["contentVideo"];
			var contentImage = RichContent.Content ["contentImage"];

			if (headerImage != null) {
				var headerImageUri = new Uri (headerImage.ToString ());
				HeaderImage.Source = ImageSource.FromUri (headerImageUri);
			}
					
			if (header != null) {
				Header.Text = header.ToString ();
			}

			if (subHeader != null) {
				SubHeader.Text = subHeader.ToString ();
			}

			if (contentVideo != null) {
				ContentImage.HeightRequest = 1;
				ContentImage.IsVisible = false;

				if (VideoHeightCache.ContainsKey (InboxMessage.InboxMessageId))
					ContentVideo.HeightRequest = VideoHeightCache [InboxMessage.InboxMessageId];
				else
					ContentVideo.HeightRequest = 200;
				
				ContentVideo.Url = contentVideo.ToString ();
				ContentVideo.ContentSizeChange += (object sender, EventArgs e) => {
					if(viewCell != null)
					{
						if (!VideoHeightCache.ContainsKey (InboxMessage.InboxMessageId)) {
							VideoHeightCache [InboxMessage.InboxMessageId] = ContentVideo.ContentSize.Height;
							SizeCell ();
							RefreshCell ();
						}
					}
				};

				// Android
				var tap = new TapGestureRecognizer ();
				tap.Tapped += (object sender, EventArgs e) => {
					if (ContentVideo.Playing)
						ContentVideo.Pause ();
					else
						ContentVideo.Play ();
				};
				ContentVideo.GestureRecognizers.Add (tap);

				// iOS
				ContentVideo.ClickEvent += (object sender, EventArgs e) => {
					if (ContentVideo.Playing)
						ContentVideo.Pause ();
					else
						ContentVideo.Play ();
				};


			} else if (contentImage != null) {
				ContentVideo.HeightRequest = 1;
				ContentVideo.IsVisible = false;

				ContentImage.Success += (object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e) => 
				{
					if(viewCell != null)
					{
						if (!ImageHeightCache.ContainsKey (InboxMessage.InboxMessageId)) {
							ImageHeightCache [InboxMessage.InboxMessageId] = new Size(e.ImageInformation.OriginalWidth, e.ImageInformation.OriginalHeight);
							SizeCell ();
							RefreshCell ();
						}
					}
				};

				var contentImageUri = new Uri (contentImage.ToString ());
				var contentImageSource = ImageSource.FromUri (contentImageUri);
				ContentImage.Source = contentImageSource;
				if (ImageHeightCache.ContainsKey (InboxMessage.InboxMessageId)) {
					var size = SDK.Instance.ScreenSize ();
					var cache = ImageHeightCache [InboxMessage.InboxMessageId];
					var scaledHeight = cache.Height * size.Width / cache.Width;
					ContentImage.HeightRequest = scaledHeight;
					VerticalLayout.ForceLayout ();
				}
					
			} else {
				ContentImage.HeightRequest = 1;
				ContentVideo.HeightRequest = 1;
				ContentImage.IsVisible = false;
				ContentVideo.IsVisible = false;
			}

			if (contentText == null) 
			{
				ContentText.HeightRequest = 1;
				ContentText.IsVisible = false;
			}
			else
			{
				if (viewCell == null)
					ContentText.HeightRequest = -1;
				else
					ContentText.HeightRequest = 44;
				ContentText.Text = contentText.ToString ();
			}

			if (actions == null) {
				Actions.HeightRequest = 1;
				Actions.IsVisible = false;
			}
			else
			{
				var attributes = new Dictionary<string, string>() {
							{"richContentId", content.RichContentId},
							{"inboxMessageId", message.InboxMessageId}
						};

				Actions.HeightRequest = 44;
				if (actions [0] != null) {
					CenterAction.Text = actions [0]["name"].ToString();
					CenterAction.Clicked += (object sender, EventArgs e) => {
						SDK.Instance.ExecuteInboxAction(actions[0], message.Attribution, attributes);
					};
				}
				if (actions [1] != null) {
					LeftAction.Text = actions [1]["name"].ToString();
					LeftAction.Clicked += (object sender, EventArgs e) => {
						SDK.Instance.ExecuteInboxAction(actions[1], message.Attribution, attributes);
					};
				}
				if (actions [2] != null) {
					RightAction.Text = actions [2]["name"].ToString();
					RightAction.Clicked += (object sender, EventArgs e) => {
						SDK.Instance.ExecuteInboxAction(actions[2], message.Attribution, attributes);
					};
				}
			}

			if (viewCell != null)
				viewCell.Height = SizeCell();
		}

		double SizeCell()
		{
			var height = HeaderLayout.HeightRequest + VerticalLayout.Spacing + VerticalLayout.Spacing;

			if (RichContent.Content ["contentVideo"] != null) {
				height += VerticalLayout.Spacing;
				if (VideoHeightCache.ContainsKey (InboxMessage.InboxMessageId))
					height += VideoHeightCache [InboxMessage.InboxMessageId];
				else
					height += 200;
			}
			else if (RichContent.Content ["contentImage"] != null) {
				height += VerticalLayout.Spacing;
				if (ImageHeightCache.ContainsKey (InboxMessage.InboxMessageId)) {
					var size = SDK.Instance.ScreenSize ();
					var cache = ImageHeightCache [InboxMessage.InboxMessageId];
					var scaledHeight = cache.Height * size.Width / cache.Width;
					height += scaledHeight;
				}
				else
					height += 200;
			}

			if (ContentText.HeightRequest > 0) {
				height += ContentText.HeightRequest;
				height += VerticalLayout.Spacing;
			}

			if (Actions.HeightRequest > 0) {
				height += Actions.HeightRequest;
				height += VerticalLayout.Spacing;
			}

			return height;
		}

		void RefreshCell()
		{
			var viewCell = Parent as ViewCell;
			if (viewCell == null)
				return;
			var listView = viewCell.Parent as ListView;
			if (listView != null) {
				var source = listView.ItemsSource as ObservableCollection<InboxMessage>;
				var index = source.IndexOf (InboxMessage);
				if (index >= 0) {
					Device.BeginInvokeOnMainThread (() => {
						source [index] = InboxMessage;
					});
				}
			}
		}

		public void Disappearing()
		{
			if (ContentVideo != null)
				ContentVideo.Hide ();
		}
	}
}

