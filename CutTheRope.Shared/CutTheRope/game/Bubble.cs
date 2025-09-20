using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;

namespace CutTheRope.game;

internal class Bubble : GameObject
{
	public bool popped;

	public static Bubble Bubble_create(Texture2D t)
	{
		return (Bubble)new Bubble().initWithTexture(t);
	}

	public static Bubble Bubble_createWithResID(int r)
	{
		return Bubble_create(Application.getTexture(r));
	}

	public static Bubble Bubble_createWithResIDQuad(int r, int q)
	{
		Bubble bubble = Bubble_create(Application.getTexture(r));
		bubble.setDrawQuad(q);
		return bubble;
	}
}
