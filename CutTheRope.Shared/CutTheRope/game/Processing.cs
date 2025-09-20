using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class Processing : RectangleElement, TimelineDelegate
{
	private bool blockTouches = true;

	private static NSObject createWithLoading()
	{
		return new Processing().initWithLoading(loading: true);
	}

	public virtual NSObject initWithLoading(bool loading)
	{
		if (init() != null)
		{
			width = (int)FrameworkTypes.SCREEN_WIDTH_EXPANDED;
			height = (int)FrameworkTypes.SCREEN_HEIGHT_EXPANDED + 1;
			x = 0f - FrameworkTypes.SCREEN_OFFSET_X;
			y = 0f - FrameworkTypes.SCREEN_OFFSET_Y;
			blendingMode = 0;
			if (loading)
			{
				Image image = Image.Image_createWithResIDQuad(91, 0);
				Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
				timeline.addKeyFrame(KeyFrame.makeRotation(0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
				timeline.addKeyFrame(KeyFrame.makeRotation(360, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1f));
				timeline.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
				image.addTimeline(timeline);
				image.playTimeline(0);
				Text c = Text.createWithFontandString(5, Application.getString(1179697));
				HBox hBox = new HBox().initWithOffsetAlignHeight(10f, 16, image.height);
				hBox.parentAnchor = (hBox.anchor = 18);
				addChild(hBox);
				hBox.addChild(image);
				hBox.addChild(c);
			}
			Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0f));
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 0.4), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
			addTimeline(timeline2);
			timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline2.delegateTimelineDelegate = this;
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 0.4), KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0f));
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
			addTimeline(timeline2);
			playTimeline(0);
		}
		return this;
	}

	public virtual NSObject initWithTouchesBlocking(bool b)
	{
		if (init() != null)
		{
			width = (int)FrameworkTypes.SCREEN_WIDTH;
			height = (int)FrameworkTypes.SCREEN_HEIGHT;
			blendingMode = 0;
			Image image = Image.Image_createWithResIDQuad(91, 0);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeRotation(0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
			timeline.addKeyFrame(KeyFrame.makeRotation(360, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1f));
			timeline.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
			image.addTimeline(timeline);
			image.playTimeline(0);
			Text text = Text.createWithFontandString(5, Application.getString(1179679));
			HBox hBox = new HBox().initWithOffsetAlignHeight(10f, 16, image.height);
			hBox.parentAnchor = (hBox.anchor = 18);
			addChild(hBox);
			hBox.addChild(image);
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_IT)
			{
				text = new Text().initWithFont(Application.getFont(5));
				text.setStringandWidth(Application.getString(1179679), 120f);
				hBox.x -= 15f;
			}
			hBox.addChild(text);
			blockTouches = b;
			color = RGBAColor.transparentRGBA;
			if (blockTouches)
			{
				Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
				timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0f));
				timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.0, 0.0, 0.0, 0.4), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
				addTimeline(timeline2);
				playTimeline(0);
			}
		}
		return this;
	}

	public override bool onTouchDownXY(float tx, float ty)
	{
		bool result = base.onTouchDownXY(tx, ty);
		if (blockTouches)
		{
			return true;
		}
		return result;
	}

	public override bool onTouchUpXY(float tx, float ty)
	{
		bool result = base.onTouchUpXY(tx, ty);
		if (blockTouches)
		{
			return true;
		}
		return result;
	}

	public override bool onTouchMoveXY(float tx, float ty)
	{
		bool result = base.onTouchMoveXY(tx, ty);
		if (blockTouches)
		{
			return true;
		}
		return result;
	}

	public override void playTimeline(int t)
	{
		if (t == 0)
		{
			setEnabled(e: true);
		}
		base.playTimeline(t);
	}

	public void timelineFinished(Timeline t)
	{
		setEnabled(e: false);
	}

	public void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}
}
