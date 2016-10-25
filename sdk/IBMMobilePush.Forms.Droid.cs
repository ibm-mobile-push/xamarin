/*
 * Licensed Materials - Property of IBM
 *
 * 5725E28, 5725I03
 *
 * © Copyright IBM Corp. 2016, 2016
 * US Government Users Restricted Rights - Use, duplication or disclosure restricted by GSA ADP Schedule Contract with IBM Corp.
 */
using Sample.Droid;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IBMMobilePush.Forms;
using IBMMobilePush.Droid.API;
using IBMMobilePush.Droid.API.Attribute;
using IBMMobilePush.Droid.Notification;
using IBMMobilePush.Droid.API.Notification;
using IBMMobilePush.Droid.API.Event;
using IBMMobilePush.Droid.Plugin.Inbox;

using Java.Util;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using Org.Json;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;

using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinAndroid;

[assembly: Dependency(typeof(IBMMobilePush.Forms.Droid.IBMMobilePushImpl))]
namespace IBMMobilePush.Forms.Droid
{
	public class IBMMobilePushImpl : IIBMMobilePush
	{
		delegate void InboxCallbackDelegate();

		Context ApplicationContext;
		IBMMobilePushBroadcastReceiver BroadcastReceiver; 
		public IBMMobilePushImpl()
		{
			ApplicationContext = Xamarin.Forms.Forms.Context;

			IntentFilter intentFilter = new IntentFilter("com.ibm.mce.sdk.NOTIFIER");
			BroadcastReceiver = new IBMMobilePushBroadcastReceiver(this);
			ApplicationContext.RegisterReceiver (BroadcastReceiver, intentFilter);
		}

		~IBMMobilePushImpl()
		{
			ApplicationContext.UnregisterReceiver (BroadcastReceiver);
		}

		public RegistrationUpdatedDelegate RegistrationUpdated { set; get; }
		public AttributeResultsDelegate AttributeQueueResults { set; get; }
		public EventResultsDelegate EventQueueResults { set; get; }
		public InboxResultsDelegate InboxMessagesUpdate { set; get; }

		void RegistrationUpdate()
		{
			if (RegistrationUpdated != null) {
				RegistrationUpdated ();
			}
		}

		public String Version()
		{
			return MceSdk.SdkVerNumber;
		}

		public String UserId()
		{
			var details = MceSdk.RegistrationClient.GetRegistrationDetails (ApplicationContext);
			return details.UserId;
		}

		public String ChannelId()
		{
			var details = MceSdk.RegistrationClient.GetRegistrationDetails (ApplicationContext);
			return details.ChannelId;
		}

		public String AppKey()
		{
			return MceSdk.RegistrationClient.GetAppKey (ApplicationContext);
		}

		IBMMobilePush.Droid.API.Attribute.Attribute AttributeConverter(String key, object value)
		{
			var stringValue = value as String;
			if (stringValue != null) {
				return new StringAttribute (key, stringValue);
			}

			var dateValue = value as Java.Util.Date;
			if (dateValue != null) {
				return new DateAttribute (key, dateValue);
			}

			try
			{
				var intValue = (int)Convert.ChangeType(value, typeof(int));
				return new NumberAttribute(key, new Java.Lang.Integer(intValue));
			}
			catch (InvalidCastException ex)
			{
			}


			try
			{
				var floatValue = (int)Convert.ChangeType(value, typeof(float));
				return new NumberAttribute(key, new Java.Lang.Float(floatValue));
			}
			catch (InvalidCastException ex)
			{
			}


			try
			{
				var doubleValue = (double)Convert.ChangeType(value, typeof(double));
				return new NumberAttribute(key, new Java.Lang.Double(doubleValue));
			}
			catch (InvalidCastException ex)
			{
			}


			try
			{
				var longValue = (long)Convert.ChangeType(value, typeof(long));
				return new NumberAttribute(key, new Java.Lang.Long(longValue));
			}
			catch (InvalidCastException ex)
			{
			}

			try
			{
				var boolValue = (bool)Convert.ChangeType(value, typeof(bool));
				return new BooleanAttribute(key, new Java.Lang.Boolean(boolValue));
			}
			catch (InvalidCastException ex)
			{
			}

			return null;
		}

		class AttributeCallback : Java.Lang.Object, IOperationCallback
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute;
			AttributeResultsDelegate callback;
			AttributeOperation operation;
			String key;

