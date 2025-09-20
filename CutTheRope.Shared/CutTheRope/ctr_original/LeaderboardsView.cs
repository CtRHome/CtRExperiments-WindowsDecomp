#if false
using System;
using System.Globalization;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.Specials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace CutTheRope.ctr_original;

internal class LeaderboardsView : View, ButtonDelegate
{
	private const int Y_OFFSET = 50;

	private const int X_OFFSET = 50;

	private const int ENTRY_HEIGHT = 36;

	private const int LEADERBOARDS_BUTTON_NEXT = 0;

	private const int LEADERBOARDS_BUTTON_PREVIOUS = 1;

	private const int LEADERBOARDS_BUTTON_EDIT = 2;

	private const int LEADERBOARDS_BUTTON_PAGE_UP = 3;

	private const int LEADERBOARDS_BUTTON_PAGE_DOWN = 4;

	private const int containerID = 1;

	private ScoreStruct[] CurrentLeaderboard;

	private bool canpageup;

	private bool canpagedown;

	private int NeedsRecreate;

	private LeaderboardReader leaderboardReader;

	private int mode;

	private LiftScrollbar liftScrollbar;

	private ScrollableContainer container;

	private BaseElement playerPlace;

	private Image playerPlaceContainer;

	private Button nextb;

	private Button prevb;

	private Button pageupb;

	private Button pagedownb;

	private Text boxTitle;

	public override void draw()
	{
		if (NeedsRecreate == 1)
		{
			recreateContainerError(isLoading: false, null);
			NeedsRecreate = 0;
		}
		if (NeedsRecreate == 2)
		{
			recreateContainerError(isLoading: false, Application.getString(1179732));
			NeedsRecreate = 0;
		}
		base.draw();
	}

	private ScoreStruct[] getLeaderboard(LeaderboardReader leaderboardReader)
	{
		ScoreStruct[] array = new ScoreStruct[leaderboardReader.Entries.Count];
		for (int i = 0; i < leaderboardReader.Entries.Count; i++)
		{
			LeaderboardEntry leaderboardEntry = leaderboardReader.Entries[i];
			array[i].name = NSObject.NSS(leaderboardEntry.Gamer.Gamertag);
			array[i].result = leaderboardEntry.Columns.GetValueInt32("BestScore");
			array[i].rank = i + leaderboardReader.PageStart + 1;
		}
		return array;
	}

	private void InitLeaderboard(int n)
	{
		SignedInGamer signedInGamer = Gamer.SignedInGamers[(PlayerIndex)0];
		if (signedInGamer == null)
		{
			recreateContainerError(isLoading: false, Application.getString(1179732));
			return;
		}
		LeaderboardIdentity leaderboardId = LeaderboardIdentity.Create(LeaderboardKey.BestScoreLifeTime, n);
		LeaderboardReader.BeginRead(leaderboardId, signedInGamer, 100, LeaderboardReadCallback, signedInGamer);
	}

	protected void LeaderboardReadCallback(IAsyncResult result)
	{
		if (result.AsyncState is SignedInGamer)
		{
			try
			{
				leaderboardReader = LeaderboardReader.EndRead(result);
				canpageup = leaderboardReader.CanPageUp;
				canpagedown = leaderboardReader.CanPageDown;
				CurrentLeaderboard = getLeaderboard(leaderboardReader);
				NeedsRecreate = 1;
			}
			catch (Exception)
			{
				canpageup = false;
				canpagedown = false;
				NeedsRecreate = 2;
			}
		}
	}

	public static NSString numberToSeparatedString(int theScore)
	{
		return NSObject.NSS(theScore.ToString(CultureInfo.CurrentCulture));
	}

	public static Button createButtonWithImageQuad1Quad2IDDelegate(int res, int q1, int q2, int bid, NSObject d)
	{
		Image image = Image.Image_createWithResIDQuad(res, q1);
		Image image2 = Image.Image_createWithResIDQuad(res, q2);
		image2.scaleX = (image2.scaleY = 1.2f);
		image.parentAnchor = (image2.parentAnchor = 9);
		image.anchor = (image2.anchor = 9);
		Button button = new Button().initWithUpElementDownElementandID(image, image2, bid);
		button.delegateButtonDelegate = (ButtonDelegate)d;
		button.setTouchIncreaseLeftRightTopBottom(button.width / 2, button.width / 2, button.height / 2, button.height / 2);
		return button;
	}

