using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.sfe;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class Snail : GameObject, TimelineDelegate
{
	public const int SNAIL_STATE_INACTIVE = 0;

	public const int SNAIL_STATE_ACTIVE = 1;

	public const int SNAIL_STATE_VANISH = 2;

	public const int SNAIL_STATE_VANISHED = 3;

	private const string SNAIL_ACTION_DETACH = "SNAIL_ACTION_DETACH";

	private BaseElement backContainer;

	private ConstraintedPoint point;

	public float startRotation;

	private static Snail Snail_create(Texture2D t)
	{
		return (Snail)new Snail().initWithTexture(t);
	}

	private static Snail Snail_createWithResID(int r)
	{
		return Snail_create(Application.getTexture(r));
	}

	public static Snail Snail_createWithResIDQuad(int r, int q)
	{
		Snail snail = Snail_create(Application.getTexture(r));
		snail.setDrawQuad(q);
		return snail;
	}

	public virtual void attachToPoint(ConstraintedPoint p)
	{
		point = p;
		state = 1;
		BaseElement childWithName = backContainer.getChildWithName(NSObject.NSS("sleepy_eyes"));
		childWithName.setEnabled(e: false);
		BaseElement childWithName2 = backContainer.getChildWithName(NSObject.NSS("wake_up"));
		childWithName2.setEnabled(e: true);
		childWithName2.playTimeline(0);
		stopCurrentTimeline();
		int num = Preferences._getIntForKey("PREFS_GRAB_SNAILS") + 1;
		Preferences._setIntforKey(num, "PREFS_GRAB_SNAILS", comit: false);
		if (num >= 100)
		{
			CTRRootController.postAchievementName(NSObject.NSS("acSnailTamer"));
		}
		CTRSoundMgr._playSound(256);
	}

	public virtual void detach()
	{
		point = null;
		state = 2;
		backContainer.getChildWithName(NSObject.NSS("eye1")).setEnabled(e: false);
		backContainer.getChildWithName(NSObject.NSS("eye2")).setEnabled(e: false);
		BaseElement childWithName = backContainer.getChildWithName(NSObject.NSS("sleep"));
		childWithName.setEnabled(e: true);
		childWithName.playTimeline(0);
		BaseElement childWithName2 = backContainer.getChildWithName(NSObject.NSS("wake_up"));
		childWithName2.setEnabled(e: false);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline.addKeyFrame(KeyFrame.makePos(x, y, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(x, (double)y - 50.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
		timeline.addKeyFrame(KeyFrame.makePos(x, y + FrameworkTypes.SCREEN_HEIGHT, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 2.1));
		timeline.addKeyFrame(KeyFrame.makeRotation(0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeRotation(MathHelper.RND_RANGE(-120, 120), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 2.4));
		timeline.addKeyFrame(KeyFrame.makeSingleAction(this, "SNAIL_ACTION_DETACH", 0, 0, 2.4));
		int t = addTimeline(timeline);
		Track track = timeline.getTrack(Track.TrackType.TRACK_ROTATION);
		track.relative = true;
		playTimeline(t);
		CTRSoundMgr._playSound(257);
	}

	public virtual void setBBFromQuad(int quad)
	{
		bb = FrameworkTypes.MakeRectangle(MathHelper.round(texture.quadOffsets[quad].x), MathHelper.round(texture.quadOffsets[quad].y), texture.quadRects[quad].w, texture.quadRects[quad].h);
		rbb = Quad2D.MakeQuad2D(bb.x, bb.y, bb.w, bb.h);
	}

	public override Image initWithTexture(Texture2D t)
	{
		if (base.initWithTexture(t) != null)
		{
			doRestoreCutTransparency();
			setBBFromQuad(8);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
			float num = (float)MathHelper.RND_RANGE(9, 10) / 10f;
			float time = (float)MathHelper.RND_RANGE(0, 6) / 10f;
			timeline.addKeyFrame(KeyFrame.makeScale(num, num, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0f));
			timeline.addKeyFrame(KeyFrame.makeScale(1f, 1f, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, time));
			timeline.addKeyFrame(KeyFrame.makeSingleAction(this, "ACTION_PLAY_TIMELINE", 1, 1, time));
			addTimeline(timeline);
			Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(5);
			timeline2.addKeyFrame(KeyFrame.makeScale(1f, 1f, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0f));
			timeline2.addKeyFrame(KeyFrame.makeScale(0.9, 0.9, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
			timeline2.addKeyFrame(KeyFrame.makeScale(0.9, 0.9, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.3));
			timeline2.addKeyFrame(KeyFrame.makeScale(1f, 1f, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 1f));
			timeline2.addKeyFrame(KeyFrame.makeScale(1.0, 1.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
			timeline2.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
			addTimeline(timeline2);
			playTimeline(0);
			backContainer = new BaseElement();
			backContainer.init();
			backContainer.anchor = 18;
			backContainer.width = width;
			backContainer.height = height;
			Image image = Image.Image_createWithResIDQuad(262, 2);
			image.setName(NSObject.NSS("sleepy_eyes"));
			image.doRestoreCutTransparency();
			image.parentAnchor = (image.anchor = 9);
			backContainer.addChild(image);
			Image image2 = Image.Image_createWithResIDQuad(262, 6);
			image2.setName(NSObject.NSS("eye1"));
			image2.doRestoreCutTransparency();
			image2.parentAnchor = 9;
			image2.setEnabled(e: false);
			backContainer.addChild(image2);
			Image image3 = Image.Image_createWithResIDQuad(262, 7);
			image3.setName(NSObject.NSS("eye2"));
			image3.doRestoreCutTransparency();
			image3.parentAnchor = 9;
			image3.setEnabled(e: false);
			backContainer.addChild(image3);
			Animation animation = Animation.Animation_createWithResIDQuad(262, 3);
			animation.parentAnchor = 9;
			animation.setEnabled(e: false);
			animation.setName(NSObject.NSS("wake_up"));
			animation.doRestoreCutTransparency();
			animation.addAnimationDelayLoopFirstLast(0.1f, Timeline.LoopType.TIMELINE_NO_LOOP, 3, 5);
			Timeline timeline3 = animation.getTimeline(0);
			timeline3.delegateTimelineDelegate = this;
			backContainer.addChild(animation);
			Animation animation2 = Animation.Animation_createWithResIDQuad(262, 5);
			animation2.parentAnchor = 9;
			animation2.setEnabled(e: false);
			animation2.setName(NSObject.NSS("sleep"));
			animation2.doRestoreCutTransparency();
			animation2.addAnimationDelayLoopFirstLast(0.1f, Timeline.LoopType.TIMELINE_NO_LOOP, 5, 3);
			animation2.playTimeline(0);
			Timeline timeline4 = animation2.getTimeline(0);
			timeline4.delegateTimelineDelegate = this;
			backContainer.addChild(animation2);
			state = 0;
		}
		return this;
	}

	public override void dealloc()
	{
		NSObject.NSREL(backContainer);
		base.dealloc();
	}

	public override void draw()
	{
		backContainer.draw();
		base.draw();
	}

	public override void update(float delta)
	{
		base.update(delta);
		backContainer.update(delta);
		if (point != null)
		{
			x = point.pos.x;
			y = point.pos.y;
		}
		backContainer.x = x;
		backContainer.y = y;
		backContainer.color = color;
		backContainer.scaleX = scaleX;
		backContainer.scaleY = scaleY;
		backContainer.rotation = rotation;
	}

	public override bool handleAction(ActionData a)
	{
		if (!base.handleAction(a))
		{
			if (!(a.actionName == "SNAIL_ACTION_DETACH"))
			{
				return false;
			}
			state = 3;
		}
		return true;
	}

	public virtual void timelineFinished(Timeline t)
	{
		if (t.element.name.isEqualToString(NSObject.NSS("wake_up")))
		{
			backContainer.getChildWithName(NSObject.NSS("eye1")).setEnabled(e: true);
			backContainer.getChildWithName(NSObject.NSS("eye2")).setEnabled(e: true);
			t.element.setEnabled(e: false);
		}
		else if (t.element.name.isEqualToString(NSObject.NSS("sleep")))
		{
			backContainer.getChildWithName(NSObject.NSS("sleepy_eyes")).setEnabled(e: true);
			t.element.setEnabled(e: false);
		}
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}
}
