using System.Web.Mvc;
using CSharp.Meetup.Api.Interfaces;
using CSharp.Meetup.Connect;
using Spring.Json;
using Spring.Social.OAuth2;

namespace CSharp.Meetup.MVC_3_Example.Controllers
{
    public class MeetupController : Controller
    {
		// Register your own Meetup app at http://www.meetup.com/meetup_api/docs/

		// Configure the Callback URL
		private const string CallbackUrl = "http://localhost/Meetup/Callback";

		// Set your consumer key & secret here
		private const string MeetupApiKey = "ENTER YOUR KEY HERE";
		private const string MeetupApiSecret = "ENTER YOUR SECRET HERE";

		// Set your member id for API call
		private const string MemberId = "42116682";

        readonly IOAuth2ServiceProvider<IMeetup> _meetupProvider = new MeetupOAuth2ServiceProvider(MeetupApiKey, MeetupApiSecret);

		public ActionResult Index()
		{
			var token = Session["AccessToken"] as AccessGrant;
			if (token != null)
			{
				var meetupClient = _meetupProvider.GetApi(token.AccessToken);
				var result = meetupClient.RestOperations.GetForObjectAsync<string>("https://api.meetup.com/2/members?member_id=" + MemberId).Result;

				ViewBag.TokenValue = token.AccessToken;
				ViewBag.ResultText = result;

				return View();
			}

			return Redirect(_meetupProvider.OAuthOperations.BuildAuthorizeUrl(GrantType.AuthorizationCode, new OAuth2Parameters() { RedirectUrl = CallbackUrl }));
		}

		public ActionResult Callback(string code)
		{
			AccessGrant accessGrant = _meetupProvider.OAuthOperations.ExchangeForAccessAsync(authorizationCode: code, redirectUri: CallbackUrl, additionalParameters: null).Result;

			Session["AccessToken"] = accessGrant;

			return RedirectToAction("Index");
		}
    }
}
