using System;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class BulletScrollbarQuad : Scrollbar
{
	private static float BULLET_OFFSET = 5f;

	private static float BULLET_OPACITY = 0.5f;

	private static int BULLET_VERTEX_COUNT = 14;

	private Texture2D bullet;

	private int textureID;

	private int active;

	private int inactive;

	private float w;

	private Vector av;

	private Vector iv;

	private int bullets;

	public virtual NSObject initWithBulletTextureactiveQuadinactiveQuadandTotalBullets(int tID, int aq, int iq, int tb)
	{
		bullet = Application.getTexture(tID);
		bullets = tb;
		textureID = tID;
		av = Image.getQuadSize(tID, aq);
		iv = Image.getQuadSize(tID, iq);
		w = MathHelper.MAX(av.x, iv.x);
		height = (int)MathHelper.MAX(av.y, iv.y);
		width = (int)((float)tb * (w + BULLET_OFFSET) - BULLET_OFFSET);
		if (base.initWithWidthHeightVertical(width, height, v: false) != null)
		{
			active = aq;
			inactive = iq;
		}
		return this;
	}

	public override void dealloc()
	{
		NSObject.NSREL(bullet);
		base.dealloc();
	}

	public override void draw()
	{
		preDraw();
		if (MathHelper.vectEqual(sp, MathHelper.vectUndefined))
		{
			delegateProvider(ref sp, ref mp, ref sc);
		}
		float num = ((mp.x != 0f) ? (sp.x / mp.x) : 1f);
		int num2 = (int)MathHelper.round((float)bullets * num);
		float num3 = drawX;
		float num4 = drawY + MathHelper.ceil(height / 2);
		for (int i = 0; i <= bullets; i++)
		{
			if (i == num2)
			{
				GLDrawer.drawImageQuad(bullet, active, num3, (double)num4 - Math.Floor(av.y / 2f));
			}
			else
			{
				GLDrawer.drawImageQuad(bullet, inactive, num3, (double)num4 - Math.Floor(iv.y / 2f));
			}
			num3 += BULLET_OFFSET + w;
		}
		postDraw();
	}
}
