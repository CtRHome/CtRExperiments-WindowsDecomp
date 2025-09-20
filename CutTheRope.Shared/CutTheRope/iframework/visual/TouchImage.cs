using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;

namespace CutTheRope.iframework.visual;

internal class TouchImage : Image
{
	public int bid;

	public ButtonDelegate delegateButtonDelegate;

	public override bool onTouchDownXY(float tx, float ty)
	{
		base.onTouchDownXY(tx, ty);
		if (MathHelper.pointInRect(tx, ty, drawX, drawY, width, height))
		{
			if (delegateButtonDelegate != null)
			{
				delegateButtonDelegate.onButtonPressed(bid);
			}
			return true;
		}
		return false;
	}

	private static TouchImage TouchImage_create(Texture2D t)
	{
		return (TouchImage)new TouchImage().initWithTexture(t);
	}

	public static TouchImage TouchImage_createWithResIDQuad(int r, int q)
	{
		TouchImage touchImage = TouchImage_create(Application.getTexture(r));
		touchImage.setDrawQuad(q);
		return touchImage;
	}
}
