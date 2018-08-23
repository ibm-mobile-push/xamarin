/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
namespace Sample
{
	public delegate void UpdatedDelegate(double latitude, double longitude);

	public interface ICurrentLocation
	{
		UpdatedDelegate LocationUpdated { get; set; }
	}
}
