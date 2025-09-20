using System;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.ctr_original;

internal class CTRPreferences : Preferences
{
	public const int TOTAL_DRAWINGS_COUNT = 7;

	public const int VERSION_NUMBER_AT_WHICH_SCORE_HASH_INTRODUCED = 1;

	public const int VERSION_NUMBER = 2;

	public const int MAX_LEVEL_SCORE = 5999;

	public const int MAX_PACK_SCORE = 149999;

	public const int CANDIES_COUNT = 3;

	public const string TWITTER_LINK = "https://mobile.twitter.com/zeptolab";

	public const string FACEBOOK_LINK = "http://www.facebook.com/cuttherope";

	public const string ORIGINAL_LINK = "http://www.windowsphone.com/en-us/store/app/cut-the-rope/4c0cf9f4-31f0-444e-9321-0237d04f6a38";

	public const string EXPERIMENTS_LINK = "http://";

	public const int BOXES_CUT_OUT = 0;

	public const int MAX_PACKS = 12;

	public const int MAX_LEVELS_IN_A_PACK = 25;

	public const string APP_ID = "ctr_exp";

	public const string PREFS_IS_EXIST = "PREFS_EXIST";

	public const string PREFS_SOUND_ON = "SOUND_ON";

	public const string PREFS_MUSIC_ON = "MUSIC_ON";

	public const string PREFS_VOICE_ON = "VOICE_ON";

	public const string PREFS_SCORE_ = "SCORE_";

	public const string PREFS_STARS_ = "STARS_";

	public const string PREFS_UNLOCKED_ = "UNLOCKED_";

	public const string PREFS_DRAWINGS_ = "DRAWINGS_";

	public const string PREFS_NEW_DRAWINGS_COUNTER = "PREFS_NEW_DRAWINGS_COUNTER";

	public const string PREFS_LOCALE = "PREFS_LOCALE";

	public const string PREFS_COPPA_SHOWED = "PREFS_COPPA_SHOWED";

	public const string PREFS_USER_AGE = "PREFS_USER_AGE";

	public const string PREFS_COPPA_RESTRICTED = "PREFS_COPPA_RESTRICTED";

	public const string PREFS_INTRO_MOVIE_ON_STARTUP = "PREFS_INTRO_MOVIE_ON_STARTUP";

	public const string PREFS_ROPES_CUT = "PREFS_ROPES_CUT";

	public const string PREFS_ROPES_SHOOT = "PREFS_ROPES_SHOOT";

	public const string PREFS_WALL_CLIMBER = "PREFS_WALL_CLIMBER";

	public const string PREFS_UNDERWATER = "PREFS_UNDERWATER";

	public const string PREFS_ROCKETS = "PREFS_ROCKETS";

	public const string PREFS_GRAB_SNAILS = "PREFS_GRAB_SNAILS";

	public const string PREFS_CANDIES_WON = "PREFS_CANDIES_WON";

	public const string PREFS_ROPES_JUMPS = "PREFS_ROPES_JUMPS";

	public const string PREFS_BUBBLES_POPPED = "PREFS_BUBBLES_POPPED";

	public const string PREFS_SPIDERS_BUSTED = "PREFS_SPIDERS_BUSTED";

	public const string PREFS_CANDIES_LOST = "PREFS_CANDIES_LOST";

	public const string PREFS_CANDIES_UNITED = "PREFS_CANDIES_UNITED";

	public const string PREFS_SOCKS_USED = "PREFS_SOCKS_USED";

	public const string PREFS_LAST_PACK = "PREFS_LAST_PACK";

	public const string PREFS_GRAB_ROCKET = "PREFS_GRAB_ROCKET";

	public const string PREFS_CANDY_WAS_CHANGED = "PREFS_CANDY_WAS_CHANGED";

	public const string PREFS_SELECTED_CANDY = "PREFS_SELECTED_CANDY";

	public const string PREFS_GAME_CENTER_ENABLED = "PREFS_GAME_CENTER_ENABLED";

	public const string PREFS_SCORE_HASH = "PREFS_SCORE_HASH";

	public const string PREFS_VERSION = "PREFS_VERSION";

	public const string PREFS_GAME_STARTS = "PREFS_GAME_STARTS";

	public const string PREFS_LEVELS_WON = "PREFS_LEVELS_WON";

	public const string PREFS_IAP_UNLOCK = "IAP_UNLOCK";

	public const string PREFS_IAP_BANNERS = "IAP_BANNERS";

	public const string PREFS_IAP_SHAREWARE = "IAP_SHAREWARE";

	public const string acStudent = "acStudent";

	public const string acAssistant = "acAssistant";

	public const string acDoctor = "acDoctor";

	public const string acRopeCollector = "acRopeCollector";

	public const string acRopeExpert = "acRopeExpert";

