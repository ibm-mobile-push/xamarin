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
using MessageUI;
using UIKit;
using IBMMobilePush.Forms.iOS;
using IBMMobilePush.Forms;

[assembly: Dependency(typeof(Sample.iOS.EmailActionImpl))]
namespace Sample.iOS
{
	public class EmailActionImpl : IEmailAction
	{
		MFMailComposeViewController MailController;

		public EmailActionImpl ()
		{
		}

		public void SendEmail(string subject, string body, string recipient)
		{
			if (!MFMailComposeViewController.CanSendMail) {
				var alert = new UIAlertView ();  
				alert.Title = "Cannot send mail";
				alert.Message = "Please verify that you have a mail account set up.";
				alert.AddButton ("Okay");
				alert.Show ();
				return;
			}

			MailController = new MFMailComposeViewController ();
			MailController.SetSubject (subject);
			MailController.SetToRecipients (new string[] { recipient });
			MailController.SetMessageBody(body,false);
			MailController.Finished += (object sender, MFComposeResultEventArgs e) => {
				switch(e.Result)
				{
				case MFMailComposeResult.Cancelled:
					Logging.Info("Mail send was canceled");
					break;
				case MFMailComposeResult.Saved:
					Logging.Info("Mail was saved as draft");
					break;
				case MFMailComposeResult.Sent:
					Logging.Info("Mail was sent");
					break;
				case MFMailComposeResult.Failed:
					Logging.Info("Mail send failed");
					break;

					
				}
				MailController.DismissViewController(true, null);
			};

			Utility.FindCurrentViewController ().PresentViewController (MailController, true, null);
		}


	}
}