using System;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class Grab : GameObject
{
	private enum SPIDER_ANI
	{
		SPIDER_START_ANI,
		SPIDER_WALK_ANI,
		SPIDER_BUSTED_ANI,
		SPIDER_CATCH_ANI
	}

	public const int GUN_CUP_SHOW = 0;

	public const int GUN_CUP_HIDE = 1;

	public const int GUN_CUP_DROP_AND_HIDE = 2;

	private const float SPIDER_SPEED = 45f;

	public const int KICK_MOVE_LENGTH = 10;

	public const int KICK_CUT_RADIUS = 15;

	public const int GUN_CUT_RADIUS = 15;

	public const int KICK_TAP_RADIUS = 30;

	public const int WHEEL_TAP_RADIUS = 45;

	public const int MOVER_TAP_RADIUS = 30;

	public const int BUBBLE_TAP_RADIUS = 30;

	public const int GUN_TAP_RADIUS = 35;

	public const float STICK_DELAY = 0.05f;

	public const int MAX_STAINS = 10;

	public Image back;

	public Image front;

	public Image dot;

	public Bungee rope;

	public float radius;

	public float radiusAlpha;

	public bool hideRadius;

	public float[] vertices;

	public int vertexCount;

	public bool wheel;

	public Image wheelHighlight;

	public Image wheelImage;

	public Image wheelImage2;

	public Image wheelImage3;

	public int wheelOperating;

	public Vector lastWheelTouch;

	public float moveLength;

	public bool moveVertical;

	public float moveOffset;

	public HorizontallyTiledImage moveBackground;

	public Image grabMoverHighlight;

	public Image grabMover;

	public int moverDragging;

	public float minMoveValue;

	public float maxMoveValue;

	public bool hasSpider;

	public bool spiderActive;

	public Animation spider;

	public float spiderPos;

	public bool shouldActivate;

	public bool wheelDirty;

	public bool launcher;

	public float launcherSpeed;

	public bool launcherIncreaseSpeed;

	public bool baloon;

	public bool gun;

	public bool gunFired;

	private Image gunBack;

	public Image gunArrow;

	public Image gunFront;

	public Animation gunCup;

	public float gunInitialRotation;

	public float gunCandyInitialRotation;

	public int stainCounter;

	public bool kickable;

	public bool kicked;

	public bool kickActive;

	public bool invisible;

	private Vector kickPrevPos;

	public float stickTimer;

	public Image bee;

	public override NSObject init()
	{
		if (base.init() != null)
		{
			rope = null;
			wheelOperating = -1;
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			baloon = cTRRootController.isSurvival();
			gun = false;
			gunFired = false;
			invisible = false;
			kicked = false;
			stickTimer = -1f;
		}
		return this;
	}

	public override void update(float delta)
	{
		base.update(delta);
		if (gunFired && gunCup != null)
		{
			gunCup.update(delta);
		}
		if (launcher && rope != null)
		{
			rope.bungeeAnchor.pos = MathHelper.vect(x, y);
			rope.bungeeAnchor.pin = rope.bungeeAnchor.pos;
			if (launcherIncreaseSpeed)
			{
				if (Mover.moveVariableToTarget(ref launcherSpeed, 200f, 30f, delta))
				{
					launcherIncreaseSpeed = false;
				}
			}
			else if (Mover.moveVariableToTarget(ref launcherSpeed, 130f, 30f, delta))
			{
				launcherIncreaseSpeed = true;
			}
			mover.setMoveSpeed(launcherSpeed);
		}
		if (hideRadius)
		{
			radiusAlpha -= 1.5f * delta;
			if ((double)radiusAlpha <= 0.0)
			{
				radius = -1f;
				hideRadius = false;
			}
		}
		if (wheel && wheelDirty)
		{
			float num = ((rope == null) ? 0f : ((float)rope.getLength() * 0.7f));
			if (num == 0f)
			{
				wheelImage2.scaleX = (wheelImage2.scaleY = 0f);
			}
			else
			{
				wheelImage2.scaleX = (wheelImage2.scaleY = (float)Math.Max(0.0, Math.Min(1.2, 1.0 - (double)num / 700.0)));
			}
		}
		if (bee != null)
		{
			Vector v = mover.path[mover.targetPoint];
			Vector pos = mover.pos;
			Vector vector = MathHelper.vectSub(v, pos);
			float t = 0f;
			if (Math.Abs(vector.x) > 15f)
			{
				t = ((vector.x > 0f) ? 10f : (-10f));
			}
			Mover.moveVariableToTarget(ref bee.rotation, t, 60f, delta);
		}
	}

	public override void draw()
	{
		if (invisible)
		{
			return;
		}
		if (kickable && kicked && rope != null)
		{
			x = rope.bungeeAnchor.pos.x;
			y = rope.bungeeAnchor.pos.y;
		}
		base.preDraw();
		OpenGL.glEnable(0);
		Bungee bungee = rope;
		if (wheel)
		{
			wheelHighlight.visible = wheelOperating != -1;
			wheelImage3.visible = wheelOperating == -1;
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			wheelImage.draw();
		}
		OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		if (gunBack != null)
		{
			gunBack.draw();
			if (!gunFired && gunArrow != null)
			{
				gunArrow.draw();
			}
		}
		OpenGL.glDisable(0);
		bungee?.draw();
		OpenGL.SetWhiteColor();
		OpenGL.glEnable(0);
		if (gunFront != null)
		{
			gunFront.draw();
		}
		if ((double)moveLength <= 0.0)
		{
			if (front != null)
			{
				front.draw();
			}
		}
		else if (moverDragging != -1)
		{
			if (grabMoverHighlight != null)
			{
				grabMoverHighlight.draw();
			}
		}
		else if (grabMover != null)
		{
			grabMover.draw();
		}
		if (wheel)
		{
			wheelImage2.draw();
		}
		base.postDraw();
	}

	public override void dealloc()
	{
		if (vertices != null)
		{
			vertices = null;
		}
		destroyRope();
		base.dealloc();
	}

	public virtual void setRope(Bungee r)
	{
		NSObject.NSREL(rope);
		rope = r;
		radius = -1f;
		if (hasSpider)
		{
			shouldActivate = true;
		}
	}

	public virtual void setRadius(float r)
	{
		radius = r;
		if (gun)
		{
			gunBack = Image.Image_createWithResIDQuad(115, 0);
			gunBack.doRestoreCutTransparency();
			gunBack.anchor = (gunBack.parentAnchor = 18);
			addChild(gunBack);
			gunBack.visible = false;
			gunArrow = Image.Image_createWithResIDQuad(115, 1);
			gunArrow.doRestoreCutTransparency();
			gunArrow.anchor = (gunArrow.parentAnchor = 18);
			addChild(gunArrow);
			gunArrow.visible = false;
			gunFront = Image.Image_createWithResIDQuad(115, 2);
			gunFront.doRestoreCutTransparency();
			gunFront.anchor = (gunFront.parentAnchor = 18);
			addChild(gunFront);
			gunFront.visible = false;
			gunCup = Animation.Animation_createWithResID(115);
			gunCup.doRestoreCutTransparency();
			gunCup.addAnimationWithIDDelayLoopFirstLast(0, 0.1f, Timeline.LoopType.TIMELINE_NO_LOOP, 4, 10);
			gunCup.anchor = 18;
			addChild(gunCup);
			gunCup.visible = false;
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
			gunCup.addTimelinewithID(timeline, 1);
			Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
			timeline2.addKeyFrame(KeyFrame.makePos(0.0, 0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline2.addKeyFrame(KeyFrame.makePos(0.0, 50.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
			gunCup.addTimelinewithID(timeline2, 2);
			Track track = timeline2.getTrack(Track.TrackType.TRACK_POSITION);
			track.relative = true;
			return;
		}
		if (kickable)
		{
			stainCounter = 10;
			int r2 = 116;
			back = Image.Image_createWithResIDQuad(r2, 3);
			back.doRestoreCutTransparency();
			back.anchor = (back.parentAnchor = 18);
			front = Image.Image_createWithResIDQuad(r2, 4);
			front.doRestoreCutTransparency();
			front.anchor = (front.parentAnchor = 18);
			addChild(back);
			addChild(front);
			back.visible = false;
			front.visible = false;
			updateKickState();
		}
		else if (radius == -1f)
		{
			int r3 = MathHelper.RND_RANGE(142, 143);
			back = Image.Image_createWithResIDQuad(r3, 0);
			back.doRestoreCutTransparency();
			back.anchor = (back.parentAnchor = 18);
			front = Image.Image_createWithResIDQuad(r3, 1);
			front.anchor = (front.parentAnchor = 18);
			addChild(back);
			addChild(front);
			back.visible = false;
			front.visible = false;
		}
		else
		{
			back = Image.Image_createWithResIDQuad(139, 0);
			back.doRestoreCutTransparency();
			back.anchor = (back.parentAnchor = 18);
			front = Image.Image_createWithResIDQuad(139, 1);
			front.anchor = (front.parentAnchor = 18);
			addChild(back);
			addChild(front);
			back.visible = false;
			front.visible = false;
		}
		if (radius != -1f)
		{
			radiusAlpha = 1f;
			hideRadius = false;
			vertexCount = (int)Math.Max(16f, radius);
			if (vertexCount % 2 != 0)
			{
				vertexCount++;
			}
			vertices = new float[vertexCount * 2];
			GLDrawer.calcCircle(x, y, radius, vertexCount, vertices);
		}
	}

	public virtual void setMoveLengthVerticalOffset(float l, bool v, float o)
	{
		moveLength = l;
		moveVertical = v;
		moveOffset = o;
		if ((double)moveLength > 0.0)
		{
			moveBackground = HorizontallyTiledImage.HorizontallyTiledImage_createWithResID(151);
			moveBackground.setTileHorizontallyLeftCenterRight(0, 2, 1);
			moveBackground.width = (int)((double)l + 37.0);
			moveBackground.rotationCenterX = (float)(0.0 - Math.Round((double)moveBackground.width / 2.0) + 17.0);
			moveBackground.x = -17f;
			grabMoverHighlight = Image.Image_createWithResIDQuad(151, 3);
			grabMoverHighlight.visible = false;
			grabMoverHighlight.anchor = (grabMoverHighlight.parentAnchor = 18);
			addChild(grabMoverHighlight);
			grabMover = Image.Image_createWithResIDQuad(151, 4);
			grabMover.visible = false;
			grabMover.anchor = (grabMover.parentAnchor = 18);
			addChild(grabMover);
			grabMover.addChild(moveBackground);
			if (moveVertical)
			{
				moveBackground.rotation = 90f;
				moveBackground.y = 0f - moveOffset;
				minMoveValue = y - moveOffset;
				maxMoveValue = y + (moveLength - moveOffset);
				grabMover.rotation = 90f;
				grabMoverHighlight.rotation = 90f;
			}
			else
			{
				minMoveValue = x - moveOffset;
				maxMoveValue = x + (moveLength - moveOffset);
				moveBackground.x += 0f - moveOffset;
			}
			moveBackground.anchor = 19;
			moveBackground.x += x;
			moveBackground.y += y;
			moveBackground.visible = false;
		}
		moverDragging = -1;
		if (moveLength >= 0f)
		{
			kickable = false;
		}
	}

	public virtual void setBee()
	{
	}

	public virtual void setSpider(bool s)
	{
		hasSpider = s;
		shouldActivate = false;
		spiderActive = false;
		spider = Animation.Animation_createWithResID(113);
		spider.texture.PixelCorrectionY();
		spider.doRestoreCutTransparency();
		spider.anchor = 18;
		spider.x = x;
		spider.y = y;
		spider.visible = false;
		spider.addAnimationWithIDDelayLoopFirstLast(0, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 6);
		spider.setDelayatIndexforAnimation(0.4f, 5, 0);
		spider.addAnimationWithIDDelayLoopFirstLast(1, 0.1f, Timeline.LoopType.TIMELINE_REPLAY, 7, 10);
		spider.switchToAnimationatEndOfAnimationDelay(1, 0, 0.05f);
		addChild(spider);
	}

	public virtual void setLauncher()
	{
		launcher = true;
		launcherIncreaseSpeed = true;
		launcherSpeed = 130f;
		Mover mover = new Mover().initWithPathCapacityMoveSpeedRotateSpeed(100, launcherSpeed, 0f);
		mover.setPathFromStringandStart(new NSString("RC30"), MathHelper.vect(x, y));
		setMover(mover);
		mover.start();
	}

	public virtual void destroyRope()
	{
		NSObject.NSREL(rope);
		rope = null;
	}

	public virtual void reCalcCircle()
	{
		if (vertices != null)
		{
			GLDrawer.calcCircle(x, y, radius, vertexCount, vertices);
		}
	}

	public virtual void drawBack()
	{
		if (invisible)
		{
			return;
		}
		if (kickable && kicked && rope != null)
		{
			x = rope.bungeeAnchor.pos.x * 0.8f + x * 0.19999999f;
			y = rope.bungeeAnchor.pos.y * 0.8f + y * 0.19999999f;
		}
		if (!gun)
		{
			BaseElement.calculateTopLeft(this);
			if ((double)moveLength > 0.0)
			{
				moveBackground.draw();
			}
			else
			{
				back.draw();
			}
			OpenGL.glDisable(0);
			if (radius != -1f || hideRadius)
			{
				CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
				int pack = cTRRootController.getPack();
				drawGrabCircle(color: (pack != 1) ? RGBAColor.MakeRGBA(0.2, 0.5, 0.9, radiusAlpha) : RGBAColor.MakeRGBA(14.0 / 85.0, 0.23529411764705882, 8.0 / 15.0, radiusAlpha), s: this, x: x, y: y, radius: radius, vertexCount: vertexCount);
			}
			OpenGL.SetWhiteColor();
			OpenGL.glEnable(0);
		}
	}

	public virtual void drawSpider()
	{
		spider.draw();
	}

	public virtual void drawGunCup()
	{
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		if (gunCup != null)
		{
			gunCup.draw();
		}
	}

	public virtual void updateSpider(float delta)
	{
		if (hasSpider && shouldActivate)
		{
			shouldActivate = false;
			spiderActive = true;
			CTRSoundMgr._playSound(39);
			spider.playTimeline(0);
		}
		if (!hasSpider || !spiderActive)
		{
			return;
		}
		if (spider.getCurrentTimelineIndex() != 0)
		{
			spiderPos += delta * 45f;
		}
		float num = 0f;
		bool flag = false;
		if (rope != null)
		{
			for (int i = 0; i < rope.drawPtsCount; i += 2)
			{
				Vector vector = MathHelper.vect(rope.drawPts[i], rope.drawPts[i + 1]);
				Vector vector2 = MathHelper.vect(rope.drawPts[i + 2], rope.drawPts[i + 3]);
				float num2 = Math.Max(20f, MathHelper.vectDistance(vector, vector2));
				if (spiderPos >= num && (spiderPos < num + num2 || i > rope.drawPtsCount - 3))
				{
					float num3 = spiderPos - num;
					Vector v = MathHelper.vectSub(vector2, vector);
					v = MathHelper.vectMult(v, num3 / num2);
					spider.x = vector.x + v.x;
					spider.y = vector.y + v.y;
					if (i > rope.drawPtsCount - 3)
					{
						flag = true;
					}
					if (spider.getCurrentTimelineIndex() != 0)
					{
						spider.rotation = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v)) + 270f;
					}
					break;
				}
				num += num2;
			}
		}
		if (flag)
		{
			spiderPos = -1f;
		}
	}

	public virtual void handleWheelTouch(Vector v)
	{
		lastWheelTouch = v;
	}

	public virtual void handleWheelRotate(Vector v)
	{
		if (lastWheelTouch.x - v.x == 0f && lastWheelTouch.y - v.y == 0f)
		{
			return;
		}
		float num = getRotateAngleForStartEndCenter(lastWheelTouch, v, MathHelper.vect(x, y));
		if ((double)num > 180.0)
		{
			num -= 360f;
		}
		else if ((double)num < -180.0)
		{
			num += 360f;
		}
		wheelImage2.rotation += num;
		wheelImage3.rotation += num;
		wheelHighlight.rotation += num;
		num = ((num > 0f) ? MathHelper.MIN(MathHelper.MAX(1.0, num), 2.0) : MathHelper.MAX(MathHelper.MIN(-1.0, num), -2.0));
		if (rope != null)
		{
			float num2 = rope.getLength();
			if (num > 0f)
			{
				if ((double)num2 < 500.0)
				{
					rope.roll(num);
				}
			}
			else if (num != 0f && rope.parts.Count > 3)
			{
				rope.rollBack(0f - num);
			}
			wheelDirty = true;
		}
		lastWheelTouch = v;
	}

	public virtual void updateKickState()
	{
		if (kicked)
		{
			back.setDrawQuad(1);
			front.setDrawQuad(2);
		}
		else
		{
			back.setDrawQuad(3);
			front.setDrawQuad(4);
		}
		if (rope != null)
		{
			x = rope.bungeeAnchor.pos.x;
			y = rope.bungeeAnchor.pos.y;
		}
	}

	public virtual float getRotateAngleForStartEndCenter(Vector v1, Vector v2, Vector c)
	{
		Vector v3 = MathHelper.vectSub(v1, c);
		Vector v4 = MathHelper.vectSub(v2, c);
		float r = MathHelper.vectAngleNormalized(v4) - MathHelper.vectAngleNormalized(v3);
		return MathHelper.RADIANS_TO_DEGREES(r);
	}

	private void drawGrabCircle(Grab s, float x, float y, float radius, int vertexCount, RGBAColor color)
	{
		if (s != null && s.vertices != null)
		{
			OpenGL.glColor4f(color.r, color.g, color.b, color.a);
			OpenGL.glDisableClientState(0);
			OpenGL.glEnableClientState(13);
			OpenGL.glColorPointer_setAdditive(s.vertexCount * 8);
			OpenGL.glVertexPointer_setAdditive(2, 5, 0, s.vertexCount * 16);
			for (int i = 0; i < s.vertexCount; i += 2)
			{
				GLDrawer.drawAntialiasedLine(s.vertices[i * 2], s.vertices[i * 2 + 1], s.vertices[i * 2 + 2], s.vertices[i * 2 + 3], 1f, color);
			}
			OpenGL.glDrawArrays(8, 0, 8);
			OpenGL.glEnableClientState(0);
			OpenGL.glDisableClientState(13);
		}
	}
}
