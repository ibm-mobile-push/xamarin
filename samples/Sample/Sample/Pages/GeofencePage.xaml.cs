/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using IBMMobilePush.Forms;
using System.Collections.Generic;

namespace Sample
{
	public partial class GeofencePage : ContentPage
	{
		ICurrentLocation CurrentLocation;
		bool follow;

        void UpdateStatus()
        {
			if (SDK.Instance.GeofenceEnabled())
			{
				if (SDK.Instance.LocationInitialized())
				{
					Status.Text = "ENABLED";
					Status.TextColor = Color.Green;

				}
				else
				{
					Status.Text = "DELAYED (Touch to enable)";
					Status.TextColor = Color.Gray;
				}
			}
			else
			{
				Status.Text = "DISABLED";
				Status.TextColor = Color.Red;
			}
		}    

		public GeofencePage()
		{
			InitializeComponent();

            SDK.Instance.LocationAuthorizationChanged += () => {
                UpdateStatus();
            };

            Status.Clicked += (sender, e) => {
                SDK.Instance.ManualLocationInitialization();
            };
            UpdateStatus();

			follow = true;
			var syncItem = new ToolbarItem();
			syncItem.Text = "Sync";
			syncItem.Command = new Command(() =>
			{
				SDK.Instance.SyncGeofences();
			});
			ToolbarItems.Add(syncItem);

			var followItem = new ToolbarItem();
			followItem.Text = "Follow";
			followItem.Command = new Command(() =>
			{
				follow = !follow;
                if(follow)
                {
                    followItem.Text = "Follow";
                } else
                {
                    followItem.Text = "Don't Follow";
                }
			});
			ToolbarItems.Add(followItem);

			CustomMap.Circles = new HashSet<CustomCircle>();

			CurrentLocation = DependencyService.Get<ICurrentLocation>();
			CurrentLocation.LocationUpdated += (latitude, longitude) =>
			{
				CustomMap.Circles.Clear();

				var geofences = SDK.Instance.GeofencesNear(latitude, longitude);
				foreach (Geofence geofence in geofences)
				{
					var position = new Position(geofence.Latitude, geofence.Longitude);
					CustomMap.Circles.Add(new CustomCircle
					{
						Position = position,
						Radius = geofence.Radius
					});
				}
				if(CustomMap.Refresh != null)
					CustomMap.Refresh();
				if (follow)
				{
					CustomMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromMiles(1.0)));
				}
			};
				
		}
	}
}
