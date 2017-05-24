/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using IBMMobilePush.Forms;

namespace Sample
{
	public partial class BeaconPage : ContentPage
	{
		Dictionary<int, string> RegionDetails = new Dictionary<int, string>();

		public BeaconPage()
		{
			InitializeComponent();

			if (SDK.Instance.BeaconEnabled())
			{
				Status.Detail = "ENABLED";
				Status.DetailColor = Color.Green;
			}
			else
			{
				Status.Detail = "DISABLED";
				Status.DetailColor = Color.Red;
			}

			var uuid = SDK.Instance.BeaconUUID();
			if (uuid == null)
			{
				UUID.Detail = "No UUID Found";
			}
			else
			{
				UUID.Detail = SDK.Instance.BeaconUUID().Value.ToString();
			}
			UpdateRegions();

			SDK.Instance.BeaconEntered += (beaconRegion) => {
				RegionDetails[beaconRegion.Major.Value] = "Entered Minor " + beaconRegion.Minor.Value.ToString();
				UpdateRegions();
			};

			SDK.Instance.BeaconExited += (beaconRegion) =>
			{
				RegionDetails[beaconRegion.Major.Value] = "Exited Minor " + beaconRegion.Minor.Value.ToString();
				UpdateRegions();
			};
		}		

		void UpdateRegions()
		{
			Regions.Clear();
			foreach (BeaconRegion region in SDK.Instance.BeaconRegions())
			{
				var cell = new TextCell() { Text = region.Major.ToString() };
				if (RegionDetails.ContainsKey(region.Major.Value))
					cell.Detail = RegionDetails[region.Major.Value];
				Regions.Add(cell);
			}
		}
	}
}
