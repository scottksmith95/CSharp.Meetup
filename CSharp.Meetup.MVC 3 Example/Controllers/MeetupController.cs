using System.Web.Mvc;
using CSharp.Meetup.Api.Interfaces;
using CSharp.Meetup.Connect;
using Spring.Json;
using Spring.Social.OAuth1;

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

		readonly IOAuth1ServiceProvider<IMeetup> _meetupProvider = new MeetupServiceProvider(MeetupApiKey, MeetupApiSecret);

		public ActionResult Index()
		{
			var token = Session["AccessToken"] as OAuthToken;
			if (token != null)
			{
				var meetupClient = _meetupProvider.GetApi(token.Value, token.Secret);
				var result = meetupClient.RestOperations.GetForObjectAsync<string>("https://api.meetup.com/2/members?member_id=" + MemberId).Result;

				ViewBag.TokenValue = token.Value;
				ViewBag.TokenSecret = token.Secret;
				ViewBag.ResultText = result;

				return View();
			}

			var requestToken = _meetupProvider.OAuthOperations.FetchRequestTokenAsync(CallbackUrl, null).Result;

			Session["RequestToken"] = requestToken;

			return Redirect(_meetupProvider.OAuthOperations.BuildAuthenticateUrl(requestToken.Value, null));
		}

		public ActionResult Callback(string oauth_verifier)
		{
			var requestToken = Session["RequestToken"] as OAuthToken;
			var authorizedRequestToken = new AuthorizedRequestToken(requestToken, oauth_verifier);
			var token = _meetupProvider.OAuthOperations.ExchangeForAccessTokenAsync(authorizedRequestToken, null).Result;

			Session["AccessToken"] = token;

			return RedirectToAction("Index");
		}
    }
}
