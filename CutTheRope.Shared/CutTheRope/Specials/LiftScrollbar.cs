using CutTheRope.ctr_commons;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.Specials;

internal class LiftScrollbar : Image
{
	private Lift lift;

	public ScrollableContainer container;

	public static LiftScrollbar createWithResIDBackQuadLiftQuadLiftQuadPressed(int resID, int bq, int lq, int lqp)
	{
		return (LiftScrollbar)new LiftScrollbar().initWithResIDBackQuadLiftQuadLiftQuadPressed(resID, bq, lq, lqp);
	}

	public virtual NSObject initWithResIDBackQuadLiftQuadLiftQuadPressed(int resID, int bq, int lq, int lqp)
	{
		if (base.initWithTexture(Application.getTexture(resID)) != null)
		{
			setDrawQuad(bq);
			Image up = Image.Image_createWithResIDQuad(resID, lq);
			Image down = Image.Image_createWithResIDQuad(resID, lqp);
			lift = (Lift)new Lift().initWithUpElementDownElementandID(up, down, 0);
			lift.parentAnchor = 10;
			lift.anchor = 18;
			lift.maxY = height;
			lift.liftDelegate = percentXY;
			addChild(lift);
		}
		return this;
	}

	public void percentXY(float px, float py)
	{
		Vector maxScroll = container.getMaxScroll();
		container.setScroll(MathHelper.vect(maxScroll.x * px, maxScroll.y * py));
	}

	public override void update(float delta)
	{
		base.update(delta);
		Vector scroll = container.getScroll();
		Vector maxScroll = container.getMaxScroll();
		float num = 0f;
		float num2 = 0f;
		if (maxScroll.x != 0f)
		{
			num = scroll.x / maxScroll.x;
		}
		if (maxScroll.y != 0f)
		{
			num2 = scroll.y / maxScroll.y;
		}
		lift.x = (lift.maxX - lift.minX) * num + lift.minX;
		lift.y = (lift.maxY - lift.minY) * num2 + lift.minY;
	}

	public override void dealloc()
	{
		container = null;
		base.dealloc();
	}

	public override bool onTouchUpXY(float tx, float ty)
	{
		bool result = base.onTouchUpXY(tx, ty);
		container.startMovingToSpointInDirection(MathHelper.vectZero);
		return result;
	}
}
