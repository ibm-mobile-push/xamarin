using Xamarin.Forms;
using Android.Content;


[assembly: Dependency(typeof(Sample.Droid.CalendarActionImpl))]
namespace Sample.Droid
{
	public class OpenAppActionImpl : IOpenAppAction
	{
		public OpenAppActionImpl()
		{
		}

		public async void OpenApp()
		{
			Context context = Android.App.Application.Context;
			Intent intent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
			Forms.Context.StartActivity(intent);
		}
	}
}

