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

namespace Sample.Droid
{
	[Activity(Label = "Sample.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
        protected override void OnResume()
        {
            base.OnResume();
            SDK.Instance.OnResume();
        }

		protected override void OnCreate(Bundle bundle)
		{
			CachedImageRenderer.Init();
			base.OnCreate(bundle);
			global::Xamarin.Forms.Forms.Init(this, bundle);

			if(ContextCompat.CheckSelfPermission(ApplicationContext, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
			{
				ActivityCompat.RequestPermissions(this, new String[] { Android.Manifest.Permission.AccessFineLocation, Android.Manifest.Permission.AccessCoarseLocation }, 0);
			}
			else {
				IBMMobilePush.Droid.Location.LocationManager.EnableLocationSupport(this);
			}
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
