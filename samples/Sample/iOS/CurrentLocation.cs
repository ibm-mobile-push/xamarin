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
using IBMMobilePush.Forms.iOS;
using CoreLocation;

[assembly: Dependency(typeof(Sample.iOS.CurrentLocation))]
namespace Sample.iOS
{
	public class CurrentLocation : ICurrentLocation
	{
		CLLocationManager LocationManager;
		public CurrentLocation()
		{
			LocationManager = new CLLocationManager();
			LocationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {
				LocationUpdated(e.Locations[0].Coordinate.Latitude, e.Locations[0].Coordinate.Longitude);
			};
			LocationManager.StartUpdatingLocation();
		}

		public UpdatedDelegate LocationUpdated { get; set; }
		
	}
}
