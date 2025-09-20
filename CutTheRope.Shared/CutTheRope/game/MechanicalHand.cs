using System.Collections.Generic;
using System.Linq;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.sfe;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class MechanicalHand : BaseElement
{
	public const int MH_CLAW_RADIUS = 17;

	public const int MH_JOINT_RADIUS = 12;

	public const int STATE_HAND_IDLE = 0;

	public const int STATE_HAND_CANDY = 1;

	public const int STATE_HAND_RELEASE = 2;

	public int state;

	public bool doRotateCandy;

	private Vector clawOffset;

	public ConstraintedPoint cPoint;

	public List<MechanicalHandSegment> segments;

	public MechanicalHandSegment rotatingSegment;

	public override NSObject init()
	{
		if (base.init() != null)
		{
			rotatingSegment = null;
			state = 0;
			cPoint = (ConstraintedPoint)new ConstraintedPoint().init();
			cPoint.disableGravity = true;
			cPoint.setWeight(0.0001f);
			Vector quadCenter = Image.getQuadCenter(272, 2);
			Vector quadCenter2 = Image.getQuadCenter(272, 8);
			clawOffset = MathHelper.vectSub(quadCenter2, quadCenter);
		}
		return this;
	}

	public virtual void addSegmentWithLengthAngleRotatable(float l, float a, bool r)
	{
		Vector vector = MathHelper.vect(0f, 0f);
		if (segments == null)
		{
			segments = new List<MechanicalHandSegment>();
		}
		else
		{
			vector = lastSegment().endPosition;
		}
		MechanicalHandSegment mechanicalHandSegment = (MechanicalHandSegment)new MechanicalHandSegment().initWithPositionlengthanglerotatable(MathHelper.vect(vector.x, vector.y), l, a, r);
		mechanicalHandSegment.anchor = 18;
		mechanicalHandSegment.parentAnchor = 18;
		mechanicalHandSegment.theHand = this;
		if (segments.Count - 1 >= 0)
		{
			lastSegment().removeChildWithID(0);
			lastSegment().endsWithHand = false;
			lastSegment().addChild(mechanicalHandSegment);
			BaseElement baseElement = mechanicalHandSegment.parent;
			for (int i = 0; i <= segments.Count - 1; i++)
			{
				mechanicalHandSegment.rotation -= baseElement.rotation;
				baseElement = baseElement.parent;
			}
		}
		else
		{
			addChild(mechanicalHandSegment);
			mechanicalHandSegment.drawBase = true;
		}
		segments.Add(mechanicalHandSegment);
		BaseElement.calculateTopLeft(mechanicalHandSegment);
		mechanicalHandSegment = null;
		theClaw().prevSegments = segments.Count - 1;
	}

	public virtual Vector jointAtIndexPosition(int index)
	{
		if (index == 0)
		{
			return MathHelper.vect(drawX, drawY);
		}
		Vector vector = MathHelper.vect(drawX, drawY);
		float num = 0f;
		for (int i = 0; i < index; i++)
		{
			num += segmentAtIndex(i).rotation;
			vector = MathHelper.vectAdd(vector, MathHelper.vectRotate(segmentAtIndex(i).endPosition, MathHelper.DEGREES_TO_RADIANS(num)));
		}
		return vector;
	}

	public virtual Vector clawPosition()
	{
		BaseElement child = getChild(0);
		Vector v = MathHelper.vect(drawX, drawY);
		float num = 0f;
		for (int i = 0; i <= segments.Count - 1; i++)
		{
			Vector endPosition = ((MechanicalHandSegment)child).endPosition;
			num += child.rotation;
			v = MathHelper.vectAdd(v, MathHelper.vectRotate(endPosition, MathHelper.DEGREES_TO_RADIANS(num)));
			child = child.getChild(0);
		}
		return MathHelper.vectAdd(v, MathHelper.vectRotate(clawOffset, MathHelper.DEGREES_TO_RADIANS(num)));
	}

	public virtual bool isRotating()
	{
		if (segments != null)
		{
			foreach (MechanicalHandSegment segment in segments)
			{
				if (segment != null && segment.getCurrentTimeline() != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public virtual void animateReleaseWithAnimationsPool(AnimationsPool aniPool)
	{
		Timeline timeline = catchBounceTimelineWithInitialScaleandAmplitude(theClaw().clawIdle.scaleX, 0.25f);
		timeline.delegateTimelineDelegate = aniPool;
		int t = theClaw().clawIdle.addTimeline(timeline);
		theClaw().clawIdle.playTimeline(t);
	}

	public virtual void animateCatchWithCandyPartsandAnimationsPool(List<BaseElement> candyParts, AnimationsPool aniPool)
	{
		float amp = 0.1f;
		Timeline timeline = catchBounceTimelineWithInitialScaleandAmplitude(theClaw().clawActive.scaleX, amp);
		Timeline timeline2 = catchBounceTimelineWithInitialScaleandAmplitude(theClaw().clawActiveFingers.scaleX, amp);
		timeline.delegateTimelineDelegate = aniPool;
		timeline2.delegateTimelineDelegate = aniPool;
		int t = theClaw().clawActive.addTimeline(timeline);
		int t2 = theClaw().clawActiveFingers.addTimeline(timeline2);
		theClaw().clawActive.playTimeline(t);
		theClaw().clawActiveFingers.playTimeline(t2);
		if (candyParts == null)
		{
			return;
		}
		foreach (BaseElement candyPart in candyParts)
		{
			if (candyPart != null)
			{
				Timeline timeline3 = catchBounceTimelineWithInitialScaleandAmplitude(0.71f, amp);
				timeline3.delegateTimelineDelegate = aniPool;
				int t3 = candyPart.addTimeline(timeline3);
				candyPart.playTimeline(t3);
			}
		}
	}

	public virtual MechanicalHandSegment segmentAtIndex(int value)
	{
		return segments[value];
	}

	public virtual MechanicalHandSegment lastSegment()
	{
		return segments.Last();
	}

	public virtual MechanicalHandClaw theClaw()
	{
		return (MechanicalHandClaw)lastSegment().getChild(0);
	}

	private static Timeline catchBounceTimelineWithInitialScaleandAmplitude(float startScale, float amp)
	{
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeScale(startScale + amp * startScale, startScale + amp * startScale, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.05));
		timeline.addKeyFrame(KeyFrame.makeScale(startScale, startScale, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.1));
		return timeline;
	}

	public override void dealloc()
	{
		cPoint = null;
		segments = null;
		base.dealloc();
	}

	public override void update(float delta)
	{
		base.update(delta);
		cPoint.pos = clawPosition();
	}
}
