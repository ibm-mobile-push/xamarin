/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using Xamarin.Forms.Maps;
using System.Collections.Generic;

namespace Sample
{
	public delegate void RefreshDelegate();
	public class CustomMap : Map
	{
		public ISet<CustomCircle> Circles { get; set; }
		public RefreshDelegate Refresh { get; set; }
	}
}

