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

namespace Sample
{
	public enum RightStatus {Normal, Sending, Received, Queued, Failed};
	public class RightStatusCell : RightDetailCell
	{
		RightStatus status;
		public RightStatus Status { 
			get { 
				return status; 
			} 
			set {
				status = value;
				if (status == RightStatus.Received) {
					Device.StartTimer (new TimeSpan (0, 0, 5), () => {
						Device.BeginInvokeOnMainThread(() => {
							status = RightStatus.Normal;
							Detail = "";
						});
						return false;
					});
				}
				if (status == RightStatus.Normal) {
					Detail = "";
				}
				else
				{
					Detail = status.ToString ();
				}
			}
		}

		public RightStatusCell ()
		{
			Status = RightStatus.Normal;
		}
	}
}