	public const string acRookieSniper = "acRookieSniper";

	public const string acSkilledSniper = "acSkilledSniper";

	public const string acHighJumper = "acHighJumper";

	public const string acCrazyJumper = "acCrazyJumper";

	public const string acNoviceWallClimber = "acNoviceWallClimber";

	public const string acVeteranWallClimber = "acVeteranWallClimber";

	public const string acTummyDelight = "acTummyDelight";

	public const string acPartyAnimal = "acPartyAnimal";

	public const string acPhotoObserver = "acPhotoObserver";

	public const string acSnailTamer = "acSnailTamer";

	public const string acDeepDiver = "acDeepDiver";

	public const string acRoboMaster = "acRoboMaster";

	public const string lastVersionLaunched = "lastVersionLaunched";

	private bool firstLaunch;

	private bool showPromoBanner;

	private bool playLevelScroll;

	private static bool isTrial;

	private static int[] PACK_UNLOCK_STARS_LITE = new int[9] { 0, 0, 0, 150, 240, 300, 350, 400, 450 };

	private static int[] PACK_UNLOCK_STARS = new int[9] { 0, 0, 80, 150, 240, 300, 350, 400, 450 };

	public static bool IsTrial
	{
		set
		{
			isTrial = value;
		}
	}

	public override NSObject init()
	{
		if (base.init() != null)
		{
			if (!getBooleanForKey("PREFS_EXIST"))
			{
				setBooleanforKey(v: true, "PREFS_EXIST", comit: true);
				setIntforKey(0, "PREFS_GAME_STARTS", comit: true);
				setIntforKey(0, "PREFS_LEVELS_WON", comit: true);
				resetToDefaults();
				resetMusicSound();
				firstLaunch = true;
				showPromoBanner = false;
				playLevelScroll = false;
			}
			else
			{
				int intForKey = getIntForKey("PREFS_VERSION");
				if (intForKey < 1)
				{
					getTotalScore();
					int i = 0;
					for (int packsCount = getPacksCount(); i < packsCount; i++)
					{
						int num = 0;
						int j = 0;
						for (int levelsInPackCount = getLevelsInPackCount(); j < levelsInPackCount; j++)
						{
							int intForKey2 = getIntForKey(getPackLevelKey("SCORE_", i, j));
							if (intForKey2 > 5999)
							{
								num = 150000;
								break;
							}
							num += intForKey2;
						}
						if (num > 149999)
						{
							resetToDefaults();
							resetMusicSound();
							break;
						}
					}
					setScoreHash();
				}
				firstLaunch = false;
				playLevelScroll = false;
			}
			setIntforKey(2, "PREFS_VERSION", comit: true);
		}
		return this;
	}

	private void resetMusicSound()
	{
		setBooleanforKey(v: true, "SOUND_ON", comit: true);
		setBooleanforKey(v: true, "MUSIC_ON", comit: true);
		setBooleanforKey(v: true, "VOICE_ON", comit: true);
	}

	private static bool isShareware()
	{
		return false;
	}

	public static bool isSharewareUnlocked()
	{
		bool flag = isShareware();
		if (flag)
		{
			if (flag)
			{
				return Preferences._getBooleanForKey("IAP_SHAREWARE");
			}
			return false;
		}
		return true;
	}

	public static bool isLiteVersion()
	{
		return isTrial;
	}

	public static bool isBannersMustBeShown()
	{
		return false;
	}

	public static int getStarsForPackLevel(int p, int l)
	{
		return Preferences._getIntForKey(getPackLevelKey("STARS_", p, l));
	}

	public static UNLOCKED_STATE getUnlockedForPackLevel(int p, int l)
	{
		return (UNLOCKED_STATE)Preferences._getIntForKey(getPackLevelKey("UNLOCKED_", p, l));
	}

	public static int getPacksCount()
	{
		return 6;
	}

	public static int getLevelsInPackCount()
	{
		if (!isLiteVersion())
		{
			return 25;
		}
		return 4;
	}

	public static int getTotalStars()
	{
		int num = 0;
		int i = 0;
		for (int packsCount = getPacksCount(); i < packsCount; i++)
		{
			int j = 0;
			for (int levelsInPackCount = getLevelsInPackCount(); j < levelsInPackCount; j++)
			{
				num += getStarsForPackLevel(i, j);
			}
		}
		return num;
	}

	public static int packUnlockStars(int n)
	{
		if (!isLiteVersion())
		{
			return PACK_UNLOCK_STARS[n];
		}
		return PACK_UNLOCK_STARS_LITE[n];
	}

	private static string getPackLevelKey(string prefs, int p, int l)
	{
		return prefs + p + "_" + l;
	}

	public static void setUnlockedForPackLevel(UNLOCKED_STATE s, int p, int l)
	{
		Preferences._setIntforKey((int)s, getPackLevelKey("UNLOCKED_", p, l), comit: true);
	}

