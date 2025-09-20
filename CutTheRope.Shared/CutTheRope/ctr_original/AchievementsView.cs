#if false
using System.IO;
using System.Threading;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.Specials;
using CutTheRope.utils;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.ctr_original;

internal class AchievementsView : View
{
	public struct AchievementHelper
	{
		public NSString title;

		public NSString description;

		public bool achieved;

		public Microsoft.Xna.Framework.Graphics.Texture2D texture;
	}

	private static AchievementHelper[] ACHIEVEMENTS;

	public static bool Init = false;

	private int CONTAINER_ADDITIONAL_WIDTH = 80;

	public static bool navigated = true;

	private ScrollableContainer container;

	private LiftScrollbar liftScrollbar;

	public virtual BaseElement createEntryForAchievementWidth(AchievementHelper achievement, float w, int scotchNum)
	{
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		NSString title = achievement.title;
		NSString description = achievement.description;
		Image image = null;
		if (achievement.achieved)
		{
			CutTheRope.iframework.visual.Texture2D t = new CutTheRope.iframework.visual.Texture2D().initWithTexture(achievement.texture);
			image = Image.Image_create(t);
			Image image2 = Image.Image_createWithResID(306 + scotchNum);
			image2.y = 10f;
			image2.parentAnchor = 10;
			image2.anchor = 18;
			image.addChild(image2);
		}
		if (image == null)
		{
			image = Image.Image_createWithResID(304);
		}
		image.blendingMode = 1;
		image.parentAnchor = 9;
		image.scaleX = (image.scaleY = 2f / 3f);
		image.passTransformationsToChilds = false;
		baseElement.addChild(image);
		Text text = Text.createWithFontandString(5, title);
		text.parentAnchor = 9;
		text.anchor = 17;
		baseElement.addChild(text);
		text.x = 70f;
		text.y = 17f;
		text.rotationCenterX = -text.width / 2;
		float num = 1f;
		while (text.x + (float)text.width * num > w)
		{
			num -= 0.05f;
		}
		text.scaleX = (text.scaleY = num);
		Text text2 = new Text().initWithFont(Application.getFont(6));
		text2.setStringandWidth(description, 190f);
		text2.parentAnchor = 9;
		text2.anchor = 9;
		baseElement.addChild(text2);
		text2.rotationCenterX = -text2.width / 2;
		text2.x = 70f;
		text2.y = 15f;
		num = 1f;
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
		{
			text2.y += 10f;
			while (text2.formattedStrings.Count > 2)
			{
				num -= 0.1f;
				text2.setStringandWidth(description, 190f / num);
				text2.scaleX = (text2.scaleY = num);
				text2.x -= 2f;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA && description.ToString().Length > 25)
			{
				text2.y -= 5f;
				text2.x -= 3f;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO && description.ToString().Length > 25)
			{
				text2.x -= 4f;
			}
		}
		else
		{
			while (text2.formattedStrings.Count > 3)
			{
				num -= 0.1f;
				text2.setStringandWidth(description, 190f / num);
				text2.scaleX = (text2.scaleY = num);
				text2.x -= 3f;
			}
		}
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR && title.ToString().Length == 8)
		{
			text.y += 3f;
			text2.y += 3f;
		}
		baseElement.height = 95;
		baseElement.width = (int)w;
		return baseElement;
	}

	public virtual void recreateContainerError(bool isLoading, object error)
	{
		if (container != null)
		{
			removeChild(container);
		}
		Vector vector = new Vector(180f, 380f);
		vector.x += CONTAINER_ADDITIONAL_WIDTH;
		VBox vBox = new VBox().initWithOffsetAlignWidth(0f, 2, vector.x);
		container = new ScrollableContainer().initWithWidthHeightContainer(vector.x, vector.y, vBox);
		container.canSkipScrollPoints = true;
		container.y += 34f;
		addChild(container);
		if (isLoading)
		{
			Factory.showProcessingOnViewwithTouchesBlocking(this, b: true);
			return;
		}
		Factory.hideProcessingFromView(this);
		if (error != null)
		{
			Text text = Text.createWithFontandString(6, NSObject.NSS(error.ToString()));
			text.setAlignment(2);
			vBox.addChild(text);
			vBox.parentAnchor = 18;
			vBox.anchor = 34;
			return;
		}
		if (Init)
		{
			container.turnScrollPointsOnWithCapacity(ACHIEVEMENTS.Length + 1);
			for (int i = 0; i < ACHIEVEMENTS.Length; i++)
			{
				AchievementHelper achievement = ACHIEVEMENTS[i];
				BaseElement baseElement = createEntryForAchievementWidth(achievement, vector.x, i % 4);
				vBox.addChild(baseElement);
				container.addScrollPointAtXY(baseElement.x, baseElement.y);
			}
			if (container.getMaxScroll().y > 0f)
			{
				container.addScrollPointAtXY(0f, container.getMaxScroll().y);
			}
			liftScrollbar.container = container;
			return;
		}
		int num = 19;
		if (num > 0)
		{
			container.turnScrollPointsOnWithCapacity(num + 1);
			if (container.getMaxScroll().y > 0f)
			{
				container.addScrollPointAtXY(0f, container.getMaxScroll().y);
			}
			liftScrollbar.container = container;
		}
	}

	public static void InitAllAchievements(AchievementCollection achievements)
	{
		ACHIEVEMENTS = new AchievementHelper[achievements.Count];
		for (int i = 0; i < achievements.Count; i++)
		{
			Achievement achievement = achievements[i];
			ACHIEVEMENTS[i] = default(AchievementHelper);
			ACHIEVEMENTS[i].achieved = achievement.IsEarned;
			ACHIEVEMENTS[i].description = NSObject.NSS(achievement.Description);
			ACHIEVEMENTS[i].title = NSObject.NSS(achievement.Name);
			using (Stream stream = achievement.GetPicture())
			{
				while (!navigated)
				{
					Thread.Sleep(100);
				}
				ACHIEVEMENTS[i].texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Global.GraphicsDevice, stream);
			}
			if (!Preferences._getBooleanForKey(achievement.Key) && achievement.IsEarned)
			{
				Preferences._setBooleanforKey(v: true, achievement.Key, comit: true);
			}
		}
		Init = true;
	}

	public override NSObject initFullscreen()
	{
		if (base.initFullscreen() != null)
		{
			Image image = Image.Image_createWithResIDQuad(67, 0);
			image.scaleY = FrameworkTypes.SCREEN_BG_SCALE_Y;
			image.scaleX = FrameworkTypes.SCREEN_BG_SCALE_X;
			image.parentAnchor = (image.anchor = 18);
			addChild(image);
			liftScrollbar = LiftScrollbar.createWithResIDBackQuadLiftQuadLiftQuadPressed(301, 1, 2, 3);
			liftScrollbar.x = 200f;
			liftScrollbar.y = 60f;
			liftScrollbar.x += CONTAINER_ADDITIONAL_WIDTH;
			liftScrollbar.blendingMode = 1;
			addChild(liftScrollbar);
			Image image2 = Image.Image_createWithResIDQuad(88, 0);
			image2.anchor = (image2.parentAnchor = 18);
			image2.scaleX = (image2.scaleY = 2f);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
			timeline.addKeyFrame(KeyFrame.makeRotation(45.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeRotation(405.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 75.0));
			timeline.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
			image2.addTimeline(timeline);
			image2.playTimeline(0);
			addChild(image2);
		}
		return this;
	}

	public override void show()
	{
		recreateContainerError(isLoading: false, null);
		base.show();
	}

	public override void dealloc()
	{
		base.dealloc();
	}

	public void resetScroll()
	{
		if (container != null && liftScrollbar != null)
		{
			container.show();
			liftScrollbar.update(0f);
		}
	}
}
#endif