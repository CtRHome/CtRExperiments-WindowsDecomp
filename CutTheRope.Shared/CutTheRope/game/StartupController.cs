using System;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.media;
using CutTheRope.ios;
using CutTheRope.utils;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.game;

internal class StartupController : ViewController, ResourceMgrDelegate, MovieMgrDelegate
{
	private enum StartupControllerViewId
	{
		VIEW_ZEPTOLAB
	}

	public override NSObject initWithParent(ViewController p)
	{
		base.initWithParent(p);
		return this;
	}

	public virtual void moviePlaybackFinished(NSString url)
	{
		ResourceMgr resourceMgr = Application.sharedResourceMgr();
		resourceMgr.resourcesDelegate = this;
		resourceMgr.initLoading();
		resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_COMMON);
		resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_COMMON_IMAGES);
		resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_MENU);
		resourceMgr.loadPack(ResDataPhoneFullExperiments.PACK_MUSIC);
		resourceMgr.startLoading();
		showView(0);
	}

	public override void activate()
	{
		FrameworkTypes._LOG("!!!!!!!!!!!!! activate");
		base.activate();
		/*
		int packsCount = CTRPreferences.getPacksCount();
		int levelsInPackCount = CTRPreferences.getLevelsInPackCount();
		for (int i = 0; i < packsCount; i++)
		{
			FrameworkTypes._LOG("{");
			for (int j = 0; j < levelsInPackCount; j++)
			{
				NSString mD = MathHelper.getMD5(ContentHelper.OpenResourceAsString("maps/" + LevelsList.LEVEL_NAMES[i, j].ToString()).ToCharArray());
				FrameworkTypes._LOG("NSS(\"" + mD.ToString() + "\"),");
			}
			FrameworkTypes._LOG("},");
		}
		*/
		StartupView startupView = (StartupView)new StartupView().initFullscreen();
		addViewwithID(startupView, 0);
		NSObject.NSREL(startupView);
		moviePlaybackFinished(null);
	}

	public virtual void onVideoBannerFinished()
	{
		Application.sharedRootController().setViewTransition(4);
		base.deactivate();
	}

	public override void update(float delta)
	{
		base.update(delta);
	}

	public override bool touchesBeganwithEvent(TouchCollection touches)
	{
		return true;
	}

	public virtual void resourceLoaded(int resName)
	{
		FrameworkTypes._LOG("res loaded");
	}

	public virtual void allResourcesLoaded()
	{
		GC.Collect();
		FrameworkTypes._LOG("all res loaded");
		int num = Preferences._getIntForKey("PREFS_GAME_STARTS");
		Preferences._setIntforKey(num + 1, "PREFS_GAME_STARTS", comit: false);
		if (CTRPreferences.isBannersMustBeShown())
		{
			AndroidAPI.showVideoBanner();
		}
		else
		{
			onVideoBannerFinished();
		}
	}

	public override bool backButtonPressed()
	{
		return false;
	}
}
