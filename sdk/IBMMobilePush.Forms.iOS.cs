﻿/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */

using System;
using System.IO;
using System.Collections.Generic;

using Xamarin.Forms;

using IBMMobilePush.Forms;
using IBMMobilePush.iOS;

using UIKit;
using Foundation;
using ObjCRuntime;

using Newtonsoft.Json.Linq;

using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinIOS;

[assembly: Dependency(typeof(IBMMobilePush.Forms.iOS.IBMMobilePushImpl))]
namespace IBMMobilePush.Forms.iOS
{
	public class IBMMobilePushImpl : IIBMMobilePush
	{
		public IBMMobilePushImpl()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("RegistrationChangedNotification"), RegistrationUpdatedNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("RegisteredNotification"), RegistrationUpdatedNotification);
		
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("SetUserAttributesSuccess"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.SetUserAttributes, true, note);
			});
		
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UpdateUserAttributesSuccess"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.UpdateUserAttributes, true, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("DeleteUserAttributesSuccess"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.DeleteUserAttributes, true, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("SetChannelAttributesSuccess"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.SetChannelAttributes, true, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UpdateChannelAttributesSuccess"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.UpdateChannelAttributes, true, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("DeleteChannelAttributesSuccess"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.DeleteChannelAttributes, true, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("SetUserAttributesError"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.SetUserAttributes, false, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UpdateUserAttributesError"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.UpdateUserAttributes, false, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("DeleteUserAttributesError"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.DeleteUserAttributes, false, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("SetChannelAttributesError"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.SetChannelAttributes, false, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UpdateChannelAttributesError"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.UpdateChannelAttributes, false, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("DeleteChannelAttributesError"), (note) => {
				AttributeQueueResultNotification(AttributeOperation.DeleteChannelAttributes, false, note);
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("MCEEventFailure"), (note) => {
				EventQueueResultNotification(false, note);
			});
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("MCEEventSuccess"),(note) => {
				EventQueueResultNotification(true, note);
			});


			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("MCESyncDatabase"),(note) => {
				if(InboxMessagesUpdate != null)
				{
					InboxMessagesUpdate();
				}
			});

		}

		public RegistrationUpdatedDelegate RegistrationUpdated { set; get; }
		public AttributeResultsDelegate AttributeQueueResults { set; get; }
		public EventResultsDelegate EventQueueResults { set; get; }
		public InboxResultsDelegate InboxMessagesUpdate { set; get; }

		public void AttributeQueueResultNotification (AttributeOperation operation, bool success, NSNotification note)
		{
			if(AttributeQueueResults != null)
			{
				NSDictionary attributesDict = (NSDictionary) note.UserInfo["attributes"];
				NSArray keys = (NSArray) note.UserInfo["keys"];
				if(keys != null)
				{
					for(nuint i=0;i<keys.Count;i++)
					{
						NSString key = keys.GetItem<NSString> (i);
						AttributeQueueResults(success, key, null, operation);
					}

				}
				else if(attributesDict != null)
				{
					foreach(NSString key in attributesDict.Keys)
					{
						NSObject val = attributesDict[key];
						AttributeQueueResults(success, key, val, operation);
					}
				}

			}
		}

		public void RegistrationUpdatedNotification(NSNotification note)
		{
			if(RegistrationUpdated != null)
			{
				RegistrationUpdated();
			}
		}

		public String Version()
		{
			return MCESdk.SharedInstance().SdkVersion;
		}

		public String UserId()
		{
			return MCERegistrationDetails.UserId;
		}

		public String ChannelId()
		{
			return MCERegistrationDetails.ChannelId;
		}

		public String AppKey()
		{
			return MCESdk.SharedInstance().Config.AppKey;
		}


		MCEAttributesClient _AttributeClient;
		MCEAttributesClient AttributeClient { 
			get {
				if (_AttributeClient == null) {
					_AttributeClient = new MCEAttributesClient ();
				}
				return _AttributeClient;
			} 
		}

		public void SetUserAttribute<T> (String key, T value, AttributeResultsDelegate callback)
		{
			AttributeClient.SetUserAttributes(new NSDictionary(key, value), (error) => {
				callback(error == null, key, value, AttributeOperation.SetUserAttributes);
			});
		}

		public void UpdateUserAttribute<T> (string key, T value, AttributeResultsDelegate callback)
		{
			AttributeClient.UpdateUserAttributes(new NSDictionary(key, value), (error) => {
				callback(error == null, key, value, AttributeOperation.UpdateUserAttributes);
			});
		}

		public void DeleteUserAttribute (string key, AttributeResultsDelegate callback)
		{
			AttributeClient.DeleteUserAttributes(new NSObject[] { (NSString) key }, (error) => {
				callback(error == null, key, null, AttributeOperation.DeleteUserAttributes);
			});
		}

		public void SetChannelAttribute<T> (string key, T value, AttributeResultsDelegate callback)
		{
			AttributeClient.SetChannelAttributes(new NSDictionary(key, value), (error) => {
				callback(error == null, key, value, AttributeOperation.SetChannelAttributes);
			});
		}

		public void UpdateChannelAttribute<T> (string key, T value, AttributeResultsDelegate callback)
		{
			AttributeClient.UpdateChannelAttributes(new NSDictionary(key, value), (error) => {
				callback(error == null, key, value, AttributeOperation.UpdateChannelAttributes);
			});
		}

		public void DeleteChannelAttribute (string key, AttributeResultsDelegate callback)
		{
			AttributeClient.DeleteChannelAttributes(new NSObject[] { (NSString) key }, (error) => {
				callback(error == null, key, null, AttributeOperation.DeleteChannelAttributes);
			});
		}

		public void QueueSetUserAttribute<T> (string key, T value)
		{
			MCEAttributesQueueManager.SharedInstance ().SetUserAttributes (new NSDictionary (key, value));
		}

		public void QueueUpdateUserAttribute<T> (string key, T value)
		{
			MCEAttributesQueueManager.SharedInstance ().UpdateUserAttributes (new NSDictionary (key, value));
		}

		public void QueueDeleteUserAttribute (string key)
		{
			MCEAttributesQueueManager.SharedInstance ().DeleteUserAttributes (new NSObject[] { (NSString) key } );
		}

		public void QueueSetChannelAttribute<T> (string key, T value)
		{	
			MCEAttributesQueueManager.SharedInstance ().SetChannelAttributes (new NSDictionary (key, value));
		}

		public void QueueUpdateChannelAttribute<T> (string key, T value)
		{
			MCEAttributesQueueManager.SharedInstance ().UpdateChannelAttributes (new NSDictionary (key, value));
		}

		public void QueueDeleteChannelAttribute (string key)
		{
			MCEAttributesQueueManager.SharedInstance ().DeleteChannelAttributes (new NSObject[] { (NSString) key } );
		}

		public void QueueAddEvent (string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,object> attributes, bool flush)
		{
			var apiEvent = GenerateEvent (name, type, timestamp, attribution, attributes);
			MCEEventService.SharedInstance ().AddEvent (apiEvent, flush);
		}

		MCEEventClient _EventClient;
		MCEEventClient EventClient { 
			get {
				if (_EventClient == null) {
					_EventClient = new MCEEventClient ();
				}
				return _EventClient;
			} 
		}

		public static DateTimeOffset ConvertDate(NSDate date, DateTimeOffset defaultValue)
		{					
			if(date != null)
			{
				DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( new DateTime(2001, 1, 1, 0, 0, 0) );
				return reference.AddSeconds(date.SecondsSinceReferenceDate);
			}
			return defaultValue;
		}

		public static NSDate ConvertDate(DateTimeOffset timestamp, NSDate defaultValue)
		{
			if (timestamp != default(DateTimeOffset)) {
				DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (2001, 1, 1, 0, 0, 0));
				return NSDate.FromTimeIntervalSinceReferenceDate ((timestamp - reference).TotalSeconds);
			}
			return defaultValue;
		}

		MCEEvent GenerateEvent<T>(string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,T> attributes)
		{
			var apiEvent = new MCEEvent ();
			apiEvent.Name = name;
			apiEvent.Type = type;

			if (timestamp != default(DateTimeOffset)) {
				apiEvent.Timestamp = ConvertDate (timestamp, NSDate.Now);
			}

			if (attribution != null)
				apiEvent.Attribution = attribution;

			if (attributes != null) {
				var mutableAttributes = new NSMutableDictionary ();
				foreach (KeyValuePair<string, T> attribute in attributes) {
					mutableAttributes.Add (NSObject.FromObject (attribute.Key), NSObject.FromObject (attribute.Value));
				}
				apiEvent.Attributes = mutableAttributes;
			}

			return apiEvent;
		}

		public void AddEvent(string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,object> attributes, EventResultsDelegate callback)
		{
			var apiEvent = GenerateEvent (name, type, timestamp, attribution, attributes);
			EventClient.SendEvents (new NSObject[] { apiEvent }, delegate(NSError error) {
				callback(error == null, name, type, timestamp, attribution, attributes);
			});
		}

		public void FlushEventQueue()
		{
			MCEEventService.SharedInstance ().SendEvents ();
		}

		void EventQueueResultNotification(bool success, NSNotification note)
		{
			if (EventQueueResults == null)
				return;
			
			NSArray apiEvents = (NSArray) note.UserInfo["events"];
			if(apiEvents != null)
			{
				for(nuint i=0;i<apiEvents.Count;i++)
				{
					MCEEvent apiEvent = apiEvents.GetItem<MCEEvent> (i);
					string name = apiEvent.Name;
					string type = apiEvent.Type;

					DateTimeOffset timestamp = ConvertDate (apiEvent.Timestamp, default(DateTimeOffset));
					string attribution = apiEvent.Attribution;
					Dictionary<string,object> attributes = new Dictionary<string, object> ();

					NSDictionary attributesDict = apiEvent.Attributes;
					if(attributesDict != null)
					{
						var keys = attributesDict.Keys;
						if(keys != null)
						{
							for(int ii=0;ii<keys.Length;ii++)
							{
								NSString key = (NSString) keys[ii];
								attributes.Add(key, attributesDict.ObjectForKey(key));
							}
						}
					}

					EventQueueResults(success, name, type, timestamp, attribution, attributes);
				}

			}
		}

		public int Badge { 
			set {
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = value;
			}
			get { 
				return (int) UIApplication.SharedApplication.ApplicationIconBadgeNumber;
			}
		}

		// iOS doesn't natively support setting icons.
		public int Icon { set; get; }

		public bool IsProviderRegistered()
		{
			return MCERegistrationDetails.ApsRegistered();
		}

		public bool IsMceRegistered()
		{
			return MCERegistrationDetails.MceRegistered();
		}

		public bool IsRegistered()
		{
			return IsProviderRegistered () && IsMceRegistered ();
		}

		public void RegisterAction (string name, PushAction handler)
		{
			MCEActionRegistry.SharedInstance ().RegisterTarget (new ActionHandler (handler), new Selector("handleAction:payload:"), name);
		}

		public class ActionHandler : NSObject
		{
			PushAction Handler;

			public ActionHandler(PushAction handler)
			{
				Handler = handler;
			}

			[Export ("handleAction:payload:")]
			public void HandleAction(NSDictionary action, NSDictionary payload)
			{
				string attribution = null;
				NSDictionary mce = (NSDictionary) payload["mce"];
				if (mce != null) {
					attribution = (NSString)mce ["attribution"];
				}
				NSError error = null;
				var jsonPayloadData = NSJsonSerialization.Serialize (payload, NSJsonWritingOptions.PrettyPrinted, out error);
				var jsonPayloadString = NSString.FromData (jsonPayloadData, NSStringEncoding.UTF8);
				var jsonPayloadObject = JObject.Parse (jsonPayloadString);

				var jsonActionData = NSJsonSerialization.Serialize (action, NSJsonWritingOptions.PrettyPrinted, out error);
				var jsonActionString = NSString.FromData (jsonActionData, NSStringEncoding.UTF8);
				var jsonActionObject = JObject.Parse (jsonActionString);

				Handler.HandleAction (jsonActionObject, jsonPayloadObject, attribution, 0);
			}
		}

		public void PhoneHome()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			defaults ["MCELastPhoneHome"] = NSDate.DistantPast;
			defaults.Synchronize();
			MCEPhoneHomeManager.PhoneHome ();
		}

		public void SyncInboxMessages()
		{
			MCEInboxQueueManager.SharedInstance ().SyncInbox ();
		}

		public RichContent FetchRichContent (string richContentId)
		{
			var richContent = MCEInboxDatabase.SharedInstance ().FetchRichContentId (richContentId);

			NSError error = null;
			var jsonData = NSJsonSerialization.Serialize (richContent.Content, NSJsonWritingOptions.PrettyPrinted, out error);
			var jsonString = NSString.FromData (jsonData, NSStringEncoding.UTF8);

			return new RichContent () {
				RichContentId = richContent.RichContentId,
				Content = JObject.Parse(jsonString)
			};
		}

		public static InboxMessage Convert(MCEInboxMessage inboxMessage)
		{
			DateTimeOffset expirationDate = ConvertDate(inboxMessage.ExpirationDate, DateTimeOffset.MaxValue);
			DateTimeOffset sendDate = DateTimeOffset.MinValue;
			if(inboxMessage.SendDate != null)
			{
				DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( new DateTime(2001, 1, 1, 0, 0, 0) );
				sendDate = reference.AddSeconds(inboxMessage.SendDate.SecondsSinceReferenceDate);
			}

			return new InboxMessage () {
				InboxMessageId = inboxMessage.InboxMessageId,
				RichContentId = inboxMessage.RichContentId,
				ExpirationDate = expirationDate,
				SendDate = sendDate,
				TemplateName = inboxMessage.Template,
				Attribution=inboxMessage.Attribution,
				IsRead=inboxMessage.IsRead,
				IsDeleted=inboxMessage.IsDeleted
			};
		}

		public void FetchInboxMessage (string inboxMessageId, InboxMessageResultsDelegate callback)
		{
			MCEInboxDatabase.SharedInstance ().FetchInboxMessageId (inboxMessageId, (MCEInboxMessage inboxMessage, NSError error) => {

				if(error != null)
					callback(null);

				callback(Convert(inboxMessage));
			});
		}

		public void ExecuteAction(JToken action, string attribution, string source, Dictionary<string, string> attributes)
		{
			var actionString = action.ToString ();
			var actionData = NSData.FromString (actionString);
			NSError error = null;
			var actionDict = (NSDictionary) NSJsonSerialization.Deserialize(actionData, 0, out error);
			var payload = new NSDictionary ("mce", new NSDictionary ("attribution", attribution));

			var attributesDict = new NSMutableDictionary();
			foreach (KeyValuePair<string, string> entry in attributes)
			{
				attributesDict.Add( new NSString( entry.Key ), new NSString(entry.Value));
			}
			MCEActionRegistry.SharedInstance ().PerformAction (actionDict, payload, source, attributesDict);
		}

		public void DeleteInboxMessage(InboxMessage message)
		{
			MCEInboxDatabase.SharedInstance ().SetDeletedForInboxMessageId (message.InboxMessageId);
		}

		public void ReadInboxMessage (InboxMessage message)
		{
			MCEInboxDatabase.SharedInstance ().SetReadForInboxMessageId (message.InboxMessageId);
		}

		InboxMessageResultsDelegate fetchCallback;
		string richContentIdCallback;
		public void FetchInboxMessageWithRichContentId(string richContentId, InboxMessageResultsDelegate callback)
		{
			MCEInboxDatabase.SharedInstance ().FetchInboxMessageViaRichContentId (richContentId, (MCEInboxMessage inboxMessage, NSError error) => {

				if(error != null && !error.Domain.Equals("Inbox message not in storage"))
					callback(null);

				if (inboxMessage == null)
				{
					richContentIdCallback = richContentId;
					fetchCallback = callback;
					InboxMessagesUpdate += CallbackFetchInboxMessageWithRichContentId;
					SyncInboxMessages();
				}
				else
				{
					callback(Convert(inboxMessage));
				}
			});

		}

		void CallbackFetchInboxMessageWithRichContentId()
		{
			InboxMessagesUpdate -= CallbackFetchInboxMessageWithRichContentId;
			if (richContentIdCallback != null)
			{
				FetchInboxMessageWithRichContentId(richContentIdCallback, fetchCallback);
				richContentIdCallback = null;
				fetchCallback = null;
			}
		}

		public void FetchInboxMessages (Action<InboxMessage[]> completion, bool ascending)
		{
			MCEInboxDatabase.SharedInstance().FetchInboxMessages((messages, error) => {
				var messageList = new List<InboxMessage>();
				for(nuint i=0;i< messages.Count; i++)
				{
					var message = messages.GetItem<MCEInboxMessage>(i);
					messageList.Add(Convert(message));
				}
				completion(messageList.ToArray());
			}, ascending);
		}

		public float StatusBarHeight()
		{
			return (float) (UIApplication.SharedApplication.StatusBarFrame.Height);
		}

		public Size ScreenSize()
		{
			var size = UIScreen.MainScreen.Bounds.Size;
			return new Size (size.Width, size.Height);
		}

		public SQLiteAsyncConnection GetConnection(string filename)
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string libraryPath = Path.Combine (documentsPath, "..", "Library");
			var path = Path.Combine(libraryPath, filename);

			var platform = new SQLitePlatformIOS();
			var param = new SQLiteConnectionString(path, false);
			var connection = new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(platform, param));
			return connection;
		}

		public void ExecuteAction(JObject action, JObject payload, string attribution, int id)
		{
			// only used in Android
		}
	}

	static class Utility
	{
		public static UIViewController FindCurrentViewController()
		{
			return FindCurrentViewController (UIApplication.SharedApplication.KeyWindow.RootViewController);
		}

		static UIViewController FindCurrentViewController(UIViewController viewController)
		{
			UINavigationController navController = viewController as UINavigationController;
			UITabBarController tabController = viewController as UITabBarController;

			if (navController != null) {
				return FindCurrentViewController(navController.VisibleViewController);
			} else if (tabController != null) {
				return FindCurrentViewController (tabController.SelectedViewController);
			} else {
				if (viewController.PresentedViewController != null) {
					return FindCurrentViewController(viewController.PresentedViewController);
				} else {
					return viewController;
				}
			}
		}

		public static void ProcessInApp(NSDictionary userInfo)
		{
			NSError error = null;
			var jsonData = NSJsonSerialization.Serialize (userInfo, NSJsonWritingOptions.PrettyPrinted, out error);
			var jsonString = NSString.FromData (jsonData, NSStringEncoding.UTF8);
			var json = JObject.Parse(jsonString);
			InAppManager.Instance.InsertInAppAsync(json);
		}
	}
}

