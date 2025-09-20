using CutTheRope.game;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.ctr_original;

internal class DrawingsView : MenuView, DrawingDelegate
{
	public Drawing ad;

	private Text totalFound;

	public override void draw()
	{
		OpenGL.SetWhiteColor();
		OpenGL.glEnable(0);
		OpenGL.glEnable(1);
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		base.preDraw();
		base.postDraw();
		if (ad != null)
		{
			ad.drawDrawing();
		}
		OpenGL.glDisable(0);
		OpenGL.glDisable(1);
	}

	public override bool onTouchDownXY(float tx, float ty)
	{
		if (ad != null && ad.drawingVisible)
		{
			return ad.onTouchDownXY(tx, ty);
		}
		return base.onTouchDownXY(tx, ty);
	}

	public override bool onTouchMoveXY(float tx, float ty)
	{
		if (ad != null && ad.drawingVisible)
		{
			return ad.onTouchMoveXY(tx, ty);
		}
		return base.onTouchMoveXY(tx, ty);
	}

	public override bool onTouchUpXY(float tx, float ty)
	{
		if (ad != null && ad.drawingVisible)
		{
			return ad.onTouchUpXY(tx, ty);
		}
		return base.onTouchUpXY(tx, ty);
	}

	public virtual void drawingShowing(Drawing d)
	{
		ad = d;
		if (totalFound != null)
		{
			totalFound.setString(NSObject.NSS(Application.getString(1179709).ToString() + CTRPreferences.getDrawingUnlockedCount() + CTRPreferences.DRAWINGS_COUNT()));
		}
	}

	public virtual void drawingHidden(Drawing d)
	{
		ad = null;
	}
}
