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
		public int Action { 
			get {
				return GetValue<int> (ActionKey, 0);
			}
			set {
				SetValue<int> (ActionKey, value);
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

        private string TypeKey = "AttributesType";
        public int Type
        {
            get
            {
                return GetValue<int>(TypeKey, 0);
            }
            set
            {
                SetValue<int>(TypeKey, value);
            }
        }

        private String DateKey = "AttributesDate";
        public DateTime DateTime
        {
            get
            {
                return GetValue<DateTime>(DateKey, DateTime.Now);
            }
            set
            {
                SetValue<DateTime>(DateKey, value);
            }
        }

        private String BoolKey = "AttributesBoolValue";
        public int BoolValue
        {
            get
            {
                return GetValue<int>(BoolKey, 0);
            }
            set
            {
                SetValue<int>(BoolKey, value);
            }
        }


        private String NumberKey = "AttributesNumberValue";
        public Double NumberValue
        {
            get
            {
                return GetValue<Double>(NumberKey, 0);
            }
            set
            {
                SetValue<Double>(NumberKey, value);
            }
        }

    }
}

