using CutTheRope.iframework;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;

namespace CutTheRope.ctr_original;

internal class WaterDrops : CandyBreak
{
	public override Particles initWithTotalParticlesandImageGrid(int p, Image grid)
	{
		if (base.initWithTotalParticlesandImageGrid(p, grid) != null)
		{
			size = 1f;
			life = 1f;
			duration = 1f;
			speed = 100f;
			speedVar = 10f;
			gravity.y = 175f;
			endColor = RGBAColor.transparentRGBA;
			blendAdditive = true;
		}
		return this;
	}

	public override void initParticle(ref Particle particle)
	{
		base.initParticle(ref particle);
		int num = MathHelper.RND_RANGE(8, 10);
		Quad2D qt = imageGrid.texture.quads[num];
		Quad3D qv = Quad3D.MakeQuad3D(0f, 0f, 0f, 0f, 0f);
		drawer.setTextureQuadatVertexQuadatIndex(qt, qv, particleCount);
		Rectangle rectangle = imageGrid.texture.quadRects[num];
		particle.width = rectangle.w * size;
		particle.height = rectangle.h * size;
	}
}
