using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using CutTheRope.ctr_commons;
using CutTheRope.game;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.media;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.Specials;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.ctr_original;

internal class MenuController : ViewController, ButtonDelegate, MovieMgrDelegate, ScrollableContainerProtocol, TimelineDelegate, LiftScrollbarDelegate
{
	private enum CHILD_TYPE
	{
		CHILD_PICKER
	}

	public enum ViewID
	{
		VIEW_MAIN_MENU,
		VIEW_OPTIONS,
		VIEW_HELP,
		VIEW_ABOUT,
		VIEW_RESET,
		VIEW_PACK_SELECT,
		VIEW_LEVEL_SELECT,
		VIEW_MOVIE,
		VIEW_DRAWINGS,
		VIEW_ACHIEVEMENTS,
		VIEW_LEADERBOARDS
	}

	public enum ButtonID
	{
		BUTTON_PLAY,
		BUTTON_OPTIONS,
		BUTTON_EXTRAS,
		BUTTON_SURVIVAL,
		BUTTON_CRYSTAL,
		BUTTON_BUYGAME,
		BUTTON_SOUND_ONOFF,
		BUTTON_VOICE_ONOFF,
		BUTTON_LOCALIZATION,
		BUTTON_MUSIC_ONOFF,
		BUTTON_ABOUT,
		BUTTON_RESET,
		BUTTON_HELP,
		BUTTON_BACK_TO_MAIN_MENU,
		BUTTON_BACK_TO_OPTIONS,
		BUTTON_BACK_TO_PACK_SELECT,
		BUTTON_RESET_YES,
		BUTTON_RESET_NO,
		BUTTON_PACK1,
		BUTTON_PACK2,
		BUTTON_PACK3,
		BUTTON_PACK4,
		BUTTON_PACK5,
		BUTTON_PACK6,
		BUTTON_PACK_SOON,
		BUTTON_GAMECENTER_ONOFF,
		BUTTON_TWITTER,
		BUTTON_FACEBOOK,
		BUTTON_EXIT_YES,
		BUTTON_RESTORE,
		BUTTON_POPUP_OK,
		BUTTON_POPUP_HIDE,
		BUTTON_CANDY,
		BUTTON_DRAWINGS,
		BUTTON_UNLOCK,
		BUTTON_DISABLEBANNERS,
		BUTTON_PROMO,
		BUTTON_TOYS,
		BUTTON_UNLOCK_SHAREWARE,
		BUTTON_ORIGINAL_CTR,
		BUTTON_GIFT,
		BUTTON_ACHIEVEMENTS_BACK,
		BUTTON_ACHIEVEMENTS,
		BUTTON_LEADERBOARDS_BACK,
		BUTTON_LEADERBOARDS,
		BUTTON_BUY_FULL,
		BUTTON_PRIVACY_P
	}

	private enum Status
	{
		STATUS,
		STATUS_RESTORE_BROKEN,
		STATUS_RESTORE_OK
	}

	private class TouchBaseElement : BaseElement
	{
		public int bid;

		public Rectangle bbc;

		public ButtonDelegate delegateButtonDelegate;

		public override bool onTouchDownXY(float tx, float ty)
		{
			base.onTouchDownXY(tx, ty);
			Rectangle r = FrameworkTypes.MakeRectangle(drawX + bbc.x, drawY + bbc.y, (float)width + bbc.w, (float)height + bbc.h);
			Rectangle rectangle = MathHelper.rectInRectIntersection(FrameworkTypes.MakeRectangle(0.0, 0.0, FrameworkTypes.SCREEN_WIDTH, FrameworkTypes.SCREEN_HEIGHT), r);
			if (MathHelper.pointInRect(tx, ty, r.x, r.y, r.w, r.h) && (double)rectangle.w > (double)r.w / 2.0)
			{
				if (delegateButtonDelegate != null)
				{
					delegateButtonDelegate.onButtonPressed(bid);
				}
				return true;
			}
			return false;
		}
	}

	private class MonsterSlot : Image
	{
		public ScrollableContainer c;

		public float s;

		public float e;

		private static MonsterSlot MonsterSlot_create(Texture2D t)
		{
			return (MonsterSlot)new MonsterSlot().initWithTexture(t);
		}

		private static MonsterSlot MonsterSlot_createWithResID(int r)
		{
			return MonsterSlot_create(Application.getTexture(r));
		}

		public static MonsterSlot MonsterSlot_createWithResIDQuad(int r, int q)
		{
			MonsterSlot monsterSlot = MonsterSlot_create(Application.getTexture(r));
			monsterSlot.setDrawQuad(q);
			return monsterSlot;
		}

		public override void draw()
		{
			base.preDraw();
			if (quadToDraw == -1)
			{
				GLDrawer.drawImage(texture, drawX, drawY);
			}
			else
			{
				drawQuad(quadToDraw);
			}
			float num = c.getScroll().x;
			if (num >= s && num < e)
			{
				OpenGL.glEnable(4);
				float num2 = num - (s + e) / 2f;
				OpenGL.setScissorRectangle(120.0 - (double)num2, 0.0, 100.0, FrameworkTypes.SCREEN_HEIGHT);
				postDraw();
				OpenGL.glDisable(4);
			}
		}
	}

	private const float PACK_ANIMATION_DELAY = 0.5f;

	private const int BUTTON_LEVEL_1 = 1000;

	public ScrollableContainer helpContainer;

	public ScrollableContainer aboutContainer;

	public ScrollableContainer packContainer;

	public BaseElement[] boxes = new BaseElement[12];

	private Image light;

	public StarsBreak breakParticles;

	public Button unlockb;

	public bool unlockbHidden;

	public bool introMovieOnStartup;

	public Image handAnimation;

	public Image glowAnimation;

	public static Popup ep;

	public bool showNextPackStatus;

	public bool aboutAutoScroll;

	public bool replayingIntroMovie;

	public bool needRecreate;

	public bool needUnlock;

	public int statusBackupRestore;

	public int pack;

	public int level;

	public ViewID viewToShow;

	public int animationStartPackIndex;

	public int currentPackIndex;

	public bool unlockAnimation;

	public DelayedDispatcher ddMainMenu;

	public DelayedDispatcher ddPackSelect;

	public Button promob;

	public bool promobHidden;

	private Text totalFound;

	public BaseElement bulletContainer;

	public HLiftScrollbar liftScrollbar;

	private bool FLAG_RESTORING;

	public static bool showBuyFull = false;

	private static bool openDrawings = false;

	private static bool FirstTime = true;

	public void selector_gotoNextBox(NSObject param)
	{
		gotoNextBox();
	}

	public void selector_playHandAnimation(NSObject param)
	{
	}

	private void setElementPositionWithRelativeQuadOffset2(BaseElement e, int textureID, int quadToCountFrom, int textureID2, int quad)
	{
		Vector quadOffset = Image.getQuadOffset(textureID, quadToCountFrom);
		Vector quadOffset2 = Image.getQuadOffset(textureID2, quad);
		Vector vector = MathHelper.vectSub(quadOffset2, quadOffset);
		e.x = vector.x;
		e.y = vector.y;
	}

