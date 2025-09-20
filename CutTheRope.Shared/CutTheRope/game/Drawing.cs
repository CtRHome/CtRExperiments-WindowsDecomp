using System;
using System.Collections.Generic;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.game;

internal class Drawing : GameObject, TimelineDelegate, ButtonDelegate
{
	private const int DRAWING_ANI_SHOW = 0;

	private const int DRAWING_ANI_HIDE = 1;

	private const int DRAWING_BUTTON_SHARE = 0;

	private const int DRAWING_BUTTON_LIKE = 1;

	public const int DRAWING_LOCKED = -1;

	public const int DRAWING_FACEBOOK = -2;

	private bool ingame;

	public int dg;

	private RectangleElement dim;

	private Image holder;

	private Image drawing;

	private Text tapText;

	public bool drawingVisible;

	private DelayedDispatcher dd;

	public static bool touchUpHook = false;

	public DrawingDelegate delegateDrawingDelegate;

	private RGBAColor brownColor = new RGBAColor(42.0 / 85.0, 27.0 / 85.0, 52.0 / 255.0, 1.0);

	private static int[] images = new int[8] { 228, 229, 230, 231, 0, 233, 232, 234 };

	private static NSString[] pictureLinks = new NSString[8]
	{
		NSObject.NSS("vtwa"),
		NSObject.NSS("njrf"),
		NSObject.NSS("nwyj"),
		NSObject.NSS("jzri"),
		NSObject.NSS("vwaj"),
		NSObject.NSS("olkj"),
		NSObject.NSS("vwaj"),
		NSObject.NSS("vhor")
	};

	public override void dealloc()
	{
		if (dd != null)
		{
			dd.cancelAllDispatches();
			NSObject.NSREL(dd);
			dd = null;
		}
		base.dealloc();
	}

	public override void update(float delta)
	{
		base.update(delta);
		if (dd != null)
		{
			dd.update(delta);
		}
	}

	public virtual NSObject initWithHiddenDrawing(int h, int d)
	{
		if (initWithTexture(Application.getTexture(122)) != null)
		{
			setDrawQuad(h);
			ingame = true;
			dg = d;
			delegateDrawingDelegate = null;
			passTransformationsToChilds = false;
		}
		return this;
	}

	public virtual NSObject initWithDrawing(int d)
	{
		if (initWithTexture(Application.getTexture(245)) != null)
		{
			setDrawQuad(2);
			dg = d;
			if (dg == -2)
			{
				Image image = Image.Image_createWithResID(245);
				image.setDrawQuad(7);
				image.doRestoreCutTransparency();
				image.anchor = (image.parentAnchor = 18);
				Image.setElementPositionWithRelativeQuadOffset(image, 244, 35, 43);
				image.setName(NSObject.NSS("thumb"));
				addChild(image);
			}
			else if (dg != -1)
			{
				Image image2 = Image.Image_createWithResID(254);
				image2.setDrawQuad(dg);
				image2.doRestoreCutTransparency();
				image2.anchor = (image2.parentAnchor = 18);
				Image.setElementPositionWithRelativeQuadOffset(image2, 244, 32 + dg, 24 + dg);
				image2.setName(NSObject.NSS("thumb"));
				addChild(image2);
			}
			ingame = false;
			delegateDrawingDelegate = null;
			passTransformationsToChilds = false;
			passColorToChilds = false;
		}
		return this;
	}

	public virtual void showDrawing()
	{
		if (dg >= 0 && dg != -2)
		{
			Application.sharedResourceMgr().loadResource(images[dg], ResourceMgr.ResourceType.IMAGE);
		}
		createPopupAnimation();
		drawingVisible = true;
		if (delegateDrawingDelegate != null)
		{
			delegateDrawingDelegate.drawingShowing(this);
		}
		if (dim != null)
		{
			dim.playTimeline(0);
		}
		if (holder != null)
		{
			holder.playTimeline(0);
		}
		touchUpHook = false;
	}

	public virtual void hideDrawing()
	{
		drawingVisible = false;
		if (tapText != null)
		{
			tapText.visible = false;
			tapText = null;
		}
		if (dim != null)
		{
			dim.playTimeline(1);
		}
		if (holder != null)
		{
			holder.playTimeline(1);
		}
	}

	public virtual void drawDrawing()
	{
		OpenGL.glDisable(0);
		if (dim != null)
		{
			dim.draw();
		}
		OpenGL.glEnable(0);
		if (holder != null)
		{
			holder.draw();
		}
	}

