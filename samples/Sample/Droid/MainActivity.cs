/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using IBMMobilePush.Droid.API;
using IBMMobilePush.Forms;

using FFImageLoading.Forms.Droid;
using Newtonsoft.Json.Linq;

using Org.Json;
using Android.Support.V4.Content;
using Android.Locations;
using Android.Support.V4.App;
using IBMMobilePush.Droid.Location;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Droid
{
	[Activity(Label = "Sample.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity, ActivityCompat.IOnRequestPermissionsResultCallback
	{
		public static MainActivity Current { private set; get; }

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode == 0)
			{
				// Check if the only required permission has been granted
				if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
				{
					Logging.Info("Location permission was granted.");
					SDK.Instance.ManualLocationInitialization();
				}
				else
				{
					Logging.Info("Location permission was NOT granted.");
				}
			}
			else
			{
				base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			}
		}


		public static readonly int PickImageId = 1000;

        public TaskCompletionSource<string> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    // Set the filename as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(data.DataString);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }

		protected override void OnCreate(Bundle bundle)
		{
			Current = this;

			CachedImageRenderer.Init(true);
			base.OnCreate(bundle);
			global::Xamarin.Forms.Forms.Init(this, bundle);

			var jsonActionString = this.Intent.GetStringExtra("action");
			if (jsonActionString != null)
			{
				var jsonAction = JObject.Parse(jsonActionString);
				JObject jsonPayload = null;
				if (this.Intent.HasExtra("payload"))
				{
					jsonPayload = JObject.Parse(this.Intent.GetStringExtra("payload"));
				}
				var attribution = this.Intent.GetStringExtra("attribution");
				int id=0;
				try
				{
					id = Int32.Parse(this.Intent.GetStringExtra("id"));
				}
				catch { }

				var mailingId = this.Intent.GetStringExtra("mailingId");
				LoadApplication(new App(jsonAction, jsonPayload, attribution, mailingId, id));
			}
			else
			{
				LoadApplication(new App());
			}

		}
	}
}