	public static Button createButtonWithTextIDDelegate(NSString str, int bid, ButtonDelegate d)
	{
		Image image = Image.Image_createWithResIDQuad(4, 0);
		Image image2 = Image.Image_createWithResIDQuad(4, 1);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		image.addChild(text);
		image2.addChild(text2);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 15.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static Button createBigButtonWithTextIDDelegate(NSString str, int bid, ButtonDelegate d)
	{
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str);
		Image image = Image.Image_createWithResIDQuad(75, 6);
		float num = (float)image.width * 0.9f / (float)text.width;
		if (num > 1f)
		{
			num = 1f;
		}
		Image image2 = Image.Image_createWithResIDQuad(75, 7);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		text.scaleX = (text.scaleY = (text2.scaleX = (text2.scaleY = num)));
		image.addChild(text);
		image2.addChild(text2);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 15.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static Button createButtonWithTextIDDelegateAutoScale(NSString str, int bid, ButtonDelegate d)
	{
		Image image = Image.Image_createWithResIDQuad(4, 0);
		Image image2 = Image.Image_createWithResIDQuad(4, 1);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		float num = (float)image.width * 0.9f / (float)text.width;
		if (num > 1f)
		{
			num = 1f;
		}
		text.scaleX = (text.scaleY = (text2.scaleX = (text2.scaleY = num)));
		image.addChild(text);
		image2.addChild(text2);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 15.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static TimedButton createTimedButtonWithTextIDDelegateTimer(NSString str, int bid, ButtonDelegate d, float time)
	{
		Image image = Image.Image_createWithResIDQuad(4, 0);
		Image image2 = Image.Image_createWithResIDQuad(4, 1);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		image.addChild(text);
		image2.addChild(text2);
		TimedButton timedButton = new TimedButton().initWithUpElementDownElementandID(image, image2, bid);
		timedButton.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 15.0);
		timedButton.timer = time;
		timedButton.delegateButtonDelegate = d;
		return timedButton;
	}

	public static Button createPromoBanner(ButtonDelegate d)
	{
		Button button = createButton2WithImageQuad1Quad2IDDelegate(94, 0, 0, 36, d);
		button.parentAnchor = (button.anchor = 36);
		float sCREEN_OFFSET_Y = FrameworkTypes.SCREEN_OFFSET_Y;
		float num = (button.y = sCREEN_OFFSET_Y + (float)button.height);
		button.x -= FrameworkTypes.SCREEN_OFFSET_X;
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makePos(button.x, num, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(button.x, sCREEN_OFFSET_Y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
		button.addTimeline(timeline);
		Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline2.addKeyFrame(KeyFrame.makePos(button.x, sCREEN_OFFSET_Y, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0.0));
		timeline2.addKeyFrame(KeyFrame.makePos(button.x, num, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.3));
		button.addTimeline(timeline2);
		return button;
	}

	public static Button createButtonWithTextscaleTextIDDelegate(NSString str, float scale, int bid, ButtonDelegate d, int img, int idle, int pressed)
	{
		Image image = Image.Image_createWithResIDQuad(img, idle);
		Image image2 = Image.Image_createWithResIDQuad(img, pressed);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		text.scaleX = (text.scaleY = (text2.scaleX = (text2.scaleY = scale)));
		image.addChild(text);
		image2.addChild(text2);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 15.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static Button createShortButtonWithTextIDDelegate(NSString str, int bid, ButtonDelegate d)
	{
		Image image = Image.Image_createWithResIDQuad(9, 0);
		Image image2 = Image.Image_createWithResIDQuad(9, 1);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		image.addChild(text);
		image2.addChild(text2);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 15.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static ToggleButton createToggleButtonWithText1Text2IDDelegate(NSString str1, NSString str2, int bid, ButtonDelegate d)
	{
		Image image = Image.Image_createWithResIDQuad(4, 0);
		Image image2 = Image.Image_createWithResIDQuad(4, 1);
		Image image3 = Image.Image_createWithResIDQuad(4, 0);
		Image image4 = Image.Image_createWithResIDQuad(4, 1);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(str1);
		Text text2 = new Text().initWithFont(font);
		text2.setString(str1);
		Text text3 = new Text().initWithFont(font);
		text3.setString(str2);
		Text text4 = new Text().initWithFont(font);
		text4.setString(str2);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		text3.anchor = (text3.parentAnchor = 18);
		text4.anchor = (text4.parentAnchor = 18);
		image.addChild(text);
		image2.addChild(text2);
		image3.addChild(text3);
		image4.addChild(text4);
		ToggleButton toggleButton = new ToggleButton().initWithUpElement1DownElement1UpElement2DownElement2andID(image, image2, image3, image4, bid);
		toggleButton.setTouchIncreaseLeftRightTopBottom(10.0, 10.0, 10.0, 10.0);
		toggleButton.delegateButtonDelegate = d;
		return toggleButton;
	}

	public static Button createBackButtonWithDelegateID(ButtonDelegate d, int bid)
	{
		Button button = createButtonWithImageQuad1Quad2IDDelegate(75, 0, 1, bid, d);
		button.anchor = (button.parentAnchor = 33);
		button.y += FrameworkTypes.SCREEN_OFFSET_Y;
		button.x -= FrameworkTypes.SCREEN_OFFSET_X;
		return button;
	}

	public static Button createButtonWithImageIDDelegate(int resID, int bid, ButtonDelegate d)
	{
		Texture2D texture = Application.getTexture(resID);
		Image up = Image.Image_create(texture);
		Image image = Image.Image_create(texture);
		image.scaleX = 1.2f;
		image.scaleY = 1.2f;
		Button button = new Button().initWithUpElementDownElementandID(up, image, bid);
		button.setTouchIncreaseLeftRightTopBottom(10.0, 10.0, 10.0, 10.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static Button createButtonWithImageQuadIDDelegate(int resID, int quad, int bid, ButtonDelegate d)
	{
		Image up = Image.Image_createWithResIDQuad(resID, quad);
		Image image = Image.Image_createWithResIDQuad(resID, quad);
		image.scaleX = 1.2f;
		image.scaleY = 1.2f;
		Button button = new Button().initWithUpElementDownElementandID(up, image, bid);
		button.setTouchIncreaseLeftRightTopBottom(10.0, 10.0, 10.0, 10.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static Button createButton2WithImageQuad1Quad2IDDelegate(int res, int q1, int q2, int bid, ButtonDelegate d)
	{
		Image up = Image.Image_createWithResIDQuad(res, q1);
		Image down = Image.Image_createWithResIDQuad(res, q2);
		Button button = new Button().initWithUpElementDownElementandID(up, down, bid);
		button.delegateButtonDelegate = d;
		Application.getTexture(res);
		return button;
	}

	public static Button createMenuButtonWithImgQuadTextDelegateID(int q, NSString str, ButtonDelegate d, int bid)
	{
		BaseElement up = createMenuElementQuadText(q, str);
		BaseElement baseElement = createMenuElementQuadText(q, str);
		baseElement.scaleX = (baseElement.scaleY = 1.2f);
		Button button = new Button().initWithUpElementDownElementandID(up, baseElement, bid);
		button.delegateButtonDelegate = d;
		return button;
	}

	public static BaseElement createMenuElementQuadText(int q, NSString str)
	{
		Image image = Image.Image_createWithResIDQuad(74, 14);
		Image image2 = Image.Image_createWithResIDQuad(74, q);
		image2.parentAnchor = 9;
		Image.setElementPositionWithRelativeQuadOffset(image2, 74, 14, q);
		image.addChild(image2);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.scaleX = (text.scaleY = 0.7f);
		text.setAlignment(2);
		text.setStringandWidth(str, image.width);
		text.parentAnchor = (text.anchor = 18);
		image.addChild(text);
		return image;
	}

	public static Button createButtonWithImageQuad1Quad2IDDelegate(int res, int q1, int q2, int bid, ButtonDelegate d)
	{
		Image image = Image.Image_createWithResIDQuad(res, q1);
		Image image2 = Image.Image_createWithResIDQuad(res, q2);
		image.doRestoreCutTransparency();
		image2.doRestoreCutTransparency();
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.delegateButtonDelegate = d;
		Texture2D texture = Application.getTexture(res);
		button.forceTouchRect(FrameworkTypes.MakeRectangle(texture.quadOffsets[q1].x, texture.quadOffsets[q1].y, texture.quadRects[q1].w, texture.quadRects[q1].h));
		return button;
	}

	public static Button createStarkeyButtonWithDelegateID(int bid, ButtonDelegate d)
	{
		Image image = Image.Image_createWithResIDQuad(75, 4);
		Image image2 = Image.Image_createWithResIDQuad(75, 5);
		FontGeneric font = Application.getFont(5);
		NSString @string = Application.getString(1179682);
		Text text = new Text().initWithFont(font);
		text.setString(@string);
		text.anchor = (text.parentAnchor = 20);
		text.x = -65f;
		image.addChild(text);
		Text text2 = new Text().initWithFont(font);
		text2.setString(@string);
		text2.anchor = (text2.parentAnchor = 20);
		text2.x = text.x;
		image2.addChild(text2);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.parentAnchor = (button.anchor = 36);
		button.delegateButtonDelegate = d;
		float num = FrameworkTypes.SCREEN_OFFSET_Y;
		if (CTRPreferences.isBannersMustBeShown())
		{
			num -= 50f;
		}
		float num2 = (button.y = FrameworkTypes.SCREEN_OFFSET_Y + (float)button.height);
		button.x -= FrameworkTypes.SCREEN_OFFSET_X;
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline.addKeyFrame(KeyFrame.makePos(button.x, num2, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(button.x, num2, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
		timeline.addKeyFrame(KeyFrame.makePos(button.x, num, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
		button.addTimeline(timeline);
		Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline2.addKeyFrame(KeyFrame.makePos(button.x, num, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0.0));
		timeline2.addKeyFrame(KeyFrame.makePos(button.x, num2, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.3));
		button.addTimeline(timeline2);
		return button;
	}

	private Image createBackgroundWithLogo(bool l)
	{
		int r = (l ? 68 : 67);
		int q = (l ? 0 : 0);
		Image image = Image.Image_createWithResIDQuad(r, q);
		image.anchor = (image.parentAnchor = 18);
		image.passTransformationsToChilds = false;
		image.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
		image.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		if (l)
		{
			Application.sharedPreferences();
			int num = Preferences._getIntForKey("PREFS_SELECTED_CANDY");
			Image image2 = Image.Image_createWithResIDQuad(72, num);
			image2.setName(NSObject.NSS("logoCandy"));
			image2.anchor = (image2.parentAnchor = 10);
			image2.scaleX = (image2.scaleY = 1.05f);
			image2.x -= image2.width / 12;
			image.addChild(image2);
			for (int i = 0; i < 11; i++)
			{
				CTRGameObject cTRGameObject = CTRGameObject.CTRGameObject_createWithResIDQuad(72, 1 + i);
				cTRGameObject.anchor = (cTRGameObject.parentAnchor = 9);
				Image.setElementPositionWithRelativeQuadOffset(cTRGameObject, 72, num, 1 + i);
				image2.addChild(cTRGameObject);
				cTRGameObject.scaleX = (cTRGameObject.scaleY = 0.9f);
				NSString value = NSObject.NSS(MathHelper.RND_RANGE(7, 14).ToString());
				NSString value2 = ((i % 2 != 0) ? NSObject.NSS("RC" + MathHelper.RND_RANGE(1, 3)) : NSObject.NSS("RW" + MathHelper.RND_RANGE(1, 3)));
				XMLNode xMLNode = new XMLNode();
				xMLNode["x"] = NSObject.NSS(cTRGameObject.x.ToString());
				xMLNode["y"] = NSObject.NSS(cTRGameObject.y.ToString());
				xMLNode["path"] = value2;
				xMLNode["moveSpeed"] = value;
				cTRGameObject.parseMover(xMLNode);
			}
			Image image3 = Image.Image_createWithResIDQuad(72, 12);
			image3.anchor = (image3.parentAnchor = 9);
			Image.setElementPositionWithRelativeQuadOffset(image3, 72, num, 12);
			image2.addChild(image3);
		}
		Image image4 = Image.Image_createWithResIDQuad(88, 0);
		image4.scaleX = (image4.scaleY = 2f);
		image4.anchor = (image4.parentAnchor = 18);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline.addKeyFrame(KeyFrame.makeRotation(45.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeRotation(405.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 75.0));
		timeline.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
		image4.addTimeline(timeline);
		image4.playTimeline(0);
		image.addChild(image4);
		return image;
	}

	private void createMainMenu()
	{
		new Texture2D().initWithPath("ctre/ctre_live_tile_0", assets: true);
		new Texture2D().initWithPath("ctre/ctre_live_tile_star", assets: true);
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		Image image = createBackgroundWithLogo(l: true);
		int num = -3;
		VBox vBox = new VBox().initWithOffsetAlignWidth(num, 2, FrameworkTypes.SCREEN_WIDTH);
		vBox.anchor = (vBox.parentAnchor = 34);
		vBox.y = -36f;
		Button button = createButtonWithTextIDDelegate(Application.getString(1179648), 0, this);
		vBox.addChild(button);
		if (!CTRPreferences.isLiteVersion())
		{
			Button button2 = createButtonWithTextIDDelegate(Application.getString(1179707), 33, this);
			vBox.addChild(button2);
			button2.setTouchIncreaseLeftRightTopBottom(0f, 0f, num, num);
		}
		Button button3 = createButtonWithTextIDDelegate(Application.getString(1179649), 1, this);
		vBox.addChild(button3);
		button.setTouchIncreaseLeftRightTopBottom(0f, 0f, num, num);
		button3.setTouchIncreaseLeftRightTopBottom(0f, 0f, num, num);
		if (CTRPreferences.isLiteVersion())
		{
			Button button4 = createButtonWithTextIDDelegateAutoScale(Application.getString(1179651), 45, this);
			vBox.addChild(button4);
			button4.setTouchIncreaseLeftRightTopBottom(0f, 0f, num, num);
		}
		image.addChild(vBox);
		if (ddMainMenu != null)
		{
			ddMainMenu.dealloc();
		}
		ddMainMenu = (DelayedDispatcher)new DelayedDispatcher().init();
		menuView.addChild(image);
		addViewwithID(menuView, ViewID.VIEW_MAIN_MENU);
	}

	private void createOptions()
	{
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		Image image = createBackgroundWithLogo(l: false);
		VBox vBox = new VBox().initWithOffsetAlignWidth(8.0, 2, FrameworkTypes.SCREEN_WIDTH);
		vBox.anchor = (vBox.parentAnchor = 18);
		vBox.y += 0.33f;
		ToggleButton toggleButton = createAudioButtonWithQuadDelegateID(4, this, 9);
		ToggleButton toggleButton2 = createAudioButtonWithQuadDelegateID(3, this, 6);
		ToggleButton toggleButton3 = createAudioButtonWithQuadDelegateID(2, this, 7);
		HBox hBox = new HBox().initWithOffsetAlignHeight(-15f, 16, toggleButton.height);
		hBox.addChild(toggleButton2);
		hBox.addChild(toggleButton);
		hBox.addChild(toggleButton3);
		vBox.addChild(hBox);
		Button c = createButtonWithTextIDDelegateAutoScale(Application.getString(1179731), 44, this);
		Button c2 = createButtonWithTextIDDelegateAutoScale(Application.getString(1179730), 42, this);
		vBox.addChild(c);
		vBox.addChild(c2);
		if (CTRPreferences.isLiteVersion())
		{
			Button c3 = createButtonWithTextIDDelegate(Application.getString(1179707), 33, this);
			vBox.addChild(c3);
		}
		bool flag = Preferences._getBooleanForKey("SOUND_ON");
		bool flag2 = Preferences._getBooleanForKey("MUSIC_ON");
		bool flag3 = Preferences._getBooleanForKey("VOICE_ON");
		if (!flag)
		{
			toggleButton2.toggle();
		}
		if (!flag2)
		{
			toggleButton.toggle();
		}
		if (!flag3)
		{
			toggleButton3.toggle();
		}
		Button c4 = createButtonWithTextIDDelegate(Application.getString(1179652), 11, this);
		vBox.addChild(c4);
		Button c5 = createButtonWithTextIDDelegate(Application.getString(1179653), 10, this);
		vBox.addChild(c5);
		float scale = 1f;
		switch (ResDataPhoneFullExperiments.LANGUAGE)
		{
		case Language.LANG_JA:
			scale = 0.8f;
			break;
		case Language.LANG_ES:
			scale = 0.8f;
			break;
		case Language.LANG_BR:
			scale = 0.8f;
			break;
		case Language.LANG_KO:
			scale = 0.8f;
			break;
		}
		Button c6 = createButtonWithTextscaleTextIDDelegate(Application.getString(1179738), scale, 46, this, 4, 0, 1);
		vBox.addChild(c6);
		Button button = createBackButtonWithDelegateID(this, 13);
		image.addChild(button);
		if (CTRPreferences.isBannersMustBeShown())
		{
			vBox.y -= 30f;
			button.y -= 50f;
		}
		image.addChild(vBox);
		menuView.addChild(image);
		addViewwithID(menuView, ViewID.VIEW_OPTIONS);
	}

	private void createReset()
	{
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		Image image = createBackgroundWithLogo(l: false);
		VBox vBox = new VBox().initWithOffsetAlignWidth(10.0, 2, (double)FrameworkTypes.SCREEN_WIDTH - 40.0);
		vBox.parentAnchor = (vBox.anchor = 18);
		Text text = new Text().initWithFont(Application.getFont(5));
		text.setAlignment(2);
		text.setStringandWidth(Application.getString(1179662), vBox.width);
		vBox.addChild(text);
		Text text2 = new Text().initWithFont(Application.getFont(6));
		text2.setAlignment(2);
		text2.setStringandWidth(Application.getString(1179698), vBox.width);
		vBox.addChild(text2);
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH)
		{
			text.x += 3f;
			text2.x += 4f;
		}
		image.addChild(vBox);
		TimedButton timedButton = createTimedButtonWithTextIDDelegateTimer(Application.getString(1179674), 16, this, 3f);
		timedButton.anchor = (timedButton.parentAnchor = 34);
		timedButton.y = -130f;
		Button button = createButtonWithTextIDDelegate(Application.getString(1179675), 17, this);
		button.anchor = (button.parentAnchor = 34);
		button.y = -70f;
		vBox.y = -65f;
		image.addChild(timedButton);
		image.addChild(button);
		Button button2 = createBackButtonWithDelegateID(this, 14);
		image.addChild(button2);
		if (CTRPreferences.isBannersMustBeShown())
		{
			vBox.y -= 30f;
			timedButton.y -= 30f;
			button.y -= 30f;
			button2.y -= 50f;
		}
		menuView.addChild(image);
		addViewwithID(menuView, ViewID.VIEW_RESET);
	}

	private void createMovieView()
	{
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		RectangleElement rectangleElement = (RectangleElement)new RectangleElement().init();
		rectangleElement.width = (int)FrameworkTypes.SCREEN_WIDTH;
		rectangleElement.height = (int)FrameworkTypes.SCREEN_HEIGHT;
		rectangleElement.color = RGBAColor.blackRGBA;
		menuView.addChild(rectangleElement);
		addViewwithID(menuView, ViewID.VIEW_MOVIE);
	}

	private void createAbout()
	{
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		Image image = createBackgroundWithLogo(l: false);
		NSObject.NSS("undefined version");
		if (CTRPreferences.isLiteVersion())
		{
			NSObject.NSS(" Lite");
		}
		else
		{
			NSObject.NSS("");
		}
		VBox vBox = new VBox().initWithOffsetAlignWidth(0f, 2, 310f);
		Image c = Image.Image_createWithResIDQuad(72, 13);
		vBox.addChild(c);
		string text = Application.getString(1179722).ToString();
		string[] separator = new string[1] { "%@" };
		string[] array = text.Split(separator, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (i == 0)
			{
				text = "";
			}
			if (i == 2)
			{
				string fullName = Assembly.GetExecutingAssembly().FullName;
				text += fullName.Split('=')[1].Split(',')[0];
				text += " ";
			}
			text += array[i];
		}
		Text text2 = new Text().initWithFont(Application.getFont(6));
		text2.setAlignment(2);
		text2.setStringandWidth(NSObject.NSS(text), 310f);
		vBox.addChild(text2);
		aboutContainer = new ScrollableContainer().initWithWidthHeightContainer(310f, 350f, vBox);
		aboutContainer.anchor = (aboutContainer.parentAnchor = 18);
		image.addChild(aboutContainer);
		Button c2 = createBackButtonWithDelegateID(this, 14);
		image.addChild(c2);
		menuView.addChild(image);
		addViewwithID(menuView, ViewID.VIEW_ABOUT);
	}

	private HBox createTextWithStar(string t)
	{
		HBox hBox = new HBox().initWithOffsetAlignHeight(0.0, 16, 50.0);
		Text text = new Text().initWithFont(Application.getFont(5));
		text.setString(NSObject.NSS(t));
		hBox.addChild(text);
		Image c = Image.Image_createWithResIDQuad(74, 12);
		hBox.addChild(c);
		return hBox;
	}

	private BaseElement createPackElementforContainer(int n, ScrollableContainer c)
	{
		bool flag = CTRPreferences.getLastPack() == n && n != CTRPreferences.getPacksCount();
		TouchBaseElement touchBaseElement = (TouchBaseElement)new TouchBaseElement().init();
		touchBaseElement.delegateButtonDelegate = this;
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		baseElement.setName(NSObject.NSS("boxContainer"));
		baseElement.anchor = (baseElement.parentAnchor = 9);
		touchBaseElement.addChild(baseElement);
		int totalStars = CTRPreferences.getTotalStars();
		if (n > 0 && n < CTRPreferences.getPacksCount() && CTRPreferences.getUnlockedForPackLevel(n, 0) == UNLOCKED_STATE.UNLOCKED_STATE_LOCKED && totalStars >= CTRPreferences.packUnlockStars(n))
		{
			CTRPreferences.setUnlockedForPackLevel(UNLOCKED_STATE.UNLOCKED_STATE_JUST_UNLOCKED, n, 0);
		}
		int q = 2;
		int num = 3;
		NSString @string = Application.getString(1179713 + n);
		int unlockedForPackLevel = (int)CTRPreferences.getUnlockedForPackLevel(n, 0);
		bool flag2 = n != CTRPreferences.getPacksCount() && n >= CTRPreferences.sharewareFreePacks() && CTRPreferences.isLiteVersion();
		int num2 = 2 * (flag2 ? 1 : 0);
		if (num2 == 0)
		{
			num2 = ((unlockedForPackLevel == 0 && n != CTRPreferences.getPacksCount()) ? 1 : 0);
		}
		touchBaseElement.bid = ((num2 > 0) ? (-1) : (18 + n));
		Image image = Image.Image_createWithResIDQuad(74, q);
		image.doRestoreCutTransparency();
		image.anchor = (image.parentAnchor = 9);
		image.setName(NSObject.NSS("box"));
		if (flag)
		{
			image.color = RGBAColor.transparentRGBA;
		}
		baseElement.addChild(image);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5f));
		image.addTimeline(timeline);
		Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5f));
		image.addTimeline(timeline2);
		Image image2 = Image.Image_createWithResIDQuad(74, num);
		image2.texture.PixelCorrectionQuadY(num);
		image2.doRestoreCutTransparency();
		image2.anchor = (image2.parentAnchor = 9);
		image2.setName(NSObject.NSS("boxSelected"));
		if (!flag)
		{
			image2.color = RGBAColor.transparentRGBA;
		}
		baseElement.addChild(image2);
		Timeline timeline3 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5f));
		image2.addTimeline(timeline3);
		Timeline timeline4 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5f));
		image2.addTimeline(timeline4);
		if (n != CTRPreferences.getPacksCount())
		{
			Image image3 = Image.Image_createWithResIDQuad(74, 6 + n);
			image3.doRestoreCutTransparency();
			image3.anchor = (image3.parentAnchor = 9);
			if (flag)
			{
				image3.color = new RGBAColor(172.0 / 255.0, 1.0 / 3.0, 13.0 / 255.0, 1.0);
			}
			else
			{
				image3.color = new RGBAColor(18.0 / 85.0, 20.0 / 51.0, 169.0 / 255.0, 1.0);
			}
			image3.setName(NSObject.NSS("boxPic"));
			image3.HasColor = true;
			baseElement.addChild(image3);
		}
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setAlignment(2);
		text.anchor = 10;
		text.parentAnchor = 9;
		text.setStringandWidth(@string, 210.0);
		text.x = 180f;
		if (n != CTRPreferences.getPacksCount())
		{
			text.y = 65f;
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA && n == CTRPreferences.getPacksCount() - 1)
			{
				text.scaleX = (text.scaleY = 0.8f);
				text.x += 5f;
			}
		}
		else
		{
			text.y = 110f;
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				text.x += 10f;
			}
		}
		baseElement.addChild(text);
		if (num2 > 0)
		{
			int num3 = CTRPreferences.packUnlockStars(n);
			Image image4 = Image.Image_createWithResIDQuad(74, 13);
			image4.doRestoreCutTransparency();
			image4.anchor = (image4.parentAnchor = 9);
			baseElement.addChild(image4);
			HBox hBox = createTextWithStar(num3.ToString());
			hBox.anchor = (hBox.parentAnchor = 18);
			hBox.y = 50f;
			hBox.x = 9f;
			image4.addChild(hBox);
			switch (num2)
			{
			case 2:
			{
				touchBaseElement.bid = 45;
				Button button = createButtonWithTextIDDelegateAutoScale(Application.getString(1179651), 45, this);
				button.anchor = (button.parentAnchor = 18);
				button.y = 170f;
				button.x = 40f;
				touchBaseElement.addChild(button);
				button.setTouchIncreaseLeftRightTopBottom(0f, 0f, -10f, 0f);
				hBox.visible = false;
				break;
			}
			case 1:
			{
				Text text2 = new Text().initWithFont(Application.getFont(6));
				NSString newString = NSObject.NSS(string.Format(Application.getString(1179723).ToString().Replace("%d", "{0}"), num3));
				text2.setAlignment(2);
				text2.anchor = (text2.parentAnchor = 18);
				if (ResDataPhoneFullExperiments.LANGUAGE != Language.LANG_DE)
				{
					text2.setStringandWidth(newString, 240.0);
				}
				else
				{
					text2.setStringandWidth(newString, 235.0);
				}
				text2.y = 168f;
				text2.x = 30f;
				touchBaseElement.addChild(text2);
				break;
			}
			}
		}
		else if (unlockedForPackLevel == 2 || unlockedForPackLevel == 3)
		{
			Image image5 = Image.Image_createWithResIDQuad(74, 13);
			image5.setName(NSObject.NSS("lockHideMe"));
			image5.doRestoreCutTransparency();
			image5.anchor = (image5.parentAnchor = 9);
			baseElement.addChild(image5);
			Timeline timeline5 = new Timeline().initWithMaxKeyFramesOnTrack(3);
			timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.5));
			timeline5.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline5.addKeyFrame(KeyFrame.makeScale(2.0, 2.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.5));
			image5.addTimeline(timeline5);
			baseElement.addChild(image5);
		}
		Timeline timeline6 = new Timeline().initWithMaxKeyFramesOnTrack(4);
		timeline6.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline6.addKeyFrame(KeyFrame.makeScale(0.95, 1.05, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.15));
		timeline6.addKeyFrame(KeyFrame.makeScale(1.05, 0.95, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.2));
		timeline6.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.25));
		baseElement.addTimeline(timeline6);
		baseElement.height = (touchBaseElement.height = 300);
		baseElement.width = (touchBaseElement.width = 300);
		if (CTRPreferences.isPackPerfect(n))
		{
			Image image6 = Image.Image_createWithResIDQuad(74, 17);
			image6.doRestoreCutTransparency();
			image6.parentAnchor = (image6.anchor = 9);
			baseElement.addChild(image6);
		}
		return touchBaseElement;
	}

