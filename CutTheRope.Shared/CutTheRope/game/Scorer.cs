using System.Collections.Generic;
using CutTheRope.ctr_original;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class Scorer
{
	public static void postLeaderboardResultforLaderboardIdlowestValFirstforGameCenter(int boxScore, int level, bool islowestValFirstforGameCenter)
	{
		/*
		if (!CTRPreferences.isLiteVersion() && !App.NeedsUpdate && !App.UpdateHandled)
		{
			GamePage.MainPage.PostLeaderboard(level, boxScore);
		}
		*/
	}

	public static void postAchievementName(NSString name)
	{
		/*
		if (!Preferences._getBooleanForKey(name.ToString()) && !CTRPreferences.isLiteVersion() && !App.NeedsUpdate && !App.UpdateHandled)
		{
			GamePage.MainPage.AwardAchievement(name.ToString());
		}
		*/
	}

	public static void activateScorerUIAtProfile()
	{
	}

	public static void facebookConnectAndPostOnWall(NSString _msg, NSString _description, NSString _pictureLink, NSString _redirectLink, Dictionary<string, string> _actionLinksStr, NSString _appName)
	{
		/*
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["message"] = _msg.ToString();
		dictionary["description"] = _description.ToString();
		dictionary["picture"] = _pictureLink.ToString();
		dictionary["link"] = _redirectLink.ToString();
		dictionary["name"] = _appName.ToString();
		GamePage.MainPage.PostFacebookMessage(dictionary);
		*/
	}
}
