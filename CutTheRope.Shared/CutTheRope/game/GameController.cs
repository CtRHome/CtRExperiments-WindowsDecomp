using System.Collections.Generic;
using CutTheRope.ctr_commons;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.Specials;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.game;

internal class GameController : ViewController, ButtonDelegate
{
	private const int BUTTON_PAUSE_RESUME = 0;

	private const int BUTTON_PAUSE_RESTART = 1;

	private const int BUTTON_PAUSE_SKIP = 2;

	private const int BUTTON_PAUSE_LEVEL_SELECT = 3;

	private const int BUTTON_PAUSE_EXIT = 4;

	public const int BUTTON_WIN_EXIT = 5;

	private const int BUTTON_PAUSE = 6;

	private const int BUTTON_NEXT_LEVEL = 7;

	public const int BUTTON_WIN_RESTART = 8;

	public const int BUTTON_WIN_NEXT_LEVEL = 9;

	public const int EXIT_CODE_FROM_PAUSE_MENU = 0;

	public const int EXIT_CODE_FROM_PAUSE_MENU_LEVEL_SELECT = 1;

	public const int EXIT_CODE_FROM_PAUSE_MENU_LEVEL_SELECT_NEXT_PACK = 2;

	public const int EXIT_CODE_FROM_PAUSE_MENU_LEVEL_SELECT_SHOW_UNLOCK = 3;

	public bool isGamePaused;

	public int exitCode;

	private Text mapNameLabel;

	private TouchLocation?[] touchAddressMap = new TouchLocation?[5];

	private bool boxCloseHandled;

	private bool shouldDoNextLevel;

	private float tmpDimTime;

	public override NSObject initWithParent(ViewController p)
	{
		if (base.initWithParent(p) != null)
		{
			createGameView();
			shouldDoNextLevel = false;
		}
		return this;
	}

	public override void activate()
	{
		CTRPreferences.gameViewChanged(NSObject.NSS("game"));
		Application.sharedRootController().setViewTransition(-1);
		base.activate();
		CTRSoundMgr._stopMusic();
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		if (cTRRootController.getPack() <= 3)
		{
			CTRSoundMgr._playMusic(198);
		}
		else
		{
			CTRSoundMgr._playMusic(199);
		}
		initGameView();
		showView(0);
	}

	public override void dealloc()
	{
		base.dealloc();
	}

	private new void deactivate()
	{
		View view = getView(0);
		BoxOpenClose boxOpenClose = (BoxOpenClose)view.getChild(5);
		if (boxOpenClose != null)
		{
			boxOpenClose.delegateboxClosed = null;
		}
		base.deactivate();
	}

	public override void update(float delta)
	{
		base.update(delta);
		/*
		if (App.NeedsUpdate)
		{
			UpdatePopup.showUpdatePopup();
		}
		*/
		if (shouldDoNextLevel)
		{
			shouldDoNextLevel = false;
			onNextLevel();
		}
	}

	public virtual void boxClosed()
	{
		Application.sharedPreferences();
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		_ = CTRPreferences.getLevelsInPackCount() - 1;
		checkForBoxPerfect(pack);
		Preferences._savePreferences();
		int num = 0;
		for (int i = 0; i < CTRPreferences.getLevelsInPackCount(); i++)
		{
			num += CTRPreferences.getScoreForPackLevel(pack, i);
		}
		if (!CTRRootController.isHacked())
		{
			Application.sharedPreferences().setScoreHash();
			Preferences._savePreferences();
			Scorer.postLeaderboardResultforLaderboardIdlowestValFirstforGameCenter(num, pack, islowestValFirstforGameCenter: false);
		}
		boxCloseHandled = true;
	}

