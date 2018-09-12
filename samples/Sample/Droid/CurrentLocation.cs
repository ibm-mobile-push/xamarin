/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using Android.Locations;
using Sample;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Xamarin.Forms;

[assembly: Dependency(typeof(Sample.Droid.CurrentLocation))]
namespace Sample.Droid
{
	public class CurrentLocation : Java.Lang.Object, ICurrentLocation, ILocationListener
	{
		string locationProvider;
		LocationManager LocationManager;
		string TAG = "CurrentLocation";
		Location lastLocation;

		public CurrentLocation()
		{
			LocationManager = (LocationManager)Xamarin.Forms.Forms.Context.GetSystemService(Context.LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = LocationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				locationProvider = string.Empty;
			}

			LocationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

			Log.Debug(TAG, "Using " + locationProvider + ".");
		}

		UpdatedDelegate myDelegate;
		public UpdatedDelegate LocationUpdated { 
			get 
			{
				return myDelegate;
			} 
			set 
			{
				myDelegate = value;
				if (lastLocation != null)
					myDelegate(lastLocation.Latitude, lastLocation.Longitude);
			} 
		}

		public void OnLocationChanged(Location location)
		{
			lastLocation = location;
			if (LocationUpdated != null)
				LocationUpdated(location.Latitude, location.Longitude);
		}

		public void OnProviderDisabled(string provider)
		{
			Log.Debug(TAG, "OnProviderDisabled");
		}

		public void OnProviderEnabled(string provider)
		{
			Log.Debug(TAG, "OnProviderEnabled");
		}

		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
		{
			Log.Debug(TAG, "OnStatusChanged " + status.ToString());
		}

	}
}
