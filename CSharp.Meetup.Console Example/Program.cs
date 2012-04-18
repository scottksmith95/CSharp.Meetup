using System;
using System.Diagnostics;
using CSharp.Meetup.Api;
using CSharp.Meetup.Connect;
using Spring.Json;
using Spring.Social.OAuth1;

namespace CSharp.Meetup.Console_Example
{
	class Program
	{
		// Register your own Meetup app at http://www.meetup.com/meetup_api/docs/

		// Set your consumer key & secret here
		private const string MeetupApiKey = "ENTER YOUR KEY HERE";
		private const string MeetupApiSecret = "ENTER YOUR SECRET HERE";

		// Set your member id for API call
		private const string MemberId = "42116682";

		private static void Main(string[] args)
		{
			try
			{
				var meetupServiceProvider = new MeetupServiceProvider(MeetupApiKey, MeetupApiSecret);

				/* OAuth 'dance' */

				// Authentication using Out-of-band/PIN Code Authentication
				Console.Write("Getting request token...");
				var oauthToken = meetupServiceProvider.OAuthOperations.FetchRequestTokenAsync("oob", null).Result;
				Console.WriteLine("Done");

				var authenticateUrl = meetupServiceProvider.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, null);
				Console.WriteLine("Redirect user for authentication: " + authenticateUrl);
				Process.Start(authenticateUrl);
				Console.WriteLine("Enter PIN Code from Meetup authorization page:");
				var pinCode = Console.ReadLine();

				Console.Write("Getting access token...");
				var requestToken = new AuthorizedRequestToken(oauthToken, pinCode);
				var oauthAccessToken = meetupServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
				Console.WriteLine("Done");

				/* API */

				var meetup = meetupServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);

				meetup.RestOperations.GetForObjectAsync<string>("https://api.meetup.com/2/members?member_id=" + MemberId)
					.ContinueWith(task => Console.WriteLine("Result: " + task.Result));
			}
			catch (AggregateException ae)
			{
				ae.Handle(ex =>
				{
				    if (ex is MeetupApiException)
				    {
				        Console.WriteLine(ex.Message);
				        return true;
				    }
				    return false;
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				Console.WriteLine("--- hit <return> to quit ---");
				Console.ReadLine();
			}
		}
	}
}