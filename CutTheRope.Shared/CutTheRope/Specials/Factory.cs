using CutTheRope.game;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;

namespace CutTheRope.Specials;

internal class Factory
{
	public static void showProcessingOnViewwithTouchesBlocking(View v, bool b)
	{
		Processing processing = (Processing)new Processing().initWithTouchesBlocking(b);
		processing.setName("processing");
		v.addChild(processing);
	}

	public static void hideProcessingFromView(View v)
	{
		BaseElement childWithName = v.getChildWithName("processing");
		if (childWithName != null)
		{
			v.removeChild(childWithName);
		}
	}
}
