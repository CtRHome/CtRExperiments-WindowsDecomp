using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class LoadingController : ViewController, ResourceMgrDelegate, TimelineDelegate
{
	private enum ViewID
	{
		VIEW_LOADING
	}

	private BaseElement main;

	private Image lampOn;

	private Image lampOff;

	private Image lamp2On;

	private BaseElement container;

	private float startContainerY;

	private Animation keyAnim;

	public int nextController;

	public int MusicToLoad = -1;

	public override NSObject initWithParent(ViewController p)
	{
		if (base.initWithParent(p) != null)
		{
			LoadingView loadingView = (LoadingView)new LoadingView().initFullscreen();
			loadingView.blendingMode = 1;
			addViewwithID(loadingView, 0);
			int num = 7;
			Image image = Image.Image_createWithResID(252);
			image.passTransformationsToChilds = false;
			image.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
			image.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
			image.parentAnchor = (image.anchor = 9);
			image.y -= FrameworkTypes.SCREEN_OFFSET_Y;
			Image image2 = Image.Image_createWithResIDQuad(num, 0);
			image2.parentAnchor = 12;
			image2.anchor = 20;
			image2.y = FrameworkTypes.SCREEN_HEIGHT * FrameworkTypes.SCREEN_BG_SCALE_Y + 10f;
			image2.passTransformationsToChilds = false;
			image2.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
			image.addChild(image2);
			loadingView.addChild(image);
			main = (BaseElement)new BaseElement().init();
			main.width = (int)FrameworkTypes.SCREEN_WIDTH;
			main.height = (int)FrameworkTypes.SCREEN_HEIGHT;
			loadingView.addChild(main);
			lampOff = Image.Image_createWithResIDQuad(num, 2);
			lampOff.doRestoreCutTransparency();
			main.addChild(lampOff);
			lampOn = Image.Image_createWithResIDQuad(num, 3);
			lampOn.color = RGBAColor.transparentRGBA;
			lampOn.doRestoreCutTransparency();
			main.addChild(lampOn);
			lamp2On = Image.Image_createWithResIDQuad(num, 7);
			lamp2On.color = RGBAColor.transparentRGBA;
			lamp2On.doRestoreCutTransparency();
			main.addChild(lamp2On);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
			lampOn.addTimeline(timeline);
			Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(3);
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.05));
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
			lamp2On.addTimeline(timeline2);
			timeline2.delegateTimelineDelegate = this;
			Image image3 = Image.Image_createWithResIDQuad(num, 1);
			image3.doRestoreCutTransparency();
			main.addChild(image3);
			Image image4 = Image.Image_createWithResIDQuad(num, 5);
			image4.doRestoreCutTransparency();
			container = (BaseElement)new BaseElement().init();
			Vector quadSize = Image.getQuadSize(num, 5);
			container.width = (int)quadSize.x;
			container.height = (int)quadSize.y;
			Vector quadOffset = Image.getQuadOffset(num, 5);
			container.x = quadOffset.x;
			container.y = quadOffset.y + (float)container.height;
			main.addChild(container);
			startContainerY = container.y;
			Image image5 = Image.Image_createWithResIDQuad(7, 4);
			image5.parentAnchor = 10;
			image5.anchor = 18;
			container.addChild(image5);
			ScrollableContainer scrollableContainer = new ScrollableContainer().initWithWidthHeightContainerWidthHeight(image4.width, image4.height, image4.width, image4.height);
			scrollableContainer.parentAnchor = (scrollableContainer.anchor = 9);
			scrollableContainer.addChild(image4);
			image4.parentAnchor = -1;
			container.addChild(scrollableContainer);
			Image image6 = Image.Image_createWithResIDQuad(num, 6);
			image6.doRestoreCutTransparency();
			main.addChild(image6);
			keyAnim = Animation.Animation_createWithResIDQuad(num, 8);
			keyAnim.doRestoreCutTransparency();
			keyAnim.addAnimationDelayLoopFirstLast(0.05f, Timeline.LoopType.TIMELINE_REPLAY, 8, 16);
			main.addChild(keyAnim);
			Text text = new Text().initWithFont(Application.getFont(5));
			text.setAlignment(2);
			text.setString(Application.getString(1179679));
			Image.setElementPositionWithQuadCenter(text, 7, 17);
			main.addChild(text);
		}
		return this;
	}

	public override void activate()
	{
		AndroidAPI.showBanner();
		base.activate();
		LoadingView loadingView = (LoadingView)getView(0);
		loadingView.game = nextController == 0;
		showView(0);
		Vector quadOffset = Image.getQuadOffset(7, 5);
		container.y = quadOffset.y + (float)container.height;
		lampOn.color = RGBAColor.transparentRGBA;
		lamp2On.color = RGBAColor.transparentRGBA;
		keyAnim.playTimeline(0);
	}

	public override void update(float tt)
	{
		base.update(tt);
		float num = (float)Application.sharedResourceMgr().getPercentLoaded() / 100f;
		container.y = startContainerY - (float)container.height * num;
	}

	public virtual void resourceLoaded(int res)
	{
	}

	public virtual void allResourcesLoaded()
	{
		if (MusicToLoad > 0)
		{
			CTRSoundMgr._playMusic(MusicToLoad);
			CTRSoundMgr._stopMusic();
			MusicToLoad = -1;
		}
		if (nextController == 0 || nextController == 1)
		{
			AndroidAPI.hideBanner();
		}
		if (keyAnim.getCurrentTimeline() != null)
		{
			keyAnim.stopCurrentTimeline();
		}
		lampOn.playTimeline(0);
		lamp2On.playTimeline(0);
	}

	public virtual void timelineFinished(Timeline t)
	{
		deactivate();
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int n)
	{
	}
}
