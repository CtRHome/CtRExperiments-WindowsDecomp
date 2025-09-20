using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.iframework.visual;

internal interface TouchDelegate
{
	bool touchesBeganwithEvent(TouchCollection touches);

	bool touchesEndedwithEvent(TouchCollection touches);

	bool touchesMovedwithEvent(TouchCollection touches);

	bool touchesCancelledwithEvent(TouchCollection touches);

	bool backButtonPressed();

	bool menuButtonPressed();
}
