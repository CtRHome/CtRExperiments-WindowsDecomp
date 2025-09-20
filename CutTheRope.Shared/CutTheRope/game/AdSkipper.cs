using CutTheRope.ctr_original;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class AdSkipper : BaseElement, ButtonDelegate
{
	public const int BUTTON_SKIP_AD = 0;

	private Button skipAd;

	private object skipper;

	public float timerNoDraw;

	public bool active;

	public override NSObject init()
	{
		if (base.init() != null)
		{
			timerNoDraw = 0f;
			active = false;
			skipper = null;
			skipAd = MenuController.createButtonWithTextIDDelegate(Application.getString(1179724), 0, this);
			skipAd.anchor = (skipAd.parentAnchor = 34);
			skipAd.setEnabled(e: false);
			addChild(skipAd);
			visible = false;
			anchor = (parentAnchor = 34);
		}
		return this;
	}

	public virtual void setJskipper(object jskipper)
	{
		freeJskipper();
		skipper = jskipper;
		active = true;
		skipAd.setEnabled(e: true);
	}

	public virtual void freeJskipper()
	{
		if (skipper != null)
		{
			timerNoDraw = 0f;
			skipper = null;
			active = false;
			skipAd.setEnabled(e: false);
		}
	}

	public override void dealloc()
	{
		freeJskipper();
		base.dealloc();
	}

	public override void update(float delta)
	{
		base.update(delta);
		if (active)
		{
			timerNoDraw += delta;
		}
	}

	public virtual void onButtonPressed(int n)
	{
		if (active)
		{
			_ = 0;
		}
	}
}
