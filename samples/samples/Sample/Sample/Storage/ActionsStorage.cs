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
	public class ActionsStorage : Storage
	{
		public ActionsStorage ()
		{
		}

		private string StandardTypeKey = "StandardType";
		public string StandardType { 
			get {
				return GetValue<string> (StandardTypeKey, "url");
			}
			set {
				SetValue<string> (StandardTypeKey, value);
			}
		}

		private string StandardNameKey = "StandardName";
		public string StandardName { 
			get {
				return GetValue<string> (StandardNameKey, StandardType, StandardNameDefault);
			}
			set {
				SetValue<string> (StandardNameKey, value);
			}
		}

		string StandardNameDefault { 
			get {
				switch (StandardType) {
				case "url":
					return "URL";
				case "dial":
					return "Dial";
				case "openApp":
					return "OpenApp";
				}
				return null;
			}
		}

		private string StandardValueKey = "StandardValue";
		public string StandardValue { 
			get {
				return GetValue<string> (StandardValueKey, StandardType, StandardValueDefault);
			}
			set {
				SetValue<string> (StandardValueKey, value);
			}
		}

		string StandardValueDefault { 
			get {
				switch (StandardType) {
				case "url":
					return "http://ibm.co";
				case "dial":
					return "18884266840";
				case "openApp":
					return null;
				}
				return null;
			}
		}

		private string CustomTypeKey = "CustomType";
		public string CustomType { 
			get {
				return GetValue<string> (CustomTypeKey, "emailAction");
			}
			set {
				SetValue<string> (CustomTypeKey, value);
			}
		}

	
		private string CustomNameKey = "CustomName";
		public string CustomName { 
			get {
				return GetValue<string> (CustomNameKey, "Send Email");
			}
			set {
				SetValue<string> (CustomNameKey, value);
			}
		}

		private string CustomValueKey = "CustomValue";
		public string CustomValue { 
			get {
				return GetValue<string> (CustomValueKey, "{\"subject\":\"Hello from Sample App\", \"body\": \"This is an example email body\", \"recipient\":\"fake-email@fake-site.com\"}");
			}
			set {
				SetValue<string> (CustomValueKey, value);
			}
		}

	}
}

