using System;
using System.Collections.Generic;
using CutTheRope.iframework.visual;
using CutTheRope.utils;
using CutTheRope.utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.iframework;

internal partial class OpenGL
{
	private class GLVertexPointer
	{
		public int size_;

		public float[] pointer_;

		public int Count
		{
			get
			{
				if (pointer_ == null || size_ == 0)
				{
					return 0;
				}
				return pointer_.Length / size_;
			}
		}

		public GLVertexPointer(int size, int type, int stride, object pointer)
		{
			pointer_ = ((pointer != null) ? ((float[])pointer) : null);
			size_ = size;
		}
	}

	private class GLTexCoordPointer
	{
		public int size_;

		public float[] pointer_;

		public int Count
		{
			get
			{
				if (pointer_ == null || size_ == 0)
				{
					return 0;
				}
				return pointer_.Length / size_;
			}
		}

		public GLTexCoordPointer(int size, int type, int stride, object pointer)
		{
			pointer_ = ((pointer != null) ? ((float[])pointer) : null);
			size_ = size;
		}
	}

	public const int GL_TEXTURE_2D = 0;

	public const int GL_BLEND = 1;

	public const int GL_ARRAY_BUFFER = 2;

	public const int GL_DYNAMIC_DRAW = 3;

	public const int GL_SCISSOR_TEST = 4;

	public const int GL_FLOAT = 5;

	public const int GL_UNSIGNED_SHORT = 6;

	public const int GL_TRIANGLES = 7;

	public const int GL_TRIANGLE_STRIP = 8;

	public const int GL_LINE_STRIP = 9;

	public const int GL_POINTS = 10;

	public const int GL_VERTEX_ARRAY = 11;

	public const int GL_TEXTURE_COORD_ARRAY = 12;

	public const int GL_COLOR_ARRAY = 13;

	public const int GL_MODELVIEW = 14;

	public const int GL_PROJECTION = 15;

	public const int GL_TEXTURE = 16;

	public const int GL_COLOR = 17;

	private static Dictionary<int, bool> s_glServerSideFlags = new Dictionary<int, bool>();

	private static Dictionary<int, bool> s_glClientStateFlags = new Dictionary<int, bool>();

	private static Viewport s_Viewport = default(Viewport);

	private static int s_glMatrixMode;

	private static List<Matrix> s_matrixModelViewStack = new List<Matrix>();

	private static Matrix s_matrixModelView = Matrix.Identity;

	private static Matrix s_matrixProjection = Matrix.Identity;

	private static CutTheRope.iframework.visual.Texture2D s_Texture;

	private static CutTheRope.iframework.visual.Texture2D s_Texture_OptimizeLastUsed;

	private static Color s_glClearColor = Color.White;

	private static Color s_Color = Color.White;

	private static Color RopeColor = new Color(0f, 0f, 0.4f, 1f);

	private static BlendParams s_Blend = new BlendParams();

	private static RGBAColor[] s_GLColorPointer;

	private static Dictionary<int, RGBAColor[]> RGBAColorArray = new Dictionary<int, RGBAColor[]>();

	private static GLVertexPointer s_GLVertexPointer;

	private static GLTexCoordPointer s_GLTexCoordPointer;

	private static int s_GLColorPointer_additive_position;

	private static int s_GLVertexPointer_additive_position;

	private static Dictionary<int, float[]> FloatArray = new Dictionary<int, float[]>();

	private static Dictionary<int, VertexPositionColor[]> VertexPositionColorArray = new Dictionary<int, VertexPositionColor[]>();

	private static Dictionary<int, VertexPositionNormalTexture[]> VertexPositionNormalTextureArray = new Dictionary<int, VertexPositionNormalTexture[]>();

	private static Dictionary<int, VertexPositionColorTexture[]> VertexPositionColorTextureArray = new Dictionary<int, VertexPositionColorTexture[]>();

	private static BasicEffect s_effectTexture;

	private static BasicEffect s_effectColor;

	private static BasicEffect s_effectTextureColor;

	private static RasterizerState s_rasterizerStateSolidColor;

	private static RasterizerState s_rasterizerStateTexture;

	private static VertexPositionNormalTexture[] s_LastVertices_PositionNormalTexture = null;

	public static void glGenTextures(int n, object textures)
	{
	}

	public static void glBindTexture(int target, uint texture)
	{
	}

	public static void glEnable(int cap)
	{
		_ = 4;
		if (cap == 1)
		{
			s_Blend.enable();
		}
		s_glServerSideFlags[cap] = true;
	}

