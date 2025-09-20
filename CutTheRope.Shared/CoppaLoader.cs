using CutTheRope.iframework;
using CutTheRope.ios;

internal class CoppaLoader
{
	private static bool hideCoppaPopupIsExplicit;

	private static bool hideCoppaPopup;

	private CoppaLoader()
	{
		hideCoppaPopupIsExplicit = false;
		hideCoppaPopup = false;
	}

	public static bool getHideCoppaPopupIsExplicit()
	{
		return hideCoppaPopupIsExplicit;
	}

	public static bool getHideCoppaPopup()
	{
		return hideCoppaPopup;
	}

	public static void setHideCoppaPopupIsExplicit(bool a)
	{
		hideCoppaPopupIsExplicit = a;
	}

	public static void setHideCoppaPopup(bool a)
	{
		hideCoppaPopup = a;
	}

	private static NSString getPossibleBannersResolutions()
	{
		int num = (int)FrameworkTypes.CHOOSE3(0.0, 1.0, 2.0);
		int[,] array = new int[3, 2]
		{
			{ 320, 200 },
			{ 480, 300 },
			{ 800, 400 }
		};
		NSString result = new NSString(string.Format("curtain:%dx%d", array[num, 0], array[num, 1]));
		_ = FrameworkTypes.SCREEN_RATIO;
		FrameworkTypes.CHOOSE3(0.0, 1.0, 2.0);
		return result;
	}
}
