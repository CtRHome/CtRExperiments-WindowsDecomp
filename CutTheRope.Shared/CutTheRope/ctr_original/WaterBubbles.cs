using System.Linq;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;

namespace CutTheRope.ctr_original;

internal class WaterBubbles : MultiParticles
{
	private float startSize;

	public override Particles initWithTotalParticlesandImageGrid(int p, Image grid)
	{
		if (base.initWithTotalParticlesandImageGrid(p, grid) != null)
		{
			duration = -1f;
			gravity.x = 0f;
			gravity.y = 0f;
			speed = 50f;
			speedVar = 0f;
			angle = -90f;
			posVar.x = width / 2;
			posVar.y = 0f;
			life = 5f;
			lifeVar = 1f;
			size = 0.7f;
			sizeVar = 0.3f;
			startSize = size + sizeVar;
			endSize = 0.7f;
			emissionRate = 2f;
			startColor.r = 1f;
			startColor.g = 1f;
			startColor.b = 1f;
			startColor.a = 0.6f;
			startColorVar.r = 0f;
			startColorVar.g = 0f;
			startColorVar.b = 0f;
			startColorVar.a = 0f;
			endColor.r = 1f;
			endColor.g = 1f;
			endColor.b = 1f;
			endColor.a = 0f;
			endColorVar.r = 0f;
			endColorVar.g = 0f;
			endColorVar.b = 0f;
			endColorVar.a = 0f;
			blendAdditive = true;
		}
		return this;
	}

	public override void initParticle(ref Particle particle)
	{
		base.initParticle(ref particle);
		int[] array = new int[2] { 5, 4 };
		int m = array.Count() - 1;
		int num = array[MathHelper.RND_RANGE(0, m)];
		Quad2D qt = imageGrid.texture.quads[num];
		Quad3D qv = Quad3D.MakeQuad3D(0f, 0f, 0f, 0f, 0f);
		drawer.setTextureQuadatVertexQuadatIndex(qt, qv, particleCount);
		Rectangle rectangle = imageGrid.texture.quadRects[num];
		particle.width = rectangle.w * particle.size;
		particle.height = rectangle.h * particle.size;
		particle.deltaSize = endSize;
	}

	public override void update(float delta)
	{
		base.update(delta);
		for (particleIdx = 0; particleIdx < particleCount; particleIdx++)
		{
			Particle particle = particles[particleIdx];
			if (particle.life > 0f && Mover.moveVariableToTarget(ref particle.size, particle.deltaSize, 0.5f, delta))
			{
				particle.deltaSize = ((particle.size == endSize) ? startSize : endSize);
			}
			float num = 1f - (1f - particle.size) / sizeVar;
			float num2 = particle.width * particle.size;
			float num3 = particle.height - particle.height * sizeVar * num;
			float num4 = particle.pos.x - num2 / 2f;
			float num5 = particle.pos.y - num3 / 2f;
			float num6 = particle.pos.x + num2 / 2f;
			float num7 = particle.pos.y - num3 / 2f;
			float num8 = particle.pos.x - num2 / 2f;
			float num9 = particle.pos.y + num3 / 2f;
			float num10 = particle.pos.x + num2 / 2f;
			float num11 = particle.pos.y + num3 / 2f;
			float cx = particle.pos.x;
			float cy = particle.pos.y;
			Vector v = MathHelper.vect(num4, num5);
			Vector v2 = MathHelper.vect(num6, num7);
			Vector v3 = MathHelper.vect(num8, num9);
			Vector v4 = MathHelper.vect(num10, num11);
			particle.angle += particle.deltaAngle * delta;
			float cosA = MathHelper.cosf(particle.angle);
			float sinA = MathHelper.sinf(particle.angle);
			v = Particles.rotatePreCalc(v, cosA, sinA, cx, cy);
			v2 = Particles.rotatePreCalc(v2, cosA, sinA, cx, cy);
			v3 = Particles.rotatePreCalc(v3, cosA, sinA, cx, cy);
			v4 = Particles.rotatePreCalc(v4, cosA, sinA, cx, cy);
			particles[particleIdx] = particle;
			ref Quad3D reference = ref drawer.vertices[particleIdx];
			reference = Particles.MakeQuad3DEx(v.x, v.y, v2.x, v2.y, v3.x, v3.y, v4.x, v4.y);
		}
	}
}
