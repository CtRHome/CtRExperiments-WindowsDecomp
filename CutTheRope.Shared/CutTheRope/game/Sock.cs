using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;

namespace CutTheRope.game;

internal class Sock : CTRGameObject
{
	public const float SOCK_IDLE_TIMOUT = 0.8f;

	public static int SOCK_RECEIVING = 0;

	public static int SOCK_THROWING = 1;

	public static int SOCK_IDLE = 2;

	public int group;

	public double angle;

	public Vector t1;

	public Vector t2;

	public Vector b1;

	public Vector b2;

	public float idleTimeout;

	public Animation light;

	public static Sock Sock_create(Texture2D t)
	{
		return (Sock)new Sock().initWithTexture(t);
	}

	public static Sock Sock_createWithResID(int r)
	{
		return Sock_create(Application.getTexture(r));
	}

	public static Sock Sock_createWithResIDQuad(int r, int q)
	{
		Sock sock = Sock_create(Application.getTexture(r));
		sock.setDrawQuad(q);
		return sock;
	}

	public override void update(float delta)
	{
		base.update(delta);
		if (mover != null)
		{
			updateRotation();
		}
	}

	public override void draw()
	{
		Timeline timeline = light.getCurrentTimeline();
		if (timeline != null && timeline.state == Timeline.TimelineState.TIMELINE_STOPPED)
		{
			light.visible = false;
		}
		base.draw();
	}

	public void createAnimations()
	{
	}

	public void updateRotation()
	{
		t1.x = x - 15f;
		t2.x = x + 15f;
		t1.y = (t2.y = y);
		b1.x = t1.x;
		b2.x = t2.x;
		b1.y = (b2.y = y + 1f);
		angle = MathHelper.DEGREES_TO_RADIANS(rotation);
		t1 = MathHelper.vectRotateAround(t1, angle, x, y);
		t2 = MathHelper.vectRotateAround(t2, angle, x, y);
		b1 = MathHelper.vectRotateAround(b1, angle, x, y);
		b2 = MathHelper.vectRotateAround(b2, angle, x, y);
	}
}
