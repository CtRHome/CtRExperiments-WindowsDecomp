using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class CTRGameObject : GameObject
{
	public const int MIN_CICRLE_POINTS = 10;

	public override void parseMover(XMLNode xml)
	{
		rotation = 0f;
		NSString nSString = xml["angle"];
		if (nSString != null)
		{
			rotation = nSString.floatValue();
		}
		NSString nSString2 = xml["path"];
		if (nSString2 != null && nSString2.length() != 0)
		{
			int l = 100;
			if (nSString2.characterAtIndex(0) == 'R')
			{
				NSString nSString3 = nSString2.substringFromIndex(2);
				int num = nSString3.intValue();
				l = MathHelper.MAX(11, num / 2 + 1);
			}
			float num2 = xml["moveSpeed"].floatValue();
			float m_ = num2;
			float r_ = xml["rotateSpeed"].floatValue();
			CTRMover cTRMover = (CTRMover)new CTRMover().initWithPathCapacityMoveSpeedRotateSpeed(l, m_, r_);
			cTRMover.angle = rotation;
			cTRMover.setPathFromStringandStart(nSString2, new Vector(x, y));
			setMover(cTRMover);
			cTRMover.start();
		}
	}

	public static CTRGameObject CTRGameObject_createWithResIDQuad(int r, int q)
	{
		CTRGameObject cTRGameObject = (CTRGameObject)new CTRGameObject().initWithTexture(Application.getTexture(r));
		cTRGameObject.setDrawQuad(q);
		return cTRGameObject;
	}
}
