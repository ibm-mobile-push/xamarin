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
using IBMMobilePush.Forms;
using Newtonsoft.Json.Linq;

namespace Sample
{
	public interface ISnoozeAction
	{
		void Snooze(JObject payload, int minutes, int id, string attribution);
	}

	public class SnoozeAction : PushAction
	{
		ISnoozeAction snooze;
		public SnoozeAction ()
		{
			snooze = DependencyService.Get<ISnoozeAction> ();
		}

		public override void HandleAction (JObject action, JObject payload, string attribution, int id)
		{
			var minutes = int.Parse (action ["value"].ToString ());
			snooze.Snooze (payload, minutes, id, attribution);
		}
	}
}