			public AttributeCallback(String key, AttributeOperation operation, AttributeResultsDelegate callback)
			{
				this.key = key;
				this.callback = callback;
				this.operation = operation;
			}

			public AttributeCallback(IBMMobilePush.Droid.API.Attribute.Attribute attribute, AttributeOperation operation, AttributeResultsDelegate callback)
			{
				this.callback = callback;
				this.attribute = attribute;
				this.operation = operation;
			}

			public void OnFailure (Java.Lang.Object p0, OperationResult p1)
			{
				if (key != null)
					callback (false, key, null, operation);
				else
					callback (false, attribute.Key, attribute.Value, operation);
			}

			public void OnSuccess (Java.Lang.Object p0, OperationResult p1)
			{
				if (key != null)
					callback (true, key, null, operation);
				else
					callback (true, attribute.Key, attribute.Value, operation);
			}
		}

		public void SetUserAttribute<T> (String key, T value, AttributeResultsDelegate callback)
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributeCallback = new AttributeCallback (attribute, AttributeOperation.SetUserAttributes, callback);
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				MceSdk.GetAttributesClient(false).SetUserAttributes (ApplicationContext, attributes, attributeCallback);
			} else {
				Logging.Error ("Could not set user attribute because could not convert value type.");
			}
		}

		public void UpdateUserAttribute<T> (string key, T value, AttributeResultsDelegate callback)
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributeCallback = new AttributeCallback (attribute, AttributeOperation.SetUserAttributes, callback);
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				MceSdk.GetAttributesClient(false).UpdateUserAttributes (ApplicationContext, attributes, attributeCallback);
			} else {
				Logging.Error ("Could not update user attribute because could not convert value type.");
			}
		}

		public void DeleteUserAttribute (string key, AttributeResultsDelegate callback)
		{
			var attributeCallback = new AttributeCallback (key, AttributeOperation.SetUserAttributes, callback);
			MceSdk.GetAttributesClient(false).DeleteUserAttributes (ApplicationContext, new List<string>(){key}, attributeCallback);
		}

		public void SetChannelAttribute<T> (string key, T value, AttributeResultsDelegate callback)
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributeCallback = new AttributeCallback (attribute, AttributeOperation.SetUserAttributes, callback);
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				MceSdk.GetAttributesClient(false).SetChannelAttributes (ApplicationContext, attributes, attributeCallback);
			} else {
				Logging.Error ("Could not set channel attribute because could not convert value type.");
			}

		}

		public void UpdateChannelAttribute<T> (string key, T value, AttributeResultsDelegate callback)
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				var attributeCallback = new AttributeCallback (attribute, AttributeOperation.SetUserAttributes, callback);
				MceSdk.GetAttributesClient(false).UpdateChannelAttributes (ApplicationContext, attributes, attributeCallback);
			} else {
				Logging.Error ("Could not update channel attribute because could not convert value type.");
			}

		}

		public void DeleteChannelAttribute (string key, AttributeResultsDelegate callback)
		{
			var attributeCallback = new AttributeCallback (key, AttributeOperation.SetUserAttributes, callback);
			MceSdk.GetAttributesClient(false).DeleteChannelAttributes (ApplicationContext, new List<string>(){key}, attributeCallback);
		}

		public void QueueSetUserAttribute<T> (string key, T value)
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				MceSdk.QueuedAttributesClient.SetUserAttributes (ApplicationContext, attributes);
			} else {
				Logging.Error ("Could not set user attribute because could not convert value type.");
			}
		}

		public void QueueUpdateUserAttribute<T> (string key, T value)
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				MceSdk.QueuedAttributesClient.UpdateUserAttributes (ApplicationContext, attributes);
			} else {
				Logging.Error ("Could not update user attribute because could not convert value type.");
			}
		}

		public void QueueDeleteUserAttribute (string key)
		{
			MceSdk.QueuedAttributesClient.DeleteUserAttributes (ApplicationContext, new List<string>(){key});
		}

		public void QueueSetChannelAttribute<T> (string key, T value)
		{	
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				MceSdk.QueuedAttributesClient.SetChannelAttributes (ApplicationContext, attributes);
			} else {
				Logging.Error ("Could not set channel attribute because could not convert value type.");
			}
		}

		public void QueueUpdateChannelAttribute<T> (string key, T value)
		{
			IBMMobilePush.Droid.API.Attribute.Attribute attribute = AttributeConverter (key, value);
			if (attribute != null) {
				var attributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute> () { attribute };
				MceSdk.QueuedAttributesClient.UpdateChannelAttributes (ApplicationContext, attributes);
			} else {
				Logging.Error ("Could not update channel attribute because could not convert value type.");
			}
		}

		public void QueueDeleteChannelAttribute (string key)
		{
			MceSdk.QueuedAttributesClient.DeleteChannelAttributes (ApplicationContext, new List<string>(){key});
		}

		void AttributeOperationResponse(IAttributesOperation operation)
		{
			if(AttributeQueueResults != null)
			{
				var reportOperationInt = operation.Type.Ordinal();
				AttributeOperation reportOperation = (AttributeOperation)reportOperationInt;
				if (operation.AttributeKeys != null) {
					foreach (var key in operation.AttributeKeys) {
						AttributeQueueResults (true, key, null, reportOperation);
					}
				}
				if (operation.Attributes != null) {
					foreach (var attribute in operation.Attributes) {
						AttributeQueueResults (true, attribute.Key, attribute.Value, reportOperation);
					}
				}
			}
		}

		public static Java.Util.Date ConvertDate(DateTimeOffset timestamp, Java.Util.Date defaultValue)
		{
			if (timestamp != default(DateTimeOffset)) {
				DateTime dt = DateTime.Now;
				long epocTicks = 621355968000000000L;
				return new Java.Util.Date ((timestamp.UtcTicks - epocTicks) / TimeSpan.TicksPerMillisecond);
			}
			return defaultValue;
		}

		public static DateTimeOffset ConvertDate(Java.Util.Date timestamp, DateTimeOffset defaultValue)
		{
			if (timestamp != null && timestamp.Time > 0) {
				var referenceDate = (long) new DateTime(1970, 1, 1, 0, 0, 0).Subtract (DateTime.MinValue).TotalMilliseconds;
				return new DateTimeOffset ((referenceDate + timestamp.Time) * TimeSpan.TicksPerMillisecond, TimeSpan.FromSeconds (0));
			}
			return defaultValue;
		}

		Event GenerateEvent (string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,object> attributes)
		{
			var apiAttributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute>();
			foreach(KeyValuePair<string, object> attribute in attributes)
			{
				apiAttributes.Add ( AttributeConverter(attribute.Key, attribute.Value));
			}

			return new Event(type, name, ConvertDate (timestamp, new Java.Util.Date()), apiAttributes, attribution);
		}

		public void AddEvent(string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,object> attributes, EventResultsDelegate callback)
		{
			var sdkEvent = GenerateEvent (name, type, timestamp, attribution, attributes);
			MceSdk.GetEventsClient(false).SendEvent(ApplicationContext, sdkEvent, new EventCallback (name, type, timestamp, attribution, attributes, callback));
		}

		public void FlushEventQueue()
		{
			ThreadPool.QueueUserWorkItem (o => IBMMobilePush.Droid.Events.EventsClientImpl.SendAllEvents(ApplicationContext) );
		}

		public void QueueAddEvent (string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,object> attributes, bool flush)
		{
			var sdkEvent = GenerateEvent (name, type, timestamp, attribution, attributes);
			MceSdk.QueuedEventsClient.SendEvent(ApplicationContext, sdkEvent, flush);
		}

		void EventOperationResponse(IList<Event> events)
		{
			if(EventQueueResults != null)
			{
				foreach (var apiEvent in events) {
					var name = apiEvent.Name;
					var type = apiEvent.Type;
					var timestamp = apiEvent.Timestamp;
					var attribution = apiEvent.Attribution;
					var attributes = new Dictionary<string,object>();
					foreach (var attribute in apiEvent.Attributes) {
						attributes.Add (attribute.Key, attribute.Value);
					}
					EventQueueResults (true, name, type, ConvertDate( timestamp, DateTimeOffset.Now), attribution, attributes);
				}
			}
		}

		// Android doesn't natively support setting badge numbers.
		public int Badge { set; get; }

		public int Icon { 
			set {
				MceSdk.NotificationsClient.NotificationsPreference.SetIcon(ApplicationContext, new Java.Lang.Integer(value));
			} 
			get {
				return MceSdk.NotificationsClient.NotificationsPreference.GetIcon (ApplicationContext);
			}
		}

		public bool IsProviderRegistered()
		{
			return MceSdk.RegistrationClient.GetRegistrationDetails (ApplicationContext).PushToken != null;
		}

		public bool IsMceRegistered()
		{
			return MceSdk.RegistrationClient.GetRegistrationDetails(ApplicationContext).ChannelId != null;
		}

		public bool IsRegistered()
		{
			return IsProviderRegistered () && IsMceRegistered ();
		}

		public void RegisterAction (string name, PushAction handler)
		{
			MceNotificationActionRegistry.RegisterNotificationAction (name, new ActionHandler(handler));
		}

		public void ExecuteAction(JObject action, JObject payload, string attribution, int id)
		{
			var name = action["type"].ToString();
			var handler = MceNotificationActionRegistry.GetNotificationAction(name) as ActionHandler;
			if (handler != null)
			{
				handler.ExecuteAction(action, payload, attribution, id);
			}
		}

		class ActionHandler : Java.Lang.Object, IMceNotificationAction
		{
			PushAction Handler;

			public ActionHandler(PushAction handler)
			{
				this.Handler = handler;
			}

			public void ExecuteAction(JObject action, JObject payload, string attribution, int id)
			{
				Handler.HandleAction(action, payload, attribution, id);
			}

			public void HandleAction (Context context, string type, string name, string attribution, IDictionary<string, string> payload, bool fromNotification)
			{
				string[] skipKeys = {"name", "type", "com.ibm.mce.sdk.NOTIF_SOURCE", "com.ibm.mce.sdk.NOTIF_SOURCE_ID", "com.ibm.mce.sdk.NOTIF_SOURCE_PAYLOAD" };

				var intent = new Intent(context, typeof(MainActivity));

				if (payload.ContainsKey("com.ibm.mce.sdk.NOTIF_SOURCE"))
				{
					var jsonPayload = JObject.Parse(payload["com.ibm.mce.sdk.NOTIF_SOURCE"]);
					intent.PutExtra("jsonPayload", jsonPayload.ToString());
				}

				if (payload.ContainsKey("com.ibm.mce.sdk.NOTIF_SOURCE_ID"))
				{
					var id = int.Parse(payload["com.ibm.mce.sdk.NOTIF_SOURCE_ID"]);
					intent.PutExtra("id", id);
				}

				var jsonAction = new JObject (){ {"type", type}, {"name", name} };
				foreach (KeyValuePair<string, string> item in payload) {
					if(skipKeys.Contains(item.Key))
						continue;
					
					try
					{
						jsonAction.Add (item.Key, JObject.Parse(item.Value));
					}
					catch (Exception ex) {
						jsonAction.Add (item.Key, item.Value);
					}
				}
						
				intent.PutExtra("jsonAction", jsonAction.ToString());
				if (attribution != null)
				{
					intent.PutExtra("attribution", attribution);
				}
				intent.AddFlags(ActivityFlags.NewTask);
				context.StartActivity(intent);
			}

			public void Init (Context context, JSONObject jsonObject)
			{
			}

			public bool ShouldDisplayNotification (Context context, INotificationDetails notificationDetails, Bundle bundle)
			{
				return true;
			}

			public void Update (Context context, JSONObject jsonObject)
			{
			}
		}

		public void PhoneHome()
		{
			ThreadPool.QueueUserWorkItem (o => IBMMobilePush.Droid.Registration.PhoneHomeManager.PhoneHome (ApplicationContext) );
		}

		public void SyncInboxMessages()
		{
			ThreadPool.QueueUserWorkItem (o => InboxMessagesClient.LoadInboxMessages(ApplicationContext, new InboxCallback(InboxMessagesUpdate)) );
		}

		public RichContent FetchRichContent (string richContentId)
		{
			var messageCursor = RichContentDatabaseHelper.GetRichContentDatabaseHelper (ApplicationContext).GetMessagesByContentId (richContentId);
			messageCursor.MoveToFirst();
			var message = messageCursor.RichContent;
			if(message == null)
			{
				Logging.Error("Inbox Message not found in database");
				return null;
			}

			return ConvertToRichContent (message);		
		}
			
		RichContent ConvertToRichContent(IBMMobilePush.Droid.Plugin.Inbox.RichContent message)
		{
			var json = message.Content.ToString ();
			var content = JObject.Parse (json);
			return new RichContent () { 
				RichContentId = message.ContentId,
				Content = content
			};
		}

		InboxMessage ConvertToInboxMessage(IBMMobilePush.Droid.Plugin.Inbox.RichContent message)
		{
			return new InboxMessage () { 
				InboxMessageId = message.MessageId,
				RichContentId = message.ContentId,
				ExpirationDate = ConvertDate(message.ExpirationDate, DateTimeOffset.MaxValue),
				SendDate = ConvertDate(message.SendDate, DateTimeOffset.MinValue),
				TemplateName = message.Template,
				Attribution = message.Attribution,
				IsRead = (bool) message.IsRead,
				IsDeleted = (bool) message.IsDeleted
			};
		}

		public void DeleteInboxMessage(InboxMessage message)
		{
			InboxMessagesClient.DeleteMessageById(ApplicationContext, message.InboxMessageId);
		}

		public void ReadInboxMessage (InboxMessage message)
		{
			InboxMessagesClient.SetMessageReadById(ApplicationContext, message.InboxMessageId);
		}

		public void FetchInboxMessageWithRichContentId(string richContentId, InboxMessageResultsDelegate callback)
		{
			var message = InboxMessagesClient.GetInboxMessageByContentId (ApplicationContext, richContentId);
			if(message == null)
			{
				// sync db then call back.
				ThreadPool.QueueUserWorkItem (o => InboxMessagesClient.LoadInboxMessages(ApplicationContext, new FetchInboxMessageWithRichContentIdCallback(richContentId, callback, this)) );
				return;
			}

			callback( ConvertToInboxMessage (message) );
		}

		class FetchInboxMessageWithRichContentIdCallback : Java.Lang.Object, IOperationCallback
		{
			string RichContentId;
			InboxMessageResultsDelegate Callback;
			IBMMobilePushImpl Push;
			public FetchInboxMessageWithRichContentIdCallback(string richContentId, InboxMessageResultsDelegate callback, IBMMobilePushImpl push)
			{
				Callback = callback;
				RichContentId = richContentId;
				Push = push;
			}

			public void OnFailure(Java.Lang.Object p0, OperationResult p1)
			{
				Logging.Error ("FetchInboxMessageWithRichContentIdCallback Failed.");
			}

			public void OnSuccess(Java.Lang.Object p0, OperationResult p1)
			{
				var message = InboxMessagesClient.GetInboxMessageByContentId (Xamarin.Forms.Forms.Context, RichContentId);
				if (message != null) 
				{
					Callback(Push.ConvertToInboxMessage(message));
				}
			}
		}

		public void FetchInboxMessage (string inboxMessageId, InboxMessageResultsDelegate callback)
		{
			var messageCursor = RichContentDatabaseHelper.GetRichContentDatabaseHelper(ApplicationContext).GetMessagesByMessageId(inboxMessageId);
			messageCursor.MoveToFirst();
			var message = messageCursor.RichContent;
			if(message == null)
			{
				Logging.Error("Inbox Message not found in database");
				callback(null);
			}

			callback( ConvertToInboxMessage (message) );
		}

		public void FetchInboxMessages (Action<InboxMessage[]> completion, bool ascending)
		{
			var messageCursor = RichContentDatabaseHelper.GetRichContentDatabaseHelper (ApplicationContext).Messages;

			var messages = new List<InboxMessage> ();
			while (messageCursor.MoveToNext ()) {
				var message = messageCursor.RichContent;
				messages.Add (ConvertToInboxMessage (message));
			}

			completion (messages.ToArray());
		}

		public void ExecuteAction(JToken action, string attribution, string source, Dictionary<string, string> attributes)
		{
			var actionType = action ["type"].ToString();

			var actionImpl = MceNotificationActionRegistry.GetNotificationAction(actionType);
			if (actionImpl != null) {
				var payload = new Dictionary<string,string> ();

				JObject actionObject = action.Value<JObject>();

				List<string> keys = actionObject.Properties().Select(p => p.Name).ToList();

				foreach (string key in keys) {
					payload [key] = action [key].ToString ();
				}

				actionImpl.HandleAction(ApplicationContext, actionType, null, null, payload, false);

				var eventAttributes = new List<IBMMobilePush.Droid.API.Attribute.Attribute>();
				foreach (KeyValuePair<string, string> entry in attributes)
				{
					eventAttributes.Add(new StringAttribute(entry.Key, entry.Value));
				}

				eventAttributes.Add(new StringAttribute("actionTaken", actionType));
				var name = actionType;
				var clickEventDetails = MceNotificationActionImpl.GetClickEventDetails(actionType);
				if (clickEventDetails != null)
				{
					name = clickEventDetails.EventName;
					var value = action["value"].ToString();
					eventAttributes.Add(new StringAttribute(clickEventDetails.ValueName, value));
				}
				else {
					foreach (KeyValuePair<string, string> entry in payload)
					{
						eventAttributes.Add(new StringAttribute(entry.Key, entry.Value));
					}
				}

				var sdkEvent = new Event(source, name, new Date(), eventAttributes, attribution);

				MceSdk.QueuedEventsClient.SendEvent(ApplicationContext, sdkEvent, true);
			}
		}

		public float StatusBarHeight()
		{
			return 0;
		}

		public SQLiteAsyncConnection GetConnection(string filename)
		{
			string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			var path = Path.Combine(documentsPath, filename);

			var platform = new SQLitePlatformAndroid();
			var param = new SQLiteConnectionString(path, false);
			var connection = new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(platform, param));
			return connection;
		}

		public Xamarin.Forms.Size ScreenSize ()
		{
			var metrics = ApplicationContext.Resources.DisplayMetrics;
			return new Xamarin.Forms.Size (metrics.WidthPixels/metrics.Density, metrics.HeightPixels/metrics.Density);
		}

		class InboxCallback : Java.Lang.Object, IOperationCallback
		{
			InboxResultsDelegate Callback;
			public InboxCallback(InboxResultsDelegate callback)
			{
				Callback = callback;
			}
			public void OnFailure(Java.Lang.Object p0, OperationResult p1)
			{
				Logging.Error ("InboxCallback Failed.");
			}

			public void OnSuccess(Java.Lang.Object p0, OperationResult p1)
			{
				Callback();
			}
		}

		class EventCallback : Java.Lang.Object, IOperationCallback
		{
			string Name;
			string Type;
			DateTimeOffset Timestamp;
			string Attribution;
			Dictionary<string,object> Attributes;
			EventResultsDelegate Callback;

			public EventCallback(string name, string type, DateTimeOffset timestamp, string attribution, Dictionary<string,object> attributes, EventResultsDelegate callback)
			{
				Name = name;
				Type = type;
				Timestamp = timestamp;
				attribution = attribution;
				Attributes = attributes;
				Callback = callback;
			}

			public void OnFailure(Java.Lang.Object p0, OperationResult p1)
			{
				Callback(false, Name, Type, Timestamp, Attribution, Attributes);
			}

			public void OnSuccess(Java.Lang.Object p0, OperationResult p1)
			{
				Callback(true, Name, Type, Timestamp, Attribution, Attributes);
			}

		}

		class IBMMobilePushBroadcastReceiver : MceBroadcastReceiver
		{
			IBMMobilePushImpl IBMMobilePush;
			public IBMMobilePushBroadcastReceiver(IBMMobilePushImpl ibmMobilePush)
			{
				IBMMobilePush = ibmMobilePush;
			}
			public override void OnSdkRegistered (Context p0)
			{
				IBMMobilePush.RegistrationUpdate();
			}
			public override void OnAttributesOperation (Context p0, IAttributesOperation p1)
			{
				IBMMobilePush.AttributeOperationResponse (p1);
			}
			public override void OnC2dmError (Context p0, string p1)
			{
			}
			public override void OnDeliveryChannelRegistered (Context p0)
			{
			}
			public override void OnEventsSend (Context p0, IList<Event> p1)
			{
				IBMMobilePush.EventOperationResponse (p1);
			}
			public override void OnIllegalNotification (Context p0, Intent p1)
			{
			}

			public override void OnMessage (Context context, INotificationDetails notificationDetails, Bundle bundle)
			{
				if (!bundle.ContainsKey("inApp")) {
					return;
				}
				var json = new JObject ();
				json["inApp"] = JObject.Parse(bundle.GetString("inApp"));
				InAppManager.Instance.InsertInAppAsync(json);
			}
			public override void OnNonMceBroadcast (Context p0, Intent p1)
			{
			}
			public override void OnNotificationAction (Context p0, Date p1, string p2, string p3, string p4)
			{
			}
			public override void OnSdkRegistrationChanged (Context p0)
			{
				IBMMobilePush.RegistrationUpdate();
			}
			public override void OnSessionEnd (Context p0, Date p1, long p2)
			{
			}
			public override void OnSessionStart (Context p0, Date p1)
			{
			}
		}
	}
}

