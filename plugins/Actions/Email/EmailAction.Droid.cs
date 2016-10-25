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
using Android.Content;
using IBMMobilePush.Forms;

[assembly: Dependency(typeof(Sample.Droid.EmailActionImpl))]
namespace Sample.Droid
{
	public class EmailActionImpl : IEmailAction
	{
		Context ApplicationContext;
		public EmailActionImpl ()
		{
			ApplicationContext = Xamarin.Forms.Forms.Context;
		}

		public void SendEmail(string subject, string body, string recipient)
		{
			var address = recipient + "?subject=" + Uri.EscapeDataString (subject) + "&body=" + Uri.EscapeDataString (body);
			var intent = new Intent (Intent.ActionSendto, Android.Net.Uri.FromParts("mailto", address, null));
			intent.SetType ("text/plain");
			intent.PutExtra (Intent.ExtraEmail, recipient);
			intent.SetData (Android.Net.Uri.Parse ("mailto:" + address));
			intent.PutExtra (Intent.ExtraSubject, subject);
			intent.PutExtra (Intent.ExtraText, body);
			intent.AddFlags (ActivityFlags.NewTask);
			//intent.AddFlags(Intent.FLAG_FROM_BACKGROUND);
			try {
				ApplicationContext.StartActivity (intent);
			} catch (Android.Content.ActivityNotFoundException e) {
				Logging.Error ("No Email activity found:" + e.Message, e);
			}
		}
	}
}