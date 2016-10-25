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

using FFImageLoading.Forms.Droid;
using Newtonsoft.Json.Linq;

using Org.Json;

namespace Sample.Droid
{
	[Activity(Label = "Sample.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			CachedImageRenderer.Init();
			MceApplication.Init(Application, null);
			Xamarin.Insights.Initialize(global::Sample.Droid.XamarinInsights.ApiKey, this);
			base.OnCreate(bundle);
			global::Xamarin.Forms.Forms.Init(this, bundle);

			var jsonActionString = this.Intent.GetStringExtra("jsonAction");
			if (jsonActionString != null)
			{
				var jsonAction = JObject.Parse(jsonActionString);
				JObject jsonPayload = null;
				if (this.Intent.HasExtra("jsonPayload"))
				{
					jsonPayload = JObject.Parse(this.Intent.GetStringExtra("jsonPayload"));
				}
				var attribution = this.Intent.GetStringExtra("attribution");
				var id = this.Intent.GetIntExtra("id", 0);

				LoadApplication(new App(jsonAction, jsonPayload, attribution, id));
			}
			else
			{
				LoadApplication(new App());
			}

		}
	}
}
