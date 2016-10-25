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
using Newtonsoft.Json;

namespace Sample
{
	public partial class ActionsPage : ContentPage
	{
		ActionsStorage storage;
		public ActionsPage ()
		{
			InitializeComponent ();
			storage = new ActionsStorage ();

			StandardType.Detail = storage.StandardType;
			StandardType.Tapped += async (object sender, EventArgs e) => 
			{
				var action = await DisplayActionSheet ("Choose Standard Action Type", "Cancel", null, "url", "dial", "openApp");
				if(action == null || action.Equals("Cancel"))
				{
					return;
				}
				storage.StandardType = action;
				StandardType.Detail = storage.StandardType;
				StandardName.Text = storage.StandardName;
				StandardValue.Text = storage.StandardValue;
				UpdateStandardJSON ();
			};

			StandardName.Text = storage.StandardName;
			StandardName.Completed += (object sender, EventArgs e) => {
				storage.StandardName = StandardName.Text;
				UpdateStandardJSON ();
			};

			StandardValue.Text = storage.StandardValue;
			StandardValue.Completed += (object sender, EventArgs e) => {
				storage.StandardValue = StandardValue.Text;
				UpdateStandardJSON ();
			};

			UpdateStandardJSON ();

			CustomType.Text = storage.CustomType;
			CustomType.Completed += (object sender, EventArgs e) => {
				storage.CustomType = CustomType.Text;
				UpdateCustomJSON ();
			};

			CustomName.Text = storage.CustomName;
			CustomName.Completed += (object sender, EventArgs e) => {
				storage.CustomName = CustomName.Text;
				UpdateCustomJSON ();
			};

			CustomValue.Text = storage.CustomValue;
			CustomValue.Completed += (object sender, EventArgs e) => {
				storage.CustomValue = CustomValue.Text;
				UpdateCustomJSON ();
			};

			UpdateCustomJSON ();
		}

		string ConstructJSON(string type, string name, string value)
		{
			try
			{
				var jsonValue = JsonConvert.DeserializeObject<Object>(value);
				return JsonConvert.SerializeObject(new { type = type, name = name, value = jsonValue });
			}
			catch(Exception ex) {
				if(value == null)
					return JsonConvert.SerializeObject(new { type = type, name = name });
				else
					return JsonConvert.SerializeObject(new { type = type, name = name, value = value });
			}
		}

		void UpdateStandardJSON()
		{
			StandardJSON.Detail = ConstructJSON (storage.StandardType, storage.StandardName, storage.StandardValue);
		}
		void UpdateCustomJSON()
		{
			CustomJSON.Detail = ConstructJSON (storage.CustomType, storage.CustomName, storage.CustomValue);
		}

	}
}

