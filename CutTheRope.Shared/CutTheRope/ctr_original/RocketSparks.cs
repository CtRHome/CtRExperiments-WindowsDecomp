using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.ctr_original;

internal class RocketSparks : RotatableScalableMultiParticles
{
	public virtual NSObject initWithTotalParticlesAngleandImageGrid(int p, float a, Image grid)
	{
		if (base.initWithTotalParticlesandImageGrid(p, grid) == null)
		{
			return null;
		}
		duration = -1f;
		gravity.x = 0f;
		gravity.y = 0f;
		angle = a;
		angleVar = 10f;
		speed = 50f;
		speedVar = 10f;
		radialAccel = 0f;
		radialAccelVar = 0f;
		tangentialAccel = 0f;
		tangentialAccelVar = 0f;
		posVar.x = 5f;
		posVar.y = 5f;
		life = 0.5f;
		lifeVar = 0.1f;
		size = 0.5f;
		sizeVar = 0f;
		endSize = size;
		emissionRate = 20f;
		startColor.r = 1f;
		startColor.g = 1f;
		startColor.b = 1f;
		startColor.a = 1f;
		startColorVar.r = 0f;
		startColorVar.g = 0f;
		startColorVar.b = 0f;
		startColorVar.a = 0f;
		endColor.r = 0f;
		endColor.g = 0f;
		endColor.b = 0f;
		endColor.a = 0f;
		endColorVar.r = 0f;
		endColorVar.g = 0f;
		endColorVar.b = 0f;
		endColorVar.a = 0f;
		blendAdditive = true;
		return this;
	}

	public override void initParticle(ref Particle particle)
	{
		base.initParticle(ref particle);
		int num = MathHelper.RND_RANGE(6, 9);
		Quad2D qt = imageGrid.texture.quads[num];
		Quad3D qv = Quad3D.MakeQuad3D(0f, 0f, 0f, 0f, 0f);
		drawer.setTextureQuadatVertexQuadatIndex(qt, qv, particleCount);
		Vector quadSize = Image.getQuadSize(154, num);
		particle.width = quadSize.x;
		particle.height = quadSize.y;
	}
}
