using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.iframework.visual;

internal class CircleElement : BaseElement
{
	public bool solid;

	public int vertextCount;

	public override NSObject init()
	{
		if (base.init() != null)
		{
			vertextCount = 32;
			solid = true;
		}
		return this;
	}

	public override void draw()
	{
		base.preDraw();
		OpenGL.glDisable(0);
		MathHelper.MIN(width, height);
		_ = solid;
		OpenGL.glEnable(0);
		OpenGL.SetWhiteColor();
		base.postDraw();
	}
}
