using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;

namespace CutTheRope.game;

internal class Confetti : Animation
{
	public Timeline ani;

	public static Confetti Confetti_createWithResID(int r)
	{
		return Confetti_create(Application.getTexture(r));
	}

	public static Confetti Confetti_create(Texture2D t)
	{
		return (Confetti)new Confetti().initWithTexture(t);
	}

	public override void update(float delta)
	{
		base.update(delta);
		Timeline.updateTimeline(ani, delta);
	}
}
