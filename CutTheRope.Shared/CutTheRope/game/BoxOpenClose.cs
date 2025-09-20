using System;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class BoxOpenClose : BaseElement, TimelineDelegate
{
	public delegate void boxClosed();

	public const int BOX_ANIM_LEVEL_FIRST_START = 0;

	public const int BOX_ANIM_LEVEL_START = 1;

	public const int BOX_ANIM_LEVEL_WON = 2;

	public const int BOX_ANIM_LEVEL_LOST = 3;

	public const int BOX_ANIM_LEVEL_QUIT = 4;

	public const int RESULT_STATE_WAIT = 0;

	public const int RESULT_STATE_SHOW_STAR_BONUS = 1;

	public const int RESULT_STATE_COUNTDOWN_STAR_BONUS = 2;

	public const int RESULT_STATE_HIDE_STAR_BONUS = 3;

	public const int RESULT_STATE_SHOW_TIME_BONUS = 4;

	public const int RESULT_STATE_COUNTDOWN_TIME_BONUS = 5;

	public const int RESULT_STATE_HIDE_TIME_BONUS = 6;

	public const int RESULT_STATE_SHOW_FINAL_SCORE = 7;

	public const int RESULTS_SHOW_ANIM = 0;

	public const int RESULTS_HIDE_ANIM = 1;

	public BaseElement openCloseAnims;

	public BaseElement confettiAnims;

	public BaseElement result;

	public int boxAnim;

	public bool shouldShowConfetti;

	public bool shouldShowImprovedResult;

	public bool shouldShowMachine;

	public bool machineEndState;

	public Image stamp;

	public int raState;

	public int timeBonus;

	public int starBonus;

	public int score;

	public float time;

	public float ctime;

	public int cstarBonus;

	public int cscore;

	public float raDelay;

	public boxClosed delegateboxClosed;

	public virtual NSObject initWithButtonDelegate(ButtonDelegate b)
	{
		if (init() != null)
		{
			result = (BaseElement)new BaseElement().init();
			addChildwithID(result, 1);
			anchor = (parentAnchor = 18);
			result.anchor = (result.parentAnchor = 18);
			result.setEnabled(e: false);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
			result.addTimelinewithID(timeline, 0);
			timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
			result.addTimelinewithID(timeline, 1);
			Image image = Image.Image_createWithResIDQuad(118, 14);
			image.anchor = 18;
			image.setName("star1");
			Image.setElementPositionWithQuadOffset(image, 118, 0);
			result.addChild(image);
			Image image2 = Image.Image_createWithResIDQuad(118, 14);
			image2.anchor = 18;
			image2.setName("star2");
			Image.setElementPositionWithQuadOffset(image2, 118, 1);
			result.addChild(image2);
			Image image3 = Image.Image_createWithResIDQuad(118, 14);
			image3.anchor = 18;
			image3.setName("star3");
			Image.setElementPositionWithQuadOffset(image3, 118, 2);
			result.addChild(image3);
			Text text = new Text().initWithFont(Application.getFont(5));
			text.setString(Application.getString(1179664));
			Image.setElementPositionWithQuadOffset(text, 118, 3);
			text.anchor = 18;
			text.setName("passText");
			result.addChild(text);
			Image image4 = Image.Image_createWithResIDQuad(118, 15);
			image4.anchor = 18;
			Image.setElementPositionWithQuadOffset(image4, 118, 4);
			result.addChild(image4);
			stamp = Image.Image_createWithResIDQuad(120, 0);
			Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(7);
			timeline2.addKeyFrame(KeyFrame.makeScale(3.0, 3.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline2.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.5));
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.5));
			stamp.addTimeline(timeline2);
			stamp.anchor = 18;
			stamp.setEnabled(e: false);
			Image.setElementPositionWithQuadOffset(stamp, 118, 12);
			result.addChild(stamp);
			Button button = MenuController.createShortButtonWithTextIDDelegate(Application.getString(1179676), 8, b);
			button.anchor = 18;
			button.setName("buttonWinRestart");
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
			{
				button.getChild(0).getChild(0).scaleX = 0.9f;
				button.getChild(0).getChild(0).scaleY = 0.9f;
				button.getChild(1).getChild(0).scaleX = 0.9f;
				button.getChild(1).getChild(0).scaleY = 0.9f;
			}
			Image.setElementPositionWithQuadOffset(button, 118, 11);
			result.addChild(button);
			Button button2 = MenuController.createShortButtonWithTextIDDelegate(Application.getString(1179677), 9, b);
			button2.anchor = 18;
			button2.setName("buttonWinNextLevel");
			Image.setElementPositionWithQuadOffset(button2, 118, 10);
			result.addChild(button2);
			Button button3 = MenuController.createShortButtonWithTextIDDelegate(Application.getString(1179678), 5, b);
			button3.anchor = 18;
			button3.setName("buttonWinExit");
			Image.setElementPositionWithQuadOffset(button3, 118, 9);
			result.addChild(button3);
			Text text2 = new Text().initWithFont(Application.getFont(6));
			text2.setName("dataTitle");
			text2.anchor = 18;
			Image.setElementPositionWithQuadOffset(text2, 118, 5);
			result.addChild(text2);
			Text text3 = new Text().initWithFont(Application.getFont(6));
			text3.setName("dataValue");
			text3.anchor = 18;
			Image.setElementPositionWithQuadOffset(text3, 118, 6);
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ES)
			{
				text3.x += 20f;
			}
			result.addChild(text3);
			Text text4 = new Text().initWithFont(Application.getFont(119));
			text4.setName("scoreValue");
			text4.anchor = 18;
			Image.setElementPositionWithQuadOffset(text4, 118, 8);
			result.addChild(text4);
			confettiAnims = (BaseElement)new BaseElement().init();
			result.addChild(confettiAnims);
			openCloseAnims = null;
			boxAnim = -1;
		}
		return this;
	}

	public virtual void levelFirstStart()
	{
		shouldShowMachine = true;
		machineEndState = true;
		boxAnim = 0;
		removeOpenCloseAnims();
		showOpenAnim();
		if (result.isEnabled())
		{
			result.playTimeline(1);
		}
	}

	public virtual void levelStart()
	{
		boxAnim = 1;
		removeOpenCloseAnims();
		showOpenAnim();
		if (result.isEnabled())
		{
			result.playTimeline(1);
		}
	}

	public virtual void levelWon()
	{
		boxAnim = 2;
		raState = -1;
		removeOpenCloseAnims();
		showCloseAnim();
		Text text = (Text)result.getChildWithName("scoreValue");
		text.setEnabled(e: false);
		Text text2 = (Text)result.getChildWithName("dataTitle");
		text2.setEnabled(e: false);
		Image.setElementPositionWithQuadOffset(text2, 118, 5);
		Text text3 = (Text)result.getChildWithName("dataValue");
		text3.setEnabled(e: false);
		result.playTimeline(0);
		result.setEnabled(e: true);
		stamp.setEnabled(e: false);
	}

	public virtual void levelLost()
	{
		boxAnim = 3;
		removeOpenCloseAnims();
		showCloseAnim();
	}

	public virtual void levelQuit()
	{
		shouldShowMachine = true;
		boxAnim = 4;
		result.setEnabled(e: false);
		removeOpenCloseAnims();
		showCloseAnim();
	}

	public virtual void postBoxClosed()
	{
		if (delegateboxClosed != null)
		{
			delegateboxClosed();
		}
		if (shouldShowConfetti)
		{
			showConfetti();
		}
	}

	public virtual void showOpenAnim()
	{
		showOpenCloseAnim(open: true);
	}

	public virtual void showCloseAnim()
	{
		showOpenCloseAnim(open: false);
	}

	public virtual BaseElement createConfettiParticleNear(Vector p)
	{
		Confetti confetti = Confetti.Confetti_createWithResID(114);
		confetti.doRestoreCutTransparency();
		int num = MathHelper.RND_RANGE(0, 2);
		int num2 = 18;
		int num3 = 26;
		switch (num)
		{
		case 1:
			num2 = 9;
			num3 = 17;
			break;
		case 2:
			num2 = 0;
			num3 = 8;
			break;
		}
		float num4 = MathHelper.RND_RANGE(-100, (int)FrameworkTypes.SCREEN_WIDTH);
		float num5 = MathHelper.RND_RANGE(-20, 50);
		float num6 = MathHelper.FLOAT_RND_RANGE(2, 5);
		int n = confetti.addAnimationDelayLoopFirstLast(0.05f, Timeline.LoopType.TIMELINE_REPLAY, num2, num3);
		confetti.ani = confetti.getTimeline(n);
		confetti.ani.playTimeline();
		confetti.ani.jumpToTrackKeyFrame(4, MathHelper.RND_RANGE(0, num3 - num2 - 1));
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num6));
		timeline.addKeyFrame(KeyFrame.makePos(num4, num5, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(num4, num5 + MathHelper.FLOAT_RND_RANGE(150, 250), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num6));
		timeline.addKeyFrame(KeyFrame.makeScale(0.0, 0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.3));
		timeline.addKeyFrame(KeyFrame.makeRotation(MathHelper.RND_RANGE(-360, 360), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline.addKeyFrame(KeyFrame.makeRotation(MathHelper.RND_RANGE(-360, 360), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num6));
		confetti.addTimeline(timeline);
		confetti.playTimeline(1);
		return confetti;
	}

	public virtual void removeOpenCloseAnims()
	{
		if (getChild(0) != null)
		{
			removeChild(openCloseAnims);
			openCloseAnims = null;
		}
		Text text = (Text)result.getChildWithName("dataTitle");
		Text text2 = (Text)result.getChildWithName("dataValue");
		Text text3 = (Text)result.getChildWithName("scoreValue");
		text.color.a = (text2.color.a = (text3.color.a = 1f));
	}

	public virtual void createOpenCloseAnims()
	{
		openCloseAnims = (BaseElement)new BaseElement().init();
		addChildwithID(openCloseAnims, 0);
		openCloseAnims.passTransformationsToChilds = true;
	}

	public virtual void showConfetti()
	{
		for (int i = 0; i < 70; i++)
		{
			confettiAnims.addChild(createConfettiParticleNear(MathHelper.vectZero));
		}
	}

	public override void update(float delta)
	{
		base.update(delta);
		if (boxAnim != 2)
		{
			return;
		}
		bool flag = Mover.moveVariableToTarget(ref raDelay, 0f, 1f, delta);
		switch (raState)
		{
		case -1:
		{
			cscore = 0;
			ctime = time;
			cstarBonus = starBonus;
			Text text7 = (Text)result.getChildWithName("scoreValue");
			text7.setString(NSObject.NSS(string.Concat(cscore)));
			Text text8 = (Text)result.getChildWithName("dataTitle");
			Image.setElementPositionWithQuadOffset(text8, 118, 5);
			text8.setString(Application.getString(1179670));
			Text text9 = (Text)result.getChildWithName("dataValue");
			text9.setString(NSObject.NSS(string.Concat(cstarBonus)));
			raState = 1;
			raDelay = 1f;
			break;
		}
		case 0:
			if (flag)
			{
				raState = 1;
				raDelay = 0.2f;
			}
			break;
		case 1:
		{
			Text text20 = (Text)result.getChildWithName("dataTitle");
			text20.setEnabled(e: true);
			Text text21 = (Text)result.getChildWithName("dataValue");
			text21.setEnabled(e: true);
			Text text22 = (Text)result.getChildWithName("scoreValue");
			text20.color.a = (text21.color.a = (text22.color.a = 1f - raDelay / 0.2f));
			if (flag)
			{
				raState = 2;
				raDelay = 1f;
			}
			break;
		}
		case 2:
		{
			cstarBonus = (int)((float)starBonus * raDelay);
			cscore = (int)((1.0 - (double)raDelay) * (double)starBonus);
			Text text14 = (Text)result.getChildWithName("dataValue");
			text14.setString(NSObject.NSS(string.Concat(cstarBonus)));
			Text text15 = (Text)result.getChildWithName("scoreValue");
			text15.setEnabled(e: true);
			text15.setString(NSObject.NSS(string.Concat(cscore)));
			if (flag)
			{
				raState = 3;
				raDelay = 0.2f;
			}
			break;
		}
		case 3:
		{
			Text text3 = (Text)result.getChildWithName("dataTitle");
			Text text4 = (Text)result.getChildWithName("dataValue");
			text3.color.a = (text4.color.a = raDelay / 0.2f);
			if (flag)
			{
				raState = 4;
				raDelay = 0.2f;
				int num = (int)Math.Floor(Math.Round(time) / 60.0);
				int num2 = (int)(Math.Round(time) - (double)num * 60.0);
				Text text5 = (Text)result.getChildWithName("dataTitle");
				text5.setString(Application.getString(1179669));
				Text text6 = (Text)result.getChildWithName("dataValue");
				text6.setString(NSObject.NSS(num + ":" + num2.ToString("D2")));
			}
			break;
		}
		case 4:
		{
			Text text18 = (Text)result.getChildWithName("dataTitle");
			Text text19 = (Text)result.getChildWithName("dataValue");
			text18.color.a = (text19.color.a = 1f - raDelay / 0.2f);
			if (flag)
			{
				raState = 5;
				raDelay = 1f;
			}
			break;
		}
		case 5:
		{
			ctime = time * raDelay;
			cscore = (int)((double)starBonus + (1.0 - (double)raDelay) * (double)timeBonus);
			int num3 = (int)Math.Floor(Math.Round(ctime) / 60.0);
			int num4 = (int)(Math.Round(ctime) - (double)num3 * 60.0);
			Text text16 = (Text)result.getChildWithName("dataValue");
			text16.setString(NSObject.NSS(num3 + ":" + num4.ToString("D2")));
			Text text17 = (Text)result.getChildWithName("scoreValue");
			text17.setString(NSObject.NSS(string.Concat(cscore)));
			if (flag)
			{
				raState = 6;
				raDelay = 0.2f;
			}
			break;
		}
		case 6:
		{
			Text text10 = (Text)result.getChildWithName("dataTitle");
			Text text11 = (Text)result.getChildWithName("dataValue");
			text10.color.a = (text11.color.a = raDelay / 0.2f);
			if (flag)
			{
				raState = 7;
				raDelay = 0.2f;
				Text text12 = (Text)result.getChildWithName("dataTitle");
				Image.setElementPositionWithQuadOffset(text12, 118, 7);
				text12.setString(Application.getString(1179671));
				Text text13 = (Text)result.getChildWithName("dataValue");
				text13.setString(NSObject.NSS(""));
			}
			break;
		}
		case 7:
		{
			Text text = (Text)result.getChildWithName("dataTitle");
			Text text2 = (Text)result.getChildWithName("dataValue");
			text.color.a = (text2.color.a = 1f - raDelay / 0.2f);
			if (flag)
			{
				raState = 8;
				if (shouldShowImprovedResult)
				{
					stamp.setEnabled(e: true);
					stamp.playTimeline(0);
				}
			}
			break;
		}
		}
	}

	public virtual void showOpenCloseAnim(bool open)
	{
		createOpenCloseAnims();
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		baseElement.parentAnchor = (baseElement.anchor = 9);
		baseElement.width = (int)FrameworkTypes.SCREEN_WIDTH;
		baseElement.height = (int)FrameworkTypes.SCREEN_HEIGHT;
		openCloseAnims.addChild(baseElement);
		ScrollableContainer scrollableContainer = new ScrollableContainer().initWithWidthHeightContainerWidthHeight(FrameworkTypes.SCREEN_WIDTH_EXPANDED, FrameworkTypes.SCREEN_HEIGHT_EXPANDED, FrameworkTypes.SCREEN_WIDTH_EXPANDED, FrameworkTypes.SCREEN_HEIGHT_EXPANDED);
		scrollableContainer.parentAnchor = (scrollableContainer.anchor = 9);
		scrollableContainer.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		scrollableContainer.x -= FrameworkTypes.SCREEN_OFFSET_X;
		scrollableContainer.touchable = false;
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
		image2.y = FrameworkTypes.SCREEN_HEIGHT_EXPANDED + 10f;
		image2.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		image2.passTransformationsToChilds = false;
		image2.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		scrollableContainer.addChild(image);
		image.parentAnchor = -1;
		baseElement.addChild(scrollableContainer);
		baseElement.addChild(image2);
		if (shouldShowMachine)
		{
			BaseElement baseElement2 = (BaseElement)new BaseElement().init();
			baseElement2.parentAnchor = 9;
			baseElement2.width = (int)FrameworkTypes.SCREEN_WIDTH;
			baseElement2.height = (int)FrameworkTypes.SCREEN_HEIGHT;
			baseElement.addChild(baseElement2);
			Image image3 = Image.Image_createWithResIDQuad(r, 2);
			image3.parentAnchor = 9;
			image3.doRestoreCutTransparency();
			baseElement2.addChild(image3);
			if (machineEndState)
			{
				Image image4 = Image.Image_createWithResIDQuad(r, 3);
				image4.parentAnchor = 9;
				image4.doRestoreCutTransparency();
				baseElement2.addChild(image4);
				Image image5 = Image.Image_createWithResIDQuad(r, 7);
				image5.parentAnchor = 9;
				image5.doRestoreCutTransparency();
				baseElement2.addChild(image5);
			}
			Image image6 = Image.Image_createWithResIDQuad(r, 1);
			image6.parentAnchor = 9;
			image6.doRestoreCutTransparency();
			baseElement2.addChild(image6);
			if (machineEndState)
			{
				Image image7 = Image.Image_createWithResIDQuad(r, 5);
				image7.parentAnchor = 9;
				image7.doRestoreCutTransparency();
				baseElement2.addChild(image7);
			}
			Image image8 = Image.Image_createWithResIDQuad(r, 6);
			image8.parentAnchor = 9;
			image8.doRestoreCutTransparency();
			baseElement2.addChild(image8);
			Animation animation = Animation.Animation_createWithResIDQuad(r, 8);
			animation.parentAnchor = 9;
			animation.doRestoreCutTransparency();
			animation.addAnimationDelayLoopFirstLast(0.05f, Timeline.LoopType.TIMELINE_REPLAY, 8, 16);
			baseElement2.addChild(animation);
		}
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		if (open)
		{
			timeline.addKeyFrame(KeyFrame.makePos(0, 0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0f));
			timeline.addKeyFrame(KeyFrame.makePos(0.0, 0f - FrameworkTypes.SCREEN_HEIGHT - 50f, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.800000011920929));
		}
		else
		{
			timeline.addKeyFrame(KeyFrame.makePos(0.0, 0f - FrameworkTypes.SCREEN_HEIGHT - 50f, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.0));
			timeline.addKeyFrame(KeyFrame.makePos(0, 0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.8f));
		}
		timeline.delegateTimelineDelegate = this;
		baseElement.addTimeline(timeline);
		baseElement.playTimeline(0);
		shouldShowMachine = false;
		machineEndState = false;
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}

	public virtual void timelineFinished(Timeline t)
	{
		switch (boxAnim)
		{
		case 0:
		case 1:
			NSTimer.registerDelayedObjectCall(selector_removeOpenCloseAnims, this, 0.001);
			if (result.isEnabled())
			{
				confettiAnims.removeAllChilds();
				result.setEnabled(e: false);
			}
			break;
		case 4:
		{
			ViewController currentController = Application.sharedRootController().getCurrentController();
			currentController.deactivate();
			break;
		}
		case 2:
			NSTimer.registerDelayedObjectCall(selector_postBoxClosed, this, 0.001);
			break;
		case 3:
			break;
		}
	}

	private static void selector_removeOpenCloseAnims(NSObject obj)
	{
		((BoxOpenClose)obj).removeOpenCloseAnims();
	}

	private static void selector_postBoxClosed(NSObject obj)
	{
		((BoxOpenClose)obj).postBoxClosed();
	}
}
