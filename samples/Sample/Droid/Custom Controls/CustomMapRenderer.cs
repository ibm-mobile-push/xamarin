/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Sample;
using Sample.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer (typeof(CustomMap), typeof(CustomMapRenderer))]
namespace Sample.Droid
{
	public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
	{
		GoogleMap map;
		CustomMap mapElement;
		CurrentLocation currentLocation;
		List<Circle> Circles;

		void Refresh()
		{
			foreach (var circle in Circles)
				circle.Remove();
			
			foreach (var circle in mapElement.Circles)
			{
				var circleOptions = new CircleOptions();
				circleOptions.InvokeCenter(new LatLng(circle.Position.Latitude, circle.Position.Longitude));
				circleOptions.InvokeRadius(circle.Radius);
				circleOptions.InvokeFillColor(0X66FF0000);
				circleOptions.InvokeStrokeColor(0X66FF0000);
				circleOptions.InvokeStrokeWidth(0);
				Circles.Add(map.AddCircle(circleOptions));
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null) {
				mapElement.Refresh -= Refresh;
			}

			if (e.NewElement != null) {
				if(Circles == null)
					Circles = new List<Circle>();
				mapElement = (CustomMap)e.NewElement;
				mapElement.Refresh += Refresh;

				((MapView)Control).GetMapAsync (this);
			}
		}

		public void OnMapReady (GoogleMap googleMap)
		{
			map = googleMap;
			map.MyLocationEnabled = true;
			Refresh();
		}
	}
}
