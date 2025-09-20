using System;
using System.Collections.Generic;
using CutTheRope.ctr_commons;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using Microsoft.Xna.Framework;
using MathHelper = CutTheRope.iframework.helpers.MathHelper;

namespace CutTheRope.game;

internal class CTRRootController : RootController
{
	public const int NEXT_GAME = 0;

	public const int NEXT_MENU = 1;

	public const int NEXT_PICKER = 2;

	public const int NEXT_PICKER_NEXT_PACK = 3;

	public const int NEXT_PICKER_SHOW_UNLOCK = 4;

	public const int CHILD_START = 0;

	public const int CHILD_MENU = 1;

	public const int CHILD_LOADING = 2;

	public const int CHILD_GAME = 3;

	public const int CHILD_COPPA = 4;

	public int pack;

	private NSString mapName;

	private XMLNode loadedMap;

	private int level;

	private bool picker;

	private bool survival;

	private bool inCrystal;

	private bool showGreeting;

	private bool hacked;

	public virtual void setMap(XMLNode map)
	{
		loadedMap = map;
	}

	public virtual XMLNode getMap()
	{
		return loadedMap;
	}

	public virtual NSString getMapName()
	{
		return mapName;
	}

	public virtual void setMapName(NSString map)
	{
		NSObject.NSREL(mapName);
		mapName = map;
	}

	public virtual void setMapsList(Dictionary<string, XMLNode> l)
	{
	}

	public virtual Dictionary<string, XMLNode> getMapsList()
	{
		throw new NotImplementedException();
	}

	public virtual int getPack()
	{
		return pack;
	}