	public static int sharewareFreeLevels()
	{
		return 10;
	}

	public static int sharewareFreePacks()
	{
		return 3;
	}

	public static void setLastPack(int p)
	{
		Preferences._setIntforKey(p, "PREFS_LAST_PACK", comit: false);
	}

	public static bool isPackPerfect(int p)
	{
		int i = 0;
		for (int levelsInPackCount = getLevelsInPackCount(); i < levelsInPackCount; i++)
		{
			if (getStarsForPackLevel(p, i) < 3)
			{
				return false;
			}
		}
		return true;
	}

	public static int getLastPack()
	{
		int val = Preferences._getIntForKey("PREFS_LAST_PACK");
		return Math.Min(Math.Max(0, val), getPacksCount() + 1);
	}

	public static void gameViewChanged(NSString NameOfView)
	{
	}

	public static int getScoreForPackLevel(int p, int l)
	{
		return Preferences._getIntForKey("SCORE_" + p + "_" + l);
	}

	public static void setScoreForPackLevel(int s, int p, int l)
	{
		Preferences._setIntforKey(s, "SCORE_" + p + "_" + l, comit: true);
	}

	public static void setStarsForPackLevel(int s, int p, int l)
	{
		Preferences._setIntforKey(s, "STARS_" + p + "_" + l, comit: true);
	}

	public static int getTotalStarsInPack(int p)
	{
		int num = 0;
		int i = 0;
		for (int levelsInPackCount = getLevelsInPackCount(); i < levelsInPackCount; i++)
		{
			num += getStarsForPackLevel(p, i);
		}
		return num;
	}

	public static void disablePlayLevelScroll()
	{
		CTRPreferences cTRPreferences = Application.sharedPreferences();
		cTRPreferences.playLevelScroll = false;
	}

	internal static bool shouldPlayLevelScroll()
	{
		CTRPreferences cTRPreferences = Application.sharedPreferences();
		return cTRPreferences.playLevelScroll;
	}

	internal static bool shouldShowPromo()
	{
		CTRPreferences cTRPreferences = Application.sharedPreferences();
		return cTRPreferences.showPromoBanner;
	}

	internal static void disablePromoBanner()
	{
		CTRPreferences cTRPreferences = Application.sharedPreferences();
		cTRPreferences.showPromoBanner = false;
	}

	public void resetToDefaults()
	{
		Preferences._setBooleanforKey(v: false, "PREFS_INTRO_MOVIE_ON_STARTUP", comit: true);
		int i = 0;
		for (int packsCount = getPacksCount(); i < packsCount; i++)
		{
			int j = 0;
			for (int levelsInPackCount = getLevelsInPackCount(); j < levelsInPackCount; j++)
			{
				int v = (((i < 2 + (isLiteVersion() ? 1 : 0) || (isShareware() && i < sharewareFreePacks())) && j == 0) ? 1 : 0);
				setIntforKey(0, getPackLevelKey("SCORE_", i, j), comit: false);
				setIntforKey(0, getPackLevelKey("STARS_", i, j), comit: false);
				setIntforKey(v, getPackLevelKey("UNLOCKED_", i, j), comit: false);
			}
		}
		for (int k = 0; k <= 7; k++)
		{
			setIntforKey(0, "DRAWINGS_" + k, comit: true);
		}
		setIntforKey(0, "PREFS_ROPES_CUT", comit: true);
		setIntforKey(0, "PREFS_ROPES_SHOOT", comit: true);
		setIntforKey(0, "PREFS_ROPES_JUMPS", comit: true);
		setIntforKey(0, "PREFS_WALL_CLIMBER", comit: true);
		setIntforKey(0, "PREFS_UNDERWATER", comit: true);
		setIntforKey(0, "PREFS_ROCKETS", comit: true);
		setIntforKey(0, "PREFS_GRAB_SNAILS", comit: true);
		setIntforKey(0, "PREFS_CANDIES_WON", comit: true);
		setIntforKey(0, "PREFS_BUBBLES_POPPED", comit: true);
		setIntforKey(0, "PREFS_SPIDERS_BUSTED", comit: true);
		setIntforKey(0, "PREFS_CANDIES_LOST", comit: true);
		setIntforKey(0, "PREFS_CANDIES_UNITED", comit: true);
		setIntforKey(0, "PREFS_SOCKS_USED", comit: true);
		setIntforKey(0, "PREFS_GRAB_ROCKET", comit: true);
		setIntforKey(0, "PREFS_SELECTED_CANDY", comit: true);
		setBooleanforKey(v: false, "PREFS_CANDY_WAS_CHANGED", comit: true);
		setBooleanforKey(v: true, "PREFS_GAME_CENTER_ENABLED", comit: true);
		setIntforKey(0, "PREFS_NEW_DRAWINGS_COUNTER", comit: true);
		setIntforKey(0, "PREFS_LAST_PACK", comit: true);
		setStringforKey("en", "PREFS_LOCALE", comit: true);
		checkForUnlockIAP();
		savePreferences();
		setScoreHash();
	}