	public virtual void createPopupAnimation()
	{
		if (dim != null)
		{
			removeChild(dim);
			dim = null;
		}
		dim = (RectangleElement)new RectangleElement().init();
		dim.width = (int)FrameworkTypes.SCREEN_WIDTH;
		dim.height = (int)FrameworkTypes.SCREEN_HEIGHT;
		dim.passTransformationsToChilds = false;
		dim.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
		dim.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		dim.visible = false;
		dim.color = RGBAColor.transparentRGBA;
		addChild(dim);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 0.0), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 0.5), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
		dim.addTimelinewithID(timeline, 0);
		timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 0.5), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 0.0), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
		dim.addTimelinewithID(timeline, 1);
		holder = Image.Image_createWithResID(8);
		holder.doRestoreCutTransparency();
		if (dg == -1)
		{
			holder.passColorToChilds = false;
			drawing = null;
			Text text = new Text().initWithFont(Application.getFont(6));
			text.setAlignment(2);
			text.setStringandWidth(Application.getString(1179705), (double)holder.width - (double)holder.width / 3.0);
			text.parentAnchor = (text.anchor = 18);
			holder.addChild(text);
		}
		else if (dg == -2)
		{
			holder.passColorToChilds = false;
			drawing = Image.Image_createWithResIDQuad(245, 6);
			drawing.anchor = (drawing.parentAnchor = 18);
			holder.addChild(drawing);
			Text text2 = new Text().initWithFont(Application.getFont(6));
			text2.anchor = 18;
			text2.setAlignment(2);
			text2.setStringandWidth(Application.getString(1179706), (double)holder.width - (double)holder.width / 3.0);
			text2.parentAnchor = (text2.anchor = 18);
			holder.addChild(text2);
		}
		else
		{
			drawing = Image.Image_createWithResID(images[dg]);
			GameScene.enableFingercutsCreation(b: false);
			drawing.doRestoreCutTransparency();
			drawing.anchor = (drawing.parentAnchor = 18);
			holder.addChild(drawing);
		}
		holder.visible = false;
		dim.addChild(holder);
		dim.passColorToChilds = false;
		Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline2.addKeyFrame(KeyFrame.makeScale(0.0, 0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline2.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
		holder.addTimelinewithID(timeline2, 0);
		timeline2.delegateTimelineDelegate = this;
		timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline2.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline2.addKeyFrame(KeyFrame.makeScale(0.0, 0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
		holder.addTimelinewithID(timeline2, 1);
		timeline2.delegateTimelineDelegate = this;
		tapText = new Text().initWithFont(Application.getFont(5));
		tapText.visible = false;
		tapText.anchor = (tapText.parentAnchor = 18);
		tapText.scaleX = (tapText.scaleY = 0.7f);
		tapText.setString(Application.getString(1179693));
		tapText.y = FrameworkTypes.SCREEN_HEIGHT - (holder.y + (float)(holder.height / 2));
		holder.addChild(tapText);
		if (ingame)
		{
			FontGeneric font = Application.getFont(5);
			NSString @string = Application.getString(1179712);
			Text text3 = new Text().initWithFont(font);
			text3.parentAnchor = 34;
			text3.anchor = 34;
			text3.setString(@string);
			if (drawing != null)
			{
				drawing.addChild(text3);
			}
			else
			{
				holder.addChild(text3);
			}
			AnimationsPool animationsPool = (AnimationsPool)new AnimationsPool().init();
			holder.addChild(animationsPool);
			Image image = Image.Image_createWithResID(10);
			image.doRestoreCutTransparency();
			StarsBreak starsBreak = (StarsBreak)new StarsBreak().initWithTotalParticlesandImageGrid(10, image);
			starsBreak.particlesDelegate = animationsPool.particlesFinished;
			starsBreak.startSystem(10);
			animationsPool.addChild(starsBreak);
		}
	}

	public virtual void removePopupAnimation()
	{
		if (holder != null)
		{
			if (drawing != null)
			{
				holder.removeChild(drawing);
			}
			dim.removeChild(holder);
			holder = null;
		}
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}

	public virtual void timelineFinished(Timeline t)
	{
		if (holder == null)
		{
			return;
		}
		int num = holder.getCurrentTimelineIndex();
		if (num == 1)
		{
			removePopupAnimation();
			if (dg != -1 && dg != -2)
			{
				Application.sharedResourceMgr().freeResource(CTRResourceMgr.handleResource(images[dg]));
			}
			GameScene.enableFingercutsCreation(b: true);
			if (delegateDrawingDelegate != null)
			{
				delegateDrawingDelegate.drawingHidden(this);
			}
		}
		else
		{
			tapText.visible = true;
		}
	}

	public virtual void onButtonPressed(int n)
	{
		CTRSoundMgr._playSound(19);
		switch (n)
		{
		case 1:
			if (dd == null)
			{
				dd = (DelayedDispatcher)new DelayedDispatcher().init();
			}
			dd.callObjectSelectorParamafterDelay(selector_openFacebook, this, 0.7f);
			break;
		case 0:
		{
			NSString pictureLink = NSObject.NSS(string.Concat("http://zeptolab.com/photos/", pictureLinks[dg], ".jpg"));
			NSString redirectLink = NSObject.NSS("http://zeptolab.com/photos/photo.php?d=" + pictureLinks[dg]);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["name"] = "Cut the Rope";
			dictionary["link"] = "http://www.zeptolab.com/";
			NSString appName = NSObject.NSS("Cut the Rope");
			NSString @string = Application.getString(1179711);
			NSString string2 = Application.getString(1179710);
			Scorer.facebookConnectAndPostOnWall(string2, @string, pictureLink, redirectLink, dictionary, appName);
			break;
		}
		}
	}

	public static bool shouldCutTouches(TouchCollection tc)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		float num = 4f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = tc[0].Position.X;
		float num8 = tc[0].Position.Y;
		for (int i = 1; i < tc.Count; i++)
		{
			TouchLocation touchLocation = tc[i];
			num5 = Math.Abs(num7 - touchLocation.Position.X);
			num6 = Math.Abs(num8 - touchLocation.Position.Y);
			if (num5 > num2)
			{
				num2 = num5;
			}
			if (num6 > num3)
			{
				num3 = num6;
			}
		}
		num4 = num2 * num2 + num3 * num3;
		if (num4 < num)
		{
			return true;
		}
		return false;
	}

	public override bool onTouchDownXY(float tx, float ty)
	{
		if (base.onTouchDownXY(tx, ty))
		{
			return true;
		}
		Timeline timeline = ((dim != null) ? dim.getCurrentTimeline() : null);
		if (timeline != null && timeline.state == Timeline.TimelineState.TIMELINE_PLAYING)
		{
			return false;
		}
		if (drawingVisible)
		{
			hideDrawing();
			return true;
		}
		Vector quadSize = Image.getQuadSize(254, 0);
		Vector quadOffset = Image.getQuadOffset(254, 0);
		if (MathHelper.pointInRect(tx, ty, drawX + quadOffset.x, drawY + quadOffset.y, quadSize.x, quadSize.y))
		{
			showDrawing();
			return true;
		}
		return false;
	}

	public virtual void openFacebookImage()
	{
		dg = 3;
		CTRPreferences.setDrawingUnlocked(dg, 1);
		color = RGBAColor.whiteRGBA;
		BaseElement childWithName = getChildWithName(NSObject.NSS("thumb"));
		if (childWithName != null)
		{
			removeChild(childWithName);
		}
		Image image = Image.Image_createWithResID(254);
		image.doRestoreCutTransparency();
		image.setDrawQuad(dg);
		image.anchor = (image.parentAnchor = 18);
		Image.setElementPositionWithRelativeQuadOffset(image, 244, 32 + dg, 24 + dg);
		addChild(image);
		drawingVisible = false;
		if (tapText != null)
		{
			tapText.visible = false;
			tapText = null;
		}
		removePopupAnimation();
		if (!ingame)
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			MenuController menuController = (MenuController)cTRRootController.getChild(1);
			menuController.updateNewDrawingsCounter();
		}
		int num = 0;
		for (int i = 0; i <= 7; i++)
		{
			if (CTRPreferences.getDrawingUnlocked(i) != 0)
			{
				num++;
			}
		}
		if (num == 4)
		{
			CTRRootController.postAchievementName(NSObject.NSS("acPhotoObserver"));
		}
		showDrawing();
	}

	public static BaseElement createFacebookShortButtonWithTextIDDelegate(NSString str, int bid, ButtonDelegate d)
	{
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		bool flag = font.stringWidth(str) > 78f;
		Image image = null;
		Image image2 = null;
		if (flag)
		{
			image = Image.Image_createWithResIDQuad(4, 0);
			image2 = Image.Image_createWithResIDQuad(4, 1);
		}
		else
		{
			image = Image.Image_createWithResIDQuad(9, 0);
			image2 = Image.Image_createWithResIDQuad(9, 1);
		}
		image.addChild(text);
		image2.addChild(text2);
		text2.x = (text.x = 3f);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 0.0);
		button.delegateButtonDelegate = d;
		button.parentAnchor = (button.anchor = 9);
		baseElement.width = button.width;
		baseElement.height = button.height;
		baseElement.addChild(button);
		Image image3 = Image.Image_createWithResIDQuad(75, 2);
		image3.parentAnchor = (image3.anchor = 17);
		image3.y = -2f;
		baseElement.addChild(image3);
		return baseElement;
	}

	private static void selector_openFacebook(NSObject obj)
	{
		if (obj != null)
		{
			((Drawing)obj).openFacebookImage();
		}
		AndroidAPI.openUrl("http://www.facebook.com/cuttherope");
	}
}
