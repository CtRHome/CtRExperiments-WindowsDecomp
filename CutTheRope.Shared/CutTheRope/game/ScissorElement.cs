using CutTheRope.iframework;
using CutTheRope.iframework.visual;

namespace CutTheRope.game;

internal class ScissorElement : BaseElement
{
	public override void draw()
	{
		OpenGL.glEnable(4);
		OpenGL.setScissorRectangle(drawX, drawY, width, height);
		base.draw();
		OpenGL.glDisable(4);
	}
}