	private void createPackSelect()
	{
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		Image image = Image.Image_createWithResIDQuad(67, 1);
		image.anchor = (image.parentAnchor = 18);
		image.passTransformationsToChilds = false;
		image.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
		image.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		image.texture.PixelCorrectionQuadY(1);
		string text = Application.getString(1179680).ToString();
		text = text.Replace("%d", "");
		HBox hBox = createTextWithStar(text + CTRPreferences.getTotalStars());
		hBox.x = -10f;
		hBox.x += FrameworkTypes.SCREEN_OFFSET_X;
		hBox.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		HBox hBox2 = new HBox().initWithOffsetAlignHeight(-50f, 16, FrameworkTypes.SCREEN_HEIGHT);
		packContainer = new ScrollableContainer().initWithWidthHeightContainer(FrameworkTypes.SCREEN_WIDTH_EXPANDED, FrameworkTypes.SCREEN_HEIGHT, hBox2);
		packContainer.minAutoScrollToSpointLength = 5f;
		packContainer.shouldBounceHorizontally = true;
		packContainer.resetScrollOnShow = false;
		packContainer.touchMoveIgnoreLength = 15f;
		packContainer.x = 0f - FrameworkTypes.SCREEN_OFFSET_X;
		packContainer.y = -25f;
		packContainer.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		packContainer.untouchChildsOnMove = true;
		bool flag = true;
		bool flag2 = /*!Application.sharedPreferences().remoteDataManager.getHideSocialNetworks()*/false;
		bool flag3 = false;
		packContainer.turnScrollPointsOnWithCapacity(CTRPreferences.getPacksCount() + ((!flag3) ? 1 : 2));
		packContainer.delegateScrollableContainerProtocol = this;
		hBox.anchor = (hBox.parentAnchor = 12);
		image.addChild(hBox);
		float num = 0f;
		if (flag3)
		{
			BaseElement baseElement = null;
			BaseElement baseElement2 = null;
			float num2 = 25f;
			float num3 = 0f;
			if (flag)
			{
				int strResID = 1179702;
				baseElement = createMenuButtonWithImgQuadTextDelegateID(15, Application.getString(strResID), this, 39);
				baseElement.parentAnchor = (baseElement.anchor = 18);
				baseElement.parentAnchor = (baseElement.anchor = 18);
				num3 = baseElement.width;
			}
			if (flag2)
			{
				baseElement2 = createMenuButtonWithImgQuadTextDelegateID(16, Application.getString(1179725), this, 27);
				baseElement2.parentAnchor = (baseElement2.anchor = 18);
				num3 = baseElement2.width;
			}
			VBox vBox = new VBox().initWithOffsetAlignWidth(30f, 2, num3 + (FrameworkTypes.SCREEN_WIDTH_EXPANDED - num3) / 2f + num2);
			vBox.parentAnchor = (vBox.anchor = 20);
			vBox.x -= num2;
			if (flag)
			{
				vBox.addChild(baseElement);
			}
			if (flag2)
			{
				vBox.addChild(baseElement2);
			}
			hBox2.addChild(vBox);
			num = (float)vBox.width + -50f;
		}
		int num4 = CTRPreferences.getPacksCount();
		if (!CTRPreferences.isLiteVersion() && CTRPreferences.isSharewareUnlocked())
		{
			num4++;
		}
		for (int i = 0; i < num4; i++)
		{
			TouchBaseElement touchBaseElement = (TouchBaseElement)createPackElementforContainer(i, packContainer);
			boxes[i] = touchBaseElement;
			hBox2.addChild(touchBaseElement);
			touchBaseElement.x -= 25f - FrameworkTypes.SCREEN_OFFSET_X;
			touchBaseElement.y -= 35f;
			packContainer.addScrollPointAtXY(num, 0.0);
			touchBaseElement.bbc = FrameworkTypes.MakeRectangle(70.0, 0.0, -70.0, 0.0);
			num += (float)touchBaseElement.width + -50f;
		}
		if (flag3)
		{
			packContainer.addScrollPointAtXY(0.0, 0.0);
		}
		hBox2.width += 20;
		int num5 = (flag3 ? 1 : 0);
		if (CTRPreferences.isLiteVersion())
		{
			num5--;
		}
		BulletScrollbarQuad bulletScrollbarQuad = (BulletScrollbarQuad)new BulletScrollbarQuad().initWithBulletTextureactiveQuadinactiveQuadandTotalBullets(74, 5, 4, CTRPreferences.getPacksCount() + num5);
		bulletScrollbarQuad.delegateProvider = packContainer.provideScrollPosMaxScrollPosScrollCoeff;
		bulletScrollbarQuad.parentAnchor = (bulletScrollbarQuad.anchor = 34);
		bulletScrollbarQuad.y = -80f;
		bulletScrollbarQuad.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		image.addChild(bulletScrollbarQuad);
		light = Image.Image_createWithResIDQuad(74, 1);
		Image.setElementPositionWithQuadOffset(light, 67, 2);
		Image image2 = Image.Image_createWithResIDQuad(74, 0);
		Image.setElementPositionWithQuadOffset(image2, 67, 2);
		image.addChild(image2);
		image.addChild(light);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5f));
		light.addTimeline(timeline);
		Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5f));
		light.addTimeline(timeline2);
		image.addChild(packContainer);
		Button button = createBackButtonWithDelegateID(this, 13);
		if (CTRPreferences.isBannersMustBeShown())
		{
			button.y -= 50f;
		}
		image.addChild(button);
		promob = createPromoBanner(this);
		image.addChild(promob);
		promobHidden = true;
		menuView.addChild(image);
		addViewwithID(menuView, ViewID.VIEW_PACK_SELECT);
		if (ddPackSelect != null)
		{
			ddPackSelect = null;
		}
		ddPackSelect = (DelayedDispatcher)new DelayedDispatcher().init();
		int lastPack = CTRPreferences.getLastPack();
		packContainer.placeToScrollPoint(lastPack);
		if (lastPack > CTRPreferences.getPacksCount() - 1)
		{
			light.color = RGBAColor.transparentRGBA;
		}
	}

	public virtual void scrollableContainerreachedScrollPoint(ScrollableContainer e, int i)
	{
		currentPackIndex = i;
		if (i > CTRPreferences.getPacksCount())
		{
			return;
		}
		CTRPreferences.setLastPack(i);
		if (i != CTRPreferences.getPacksCount())
		{
			boxes[i].getChildWithName(NSObject.NSS("boxContainer"))?.playTimeline(0);
			int unlockedForPackLevel = (int)CTRPreferences.getUnlockedForPackLevel(i, 0);
			Image image = (Image)boxes[i].getChildWithName(NSObject.NSS("lockHideMe"));
			if (image != null && (unlockedForPackLevel == 2 || unlockedForPackLevel == 3))
			{
				CTRPreferences.setUnlockedForPackLevel(UNLOCKED_STATE.UNLOCKED_STATE_UNLOCKED, i, 0);
				image.playTimeline(0);
				if (unlockedForPackLevel == 3)
				{
					breakParticles.stopSystem();
					breakParticles.startSystem(10);
					CTRSoundMgr._playSound(34);
				}
			}
			if (showBuyFull && CTRPreferences.isLiteVersion() && i >= CTRPreferences.sharewareFreePacks())
			{
				if (ep == null)
				{
					showBuyFullPopup();
				}
				showBuyFull = false;
			}
			if (!promobHidden && promob != null)
			{
				promob.playTimeline(1);
				promobHidden = true;
			}
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			int num = cTRRootController.getPack() + 1;
			if (!showNextPackStatus || i != num)
			{
				return;
			}
			showNextPackStatus = false;
			if (unlockedForPackLevel != 0)
			{
				return;
			}
			if (CTRPreferences.isLiteVersion())
			{
				if (num < 2)
				{
					showCantUnlockPopupForPack(num);
				}
			}
			else
			{
				showCantUnlockPopupForPack(num);
			}
		}
		else if (promobHidden && promob != null && !CTRPreferences.isBannersMustBeShown())
		{
			promob.playTimeline(0);
			promobHidden = false;
		}
	}

	public virtual void scrollableContainerchangedTargetScrollPoint(ScrollableContainer e, int i)
	{
		CTRPreferences.setLastPack(i);
	}

	private BaseElement createButtonForLevelPack(int l, int p)
	{
		bool flag = l >= CTRPreferences.getLevelsInPackCount() && CTRPreferences.isLiteVersion();
		int num = 2 * (flag ? 1 : 0);
		if (num == 0)
		{
			num = ((CTRPreferences.getUnlockedForPackLevel(p, l) == UNLOCKED_STATE.UNLOCKED_STATE_LOCKED) ? 1 : 0);
		}
		int starsForPackLevel = CTRPreferences.getStarsForPackLevel(p, l);
		TouchBaseElement touchBaseElement = (TouchBaseElement)new TouchBaseElement().init();
		touchBaseElement.bbc = FrameworkTypes.MakeRectangle(5.0, 0.0, -10.0, 0.0);
		touchBaseElement.delegateButtonDelegate = this;
		Image image = null;
		switch (num)
		{
		case 2:
			touchBaseElement.bid = 45;
			image = Image.Image_createWithResIDQuad(73, 1);
			image.doRestoreCutTransparency();
			break;
		case 1:
			touchBaseElement.bid = -1;
			image = Image.Image_createWithResIDQuad(73, 0);
			image.doRestoreCutTransparency();
			break;
		default:
		{
			touchBaseElement.bid = 1000 + l;
			image = Image.Image_createWithResIDQuad(73, 2);
			image.doRestoreCutTransparency();
			Text text = new Text().initWithFont(Application.getFont(5));
			NSString @string = NSObject.NSS((l + 1).ToString());
			text.setString(@string);
			text.anchor = (text.parentAnchor = 18);
			text.y -= 5f;
			image.addChild(text);
			Image image2 = Image.Image_createWithResIDQuad(73, 3 + starsForPackLevel);
			image2.doRestoreCutTransparency();
			image2.anchor = (image2.parentAnchor = 9);
			image.addChild(image2);
			break;
		}
		}
		image.anchor = (image.parentAnchor = 18);
		touchBaseElement.addChild(image);
		touchBaseElement.setSizeToChildsBounds();
		return touchBaseElement;
	}

	private void createLevelSelect()
	{
		float num = 0.3f;
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		int r = 7;
		Image image = Image.Image_createWithResID(252);
		image.passTransformationsToChilds = false;
		image.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
		image.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		image.parentAnchor = (image.anchor = 9);
		image.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		Image image2 = Image.Image_createWithResIDQuad(r, 0);
		image2.parentAnchor = 12;
		image2.anchor = 20;
		image2.y = FrameworkTypes.SCREEN_HEIGHT * FrameworkTypes.SCREEN_BG_SCALE_Y + 10f;
		image2.passTransformationsToChilds = false;
		image2.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		image.addChild(image2);
		menuView.addChild(image);
		Image image3 = Image.Image_createWithResIDQuad(88, 0);
		image3.scaleX = (image3.scaleY = 2.3f);
		image3.setName(NSObject.NSS("shadow"));
		image3.anchor = (image3.parentAnchor = 18);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeScale(2.0, 2.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeScale(5.0, 5.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num));
		timeline.delegateTimelineDelegate = this;
		image3.addTimeline(timeline);
		Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline2.addKeyFrame(KeyFrame.makeRotation(45.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline2.addKeyFrame(KeyFrame.makeRotation(405.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 75.0));
		timeline2.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
		image3.addTimeline(timeline2);
		image3.playTimeline(1);
		menuView.addChild(image3);
		HBox hBox = createTextWithStar(CTRPreferences.getTotalStarsInPack(pack) + "/" + CTRPreferences.getLevelsInPackCount() * 3);
		hBox.x = -10f;
		hBox.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		hBox.x += FrameworkTypes.SCREEN_OFFSET_X;
		float of = 8f;
		float of2 = -9f;
		float h = 63f;
		VBox vBox = new VBox().initWithOffsetAlignWidth(of, 2, FrameworkTypes.SCREEN_WIDTH);
		vBox.setName(NSObject.NSS("levelsBox"));
		vBox.x = 0f;
		vBox.y = ((CTRPreferences.isBannersMustBeShown() && FrameworkTypes.SCREEN_RATIO >= 1.6f) ? 30f : 50f);
		int num2 = -1;
		num2 = 5;
		int num3 = 0;
		for (int i = 0; i < num2; i++)
		{
			HBox hBox2 = new HBox().initWithOffsetAlignHeight(of2, 16, h);
			for (int j = 0; j < num2; j++)
			{
				hBox2.addChild(createButtonForLevelPack(num3++, pack));
			}
			vBox.addChild(hBox2);
		}
		Timeline timeline3 = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num));
		vBox.addTimeline(timeline3);
		hBox.anchor = (hBox.parentAnchor = 12);
		hBox.setName(NSObject.NSS("starText"));
		Timeline timeline4 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num));
		hBox.addTimeline(timeline4);
		menuView.addChild(hBox);
		menuView.addChild(vBox);
		Button button = createBackButtonWithDelegateID(this, 15);
		button.setName(NSObject.NSS("backButton"));
		Timeline timeline5 = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num));
		button.addTimeline(timeline5);
		if (CTRPreferences.isBannersMustBeShown() && FrameworkTypes.SCREEN_RATIO >= 1.6f)
		{
			button.y -= 50f;
		}
		menuView.addChild(button);
		addViewwithID(menuView, ViewID.VIEW_LEVEL_SELECT);
	}

	public override NSObject initWithParent(ViewController p)
	{
		if (base.initWithParent(p) != null)
		{
			needRecreate = false;
			needUnlock = false;
			statusBackupRestore = 0;
			animationStartPackIndex = 0;
			currentPackIndex = 0;
			unlockAnimation = false;
			ddMainMenu = null;
			ddPackSelect = null;
			ep = null;
			createMainMenu();
			createOptions();
			createReset();
			createAbout();
			createMovieView();
			createPackSelect();
			createDrawings();
			createAchievements();
			createLeaderboards();
			MapPickerController c = (MapPickerController)new MapPickerController().initWithParent(this);
			addChildwithID(c, 0);
		}
		return this;
	}

	public override void dealloc()
	{
		ddMainMenu.cancelAllDispatches();
		ddMainMenu.dealloc();
		ddMainMenu = null;
		ddPackSelect.cancelAllDispatches();
		ddPackSelect.dealloc();
		ddPackSelect = null;
		base.dealloc();
	}

	public override void activate()
	{
		showNextPackStatus = false;
		base.activate();
		if (viewToShow == ViewID.VIEW_LEVEL_SELECT)
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			pack = cTRRootController.getPack();
			preLevelSelect();
		}
		showView(viewToShow);
		CTRSoundMgr._stopMusic();
		CTRSoundMgr._playMusic(197);
	}

	public virtual void showNextPack()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		int num = cTRRootController.getPack();
		if (num < CTRPreferences.getPacksCount() - 1)
		{
			packContainer.delegateScrollableContainerProtocol = this;
			packContainer.moveToScrollPointmoveMultiplier(num + 1, 0.8);
			showNextPackStatus = true;
			return;
		}
		replayingIntroMovie = false;
		packContainer.placeToScrollPoint(cTRRootController.getPack() + 1);
		CTRSoundMgr._stopMusic();
		NSString moviePath = NSObject.NSS("outro.wmv");
		Application.sharedMovieMgr().delegateMovieMgrDelegate = this;
		Application.sharedMovieMgr().playURL(moviePath, !Preferences._getBooleanForKey("SOUND_ON"));
	}

	public override void onChildDeactivated(int n)
	{
		base.onChildDeactivated(n);
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		cTRRootController.setSurvival(s: false);
		base.deactivate();
	}

	public virtual void moviePlaybackFinished(NSString url)
	{
		if (replayingIntroMovie)
		{
			hideActiveView();
			replayingIntroMovie = false;
			activateChild(CHILD_TYPE.CHILD_PICKER);
			return;
		}
		CTRSoundMgr._playMusic(197);
		_ = (CTRRootController)Application.sharedRootController();
		if (CTRPreferences.shouldPlayLevelScroll())
		{
			packContainer.placeToScrollPoint(CTRPreferences.getPacksCount() - 1);
			packContainer.moveToScrollPointmoveMultiplier(0, 0.6);
			CTRPreferences.disablePlayLevelScroll();
		}
		else
		{
			packContainer.placeToScrollPoint(CTRPreferences.getLastPack());
		}
		showView(ViewID.VIEW_PACK_SELECT);
		AndroidAPI.showBanner();
		if (url != null && url.rangeOfString(NSObject.NSS("outro.wmv")).length != 0)
		{
			packContainer.moveToScrollPointmoveMultiplier(CTRPreferences.getPacksCount(), 0.8);
			if (!CTRPreferences.isLiteVersion())
			{
				showGameFinishedPopup();
			}
		}
	}

	private void preLevelSelect()
	{
		if (getView(ViewID.VIEW_LEVEL_SELECT) != null)
		{
			deleteView(ViewID.VIEW_LEVEL_SELECT);
		}
		createLevelSelect();
	}

	public virtual void timelineFinished(Timeline t)
	{
		CTRSoundMgr._stopMusic();
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		cTRRootController.setPack(pack);
		cTRRootController.setLevel(level);
		Application.sharedRootController().setViewTransition(-1);
		MapPickerController mapPickerController = (MapPickerController)getChild(0);
		mapPickerController.setAutoLoadMap(LevelsList.LEVEL_NAMES[pack, level]);
		if ((pack == 0 && level == 0 && CTRPreferences.getScoreForPackLevel(0, 0) != 0) || (!introMovieOnStartup && pack == 0 && level == 0))
		{
			replayingIntroMovie = true;
			CTRSoundMgr._stopMusic();
			NSString moviePath = NSObject.NSS("intro.wmv");
			Application.sharedMovieMgr().delegateMovieMgrDelegate = this;
			Application.sharedMovieMgr().playURL(moviePath, !Preferences._getBooleanForKey("SOUND_ON"));
		}
		else
		{
			activateChild(CHILD_TYPE.CHILD_PICKER);
		}
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}

	public virtual void onButtonPressed(int n)
	{
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Expected O, but got Unknown
		if (n != -1 && n != 24 && n != 32)
		{
			CTRSoundMgr._playSound(19);
		}
		if (n >= 1000)
		{
			level = n - 1000;
			BaseElement childWithName = activeView().getChildWithName(NSObject.NSS("levelsBox"));
			childWithName.playTimeline(0);
			BaseElement childWithName2 = activeView().getChildWithName(NSObject.NSS("shadow"));
			childWithName2.playTimeline(0);
			BaseElement childWithName3 = activeView().getChildWithName(NSObject.NSS("starText"));
			childWithName3.playTimeline(0);
			BaseElement childWithName4 = activeView().getChildWithName(NSObject.NSS("backButton"));
			childWithName4.playTimeline(0);
			return;
		}
		switch (n)
		{
		case 45:
			if (ep == null)
			{
				showBuyFullPopup();
			}
			break;
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
			pack = n - 18;
			preLevelSelect();
			showView(ViewID.VIEW_LEVEL_SELECT);
			break;
		case 0:
		{
			FlurryAPI.logEvent("MMENU_PLAYBT_PRESSED", new List<string>
			{
				"game_unlocked",
				CTRPreferences.isLiteVersion() ? "0" : "1"
			});
			int i = 0;
			for (int packsCount = CTRPreferences.getPacksCount(); i < packsCount; i++)
			{
				GameController.checkForBoxPerfect(i);
			}
			introMovieOnStartup = Preferences._getBooleanForKey("PREFS_INTRO_MOVIE_ON_STARTUP");
			introMovieOnStartup = !introMovieOnStartup;
			replayingIntroMovie = false;
			if (CTRPreferences.getScoreForPackLevel(0, 0) == 0 && introMovieOnStartup)
			{
				showView(ViewID.VIEW_MOVIE);
				CTRSoundMgr._stopMusic();
				NSString moviePath = NSObject.NSS("intro.wmv");
				Application.sharedMovieMgr().delegateMovieMgrDelegate = this;
				Application.sharedMovieMgr().playURL(moviePath, !Preferences._getBooleanForKey("SOUND_ON"));
			}
			else
			{
				moviePlaybackFinished(null);
			}
			showBuyFull = false;
			break;
		}
		case 6:
		{
			bool flag2 = Preferences._getBooleanForKey("SOUND_ON");
			Preferences._setBooleanforKey(!flag2, "SOUND_ON", comit: true);
			if (!flag2)
			{
			}
			break;
		}
		case 7:
		{
			bool flag3 = Preferences._getBooleanForKey("VOICE_ON");
			Preferences._setBooleanforKey(!flag3, "VOICE_ON", comit: true);
			if (flag3)
			{
				CTRSoundMgr._stopVoices();
			}
			else
			{
				CTRSoundMgr._playVoice(61);
			}
			break;
		}
		case 9:
		{
			bool flag = Preferences._getBooleanForKey("MUSIC_ON");
			Preferences._setBooleanforKey(!flag, "MUSIC_ON", comit: true);
			Thread.Sleep(100);
			if (flag)
			{
				CTRSoundMgr._stopMusic();
			}
			else
			{
				CTRSoundMgr._playMusic(197);
			}
			break;
		}
		case 1:
			showView(ViewID.VIEW_OPTIONS);
			break;
		case 11:
			AndroidAPI.hideBanner();
			showView(ViewID.VIEW_RESET);
			break;
		case 12:
			helpContainer.placeToScrollPoint(0);
			showView(ViewID.VIEW_HELP);
			break;
		case 33:
			CTRPreferences.setNewDrawingsCounter(0);
			showView(ViewID.VIEW_DRAWINGS);
			break;
		case 10:
			aboutContainer.setScroll(MathHelper.vect(0.0, 0.0));
			aboutAutoScroll = true;
			showView(ViewID.VIEW_ABOUT);
			break;
		case 46:
			AndroidAPI.openUrl(Application.getString(1179737));
			break;
		case 16:
		{
			CTRPreferences cTRPreferences = Application.sharedPreferences();
			cTRPreferences.resetToDefaults();
			cTRPreferences.savePreferences();
			deleteView(ViewID.VIEW_PACK_SELECT);
			createPackSelect();
			deleteView(ViewID.VIEW_DRAWINGS);
			createDrawings();
			showView(ViewID.VIEW_OPTIONS);
			break;
		}
		case 29:
			throw new NotImplementedException();
		case 30:
			ep.hidePopup();
			ep = null;
			FLAG_RESTORING = false;
			break;
		case 31:
			ep.hidePopup();
			ep = null;
			break;
		case 17:
			showView(ViewID.VIEW_OPTIONS);
			break;
		case 13:
			showView(ViewID.VIEW_MAIN_MENU);
			break;
		case 14:
		case 41:
		case 43:
			showView(ViewID.VIEW_OPTIONS);
			break;
		case 15:
			Application.sharedRootController().setViewTransition(4);
			Application.sharedRootController().setTransitionTime();
			Application.sharedRootController().onControllerViewHide(getView(ViewID.VIEW_LEVEL_SELECT));
			deleteView(ViewID.VIEW_LEVEL_SELECT);
			Application.sharedResourceMgr();
			scrollableContainerreachedScrollPoint(packContainer, pack);
			showView(ViewID.VIEW_PACK_SELECT);
			Application.sharedRootController().onControllerViewShow(getView(ViewID.VIEW_PACK_SELECT));
			break;
		case 3:
		{
			CTRSoundMgr._stopMusic();
			pack = 0;
			Application.sharedRootController().setViewTransition(-1);
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			CTRResourceMgr cTRResourceMgr = Application.sharedResourceMgr();
			cTRResourceMgr.initLoading();
			cTRResourceMgr.loadImmediately();
			cTRRootController.setSurvival(s: true);
			cTRRootController.setPack(pack);
			deactivate();
			break;
		}
		case 2:
		{
			pack = ((CTRRootController)Application.sharedRootController()).getPack();
			preLevelSelect();
			Application.sharedRootController().setViewTransition(-1);
			MapPickerController mapPickerController = (MapPickerController)getChild(0);
			mapPickerController.setNormalMode();
			activateChild(CHILD_TYPE.CHILD_PICKER);
			break;
		}
		case 28:
			AndroidAPI.exitApp();
			break;
		case 4:
			Scorer.activateScorerUIAtProfile();
			break;
		case 5:
			CTRRootController.openFullVersionPage();
			break;
		case 26:
			AndroidAPI.openUrl("https://mobile.twitter.com/zeptolab");
			break;
		case 27:
			AndroidAPI.openUrl("http://www.facebook.com/cuttherope");
			break;
		case 36:
		case 39:
		{
			break;
		}
		case 44:
			showView(ViewID.VIEW_LEADERBOARDS);
			break;
		case 42:
			/*
			if (AchievementsView.Init)
			{
				((AchievementsView)views[9]).resetScroll();
				showView(ViewID.VIEW_ACHIEVEMENTS);
			}
			*/
			break;
		case 8:
		case 24:
		case 25:
		case 32:
		case 34:
		case 35:
		case 37:
		case 38:
		case 40:
			break;
		}
	}

	private void gotoNextBox()
	{
		Application.sharedPreferences();
		int i = 0;
		for (int packsCount = CTRPreferences.getPacksCount(); i < packsCount; i++)
		{
			if (CTRPreferences.getUnlockedForPackLevel(i, 0) == UNLOCKED_STATE.UNLOCKED_STATE_JUST_UNLOCKED_WITH_CHEAT)
			{
				packContainer.moveToScrollPointmoveMultiplier(i, 0.8);
				ddPackSelect.callObjectSelectorParamafterDelay(selector_gotoNextBox, null, 0.7);
				return;
			}
		}
		packContainer.moveToScrollPointmoveMultiplier(animationStartPackIndex, 0.3);
		unlockAnimation = false;
	}

	private void unlockBoxes()
	{
		needUnlock = false;
		Application.sharedPreferences();
		unlockAnimation = true;
		animationStartPackIndex = currentPackIndex;
		int i = 0;
		for (int packsCount = CTRPreferences.getPacksCount(); i < packsCount; i++)
		{
			if (CTRPreferences.getUnlockedForPackLevel(i, 0) == UNLOCKED_STATE.UNLOCKED_STATE_LOCKED)
			{
				CTRPreferences.setUnlockedForPackLevel(UNLOCKED_STATE.UNLOCKED_STATE_JUST_UNLOCKED_WITH_CHEAT, i, 0);
			}
		}
		deleteView(ViewID.VIEW_PACK_SELECT);
		createPackSelect();
		ddPackSelect.callObjectSelectorParamafterDelay(selector_gotoNextBox, null, 0.5);
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
		if (activeViewID == 3 && aboutAutoScroll)
		{
			Vector scroll = aboutContainer.getScroll();
			Vector maxScroll = aboutContainer.getMaxScroll();
			scroll.y += 0.5f;
			scroll.y = MathHelper.FIT_TO_BOUNDARIES(scroll.y, 0.0, maxScroll.y);
			aboutContainer.setScroll(scroll);
		}
		else if (activeViewID == 5)
		{
			float num = 0f;
			for (int i = 0; i < CTRPreferences.getPacksCount(); i++)
			{
				BaseElement baseElement = boxes[i];
				BaseElement childWithName = baseElement.getChildWithName(NSObject.NSS("boxContainer"));
				BaseElement childWithName2 = childWithName.getChildWithName(NSObject.NSS("box"));
				BaseElement childWithName3 = childWithName.getChildWithName(NSObject.NSS("boxSelected"));
				BaseElement childWithName4 = childWithName.getChildWithName(NSObject.NSS("boxPic"));
				Vector scroll2 = packContainer.getScroll();
				float x = packContainer.getScrollPoint(1).x;
				float num2 = Math.Abs((scroll2.x + packContainer.getScrollPoint(i).x) / x);
				if (num2 <= 1f)
				{
					num = MathHelper.MAX(num, 1f - num2);
					childWithName3.color = RGBAColor.MakeRGBA(1f - num2, 1f - num2, 1f - num2, 1f - num2);
					childWithName2.color = RGBAColor.MakeRGBA(num2, num2, num2, num2);
					RGBAColor rGBAColor = new RGBAColor(172.0 * (double)(1f - num2) / 255.0, 85.0 * (double)(1f - num2) / 255.0, 13.0 * (double)(1f - num2) / 255.0, 255.0 * (double)(1f - num2) / 255.0);
					RGBAColor rGBAColor2 = new RGBAColor(54.0 * (double)num2 / 255.0, 100.0 * (double)num2 / 255.0, 169.0 * (double)num2 / 255.0, 255.0 * (double)num2 / 255.0);
					childWithName4.color.r = rGBAColor2.r * (1f - num) + rGBAColor.r * num;
					childWithName4.color.g = rGBAColor2.g * (1f - num) + rGBAColor.g * num;
					childWithName4.color.b = rGBAColor2.b * (1f - num) + rGBAColor.b * num;
					childWithName4.color.a = rGBAColor2.a * (1f - num) + rGBAColor.a * num;
				}
			}
			float num3 = 1f - num;
			num -= num3;
			light.color = RGBAColor.MakeRGBA(num, num, num, num);
			if (ddPackSelect != null)
			{
				ddPackSelect.update(delta);
			}
		}
		else if (activeViewID == 0 && ddMainMenu != null)
		{
			ddMainMenu.update(delta);
		}
		if (needRecreate)
		{
			needRecreate = false;
			deleteView(ViewID.VIEW_PACK_SELECT);
			createPackSelect();
		}
		if (needUnlock)
		{
			unlockBoxes();
		}
		if (statusBackupRestore > 0)
		{
			if (ep != null)
			{
				ep.hidePopup();
				ep = null;
			}
			switch (statusBackupRestore)
			{
			case 1:
				showStatusPopup(Application.getString(1179701));
				break;
			case 2:
				deleteView(ViewID.VIEW_DRAWINGS);
				createDrawings();
				showStatusPopup(Application.getString(1179700));
				break;
			}
			statusBackupRestore = 0;
		}
	}

	public override bool touchesBeganwithEvent(TouchCollection touches)
	{
		if (unlockAnimation)
		{
			return true;
		}
		base.touchesBeganwithEvent(touches);
		if (activeViewID == 3 && aboutAutoScroll)
		{
			aboutAutoScroll = false;
		}
		return true;
	}

	public override bool backButtonPressed()
	{
		if (FLAG_RESTORING || unlockAnimation)
		{
			return true;
		}
		if (ep != null)
		{
			ep.hidePopup();
			ep = null;
			return true;
		}
		int num = activeViewID;
		if (num == 0)
		{
			/*
			PromoBanner promoBanner = (PromoBanner)activeView().getChildWithName("promoBanner");
			if (promoBanner != null && !promoBanner.promoMainHidden)
			{
				promoBanner.closeMainPromo();
				return true;
			}
			*/
			showYesNoPopup(Application.getString(1179663), 28, 31);
		}
		if (num == 8 && ((DrawingsView)activeView()).ad != null)
		{
			((DrawingsView)activeView()).ad.hideDrawing();
			return true;
		}
		switch (num)
		{
		case 1:
		case 8:
			onButtonPressed(13);
			break;
		case 2:
		case 3:
		case 4:
			onButtonPressed(14);
			break;
		case 5:
			onButtonPressed(13);
			break;
		case 6:
			onButtonPressed(15);
			break;
		case 9:
			onButtonPressed(14);
			break;
		case 10:
			onButtonPressed(14);
			break;
		}
		return true;
	}

	public void updateNewDrawingsCounter()
	{
		string text = Application.getString(1179709).ToString();
		int num = 0;
		while (text.Contains("%d"))
		{
			int startIndex = text.IndexOf("%d");
			text = text.Remove(text.IndexOf("%d"), 2).Insert(startIndex, "{" + num + "}");
			num++;
		}
		totalFound.setString(NSObject.NSS(string.Format(text, CTRPreferences.getDrawingUnlockedCount(), CTRPreferences.DRAWINGS_COUNT())));
	}

	private void playHandAnimation()
	{
	}

	private void restoreSuccess()
	{
		FLAG_RESTORING = false;
		needRecreate = true;
		statusBackupRestore = 2;
	}

	private void setProblem(int problemID)
	{
		FLAG_RESTORING = false;
		statusBackupRestore = problemID;
	}

	private void showStatusPopup(NSString statusText)
	{
		FontGeneric font = Application.getFont(5);
		float w = 250f;
		Text text = new Text().initWithFont(font);
		text.setAlignment(2);
		text.setStringandWidth(statusText, w);
		Button buttons = createButtonWithTextIDDelegate(Application.getString(1179681), 30, this);
		showPopup(text, buttons);
	}

	private void showPopup(BaseElement message, BaseElement buttons)
	{
		Popup popup = (Popup)new Popup().init();
		popup.setName(NSObject.NSS("popup"));
		Image image = Image.Image_createWithResIDQuad(70, 0);
		image.anchor = (image.parentAnchor = 18);
		popup.addChild(image);
		message.anchor = (message.parentAnchor = 18);
		image.addChild(message);
		buttons.y += -14f;
		buttons.anchor = 18;
		buttons.parentAnchor = 34;
		image.addChild(buttons);
		popup.showPopup();
		ep = popup;
		View view = activeView();
		view.addChild(popup);
	}

	public virtual void showYesNoPopup(NSString statusText, int buttonYes, int buttonNo)
	{
		Button button = createButtonWithTextIDDelegate(Application.getString(1179675), buttonNo, this);
		button.anchor = (button.parentAnchor = 18);
		button.setTouchIncreaseLeftRightTopBottom(15f, 15f, 0f, 0f);
		Button button2 = createButtonWithTextIDDelegate(Application.getString(1179674), buttonYes, this);
		button2.anchor = 33;
		button2.parentAnchor = 9;
		button2.setTouchIncreaseLeftRightTopBottom(15f, 15f, 0f, 0f);
		button.addChild(button2);
		FontGeneric font = Application.getFont(5);
		float w = 250f;
		Text text = new Text().initWithFont(font);
		text.setAlignment(2);
		text.setStringandWidth(statusText, w);
		text.y = -34f;
		showPopup(text, button);
	}

	public virtual void showBuyFullPopup()
	{
		Button button = createButtonWithTextIDDelegate(Application.getString(1179734), 31, this);
		button.anchor = (button.parentAnchor = 18);
		button.setTouchIncreaseLeftRightTopBottom(15f, 15f, 0f, 0f);
		Button button2 = createButtonWithTextIDDelegateAutoScale(Application.getString(1179651), 5, this);
		button2.anchor = 33;
		button2.parentAnchor = 9;
		button2.setTouchIncreaseLeftRightTopBottom(15f, 15f, 0f, 0f);
		button.addChild(button2);
		FontGeneric font = Application.getFont(5);
		float w = 250f;
		Text text = new Text().initWithFont(font);
		text.setAlignment(2);
		text.setStringandWidth(Application.getString(1179733), w);
		text.y = -34f;
		showPopup(text, button);
	}

	public virtual void showUnlockShareware()
	{
		showYesNoPopup(NSObject.NSS("STR_MENU_GET_FULLVERSION_QUESTION"), 38, 31);
	}

	public virtual void showCantUnlockPopupForPack(int pack)
	{
		float w = 250f;
		VBox vBox = new VBox().initWithOffsetAlignWidth(-10.0, 2, FrameworkTypes.SCREEN_WIDTH);
		vBox.anchor = 18;
		Text text = new Text().initWithFont(Application.getFont(5));
		text.setAlignment(2);
		text.setString(Application.getString(1179683));
		vBox.addChild(text);
		int totalStars = CTRPreferences.getTotalStars();
		BaseElement c = createTextWithStar((CTRPreferences.packUnlockStars(pack) - totalStars).ToString());
		vBox.addChild(c);
		Text text2 = new Text().initWithFont(Application.getFont(5));
		text2.setAlignment(2);
		text2.setStringandWidth(Application.getString(1179684), w);
		vBox.addChild(text2);
		Text text3 = new Text().initWithFont(Application.getFont(6));
		text3.setAlignment(2);
		text3.setStringandWidth(Application.getString(1179685), w);
		vBox.addChild(text3);
		Button buttons = createButtonWithTextIDDelegate(Application.getString(1179681), 31, this);
		showPopup(vBox, buttons);
	}

	private void showGameFinishedPopup()
	{
		float w = 250f;
		VBox vBox = new VBox().initWithOffsetAlignWidth(40.0, 2, FrameworkTypes.SCREEN_WIDTH);
		vBox.anchor = 18;
		Text text = new Text().initWithFont(Application.getFont(5));
		text.setAlignment(2);
		text.setStringandWidth(Application.getString(1179686), w);
		vBox.addChild(text);
		Text text2 = new Text().initWithFont(Application.getFont(6));
		text2.setAlignment(2);
		text2.setStringandWidth(Application.getString(1179687), w);
		vBox.addChild(text2);
		Button buttons = createButtonWithTextIDDelegate(Application.getString(1179681), 31, this);
		showPopup(vBox, buttons);
	}

	public virtual void changedActiveSpointFromTo(int pp, int cp)
	{
	}

	private void showView(ViewID n)
	{
		if (n == ViewID.VIEW_PACK_SELECT)
		{
			FlurryAPI.logEvent("BOXSEL_SCREEN_SHOWN", new List<string>
			{
				"game_unlocked",
				CTRPreferences.isLiteVersion() ? "0" : "1"
			});
		}
		if (n == ViewID.VIEW_LEVEL_SELECT)
		{
			FlurryAPI.logEvent("LEVSEL_SCREEN_SHOWN", new List<string>
			{
				"box_id",
				pack.ToString(),
				"game_unlocked",
				CTRPreferences.isLiteVersion() ? "0" : "1"
			});
		}
		if (n == ViewID.VIEW_MAIN_MENU)
		{
			if (!Preferences.firstStart)
			{
				FirstTime = false;
			}
			string item = Assembly.GetExecutingAssembly().FullName.Split('=')[1].Split(',')[0];
			FlurryAPI.logEvent("MMENU_SCREEN_SHOWN", new List<string>
			{
				"version",
				item,
				"first_time",
				FirstTime.ToString(),
				"game_unlocked",
				CTRPreferences.isLiteVersion() ? "0" : "1"
			});
			FirstTime = false;
		}
		base.showView((int)n);
	}

	private View getView(ViewID n)
	{
		return base.getView((int)n);
	}

	private void deleteView(ViewID n)
	{
		base.deleteView((int)n);
	}

	private void addViewwithID(View view, ViewID n)
	{
		base.addViewwithID(view, (int)n);
	}

	private void activateChild(CHILD_TYPE c)
	{
		base.activateChild((int)c);
	}

	public void createAchievements()
	{
		/*
		AchievementsView achievementsView = (AchievementsView)new AchievementsView().init();
		addViewwithID(achievementsView, ViewID.VIEW_ACHIEVEMENTS);
		Button button = createBackButtonWithDelegateID(this, 41);
		button.setName("backb");
		button.x = 0f;
		achievementsView.addChild(button);
		*/
	}

	public virtual void createLeaderboards()
	{
		/*
		LeaderboardsView leaderboardsView = (LeaderboardsView)new LeaderboardsView().init();
		addViewwithID(leaderboardsView, ViewID.VIEW_LEADERBOARDS);
		Button button = createBackButtonWithDelegateID(this, 43);
		button.setName("backb");
		button.x = 0f;
		leaderboardsView.addChildwithID(button, leaderboardsView.childsCount());
		*/
	}

	public static Image createBlankScoresButtonWithIconpressed(int quad, bool pressed)
	{
		Image image = Image.Image_createWithResIDQuad(302, pressed ? 1 : 0);
		Image image2 = Image.Image_createWithResIDQuad(302, quad);
		image.addChild(image2);
		image2.parentAnchor = 9;
		Image.setElementPositionWithRelativeQuadOffset(image2, 302, 0, quad);
		return image;
	}

	public static Button createScoresButtonWithIconbuttonIDdelegate(int quad, int bId, ButtonDelegate delegateValue)
	{
		Image up = createBlankScoresButtonWithIconpressed(quad, pressed: false);
		Image image = createBlankScoresButtonWithIconpressed(quad, pressed: true);
		Image.setElementPositionWithRelativeQuadOffset(image, 302, 0, 1);
		Button button = new Button().initWithUpElementDownElementandID(up, image, bId);
		button.delegateButtonDelegate = delegateValue;
		return button;
	}

	public virtual MenuView createScrollableViewWithText(NSString content, ref ScrollableContainer container, int backButton)
	{
		MenuView menuView = (MenuView)new MenuView().initFullscreen();
		Image image = createBackgroundWithLogo(l: false);
		Text text = new Text().initWithFont(Application.getFont(6));
		NSObject.NSS("undefined version");
		if (CTRPreferences.isLiteVersion())
		{
			NSObject.NSS(" Lite");
		}
		else
		{
			NSObject.NSS("");
		}
		string text2 = Application.getString(1179722).ToString();
		string[] separator = new string[1] { "%@" };
		string[] array = text2.Split(separator, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (i == 0)
			{
				text2 = "";
			}
			if (i == 2)
			{
				string fullName = Assembly.GetExecutingAssembly().FullName;
				text2 += fullName.Split('=')[1].Split(',')[0];
				text2 += " ";
			}
			text2 += array[i];
		}
		text.setAlignment(2);
		text.setStringandWidth(NSObject.NSS(text2), 310f);
		text.height += 30;
		container = new ScrollableContainer().initWithWidthHeightContainerWidthHeight(310f, 350f, 310f, text.height);
		container.anchor = (container.parentAnchor = 18);
		container.y = 0f;
		container.addChild(text);
		Image image2 = Image.Image_createWithResIDQuad(72, 13);
		image2.anchor = (image2.parentAnchor = 18);
		image2.y -= 360f;
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
		{
			image2.y -= 220f;
		}
		text.addChild(image2);
		image.addChild(container);
		Button c = createBackButtonWithDelegateID(this, backButton);
		image.addChild(c);
		menuView.addChild(image);
		return menuView;
	}

	private static Image createAudioElementForQuadwithCrosspressed(int q, bool b, bool p)
	{
		int num = (p ? 1 : 0);
		Image image = Image.Image_createWithResIDQuad(89, num);
		image.texture.PixelCorrectionX();
		Image image2 = Image.Image_createWithResIDQuad(89, q);
		image2.parentAnchor = (image2.anchor = 18);
		image.addChild(image2);
		if (b)
		{
			image2.color = RGBAColor.MakeRGBA(0.5f, 0.5f, 0.5f, 0.5f);
			Image image3 = Image.Image_createWithResIDQuad(89, 5);
			image3.parentAnchor = (image3.anchor = 9);
			Image.setElementPositionWithRelativeQuadOffset(image3, 89, num, 5);
			image.addChild(image3);
		}
		return image;
	}

	private static ToggleButton createAudioButtonWithQuadDelegateID(int q, ButtonDelegate d, int bid)
	{
		Image u = createAudioElementForQuadwithCrosspressed(q, b: false, p: false);
		Image d2 = createAudioElementForQuadwithCrosspressed(q, b: false, p: true);
		Image u2 = createAudioElementForQuadwithCrosspressed(q, b: true, p: false);
		Image d3 = createAudioElementForQuadwithCrosspressed(q, b: true, p: true);
		ToggleButton toggleButton = new ToggleButton().initWithUpElement1DownElement1UpElement2DownElement2andID(u, d2, u2, d3, bid);
		toggleButton.delegateButtonDelegate = d;
		return toggleButton;
	}

	private static Button createLanguageButtonWithIDDelegate(int bid, ButtonDelegate d)
	{
		NSString @string = Application.sharedAppSettings().getString(8);
		int q = 5;
		if (@string.isEqualToString(NSObject.NSS("ru")))
		{
			q = 2;
		}
		else if (@string.isEqualToString(NSObject.NSS("de")))
		{
			q = 3;
		}
		else if (@string.isEqualToString(NSObject.NSS("fr")))
		{
			q = 4;
		}
		NSString string2 = Application.getString(1179659);
		Image image = Image.Image_createWithResIDQuad(4, 0);
		Image image2 = Image.Image_createWithResIDQuad(4, 1);
		FontGeneric font = Application.getFont(5);
		Text text = new Text().initWithFont(font);
		text.setString(string2);
		Text text2 = new Text().initWithFont(font);
		text2.setString(string2);
		text.anchor = (text.parentAnchor = 18);
		text2.anchor = (text2.parentAnchor = 18);
		image.addChild(text);
		image2.addChild(text2);
		Image image3 = Image.Image_createWithResIDQuad(76, q);
		Image image4 = Image.Image_createWithResIDQuad(76, q);
		image4.parentAnchor = (image3.parentAnchor = 20);
		image4.anchor = (image3.anchor = 20);
		text.addChild(image3);
		text2.addChild(image4);
		text.width += image3.width + 10;
		text2.width += image4.width + 10;
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.setTouchIncreaseLeftRightTopBottom(15.0, 15.0, 15.0, 15.0);
		button.delegateButtonDelegate = d;
		return button;
	}

	public override void showView(int n)
	{
		if (n == 1 || n == 4 || n == 5 || (n == 6 && FrameworkTypes.SCREEN_RATIO >= 1.6f))
		{
			AndroidAPI.showBanner();
		}
		else
		{
			AndroidAPI.hideBanner();
		}
		base.showView(n);
	}

	public virtual void createDrawings()
	{
		DrawingsView drawingsView = (DrawingsView)new DrawingsView().initFullscreen();
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		baseElement.width = (int)FrameworkTypes.SCREEN_WIDTH;
		baseElement.height = (int)FrameworkTypes.SCREEN_HEIGHT;
		baseElement.parentAnchor = 9;
		drawingsView.addChild(baseElement);
		TiledImage tiledImage = TiledImage.TiledImage_createWithResID(245);
		tiledImage.setTile(0);
		tiledImage.width = (int)FrameworkTypes.SCREEN_WIDTH;
		tiledImage.height = (int)FrameworkTypes.SCREEN_HEIGHT;
		tiledImage.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		tiledImage.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
		tiledImage.passTransformationsToChilds = false;
		tiledImage.anchor = (tiledImage.parentAnchor = 9);
		baseElement.addChild(tiledImage);
		float num = FrameworkTypes.SCREEN_WIDTH * 3f / 2f;
		ScrollableContainer scrollableContainer = new ScrollableContainer().initWithWidthHeightContainerWidthHeight(FrameworkTypes.SCREEN_WIDTH, FrameworkTypes.SCREEN_HEIGHT, num, FrameworkTypes.SCREEN_HEIGHT);
		scrollableContainer.shouldBounceHorizontally = true;
		scrollableContainer.bounceMovementDivide = 5f;
		scrollableContainer.resetScrollOnShow = false;
		scrollableContainer.parentAnchor = (scrollableContainer.anchor = 9);
		scrollableContainer.width = (int)FrameworkTypes.SCREEN_WIDTH;
		scrollableContainer.height = (int)FrameworkTypes.SCREEN_HEIGHT;
		scrollableContainer.maxBounceOffsetX = FrameworkTypes.SCREEN_WIDTH / 2f;
		scrollableContainer.isForAboutMenu = true;
		tiledImage.addChild(scrollableContainer);
		BaseElement baseElement2 = (BaseElement)new BaseElement().init();
		baseElement2.parentAnchor = (baseElement2.anchor = 9);
		baseElement2.width = (int)num;
		baseElement2.height = (int)FrameworkTypes.SCREEN_HEIGHT;
		scrollableContainer.addChild(baseElement2);
		Image image = Image.Image_createWithResIDQuad(245, 3);
		image.rotation = 90f;
		image.parentAnchor = 9;
		image.scaleX = 1f;
		Image.setElementPositionWithQuadCenter(image, 244, 5);
		image.x -= FrameworkTypes.SCREEN_WIDTH;
		baseElement2.addChild(image);
		Image image2 = Image.Image_createWithResIDQuad(245, 4);
		image2.rotation = 90f;
		image2.parentAnchor = 9;
		image2.scaleX = 1f;
		Image.setElementPositionWithQuadCenter(image2, 244, 9);
		image2.x -= FrameworkTypes.SCREEN_WIDTH;
		baseElement2.addChild(image2);
		Image image3 = Image.Image_createWithResIDQuad(245, 3);
		image3.rotation = 90f;
		image3.parentAnchor = 9;
		image3.scaleX = 1f;
		Image.setElementPositionWithQuadCenter(image3, 244, 6);
		image3.x -= FrameworkTypes.SCREEN_WIDTH;
		baseElement2.addChild(image3);
		Image image4 = Image.Image_createWithResIDQuad(245, 4);
		image4.rotation = 90f;
		image4.parentAnchor = 9;
		image4.scaleX = 1f;
		Image.setElementPositionWithQuadCenter(image4, 244, 10);
		image4.x -= FrameworkTypes.SCREEN_WIDTH;
		baseElement2.addChild(image4);
		Image image5 = Image.Image_createWithResIDQuad(245, 3);
		image5.rotation = 90f;
		image5.parentAnchor = 9;
		image5.scaleX = -1f;
		Image.setElementPositionWithQuadCenter(image5, 244, 7);
		image5.x -= FrameworkTypes.SCREEN_WIDTH;
		image5.y -= 0.4f;
		baseElement2.addChild(image5);
		Image image6 = Image.Image_createWithResIDQuad(245, 4);
		image6.rotation = 90f;
		image6.parentAnchor = 9;
		image6.scaleX = -1f;
		Image.setElementPositionWithQuadCenter(image6, 244, 11);
		image6.x -= FrameworkTypes.SCREEN_WIDTH;
		baseElement2.addChild(image6);
		Image image7 = Image.Image_createWithResIDQuad(245, 3);
		image7.rotation = 90f;
		image7.parentAnchor = 9;
		image7.scaleX = -1f;
		Image.setElementPositionWithQuadCenter(image7, 244, 8);
		image7.x -= FrameworkTypes.SCREEN_WIDTH;
		baseElement2.addChild(image7);
		Image image8 = Image.Image_createWithResIDQuad(245, 4);
		image8.rotation = 90f;
		image8.parentAnchor = 9;
		image8.scaleX = -1f;
		Image.setElementPositionWithQuadCenter(image8, 244, 12);
		image8.x -= FrameworkTypes.SCREEN_WIDTH;
		baseElement2.addChild(image8);
		int[] array = new int[8] { 32, 33, 34, 35, 36, 37, 38, 39 };
		float[] array2 = new float[9] { 0.8506f, 0.7341f, 0.7501f, 0.9015f, 0.7501f, 0.7692f, 0.7692f, 0.85f, -1f };
		float[] array3 = new float[9] { 0.26f, 0.74f, -5f, 0.3f, 9.71f, 0.74f, 0.74f, -3f, -1f };
		float[] array4 = new float[9] { 1f, 1f, 1f, 1f, 0.92f, 1f, 1f, 1f, -1f };
		int num2 = 0;
		for (int num3 = 7; num3 >= 0; num3--)
		{
			if (num3 != 3 && num3 != 4)
			{
				int d = num3;
				if (CTRPreferences.getDrawingUnlocked(num3) == 0)
				{
					d = ((num3 != 3) ? (-1) : (-2));
				}
				else
				{
					num2++;
				}
				Drawing drawing = (Drawing)new Drawing().initWithDrawing(d);
				drawing.texture.PixelCorrectionXY();
				drawing.parentAnchor = 9;
				Image.setElementPositionWithQuadOffset(drawing, 244, array[num3]);
				drawing.x -= FrameworkTypes.SCREEN_WIDTH;
				drawing.scaleX = (drawing.scaleY = array2[num3]);
				drawing.rotation = array3[num3];
				drawing.delegateDrawingDelegate = drawingsView;
				baseElement2.addChild(drawing);
				Image image9 = Image.Image_createWithResIDQuad(245, 5);
				image9.parentAnchor = 9;
				Image.setElementPositionWithQuadCenter(image9, 244, 44 + num3);
				image9.x -= FrameworkTypes.SCREEN_WIDTH;
				baseElement2.addChild(image9);
				image9.rotation = 0f;
				image9.scaleX = (image9.scaleY = array4[num3]);
			}
		}
		baseElement.addChild(tiledImage);
		Image image10 = Image.Image_createWithResIDQuad(245, 1);
		image10.parentAnchor = (image10.anchor = 33);
		baseElement.addChild(image10);
		image10.y += FrameworkTypes.SCREEN_HEIGHT * (FrameworkTypes.SCREEN_BG_SCALE_Y - 1f);
		Image image11 = Image.Image_createWithResIDQuad(88, 0);
		image11.scaleX = (image11.scaleY = 2f);
		image11.anchor = (image11.parentAnchor = 18);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline.addKeyFrame(KeyFrame.makeRotation(45.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeRotation(405.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 75.0));
		timeline.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
		image11.addTimeline(timeline);
		image11.playTimeline(0);
		baseElement.addChild(image11);
		Text text = new Text().initWithFont(Application.getFont(5));
		text.setString(Application.getString(1179708));
		text.parentAnchor = (text.anchor = 10);
		baseElement.addChild(text);
		totalFound = new Text().initWithFont(Application.getFont(6));
		string text2 = Application.getString(1179709).ToString();
		int num4 = 0;
		while (text2.Contains("%d"))
		{
			int startIndex = text2.IndexOf("%d");
			text2 = text2.Remove(text2.IndexOf("%d"), 2).Insert(startIndex, "{" + num4 + "}");
			num4++;
		}
		totalFound.setString(NSObject.NSS(string.Format(text2, num2, CTRPreferences.DRAWINGS_COUNT())));
		totalFound.anchor = 10;
		totalFound.parentAnchor = 18;
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
		{
			totalFound.y += 10f;
		}
		text.addChild(totalFound);
		Button c = createBackButtonWithDelegateID(this, 13);
		baseElement.addChild(c);
		addViewwithID(drawingsView, ViewID.VIEW_DRAWINGS);
	}
}
