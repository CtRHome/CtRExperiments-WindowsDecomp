using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;

namespace CutTheRope.game;

internal class MechanicalHandButton : Button
{
	public MechanicalHandSegment segment;

	public override bool isInTouchZoneXYforTouchDown(float tx, float ty, bool td)
	{
		MechanicalHand theHand = segment.theHand;
		int index = theHand.segments.IndexOf(segment);
		return (double)MathHelper.vectDistance(MathHelper.vect(tx, ty), theHand.jointAtIndexPosition(index)) < 30.0;
	}
}