	public static void glDisable(int cap)
	{
		if (cap == 4)
		{
			glScissor(0.0, 0.0, FrameworkTypes.REAL_SCREEN_WIDTH, FrameworkTypes.REAL_SCREEN_HEIGHT);
		}
		if (cap == 1)
		{
			s_Blend.disable();
		}
		s_glServerSideFlags[cap] = false;
	}

	public static void glEnableClientState(int cap)
	{
		s_glClientStateFlags[cap] = true;
	}

	public static void glDisableClientState(int cap)
	{
		s_glClientStateFlags[cap] = false;
	}

	public static void glViewport(double x, double y, double width, double height)
	{
		glViewport((int)x, (int)y, (int)width, (int)height);
	}

	public static void glViewport(int x, int y, int width, int height)
	{
		s_Viewport.X = x;
		s_Viewport.Y = y;
		s_Viewport.Width = width;
		s_Viewport.Height = height;
	}

	public static void glMatrixMode(int mode)
	{
		s_glMatrixMode = mode;
	}

	public static void glLoadIdentity()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (s_glMatrixMode == 14)
		{
			s_matrixModelView = Matrix.Identity;
			return;
		}
		if (s_glMatrixMode == 15)
		{
			s_matrixProjection = Matrix.Identity;
			return;
		}
		if (s_glMatrixMode == 16)
		{
			throw new NotImplementedException();
		}
		if (s_glMatrixMode != 17)
		{
			return;
		}
		throw new NotImplementedException();
	}

	public static void glOrthof(double left, double right, double bottom, double top, double near, double far)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		s_matrixProjection = Matrix.CreateOrthographicOffCenter((float)left, (float)right, (float)bottom, (float)top, (float)near, (float)far);
	}

	public static void glPopMatrix()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (s_matrixModelViewStack.Count > 0)
		{
			int index = s_matrixModelViewStack.Count - 1;
			s_matrixModelView = s_matrixModelViewStack[index];
			s_matrixModelViewStack.RemoveAt(index);
		}
	}

	public static void glPushMatrix()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		s_matrixModelViewStack.Add(s_matrixModelView);
	}

	public static void glScalef(float x, float y, float z)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		s_matrixModelView = Matrix.CreateScale(x, y, z) * s_matrixModelView;
	}

	public static void glRotatef(double angle, double x, double y, double z)
	{
		glRotatef((float)angle, (float)x, (float)y, (float)z);
	}

	public static void glRotatef(float angle, float x, float y, float z)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		s_matrixModelView = Matrix.CreateRotationZ(MathHelper.ToRadians(angle)) * s_matrixModelView;
	}

	public static void glTranslatef(double x, double y, double z)
	{
		glTranslatef((float)x, (float)y, (float)z);
	}

	public static void glTranslatef(float x, float y, float z)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		s_matrixModelView = Matrix.CreateTranslation(x, y, 0f) * s_matrixModelView;
	}

	public static void glBindTexture(CutTheRope.iframework.visual.Texture2D t)
	{
		s_Texture = t;
	}

	public static void glClearColor(double red, double green, double blue, double alpha)
	{
		s_glClearColor = new Color((float)red, (float)green, (float)blue, (float)alpha);
	}

	public static void glClear(int mask_NotUsedParam)
	{
		BlendParams.applyDefault();
		Global.GraphicsDevice.Clear(s_glClearColor);
	}

	public static void glColor4f(double red, double green, double blue, double alpha)
	{
		glColor4f((float)red, (float)green, (float)blue, (float)alpha);
	}

	public static void glColor4f(float red, float green, float blue, float alpha)
	{
		s_Color = new Color(red, green, blue, alpha);
	}

	public static void SetWhiteColor()
	{
		s_Color = Color.White;
	}

	public static void SetRopeColor()
	{
		s_Color = RopeColor;
	}

	public static void glBlendFunc(BlendingFactor sfactor, BlendingFactor dfactor)
	{
		s_Blend = new BlendParams(sfactor, dfactor);
	}

	public static void drawSegment(float x1, float y1, float x2, float y2, RGBAColor color)
	{
	}

	public static void glGenBuffers(int n, ref uint buffer)
	{
	}

	public static void glGenBuffers(int n, ref uint[] buffers)
	{
	}

	public static void glDeleteBuffers(int n, ref uint[] buffers)
	{
	}

	public static void glDeleteBuffers(int n, ref uint buffers)
	{
	}

	public static void glBindBuffer(int target, uint buffer)
	{
	}

	public static void glBufferData(int target, RGBAColor[] data, int usage)
	{
	}

	public static void glBufferData(int target, PointSprite[] data, int usage)
	{
	}

	public static void glColorPointer(int size, int type, int stride, RGBAColor[] pointer)
	{
		s_GLColorPointer = pointer;
	}

	public static void glVertexPointer(int size, int type, int stride, object pointer)
	{
		s_GLVertexPointer = new GLVertexPointer(size, type, stride, pointer);
	}

	public static void glTexCoordPointer(int size, int type, int stride, object pointer)
	{
		s_GLTexCoordPointer = new GLTexCoordPointer(size, type, stride, pointer);
	}

	public static void glColorPointer_setAdditive(int size)
	{
		if (!RGBAColorArray.TryGetValue(size, out s_GLColorPointer))
		{
			s_GLColorPointer = new RGBAColor[size];
			RGBAColorArray.Add(size, s_GLColorPointer);
		}
		s_GLColorPointer_additive_position = 0;
	}

	public static void glColorPointer_add(int size, int type, int stride, RGBAColor[] pointer)
	{
		pointer.CopyTo(s_GLColorPointer, s_GLColorPointer_additive_position);
		s_GLColorPointer_additive_position += pointer.Length;
	}

	public static void glVertexPointer_setAdditive(int size, int type, int stride, int length)
	{
		if (!FloatArray.TryGetValue(length, out var value))
		{
			value = new float[length];
			FloatArray.Add(length, value);
		}
		s_GLVertexPointer = new GLVertexPointer(size, type, stride, value);
		s_GLVertexPointer_additive_position = 0;
	}

	public static void glVertexPointer_add(int size, int type, int stride, float[] pointer)
	{
		pointer.CopyTo(s_GLVertexPointer.pointer_, s_GLVertexPointer_additive_position);
		s_GLVertexPointer_additive_position += pointer.Length;
	}

	public static void glDrawArrays(int mode, int first, int count)
	{
		switch (mode)
		{
		case 8:
			DrawTriangleStrip(first, count);
			break;
		case 9:
		case 10:
			break;
		default:
			throw new NotImplementedException();
		}
	}

	private static VertexPositionColor[] ConstructColorVertices()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (!VertexPositionColorArray.TryGetValue(s_GLVertexPointer.Count, out var value))
		{
			value = (VertexPositionColor[])(object)new VertexPositionColor[s_GLVertexPointer.Count];
			VertexPositionColorArray.Add(s_GLVertexPointer.Count, value);
		}
		int num = 0;
		Vector3 val = default(Vector3);
		for (int i = 0; i < value.Length; i++)
		{
			val.X = s_GLVertexPointer.pointer_[num++];
			val.Y = s_GLVertexPointer.pointer_[num++];
			if (s_GLVertexPointer.size_ == 2)
			{
				val.Z = 0f;
			}
			else
			{
				val.Z = s_GLVertexPointer.pointer_[num++];
			}
			Color color = s_GLColorPointer[i].toXNA();
			ref VertexPositionColor reference = ref value[i];
			reference = new VertexPositionColor(val, color);
		}
		return value;
	}

	private static VertexPositionColor[] ConstructCurrentColorVertices()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (!VertexPositionColorArray.TryGetValue(s_GLVertexPointer.Count, out var value))
		{
			value = (VertexPositionColor[])(object)new VertexPositionColor[s_GLVertexPointer.Count];
			VertexPositionColorArray.Add(s_GLVertexPointer.Count, value);
		}
		int num = 0;
		Vector3 val = default(Vector3);
		for (int i = 0; i < value.Length; i++)
		{
			val.X = s_GLVertexPointer.pointer_[num++];
			val.Y = s_GLVertexPointer.pointer_[num++];
			if (s_GLVertexPointer.size_ == 2)
			{
				val.Z = 0f;
			}
			else
			{
				val.Z = s_GLVertexPointer.pointer_[num++];
			}
			ref VertexPositionColor reference = ref value[i];
			reference = new VertexPositionColor(val, s_Color);
		}
		s_GLVertexPointer = null;
		return value;
	}

	private static short[] InitializeTriangleStripIndices(int count)
	{
		short[] array = new short[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = (short)i;
		}
		return array;
	}

	private static VertexPositionNormalTexture[] ConstructTexturedVertices()
	{
		Vector3 val = new(0f, 0f, 1f);
		if (!VertexPositionNormalTextureArray.TryGetValue(s_GLVertexPointer.Count, out var value))
		{
			value = (VertexPositionNormalTexture[])(object)new VertexPositionNormalTexture[s_GLVertexPointer.Count];
			VertexPositionNormalTextureArray.Add(s_GLVertexPointer.Count, value);
		}
		int num = 0;
		int num2 = 0;
		Vector3 val2 = new();
		for (int i = 0; i < value.Length; i++)
		{
			val2.X = s_GLVertexPointer.pointer_[num++];
			val2.Y = s_GLVertexPointer.pointer_[num++];
			if (s_GLVertexPointer.size_ == 2)
			{
				val2.Z = 0f;
			}
			else
			{
				val2.Z = s_GLVertexPointer.pointer_[num++];
			}
			Vector2 val3 = new();
			val3.X = s_GLTexCoordPointer.pointer_[num2++];
			val3.Y = s_GLTexCoordPointer.pointer_[num2++];
			int num3 = 2;
			while (num3 < s_GLTexCoordPointer.size_)
			{
				num3++;
				num2++;
			}
            value[i] = new VertexPositionNormalTexture(val2, val, val3);
		}
		s_GLTexCoordPointer = null;
		s_GLVertexPointer = null;
		return value;
	}

	private static VertexPositionColorTexture[] ConstructTexturedColoredVertices(int VertexCount)
	{
		if (!VertexPositionColorTextureArray.TryGetValue(VertexCount, out var value))
		{
			value = (VertexPositionColorTexture[])(object)new VertexPositionColorTexture[VertexCount];
			VertexPositionColorTextureArray.Add(VertexCount, value);
		}
		int num = 0;
		int num2 = 0;
		Vector3 val = default(Vector3);
		for (int i = 0; i < value.Length; i++)
		{
			val.X = s_GLVertexPointer.pointer_[num++];
			val.Y = s_GLVertexPointer.pointer_[num++];
			if (s_GLVertexPointer.size_ == 2)
			{
				val.Z = 0f;
			}
			else
			{
				val.Z = s_GLVertexPointer.pointer_[num++];
			}
			Vector2 val2 = default(Vector2);
			val2.X = s_GLTexCoordPointer.pointer_[num2++];
			val2.Y = s_GLTexCoordPointer.pointer_[num2++];
			int num3 = 2;
			while (num3 < s_GLTexCoordPointer.size_)
			{
				num3++;
				num2++;
			}
			Color color = s_GLColorPointer[i].toXNA();
            value[i] = new VertexPositionColorTexture(val, color, val2);
		}
		s_GLTexCoordPointer = null;
		s_GLVertexPointer = null;
		return value;
	}

	public static void Init()
	{
		InitRasterizerState();
		s_glServerSideFlags[0] = true;
		s_glClientStateFlags[0] = true;
		s_effectTexture = new BasicEffect(Global.GraphicsDevice);
		s_effectTexture.VertexColorEnabled = false;
		s_effectTexture.TextureEnabled = true;
		s_effectTexture.View = Matrix.Identity;
		s_effectTextureColor = new BasicEffect(Global.GraphicsDevice);
		s_effectTextureColor.VertexColorEnabled = true;
		s_effectTextureColor.TextureEnabled = true;
		s_effectTextureColor.View = Matrix.Identity;
		s_effectColor = new BasicEffect(Global.GraphicsDevice);
		s_effectColor.VertexColorEnabled = true;
		s_effectColor.TextureEnabled = false;
		s_effectColor.Alpha = 1f;
		s_effectColor.Texture = null;
		s_effectColor.View = Matrix.Identity;
	}

	private static BasicEffect getEffect(bool useTexture, bool useColor)
	{
		BasicEffect val = ((!useTexture) ? s_effectColor : (useColor ? s_effectTextureColor : s_effectTexture));
		if (useTexture)
		{
			val.Alpha = (float)(int)s_Color.A / 255f;
			if (val.Alpha == 0f)
			{
				return val;
			}
			val.Texture = s_Texture.xnaTexture_;
			val.DiffuseColor = new Vector3((float)(int)s_Color.R / 255f, (float)(int)s_Color.G / 255f, (float)(int)s_Color.B / 255f);
			Global.GraphicsDevice.RasterizerState = s_rasterizerStateTexture;
			Global.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
		}
		else
		{
			Global.GraphicsDevice.RasterizerState = s_rasterizerStateSolidColor;
		}
		val.World = s_matrixModelView;
		val.Projection = s_matrixProjection;
		s_Blend.apply();
		return val;
	}

	private static void InitRasterizerState()
	{
		s_rasterizerStateSolidColor = new RasterizerState();
		s_rasterizerStateSolidColor.FillMode = FillMode.Solid;
		s_rasterizerStateSolidColor.CullMode = CullMode.None;
		s_rasterizerStateSolidColor.ScissorTestEnable = true;
		s_rasterizerStateTexture = new RasterizerState();
		s_rasterizerStateTexture.CullMode = CullMode.None;
		s_rasterizerStateTexture.ScissorTestEnable = true;
	}

	private static void DrawTriangleStrip(int first, int count)
	{
		bool value = false;
		s_glServerSideFlags.TryGetValue(0, out value);
		if (value)
		{
			s_glClientStateFlags.TryGetValue(0, out value);
		}
		if (value)
		{
			DrawTriangleStripTextured(first, count);
		}
		else
		{
			DrawTriangleStripColored(first, count);
		}
	}

	private static void DrawTriangleStripColored(int first, int count)
	{
		BasicEffect effect = getEffect(useTexture: false, useColor: true);
		if (effect.Alpha == 0f)
		{
			return;
		}
		bool value = false;
		s_glClientStateFlags.TryGetValue(13, out value);
		VertexPositionColor[] array = (value ? ConstructColorVertices() : ConstructCurrentColorVertices());
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Apply();
			Global.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, array, 0, array.Length - 2);
		}
	}

	private static void DrawTriangleStripTextured(int first, int count)
	{
		BasicEffect effect = getEffect(useTexture: true, useColor: false);
		if (effect.Alpha == 0f)
		{
			return;
		}
		VertexPositionNormalTexture[] array = ConstructTexturedVertices();
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Apply();
			Global.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, array, 0, array.Length - 2);
		}
	}

	public static VertexPositionNormalTexture[] GetLastVertices_PositionNormalTexture()
	{
		return s_LastVertices_PositionNormalTexture;
	}

	public static void Optimized_DrawTriangleList(VertexPositionNormalTexture[] vertices, short[] indices)
	{
		BasicEffect effect = getEffect(useTexture: true, useColor: false);
		if (effect.Alpha == 0f)
		{
			return;
		}
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Apply();
			Global.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
		}
	}

	private static void DrawTriangleList(int first, int count, short[] indices)
	{
		bool value = false;
		s_glClientStateFlags.TryGetValue(13, out value);
		if (value)
		{
			DrawTriangleListColored(first, count, indices);
			return;
		}
		BasicEffect effect = getEffect(useTexture: true, useColor: false);
		if (effect.Alpha == 0f)
		{
			s_LastVertices_PositionNormalTexture = null;
			return;
		}
		VertexPositionNormalTexture[] array = (s_LastVertices_PositionNormalTexture = ConstructTexturedVertices());
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Apply();
			Global.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, array, 0, array.Length, indices, 0, indices.Length / 3);
		}
	}

	private static void DrawTriangleListColored(int first, int count, short[] indices)
	{
		if (count == 0)
		{
			return;
		}
		BasicEffect effect = getEffect(useTexture: true, useColor: true);
		if (effect.Alpha == 0f)
		{
			return;
		}
		VertexPositionColorTexture[] array = ConstructTexturedColoredVertices(count / 3 * 2);
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Apply();
			Global.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, array, 0, count / 3 * 2, indices, 0, count / 3);
		}
	}

	public static void glDrawElements(int mode, int count, short[] indices)
	{
		if (mode == 7)
		{
			DrawTriangleList(0, count, indices);
		}
	}

	public static void glScissor(double x, double y, double width, double height)
	{
		glScissor((int)x, (int)y, (int)width, (int)height);
	}

	private static partial Microsoft.Xna.Framework.Rectangle GetScreenRectangle();

	public static void glScissor(int x, int y, int width, int height)
	{
		try
		{
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(x, y, width, height);
            Global.GraphicsDevice.ScissorRectangle = Microsoft.Xna.Framework.Rectangle.Intersect(rectangle, GetScreenRectangle());
		}
		catch (Exception)
		{
		}
	}

	public static void setScissorRectangle(double x, double y, double w, double h)
	{
		setScissorRectangle((float)x, (float)y, (float)w, (float)h);
	}

	public static void setScissorRectangle(float x, float y, float w, float h)
	{
		x = FrameworkTypes.transformToRealX(x);
		y = FrameworkTypes.transformToRealY(y);
		w = FrameworkTypes.transformToRealW(w);
		h = FrameworkTypes.transformToRealH(h);
		glScissor(x, y, w, h);
	}
}
