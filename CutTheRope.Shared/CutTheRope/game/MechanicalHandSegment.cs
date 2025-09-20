using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class MechanicalHandSegment : BaseElement, TimelineDelegate
{
	private const float ROTATE_ONCE_AGAIN_TIME = 0.125f;

	private Image _base;

	private Image buttonNone;

	private TiledImage armImage;

	private float length;

	private float angle;

	private float prevRotation;

	public Vector endPosition;

	private bool rotatable;

	public bool endsWithHand;

	public bool drawBase;

	private bool canRotateOnceAgain;

	private bool rotateOnceAgain;

	private float rotationTime;

	public float targetRotation;

	public MechanicalHandButton button;

	public MechanicalHand theHand;

	public virtual NSObject initWithPositionlengthanglerotatable(Vector pos, float l, float a, bool r)
	{
		if (init() != null)
		{
			x = pos.x;
			y = pos.y;
			rotation = a;
			prevRotation = a;
			length = l;
			endPosition = MathHelper.vectMult(MathHelper.vect(1.0, 0.0), length);
			rotatable = r;
			endsWithHand = true;
			passTransformationsToChilds = true;
			MechanicalHandClaw mechanicalHandClaw = (MechanicalHandClaw)new MechanicalHandClaw().init();
			mechanicalHandClaw.anchor = 18;
			mechanicalHandClaw.parentAnchor = 18;
			mechanicalHandClaw.x = endPosition.x;
			mechanicalHandClaw.y = endPosition.y;
			addChild(mechanicalHandClaw);
			_base = Image.Image_createWithResIDQuad(272, 4);
			buttonNone = Image.Image_createWithResIDQuad(272, 2);
			if (rotatable)
			{
				Image up = Image.Image_createWithResIDQuad(272, 1);
				Image down = Image.Image_createWithResIDQuad(272, 0);
				button = (MechanicalHandButton)new MechanicalHandButton().initWithUpElementDownElementandID(up, down, 0);
				button.anchor = 18;
				button.segment = this;
			}
			else
			{
				button = null;
			}
			_base.anchor = (buttonNone.anchor = 18);
			armImage = TiledImage.TiledImage_createWithResIDQuad(272, 3);
			armImage.setTile(3);
			armImage.width = (int)length;
			armImage.anchor = 17;
		}
		return this;
	}

	public virtual void rotate()
	{
		if (canRotateOnceAgain)
		{
			rotateOnceAgain = true;
		}
		if (rotatable && getCurrentTimeline() == null)
		{
			canRotateOnceAgain = false;
			rotateOnceAgain = false;
			rotationTime = 0f;
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeRotation(rotation, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeRotation(rotation + 90f, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.25));
			timeline.delegateTimelineDelegate = this;
			addTimeline(timeline);
			playTimeline(0);
			targetRotation = rotation + 90f;
		}
	}

	public virtual float rotationDelta()
	{
		return rotation - prevRotation;
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}

	public virtual void timelineFinished(Timeline t)
	{
		t.element.removeTimeline(0);
		if (rotateOnceAgain)
		{
			rotate();
		}
	}

	public override void draw()
	{
		if (drawBase)
		{
			_base.draw();
		}
		base.preDraw();
		armImage.draw();
		BaseElement.restoreTransformations(this);
		if (!rotatable)
		{
			buttonNone.draw();
		}
		else
		{
			button.draw();
		}
		bool flag = (double)scaleX != 1.0 || (double)scaleY != 1.0;
		bool flag2 = (double)rotation != 0.0;
		bool flag3 = (double)translateX != 0.0 || (double)translateY != 0.0;
		if (flag || flag2 || flag3)
		{
			OpenGL.glPushMatrix();
			if (flag || flag2)
			{
				float num = drawX + (float)(width >> 1) + rotationCenterX;
				float num2 = drawY + (float)(height >> 1) + rotationCenterY;
				OpenGL.glTranslatef(num, num2, 0.0);
				if (flag2)
				{
					OpenGL.glRotatef(rotation, 0.0, 0.0, 1.0);
				}
				if (flag)
				{
					OpenGL.glScalef(scaleX, scaleY, 1f);
				}
				OpenGL.glTranslatef(0f - num, 0f - num2, 0.0);
			}
			if (flag3)
			{
				OpenGL.glTranslatef(translateX, translateY, 0.0);
			}
		}
		base.postDraw();
	}

	public override void update(float delta)
	{
		prevRotation = rotation;
		base.update(delta);
		rotationTime += delta;
		if (rotationTime > 0.125f)
		{
			canRotateOnceAgain = true;
		}
		_base.x = (buttonNone.x = drawX);
		_base.y = (buttonNone.y = drawY);
		if (rotatable)
		{
			button.x = drawX;
			button.y = drawY;
		}
		armImage.x = drawX;
		armImage.y = drawY;
	}

	public override void dealloc()
	{
		_base = null;
		if (buttonNone != null)
		{
			buttonNone = null;
		}
		if (button != null)
		{
			button = null;
		}
		armImage = null;
		base.dealloc();
	}
}