	public override NSObject initFullscreen()
	{
		if (base.initFullscreen() != null)
		{
			x += 0.33f;
			container = null;
			Image image = Image.Image_createWithResIDQuad(67, 0);
			image.scaleX = (image.scaleY = 1.25f);
			image.parentAnchor = (image.anchor = 18);
			addChild(image);
			addChild((BaseElement)new BaseElement().init());
			nextb = MenuController.createButton2WithImageQuad1Quad2IDDelegate(305, 0, 1, 0, this);
			nextb.scaleX = -1f;
			Image.setElementPositionWithQuadCenter(nextb, 303, 7);
			Vector relativeQuadOffset = Image.getRelativeQuadOffset(303, 8, 7);
			nextb.x += relativeQuadOffset.x;
			nextb.y -= relativeQuadOffset.y;
			nextb.setTouchIncreaseLeftRightTopBottom(nextb.width / 2, nextb.width / 2, nextb.height / 2, nextb.height / 2);
			addChild(nextb);
			prevb = MenuController.createButton2WithImageQuad1Quad2IDDelegate(305, 0, 1, 1, this);
			prevb.setEnabled(e: false);
			Image.setElementPositionWithQuadCenter(prevb, 303, 8);
			prevb.setTouchIncreaseLeftRightTopBottom(prevb.width / 2, prevb.width / 2, prevb.height / 2, prevb.height / 2);
			addChild(prevb);
			pageupb = MenuController.createButton2WithImageQuad1Quad2IDDelegate(305, 0, 1, 3, this);
			pageupb.setEnabled(e: false);
			pageupb.rotation = 90f;
			pageupb.x = 200f;
			pageupb.y = 450f;
			pageupb.setTouchIncreaseLeftRightTopBottom(pageupb.width / 2, pageupb.width / 2, pageupb.height / 2, pageupb.height / 2);
			addChild(pageupb);
			pagedownb = MenuController.createButton2WithImageQuad1Quad2IDDelegate(305, 0, 1, 4, this);
			pagedownb.setEnabled(e: false);
			pagedownb.rotation = 90f;
			pagedownb.scaleX = -1f;
			pagedownb.x = 270f;
			pagedownb.y = 450f;
			pagedownb.setTouchIncreaseLeftRightTopBottom(pageupb.width / 2, pageupb.width / 2, pageupb.height / 2, pageupb.height / 2);
			addChild(pagedownb);
			boxTitle = Text.createWithFontandString(5, Application.getString(1179713 + mode));
			boxTitle.setAlignment(2);
			Image.setElementPositionWithQuadCenter(boxTitle, 303, 7);
			addChild(boxTitle);
			Image image2 = Image.Image_createWithResIDQuad(303, 12);
			image2.scaleX = 0.6f;
			image2.x = 160f;
			image2.y = 50f;
			image2.anchor = 18;
			image2.blendingMode = 1;
			addChild(image2);
			Text text = Text.createWithFontandString(5, Application.getString(1179727));
			Image.setElementPositionWithQuadOffset(text, 303, 9);
			text.anchor = 17;
			text.setAlignment(1);
			addChild(text);
			Text text2 = Text.createWithFontandString(5, Application.getString(1179728));
			Image.setElementPositionWithQuadOffset(text2, 303, 10);
			text2.anchor = 20;
			text2.setAlignment(4);
			addChild(text2);
			RGBAColor rGBAColor = new RGBAColor(1.0, 203.0 / 255.0, 127.0 / 255.0, 1.0);
			text.color = (text2.color = rGBAColor);
			playerPlaceContainer = Image.Image_createWithResIDQuad(303, 1);
			playerPlaceContainer.doRestoreCutTransparency();
			addChild(playerPlaceContainer);
			playerPlaceContainer.y += 50f;
			playerPlaceContainer.setEnabled(e: false);
			liftScrollbar = LiftScrollbar.createWithResIDBackQuadLiftQuadLiftQuadPressed(301, 0, 2, 3);
			Image.setElementPositionWithQuadOffset(liftScrollbar, 303, 2);
			liftScrollbar.blendingMode = 1;
			addChild(liftScrollbar);
			liftScrollbar.setEnabled(e: false);
			Image image3 = Image.Image_createWithResIDQuad(106, 0);
			image3.anchor = (image3.parentAnchor = 18);
			image3.scaleX = (image3.scaleY = 2f);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
			timeline.addKeyFrame(KeyFrame.makeRotation(45.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeRotation(405.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 75.0));
			timeline.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
			image3.addTimeline(timeline);
			image3.playTimeline(0);
			addChildwithID(image3, childsCount() + 1);
		}
		return this;
	}

	public virtual BaseElement createEntryForRankScoreNameWidth(int rankNumber, int score, NSString userName, float w)
	{
		Vector relativeQuadOffset = Image.getRelativeQuadOffset(303, 13, 4);
		Vector relativeQuadOffset2 = Image.getRelativeQuadOffset(303, 13, 6);
		Vector relativeQuadOffset3 = Image.getRelativeQuadOffset(303, 13, 11);
		BaseElement baseElement = (BaseElement)new BaseElement().init();
		float num = -12f;
		Text text = Text.createWithFontandStringEN(6, NSObject.NSS(rankNumber + ".  "));
		text.scaleX = (text.scaleY = 0.75f);
		text.rotationCenterY = text.height / 2;
		text.anchor = 12;
		text.parentAnchor = 9;
		text.x = relativeQuadOffset.x;
		text.y = num - 4f;
		baseElement.addChild(text);
		Text text2 = Text.createWithFontandStringEN(6, userName);
		text2.anchor = 9;
		text2.parentAnchor = 9;
		text2.x = relativeQuadOffset2.x + 3f;
		text2.y = num + 3f;
		text2.scaleX = (text2.scaleY = 0.7f);
		text2.rotationCenterX -= text2.width / 2;
		baseElement.addChild(text2);
		Text text3 = Text.createWithFontandStringEN(6, numberToSeparatedString(score));
		text3.anchor = 12;
		text3.parentAnchor = 9;
		text3.x = relativeQuadOffset3.x;
		text3.y = num + 3f;
		text3.scaleX = (text3.scaleY = 0.7f);
		text3.rotationCenterX += text3.width / 2;
		baseElement.addChild(text3);
		Font font = (Font)Application.getFontEN(6);
		float num2 = font.stringWidth(NSObject.NSS("."));
		float num3 = text3.x - text2.x - (float)text2.width * 0.7f - (float)text3.width * 0.7f;
		string text4 = "";
		float num4 = (num3 - num2 - 2f) / 0.7f;
		if (num4 > 0f)
		{
			while (font.stringWidth(NSObject.NSS(text4)) < num4)
			{
				text4 += ".";
			}
		}
		Text text5 = Text.createWithFontandStringEN(6, NSObject.NSS(text4));
		text5.anchor = 9;
		text5.parentAnchor = 12;
		text5.x += 2f;
		text2.addChild(text5);
		baseElement.width = (int)w;
		baseElement.height = 36;
		return baseElement;
	}

	public virtual BaseElement createEntryForScoreWidth(ScoreStruct score, float w)
	{
		if (score.name.ToString() == Gamer.SignedInGamers[(PlayerIndex)0].Gamertag)
		{
			BaseElement baseElement = (BaseElement)new BaseElement().init();
			Image image = Image.Image_createWithResIDQuad(303, 1);
			BaseElement baseElement2 = createEntryForRankScoreNameWidth(score.rank, score.result, score.name, w);
			baseElement2.parentAnchor = (baseElement2.anchor = 18);
			baseElement2.x = 50f;
			image.parentAnchor = (image.anchor = 18);
			image.scaleX = 0.48f;
			image.scaleY = 0.6f;
			image.x = 13f;
			image.y = 0.16f;
			baseElement.addChild(image);
			baseElement.addChild(baseElement2);
			baseElement.width = baseElement2.width;
			baseElement.height = baseElement2.height;
			baseElement.parentAnchor = (baseElement.anchor = 18);
			return baseElement;
		}
		BaseElement baseElement3 = createEntryForRankScoreNameWidth(score.rank, score.result, score.name, w);
		baseElement3.x = 50f;
		return baseElement3;
	}

	public virtual void cleanContainer()
	{
		Factory.hideProcessingFromView(this);
	}

	public virtual void recreateContainerError(bool isLoading, object error)
	{
		if (container != null)
		{
			removeChild(container);
		}
		Vector relativeQuadOffset = Image.getRelativeQuadOffset(303, 13, 14);
		relativeQuadOffset.x += 50f;
		relativeQuadOffset.y += 50f;
		VBox vBox = null;
		if (error != null)
		{
			vBox = new VBox().initWithOffsetAlignWidth(0f, 2, FrameworkTypes.SCREEN_WIDTH);
			container = new ScrollableContainer().initWithWidthHeightContainer(FrameworkTypes.SCREEN_WIDTH, relativeQuadOffset.y, vBox);
			Image.setElementPositionWithQuadOffset(container, 303, 13);
			container.x = 0f;
		}
		else
		{
			vBox = new VBox().initWithOffsetAlignWidth(0f, 2, relativeQuadOffset.x);
			container = new ScrollableContainer().initWithWidthHeightContainer(relativeQuadOffset.x, relativeQuadOffset.y, vBox);
			Image.setElementPositionWithQuadOffset(container, 303, 13);
			container.x -= 50f;
		}
		container.canSkipScrollPoints = true;
		addChildwithID(container, 1);
		if (playerPlace != null)
		{
			removeChild(playerPlace);
			playerPlace = null;
		}
		Factory.hideProcessingFromView(this);
		if (isLoading)
		{
			Factory.showProcessingOnViewwithTouchesBlocking(this, b: false);
			playerPlaceContainer.setEnabled(e: false);
			liftScrollbar.setEnabled(e: false);
			return;
		}
		if (error != null)
		{
			Text text = new Text().initWithFont(Application.getFont(6));
			text.setAlignment(2);
			text.setStringandWidth(NSObject.NSS(error.ToString()), 250f);
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				text.scaleX = 0.9f;
				text.scaleY = 0.9f;
			}
			vBox.addChild(text);
			vBox.parentAnchor = 18;
			vBox.anchor = 18;
			return;
		}
		int num = CurrentLeaderboard.Length;
		if (num > 0)
		{
			container.turnScrollPointsOnWithCapacity(num + 1);
			for (int i = 0; i < num; i++)
			{
				ScoreStruct score = CurrentLeaderboard[i];
				BaseElement baseElement = createEntryForScoreWidth(score, relativeQuadOffset.x);
				vBox.addChild(baseElement);
				container.addScrollPointAtXY(0f, baseElement.y);
			}
			if (container.getMaxScroll().y > 0f)
			{
				container.addScrollPointAtXY(0f, container.getMaxScroll().y);
			}
			int num2 = (num - 1) * 36;
			if (vBox.height < num2)
			{
				vBox.height = num2;
			}
			liftScrollbar.container = container;
			liftScrollbar.setEnabled(num > 10);
		}
		playerPlaceContainer.setEnabled(e: false);
	}

