using System;
using System.Collections.Generic;
using System.Net;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

internal class CoppaController : ViewController, ButtonDelegate, TimelineDelegate
{
	private Button okb;

	private Rollbar roll;

	private static string COPPA_URL = "http://coppa.zeptodev.com/?";

	private static int speedAccelerator = 2;

	private static int blankSpaceTop = 2;

	private static int blankSpaceBottom = 1;

	private static int minAge = 1;

	private static int maxAge = 99;

	private static int defaultIdx = maxAge / 4;

	private static float friction = 5f;

	private static float minFriction = 0.7f;

	private static float cellBounceSpeed = 3f;

	private static float boundReturnSpeed = 20f;

	private static int COPPA_VIEW_MAIN = 300;

	private static int COPPA_BUTTON_OK = 0;

	private static int COPPA_BUTTON_PRIVACY = 1;

	private new void addViewwithID(View view, int n)
	{
		base.addViewwithID(view, n);
	}

	public override NSObject initWithParent(ViewController p)
	{
		base.initWithParent(p);
		float num = FrameworkTypes.SCREEN_HEIGHT * 0.03f;
		CoppaView coppaView = (CoppaView)new CoppaView().initFullscreen();
		addViewwithID(coppaView, COPPA_VIEW_MAIN);
		coppaView.blendingMode = 1;
		Image image = Image.Image_createWithResIDQuad(312, 0);
		image.parentAnchor = (image.anchor = 18);
		image.passTransformationsToChilds = false;
		image.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
		image.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
		coppaView.addChild(image);
		okb = MenuController.createShortButtonWithTextIDDelegate(Application.getString(1179681), COPPA_BUTTON_OK, this);
		Image.setElementPositionWithQuadCenter(okb, 310, 8);
		coppaView.addChild(okb);
		Vector vector = MathHelper.vectMult(Image.getQuadSize(310, 10), 0.2f);
		Text text = Text.createWithFontandString(5, Application.getString(1179735));
		text.setAlignment(2);
		text.scaleX /= 2.2f;
		text.scaleY /= 2.2f;
		Image.setElementPositionWithQuadCenter(text, 310, 10);
		text.y += (float)((double)vector.y * 1.1 * 0.5);
		text.anchor = 18;
		coppaView.addChild(text);
		Image image2 = Image.Image_createWithResIDQuad(310, 5);
		image2.setName("baloon");
		Image.setElementPositionWithQuadCenter(image2, 310, 5);
		coppaView.addChild(image2);
		string text2 = Application.getString(1179736).ToString();
		string[] array = text2.Split('\n');
		Text[] array2 = new Text[array.Length];
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		image2.y = (float)((double)FrameworkTypes.SCREEN_HEIGHT - 0.75 * (double)FrameworkTypes.SCREEN_HEIGHT) + num;
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			array[num2] = array[num2].Replace('\n', ' ');
			array2[num2] = new Text().initWithFont(Application.getFont(6));
			array2[num2].setString(array[num2]);
			array2[num2].y = image2.y - 40f + (float)(num2 * 20);
			array2[num2].x = image2.x - 10f;
			array2[num2].color = RGBAColor.blackRGBA;
			array2[num2].setAlignment(2);
			array2[num2].anchor = 18;
			array2[num2].scaleX /= 1.5f;
			array2[num2].scaleY /= 1.5f;
			baseElement.addChild(array2[num2]);
		}
		baseElement.setName(NSObject.NSS("baloonText"));
		coppaView.addChild(baseElement);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(7);
		timeline.addKeyFrame(KeyFrame.makePos(image2.x, image2.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(image2.x + 3f, image2.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
		timeline.addKeyFrame(KeyFrame.makePos(image2.x - 2f, image2.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
		timeline.addKeyFrame(KeyFrame.makePos(image2.x + 2f, image2.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
		timeline.addKeyFrame(KeyFrame.makePos(image2.x + -3f, image2.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
		timeline.addKeyFrame(KeyFrame.makePos(image2.x + 3f, image2.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
		timeline.addKeyFrame(KeyFrame.makePos(image2.x, image2.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
		image2.addTimeline(timeline);
		roll = new Rollbar().Create();
		roll.setName("agePicker");
		Image.setElementPositionWithQuadOffset(roll, 310, 0);
		roll.x -= FrameworkTypes.SCREEN_OFFSET_X;
		roll.y -= FrameworkTypes.SCREEN_OFFSET_Y;
		roll.scrollWithSpeed(-16300f);
		coppaView.addChild(roll);
		okb.x = FrameworkTypes.SCREEN_WIDTH / 2f;
		Button button = new Button().initWithUpElementDownElementandID(Image.Image_createWithResIDQuad(310, 6), Image.Image_createWithResIDQuad(310, 7), COPPA_BUTTON_PRIVACY);
		button.delegateButtonDelegate = this;
		button.y = FrameworkTypes.SCREEN_HEIGHT - 40f;
		button.x += 20f;
		coppaView.addChild(button);
		return this;
	}

	public void onButtonPressed(int a)
	{
		CTRSoundMgr._playSound(19);
		if (a == COPPA_BUTTON_OK)
		{
			if (!ageValid())
			{
				BaseElement childWithName = activeView().getChildWithName(NSObject.NSS("baloon"));
				BaseElement childWithName2 = activeView().getChildWithName(NSObject.NSS("baloonText"));
				Rollbar rollbar = (Rollbar)activeView().getChildWithName(NSObject.NSS("agePicker"));
				if (!(Math.Abs(rollbar.getOffsetY()) > 1f))
				{
					childWithName.playTimeline(0);
					childWithName2.playTimeline(0);
					rollbar.scrollWithSpeed(100f);
				}
				return;
			}
			CTRPreferences cTRPreferences = Application.sharedPreferences();
			cTRPreferences.setCoppaShowed(b: true);
			int num = getSelectedAge();
			cTRPreferences.setCoppaRestricted(num < 13);
			if (num < 13)
			{
				FlurryAPI.enabled = false;
				num = -1;
			}
			trackCoppaParams(num);
			cTRPreferences.setUserAge(num);
			deactivate();
		}
		else if (a == COPPA_BUTTON_PRIVACY)
		{
			FlurryAPI.logEvent("COPSCR_PRIVACY_PRESSED", new List<string> { "" });
			AndroidAPI.openUrl(Application.getString(1179737));
		}
	}

	public void timelineFinished(Timeline tl)
	{
	}

	public void timelinereachedKeyFramewithIndex(Timeline tl, KeyFrame kf, int a)
	{
	}

	public override void showView(int n)
	{
		base.showView(n);
	}

	private bool ageValid()
	{
		if (getSelectedAge() >= minAge)
		{
			return getSelectedAge() <= maxAge;
		}
		return false;
	}

	private int getSelectedAge()
	{
		return roll.getIndex() + 1;
	}

	public override void activate()
	{
		base.activate();
		showView(COPPA_VIEW_MAIN);
	}

	public override void update(float delta)
	{
		base.update(delta);
		okb.color = new RGBAColor(1f, 1f, 1f, (float)((ageValid() ? 1 : 0) + 1) * 0.5f);
	}

	public void trackCoppaParams(int age)
	{
		string requestUriString = $"{COPPA_URL}age={age}&app={getAppName()}";
		WebRequest.Create(requestUriString);
	}

	public NSString getAppName()
	{
		return NSObject.NSS("ctr_exp");
	}
}
