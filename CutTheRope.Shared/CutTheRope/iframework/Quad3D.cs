namespace CutTheRope.iframework;

internal struct Quad3D
{
	public float blX;

	public float blY;

	public float blZ;

	public float brX;

	public float brY;

	public float brZ;

	public float tlX;

	public float tlY;

	public float tlZ;

	public float trX;

	public float trY;

	public float trZ;

	private float[] _array;

	public static Quad3D MakeQuad3D(double x, double y, double z, double w, double h)
	{
		return MakeQuad3D((float)x, (float)y, (float)z, (float)w, (float)h);
	}

	public static Quad3D MakeQuad3D(float x, float y, float z, float w, float h)
	{
		Quad3D result = default(Quad3D);
		result.blX = x;
		result.blY = y;
		result.blZ = z;
		result.brX = x + w;
		result.brY = y;
		result.brZ = z;
		result.tlX = x;
		result.tlY = y + h;
		result.tlZ = z;
		result.trX = x + w;
		result.trY = y + h;
		result.trZ = z;
		return result;
	}

	public float[] toFloatArray()
	{
		if (_array == null)
		{
			_array = new float[12]
			{
				blX, blY, blZ, brX, brY, brZ, tlX, tlY, tlZ, trX,
				trY, trZ
			};
		}
		return _array;
	}
}
