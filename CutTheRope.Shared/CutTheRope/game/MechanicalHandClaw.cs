using System.Collections.Generic;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class MechanicalHandClaw : BaseElement
{
	public Image clawIdle;

	public Image clawActive;

	public Image clawActiveFingers;

	private MechanicalHand mechanicalHand;

	public int prevSegments;

	public override NSObject init()
	{
		if (base.init() != null)
		{
			clawIdle = Image.Image_createWithResIDQuad(272, 5);
			clawActive = Image.Image_createWithResIDQuad(272, 6);
			clawActiveFingers = Image.Image_createWithResIDQuad(272, 7);
			clawIdle.anchor = 18;
			clawActive.anchor = 18;
			clawActiveFingers.anchor = 18;
			clawIdle.doRestoreCutTransparency();
			clawActive.doRestoreCutTransparency();
			clawActiveFingers.doRestoreCutTransparency();
		}
		return this;
	}

	public virtual MechanicalHand theHand()
	{
		BaseElement baseElement = parent;
		for (int i = 0; i <= prevSegments; i++)
		{
			baseElement = baseElement.parent;
		}
		return (MechanicalHand)baseElement;
	}

	public override void draw()
	{
		base.preDraw();
		if (mechanicalHand == null)
		{
			mechanicalHand = theHand();
		}
		if (mechanicalHand.state == 1)
		{
			clawActive.draw();
		}
		else
		{
			clawIdle.draw();
		}
		base.postDraw();
	}

	public virtual void drawFingers()
	{
		List<MechanicalHandSegment> segments = mechanicalHand.segments;
		if (segments != null)
		{
			foreach (MechanicalHandSegment item in segments)
			{
				item?.preDraw();
			}
		}
		base.preDraw();
		clawActiveFingers.draw();
		base.postDraw();
		if (segments == null)
		{
			return;
		}
		foreach (MechanicalHandSegment item2 in segments)
		{
			if (item2 != null)
			{
				if (item2.passTransformationsToChilds)
				{
					BaseElement.restoreTransformations(item2);
				}
				if (item2.passColorToChilds)
				{
					BaseElement.restoreColor(item2);
				}
			}
		}
	}

	public virtual void drawActiveHand()
	{
		List<MechanicalHandSegment> segments = mechanicalHand.segments;
		if (segments != null)
		{
			foreach (MechanicalHandSegment item in segments)
			{
				item?.preDraw();
			}
		}
		base.preDraw();
		if (mechanicalHand.state == 1)
		{
			clawActive.draw();
		}
		base.postDraw();
		if (segments == null)
		{
			return;
		}
		foreach (MechanicalHandSegment item2 in segments)
		{
			if (item2 != null)
			{
				if (item2.passTransformationsToChilds)
				{
					BaseElement.restoreTransformations(item2);
				}
				if (item2.passColorToChilds)
				{
					BaseElement.restoreColor(item2);
				}
			}
		}
	}

	public override void update(float delta)
	{
		clawActive.x = drawX;
		clawActive.y = drawY;
		clawActiveFingers.x = drawX;
		clawActiveFingers.y = drawY;
		clawIdle.x = drawX;
		clawIdle.y = drawY;
		clawActive.update(delta);
		clawActiveFingers.update(delta);
		clawIdle.update(delta);
	}

	public override void dealloc()
	{
		clawActive = null;
		clawActiveFingers = null;
		clawIdle = null;
		base.dealloc();
	}
}
