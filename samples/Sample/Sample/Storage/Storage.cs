/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using Xamarin.Forms;
using System;

namespace Sample
{
	public class Storage
	{
		protected T GetValue<T>(string key, T defaultValue)
		{
			if(Application.Current.Properties.ContainsKey(key))
			{
				return (T) Application.Current.Properties [key];
			}
			return defaultValue;
		}

		protected void SetValue<T>(string key, T value)
		{
			Application.Current.Properties [key] = value;
		}

		protected T GetValue<T>(string key, string prependKey, T defaultValue)
		{
			if(Application.Current.Properties.ContainsKey(key))
			{
				return (T) Application.Current.Properties [key];
			}
			return defaultValue;
		}

		protected void SetValue<T>(string key, string prependKey, T value)
		{
			Application.Current.Properties [key] = value;
		}
	}
}

