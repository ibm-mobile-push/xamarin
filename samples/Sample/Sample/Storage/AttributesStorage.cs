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
	public class AttributesStorage : Storage
	{
		public AttributesStorage ()
		{
		}

		private string ActionKey = "AttributesAction";
		public string Action { 
			get {
				return GetValue<string> (ActionKey, "Set");
			}
			set {
				SetValue<string> (ActionKey, value);
			}
		}

		private string NameKey = "AttributesName";
		public string Name { 
			get {
				return GetValue<string> (NameKey, "name");
			}
			set {
				SetValue<string> (NameKey, value);
			}
		}

		private string ValueKey = "AttributesValue";
		public string Value { 
			get {
				return GetValue<string> (ValueKey, "value");
			}
			set {
				SetValue<string> (ValueKey, value);
			}
		}

	}
}

