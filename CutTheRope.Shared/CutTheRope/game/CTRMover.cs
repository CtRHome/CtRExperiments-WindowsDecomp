using System;
using System.Collections.Generic;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class CTRMover : Mover
{
	public override void setPathFromStringandStart(NSString p, Vector s)
	{
		if (p.characterAtIndex(0) == 'R')
		{
			bool flag = p.characterAtIndex(1) == 'C';
			NSString nSString = p.substringFromIndex(2);
			int num = nSString.intValue();
			int num2 = MathHelper.MAX(10, num / 2);
			float num3 = (float)(Math.PI * 2.0 / (double)num2);
			if (!flag)
			{
				num3 = 0f - num3;
			}
			float num4 = 0f;
			for (int i = 0; i < num2; i++)
			{
				float xParam = (float)((double)s.x + (double)num * Math.Cos(num4));
				float yParam = (float)((double)s.y + (double)num * Math.Sin(num4));
				addPathPoint(new Vector(xParam, yParam));
				num4 += num3;
			}
		}
		else
		{
			addPathPoint(s);
			if (p.characterAtIndex(p.length() - 1) == ',')
			{
				p = p.substringToIndex(p.length() - 1);
			}
			List<NSString> list = p.componentsSeparatedByString(',');
			for (int j = 0; j < list.Count; j += 2)
			{
				NSString nSString2 = list[j];
				NSString nSString3 = list[j + 1];
				addPathPoint(new Vector(s.x + nSString2.floatValue(), s.y + nSString3.floatValue()));
			}
		}
	}
}
