using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;

namespace CutTheRope.game;

internal class StartupView : View
{
	public override void draw()
	{
		OpenGL.glEnable(0);
		OpenGL.glEnable(1);
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		preDraw();
		OpenGL.glClearColor(1.0, 1.0, 1.0, 1.0);
		OpenGL.glClear(0);
		float num = Application.sharedResourceMgr().getPercentLoaded();
		Texture2D texture = Application.getTexture(0);
		Rectangle r = FrameworkTypes.MakeRectangle(1.33f, 1.33f, texture._realWidth - 2, texture._realHeight - 2);
		if (texture.isWvga())
		{
			GLDrawer.drawImagePart(texture, r, 1f, -25f);
		}
		else
		{
			GLDrawer.drawImagePart(texture, r, 1f, 1f);
		}
		Texture2D texture2 = Application.getTexture(1);
		Rectangle r2 = FrameworkTypes.MakeRectangle(0.0, 0.0, 223.3 * (double)num / 100.0, 15.0);
		GLDrawer.drawImagePart(texture2, r2, 45f, 449f);
		postDraw();
		OpenGL.glDisable(0);
	}
}
