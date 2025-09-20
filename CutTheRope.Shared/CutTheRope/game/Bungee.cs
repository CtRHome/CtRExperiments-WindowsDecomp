using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.sfe;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class Bungee : ConstraintSystem
{
	private enum BUNGEE_MODE
	{
		BUNGEE_MODE_NORMAL,
		BUNGEE_MODE_LOCKED
	}

	public const float BUNGEE_REST_LEN = 30f;

	private const float ROLLBACK_K = 0.5f;

	private const int BUNGEE_BEZIER_POINTS = 3;

	public const int BUNGEE_RELAXION_TIMES = 30;

	private const float MAX_BUNGEE_SEGMENTS = 20f;

	public const float DEFAULT_PART_WEIGHT = 0.02f;

	private const float STRENGTHENED_PART_WEIGHT = 0.5f;

	private const float CUT_DISSAPPEAR_TIMEOUT = 2f;

	private const float WHITE_TIMEOUT = 0.05f;

	private const float APPEAR_TIMEOUT = 1f;

	public ConstraintedPoint bungeeAnchor;

	public ConstraintedPoint tail;

	public int cut;

	public int relaxed;

	public float initialCandleAngle;

	public bool chosenOne;

	public int bungeeMode;

	public float partWeight;

	public bool forceWhite;

	public float cutTime;

	public float appearTime;

	public float[] drawPts = new float[200];

	public int drawPtsCount;

	public float lineWidth;

	public int width;

	public bool hideTailParts;

	public bool dontDrawRedStretch;

	public bool alternateColors;

	private static RGBAColor[] ccolors = new RGBAColor[8]
	{
		RGBAColor.transparentRGBA,
		RGBAColor.transparentRGBA,
		RGBAColor.transparentRGBA,
		RGBAColor.transparentRGBA,
		RGBAColor.transparentRGBA,
		RGBAColor.transparentRGBA,
		RGBAColor.transparentRGBA,
		RGBAColor.transparentRGBA
	};

	private float[] cverts = new float[16];

	private void drawAntialiasedLineContinued(float x1, float y1, float x2, float y2, float size, RGBAColor color, ref float lx, ref float ly, ref float rx, ref float ry)
	{
		Vector v = MathHelper.vect(x1, y1);
		Vector v2 = MathHelper.vect(x2, y2);
		Vector vector = MathHelper.vectSub(v2, v);
		Vector v3 = MathHelper.vectMult(vector, ((double)color.a == 1.0) ? 1.02f : 1f);
		Vector v4 = MathHelper.vectPerp(vector);
		Vector vector2 = MathHelper.vectNormalize(v4);
		v4 = MathHelper.vectMult(vector2, size);
		Vector v5 = MathHelper.vectNeg(v4);
		Vector v6 = MathHelper.vectAdd(v4, vector);
		Vector v7 = MathHelper.vectAdd(v5, vector);
		MathHelper.vectAdd2(ref v6, v);
		MathHelper.vectAdd2(ref v7, v);
		Vector v8 = MathHelper.vectAdd(v4, v3);
		Vector v9 = MathHelper.vectAdd(v5, v3);
		if (lx == -1f)
		{
			MathHelper.vectAdd2(ref v4, v);
			MathHelper.vectAdd2(ref v5, v);
		}
		else
		{
			v4 = MathHelper.vect(lx, ly);
			v5 = MathHelper.vect(rx, ry);
		}
		MathHelper.vectAdd2(ref v8, v);
		MathHelper.vectAdd2(ref v9, v);
		lx = v6.x;
		ly = v6.y;
		rx = v7.x;
		ry = v7.y;
		Vector vector3 = MathHelper.vectSub(v4, vector2);
		Vector vector4 = MathHelper.vectSub(v8, vector2);
		Vector vector5 = MathHelper.vectAdd(v5, vector2);
		Vector vector6 = MathHelper.vectAdd(v9, vector2);
		cverts[0] = v4.x;
		cverts[1] = v4.y;
		cverts[2] = v8.x;
		cverts[3] = v8.y;
		cverts[4] = vector3.x;
		cverts[5] = vector3.y;
		cverts[6] = vector4.x;
		cverts[7] = vector4.y;
		cverts[8] = vector5.x;
		cverts[9] = vector5.y;
		cverts[10] = vector6.x;
		cverts[11] = vector6.y;
		cverts[12] = v5.x;
		cverts[13] = v5.y;
		cverts[14] = v9.x;
		cverts[15] = v9.y;
		ref RGBAColor reference = ref ccolors[2];
		ref RGBAColor reference2 = ref ccolors[3];
		ref RGBAColor reference3 = ref ccolors[4];
		reference = (reference2 = (reference3 = (ccolors[5] = color)));
		OpenGL.glColorPointer_add(4, 5, 0, ccolors);
		OpenGL.glVertexPointer_add(2, 5, 0, cverts);
	}

	private void drawBungee(Bungee b, Vector[] pts, int count, int points)
	{
		float num = ((b.cut == -1 || b.forceWhite) ? 1f : (b.cutTime / 1.95f));
		if (b.appearTime != 1f)
		{
			num = b.appearTime / 1f;
		}
		if (num == 0f)
		{
			return;
		}
		RGBAColor rGBAColor = RGBAColor.MakeRGBA(0f, 0f, 0f, num);
		RGBAColor rGBAColor2 = RGBAColor.MakeRGBA(152.0 / 225.0, 0.44, 62.0 / 225.0, num);
		RGBAColor rGBAColor3 = RGBAColor.MakeRGBA(0.304, 0.198, 0.124, num);
		RGBAColor rGBAColor4 = RGBAColor.MakeRGBA(0.475, 0.305, 0.185, num);
		RGBAColor rGBAColor5 = RGBAColor.MakeRGBA(0.19, 0.122, 0.074, num);
		RGBAColor rGBAColor6 = (b.alternateColors ? rGBAColor : rGBAColor2);
		RGBAColor rGBAColor7 = (b.alternateColors ? rGBAColor : rGBAColor3);
		float num2 = MathHelper.vectDistance(MathHelper.vect(pts[0].x, pts[0].y), MathHelper.vect(pts[1].x, pts[1].y));
		if ((double)num2 <= 30.3)
		{
			b.relaxed = 0;
		}
		else if ((double)num2 <= 31.0)
		{
			b.relaxed = 1;
		}
		else if ((double)num2 <= 34.0)
		{
			b.relaxed = 2;
		}
		else
		{
			b.relaxed = 3;
		}
		if ((double)num2 > 37.0 && !b.dontDrawRedStretch)
		{
			float num3 = num2 / 30f * 2f;
			rGBAColor5.r *= num3;
			rGBAColor7.r *= num3;
		}
		bool flag = false;
		int num4 = (count - 1) * points;
		float[] array = new float[num4 * 2];
		b.drawPtsCount = num4 * 2;
		float num5 = 1f / (float)num4;
		float num6 = 0f;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		RGBAColor rGBAColor8 = rGBAColor5;
		RGBAColor rGBAColor9 = rGBAColor7;
		float num10 = (rGBAColor4.r - rGBAColor5.r) / (float)(num4 - 1);
		float num11 = (rGBAColor4.g - rGBAColor5.g) / (float)(num4 - 1);
		float num12 = (rGBAColor4.b - rGBAColor5.b) / (float)(num4 - 1);
		float num13 = (rGBAColor6.r - rGBAColor7.r) / (float)(num4 - 1);
		float num14 = (rGBAColor6.g - rGBAColor7.g) / (float)(num4 - 1);
		float num15 = (rGBAColor6.b - rGBAColor7.b) / (float)(num4 - 1);
		float lx = -1f;
		float ly = -1f;
		float rx = -1f;
		float ry = -1f;
		OpenGL.glDisableClientState(0);
		OpenGL.glEnableClientState(13);
		while (true)
		{
			if ((double)num6 > 0.99)
			{
				num6 = 1f;
			}
			if (count < 3)
			{
				break;
			}
			Vector vector = GLDrawer.calcPathBezier(pts, count, num6);
			array[num7++] = vector.x;
			array[num7++] = vector.y;
			b.drawPts[num8++] = vector.x;
			b.drawPts[num8++] = vector.y;
			if (num7 >= 6 || (double)num6 >= 1.0)
			{
				RGBAColor color = (b.forceWhite ? RGBAColor.whiteRGBA : ((!flag) ? rGBAColor9 : rGBAColor8));
				OpenGL.glColor4f(color.r, color.g, color.b, color.a);
				int num16 = num7 >> 1;
				OpenGL.glVertexPointer_setAdditive(2, 5, 0, 16 * (num16 - 1));
				OpenGL.glColorPointer_setAdditive(8 * (num16 - 1));
				for (int i = 0; i < num16 - 1; i++)
				{
					drawAntialiasedLineContinued(array[i * 2], array[i * 2 + 1], array[i * 2 + 2], array[i * 2 + 3], b.width, color, ref lx, ref ly, ref rx, ref ry);
				}
				OpenGL.glDrawArrays(8, 0, 8);
				array[0] = array[num7 - 2];
				array[1] = array[num7 - 1];
				num7 = 2;
				flag = !flag;
				num9++;
				rGBAColor8.r += num10 * (float)(num16 - 1);
				rGBAColor8.g += num11 * (float)(num16 - 1);
				rGBAColor8.b += num12 * (float)(num16 - 1);
				rGBAColor9.r += num13 * (float)(num16 - 1);
				rGBAColor9.g += num14 * (float)(num16 - 1);
				rGBAColor9.b += num15 * (float)(num16 - 1);
			}
			if ((double)num6 >= 1.0)
			{
				break;
			}
			num6 += num5;
		}
		OpenGL.glEnableClientState(0);
		OpenGL.glDisableClientState(13);
	}

	public virtual NSObject initWithHeadAtXYTailAtTXTYandLength(ConstraintedPoint h, float hx, float hy, ConstraintedPoint t, float tx, float ty, float len)
	{
		if (init() != null)
		{
			relaxationTimes = 30;
			lineWidth = 3f;
			width = 2;
			cut = -1;
			bungeeMode = 0;
			if (h != null)
			{
				bungeeAnchor = h;
			}
			else
			{
				bungeeAnchor = (ConstraintedPoint)new ConstraintedPoint().init();
			}
			if (t != null)
			{
				tail = t;
			}
			else
			{
				tail = (ConstraintedPoint)new ConstraintedPoint().init();
				tail.setWeight(1f);
			}
			bungeeAnchor.setWeight(0.02f);
			bungeeAnchor.pos = MathHelper.vect(hx, hy);
			tail.pos = MathHelper.vect(tx, ty);
			addPart(bungeeAnchor);
			addPart(tail);
			tail.addConstraintwithRestLengthofType(bungeeAnchor, 30f, Constraint.CONSTRAINT.CONSTRAINT_DISTANCE);
			Vector v = MathHelper.vectSub(tail.pos, bungeeAnchor.pos);
			int num = (int)(len / 30f + 2f);
			v = MathHelper.vectDiv(v, num);
			rollplacingWithOffset(len, v);
			forceWhite = false;
			initialCandleAngle = -1f;
			chosenOne = false;
			hideTailParts = false;
			dontDrawRedStretch = false;
			alternateColors = false;
			appearTime = 1f;
		}
		return this;
	}

	public virtual int getLength()
	{
		if (this == null)
		{
			return 0;
		}
		int num = 0;
		Vector pos = MathHelper.vectZero;
		int count = parts.Count;
		for (int i = 0; i < count; i++)
		{
			ConstraintedPoint constraintedPoint = parts[i];
			if (i > 0)
			{
				num += (int)MathHelper.vectDistance(pos, constraintedPoint.pos);
			}
			pos = constraintedPoint.pos;
		}
		return num;
	}

	public virtual float rollBack(float amount)
	{
		float num = amount;
		ConstraintedPoint n = parts[parts.Count - 2];
		int num2 = (int)tail.restLengthFor(n);
		int num3 = parts.Count;
		while (num > 0f)
		{
			if (num >= 30f)
			{
				ConstraintedPoint o = parts[num3 - 2];
				ConstraintedPoint n2 = parts[num3 - 3];
				tail.changeConstraintFromTowithRestLength(o, n2, num2);
				parts.RemoveAt(parts.Count - 2);
				num3--;
				num -= 30f;
				continue;
			}
			int num4 = (int)((float)num2 - num);
			if (num4 < 1)
			{
				num = 30f;
				num2 = (int)(30f + (float)num4 + 1f);
			}
			else
			{
				ConstraintedPoint n3 = parts[num3 - 2];
				tail.changeRestLengthToFor(num4, n3);
				num = 0f;
			}
		}
		int count = tail.constraints.Count;
		for (int i = 0; i < count; i++)
		{
			Constraint constraint = tail.constraints[i];
			if (constraint != null && constraint.type == Constraint.CONSTRAINT.CONSTRAINT_NOT_MORE_THAN)
			{
				constraint.restLength = (float)(num3 - 1) * 33f;
			}
		}
		return num;
	}

	public virtual void roll(float rollLen)
	{
		rollplacingWithOffset(rollLen, MathHelper.vectZero);
	}

	public virtual void rollplacingWithOffset(float rollLen, Vector off)
	{
		ConstraintedPoint n = parts[parts.Count - 2];
		int num = (int)tail.restLengthFor(n);
		while (rollLen > 0f)
		{
			if (rollLen >= 30f)
			{
				ConstraintedPoint constraintedPoint = parts[parts.Count - 2];
				ConstraintedPoint constraintedPoint2 = (ConstraintedPoint)new ConstraintedPoint().init();
				constraintedPoint2.setWeight(0.02f);
				constraintedPoint2.pos = MathHelper.vectAdd(constraintedPoint.pos, off);
				addPartAt(constraintedPoint2, parts.Count - 1);
				tail.changeConstraintFromTowithRestLength(constraintedPoint, constraintedPoint2, num);
				constraintedPoint2.addConstraintwithRestLengthofType(constraintedPoint, 30f, Constraint.CONSTRAINT.CONSTRAINT_DISTANCE);
				rollLen -= 30f;
			}
			else
			{
				int num2 = (int)(rollLen + (float)num);
				if ((float)num2 > 30f)
				{
					rollLen = 30f;
					num = (int)((float)num2 - 30f);
				}
				else
				{
					ConstraintedPoint n2 = parts[parts.Count - 2];
					tail.changeRestLengthToFor(num2, n2);
					rollLen = 0f;
				}
			}
		}
	}

	public virtual void removePart(int part)
	{
		forceWhite = false;
		ConstraintedPoint constraintedPoint = parts[part];
		ConstraintedPoint constraintedPoint2 = ((part + 1 >= parts.Count) ? null : parts[part + 1]);
		if (constraintedPoint2 == null)
		{
			constraintedPoint.removeConstraints();
		}
		else
		{
			for (int i = 0; i < constraintedPoint2.constraints.Count; i++)
			{
				Constraint constraint = constraintedPoint2.constraints[i];
				if (constraint.cp == constraintedPoint)
				{
					constraintedPoint2.constraints.Remove(constraint);
					ConstraintedPoint constraintedPoint3 = (ConstraintedPoint)new ConstraintedPoint().init();
					constraintedPoint3.setWeight(1E-05f);
					constraintedPoint3.pos = constraintedPoint2.pos;
					constraintedPoint3.prevPos = constraintedPoint2.prevPos;
					addPartAt(constraintedPoint3, part + 1);
					constraintedPoint3.addConstraintwithRestLengthofType(constraintedPoint, 30f, Constraint.CONSTRAINT.CONSTRAINT_DISTANCE);
					break;
				}
			}
		}
		for (int j = 0; j < parts.Count; j++)
		{
			ConstraintedPoint constraintedPoint4 = parts[j];
			if (constraintedPoint4 != tail)
			{
				constraintedPoint4.setWeight(1E-05f);
			}
		}
	}

	public virtual void setCut(int part)
	{
		cut = part;
		cutTime = 2f;
		forceWhite = true;
	}

	public virtual void strengthen()
	{
		int count = parts.Count;
		for (int i = 0; i < count; i++)
		{
			ConstraintedPoint constraintedPoint = parts[i];
			if (constraintedPoint == null)
			{
				continue;
			}
			if (bungeeAnchor.pin.x != -1f)
			{
				if (constraintedPoint != tail)
				{
					constraintedPoint.setWeight(0.5f);
				}
				if (i != 0)
				{
					constraintedPoint.addConstraintwithRestLengthofType(bungeeAnchor, (float)i * 33f, Constraint.CONSTRAINT.CONSTRAINT_NOT_MORE_THAN);
				}
			}
			i++;
		}
	}

	public override void update(float delta)
	{
		update(delta, 1f);
	}

	public virtual void update(float delta, float koeff)
	{
		if ((double)cutTime > 0.0)
		{
			Mover.moveVariableToTarget(ref cutTime, 0f, 1f, delta);
			if (cutTime < 1.95f && forceWhite)
			{
				removePart(cut);
			}
		}
		if (appearTime != 1f)
		{
			Mover.moveVariableToTarget(ref appearTime, 1f, 1f, delta);
		}
		int count = parts.Count;
		for (int i = 0; i < count; i++)
		{
			ConstraintedPoint constraintedPoint = parts[i];
			if (constraintedPoint != tail)
			{
				ConstraintedPoint.qcpupdate(constraintedPoint, delta, koeff);
			}
		}
		for (int j = 0; j < relaxationTimes; j++)
		{
			int count2 = parts.Count;
			for (int k = 0; k < count2; k++)
			{
				ConstraintedPoint p = parts[k];
				ConstraintedPoint.satisfyConstraints(p);
			}
		}
	}

	public override void draw()
	{
		int count = parts.Count;
		OpenGL.SetRopeColor();
		if (cut == -1)
		{
			Vector[] array = new Vector[count];
			for (int i = 0; i < count; i++)
			{
				ConstraintedPoint constraintedPoint = parts[i];
				ref Vector reference = ref array[i];
				reference = constraintedPoint.pos;
			}
			drawBungee(this, array, count, 3);
			return;
		}
		Vector[] array2 = new Vector[count];
		Vector[] array3 = new Vector[count];
		bool flag = false;
		int num = 0;
		for (int j = 0; j < count; j++)
		{
			ConstraintedPoint constraintedPoint2 = parts[j];
			bool flag2 = true;
			if (j > 0)
			{
				ConstraintedPoint p = parts[j - 1];
				if (!constraintedPoint2.hasConstraintTo(p))
				{
					flag2 = false;
				}
			}
			if (constraintedPoint2.pin.x == -1f && !flag2)
			{
				flag = true;
				ref Vector reference2 = ref array2[j];
				reference2 = constraintedPoint2.pos;
			}
			if (!flag)
			{
				ref Vector reference3 = ref array2[j];
				reference3 = constraintedPoint2.pos;
			}
			else
			{
				ref Vector reference4 = ref array3[num];
				reference4 = constraintedPoint2.pos;
				num++;
			}
		}
		int num2 = count - num;
		if (num2 > 0)
		{
			drawBungee(this, array2, num2, 3);
		}
		if (num > 0 && !hideTailParts)
		{
			drawBungee(this, array3, num, 3);
		}
	}

	public override void dealloc()
	{
		base.dealloc();
	}
}
