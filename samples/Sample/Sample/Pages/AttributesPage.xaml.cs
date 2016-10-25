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

using Newtonsoft.Json.Linq;

using IBMMobilePush.Forms;

namespace Sample
{
	public partial class AttributesPage : ContentPage
	{
		AttributesStorage storage = new AttributesStorage();
		public AttributesPage ()
		{
			InitializeComponent ();

			Name.Text = storage.Name;
			Name.Completed += (object sender, EventArgs e) => {
				storage.Name = Name.Text;
			};
			Value.Text = storage.Value;
			Value.Completed += (object sender, EventArgs e) => {
				storage.Value = Value.Text;
			};
			Action.SelectedValue = storage.Action;
			Action.Change += (object sender, EventArgs e) => {
				storage.Action = Action.SelectedValue;
			};

			SendQueue.Tapped += (object sender, EventArgs e) => {
				SendQueue.Status = RightStatus.Sending;

				switch(Action.SelectedValue)
				{
				case "Set":
					SDK.Instance.QueueSetUserAttribute(Name.Text, Value.Text);
					break;
				case "Update":
					SDK.Instance.QueueUpdateUserAttribute(Name.Text, Value.Text);
					break;
				case "Delete":
					SDK.Instance.QueueDeleteUserAttribute(Name.Text);
					break;
				}
			};

			SendClient.Tapped += (object sender, EventArgs e) => {
				AttributeResultsDelegate clientCallback = (success, key, value, operation) => {
					if(success)
						SendClient.Status = RightStatus.Received;
					else
						SendClient.Status = RightStatus.Failed;
				};

				SendClient.Status = RightStatus.Sending;
				switch(Action.SelectedValue)
				{
				case "Set":
					SDK.Instance.SetUserAttribute(Name.Text, Value.Text, clientCallback);
					break;
				case "Update":
					SDK.Instance.UpdateUserAttribute(Name.Text, Value.Text, clientCallback);
					break;
				case "Delete":
					SDK.Instance.DeleteUserAttribute(Name.Text, clientCallback);
					break;
				}
			};
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			storage.Name = Name.Text;
			storage.Value = Value.Text;
			storage.Action = Action.SelectedValue;
			SDK.Instance.AttributeQueueResults -= QueueCallback;
		}
		protected override void OnAppearing()
		{
			SDK.Instance.AttributeQueueResults += QueueCallback;
		}

		void QueueCallback(bool success, string key, object value, AttributeOperation operation) {
			if(success)
				SendQueue.Status = RightStatus.Received;
			else
				SendQueue.Status = RightStatus.Failed;
		}

	}
}

