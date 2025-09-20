using System;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game;

internal class WaterElement : Image, TimelineDelegate
{
	private Vector topShadowSize;

	private Vector bottomShadowSize;

	private Vector topTileSize;

	private Vector backTileSize;

	private float xOffsetTop;

	private float xOffsetBack;

	private WaterBubbles bubbles;

	private AnimationsPool aniPool;

	private AnimationsPool aniPool2;

	private ScissorElement scissorElement;

	private DelayedDispatcher dd;

	private bool bReleasing;

	private static NSObject createWithWidthHeight(float w, float h)
	{
		return (WaterElement)new WaterElement().initWithWidthHeight(w, h);
	}

	private static Image createLightWithXPosquadalphaColordelegate(float x, int quad, RGBAColor color, TimelineDelegate d)
	{
		Image image = Image.Image_createWithResIDQuad(260, quad);
		image.parentAnchor = 9;
		image.anchor = 9;
		image.x = x;
		image.color = RGBAColor.transparentRGBA;
		image.doRestoreCutTransparency();
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(5);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0f));
		timeline.addKeyFrame(KeyFrame.makeColor(color, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.7));
		timeline.addKeyFrame(KeyFrame.makeColor(color, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.6));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.7));
		timeline.delegateTimelineDelegate = d;
		image.addTimeline(timeline);
		Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline2.addKeyFrame(KeyFrame.makeSingleAction(image, "ACTION_PLAY_TIMELINE", 0, 0, (float)MathHelper.RND_RANGE(0, 20) / 10f));
		image.addTimeline(timeline2);
		image.playTimeline(1);
		return image;
	}

	public virtual NSObject initWithWidthHeight(float w, float h)
	{
		if (base.initWithTexture(Application.getTexture(260)) != null)
		{
			width = (int)w;
			height = (int)h;
			topShadowSize = Image.getQuadSize(260, 1);
			bottomShadowSize = Image.getQuadSize(260, 0);
			topTileSize = Image.getQuadSize(260, 3);
			backTileSize = Image.getQuadSize(260, 2);
			xOffsetBack = backTileSize.x;
			int num = 10;
			for (int i = 0; i <= num; i++)
			{
				RGBAColor rGBAColor = ((i == 0 || i == num) ? RGBAColor.MakeRGBA(1.0, 1.0, 1.0, 0.5) : RGBAColor.solidOpaqueRGBA);
				Image c = createLightWithXPosquadalphaColordelegate(FrameworkTypes.SCREEN_WIDTH / (float)num * (float)(i - 1), 7, rGBAColor, this);
				addChild(c);
			}
			for (int j = 0; j < 1; j++)
			{
				Image image = createLightWithXPosquadalphaColordelegate(MathHelper.RND_RANGE((int)FrameworkTypes.SCREEN_WIDTH / 4, (int)FrameworkTypes.SCREEN_WIDTH * 3 / 4), 6, RGBAColor.MakeRGBA(1.0, 1.0, 1.0, 0.6), this);
				image.setName(NSObject.NSS("spot"));
				addChild(image);
			}
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, 0f));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
			addTimeline(timeline);
			playTimeline(0);
			aniPool = (AnimationsPool)new AnimationsPool().init();
			aniPool.parentAnchor = 9;
			addChild(aniPool);
			Image image2 = Image.Image_createWithResID(260);
			bubbles = (WaterBubbles)new WaterBubbles().initWithTotalParticlesandImageGrid(40, image2);
			bubbles.width = width;
			bubbles.height = height;
			bubbles.x = width / 2;
			bubbles.particlesDelegate = aniPool.particlesFinished;
			bubbles.startSystem(1);
			scissorElement = (ScissorElement)new ScissorElement().init();
			scissorElement.parentAnchor = 9;
			scissorElement.width = width;
			scissorElement.height = height;
			scissorElement.y = topTileSize.y;
			scissorElement.addChild(bubbles);
			aniPool.addChild(scissorElement);
		}
		return this;
	}

	public virtual void drawBack()
	{
		if (!bReleasing)
		{
			OpenGL.glColor4f(color.r, color.g, color.b, color.a);
			float num = ((drawY + (float)height > FrameworkTypes.SCREEN_HEIGHT) ? (drawY + (float)height) : FrameworkTypes.SCREEN_HEIGHT);
			GLDrawer.drawImageTiled(texture, 0, drawX, num - bottomShadowSize.y + FrameworkTypes.SCREEN_OFFSET_Y, width, topShadowSize.y);
			GLDrawer.drawImageTiled(texture, 2, drawX - MathHelper.ceil(xOffsetBack), drawY, (float)width + (float)Math.Floor(xOffsetBack), backTileSize.y);
			OpenGL.SetWhiteColor();
		}
	}

	public virtual void addParticlesAtXY(float tx, float ty)
	{
		if (!bReleasing)
		{
			float num = bubbles.x;
			float num2 = bubbles.y;
			bubbles.x = tx;
			bubbles.y = ty;
			bubbles.posVar.x = 10f;
			bubbles.posVar.y = 10f;
			for (int i = 0; i < 3; i++)
			{
				bubbles.addParticle();
			}
			bubbles.x = num;
			bubbles.y = num2;
			bubbles.posVar.x = width / 2;
			bubbles.posVar.y = 0f;
		}
	}

	public virtual void addWaterParticlesAtXY(float tx, float ty)
	{
		if (!bReleasing)
		{
			Image image = Image.Image_createWithResID(260);
			image.doRestoreCutTransparency();
			WaterDrops waterDrops = (WaterDrops)new WaterDrops().initWithTotalParticlesandImageGrid(10, image);
			waterDrops.color = RGBAColor.blackRGBA;
			waterDrops.x = tx;
			waterDrops.y = ty;
			waterDrops.particlesDelegate = aniPool.particlesFinished;
			aniPool.addChild(waterDrops);
			waterDrops.startSystem(10);
		}
	}

	public override void dealloc()
	{
		bReleasing = true;
		if (dd != null)
		{
			dd.cancelAllDispatches();
		}
		NSObject.NSREL(bubbles);
		base.dealloc();
	}

	public override void update(float delta)
	{
		if (!bReleasing)
		{
			base.update(delta);
			if (Mover.moveVariableToTarget(ref xOffsetBack, 0f, 100f, delta))
			{
				xOffsetBack = backTileSize.x;
			}
			if (Mover.moveVariableToTarget(ref xOffsetTop, topTileSize.x, 100f, delta))
			{
				xOffsetTop = 0f;
			}
			bubbles.y = (float)height + y;
			if (dd != null)
			{
				dd.update(delta);
			}
		}
	}

	public override void draw()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		GameController gameController = (GameController)cTRRootController.getChild(3);
		GameScene gameScene = (GameScene)gameController.getView(0).getChild(0);
		if (!bReleasing)
		{
			preDraw();
			GLDrawer.drawImageTiled(texture, 1, drawX, drawY, width, topShadowSize.y);
			OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE);
			scissorElement.y = topTileSize.y - gameScene.camera.pos.y;
			scissorElement.height = height;
			postDraw();
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			OpenGL.glColor4f(color.r, color.g, color.b, color.a);
			GLDrawer.drawImageTiled(texture, 3, drawX - MathHelper.ceil(xOffsetTop), drawY, (float)width + (float)Math.Floor(xOffsetTop), topTileSize.y);
			OpenGL.SetWhiteColor();
		}
	}

	public virtual void playFirstTimeline(BaseElement element)
	{
		element.playTimeline(0);
	}

	public virtual void timelineFinished(Timeline t)
	{
		if (!bReleasing)
		{
			if (dd == null)
			{
				dd = (DelayedDispatcher)new DelayedDispatcher().init();
			}
			dd.callObjectSelectorParamafterDelay(selector_playFirstTimeline, t.element, (float)MathHelper.RND_RANGE(0, 20) / 20f);
			if (t.element.name != null)
			{
				t.element.x = MathHelper.RND_RANGE((int)FrameworkTypes.SCREEN_WIDTH / 4, (int)FrameworkTypes.SCREEN_WIDTH * 3 / 4);
			}
		}
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
	}

	public virtual void prepareToRelease()
	{
		bReleasing = true;
		if (dd != null)
		{
			dd.cancelAllDispatches();
		}
	}

	private static void selector_playFirstTimeline(NSObject param)
	{
		if (param != null)
		{
			((BaseElement)param).playTimeline(0);
		}
	}
}
