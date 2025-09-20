using CutTheRope.iframework;
using CutTheRope.iframework.core;

internal class CoppaView : View
{
	public override void draw()
	{
		OpenGL.SetWhiteColor();
		OpenGL.glEnable(0);
		OpenGL.glEnable(1);
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		base.preDraw();
		base.postDraw();
		OpenGL.glDisable(0);
		OpenGL.glDisable(1);
	}
}
