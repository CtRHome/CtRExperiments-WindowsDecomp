using CutTheRope.ios;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.iframework.visual;

internal partial class GLCanvas : NSObject
{
	public int backingWidth;

	public int backingHeight;

	public uint viewRenderbuffer;

	public uint viewFramebuffer;

	public uint depthRenderbuffer;

	public FontGeneric fpsFont;

	public Text fpsText;

	public TouchDelegate touchDelegate;

	public virtual GLCanvas initWithFrame(Rectangle frame)
	{
		base.init();
		return this;
	}

	public virtual void show()
	{
		destroyFramebuffer();
		createFramebuffer();
	}

	public virtual void hide()
	{
		destroyFramebuffer();
	}

	public virtual void beforeRender()
	{
		setDefaultProjection();
		OpenGL.glDisable(1);
		OpenGL.glEnableClientState(11);
		OpenGL.glEnableClientState(12);
	}

	public virtual void afterRender()
	{
	}

	public virtual void initFPSMeterWithFont(FontGeneric font)
	{
		fpsFont = font;
		fpsText = new Text().initWithFont(fpsFont);
	}

	public virtual void drawFPS(int fps)
	{
		if (fpsText != null && fpsFont != null)
		{
			NSString @string = NSObject.NSS(fps.ToString());
			fpsText.setString(@string);
			OpenGL.SetWhiteColor();
			OpenGL.glEnable(0);
			OpenGL.glEnable(1);
			OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			fpsText.x = 5f;
			fpsText.y = 5f;
			fpsText.draw();
			OpenGL.glDisable(1);
			OpenGL.glDisable(0);
		}
	}

	public virtual bool createFramebuffer()
	{
		backingWidth = (int)FrameworkTypes.SCREEN_WIDTH;
		backingHeight = (int)FrameworkTypes.SCREEN_HEIGHT;
		setDefaultProjection();
		OpenGL.glEnableClientState(11);
		OpenGL.glEnableClientState(12);
		return true;
	}

	public virtual void setDefaultProjection()
	{
		OpenGL.glViewport(0.0, 0.0, FrameworkTypes.REAL_SCREEN_WIDTH, FrameworkTypes.REAL_SCREEN_HEIGHT);
		OpenGL.glMatrixMode(15);
		OpenGL.glLoadIdentity();
		OpenGL.glOrthof(0f - FrameworkTypes.SCREEN_OFFSET_X, FrameworkTypes.SCREEN_WIDTH + FrameworkTypes.SCREEN_OFFSET_X, FrameworkTypes.SCREEN_HEIGHT + FrameworkTypes.SCREEN_OFFSET_Y, 0f - FrameworkTypes.SCREEN_OFFSET_Y, -1.0, 1.0);
		OpenGL.glMatrixMode(14);
		OpenGL.glLoadIdentity();
	}

	public virtual void setDefaultRealProjection()
	{
        OpenGL.glViewport(0.0, 0.0, FrameworkTypes.REAL_SCREEN_WIDTH, FrameworkTypes.REAL_SCREEN_HEIGHT);
		OpenGL.glMatrixMode(15);
		OpenGL.glLoadIdentity();
		OpenGL.glOrthof(0.0, FrameworkTypes.REAL_SCREEN_WIDTH, FrameworkTypes.REAL_SCREEN_HEIGHT, 0.0, -1.0, 1.0);
		OpenGL.glMatrixMode(14);
		OpenGL.glLoadIdentity();
	}

	public virtual void destroyFramebuffer()
	{
	}

	public virtual void touchesBeganwithEvent(TouchCollection touches)
	{
		if (touchDelegate != null)
		{
			touchDelegate.touchesBeganwithEvent(touches);
		}
	}

	public virtual void touchesMovedwithEvent(TouchCollection touches)
	{
		if (touchDelegate != null)
		{
			touchDelegate.touchesMovedwithEvent(touches);
		}
	}

	public virtual void touchesEndedwithEvent(TouchCollection touches)
	{
		if (touchDelegate != null)
		{
			touchDelegate.touchesEndedwithEvent(touches);
		}
	}

	public virtual void touchesCancelledwithEvent(TouchCollection touches)
	{
		if (touchDelegate != null)
		{
			touchDelegate.touchesCancelledwithEvent(touches);
		}
	}

	public virtual bool backButtonPressed()
	{
		if (touchDelegate != null)
		{
			return touchDelegate.backButtonPressed();
		}
		return false;
	}

	public virtual bool menuButtonPressed()
	{
		if (touchDelegate != null)
		{
			return touchDelegate.menuButtonPressed();
		}
		return false;
	}

	public override void dealloc()
	{
		NSObject.NSREL(fpsFont);
		NSObject.NSREL(fpsText);
		hide();
	}
}
