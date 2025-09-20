using System;
using System.Collections.Generic;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.iframework.visual; // added

internal class Rollbar : BaseElement
{
	private const int speedAccelerator = 2;

	private const int blankSpaceTop = 2;

	private const int blankSpaceBottom = 1;

	private const int minAge = 1;

	private const int maxAge = 99;

	private const int defaultIdx = 24;

	private const float friction = 5f;

	private const float minFriction = 0.7f;

	private const float cellBounceSpeed = 3f;

	private const float boundReturnSpeed = 20f;

	private double offsetY;

	private double oldOffsetY;

	private double speedY;

	private double lastTouchY;

	private double preLastTouchY;

	private double lastTimeDelta;

	private double lastMoveSpeed;

	private bool manualMode;

	private Vector scissorTL;

	private Vector scissorWH;

	private int halfVisibleCount;

	private Vector centralCellTL;

	private float centralCellHeight;

	private BaseElement scrollTop;

	private List<BaseElement> elements;

	public int getIndex()
	{
		return (int)(0.0 - Math.Round(offsetY / (double)centralCellHeight) - 2.0);
	}

	public Rollbar Create()
	{
		init();
		elements = new List<BaseElement>();
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		baseElement.anchor = 9;
		baseElement.parentAnchor = 9;
		Image image = Image.Image_createWithResIDQuad(310, 0);
		image.anchor = (image.parentAnchor = 9);
		Image image2 = Image.Image_createWithResIDQuad(310, 0);
		image2.anchor = (image2.parentAnchor = 12);
		image2.scaleX = -1f;
		width = (baseElement.width = image.width * 2);
		height = (baseElement.height = image.height);
		baseElement.addChild(image);
		baseElement.addChild(image2);
		addChild(baseElement);
		scrollTop = Image.Image_createWithResIDQuad(310, 4);
		scrollTop.anchor = (scrollTop.parentAnchor = 9);
		Image.setElementPositionWithRelativeQuadOffset(scrollTop, 310, 0, 4);
		addChild(scrollTop);
		scrollTop.visible = false;
		centralCellTL = Image.getRelativeQuadOffset(310, 0, 3);
		Text text = Text.createWithFontandString(5, NSObject.NSS(" "));
		text.visible = false;
		text.anchor = (text.parentAnchor = 18);
		elements.Add(text);
		addChild(text);
		Image image3 = Image.Image_createWithResIDQuad(310, 2);
		image3.scaleY = -1f;
		image3.visible = false;
		image3.anchor = (image3.parentAnchor = 18);
		elements.Add(image3);
		addChild(image3);
		Timeline timeline = new Timeline();
		timeline.initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.8, 0.8, 0.8, 0.8), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.3, 0.3, 0.3, 0.3), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.3));
		timeline.timelineLoopType = Timeline.LoopType.TIMELINE_PING_PONG;
		image3.addTimeline(timeline);
		image3.playTimeline(0);
		for (int num = 1; num <= 99; num++)
		{
			text = Text.createWithFontandString(5, new NSString(num.ToString()));
			text.visible = false;
			text.anchor = (text.parentAnchor = 18);
			elements.Add(text);
			addChild(text);
		}
		image3 = Image.Image_createWithResIDQuad(310, 2);
		image3.visible = false;
		image3.anchor = (image3.parentAnchor = 18);
		elements.Add(image3);
		addChild(image3);
		timeline = new Timeline();
		timeline.initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.8, 0.8, 0.8, 0.8), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.MakeRGBA(0.3, 0.3, 0.3, 0.3), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.3));
		timeline.timelineLoopType = Timeline.LoopType.TIMELINE_PING_PONG;
		image3.addTimeline(timeline);
		image3.playTimeline(0);
		centralCellHeight = Image.getQuadSize(310, 3).y;
		double num2 = (double)scrollTop.height / 2.0 / (double)centralCellHeight;
		halfVisibleCount = (int)Math.Ceiling(num2);
		offsetY = -24f * centralCellHeight;
		lastTouchY = 0f - FrameworkTypes.SCREEN_HEIGHT_EXPANDED;
		BaseElement baseElement2 = (BaseElement)new BaseElement().init();
		Image.setElementPositionWithQuadCenter(baseElement2, 310, 1);
		scissorTL = MathHelper.vect(baseElement2.x - 20f, baseElement2.y - 80f);
		scissorWH = Image.getQuadSize(310, 1);
		return this;
	}

	public override bool onTouchDownXY(float x, float y)
	{
		if (x < base.x || x > base.x + (float)width || y < base.y || y > base.y + (float)height)
		{
			return false;
		}
		lastTouchY = y;
		lastMoveSpeed = 0.0;
		speedY = 0.0;
		manualMode = true;
		return true;
	}

	public override bool onTouchMoveXY(float x, float y)
	{
		if (lastTouchY > (double)(0f - FrameworkTypes.SCREEN_HEIGHT_EXPANDED))
		{
			preLastTouchY = lastTouchY;
			float num = (float)((double)y - lastTouchY);
			lastTouchY = y;
			oldOffsetY = offsetY;
			offsetY += num;
			lastMoveSpeed = (double)num / lastTimeDelta;
			speedY = 0.0;
			return true;
		}
		return false;
	}

	public override bool onTouchUpXY(float x, float y)
	{
		manualMode = false;
		if (lastTouchY <= (double)(0f - FrameworkTypes.SCREEN_HEIGHT_EXPANDED))
		{
			return false;
		}
		if (preLastTouchY == lastTouchY)
		{
			lastMoveSpeed = 0.0;
		}
		speedY = lastMoveSpeed * 2.0;
		lastTouchY = 0f - FrameworkTypes.SCREEN_HEIGHT_EXPANDED;
		return true;
	}

	public void scrollWithSpeed(float speed)
	{
		speedY = speed;
	}

	private float getCurrentScrollSpeed()
	{
		return (float)speedY;
	}

	public float getOffsetY()
	{
		float num = (float)(offsetY - Math.Floor(offsetY / (double)centralCellHeight) * (double)centralCellHeight);
		if (num > centralCellHeight / 2f)
		{
			num -= centralCellHeight;
		}
		return num;
	}

	public override void draw()
	{
		base.draw();
		OpenGL.glEnable(4);
		OpenGL.setScissorRectangle(scissorTL.x, scissorTL.y, scissorWH.x, scissorWH.y);
		for (int num = -halfVisibleCount - 1; num < halfVisibleCount + 1; num++)
		{
			int num2 = (int)(offsetY / (double)centralCellHeight);
			int num3 = -num2 + num;
			if (num3 >= 0 && num3 < elements.Count)
			{
				BaseElement baseElement = elements[num3];
				baseElement.y = (float)((double)((float)num * centralCellHeight) + (offsetY - (double)((float)num2 * centralCellHeight)));
				baseElement.draw();
			}
		}
		OpenGL.glDisable(4);
		scrollTop.draw();
	}

	public override void update(float delta)
	{
		base.update(delta);
		lastTimeDelta = delta;
		oldOffsetY = offsetY;
		offsetY += speedY * (double)delta;
		float num = (float)(offsetY - Math.Floor(offsetY / (double)centralCellHeight) * (double)centralCellHeight);
		if (num > centralCellHeight / 2f)
		{
			num -= centralCellHeight;
		}
		if (!manualMode)
		{
			speedY -= num / 3f;
		}
		speedY *= MathHelper.MAX(0.7f, 1f - delta * 5f);
		float num2 = (float)(offsetY + (double)((float)halfVisibleCount * centralCellHeight));
		if (num2 > 0f && !manualMode)
		{
			offsetY -= num2 * 20f * delta;
		}
		num2 = (float)((double)((float)(-(elements.Count - halfVisibleCount + 1)) * centralCellHeight) - offsetY);
		if (num2 > 0f && !manualMode)
		{
			offsetY += num2 * 20f * delta;
		}
	}
}
