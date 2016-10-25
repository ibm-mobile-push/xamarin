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
	public abstract class MediaInAppTemplate : InAppTemplate
	{
		const int TEXT_HEIGHT = 80;

		public async override Task<bool> Show()
		{
			InAppView.TranslationY = InAppView.Height;
			await InAppView.TranslateTo (0, 0, 250, Easing.CubicInOut);
			return true;
		}

		public async override Task<bool> Hide ()
		{
			await InAppView.TranslateTo (0, InAppView.Height, 250, Easing.CubicInOut);
			RemoveInAppLayout ();
			return true;
		}

		public override void Configure ()
		{
			if (Message.Content ["title"] != null)
				TitleLabel.Text = Message.Content ["title"].ToString ();
			else
				TitleLabel.Text = "";

			if (Message.Content ["text"] != null) 
				TextLabel.Text = Message.Content ["text"].ToString ();
			else
				TextLabel.Text = "";
		}

		RelativeLayout _MediaLayout;
		RelativeLayout MediaLayout {
			get { 
				if (_MediaLayout == null) {
					_MediaLayout = new RelativeLayout () {
						BackgroundColor = Color.Black.MultiplyAlpha (0.8),
						HeightRequest = Layout.Height,
						WidthRequest = Layout.Width
					};
				}
				return _MediaLayout;
			}
		}

		Image _DismissImage;
		Image DismissImage { 
			get {
				if (_DismissImage == null) {
					_DismissImage = new Image () {
						Source = ImageSource.FromFile ("dismiss.png")
					};

					var tapGesture = new TapGestureRecognizer ();
					tapGesture.Tapped += Dismiss;
					_DismissImage.GestureRecognizers.Add (tapGesture);
				}
				return _DismissImage;
			}
		}

		Image _HandleImage;
		Image HandleImage {
			get { 
				if (_HandleImage == null) {
					_HandleImage = new Image () {
						Source = ImageSource.FromFile ("handle.png")
					};
				}
				return _HandleImage;
			}
		}

		Label _TitleLabel;
		Label TitleLabel {
			get {
				if (_TitleLabel == null) {
					_TitleLabel = new Label (){ 
						FontAttributes = FontAttributes.Bold,
						TextColor = Color.White,
						LineBreakMode = LineBreakMode.TailTruncation
					};
				}
				return _TitleLabel;
			}
		}

		Label _TextLabel;
		Label TextLabel {
			get {
				if (_TextLabel == null) {
					_TextLabel = new Label (){ 
						VerticalOptions = LayoutOptions.StartAndExpand,
						TextColor = Color.White
					};
				}
				return _TextLabel;
			}
		}

		StackLayout _TextStack;
		StackLayout TextStack {
			get {
				if (_TextStack == null) {

					_TextStack = new StackLayout() {
						WidthRequest = Layout.Width-10,
						VerticalOptions = LayoutOptions.StartAndExpand,
						Children = {
							TitleLabel,
							new BoxView () {
								HeightRequest=1,
								WidthRequest=Layout.Width-10,
								Color=Color.White
							},
							TextLabel
						}
					};

					var expanded = false;
					var textGesture = new TapGestureRecognizer ();
					textGesture.Tapped += (object sender, EventArgs e) => {
						canceled = true;
						expanded = !expanded;
						MediaLayout.Children.Remove(_TextStack);

						if(expanded)
						{
							_TextStack.BackgroundColor = Color.Black.MultiplyAlpha(0.7);
							MediaLayout.Children.Add (_TextStack, 
								xConstraint: Constraint.Constant (5), 
								yConstraint: Constraint.Constant (30), 
								widthConstraint: Constraint.Constant (Layout.Width - 10), 
								heightConstraint: Constraint.Constant (Layout.Height - 30)
							);
						}
						else
						{
							_TextStack.BackgroundColor = Color.Transparent;
							MediaLayout.Children.Add (_TextStack, 
								xConstraint: Constraint.Constant (5), 
								yConstraint: Constraint.Constant (Layout.Height - TEXT_HEIGHT), 
								widthConstraint: Constraint.Constant (Layout.Width - 10), 
								heightConstraint: Constraint.Constant (Layout.Height - 30) 
							);
						}
					};
					_TextStack.GestureRecognizers.Add (textGesture);

				}
				return _TextStack;
			}
		}

		protected abstract View Content { get; }

		View _InAppView;
		public override View InAppView { 
			get {
				if (_InAppView == null) {
					MediaLayout.Children.Add (DismissImage, 
						xConstraint: Constraint.Constant (Layout.Width - 30 - 5), 
						yConstraint: Constraint.Constant (5), 
						widthConstraint: Constraint.Constant (30), 
						heightConstraint: Constraint.Constant (30)
					);

					MediaLayout.Children.Add (HandleImage, 
						xConstraint: Constraint.Constant (Layout.Width / 2 - 60 / 2), 
						yConstraint: Constraint.Constant (13), 
						widthConstraint: Constraint.Constant (60), 
						heightConstraint: Constraint.Constant (14)
					);

					MediaLayout.Children.Add (Content, 
						xConstraint: Constraint.Constant (0), 
						yConstraint: Constraint.Constant (40), 
						widthConstraint: Constraint.Constant (Layout.Width), 
						heightConstraint: Constraint.Constant (Layout.Height - 40 - TEXT_HEIGHT)
					);

					MediaLayout.Children.Add (TextStack, 
						xConstraint: Constraint.Constant (5), 
						yConstraint: Constraint.Constant (Layout.Height - TEXT_HEIGHT), 
						widthConstraint: Constraint.Constant (Layout.Width - 10), 
						heightConstraint: Constraint.Constant (Layout.Height - 30)
					);

					_InAppView = MediaLayout;
				}
				return _InAppView;
			}
		}
		public override Constraint XConstraint { get { return Constraint.Constant (0); } }
		public override Constraint YConstraint { get { return Constraint.Constant (0); } }
		public override Constraint WidthConstraint { get { return Constraint.Constant (Layout.Width ); } }
		public override Constraint HeightConstraint { get { return Constraint.Constant (Layout.Height ); } }
	}
}