	private void checkForUnlockIAP()
	{
		if (!getBooleanForKey("IAP_UNLOCK"))
		{
			return;
		}
		int i = 0;
		for (int packsCount = getPacksCount(); i < packsCount; i++)
		{
			if (getUnlockedForPackLevel(i, 0) == UNLOCKED_STATE.UNLOCKED_STATE_LOCKED)
			{
				setUnlockedForPackLevel(UNLOCKED_STATE.UNLOCKED_STATE_JUST_UNLOCKED, i, 0);
			}
		}
	}

	private int getTotalScore()
	{
		int num = 0;
		for (int i = 0; i < getPacksCount(); i++)
		{
			for (int j = 0; j < getLevelsInPackCount(); j++)
			{
				num += getIntForKey(getPackLevelKey("SCORE_", i, j));
			}
		}
		return num;
	}

	public void setScoreHash()
	{
		NSString input = NSObject.NSS(getTotalScore().ToString());
		NSString mD5Str = MathHelper.getMD5Str(input);
		setStringforKey(mD5Str.ToString(), "PREFS_SCORE_HASH", comit: true);
	}

	public bool isScoreHashValid()
	{
		NSString input = NSObject.NSS(getTotalScore().ToString());
		NSString mD5Str = MathHelper.getMD5Str(input);
		NSString str = NSObject.NSS(getStringForKey("PREFS_SCORE_HASH"));
		return mD5Str.isEqualToString(str);
	}

	internal static bool isFirstLaunch()
	{
		CTRPreferences cTRPreferences = Application.sharedPreferences();
		return cTRPreferences.firstLaunch;
	}

	public void unlockAllLevels(int stars)
	{
		int i = 0;
		for (int packsCount = getPacksCount(); i < packsCount; i++)
		{
			int j = 0;
			for (int levelsInPackCount = getLevelsInPackCount(); j < levelsInPackCount; j++)
			{
				setIntforKey(1, getPackLevelKey("UNLOCKED_", i, j), comit: false);
				setIntforKey(stars, getPackLevelKey("STARS_", i, j), comit: false);
			}
		}
		savePreferences();
	}

	public static void setDrawingUnlocked(int d, int s)
	{
		Preferences._setIntforKey(s, "DRAWINGS_" + d, comit: true);
	}

	public static int getDrawingUnlocked(int d)
	{
		return Preferences._getIntForKey("DRAWINGS_" + d);
	}

	public static int getDrawingUnlockedCount()
	{
		int num = 0;
		int num2 = 7;
		for (int num3 = num2; num3 >= 0; num3--)
		{
			if (getDrawingUnlocked(num3) != 0)
			{
				num++;
			}
		}
		return num;
	}

	public bool getCoppaShowed()
	{
		return getBooleanForKey("PREFS_COPPA_SHOWED");
	}

	public void setCoppaShowed(bool b)
	{
		setBooleanforKey(b, "PREFS_COPPA_SHOWED", comit: true);
	}

	public void setInitialMovieonStartup(bool b)
	{
		setBooleanforKey(b, "PREFS_INTRO_MOVIE_ON_STARTUP", comit: true);
	}

	public bool getInitialMovieonStartup()
	{
		return getBooleanForKey("PREFS_INTRO_MOVIE_ON_STARTUP");
	}

	public void setUserAge(int age)
	{
		setIntforKey(age, "PREFS_USER_AGE", comit: true);
	}

	public int getUserAge()
	{
		return getIntForKey("PREFS_USER_AGE");
	}

	public bool isCoppaRestricted()
	{
		return getBooleanForKey("PREFS_COPPA_RESTRICTED");
	}

	public void setCoppaRestricted(bool b)
	{
		setBooleanforKey(b, "PREFS_COPPA_RESTRICTED", comit: true);
	}

	public static int getNewDrawingsCounter()
	{
		return Preferences._getIntForKey("PREFS_NEW_DRAWINGS_COUNTER");
	}

	public static void setNewDrawingsCounter(int c)
	{
		Preferences._setIntforKey(c, "PREFS_NEW_DRAWINGS_COUNTER", comit: true);
	}

	public static void increaseNewDrawingsCounter()
	{
		setNewDrawingsCounter(getNewDrawingsCounter() + 1);
	}

	public static int DRAWINGS_COUNT()
	{
		return 7 - (/*Application.sharedPreferences().remoteDataManager.getHideSocialNetworks()*/false ? 1 : 0);
	}
}
