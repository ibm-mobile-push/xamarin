using Xamarin.Forms;

[assembly: Dependency(typeof(Sample.iOS.CalendarActionImpl))]
namespace Sample.iOS
{
	public class OpenAppActionImpl : IOpenAppAction
	{
		public OpenAppActionImpl()
		{
		}

		public async void OpenApp()
		{
		}
	}
}