	public virtual void updateScores()
	{
		prevb.setEnabled(mode != 0);
		nextb.setEnabled(mode != CTRPreferences.getPacksCount() - 1);
		if (mode >= 0 && mode < CTRPreferences.getPacksCount())
		{
			boxTitle.setString(Application.getString(1179713 + mode));
			recreateContainerError(isLoading: true, null);
			InitLeaderboard(mode);
		}
	}

	public override void show()
	{
		updateScores();
		base.show();
	}

	public override void dealloc()
	{
		base.dealloc();
	}

	public void onButtonPressed(int n)
	{
		switch (n)
		{
		case 0:
			cleanContainer();
			mode++;
			updateScores();
			break;
		case 1:
			cleanContainer();
			mode--;
			updateScores();
			break;
		case 3:
			if (canpageup)
			{
				recreateContainerError(isLoading: true, null);
				leaderboardReader.BeginPageUp(LeaderboardPageUpCallback, Gamer.SignedInGamers[(PlayerIndex)0]);
			}
			break;
		case 4:
			if (canpageup)
			{
				recreateContainerError(isLoading: true, null);
				leaderboardReader.BeginPageUp(LeaderboardPageUpCallback, Gamer.SignedInGamers[(PlayerIndex)0]);
			}
			break;
		case 2:
			break;
		}
	}

	protected void LeaderboardPageUpCallback(IAsyncResult result)
	{
		if (result.AsyncState is SignedInGamer)
		{
			try
			{
				leaderboardReader.EndPageUp(result);
				canpageup = leaderboardReader.CanPageUp;
				canpagedown = leaderboardReader.CanPageDown;
				CurrentLeaderboard = getLeaderboard(leaderboardReader);
				NeedsRecreate = 1;
			}
			catch (Exception)
			{
				canpageup = false;
				canpagedown = false;
			}
		}
	}

	protected void LeaderboardPageDownCallback(IAsyncResult result)
	{
		if (result.AsyncState is SignedInGamer)
		{
			try
			{
				leaderboardReader.EndPageDown(result);
				canpageup = leaderboardReader.CanPageUp;
				canpagedown = leaderboardReader.CanPageDown;
				CurrentLeaderboard = getLeaderboard(leaderboardReader);
				NeedsRecreate = 1;
			}
			catch (Exception)
			{
				canpageup = false;
				canpagedown = false;
			}
		}
	}
}
#endif