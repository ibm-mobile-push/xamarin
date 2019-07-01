using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Sample
{
    public class CustomAction : IBMMobilePush.Forms.PushAction
    {
        Action<JObject> callback;
        public CustomAction( Action<JObject> callback )
        {
            this.callback = callback;
        }

        public override void HandleAction(JObject action, JObject payload, string attribution, string mailingId, int id)
        {
            callback(action);
        }
    }


    public partial class CustomActionPage : ContentPage
    {
        public CustomActionPage()
        {
            InitializeComponent();

            IBMMobilePush.Forms.SDK.Instance.ActionNotRegistered += (actionType) =>
            {
                Status.Text = "Unregistered Custom Action Received: " + actionType;
                Status.TextColor = Color.DarkRed;
            };

            IBMMobilePush.Forms.SDK.Instance.ActionNotYetRegistered += (actionType) =>
            {
                Status.Text = "Previously Registered Custom Action Received: " + actionType;
                Status.TextColor = Color.DarkKhaki;
            };

            RegisterButton.Clicked += (sender, e) => {
                Status.Text = "Registered Action Type " + TypeEntry.Text;
                Status.TextColor = Color.DarkGreen;

                IBMMobilePush.Forms.SDK.Instance.RegisterAction(TypeEntry.Text, new CustomAction( (action) => {
                    Status.Text = "Recevied push for action type " + action["type"];
                    Status.TextColor = Color.DarkGreen;
                }));
            };

            UnregisterButton.Clicked += (sender, e) =>
            {
                IBMMobilePush.Forms.SDK.Instance.UnregisterAction(TypeEntry.Text);
                Status.Text = "Unregistered Action Type " + TypeEntry.Text;
                Status.TextColor = Color.DarkGreen;
            };

        }
    }
}
