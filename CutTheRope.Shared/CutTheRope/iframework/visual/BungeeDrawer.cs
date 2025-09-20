using System;
using CutTheRope.game;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.sfe;

namespace CutTheRope.iframework.visual;

internal class BungeeDrawer : BaseElement
{
	public ButtonDelegate delegateButtonDelegate;

	public int bid;

	public Bungee bungee;

	public Processing fadeElement;

	public bool down;

	public Vector dragStart;

	public Vector tailStart;

	public Vector tailPos;

	public override void draw()
	{
		base.preDraw();
		OpenGL.glDisable(0);
		OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		bungee.draw();
		OpenGL.SetWhiteColor();
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		OpenGL.glEnable(0);
		base.postDraw();
	}

	public override void update(float delta)
	{
		base.update(delta);
		float delta2 = 0.025f;
		Vector quadCenter = Image.getQuadCenter(92, 7);
		bungee.bungeeAnchor.pos = MathHelper.vectAdd(MathHelper.vect(parent.drawX, parent.drawY), quadCenter);
		bungee.bungeeAnchor.pin = bungee.bungeeAnchor.pos;
		float num = 25f;
		bungee.tail.applyImpulseDelta(MathHelper.vect((0f - bungee.tail.v.x) / num, (0f - bungee.tail.v.y) / num), delta2);
		bungee.update(delta2);
		bungee.tail.update(delta2);
		float num2 = 1f - parent.y / (float)(int)Math.Floor(-312.0);
		fadeElement.color.a = 0.4f * num2;
		fadeElement.setEnabled(parent.y != (float)(int)Math.Floor(-312.0));
		if (!down)
		{
			return;
		}
		bungee.tail.pos = tailPos;
		if (bungee.relaxed == 0)
		{
			return;
		}
		ConstraintedPoint constraintedPoint = bungee.parts[0];
		ConstraintedPoint constraintedPoint2 = bungee.parts[1];
		float num3 = MathHelper.vectDistance(constraintedPoint.pos, constraintedPoint2.pos);
		Vector vector = MathHelper.vectSub(bungee.tail.pos, bungee.bungeeAnchor.pos);
		float num4 = 1f;
		float num5 = (num3 - 30f) * num4;
		if ((double)Math.Abs(vector.y) > 20.0)
		{
			if (tailPos.y > bungee.bungeeAnchor.pos.y)
			{
				parent.y += num5;
			}
			else
			{
				parent.y -= num5;
			}
			parent.y = MathHelper.FIT_TO_BOUNDARIES(parent.y, (int)Math.Floor(-312.0), 100f);
		}
		else if ((double)num3 > 45.0)
		{
			down = false;
			if (delegateButtonDelegate != null)
			{
				delegateButtonDelegate.onButtonPressed(bid);
			}
		}
	}

	public override bool onTouchDownXY(float tx, float ty)
	{
		bool result = base.onTouchDownXY(tx, ty);
		if (MathHelper.pointInRect(tx, ty, bungee.tail.pos.x - 35f, bungee.tail.pos.y - 15f, 70f, 70f))
		{
			fadeElement.setEnabled(e: true);
			down = true;
			dragStart = MathHelper.vect(tx, ty);
			tailStart = bungee.tail.pos;
			onTouchMoveXY(tx, ty);
			return true;
		}
		return result;
	}

	public override bool onTouchUpXY(float tx, float ty)
	{
		bool result = base.onTouchUpXY(tx, ty);
		if (down)
		{
			down = false;
			if (delegateButtonDelegate != null)
			{
				delegateButtonDelegate.onButtonPressed(bid);
			}
			return true;
		}
		return result;
	}

	public override bool onTouchMoveXY(float tx, float ty)
	{
		bool result = base.onTouchMoveXY(tx, ty);
		if (down)
		{
			float num = 1f;
			if (ty > FrameworkTypes.SCREEN_HEIGHT / 2f)
			{
				num /= 1.5f;
			}
			Vector v = MathHelper.vectSub(MathHelper.vect(tx, ty), dragStart);
			v = MathHelper.vectMult(v, num);
			tailPos = MathHelper.vectAdd(tailStart, v);
			dragStart = MathHelper.vect(tx, ty);
			tailStart = tailPos;
		}
		return result;
	}
}