	public override NSObject initWithParent(ViewController p)
	{
		if (base.initWithParent(p) != null)
		{
			hacked = false;
			loadedMap = null;
			ResourceMgr resourceMgr = Application.sharedResourceMgr();
			resourceMgr.initLoading();
			resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_STARTUP);
			resourceMgr.loadImmediately();
			new Texture2D().initWithPath("ctre/ctre_live_tile_0", assets: true);
			new Texture2D().initWithPath("ctre/ctre_live_tile_star", assets: true);
			StartupController startupController = (StartupController)new StartupController().initWithParent(this);
			addChildwithID(startupController, 0);
			NSObject.NSREL(startupController);
			viewTransition = -1;
		}
		return this;
	}

	public override void activate()
	{
		CTRPreferences.isFirstLaunch();
		base.activate();
		activateChild(0);
		Application.sharedCanvas().beforeRender();
		activeChild().activeView().draw();
		Application.sharedCanvas().afterRender();
	}

	public virtual void deleteMenu()
	{
		ResourceMgr resourceMgr = Application.sharedResourceMgr();
		deleteChild(1);
		resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_MENU);
	}

	public virtual void disableGameCenter()
	{
	}

	public virtual void enableGameCenter()
	{
	}

	public override void suspend()
	{
		suspended = true;
	}

	public override void resume()
	{
		if (!inCrystal)
		{
			suspended = false;
		}
	}

	private void initMenu(ResourceMgr rm)
	{
		FrameworkTypes._LOG("start deactivating");
		if (FrameworkTypes.IS_WVGA)
		{
			setViewTransition(4);
		}
		LoadingController c = (LoadingController)new LoadingController().initWithParent(this);
		addChildwithID(c, 2);
		FrameworkTypes._LOG("start deactivating2");
		FrameworkTypes._LOG("start deactivating3");
		MenuController menuController = (MenuController)new MenuController().initWithParent(this);
		addChildwithID(menuController, 1);
		FrameworkTypes._LOG("start deactivating4");
		deleteChild(0);
		rm.freePack(ResDataPhoneFullExperiments.PACK_STARTUP);
		menuController.viewToShow = MenuController.ViewID.VIEW_MAIN_MENU;
		if (Preferences._getBooleanForKey("PREFS_GAME_CENTER_ENABLED"))
		{
			enableGameCenter();
		}
		else
		{
			disableGameCenter();
		}
		if (Preferences._getBooleanForKey("IAP_BANNERS"))
		{
			AndroidAPI.disableBanners();
		}
		FrameworkTypes._LOG("activate child menu");
		activateChild(1);
		if (CTRPreferences.isFirstLaunch() && SaveMgr.isSaveAvailable())
		{
			menuController.showYesNoPopup(Application.getString(1179699), 29, 30);
		}
	}

	public override void onChildDeactivated(int n)
	{
		base.onChildDeactivated(n);
		ResourceMgr resourceMgr = Application.sharedResourceMgr();
		switch (n)
		{
			case 0:
			{
                initMenu(resourceMgr);
                break;
			}
			case 4:
				deleteChild(4);
				initMenu(resourceMgr);
				break;
			case 1:
			{
				deleteMenu();
				resourceMgr.resourcesDelegate = (LoadingController)getChild(2);
				int[] array = null;
				switch (pack)
				{
					case 0:
						array = ResDataPhoneFullExperiments.PACK_GAME_01;
						break;
					case 1:
						array = ResDataPhoneFullExperiments.PACK_GAME_02;
						break;
					case 2:
						array = ResDataPhoneFullExperiments.PACK_GAME_03;
						break;
					case 3:
						array = ResDataPhoneFullExperiments.PACK_GAME_04;
						break;
					case 4:
						array = ResDataPhoneFullExperiments.PACK_GAME_05;
						break;
					case 5:
						array = ResDataPhoneFullExperiments.PACK_GAME_06;
						break;
				}
				resourceMgr.initLoading();
				resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_GAME);
				resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_GAME_NORMAL);
				resourceMgr.loadPack(array);
				resourceMgr.startLoading();
				LoadingController loadingController3 = (LoadingController)getChild(2);
				loadingController3.nextController = 0;
				loadingController3.MusicToLoad = ((pack <= 3) ? 198 : 199);
				activateChild(2);
				break;
			}
			case 2:
			{
				LoadingController loadingController2 = (LoadingController)getChild(2);
				int nextController = loadingController2.nextController;
				switch (nextController)
				{
					case 0:
					{
						setShowGreeting(s: true);
						GameController c = (GameController)new GameController().initWithParent(this);
						addChildwithID(c, 3);
						activateChild(3);
						break;
					}
					case 1:
					case 2:
					case 3:
					case 4:
					{
						MenuController menuController = (MenuController)new MenuController().initWithParent(this);
						addChildwithID(menuController, 1);
						if (FrameworkTypes.IS_WVGA)
						{
							setViewTransition(4);
						}
						if (nextController == 1)
						{
							menuController.viewToShow = MenuController.ViewID.VIEW_MAIN_MENU;
						}
						if (nextController == 2 || nextController == 4)
						{
							menuController.viewToShow = MenuController.ViewID.VIEW_LEVEL_SELECT;
						}
						if (nextController == 3)
						{
							menuController.viewToShow = ((pack < CTRPreferences.getPacksCount() - 1) ? MenuController.ViewID.VIEW_PACK_SELECT : MenuController.ViewID.VIEW_MOVIE);
						}
						activateChild(1);
						if (nextController == 4)
						{
							menuController.showUnlockShareware();
						}
						if (nextController == 3)
						{
							menuController.showNextPack();
						}
						break;
					}
				}
				break;
			}
			case 3:
			{
				SaveMgr.backup();
				GameController gameController = (GameController)getChild(3);
				int exitCode = gameController.exitCode;
				_ = (GameScene)gameController.getView(0).getChild(0);
				switch (exitCode)
				{
					case 0:
					case 1:
					case 2:
					case 3:
					{
						deleteChild(3);
						resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME);
						resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME_NORMAL);
						resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME_01);
						resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME_02);
						if (!CTRPreferences.isLiteVersion())
						{
							resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME_03);
							resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME_04);
							resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME_05);
							resourceMgr.freePack(ResDataPhoneFullExperiments.PACK_GAME_06);
						}
						resourceMgr.resourcesDelegate = (LoadingController)getChild(2);
						resourceMgr.initLoading();
						resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_MENU);
						resourceMgr.startLoading();
						LoadingController loadingController = (LoadingController)getChild(2);
						switch (exitCode)
						{
						case 0:
							loadingController.nextController = 1;
							break;
						case 1:
							loadingController.nextController = 2;
							break;
						case 3:
							loadingController.nextController = 4;
							break;
						default:
							loadingController.nextController = 3;
							break;
						}
						loadingController.MusicToLoad = 197;
						activateChild(2);
						break;
					}
				}
				break;
			}
		}
	}

	public override void dealloc()
	{
		loadedMap = null;
		mapName = null;
		base.dealloc();
	}

	public static void checkMapIsValid(char[] data)
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		NSString mD = MathHelper.getMD5(data);
		int num = cTRRootController.getPack();
		int num2 = cTRRootController.getLevel();
		if (!mD.isEqualToString(LevelsList.LEVEL_HASHES[num, num2]))
		{
			setHacked();
			FrameworkTypes._LOG("Map is hacked");
		}
		else
		{
			FrameworkTypes._LOG("Map is not hacked");
		}
	}

	public static bool isHacked()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		return cTRRootController.hacked;
	}

	public static void setHacked()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		cTRRootController.hacked = true;
	}

	public static void setInCrystal(bool b)
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		cTRRootController.inCrystal = b;
	}

	public static void openFullVersionPage()
	{
	}

	public virtual void setPack(int p)
	{
		pack = p;
	}

	public virtual void setLevel(int l)
	{
		level = l;
	}

	public virtual int getLevel()
	{
		return level;
	}

	public virtual void setPicker(bool p)
	{
		picker = p;
	}

	public virtual bool isPicker()
	{
		return picker;
	}

	public virtual void setSurvival(bool s)
	{
		survival = s;
	}

	public virtual bool isSurvival()
	{
		return survival;
	}

	public static bool isShowGreeting()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		return cTRRootController.showGreeting;
	}

	public static void setShowGreeting(bool s)
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		cTRRootController.showGreeting = s;
	}

	public static void postAchievementName(NSString name)
	{
		Scorer.postAchievementName(name);
	}
}
