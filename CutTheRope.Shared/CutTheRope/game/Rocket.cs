using System;
using CutTheRope.ctr_original;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.sfe;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class Rocket : CTRGameObject, TimelineDelegate
{
	public const int STATE_ROCKET_IDLE = 0;

	public const int STATE_ROCKET_DIST = 1;

	public const int STATE_ROCKET_FLY = 2;

	public const int STATE_ROCKET_EXAUST = 3;

	private new const int MIN_CICRLE_POINTS = 10;

	private Vector lastTouch;

	public ConstraintedPoint point;

	public float angle;

	private Vector t1;

	private Vector t2;

	public float time;

	public float impulse;

	public float impulseFactor;

	public float startCandyRotation;

	public float startRotation;

	public int isOperating;

	public bool isRotatable;

	public bool rotateHandled;

	public float anglePercent;

	public float additionalAngle;

	public bool perp;

	public bool perpSetted;

	public Bungee activeBungee;

	public Animation sparks;

	public BaseElement container;

	public AnimationsPool aniPool;

	public RocketSparks particles;

	public RocketClouds cloudParticles;

	public RocketDelegate delegateRocketDelegate;

	private static Rocket Rocket_create(Texture2D t)
	{
		return (Rocket)new Rocket().initWithTexture(t);
	}

	public static Rocket Rocket_createWithResIDQuad(int r, int q)
	{
		Rocket rocket = Rocket_create(Application.getTexture(r));
		rocket.setDrawQuad(q);
		return rocket;
	}

	public override Image initWithTexture(Texture2D tx)
	{
		if (base.initWithTexture(tx) != null)
		{
			isOperating = -1;
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			addTimeline(timeline);
			timeline.addKeyFrame(KeyFrame.makeRotation(0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
			timeline.addKeyFrame(KeyFrame.makeRotation(45.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
			timeline.delegateTimelineDelegate = this;
			Track track = timeline.getTrack(Track.TrackType.TRACK_ROTATION);
			track.relative = true;
			timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			addTimeline(timeline);
			timeline.addKeyFrame(KeyFrame.makeRotation(0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
			timeline.addKeyFrame(KeyFrame.makeRotation(-45.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
			timeline.delegateTimelineDelegate = this;
			track = timeline.getTrack(Track.TrackType.TRACK_ROTATION);
			track.relative = true;
			timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeScale(0.7, 0.7, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeScale(0.0, 0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
			timeline.delegateTimelineDelegate = this;
			addTimeline(timeline);
			point = (ConstraintedPoint)new ConstraintedPoint().init();
			point.disableGravity = true;
			point.setWeight(0.5f);
			container = (BaseElement)new BaseElement().init();
			container.width = width;
			container.height = height;
			container.anchor = 18;
			sparks = Animation.Animation_createWithResID(154);
			sparks.parentAnchor = (sparks.anchor = 18);
			sparks.setEnabled(e: false);
			sparks.doRestoreCutTransparency();
			sparks.addAnimationDelayLoopFirstLast(0.1f, Timeline.LoopType.TIMELINE_REPLAY, 1, 4);
			container.addChild(sparks);
			sparks.blendingMode = 2;
			blendingMode = 1;
			sparks.scaleX = (sparks.scaleY = 0.7f);
		}
		return this;
	}

	public override void dealloc()
	{
		NSObject.NSREL(container);
		NSObject.NSREL(point);
		base.dealloc();
	}

	public override void update(float delta)
	{
		base.update(delta);
		point.update(delta);
		container.update(delta);
		if (mover != null && !mover.paused)
		{
			point.pos.x = x;
			point.pos.y = y;
		}
		else
		{
			x = point.pos.x;
			y = point.pos.y;
		}
		container.rotation = rotation;
		container.x = x;
		container.y = y;
		float a = MathHelper.vectLength(MathHelper.vectSub(point.prevPos, point.pos));
		a = MathHelper.MAX(a, 1f);
		float initialAngle = (float)((double)angle - Math.PI);
		Vector v = MathHelper.vect(x, y);
		v = MathHelper.vectAdd(v, MathHelper.vectMult(MathHelper.vectForAngle(angle), 35f));
		if (particles != null)
		{
			particles.x = v.x;
			particles.y = v.y;
			particles.angle = rotation;
			particles.initialAngle = initialAngle;
			particles.speed = a * 50f;
		}
		if (cloudParticles != null)
		{
			cloudParticles.x = v.x;
			cloudParticles.y = v.y;
			cloudParticles.angle = rotation;
			cloudParticles.initialAngle = initialAngle;
			cloudParticles.speed = a * 40f;
		}
	}

	public override void parseMover(XMLNode xml)
	{
		NSString nSString = xml["path"];
		if (nSString != null && nSString.length() != 0)
		{
			int l = 100;
			if (nSString.characterAtIndex(0) == 'R')
			{
				NSString nSString2 = nSString.substringFromIndex(2);
				int num = nSString2.intValue();
				l = MathHelper.MAX(11, num / 2 + 1);
			}
			float num2 = xml["moveSpeed"].floatValue();
			float m_ = num2;
			float r_ = xml["rotateSpeed"].floatValue();
			CTRMover cTRMover = (CTRMover)new CTRMover().initWithPathCapacityMoveSpeedRotateSpeed(l, m_, r_);
			cTRMover.angle = rotation;
			cTRMover.setPathFromStringandStart(nSString, MathHelper.vect(x, y));
			setMover(cTRMover);
			cTRMover.start();
		}
	}

	public override void draw()
	{
		container.draw();
		base.draw();
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}

	public virtual void timelineFinished(Timeline t)
	{
		rotateWithBB(rotation);
		if (getTimeline(2) == t && delegateRocketDelegate != null)
		{
			delegateRocketDelegate.exhausted(this);
		}
	}

	public virtual void updateRotation()
	{
		t1.x = x - bb.w / 2f;
		t2.x = x + bb.w / 2f;
		t1.y = (t2.y = y);
		angle = MathHelper.DEGREES_TO_RADIANS(rotation);
		t1 = MathHelper.vectRotateAround(t1, angle, x, y);
		t2 = MathHelper.vectRotateAround(t2, angle, x, y);
	}

	private float getRotateAngleForStartEndCenter(Vector v1, Vector v2, Vector c)
	{
		Vector v3 = MathHelper.vectSub(v1, c);
		Vector v4 = MathHelper.vectSub(v2, c);
		float r = MathHelper.vectAngleNormalized(v4) - MathHelper.vectAngleNormalized(v3);
		return MathHelper.RADIANS_TO_DEGREES(r);
	}

	public virtual void handleTouch(Vector v)
	{
		lastTouch = v;
	}

	public virtual void handleRotate(Vector v)
	{
		float rotateAngleForStartEndCenter = getRotateAngleForStartEndCenter(lastTouch, v, MathHelper.vect(x, y));
		rotateAngleForStartEndCenter = MathHelper.angleTo0_360(rotateAngleForStartEndCenter);
		rotation += rotateAngleForStartEndCenter;
		lastTouch = v;
		rotateHandled = true;
		rotateWithBB(rotation);
	}

	public virtual void handleRotateFinal(Vector v)
	{
		rotation = MathHelper.angleTo0_360(rotation);
		float num = MathHelper.round(rotation / 45f);
		float num2 = 45f * num;
		removeTimeline(1);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeRotation(rotation, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeRotation(num2, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
		timeline.delegateTimelineDelegate = this;
		addTimeline(timeline);
		playTimeline(1);
	}

	public virtual void startAnimation()
	{
		sparks.setEnabled(e: true);
		sparks.playTimeline(0);
	}

	public virtual void stopAnimation()
	{
		playTimeline(2);
		Timeline timeline = sparks.getCurrentTimeline();
		if (timeline != null && timeline.state == Timeline.TimelineState.TIMELINE_PLAYING)
		{
			sparks.stopCurrentTimeline();
		}
		sparks.setEnabled(e: false);
		if (particles != null)
		{
			particles.stopSystem();
		}
		if (cloudParticles != null)
		{
			cloudParticles.stopSystem();
		}
		particles = null;
		cloudParticles = null;
		CTRSoundMgr._stopSounds();
	}
}