	public virtual void gameWon()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		string item = pack + "_" + level;
		View view = getView(0);
		GameScene gameScene = (GameScene)view.getChild(0);
		FlurryAPI.logEvent("LEVSCR_LEVEL_WON", new List<string>
		{
			"level",
			item,
			"stars",
			gameScene.starsCollected.ToString(),
			"game_unlocked",
			CTRPreferences.isLiteVersion() ? "0" : "1"
		});
		levelWon();
	}

	public virtual void gameLost()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		string item = pack + "_" + level;
		FlurryAPI.logEvent("LEVSCR_LEVEL_LOST", new List<string>
		{
			"level",
			item,
			"game_unlocked",
			CTRPreferences.isLiteVersion() ? "0" : "1"
		});
	}

	public virtual void onButtonPressed(int n)
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		View view = getView(0);
		view.onTouchMoveXY(-10000f, -10000f);
		CTRSoundMgr._playSound(19);
		switch (n)
		{
		case 0:
		{
			AndroidAPI.hideBanner();
			GameScene gameScene3 = (GameScene)view.getChild(0);
			gameScene3.dimTime = tmpDimTime;
			tmpDimTime = 0f;
			setPaused(p: false);
			break;
		}
		case 8:
			if (!boxCloseHandled)
			{
				boxClosed();
			}
			goto case 1;
		case 1:
		{
			AndroidAPI.hideBanner();
			GameScene gameScene2 = (GameScene)view.getChild(0);
			if (!gameScene2.isEnabled())
			{
				levelStart();
			}
			gameScene2.animateRestartDim = n == 1;
			gameScene2.reload();
			if (n != 1)
			{
				setPaused(p: false);
			}
			break;
		}
		case 2:
			AndroidAPI.hideBanner();
			if (lastLevelInPack() && !cTRRootController.isPicker())
			{
				if (CTRPreferences.isLiteVersion())
				{
					unlockNextLevel();
				}
				if (((CTRRootController)Application.sharedRootController()).getPack() == CTRPreferences.sharewareFreePacks() - 1)
				{
					MenuController.showBuyFull = true;
				}
				levelQuit();
			}
			else
			{
				unlockNextLevel();
				setPaused(p: false);
				GameScene gameScene4 = (GameScene)view.getChild(0);
				gameScene4.loadNextMap();
			}
			break;
		case 9:
		{
			AndroidAPI.hideBanner();
			CTRSoundMgr._stopLoopedSounds();
			if (!boxCloseHandled)
			{
				boxClosed();
			}
			int num = Preferences._getIntForKey("PREFS_LEVELS_WON");
			Preferences._setIntforKey(num + 1, "PREFS_LEVELS_WON", comit: false);
			if (CTRPreferences.isBannersMustBeShown())
			{
				GameView gameView = (GameView)view;
				gameView.videoAdLoading = true;
				AndroidAPI.showVideoBanner();
			}
			else
			{
				onNextLevel();
			}
			break;
		}
		case 7:
			AndroidAPI.hideBanner();
			onNextLevel();
			if (((CTRRootController)Application.sharedRootController()).getPack() == CTRPreferences.sharewareFreePacks() - 1)
			{
				MenuController.showBuyFull = true;
			}
			break;
		case 5:
			exitCode = 1;
			CTRSoundMgr._stopAll();
			if (!boxCloseHandled)
			{
				boxClosed();
			}
			deactivate();
			break;
		case 4:
		{
			View view3 = getView(0);
			BoxOpenClose boxOpenClose2 = (BoxOpenClose)view3.getChild(5);
			if (boxOpenClose2 != null)
			{
				boxOpenClose2.delegateboxClosed = null;
			}
			exitCode = 0;
			CTRSoundMgr._stopAll();
			levelQuit();
			break;
		}
		case 3:
		{
			View view2 = getView(0);
			BoxOpenClose boxOpenClose = (BoxOpenClose)view2.getChild(5);
			if (boxOpenClose != null)
			{
				boxOpenClose.delegateboxClosed = null;
			}
			exitCode = 1;
			CTRSoundMgr._stopAll();
			levelQuit();
			break;
		}
		case 6:
		{
			AndroidAPI.showBanner();
			GameScene gameScene = (GameScene)view.getChild(0);
			tmpDimTime = gameScene.dimTime;
			releaseAllTouches(gameScene);
			gameScene.dimTime = 0f;
			setPaused(p: true);
			break;
		}
		}
	}

	public override bool touchesBeganwithEvent(TouchCollection touches)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		View view = getView(0);
		GameView gameView = (GameView)view;
		if (gameView.videoAdLoading)
		{
			return true;
		}
		GameScene gameScene = (GameScene)view.getChild(0);
		if (gameScene.drawingPoppedUp == null && base.touchesBeganwithEvent(touches))
		{
			return true;
		}
		if (!gameScene.touchable)
		{
			return false;
		}
		foreach (TouchLocation item in touches)
		{
			if (item.State != TouchLocationState.Pressed)
			{
				continue;
			}
			int num = -1;
			for (int i = 0; i < 5; i++)
			{
				if (!touchAddressMap[i].HasValue)
				{
					ref TouchLocation? reference = ref touchAddressMap[i];
					reference = item;
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				gameScene.touchDownXYIndex(CtrRenderer.transformX(item.Position.X), CtrRenderer.transformY(item.Position.Y), num);
			}
		}
		return true;
	}

	public override bool touchesEndedwithEvent(TouchCollection touches)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		View view = getView(0);
		GameScene gameScene = (GameScene)view.getChild(0);
		if (gameScene.drawingPoppedUp == null && base.touchesEndedwithEvent(touches))
		{
			return true;
		}
		if (!gameScene.touchable)
		{
			return false;
		}
		foreach (TouchLocation item in touches)
		{
			if (item.State != TouchLocationState.Released)
			{
				continue;
			}
			int num = -1;
			for (int i = 0; i < 5; i++)
			{
				if (touchAddressMap[i].HasValue && touchAddressMap[i].Value.Id == item.Id)
				{
					touchAddressMap[i] = null;
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				gameScene.touchUpXYIndex(CtrRenderer.transformX(item.Position.X), CtrRenderer.transformY(item.Position.Y), num);
			}
			else
			{
				releaseAllTouches(gameScene);
			}
		}
		return true;
	}

	public override bool touchesMovedwithEvent(TouchCollection touches)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		View view = getView(0);
		GameScene gameScene = (GameScene)view.getChild(0);
		if (gameScene.drawingPoppedUp == null && base.touchesMovedwithEvent(touches))
		{
			return true;
		}
		if (!gameScene.touchable)
		{
			return false;
		}
		foreach (TouchLocation item in touches)
		{
			if (item.State != TouchLocationState.Moved)
			{
				continue;
			}
			int num = -1;
			for (int i = 0; i < 5; i++)
			{
				if (touchAddressMap[i].HasValue && touchAddressMap[i].Value.Id == item.Id)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				gameScene.touchMoveXYIndex(CtrRenderer.transformX(item.Position.X), CtrRenderer.transformY(item.Position.Y), num);
			}
		}
		return true;
	}

	public virtual void createGameView()
	{
		for (int i = 0; i < 5; i++)
		{
			touchAddressMap[i] = null;
		}
		GameView gameView = (GameView)new GameView().initFullscreen();
		GameScene gameScene = (GameScene)new GameScene().init();
		gameScene.gameSceneDelegate_gameWon = gameWon;
		gameScene.gameSceneDelegate_gameLost = gameLost;
		gameView.addChildwithID(gameScene, 0);
		Button button = MenuController.createButtonWithImageQuad1Quad2IDDelegate(121, 0, 1, 6, this);
		button.setTouchIncreaseLeftRightTopBottom(2f, 6f, 6f, 6f);
		button.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		button.x += FrameworkTypes.SCREEN_OFFSET_X;
		button.x += 0.33f;
		button.y += 0.33f;
		gameView.addChildwithID(button, 1);
		Button button2 = MenuController.createButtonWithImageQuad1Quad2IDDelegate(111, 0, 1, 1, this);
		button2.setTouchIncreaseLeftRightTopBottom(6f, 2f, 6f, 6f);
		button2.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		button2.x += FrameworkTypes.SCREEN_OFFSET_X;
		button2.x += 0.33f;
		button2.y += 0.33f;
		Button button3 = MenuController.createButtonWithImageQuad1Quad2IDDelegate(111, 0, 1, 7, this);
		button3.color = RGBAColor.redRGBA;
		button3.x = -40f;
		button3.setEnabled(e: false);
		button3.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		button3.x += FrameworkTypes.SCREEN_OFFSET_X;
		gameView.addChildwithID(button2, 2);
		gameView.addChildwithID(button3, 3);
		Image image = Image.Image_createWithResIDQuad(117, 0);
		image.anchor = (image.parentAnchor = 10);
		image.passTransformationsToChilds = false;
		image.y = 0f - FrameworkTypes.SCREEN_OFFSET_Y;
		image.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		mapNameLabel = new Text().initWithFont(Application.getFont(6));
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int scoreForPackLevel = CTRPreferences.getScoreForPackLevel(cTRRootController.getPack(), cTRRootController.getLevel());
		mapNameLabel.setString(NSObject.NSS(string.Concat(Application.getString(1179672), ",  %d", scoreForPackLevel)));
		mapNameLabel.anchor = (mapNameLabel.parentAnchor = 12);
		mapNameLabel.x = -10f;
		mapNameLabel.y = -5f;
		mapNameLabel.x += FrameworkTypes.SCREEN_OFFSET_X;
		image.addChild(mapNameLabel);
		VBox vBox = new VBox().initWithOffsetAlignWidth(5.0, 2, FrameworkTypes.SCREEN_WIDTH);
		Button c = MenuController.createButtonWithTextIDDelegate(Application.getString(1179689), 0, this);
		vBox.addChild(c);
		Button c2 = MenuController.createButtonWithTextIDDelegate(Application.getString(1179690), 2, this);
		vBox.addChild(c2);
		Button c3 = MenuController.createButtonWithTextIDDelegate(Application.getString(1179691), 3, this);
		vBox.addChild(c3);
		Button c4 = MenuController.createButtonWithTextIDDelegate(Application.getString(1179692), 4, this);
		vBox.addChild(c4);
		vBox.anchor = (vBox.parentAnchor = 10);
		vBox.y = 140f;
		image.addChild(vBox);
		gameView.addChildwithID(image, 4);
		addViewwithID(gameView, 0);
		BoxOpenClose boxOpenClose = (BoxOpenClose)new BoxOpenClose().initWithButtonDelegate(this);
		boxOpenClose.delegateboxClosed = boxClosed;
		gameView.addChildwithID(boxOpenClose, 5);
	}

	public virtual void initGameView()
	{
		setPaused(p: false);
		levelFirstStart();
	}

	public virtual void levelFirstStart()
	{
		View view = getView(0);
		((BoxOpenClose)view.getChild(5)).levelFirstStart();
		isGamePaused = false;
		view.getChild(0).touchable = true;
		view.getChild(1).touchable = true;
		view.getChild(2).touchable = true;
		CTRSoundMgr._playVoice(59 + MathHelper.RND_RANGE(0, 2));
	}

	public virtual void levelStart()
	{
		View view = getView(0);
		((BoxOpenClose)view.getChild(5)).levelStart();
		isGamePaused = false;
		view.getChild(0).touchable = true;
		view.getChild(1).touchable = true;
		view.getChild(2).touchable = true;
		view.getChild(5).touchable = false;
	}

	public virtual void levelWon()
	{
		Preferences._setBooleanforKey(v: true, "PREFS_INTRO_MOVIE_ON_STARTUP", comit: true);
		boxCloseHandled = false;
		Application.sharedPreferences();
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		CTRSoundMgr._playSound(42);
		View view = getView(0);
		view.getChild(5).touchable = true;
		GameScene gameScene = (GameScene)view.getChild(0);
		CTRSoundMgr._playSound(42);
		if (MathHelper.RND_RANGE(0, 1) == 0)
		{
			int[,] array = new int[5, 3]
			{
				{ 49, 50, -1 },
				{ 51, 52, -1 },
				{ 53, 54, 55 },
				{ 56, 57, 58 },
				{ 64, 65, 66 }
			};
			int num = -1;
			int num2 = gameScene.starsCollected;
			if (pack == 3)
			{
				num2++;
			}
			while (num == -1)
			{
				num = array[num2, MathHelper.RND_RANGE(0, 2)];
			}
			CTRSoundMgr._playVoice(num);
		}
		BoxOpenClose boxOpenClose = (BoxOpenClose)view.getChild(5);
		Image image = (Image)boxOpenClose.result.getChildWithName("star1");
		Image image2 = (Image)boxOpenClose.result.getChildWithName("star2");
		Image image3 = (Image)boxOpenClose.result.getChildWithName("star3");
		image.setDrawQuad((gameScene.starsCollected > 0) ? 13 : 14);
		image2.setDrawQuad((gameScene.starsCollected > 1) ? 13 : 14);
		image3.setDrawQuad((gameScene.starsCollected > 2) ? 13 : 14);
		Text text = (Text)boxOpenClose.result.getChildWithName("passText");
		text.setString(Application.getString(1179664 + gameScene.starsCollected));
		boxOpenClose.time = gameScene.time;
		boxOpenClose.starBonus = gameScene.starBonus;
		boxOpenClose.timeBonus = gameScene.timeBonus;
		boxOpenClose.score = gameScene.score;
		isGamePaused = true;
		gameScene.touchable = false;
		view.getChild(2).touchable = false;
		view.getChild(1).touchable = false;
		int scoreForPackLevel = CTRPreferences.getScoreForPackLevel(pack, level);
		int starsForPackLevel = CTRPreferences.getStarsForPackLevel(pack, level);
		boxOpenClose.shouldShowImprovedResult = false;
		if (gameScene.score > scoreForPackLevel)
		{
			CTRPreferences.setScoreForPackLevel(gameScene.score, pack, level);
			if (scoreForPackLevel > 0)
			{
				boxOpenClose.shouldShowImprovedResult = true;
			}
		}
		if (gameScene.starsCollected > starsForPackLevel)
		{
			CTRPreferences.setStarsForPackLevel(gameScene.starsCollected, pack, level);
			if (starsForPackLevel > 0)
			{
				boxOpenClose.shouldShowImprovedResult = true;
			}
		}
		boxOpenClose.shouldShowConfetti = gameScene.starsCollected == 3;
		CTRPreferences.gameViewChanged(NSObject.NSS("menu"));
		boxOpenClose.levelWon();
		unlockNextLevel();
	}

	public virtual void levelLost()
	{
		View view = getView(0);
		((BoxOpenClose)view.getChild(5)).levelLost();
	}

	public virtual void levelQuit()
	{
		View view = getView(0);
		((BoxOpenClose)view.getChild(5)).levelQuit();
		view.getChild(0).touchable = false;
	}

	public virtual void setPaused(bool p)
	{
		isGamePaused = p;
		View view = getView(0);
		view.getChild(4).setEnabled(p);
		view.getChild(1).setEnabled(!p);
		view.getChild(2).setEnabled(!p);
		view.getChild(0).touchable = !p;
		view.getChild(0).updateable = !p;
		if (isGamePaused)
		{
			CTRPreferences.gameViewChanged(NSObject.NSS("menu"));
			CTRSoundMgr._pause();
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			if (cTRRootController.isPicker())
			{
				mapNameLabel.setString(NSObject.NSS(""));
				return;
			}
			int scoreForPackLevel = CTRPreferences.getScoreForPackLevel(cTRRootController.getPack(), cTRRootController.getLevel());
			mapNameLabel.setString(NSObject.NSS(string.Concat(Application.getString(1179672), ": ", scoreForPackLevel)));
		}
		else
		{
			CTRPreferences.gameViewChanged(NSObject.NSS("game"));
			CTRSoundMgr._unpause();
		}
	}

	public static void checkForBoxPerfect(int pack)
	{
		CTRPreferences.isPackPerfect(pack);
	}

	public static void postFlurryLevelEvent(string s)
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		string item = pack + "_" + level;
		FlurryAPI.logEvent(s, new List<string> { "level", item });
	}

	public virtual bool lastLevelInPack()
	{
		Application.sharedPreferences();
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		if (level == CTRPreferences.getLevelsInPackCount() - 1)
		{
			exitCode = 2;
			CTRSoundMgr._stopAll();
			return true;
		}
		if (level == CTRPreferences.sharewareFreeLevels() - 1 && !CTRPreferences.isSharewareUnlocked())
		{
			exitCode = 3;
			CTRSoundMgr._stopAll();
			return true;
		}
		return false;
	}

	public virtual void unlockNextLevel()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		int num = CTRPreferences.getLevelsInPackCount() - 1;
		if (CTRPreferences.isLiteVersion())
		{
			num++;
		}
		if (level < num && CTRPreferences.getUnlockedForPackLevel(pack, level + 1) == UNLOCKED_STATE.UNLOCKED_STATE_LOCKED)
		{
			CTRPreferences.setUnlockedForPackLevel(UNLOCKED_STATE.UNLOCKED_STATE_UNLOCKED, pack, level + 1);
		}
	}

	public override bool backButtonPressed()
	{
		View view = getView(0);
		GameScene gameScene = (GameScene)view.getChild(0);
		if (gameScene.drawingPoppedUp != null)
		{
			gameScene.drawingPoppedUp.hideDrawing();
			return true;
		}
		AdSkipper adSkipper = (AdSkipper)view.getChild(7);
		if (adSkipper.active)
		{
			adSkipper.onButtonPressed(0);
		}
		else if (view.getChild(1).touchable)
		{
			onButtonPressed(6);
		}
		else if (view.getChild(4).isEnabled())
		{
			onButtonPressed(0);
		}
		else if (view.getChild(5).touchable)
		{
			onButtonPressed(5);
		}
		return true;
	}

	public override bool menuButtonPressed()
	{
		View view = getView(0);
		if (view.getChild(1).touchable)
		{
			onButtonPressed(6);
		}
		else if (view.getChild(4).isEnabled())
		{
			onButtonPressed(0);
		}
		return true;
	}

	public virtual void onNextLevel()
	{
		CTRPreferences.gameViewChanged(NSObject.NSS("game"));
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		View view = getView(0);
		GameView gameView = (GameView)view;
		gameView.videoAdLoading = false;
		gameView.unsetJSkipper();
		if (lastLevelInPack() && !cTRRootController.isPicker())
		{
			if (((CTRRootController)Application.sharedRootController()).getPack() == CTRPreferences.sharewareFreePacks() - 1)
			{
				MenuController.showBuyFull = true;
			}
			deactivate();
		}
		else
		{
			GameScene gameScene = (GameScene)view.getChild(0);
			gameScene.loadNextMap();
			levelStart();
		}
	}

	public virtual void onVideoBannerFinished()
	{
		shouldDoNextLevel = true;
	}

	public virtual void releaseAllTouches(GameScene gs)
	{
		for (int i = 0; i < 5; i++)
		{
			touchAddressMap[i] = null;
			gs.touchUpXYIndex(-500f, -500f, i);
		}
	}

	public virtual void setAdSkipper(object skipper)
	{
		View view = getView(0);
		GameView gameView = (GameView)view;
		gameView.setJSkipper(skipper);
		gameView.videoAdLoading = false;
	}
}
