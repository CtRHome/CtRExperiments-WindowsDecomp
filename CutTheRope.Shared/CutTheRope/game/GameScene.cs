using System;
using System.Collections.Generic;
using System.Linq;
using CutTheRope.ctr_original;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.sfe;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.utils;

namespace CutTheRope.game;

internal class GameScene : BaseElement, TimelineDelegate, ButtonDelegate, DrawingDelegate, RocketDelegate
{
	private class FingerCut : NSObject
	{
		public Vector start;

		public Vector end;

		public float startSize;

		public float endSize;

		public RGBAColor c;
	}

	private class GameObjectSpecial : CTRGameObject
	{
		public int special;

		public static GameObjectSpecial GameObjectSpecial_create(Texture2D t)
		{
			return (GameObjectSpecial)new GameObjectSpecial().initWithTexture(t);
		}

		public static GameObjectSpecial GameObjectSpecial_createWithResID(int r)
		{
			return GameObjectSpecial_create(Application.getTexture(r));
		}

		public static GameObjectSpecial GameObjectSpecial_createWithResIDQuad(int r, int q)
		{
			GameObjectSpecial gameObjectSpecial = GameObjectSpecial_create(Application.getTexture(r));
			gameObjectSpecial.texture.PixelCorrectionX();
			gameObjectSpecial.setDrawQuad(q);
			return gameObjectSpecial;
		}
	}

	private class SCandy : ConstraintedPoint
	{
		public bool good;

		public float speed;

		public float angle;

		public float lastAngleChange;
	}

	private class TutorialText : Text
	{
		public int special;
	}

	public delegate void gameWonDelegate();

	public delegate void gameLostDelegate();

	public const int MAX_TOUCHES = 5;

	public const float DIM_TIMEOUT = 0.15f;

	public const int HUD_STARS_COUNT = 3;

	public const float SCOMBO_TIMEOUT = 0.2f;

	public const int SCUT_SCORE = 10;

	public const int MAX_LOST_CANDIES = 3;

	public const float ROPE_CUT_AT_ONCE_TIMEOUT = 0.05f;

	public const float STAR_RADIUS = 15f;

	public const float MOUTH_OPEN_RADIUS = 100f;

	public const int BLINK_SKIP = 3;

	public const float MOUTH_OPEN_TIME = 1f;

	public const float PUMP_TIMEOUT = 0.05f;

	public const float CAMERA_SPEED = 7f;

	public const int RESTART_STATE_FADE_IN = 0;

	public const int RESTART_STATE_FADE_OUT = 1;

	private const int S_MOVE_DOWN = 0;

	private const int S_WAIT = 1;

	private const int S_MOVE_UP = 2;

	private const int CAMERA_MOVE_TO_CANDY_PART = 0;

	private const int CAMERA_MOVE_TO_CANDY = 1;

	private const int BUTTON_GRAVITY = 0;

	private const int BUTTON_SPIKES = 1;

	private const int PARTS_SEPARATE = 0;

	private const int PARTS_DIST = 1;

	private const int PARTS_NONE = 2;

	private const int CANDY_BLINK_INITIAL = 0;

	private const int CANDY_BLINK_STAR = 1;

	private const int TUTORIAL_SHOW_ANIM = 0;

	private const int TUTORIAL_HIDE_ANIM = 1;

	private const int EARTH_NORMAL_ANIM = 0;

	private const int EARTH_UPSIDEDOWN_ANIM = 1;

	private const int CHAR_ANIMATION_IDLE = 0;

	private const int CHAR_ANIMATION_IDLE2 = 1;

	private const int CHAR_ANIMATION_IDLE3 = 3;

	private const int CHAR_ANIMATION_EX_1 = 4;

	private const int CHAR_ANIMATION_EXCITED = 5;

	private const int CHAR_ANIMATION_PUZZLED = 6;

	private const int CHAR_ANIMATION_FAIL = 7;

	private const int CHAR_ANIMATION_WIN = 8;

	private const int CHAR_ANIMATION_MOUTH_OPEN = 9;

	private const int CHAR_ANIMATION_MOUTH_CLOSE = 10;

	private const int CHAR_ANIMATION_CHEW = 11;

	private const int CHAR_ANIMATION_GREETING = 12;

	private static bool fingerCutCreation = true;

	private static int[] CANDIES = new int[1] { 112 };

	private static int[] BGRS = new int[6] { 181, 183, 185, 187, 264, 268 };

	private static int[] supports = new int[6] { 182, 184, 186, 188, 265, 270 };

	private DelayedDispatcher dd;

	public gameWonDelegate gameSceneDelegate_gameWon;

	public gameLostDelegate gameSceneDelegate_gameLost;

	private AnimationsPool aniPool;

	private AnimationsPool particlesAniPool;

	private AnimationsPool staticAniPool;

	private BaseElement decalsLayer;

	private TileMap back;

	private GameObject target;

	private GameObject targetex;

	private Image support;

	private GameObject candy;

	private Rocket activeRocket;

	private Image candyMain;

	private Image candyTop;

	private Animation candyBlink;

	private Animation candyBubbleAnimation;

	private Animation candyBubbleAnimationL;

	private Animation candyBubbleAnimationR;

	private ConstraintedPoint star;

	private List<Grab> bungees;

	private List<Razor> razors;

	private List<Spikes> spikes;

	private List<Star> stars;

	private List<Bubble> bubbles;

	private List<Pump> pumps;

	private List<Bouncer> bouncers;

	private List<Rocket> rockets;

	private List<Snail> loads;

	private List<MechanicalHand> hands;

	private List<GameObjectSpecial> tutorialImages;

	private List<GameObjectSpecial> tutorialImages2;

	private List<TutorialText> tutorials;

	private List<Drawing> drawings;

	private Animation blink;

	private bool[] dragging = new bool[5];

	private Vector[] startPos = new Vector[5];

	private Vector[] prevStartPos = new Vector[5];

	private float ropePhysicsSpeed;

	private GameObject candyBubble;

	private Animation[] hudStar = new Animation[3];

	public Camera2D camera;

	private float mapWidth;

	private float mapHeight;

	private bool mouthOpen;

	private bool noCandy;

	private int blinkTimer;

	private int idlesTimer;

	private float mouthCloseTimer;

	private float lastCandyRotateDelta;

	private float lastCandyRotateDeltaL;

	private float lastCandyRotateDeltaR;

	private bool spiderTookCandy;

	private int special;

	private bool fastenCamera;

	private int ropesCutAtOnce;

	private float ropeAtOnceTimer;

	private BaseElement handContainer;

	private Image handCandy;

	private bool updatePhysics;

	private float waterLevel;

	private float waterSpeed;

	private WaterElement waterLayer;

	public Drawing drawingPoppedUp;

	public int starsCollected;

	public int starBonus;

	public int timeBonus;

	public int score;

	public float time;

	private float initialCameraToStarDistance;

	public float dimTime;

	public int restartState;

	public bool animateRestartDim;

	private bool freezeCamera;

	private int cameraMoveMode;

	private bool ignoreTouches;

	private bool nightLevel;

	private bool gravityNormal;

	private int twoParts;

	private bool noCandyL;

	private bool noCandyR;

	private float partsDist;

	public List<Image> earthAnims;

	private int tummyTeasers;

	private Vector slastTouch;

	private List<FingerCut>[] fingerCuts = new List<FingerCut>[5];

	private static bool splashes = false;

	private static bool underwater = false;

	public static void enableFingercutsCreation(bool b)
	{
		fingerCutCreation = b;
	}

	public static float FBOUND_PI(float a)
	{
		return (float)(((double)a > Math.PI) ? ((double)a - Math.PI * 2.0) : (((double)a < -Math.PI) ? ((double)a + Math.PI * 2.0) : ((double)a)));
	}

	public virtual void playAnimation(int animId)
	{
		GameObject gameObject;
		GameObject gameObject2;
		switch (animId)
		{
		case 1:
		case 3:
		case 4:
			gameObject = targetex;
			gameObject2 = target;
			break;
		default:
			gameObject = target;
			gameObject2 = targetex;
			break;
		}
		gameObject2.visible = false;
		gameObject.visible = true;
		gameObject.playTimeline(animId);
	}

	public virtual void animateLevelRestart()
	{
		restartState = 0;
		dimTime = 0.15f;
		if (MathHelper.RND_RANGE(0, 10) < 4)
		{
			CTRSoundMgr._playVoice(47 + MathHelper.RND_RANGE(0, 1));
		}
	}

	public override NSObject init()
	{
		if (base.init() != null)
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			dd = (DelayedDispatcher)new DelayedDispatcher().init();
			initialCameraToStarDistance = -1f;
			restartState = -1;
			decalsLayer = new BaseElement();
			decalsLayer.init();
			aniPool = new AnimationsPool();
			aniPool.visible = false;
			addChild(aniPool);
			particlesAniPool = (AnimationsPool)new AnimationsPool().init();
			particlesAniPool.visible = false;
			addChild(particlesAniPool);
			staticAniPool = new AnimationsPool();
			staticAniPool.visible = false;
			addChild(staticAniPool);
			camera = new Camera2D().initWithSpeedandType(7f, CAMERA_TYPE.CAMERA_SPEED_DELAY);
			int textureResID = BGRS[cTRRootController.getPack()];
			back = new TileMap().initWithRowsColumns(1, 1);
			back.setRepeatHorizontally(TileMap.Repeat.REPEAT_ALL);
			back.setRepeatVertically(TileMap.Repeat.REPEAT_ALL);
			back.addTileQuadwithID(Application.getTexture(textureResID), 0, 0);
			back.fillStartAtRowColumnRowsColumnswithTile(0, 0, 1, 1, 0);
			for (int i = 0; i < 3; i++)
			{
				hudStar[i] = Animation.Animation_createWithResID(145);
				hudStar[i].doRestoreCutTransparency();
				hudStar[i].addAnimationDelayLoopFirstLast(0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 10);
				hudStar[i].setPauseAtIndexforAnimation(10, 0);
				hudStar[i].x = hudStar[i].width * i;
				hudStar[i].y = 0f;
				hudStar[i].x -= FrameworkTypes.SCREEN_OFFSET_X;
				hudStar[i].y -= FrameworkTypes.SCREEN_OFFSET_Y;
				hudStar[i].texture.PixelCorrectionY();
				hudStar[i].x += 0.33f;
				hudStar[i].y += 0.33f;
				addChild(hudStar[i]);
			}
			for (int j = 0; j < 5; j++)
			{
				fingerCuts[j] = new List<FingerCut>();
				NSObject.NSRET(fingerCuts[j]);
			}
		}
		return this;
	}

	public virtual void xmlLoaderFinishedWithfromwithSuccess(XMLNode rootNode, NSString url, bool success)
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		cTRRootController.setMap(rootNode);
		if (animateRestartDim)
		{
			animateLevelRestart();
		}
		else
		{
			restart();
		}
	}

	public override void show()
	{
		CTRSoundMgr.EnableLoopedSounds(bEnable: true);
		particlesAniPool.removeAllChilds();
		aniPool.removeAllChilds();
		staticAniPool.removeAllChilds();
		twoParts = 2;
		partsDist = 0f;
		CTRSoundMgr._stopLoopedSounds();
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		XMLNode map = cTRRootController.getMap();
		bungees = new List<Grab>();
		razors = new List<Razor>();
		this.spikes = new List<Spikes>();
		stars = new List<Star>();
		bubbles = new List<Bubble>();
		pumps = new List<Pump>();
		rockets = new List<Rocket>();
		bouncers = new List<Bouncer>();
		tutorialImages = new List<GameObjectSpecial>();
		tutorialImages2 = new List<GameObjectSpecial>();
		tutorials = new List<TutorialText>();
		drawings = new List<Drawing>();
		loads = new List<Snail>();
		hands = new List<MechanicalHand>();
		this.star = (ConstraintedPoint)new ConstraintedPoint().init();
		this.star.setWeight(1f);
		int num = Preferences._getIntForKey("PREFS_SELECTED_CANDY");
		candy = GameObject.GameObject_createWithResIDQuad(CANDIES[num], 0);
		candy.doRestoreCutTransparency();
		NSObject.NSRET(candy);
		candy.anchor = 18;
		candy.bb = new Rectangle(46.0, 49.0, 35.0, 35.0);
		candy.passTransformationsToChilds = false;
		candy.scaleX = (candy.scaleY = 0.71f);
		candyMain = GameObject.GameObject_createWithResIDQuad(CANDIES[num], 1);
		candyMain.doRestoreCutTransparency();
		candyMain.anchor = (candyMain.parentAnchor = 18);
		candy.addChild(candyMain);
		candyMain.scaleX = (candyMain.scaleY = 0.71f);
		candyTop = GameObject.GameObject_createWithResIDQuad(CANDIES[num], 2);
		candyTop.doRestoreCutTransparency();
		candyTop.anchor = (candyTop.parentAnchor = 18);
		candy.addChild(candyTop);
		candyTop.scaleX = (candyTop.scaleY = 0.71f);
		candyBlink = Animation.Animation_createWithResID(CANDIES[num]);
		candyBlink.doRestoreCutTransparency();
		candyBlink.addAnimationWithIDDelayLoopFirstLast(0, 0.07f, Timeline.LoopType.TIMELINE_NO_LOOP, 8, 17);
		candyBlink.addAnimationWithIDDelayLoopCountSequence(1, 0.3f, Timeline.LoopType.TIMELINE_NO_LOOP, 2, 18, new List<int> { 18 });
		Timeline timeline = candyBlink.getTimeline(1);
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
		candyBlink.visible = false;
		candyBlink.anchor = (candyBlink.parentAnchor = 9);
		candyBlink.scaleX = (candyBlink.scaleY = 0.71f);
		candy.addChild(candyBlink);
		candyBubbleAnimation = Animation.Animation_createWithResID(137);
		candyBubbleAnimation.x = candy.x;
		candyBubbleAnimation.y = candy.y;
		candyBubbleAnimation.parentAnchor = (candyBubbleAnimation.anchor = 18);
		candyBubbleAnimation.addAnimationDelayLoopFirstLast(0.05f, Timeline.LoopType.TIMELINE_REPLAY, 0, 12);
		candyBubbleAnimation.playTimeline(0);
		candy.addChild(candyBubbleAnimation);
		candyBubbleAnimation.visible = false;
		for (int i = 0; i < 3; i++)
		{
			if (hudStar[i] != null)
			{
				if (hudStar[i].getCurrentTimeline() != null)
				{
					hudStar[i].getCurrentTimeline().stopTimeline();
				}
				hudStar[i].setDrawQuad(0);
			}
		}
		int count = map.childs().Count;
		for (int j = 0; j < count; j++)
		{
			XMLNode xMLNode = map.childs()[j];
			int count2 = xMLNode.childs().Count;
			for (int k = 0; k < count2; k++)
			{
				XMLNode xMLNode2 = xMLNode.childs()[k];
				if (xMLNode2.Name == "map")
				{
					mapWidth = xMLNode2["width"].floatValue();
					mapHeight = xMLNode2["height"].floatValue();
				}
				else if (xMLNode2.Name == "gameDesign")
				{
					special = xMLNode2["special"].intValue();
					ropePhysicsSpeed = xMLNode2["ropePhysicsSpeed"].floatValue();
					nightLevel = xMLNode2["nightLevel"].isEqualToString(NSObject.NSS("true"));
					twoParts = ((!xMLNode2["twoParts"].isEqualToString(NSObject.NSS("true"))) ? 2 : 0);
					waterLevel = xMLNode2["water"].floatValue();
					if (waterLevel != 0f)
					{
						waterLevel = waterLevel * 1f + 0f;
					}
					waterSpeed = xMLNode2["waterSpeed"].floatValue();
					if (waterLevel != 0f)
					{
						waterLayer = (WaterElement)new WaterElement().initWithWidthHeight(mapWidth, waterLevel);
						waterLayer.y = mapHeight - waterLevel;
					}
				}
				else if (xMLNode2.Name == "candy")
				{
					this.star.pos.x = (float)xMLNode2["x"].intValue() * 1f + 0f;
					this.star.pos.y = (float)xMLNode2["y"].intValue() * 1f + 0f;
					candy.x = this.star.pos.x;
					candy.y = this.star.pos.y;
					BaseElement.calculateTopLeft(candy);
				}
			}
		}
		count = map.childs().Count;
		for (int l = 0; l < count; l++)
		{
			XMLNode xMLNode3 = map.childs()[l];
			int count3 = xMLNode3.childs().Count;
			for (int m = 0; m < count3; m++)
			{
				XMLNode xMLNode4 = xMLNode3.childs()[m];
				if (xMLNode4.Name == "star")
				{
					Star star = Star.Star_createWithResID(144);
					star.x = (float)xMLNode4["x"].intValue() * 1f + 0f;
					star.y = (float)xMLNode4["y"].intValue() * 1f + 0f;
					star.timeout = xMLNode4["timeout"].floatValue();
					star.createAnimations();
					star.parseMover(xMLNode4);
					star.update(0f);
					stars.Add(star);
				}
				else if (xMLNode4.Name == "tutorialText")
				{
					if (shouldSkipTutorialElement(xMLNode4))
					{
						continue;
					}
					NSString nSString = xMLNode4["text"];
					if (nSString != null)
					{
						TutorialText tutorialText = (TutorialText)new TutorialText().initWithFont(Application.getFont(6));
						tutorialText.color = RGBAColor.MakeRGBA(1.0, 1.0, 1.0, 0.9);
						tutorialText.x = (float)xMLNode4["x"].intValue() * 1f + 0f;
						tutorialText.y = (float)xMLNode4["y"].intValue() * 1f + 0f;
						tutorialText.special = xMLNode4["special"].intValue();
						tutorialText.setAlignment(2);
						tutorialText.setStringandWidth(nSString, (float)xMLNode4["width"].intValue() * 1f);
						tutorialText.color = RGBAColor.transparentRGBA;
						Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(5);
						timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
						if (shouldShowHandAnimation())
						{
							timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.7));
						}
						timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
						if (cTRRootController.getPack() == 0 && cTRRootController.getLevel() == 0)
						{
							timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 10.0));
						}
						else
						{
							timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 5.0));
						}
						timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
						tutorialText.addTimelinewithID(timeline2, 0);
						if (tutorialText.special == 0)
						{
							tutorialText.playTimeline(0);
						}
						tutorials.Add(tutorialText);
					}
				}
				else if (xMLNode4.Name == "tutorial01" || xMLNode4.Name == "tutorial02" || xMLNode4.Name == "tutorial03" || xMLNode4.Name == "tutorial04" || xMLNode4.Name == "tutorial05" || xMLNode4.Name == "tutorial06" || xMLNode4.Name == "tutorial07" || xMLNode4.Name == "tutorial08" || xMLNode4.Name == "tutorial09" || xMLNode4.Name == "tutorial10")
				{
					if (shouldSkipTutorialElement(xMLNode4))
					{
						continue;
					}
					NSString nSString2 = new NSString(xMLNode4.Name.Substring(8));
					int q = nSString2.intValue() - 1;
					GameObjectSpecial gameObjectSpecial = GameObjectSpecial.GameObjectSpecial_createWithResIDQuad(153, q);
					gameObjectSpecial.color = RGBAColor.transparentRGBA;
					gameObjectSpecial.x = (float)xMLNode4["x"].intValue() * 1f + 0f;
					gameObjectSpecial.y = (float)xMLNode4["y"].intValue() * 1f + 0f;
					gameObjectSpecial.rotation = xMLNode4["angle"].intValue();
					gameObjectSpecial.special = xMLNode4["special"].intValue();
					gameObjectSpecial.parseMover(xMLNode4);
					Timeline timeline3 = new Timeline().initWithMaxKeyFramesOnTrack(5);
					timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
					if (shouldShowHandAnimation())
					{
						timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.7));
					}
					timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
					if (cTRRootController.getPack() == 0 && cTRRootController.getLevel() == 0)
					{
						timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 10.0));
					}
					else
					{
						timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 5.2));
					}
					timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
					gameObjectSpecial.addTimelinewithID(timeline3, 0);
					if (gameObjectSpecial.special == 0)
					{
						gameObjectSpecial.playTimeline(0);
					}
					if (gameObjectSpecial.special == 2)
					{
						Timeline timeline4 = new Timeline().initWithMaxKeyFramesOnTrack(6);
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
						if (shouldShowHandAnimation())
						{
							timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.7));
						}
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.1));
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
						timeline4.addKeyFrame(KeyFrame.makePos(gameObjectSpecial.x, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
						if (shouldShowHandAnimation())
						{
							timeline4.addKeyFrame(KeyFrame.makePos(gameObjectSpecial.x, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.7));
						}
						timeline4.addKeyFrame(KeyFrame.makePos(gameObjectSpecial.x, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
						timeline4.addKeyFrame(KeyFrame.makePos(gameObjectSpecial.x, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
						timeline4.addKeyFrame(KeyFrame.makePos((double)gameObjectSpecial.x + 115.0, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.5));
						timeline4.addKeyFrame(KeyFrame.makePos((double)gameObjectSpecial.x + 220.0, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.5));
						timeline4.loopsLimit = 2;
						timeline4.setTimelineLoopType(Timeline.LoopType.TIMELINE_REPLAY);
						gameObjectSpecial.addTimelinewithID(timeline4, 1);
						gameObjectSpecial.playTimeline(1);
						gameObjectSpecial.rotation = 10f;
						tutorialImages2.Add(gameObjectSpecial);
					}
					else
					{
						tutorialImages.Add(gameObjectSpecial);
					}
				}
				else if (xMLNode4.Name == "hidden01" || xMLNode4.Name == "hidden02" || xMLNode4.Name == "hidden03")
				{
					NSString nSString3 = new NSString(xMLNode4.Name.Substring(7));
					int h = nSString3.intValue() - 1;
					int num2 = xMLNode4["drawing"].intValue() - 1;
					if (num2 != 4 && CTRPreferences.getDrawingUnlocked(num2) == 0)
					{
						Drawing drawing = (Drawing)new Drawing().initWithHiddenDrawing(h, num2);
						drawing.delegateDrawingDelegate = this;
						drawing.x = xMLNode4["x"].floatValue() * 1f + 0f;
						drawing.y = xMLNode4["y"].floatValue() * 1f + 0f;
						drawing.rotation = xMLNode4["angle"].floatValue();
						drawings.Add(drawing);
					}
				}
				else if (xMLNode4.Name == "bubble")
				{
					int q2 = MathHelper.RND_RANGE(1, 3);
					Bubble bubble = Bubble.Bubble_createWithResIDQuad(141, q2);
					bubble.doRestoreCutTransparency();
					bubble.bb = new Rectangle(0.0, 0.0, 57.0, 57.0);
					bubble.x = (float)xMLNode4["x"].intValue() * 1f + 0f;
					bubble.y = (float)xMLNode4["y"].intValue() * 1f + 0f;
					bubble.anchor = 18;
					bubble.popped = false;
					Image image = Image.Image_createWithResIDQuad(141, 0);
					image.doRestoreCutTransparency();
					image.parentAnchor = (image.anchor = 18);
					bubble.addChild(image);
					bubbles.Add(bubble);
				}
				else if (xMLNode4.Name == "pump")
				{
					Pump pump = Pump.Pump_createWithResID(152);
					pump.texture.PixelCorrectionXY();
					pump.doRestoreCutTransparency();
					pump.addAnimationWithDelayLoopedCountSequence(0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 4, 1, new List<int> { 2, 3, 0 });
					pump.bb = new Rectangle(94.0, 95.0, 57.0, 57.0);
					pump.x = (float)xMLNode4["x"].intValue() * 1f + 0f;
					pump.y = (float)xMLNode4["y"].intValue() * 1f + 0f;
					pump.rotation = xMLNode4["angle"].floatValue() + 90f;
					pump.updateRotation();
					pump.anchor = 18;
					pumps.Add(pump);
				}
				else if (xMLNode4.Name == "rocket")
				{
					Rocket rocket = Rocket.Rocket_createWithResIDQuad(154, 10);
					rocket.scaleX = (rocket.scaleY = 0.7f);
					rocket.doRestoreCutTransparency();
					rocket.delegateRocketDelegate = this;
					Vector quadCenter = Image.getQuadCenter(154, 10);
					Vector quadSize = Image.getQuadSize(154, 10);
					quadSize.x *= 0.6f;
					quadSize.y *= 0.05f;
					rocket.bb = FrameworkTypes.MakeRectangle(quadCenter.x - quadSize.x / 2f, quadCenter.y - quadSize.y / 2f, quadSize.x, quadSize.y);
					rocket.x = (float)xMLNode4["x"].intValue() * 1f + 0f;
					rocket.y = (float)xMLNode4["y"].intValue() * 1f + 0f;
					rocket.rotation = xMLNode4["angle"].floatValue() - 180f;
					rocket.impulse = xMLNode4["impulse"].floatValue();
					rocket.impulseFactor = xMLNode4["impulseFactor"].floatValue();
					if (rocket.impulseFactor == 0f)
					{
						rocket.impulseFactor = 0.6f;
					}
					rocket.time = xMLNode4["time"].floatValue();
					rocket.isRotatable = xMLNode4["isRotatable"].isEqualToString(NSObject.NSS("true"));
					rocket.startRotation = rocket.rotation;
					rocket.parseMover(xMLNode4);
					rocket.rotateWithBB(rocket.rotation);
					rocket.updateRotation();
					rocket.anchor = 18;
					rockets.Add(rocket);
					rocket.point.pos.x = rocket.x;
					rocket.point.pos.y = rocket.y;
					if (rocket.isRotatable)
					{
						Image image2 = Image.Image_createWithResIDQuad(154, 0);
						image2.parentAnchor = (image2.anchor = 18);
						image2.doRestoreCutTransparency();
						image2.x = rocket.x;
						image2.y = rocket.y;
						decalsLayer.addChild(image2);
					}
				}
				else if (xMLNode4.Name == "hand")
				{
					int num3 = xMLNode4["segmentsCount"].intValue();
					MechanicalHand mechanicalHand = (MechanicalHand)new MechanicalHand().init();
					mechanicalHand.x = (float)xMLNode4["x"].intValue() * 1f + 0f;
					mechanicalHand.y = (float)xMLNode4["y"].intValue() * 1f + 0f;
					BaseElement.calculateTopLeft(mechanicalHand);
					for (int n = 1; n <= num3; n++)
					{
						float num4 = xMLNode4["segment" + n + "Angle"].floatValue();
						if (num4 < 0f)
						{
							num4 += 360f;
						}
						float l2 = xMLNode4["segment" + n + "Length"].floatValue();
						bool r = xMLNode4["segment" + n + "Rotatable"].boolValue();
						mechanicalHand.addSegmentWithLengthAngleRotatable(l2, num4, r);
					}
					BaseElement.calculateTopLeft(mechanicalHand.theClaw());
					mechanicalHand.update(0f);
					hands.Add(mechanicalHand);
				}
				else if (xMLNode4.Name == "spike1" || xMLNode4.Name == "spike2" || xMLNode4.Name == "spike3" || xMLNode4.Name == "spike4" || xMLNode4.Name == "electro")
				{
					float px = (float)xMLNode4["x"].intValue() * 1f + 0f;
					float py = (float)xMLNode4["y"].intValue() * 1f + 0f;
					int w = xMLNode4["size"].intValue();
					double an = xMLNode4["angle"].intValue();
					NSString nSString4 = xMLNode4["toggled"];
					int t = -1;
					if (nSString4 != null && nSString4.length() > 0)
					{
						t = (nSString4.isEqualToString(NSObject.NSS("false")) ? (-1) : nSString4.intValue());
					}
					Spikes spikes = (Spikes)new Spikes().initWithPosXYWidthAndAngleToggled(px, py, w, an, t);
					spikes.parseMover(xMLNode4);
					if (xMLNode4.Name == "electro")
					{
						spikes.electro = true;
						spikes.initialDelay = xMLNode4["initialDelay"].floatValue();
						spikes.onTime = xMLNode4["onTime"].floatValue();
						spikes.offTime = xMLNode4["offTime"].floatValue();
						spikes.electroTimer = 0f;
						spikes.turnElectroOff();
						spikes.electroTimer += spikes.initialDelay;
						spikes.updateRotation();
					}
					else
					{
						spikes.electro = false;
					}
					this.spikes.Add(spikes);
				}
				else if (xMLNode4.Name == "load")
				{
					float num5 = (float)xMLNode4["x"].intValue() * 1f + 0f;
					float num6 = (float)xMLNode4["y"].intValue() * 1f + 0f;
					Snail snail = Snail.Snail_createWithResIDQuad(262, 8);
					snail.anchor = 18;
					snail.x = num5;
					snail.y = num6;
					loads.Add(snail);
				}
				else if (xMLNode4.Name == "bouncer1" || xMLNode4.Name == "bouncer2")
				{
					float px2 = (float)xMLNode4["x"].intValue() * 1f + 0f;
					float py2 = (float)xMLNode4["y"].intValue() * 1f + 0f;
					int w2 = xMLNode4["size"].intValue();
					double an2 = xMLNode4["angle"].intValue();
					Bouncer bouncer = (Bouncer)new Bouncer().initWithPosXYWidthAndAngle(px2, py2, w2, an2);
					bouncer.parseMover(xMLNode4);
					bouncers.Add(bouncer);
				}
				else if (xMLNode4.Name == "grab")
				{
					float hx = (float)xMLNode4["x"].intValue() * 1f + 0f;
					float hy = (float)xMLNode4["y"].intValue() * 1f + 0f;
					float len = (float)xMLNode4["length"].intValue() * 1f;
					float num7 = xMLNode4["radius"].floatValue();
					bool wheel = xMLNode4["wheel"].isEqualToString(NSObject.NSS("true"));
					bool kickable = xMLNode4["kickable"].isEqualToString(NSObject.NSS("true"));
					bool kicked = xMLNode4["kicked"].isEqualToString(NSObject.NSS("true"));
					bool invisible = xMLNode4["invisible"].isEqualToString(NSObject.NSS("true"));
					float l3 = xMLNode4["moveLength"].floatValue() * 1f;
					bool v = xMLNode4["moveVertical"].isEqualToString(NSObject.NSS("true"));
					float o = xMLNode4["moveOffset"].floatValue() * 1f;
					bool spider = xMLNode4["spider"].isEqualToString(NSObject.NSS("true"));
					xMLNode4["part"].isEqualToString(NSObject.NSS("L"));
					bool flag = xMLNode4["gun"].isEqualToString(NSObject.NSS("true"));
					Grab grab = (Grab)new Grab().init();
					grab.x = hx;
					grab.y = hy;
					grab.wheel = wheel;
					grab.gun = flag;
					grab.kickable = kickable;
					grab.kicked = kicked;
					grab.invisible = invisible;
					grab.setSpider(spider);
					if (num7 != -1f)
					{
						num7 *= 1f;
					}
					if (num7 == -1f && !flag)
					{
						ConstraintedPoint constraintedPoint = this.star;
						Bungee bungee = (Bungee)new Bungee().initWithHeadAtXYTailAtTXTYandLength(null, hx, hy, constraintedPoint, constraintedPoint.pos.x, constraintedPoint.pos.y, len);
						bungee.bungeeAnchor.pin = bungee.bungeeAnchor.pos;
						grab.setRope(bungee);
						if (grab.kicked)
						{
							bungee.bungeeAnchor.pin = MathHelper.vect(-1f, -1f);
							bungee.bungeeAnchor.setWeight(0.1f);
						}
					}
					grab.setRadius(num7);
					grab.setMoveLengthVerticalOffset(l3, v, o);
					if (grab.gun)
					{
						Vector v2 = MathHelper.vectSub(MathHelper.vect(grab.x, grab.y), this.star.pos);
						grab.gunArrow.rotation = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v2));
					}
					bungees.Add(grab);
				}
				else if (xMLNode4.Name == "target")
				{
					target = GameObject.GameObject_createWithResID(149);
					target.doRestoreCutTransparency();
					target.bb = new Rectangle(90.0, 110.0, 25.0, 1.0);
					target.addAnimationWithIDDelayLoopFirstLast(0, 0.05f, Timeline.LoopType.TIMELINE_REPLAY, 0, 18);
					target.addAnimationWithIDDelayLoopFirstLast(12, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 103, 131);
					target.addAnimationWithIDDelayLoopFirstLast(5, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 56, 75);
					target.addAnimationWithIDDelayLoopFirstLast(6, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 76, 102);
					target.addAnimationWithIDDelayLoopFirstLast(7, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 19, 31);
					target.addAnimationWithIDDelayLoopFirstLast(8, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 41, 44);
					target.addAnimationWithIDDelayLoopFirstLast(9, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 32, 40);
					target.addAnimationWithIDDelayLoopFirstLast(10, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 41, 44);
					target.addAnimationWithIDDelayLoopFirstLast(11, 0.05f, Timeline.LoopType.TIMELINE_REPLAY, 45, 53);
					target.switchToAnimationatEndOfAnimationDelay(11, 8, 0.05f);
					target.switchToAnimationatEndOfAnimationDelay(6, 10, 0.05f);
					target.switchToAnimationatEndOfAnimationDelay(0, 12, 0.05f);
					target.switchToAnimationatEndOfAnimationDelay(0, 5, 0.05f);
					target.switchToAnimationatEndOfAnimationDelay(0, 6, 0.05f);
					NSObject.NSRET(target);
					targetex = GameObject.GameObject_createWithResID(150);
					targetex.anchor = 18;
					targetex.doRestoreCutTransparency();
					targetex.visible = false;
					targetex.addAnimationWithIDDelayLoopFirstLast(4, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 29);
					Timeline timeline5 = targetex.getTimeline(4);
					timeline5.delegateTimelineDelegate = this;
					targetex.addAnimationWithIDDelayLoopFirstLast(1, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 54, 78);
					timeline5 = targetex.getTimeline(1);
					timeline5.delegateTimelineDelegate = this;
					targetex.addAnimationWithIDDelayLoopFirstLast(3, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 30, 53);
					timeline5 = targetex.getTimeline(3);
					timeline5.delegateTimelineDelegate = this;
					NSObject.NSRET(targetex);
					target.texture.PixelCorrectionXY();
					targetex.texture.PixelCorrectionXY();
					if (CTRRootController.isShowGreeting())
					{
						dd.callObjectSelectorParamafterDelay(selector_showGreeting, null, 1.3f);
						CTRRootController.setShowGreeting(s: false);
					}
					playAnimation(0);
					Timeline timeline6 = target.getTimeline(0);
					timeline6.delegateTimelineDelegate = this;
					target.isDrawBB = true;
					target.setPauseAtIndexforAnimation(8, 9);
					blink = Animation.Animation_createWithResID(149);
					blink.parentAnchor = 9;
					blink.visible = false;
					blink.addAnimationWithIDDelayLoopCountSequence(0, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 4, 54, new List<int> { 55, 55, 55 });
					blink.setActionTargetParamSubParamAtIndexforAnimation("ACTION_SET_VISIBLE", blink, 0, 0, 2, 0);
					blinkTimer = 3;
					blink.doRestoreCutTransparency();
					target.addChild(blink);
					_ = (CTRRootController)Application.sharedRootController();
					int r2 = supports[cTRRootController.getPack()];
					support = Image.Image_createWithResID(r2);
					NSObject.NSRET(support);
					support.doRestoreCutTransparency();
					support.anchor = 18;
					NSString nSString5 = xMLNode4["x"];
					float num8 = (float)(1 - nSString5.intValue() % 2) * 0.33f;
					target.x = (targetex.x = (support.x = (float)nSString5.intValue() * 1f + 0f + num8));
					NSString nSString6 = xMLNode4["y"];
					float num9 = (float)(1 - nSString6.intValue() % 2) * 0.33f;
					target.y = (targetex.y = (support.y = (float)nSString6.intValue() * 1f + 0f + num9));
					idlesTimer = MathHelper.RND_RANGE(5, 20);
				}
			}
		}
		startCamera();
		tummyTeasers = 0;
		starsCollected = 0;
		candyBubble = null;
		mouthOpen = false;
		noCandy = twoParts != 2;
		blink.playTimeline(0);
		spiderTookCandy = false;
		time = 0f;
		score = 0;
		gravityNormal = true;
		MaterialPoint.globalGravity = MathHelper.vect(0f, 784f);
		dimTime = 0f;
		ropesCutAtOnce = 0;
		ropeAtOnceTimer = 0f;
		dd.callObjectSelectorParamafterDelay(selector_doCandyBlink, null, 1f);
		Text text = Text.createWithFontandString(5, NSObject.NSS(cTRRootController.getPack() + 1 + " - " + (cTRRootController.getLevel() + 1)));
		text.anchor = 33;
		Text text2 = Text.createWithFontandString(5, Application.getString(1179668));
		text2.anchor = 33;
		text2.parentAnchor = 9;
		text.x = 5f;
		text.y = FrameworkTypes.SCREEN_HEIGHT + 10f;
		text2.y = 30f;
		text2.rotationCenterX -= (float)text2.width / 2f;
		text2.scaleX = (text2.scaleY = 0.7f);
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
		{
			text2.y -= 7f;
		}
		text.addChild(text2);
		Timeline timeline7 = new Timeline().initWithMaxKeyFramesOnTrack(5);
		timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
		timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
		timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
		timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
		text.addTimelinewithID(timeline7, 0);
		text.playTimeline(0);
		timeline7.delegateTimelineDelegate = staticAniPool;
		staticAniPool.addChild(text);
		text.y += FrameworkTypes.SCREEN_OFFSET_Y;
		text.x -= FrameworkTypes.SCREEN_OFFSET_X;
		updatePhysics = true;
		if (shouldShowHandAnimation())
		{
			Vector quadOffset = Image.getQuadOffset(CTRResourceMgr.handleWvgaResource(135), 1);
			Vector quadOffset2 = Image.getQuadOffset(CTRResourceMgr.handleWvgaResource(135), 0);
			Vector quadOffset3 = Image.getQuadOffset(CTRResourceMgr.handleWvgaResource(135), 2);
			Image image3 = Image.Image_createWithResIDQuad(135, 0);
			image3.anchor = (image3.parentAnchor = 9);
			handContainer = (BaseElement)new BaseElement().init();
			handContainer.width = image3.width;
			handContainer.height = image3.height;
			handContainer.parentAnchor = (handContainer.anchor = 9);
			aniPool.addChild(handContainer);
			handCandy = Image.Image_createWithResIDQuad(112, 0);
			handCandy.doRestoreCutTransparency();
			handCandy.scaleX = (handCandy.scaleY = 0.71f);
			handCandy.parentAnchor = 9;
			handCandy.anchor = 18;
			Image.setElementPositionWithQuadCenter(handCandy, 135, 1);
			handCandy.x -= quadOffset2.x;
			handCandy.y -= quadOffset2.y;
			handContainer.addChild(handCandy);
			handContainer.addChild(image3);
			Image image4 = Image.Image_createWithResIDQuad(112, 1);
			image4.doRestoreCutTransparency();
			image4.parentAnchor = (image4.anchor = 18);
			handCandy.addChild(image4);
			Image image5 = Image.Image_createWithResIDQuad(112, 2);
			image5.doRestoreCutTransparency();
			image5.parentAnchor = (image5.anchor = 18);
			handCandy.addChild(image5);
			Vector pos = this.star.pos;
			Vector v3 = MathHelper.vectSub(quadOffset, quadOffset2);
			Vector vector = MathHelper.vectSub(pos, v3);
			handContainer.x = vector.x;
			handContainer.y = vector.y;
			float num10 = -handContainer.height;
			TiledImage tiledImage = TiledImage.TiledImage_createWithResIDQuad(135, 2);
			tiledImage.setTile(2);
			tiledImage.height = (int)FrameworkTypes.SCREEN_HEIGHT - 1;
			tiledImage.parentAnchor = (tiledImage.anchor = 9);
			tiledImage.x = MathHelper.vectSub(quadOffset3, quadOffset2).x;
			tiledImage.y = -tiledImage.height;
			image3.addChild(tiledImage);
			Timeline timeline8 = new Timeline().initWithMaxKeyFramesOnTrack(5);
			timeline8.addKeyFrame(KeyFrame.makePos(vector.x, num10, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline8.addKeyFrame(KeyFrame.makePos(vector.x, num10, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
			timeline8.addKeyFrame(KeyFrame.makePos(vector.x, vector.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.7));
			handContainer.addTimeline(timeline8);
			timeline8.delegateTimelineDelegate = this;
			Timeline timeline9 = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline9.addKeyFrame(KeyFrame.makePos(vector.x, vector.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.4));
			timeline9.addKeyFrame(KeyFrame.makePos(vector.x, num10, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.7));
			handContainer.addTimeline(timeline9);
			timeline9.delegateTimelineDelegate = aniPool;
			handContainer.playTimeline(0);
			candy.setEnabled(e: false);
			updatePhysics = false;
			int num11 = bungees.Count();
			for (int num12 = 0; num12 < num11; num12++)
			{
				Grab grab2 = bungees[num12];
				Bungee rope = grab2.rope;
				if (rope != null)
				{
					rope.appearTime = 0f;
				}
			}
		}
		activeRocket = null;
	}

	public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
	{
		if (t.element == handContainer || i != 1)
		{
			return;
		}
		blinkTimer--;
		if (blinkTimer == 0)
		{
			blink.visible = true;
			blink.playTimeline(0);
			blinkTimer = 3;
		}
		idlesTimer--;
		if (idlesTimer == 0)
		{
			switch (MathHelper.RND_RANGE(0, 2))
			{
			case 0:
				playAnimation(1);
				break;
			case 1:
				playAnimation(3);
				break;
			default:
				playAnimation(4);
				break;
			}
			idlesTimer = MathHelper.RND_RANGE(5, 20);
		}
	}

	public virtual void timelineFinished(Timeline t)
	{
		if (t.element == targetex)
		{
			playAnimation(0);
		}
		else if (t.element == handContainer)
		{
			candy.setEnabled(e: true);
			handCandy.setEnabled(e: false);
			handContainer.playTimeline(1);
			updatePhysics = true;
		}
	}

	public override void hide()
	{
		if (waterLayer != null)
		{
			waterLayer.prepareToRelease();
			NSObject.NSREL(waterLayer);
			waterLayer = null;
		}
		CTRSoundMgr._stopLoopedSounds();
		if (decalsLayer != null)
		{
			decalsLayer.removeAllChilds();
		}
		star = null;
		candy = null;
	}

	public override void update(float delta)
	{
		if (drawings != null)
		{
			for (int i = 0; i < drawings.Count; i++)
			{
				drawings[i]?.update(delta);
			}
		}
		if (drawingPoppedUp != null)
		{
			return;
		}
		base.update(delta);
		if (decalsLayer != null)
		{
			decalsLayer.update(delta);
		}
		if (waterLayer != null)
		{
			waterLayer.update(delta);
			if (GameObject.rectInObject(0f, mapHeight - waterLevel - 2f, mapWidth, mapHeight - waterLevel + 2f, candy))
			{
				if (!splashes)
				{
					waterLayer.addWaterParticlesAtXY(candy.x, mapHeight - waterLevel + 3f);
					CTRSoundMgr._playSound(258);
				}
				splashes = true;
			}
			else
			{
				splashes = false;
			}
			if (candy.y - candy.texture.quadRects[0].h / 2f > mapHeight - waterLevel)
			{
				if (!underwater)
				{
					int num = Preferences._getIntForKey("PREFS_UNDERWATER") + 1;
					Preferences._setIntforKey(num, "PREFS_UNDERWATER", comit: false);
					if (num >= 150)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acDeepDiver"));
					}
				}
				underwater = true;
			}
			else
			{
				underwater = false;
			}
		}
		dd.update(delta);
		for (int j = 0; j < 5; j++)
		{
			for (int k = 0; k < fingerCuts[j].Count; k++)
			{
				FingerCut fingerCut = fingerCuts[j][k];
				if (Mover.moveVariableToTarget(ref fingerCut.c.a, 0f, 10f, delta))
				{
					fingerCuts[j].Remove(fingerCut);
					k--;
				}
			}
		}
		Mover.moveVariableToTarget(ref ropeAtOnceTimer, 0f, 1f, delta);
		ConstraintedPoint constraintedPoint = this.star;
		float v = constraintedPoint.pos.x - FrameworkTypes.SCREEN_WIDTH / 2f;
		float v2 = constraintedPoint.pos.y - FrameworkTypes.SCREEN_HEIGHT / 2f;
		float num2 = MathHelper.FIT_TO_BOUNDARIES(v, 0f, mapWidth - FrameworkTypes.SCREEN_WIDTH);
		float num3 = MathHelper.FIT_TO_BOUNDARIES(v2, 0f, mapHeight - FrameworkTypes.SCREEN_HEIGHT);
		camera.moveToXYImmediate(num2, num3, immediate: false);
		if (!freezeCamera || camera.type != CAMERA_TYPE.CAMERA_SPEED_DELAY)
		{
			camera.update(delta);
		}
		if (camera.type == CAMERA_TYPE.CAMERA_SPEED_PIXELS)
		{
			float num4 = MathHelper.vectDistance(camera.pos, MathHelper.vect(num2, num3));
			if (num4 < 50f)
			{
				ignoreTouches = false;
			}
			if (fastenCamera)
			{
				if (camera.speed < 2700f)
				{
					camera.speed *= 1.5f;
				}
			}
			else if ((double)num4 > (double)initialCameraToStarDistance / 2.0)
			{
				camera.speed += delta * 200f;
				camera.speed = Math.Min(300f, camera.speed);
			}
			else
			{
				camera.speed -= delta * 200f;
				camera.speed = Math.Max(50f, camera.speed);
			}
			if ((double)Math.Abs(camera.pos.x - num2) < 1.0 && (double)Math.Abs(camera.pos.y - num3) < 1.0)
			{
				camera.type = CAMERA_TYPE.CAMERA_SPEED_DELAY;
				camera.speed = 7f;
			}
		}
		else
		{
			time += delta;
		}
		bool flag = false;
		if (hands != null)
		{
			foreach (MechanicalHand hand in hands)
			{
				if (hand != null && hand.state == 1)
				{
					flag = true;
					break;
				}
			}
		}
		if (bungees.Count > 0)
		{
			bool flag2 = false;
			int count = bungees.Count;
			for (int l = 0; l < count; l++)
			{
				Grab grab = bungees[l];
				grab.update(delta);
				Bungee rope = grab.rope;
				if (rope != null && updatePhysics)
				{
					if (rope.cut != -1 && (double)rope.cutTime == 0.0)
					{
						continue;
					}
					rope?.update(delta * ropePhysicsSpeed, ropePhysicsSpeed);
					if (grab.hasSpider)
					{
						if ((updatePhysics && camera.type != 0) || !ignoreTouches)
						{
							grab.updateSpider(delta);
						}
						if (grab.spiderPos == -1f)
						{
							spiderWon(grab);
							break;
						}
					}
					if (grab.stickTimer != -1f)
					{
						grab.stickTimer += delta;
						if (grab.stickTimer > 0.05f)
						{
							if (GameObject.rectInObject(0f, 0f, mapWidth, mapHeight, grab))
							{
								rope.bungeeAnchor.pin = rope.bungeeAnchor.pos;
								grab.kicked = false;
								rope.bungeeAnchor.setWeight(0.02f);
								grab.updateKickState();
								CTRSoundMgr._playSound(45);
								int num5 = Preferences._getIntForKey("PREFS_WALL_CLIMBER") + 1;
								Preferences._setIntforKey(num5, "PREFS_WALL_CLIMBER", comit: false);
								if (num5 >= 50)
								{
									CTRRootController.postAchievementName(NSObject.NSS("acNoviceWallClimber"));
								}
								if (num5 >= 400)
								{
									CTRRootController.postAchievementName(NSObject.NSS("acVeteranWallClimber"));
								}
							}
							grab.stickTimer = -1f;
						}
					}
				}
				if (grab.radius != -1f && grab.rope == null)
				{
					float num6 = MathHelper.vectDistance(MathHelper.vect(grab.x, grab.y), this.star.pos);
					if (num6 <= grab.radius + 15f)
					{
						Bungee bungee = (Bungee)new Bungee().initWithHeadAtXYTailAtTXTYandLength(null, grab.x, grab.y, this.star, this.star.pos.x, this.star.pos.y, grab.radius + 15f);
						bungee.bungeeAnchor.pin = bungee.bungeeAnchor.pos;
						grab.hideRadius = true;
						grab.setRope(bungee);
						CTRSoundMgr._playSound(31);
						if (activeRocket != null)
						{
							activeRocket.anglePercent = 0f;
							activeRocket.perpSetted = false;
							activeRocket.startRotation += activeRocket.additionalAngle;
							activeRocket.additionalAngle = 0f;
						}
					}
				}
				if (rope == null)
				{
					continue;
				}
				ConstraintedPoint bungeeAnchor = rope.bungeeAnchor;
				ConstraintedPoint constraintedPoint2 = rope.parts[rope.parts.Count - 1];
				Vector v3 = MathHelper.vectSub(bungeeAnchor.pos, constraintedPoint2.pos);
				bool flag3 = false;
				if (!noCandy && !flag2)
				{
					flag3 = true;
				}
				if (rope.relaxed != 0 && rope.cut == -1 && flag3)
				{
					float num7 = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v3));
					if (!rope.chosenOne)
					{
						rope.initialCandleAngle = candyMain.rotation - num7;
					}
					lastCandyRotateDelta = num7 + rope.initialCandleAngle - candyMain.rotation;
					recalcCandyRotateDelta(ref lastCandyRotateDelta);
					candyMain.rotation = num7 + rope.initialCandleAngle;
					flag2 = true;
					rope.chosenOne = true;
				}
				else
				{
					rope.chosenOne = false;
				}
			}
			if (!flag2 && !noCandy && !flag)
			{
				candyMain.rotation += lastCandyRotateDelta;
				lastCandyRotateDelta *= 0.98f;
			}
		}
		if (!noCandy && updatePhysics)
		{
			this.star.update(delta * ropePhysicsSpeed);
			candy.x = this.star.pos.x;
			candy.y = this.star.pos.y;
			candy.update(delta);
			BaseElement.calculateTopLeft(candy);
		}
		target.update(delta);
		targetex.update(delta);
		if (!updatePhysics)
		{
			return;
		}
		if (camera.type != 0 || !ignoreTouches)
		{
			int count2 = stars.Count;
			for (int m = 0; m < count2; m++)
			{
				Star star = stars[m];
				if (star == null)
				{
					continue;
				}
				star.update(delta);
				if ((double)star.timeout > 0.0 && (double)star.time == 0.0)
				{
					star.getTimeline(1).delegateTimelineDelegate = aniPool;
					aniPool.addChild(star);
					stars.Remove(star);
					star.timedAnim.playTimeline(1);
					star.playTimeline(1);
					break;
				}
				bool flag4 = false;
				if (GameObject.objectsIntersect(candy, star) && !noCandy)
				{
					candyBlink.playTimeline(1);
					starsCollected++;
					GameController.postFlurryLevelEvent("LEVSCR_STAR_COLLECTED");
					hudStar[starsCollected - 1].playTimeline(0);
					Animation animation = Animation.Animation_createWithResID(136);
					animation.doRestoreCutTransparency();
					animation.x = star.x;
					animation.y = star.y;
					stars.Remove(star);
					animation.anchor = 18;
					int n = animation.addAnimationDelayLoopFirstLast(0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 12);
					animation.getTimeline(n).delegateTimelineDelegate = aniPool;
					animation.playTimeline(0);
					aniPool.addChild(animation);
					CTRSoundMgr._playSound(32 + starsCollected - 1);
					if (target.getCurrentTimelineIndex() == 0)
					{
						playAnimation(5);
					}
					int totalStars = CTRPreferences.getTotalStars();
					if (totalStars >= 50 && totalStars < 100)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acStudent"));
					}
					else if (totalStars >= 100 && totalStars < 200)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acStudent"));
						CTRRootController.postAchievementName(NSObject.NSS("acAssistant"));
					}
					else if (totalStars >= 200)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acStudent"));
						CTRRootController.postAchievementName(NSObject.NSS("acAssistant"));
						CTRRootController.postAchievementName(NSObject.NSS("acDoctor"));
					}
					break;
				}
			}
		}
		int count3 = bubbles.Count;
		for (int num8 = 0; num8 < count3; num8++)
		{
			Bubble bubble = bubbles[num8];
			if (bubble == null)
			{
				continue;
			}
			bubble.update(delta);
			if (!noCandy && !bubble.popped && MathHelper.pointInRect(candy.x, candy.y, bubble.x - 30f, bubble.y - 30f, 60f, 60f))
			{
				if (candyBubble != null)
				{
					popBubbleAtXY(bubble.x, bubble.y);
				}
				candyBubble = bubble;
				candyBubbleAnimation.visible = true;
				CTRSoundMgr._playSound(21);
				bubble.popped = true;
				bubble.removeChildWithID(0);
				break;
			}
			if (hands == null)
			{
				continue;
			}
			foreach (MechanicalHand hand2 in hands)
			{
				if (hand2 != null && hand2.state == 1 && GameObject.objectsIntersect(candy, bubble) && candyBubble != null)
				{
					candyBubble = null;
					candyBubbleAnimation.visible = false;
					popBubbleAtXY(bubble.x, bubble.y);
				}
			}
		}
		count3 = tutorials.Count;
		for (int num9 = 0; num9 < count3; num9++)
		{
			tutorials[num9]?.update(delta);
		}
		count3 = tutorialImages.Count;
		for (int num10 = 0; num10 < count3; num10++)
		{
			tutorialImages[num10]?.update(delta);
		}
		count3 = tutorialImages2.Count;
		for (int num11 = 0; num11 < count3; num11++)
		{
			tutorialImages2[num11]?.update(delta);
		}
		count3 = pumps.Count;
		for (int num12 = 0; num12 < count3; num12++)
		{
			Pump pump = pumps[num12];
			if (pump != null)
			{
				pump.update(delta);
				if (Mover.moveVariableToTarget(ref pump.pumpTouchTimer, 0f, 1f, delta))
				{
					operatePump(pump);
				}
			}
		}
		int num13 = hands.Count() - 1;
		bool flag5 = false;
		if (hands != null)
		{
			foreach (MechanicalHand hand3 in hands)
			{
				if (hand3 == null)
				{
					continue;
				}
				hand3.update(delta);
				if (hand3.state == 1)
				{
					candy.drawX += hand3.cPoint.pos.x - this.star.pos.x;
					candy.drawY += hand3.cPoint.pos.y - this.star.pos.y;
					this.star.pos = hand3.cPoint.pos;
					if (hand3.doRotateCandy)
					{
						if (hand3.rotatingSegment != null)
						{
							candyMain.rotation += hand3.rotatingSegment.rotationDelta();
						}
					}
					else if (activeRocket != null)
					{
						hand3.isRotating();
						hand3.doRotateCandy = true;
					}
				}
				float num14 = MathHelper.vectDistance(hand3.cPoint.pos, this.star.pos);
				if (hands != null)
				{
					foreach (MechanicalHand hand4 in hands)
					{
						if (hand4 != null && hand4 != hand3 && hand4.state == 1)
						{
							num14 = MathHelper.vectDistance(hand3.cPoint.pos, hand4.cPoint.pos);
						}
					}
				}
				if (hand3.state == 0 && (double)num14 < 25.2 && !noCandy)
				{
					if (hands.Count() > 1 && hands != null)
					{
						foreach (MechanicalHand hand5 in hands)
						{
							if (hand5 != null && hand5 != hand3 && hand5.state == 1)
							{
								hand5.cPoint.removeConstraint(this.star);
								hand5.state = 2;
								flag5 = true;
								break;
							}
						}
					}
					hand3.cPoint.addConstraintwithRestLengthofType(this.star, 1f, Constraint.CONSTRAINT.CONSTRAINT_NOT_MORE_THAN);
					hand3.state = 1;
					num13 = hands.IndexOf(hand3);
					if (candyBubble != null)
					{
						candyBubble = null;
						candyBubbleAnimation.visible = false;
						Vector vector = hand3.clawPosition();
						popBubbleAtXY(vector.x, vector.y);
					}
					if (activeRocket != null)
					{
						int num15 = Preferences._getIntForKey("PREFS_GRAB_ROCKET") + 1;
						Preferences._setIntforKey(num15, "PREFS_GRAB_ROCKET", comit: false);
						if (num15 >= 50)
						{
							CTRRootController.postAchievementName(NSObject.NSS("acRoboMaster"));
						}
					}
					detachActiveSnails();
					List<BaseElement> list = new List<BaseElement>();
					list.Add(candy);
					list.Add(candyMain);
					list.Add(candyTop);
					hand3.animateCatchWithCandyPartsandAnimationsPool(list, aniPool);
					CTRSoundMgr._playSound(274);
				}
				if (hand3.state == 2 && num14 > 34f)
				{
					hand3.state = 0;
					CTRSoundMgr._playSound(275);
				}
			}
		}
		if (num13 != hands.Count() - 1 && flag5)
		{
			MechanicalHand mechanicalHand = hands[num13];
			hands.Remove(mechanicalHand);
			hands.Add(mechanicalHand);
			NSObject.NSREL(mechanicalHand);
		}
		if (rockets != null)
		{
			foreach (Rocket rocket in rockets)
			{
				if (rocket == null)
				{
					continue;
				}
				rocket.update(delta);
				rocket.updateRotation();
				float v4 = MathHelper.vectLength(MathHelper.vectSub(this.star.pos, rocket.point.pos));
				if (rocket.state == 2 || rocket.state == 1)
				{
					for (int num16 = 0; num16 < 30; num16++)
					{
						ConstraintedPoint.satisfyConstraints(this.star);
						ConstraintedPoint.satisfyConstraints(rocket.point);
					}
					rocket.rotation = MathHelper.angleTo0_360(rocket.startRotation + candyMain.rotation - rocket.startCandyRotation);
				}
				if (rocket.state == 2)
				{
					lastCandyRotateDelta = 0f;
					bool flag6 = false;
					if (bungees != null)
					{
						foreach (Grab bungee2 in bungees)
						{
							if (bungee2 != null)
							{
								Bungee rope2 = bungee2.rope;
								if (rope2 != null && rope2.tail == this.star && rope2.cut == -1 && rope2.relaxed > 0 && !flag)
								{
									flag6 = true;
									ConstraintedPoint bungeeAnchor2 = rope2.bungeeAnchor;
									ConstraintedPoint constraintedPoint3 = rope2.parts[rope2.parts.Count() - 1];
									Vector v5 = MathHelper.vectSub(bungeeAnchor2.pos, constraintedPoint3.pos);
									Vector v6 = MathHelper.vectPerp(v5);
									Vector v7 = MathHelper.vectRperp(v5);
									float fa = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v6) - MathHelper.DEGREES_TO_RADIANS(rocket.rotation));
									float fa2 = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v7) - MathHelper.DEGREES_TO_RADIANS(rocket.rotation));
									rocket.additionalAngle = MathHelper.angleTo0_360(rocket.additionalAngle);
									fa = nearestAngleTofrom(rocket.additionalAngle, fa);
									fa2 = nearestAngleTofrom(rocket.additionalAngle, fa2);
									float num17 = minAngleBetweenAandB(rocket.additionalAngle, fa);
									float num18 = minAngleBetweenAandB(rocket.additionalAngle, fa2);
									float t = ((num17 < num18) ? fa : fa2);
									Mover.moveVariableToTarget(ref rocket.additionalAngle, t, 90f, delta);
								}
							}
						}
					}
					rocket.rotation += rocket.additionalAngle;
					rocket.updateRotation();
					float angle = rocket.angle;
					Vector v8 = MathHelper.vectRotate(MathHelper.vect(-1.0, 0.0), angle);
					v8 = MathHelper.vectMult(v8, rocket.impulse);
					if (flag6)
					{
						v8 = MathHelper.vectMult(v8, rocket.impulseFactor);
					}
					this.star.applyImpulseDelta(v8, delta);
					this.star.gravity = MathHelper.vectZero;
					rocket.point.pos.x = this.star.pos.x;
					rocket.point.pos.y = this.star.pos.y;
					if (rocket.time != -1f && Mover.moveVariableToTarget(ref rocket.time, 0f, 1f, delta))
					{
						activeRocket = null;
						rocket.state = 3;
						this.star.disableGravity = false;
						rocket.stopAnimation();
					}
				}
				if (rocket.state == 1)
				{
					if (Mover.moveVariableToTarget(ref v4, 0f, 200f, delta))
					{
						rocket.state = 2;
					}
					else
					{
						rocket.point.changeRestLengthToFor(v4, this.star);
					}
				}
				if (rocket.state == 0 && GameObject.objectsIntersectRotatedWithUnrotated(rocket, candy) && !noCandy)
				{
					if (rocket.mover != null)
					{
						rocket.mover.pause();
					}
					rocket.startRotation = rocket.rotation;
					rocket.point.addConstraintwithRestLengthofType(this.star, v4, Constraint.CONSTRAINT.CONSTRAINT_NOT_MORE_THAN);
					rocket.state = 1;
					lastCandyRotateDelta = 0f;
					if (!this.star.disableGravity)
					{
						Vector v9 = MathHelper.vectSub(this.star.pos, this.star.prevPos);
						this.star.prevPos = MathHelper.vectAdd(this.star.prevPos, MathHelper.vectDiv(v9, 1.25f));
					}
					else
					{
						Vector v10 = MathHelper.vectSub(this.star.pos, this.star.prevPos);
						this.star.prevPos = MathHelper.vectAdd(this.star.prevPos, MathHelper.vectDiv(v10, 2f));
					}
					this.star.disableGravity = true;
					if (activeRocket != null)
					{
						activeRocket.state = 3;
						activeRocket.stopAnimation();
					}
					CTRSoundMgr._playSound(63);
					CTRSoundMgr._playSoundLooped(62);
					activeRocket = rocket;
					rocket.isOperating = -1;
					rocket.startCandyRotation = candyMain.rotation;
					Image image = Image.Image_createWithResID(154);
					image.doRestoreCutTransparency();
					RocketSparks rocketSparks = (RocketSparks)new RocketSparks().initWithTotalParticlesAngleandImageGrid(40, rocket.rotation, image);
					rocketSparks.particlesDelegate = particlesAniPool.particlesFinished;
					rocketSparks.x = rocket.x;
					rocketSparks.y = rocket.y;
					rocketSparks.startSystem(0);
					particlesAniPool.addChild(rocketSparks);
					rocket.particles = rocketSparks;
					RocketClouds rocketClouds = (RocketClouds)new RocketClouds().initWithTotalParticlesAngleandImageGrid(20, rocket.rotation, image);
					rocketClouds.particlesDelegate = particlesAniPool.particlesFinished;
					rocketClouds.x = rocket.x;
					rocketClouds.y = rocket.y;
					rocketClouds.startSystem(0);
					particlesAniPool.addChild(rocketClouds);
					rocket.cloudParticles = rocketClouds;
					rocket.startAnimation();
					int num19 = Preferences._getIntForKey("PREFS_ROCKETS") + 1;
					Preferences._setIntforKey(num19, "PREFS_ROCKETS", comit: false);
					if (num19 >= 100)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acPartyAnimal"));
					}
				}
			}
		}
		count3 = razors.Count;
		for (int num20 = 0; num20 < count3; num20++)
		{
			Razor razor = razors[num20];
			if (razor != null)
			{
				razor.update(delta);
				cutWithRazorOrLine1Line2Immediate(razor, MathHelper.vectZero, MathHelper.vectZero, im: false);
			}
		}
		count3 = this.spikes.Count;
		for (int num21 = 0; num21 < count3; num21++)
		{
			Spikes spikes = this.spikes[num21];
			if (spikes == null)
			{
				continue;
			}
			spikes.update(delta);
			if (spikes.electro && (!spikes.electro || !spikes.electroOn))
			{
				continue;
			}
			bool flag7 = false;
			bool left = false;
			if ((MathHelper.lineInRect(spikes.t1.x, spikes.t1.y, spikes.t2.x, spikes.t2.y, this.star.pos.x - 5f, this.star.pos.y - 5f, 10f, 10f) || MathHelper.lineInRect(spikes.b1.x, spikes.b1.y, spikes.b2.x, spikes.b2.y, this.star.pos.x - 5f, this.star.pos.y - 5f, 10f, 10f)) && !noCandy)
			{
				if (candyBubble != null)
				{
					popCandyBubble(left: false);
				}
				CTRPreferences cTRPreferences = Application.sharedPreferences();
				int intForKey = cTRPreferences.getIntForKey("PREFS_SELECTED_CANDY");
				Image image2 = Image.Image_createWithResID(CANDIES[intForKey]);
				image2.doRestoreCutTransparency();
				CandyBreak candyBreak = (CandyBreak)new CandyBreak().initWithTotalParticlesandImageGrid(5, image2);
				candyBreak.particlesDelegate = aniPool.particlesFinished;
				candyBreak.x = candy.x;
				candyBreak.y = candy.y;
				noCandy = true;
				if (activeRocket != null)
				{
					activeRocket.state = 3;
					activeRocket.stopAnimation();
				}
				candyBreak.startSystem(5);
				aniPool.addChild(candyBreak);
				CTRSoundMgr._playSound(22);
				releaseAllRopes(left);
				detachActiveHands();
				if (activeRocket != null)
				{
					activeRocket.state = 3;
					activeRocket.stopAnimation();
				}
				if (restartState != 0)
				{
					dd.callObjectSelectorParamafterDelay(selector_gameLost, null, 0.3f);
				}
				detachActiveSnails();
				return;
			}
		}
		count3 = bouncers.Count;
		for (int num22 = 0; num22 < count3; num22++)
		{
			Bouncer bouncer = bouncers[num22];
			if (bouncer == null)
			{
				continue;
			}
			bouncer.update(delta);
			bool flag8 = false;
			if ((MathHelper.lineInRect(bouncer.t1.x, bouncer.t1.y, bouncer.t2.x, bouncer.t2.y, this.star.pos.x - 20f, this.star.pos.y - 20f, 40f, 40f) || MathHelper.lineInRect(bouncer.b1.x, bouncer.b1.y, bouncer.b2.x, bouncer.b2.y, this.star.pos.x - 20f, this.star.pos.y - 20f, 40f, 40f)) && !noCandy)
			{
				handleBouncePtDelta(bouncer, this.star, delta);
				CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
				if (cTRRootController.getPack() + 1 == 1 && cTRRootController.getLevel() + 1 == 25)
				{
					bouncer.skip = false;
				}
			}
			else
			{
				bouncer.skip = false;
			}
		}
		float num23 = -18f;
		float num24 = 20f;
		float num25 = 0f - FrameworkTypes.SCREEN_HEIGHT;
		if (waterLevel > num25 && waterSpeed > 0f)
		{
			Mover.moveVariableToTarget(ref waterLevel, num25, waterSpeed, delta);
			waterLayer.y = mapHeight - waterLevel;
			waterLayer.height = ((waterLevel > 0f) ? ((int)waterLevel) : 0);
		}
		float num26 = 15f;
		if (waterLevel > 0f && this.star.pos.y > mapHeight - waterLevel && this.star.pos.x + num26 >= 0f && this.star.pos.x - num26 <= mapWidth)
		{
			float num27 = -25f / this.star.weight;
			if (activeRocket != null)
			{
				num27 /= 45f;
				num24 *= 15f;
				if (activeRocket.state == 2)
				{
					CTRSoundMgr._playSound(259);
					activeRocket.state = 3;
					activeRocket.stopAnimation();
				}
			}
			this.star.applyImpulseDelta(MathHelper.vect((0f - this.star.v.x) / num24, (0f - this.star.v.y) / num24 + num27), delta);
		}
		if (waterLayer != null && bungees != null)
		{
			foreach (Grab bungee3 in bungees)
			{
				if (bungee3 != null && bungee3.kickable && bungee3.kicked && bungee3.y > mapHeight - waterLevel && bungee3.rope != null)
				{
					float num28 = 20f;
					ConstraintedPoint bungeeAnchor3 = bungee3.rope.bungeeAnchor;
					float num29 = -20f;
					bungeeAnchor3.applyImpulseDelta(MathHelper.vect((0f - bungeeAnchor3.v.x) / num28, (0f - bungeeAnchor3.v.y) / num28 + num29), delta);
				}
			}
		}
		count3 = loads.Count();
		for (int num30 = count3 - 1; num30 >= 0; num30--)
		{
			Snail snail = loads[num30];
			snail.update(delta);
			if (snail.state == 1)
			{
				snail.rotation = candyMain.rotation - snail.startRotation;
			}
			if (snail.state == 0 && GameObject.objectsIntersect(candy, snail))
			{
				detachActiveSnails();
				snail.startRotation += candyMain.rotation;
				snail.attachToPoint(this.star);
				this.star.setWeight(this.star.weight + 3f);
			}
			if (snail.state == 3)
			{
				loads.RemoveAt(num30);
			}
		}
		if (candyBubble != null)
		{
			this.star.applyImpulseDelta(MathHelper.vect((0f - this.star.v.x) / num24, (0f - this.star.v.y) / num24 + num23), delta);
		}
		if (activeRocket != null)
		{
			Vector impulse = MathHelper.vect((0f - this.star.v.x) / num24, (0f - this.star.v.y) / num24);
			this.star.applyImpulseDelta(impulse, delta);
		}
		if (!noCandy)
		{
			if (!mouthOpen)
			{
				if (MathHelper.vectDistance(this.star.pos, MathHelper.vect(target.x, target.y)) < 100f)
				{
					stopTargetExAnims();
					mouthOpen = true;
					playAnimation(9);
					CTRSoundMgr._playSound(25);
					mouthCloseTimer = 1f;
				}
			}
			else if ((double)mouthCloseTimer > 0.0)
			{
				Mover.moveVariableToTarget(ref mouthCloseTimer, 0f, 1f, delta);
				if ((double)mouthCloseTimer <= 0.0)
				{
					if (MathHelper.vectDistance(this.star.pos, MathHelper.vect(target.x, target.y)) > 100f)
					{
						mouthOpen = false;
						playAnimation(10);
						CTRSoundMgr._playSound(24);
						tummyTeasers++;
					}
					else
					{
						mouthCloseTimer = 1f;
					}
				}
			}
			if (restartState != 0 && GameObject.objectsIntersect(candy, target))
			{
				GameController.postFlurryLevelEvent("LEVSCR_CANDY_EATEN");
				gameWon();
				return;
			}
		}
		bool flag9 = twoParts == 2 && pointOutOfScreen(this.star) && !noCandy;
		if (flag9)
		{
			bool flag10 = false;
			if (flag9)
			{
				noCandy = true;
			}
			if (restartState != 0)
			{
				if (!flag10)
				{
					gameLost();
				}
				return;
			}
		}
		if (special != 0 && special == 1 && !noCandy && candyBubble != null && candy.y < 150f)
		{
			special = 0;
			int count4 = tutorials.Count;
			for (int num31 = 0; num31 < count4; num31++)
			{
				TutorialText tutorialText = tutorials[num31];
				if (tutorialText != null && tutorialText.special == 1)
				{
					tutorialText.playTimeline(0);
				}
			}
			count4 = tutorialImages.Count;
			for (int num32 = 0; num32 < count4; num32++)
			{
				GameObjectSpecial gameObjectSpecial = tutorialImages[num32];
				if (gameObjectSpecial.special == 1)
				{
					gameObjectSpecial.playTimeline(0);
				}
			}
		}
		if (Mover.moveVariableToTarget(ref dimTime, 0f, 1f, delta))
		{
			if (restartState == 0)
			{
				restartState = 1;
				hide();
				show();
				dimTime = 0.15f;
			}
			else
			{
				restartState = -1;
			}
		}
	}

	public override void draw()
	{
		base.preDraw();
		OpenGL.glPushMatrix();
		OpenGL.glTranslatef(0f - FrameworkTypes.SCREEN_OFFSET_X, 0f - FrameworkTypes.SCREEN_OFFSET_Y, 0f);
		float num = FrameworkTypes.SCREEN_BG_SCALE_X;
		float num2 = FrameworkTypes.SCREEN_BG_SCALE_Y;
		if (mapHeight > FrameworkTypes.SCREEN_HEIGHT)
		{
			num2 = (FrameworkTypes.SCREEN_HEIGHT_EXPANDED - FrameworkTypes.SCREEN_OFFSET_Y) / FrameworkTypes.SCREEN_HEIGHT;
		}
		if (mapWidth > FrameworkTypes.SCREEN_WIDTH)
		{
			num = (FrameworkTypes.SCREEN_WIDTH_EXPANDED - FrameworkTypes.SCREEN_OFFSET_X) / FrameworkTypes.SCREEN_WIDTH;
		}
		OpenGL.glScalef(num, num2, 1f);
		OpenGL.glEnable(0);
		OpenGL.glDisable(1);
		Vector pos = camera.pos;
		pos.x /= num;
		pos.y /= num2;
		OpenGL.glTranslatef(0f - pos.x, 0f - pos.y, 0f);
		back.updateWithCameraPos(pos);
		back.draw();
		OpenGL.glEnable(1);
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		if (mapHeight > FrameworkTypes.SCREEN_HEIGHT)
		{
			float num3 = 2f;
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			int pack = cTRRootController.getPack();
			int textureResID = BGRS[pack];
			Texture2D texture = Application.getTexture(textureResID);
			int num4 = ((pack == 0) ? 1 : 2);
			float num5 = texture.quadOffsets[num4].y;
			Rectangle r = texture.quadRects[num4];
			r.y += num3;
			r.h -= num3 * 2f;
			GLDrawer.drawImagePart(texture, r, 0f, num5 + num3);
		}
		if (mapWidth > FrameworkTypes.SCREEN_WIDTH)
		{
			float num6 = 2f;
			CTRRootController cTRRootController2 = (CTRRootController)Application.sharedRootController();
			int pack2 = cTRRootController2.getPack();
			int textureResID2 = BGRS[pack2];
			Texture2D texture2 = Application.getTexture(textureResID2);
			float num7 = texture2.quadOffsets[1].x;
			Rectangle r2 = texture2.quadRects[1];
			r2.x += num6;
			r2.w -= num6 * 2f;
			GLDrawer.drawImagePart(texture2, r2, num7 + num6, 0f);
		}
		OpenGL.glPopMatrix();
		camera.applyCameraTransformation();
		OpenGL.SetWhiteColor();
		OpenGL.glEnable(0);
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		if (decalsLayer != null)
		{
			decalsLayer.draw();
		}
		support.draw();
		if (drawings != null)
		{
			foreach (Drawing drawing in drawings)
			{
				drawing?.draw();
			}
		}
		if (waterLayer != null)
		{
			waterLayer.drawBack();
		}
		if (target != null && target.visible)
		{
			target.draw();
		}
		if (targetex != null && targetex.visible)
		{
			targetex.draw();
		}
		if (tutorials != null)
		{
			for (int i = 0; i < tutorials.Count; i++)
			{
				tutorials[i]?.draw();
			}
		}
		if (tutorialImages != null)
		{
			for (int j = 0; j < tutorialImages.Count; j++)
			{
				tutorialImages[j]?.draw();
			}
		}
		if (razors != null)
		{
			for (int k = 0; k < razors.Count; k++)
			{
				razors[k]?.draw();
			}
		}
		if (bubbles != null)
		{
			for (int l = 0; l < bubbles.Count; l++)
			{
				bubbles[l]?.draw();
			}
		}
		if (pumps != null)
		{
			for (int m = 0; m < pumps.Count; m++)
			{
				pumps[m]?.draw();
			}
		}
		if (spikes != null)
		{
			for (int n = 0; n < spikes.Count; n++)
			{
				spikes[n]?.draw();
			}
		}
		if (bouncers != null)
		{
			for (int num8 = 0; num8 < bouncers.Count; num8++)
			{
				bouncers[num8]?.draw();
			}
		}
		MechanicalHand mechanicalHand = null;
		if (hands != null)
		{
			foreach (MechanicalHand hand in hands)
			{
				if (hand != null)
				{
					hand.draw();
					if (hand.state == 1)
					{
						mechanicalHand = hand;
					}
				}
			}
		}
		mechanicalHand?.theClaw().drawActiveHand();
		OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		if (bungees != null)
		{
			for (int num9 = 0; num9 < bungees.Count; num9++)
			{
				Grab grab = bungees[num9];
				if (grab != null && !grab.kickable)
				{
					grab.drawBack();
				}
			}
		}
		if (bungees != null)
		{
			for (int num10 = 0; num10 < bungees.Count; num10++)
			{
				Grab grab2 = bungees[num10];
				if (grab2 != null && !grab2.kickable)
				{
					grab2.draw();
				}
			}
		}
		if (bungees != null)
		{
			foreach (Grab bungee in bungees)
			{
				if (bungee != null && bungee.kickable)
				{
					bungee.drawBack();
					bungee.draw();
				}
			}
		}
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		particlesAniPool.draw();
		if (rockets != null)
		{
			foreach (Rocket rocket in rockets)
			{
				rocket?.draw();
			}
		}
		if (!noCandy && candy.visible)
		{
			candy.x = star.pos.x;
			candy.y = star.pos.y;
			candy.draw();
			Timeline timeline = candyBlink.getCurrentTimeline();
			if (timeline != null && timeline.state == Timeline.TimelineState.TIMELINE_PLAYING)
			{
				OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE);
				candyBlink.draw();
				OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			}
		}
		if (hands != null)
		{
			foreach (MechanicalHand hand2 in hands)
			{
				if (hand2 != null && hand2.state == 1)
				{
					hand2.theClaw().drawFingers();
				}
			}
		}
		if (loads != null)
		{
			foreach (Snail load in loads)
			{
				load?.draw();
			}
		}
		if (stars != null)
		{
			for (int num11 = 0; num11 < stars.Count; num11++)
			{
				stars[num11]?.draw();
			}
		}
		if (bungees != null)
		{
			foreach (Grab bungee2 in bungees)
			{
				if (bungee2 == null || !bungee2.gun)
				{
					continue;
				}
				if (!bungee2.gunFired)
				{
					Vector v = MathHelper.vectSub(MathHelper.vect(bungee2.x, bungee2.y), star.pos);
					bungee2.gunArrow.rotation = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v));
					continue;
				}
				int num12 = bungee2.gunCup.getCurrentTimelineIndex();
				if (num12 != 2)
				{
					bungee2.gunCup.x = star.pos.x;
					bungee2.gunCup.y = star.pos.y;
					bungee2.gunCup.rotation = bungee2.gunInitialRotation + candyMain.rotation - bungee2.gunCandyInitialRotation;
				}
				bungee2.drawGunCup();
			}
		}
		if (bungees != null)
		{
			for (int num13 = 0; num13 < bungees.Count; num13++)
			{
				Grab grab3 = bungees[num13];
				if (grab3 != null && grab3.hasSpider)
				{
					grab3.drawSpider();
				}
			}
		}
		if (tutorialImages2 != null)
		{
			foreach (GameObjectSpecial item in tutorialImages2)
			{
				item?.draw();
			}
		}
		if (waterLayer != null)
		{
			waterLayer.draw();
		}
		aniPool.draw();
		_ = nightLevel;
		OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		OpenGL.glDisable(0);
		OpenGL.SetWhiteColor();
		drawCuts();
		OpenGL.SetWhiteColor();
		OpenGL.glEnable(0);
		OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
		camera.cancelCameraTransformation();
		staticAniPool.draw();
		if (nightLevel)
		{
			OpenGL.glDisable(4);
		}
		base.postDraw();
	}

	public override void dealloc()
	{
		for (int i = 0; i < 5; i++)
		{
			fingerCuts[i].Clear();
		}
		NSObject.NSREL(decalsLayer);
		NSObject.NSREL(dd);
		NSObject.NSREL(camera);
		NSObject.NSREL(back);
		base.dealloc();
	}

	public virtual void popBubbleAtXY(float bx, float by)
	{
		CTRSoundMgr._playSound(20);
		Animation animation = Animation.Animation_createWithResID(138);
		animation.doRestoreCutTransparency();
		animation.x = bx;
		animation.y = by;
		animation.anchor = 18;
		int n = animation.addAnimationDelayLoopFirstLast(0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 11);
		animation.getTimeline(n).delegateTimelineDelegate = aniPool;
		animation.playTimeline(0);
		aniPool.addChild(animation);
	}

	public virtual void popCandyBubble(bool left)
	{
		candyBubble = null;
		candyBubbleAnimation.visible = false;
		popBubbleAtXY(candy.x, candy.y);
	}

	public virtual void loadNextMap()
	{
		dd.cancelAllDispatches();
		initialCameraToStarDistance = -1f;
		animateRestartDim = false;
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		if (cTRRootController.isPicker())
		{
			xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("mappicker://next"), NSObject.NSS("mappicker://next"), success: true);
			return;
		}
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		if (level < CTRPreferences.getLevelsInPackCount() - 1)
		{
			cTRRootController.setLevel(++level);
			cTRRootController.setMapName(LevelsList.LEVEL_NAMES[pack, level]);
			xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("maps/" + LevelsList.LEVEL_NAMES[pack, level].ToString()), NSObject.NSS("maps/" + LevelsList.LEVEL_NAMES[pack, level].ToString()), success: true);
		}
	}

	public virtual void reload()
	{
		dd.cancelAllDispatches();
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		if (cTRRootController.isPicker())
		{
			xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("mappicker://reload"), NSObject.NSS("mappicker://reload"), success: true);
			return;
		}
		int pack = cTRRootController.getPack();
		int level = cTRRootController.getLevel();
		xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("maps/" + LevelsList.LEVEL_NAMES[pack, level].ToString()), NSObject.NSS("maps/" + LevelsList.LEVEL_NAMES[pack, level].ToString()), success: true);
	}

	public virtual bool touchDownXYIndex(float tx, float ty, int ti)
	{
		if (drawingPoppedUp != null && drawingPoppedUp.onTouchDownXY(tx, ty))
		{
			return true;
		}
		if (ignoreTouches)
		{
			if (camera.type == CAMERA_TYPE.CAMERA_SPEED_PIXELS)
			{
				fastenCamera = true;
			}
			return true;
		}
		if (ti >= 5)
		{
			return true;
		}
		if (!updatePhysics)
		{
			return true;
		}
		Vector vector = MathHelper.vect(tx, ty);
		ref Vector reference = ref prevStartPos[ti];
		reference = (startPos[ti] = vector);
		Vector vector2 = MathHelper.vectAdd(vector, camera.pos);
		if (waterLayer != null)
		{
			waterLayer.addParticlesAtXY(vector2.x, vector2.y);
		}
		if (rockets != null)
		{
			foreach (Rocket rocket in rockets)
			{
				if (rocket != null && rocket.state == 0 && rocket.isRotatable && rocket.isOperating == -1 && MathHelper.vectLength(MathHelper.vectSub(vector2, MathHelper.vect(rocket.x, rocket.y))) < 60f)
				{
					rocket.handleTouch(vector2);
					rocket.isOperating = ti;
					return true;
				}
			}
		}
		if (MathHelper.pointInRect(tx + camera.pos.x, ty + camera.pos.y, star.pos.x - 30f, star.pos.y - 30f, 60f, 60f) && (double)star.weight > 1.0)
		{
			star.setWeight(star.weight - 3f);
			if ((double)star.weight == 1.0)
			{
				for (int i = 0; i < loads.Count(); i++)
				{
					Snail snail = loads[i];
					if (snail.state == 1)
					{
						snail.detach();
						break;
					}
				}
			}
			return true;
		}
		if (candyBubble != null && handleBubbleTouchXY(star, tx, ty))
		{
			return true;
		}
		int count = pumps.Count;
		for (int j = 0; j < count; j++)
		{
			Pump pump = pumps[j];
			if (GameObject.pointInObject(vector2, pump))
			{
				pump.pumpTouchTimer = 0.05f;
				pump.pumpTouch = ti;
				return true;
			}
		}
		int num = drawings.Count();
		for (int k = 0; k < num; k++)
		{
			Drawing drawing = drawings[k];
			if (GameObject.pointInObject(vector2, drawing))
			{
				drawingPoppedUp = drawing;
				drawing.showDrawing();
				break;
			}
		}
		bool flag = false;
		if (bungees != null)
		{
			foreach (Grab bungee2 in bungees)
			{
				if (bungee2 != null && bungee2.kickable && MathHelper.pointInRect(vector2.x, vector2.y, bungee2.x - 30f, bungee2.y - 30f, 60f, 60f))
				{
					if (!bungee2.kicked)
					{
						flag = true;
						break;
					}
					bungee2.kickActive = true;
				}
			}
		}
		if (bungees != null)
		{
			for (int l = 0; l < bungees.Count; l++)
			{
				Grab grab = bungees[l];
				if (grab != null && grab.wheel && MathHelper.pointInRect(vector2.x, vector2.y, grab.x - 45f, grab.y - 45f, 90f, 90f))
				{
					grab.handleWheelTouch(vector2);
					grab.wheelOperating = ti;
				}
				if ((double)grab.moveLength > 0.0 && MathHelper.pointInRect(vector2.x, vector2.y, grab.x - 30f, grab.y - 30f, 60f, 60f))
				{
					grab.moverDragging = ti;
					return true;
				}
				if (!grab.gun || grab.gunFired || noCandy || !updatePhysics)
				{
					continue;
				}
				bool flag2 = GameObject.rectInObject(0f, 0f, mapWidth, mapHeight, candy);
				if ((waterLayer == null || flag2 || mapHeight - waterLevel > star.pos.y) && MathHelper.pointInRect(vector2.x, vector2.y, grab.x - 35f, grab.y - 35f, 70f, 70f))
				{
					ConstraintedPoint constraintedPoint = star;
					Vector v = MathHelper.vectSub(MathHelper.vect(grab.x, grab.y), star.pos);
					grab.gunFired = true;
					grab.gunInitialRotation = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v)) + 90f;
					grab.gunCandyInitialRotation = candyMain.rotation;
					grab.gunCup.rotation = grab.gunInitialRotation;
					grab.gunFront.setDrawQuad(3);
					grab.gunCup.playTimeline(0);
					float a = MathHelper.vectDistance(MathHelper.vect(grab.x, grab.y), star.pos) - 30f;
					a = MathHelper.MAX(a, 30f);
					Bungee bungee = (Bungee)new Bungee().initWithHeadAtXYTailAtTXTYandLength(null, grab.x, grab.y, constraintedPoint, constraintedPoint.pos.x, constraintedPoint.pos.y, a);
					bungee.bungeeAnchor.pin = bungee.bungeeAnchor.pos;
					grab.setRope(bungee);
					CTRSoundMgr._playSound(46);
					int num2 = Preferences._getIntForKey("PREFS_ROPES_SHOOT") + 1;
					Preferences._setIntforKey(num2, "PREFS_ROPES_SHOOT", comit: false);
					if (num2 >= 50)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acRookieSniper"));
					}
					if (num2 >= 150)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acSkilledSniper"));
					}
					return true;
				}
			}
		}
		bool flag3 = false;
		if (hands != null)
		{
			foreach (MechanicalHand hand in hands)
			{
				if (hand == null)
				{
					continue;
				}
				if (MathHelper.vectDistance(vector2, hand.clawPosition()) < 17f && hand.state == 1)
				{
					hand.cPoint.removeConstraint(star);
					hand.state = 2;
					hand.doRotateCandy = false;
					hand.animateReleaseWithAnimationsPool(aniPool);
					return true;
				}
				for (int num3 = hand.segments.Count() - 1; num3 >= 0; num3--)
				{
					MechanicalHandSegment mechanicalHandSegment = hand.segmentAtIndex(num3);
					if (mechanicalHandSegment.button != null && mechanicalHandSegment.button.onTouchDownXY(vector2.x, vector2.y))
					{
						mechanicalHandSegment.rotate();
						hand.rotatingSegment = mechanicalHandSegment;
						flag3 = true;
						CTRSoundMgr._playSound(276);
						break;
					}
				}
			}
		}
		if (flag3)
		{
			return true;
		}
		if (bungees != null)
		{
			foreach (Grab bungee3 in bungees)
			{
				if (bungee3 != null && bungee3.kickable && bungee3.rope != null && !flag && bungee3.kicked)
				{
					bungee3.stickTimer = 0f;
				}
			}
		}
		Vector quadSize = Image.getQuadSize(149, 0);
		Vector quadOffset = Image.getQuadOffset(149, 0);
		if (MathHelper.pointInRect(vector2.x, vector2.y, target.drawX + quadOffset.x, target.drawY + quadOffset.y, quadSize.x, quadSize.y) && target.visible && target.getCurrentTimelineIndex() == 0)
		{
			startTargetExAnims();
			return true;
		}
		if (!dragging[ti])
		{
			dragging[ti] = true;
		}
		return true;
	}

	public virtual bool touchUpXYIndex(float tx, float ty, int ti)
	{
		if (drawingPoppedUp != null && drawingPoppedUp.onTouchUpXY(tx, ty))
		{
			return true;
		}
		if (ignoreTouches)
		{
			return true;
		}
		dragging[ti] = false;
		if (ti >= 5)
		{
			return true;
		}
		if (!updatePhysics)
		{
			return true;
		}
		if (rockets != null)
		{
			foreach (Rocket rocket in rockets)
			{
				if (rocket == null || rocket.isOperating != ti)
				{
					continue;
				}
				if (!rocket.rotateHandled)
				{
					Timeline timeline = rocket.getCurrentTimeline();
					if (timeline != null && timeline.state == Timeline.TimelineState.TIMELINE_PLAYING)
					{
						timeline.jumpToTrackKeyFrame(2, 1);
						timeline.stopTimeline();
					}
					rocket.playTimeline(0);
					rocket.startRotation += 45f;
				}
				else
				{
					Vector v = MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y);
					rocket.handleRotateFinal(v);
				}
				rocket.rotateHandled = false;
				rocket.isOperating = -1;
				return true;
			}
		}
		if (hands != null)
		{
			foreach (MechanicalHand hand in hands)
			{
				if (hand == null)
				{
					continue;
				}
				List<MechanicalHandSegment> segments = hand.segments;
				if (segments == null)
				{
					continue;
				}
				foreach (MechanicalHandSegment item in segments)
				{
					if (item != null && item.button != null)
					{
						item.button.onTouchUpXY(tx + camera.pos.x, ty + camera.pos.y);
					}
				}
			}
		}
		if (bungees != null)
		{
			for (int i = 0; i < bungees.Count; i++)
			{
				Grab grab = bungees[i];
				if (grab != null && grab.wheel && grab.wheelOperating == ti)
				{
					grab.wheelOperating = -1;
				}
				if ((double)grab.moveLength > 0.0 && grab.moverDragging == ti)
				{
					grab.moverDragging = -1;
				}
				if (!grab.kickable || grab.rope == null || !updatePhysics)
				{
					continue;
				}
				if (!grab.kickActive && !grab.kicked && grab.rope.cut == -1 && MathHelper.pointInRect(tx + camera.pos.x, ty + camera.pos.y, grab.x - 30f, grab.y - 30f, 60f, 60f))
				{
					if (grab.stainCounter > 0)
					{
						Image image = Image.Image_createWithResIDQuad(116, 0);
						image.doRestoreCutTransparency();
						image.x = grab.rope.bungeeAnchor.pos.x;
						image.y = grab.rope.bungeeAnchor.pos.y;
						image.anchor = 18;
						if (decalsLayer != null)
						{
							decalsLayer.addChild(image);
						}
						image.color.a = (float)grab.stainCounter / 10f;
						grab.stainCounter--;
					}
					grab.rope.bungeeAnchor.pin = MathHelper.vect(-1f, -1f);
					grab.rope.bungeeAnchor.setWeight(0.1f);
					grab.kicked = true;
					grab.stickTimer = -1f;
					grab.updateKickState();
					CTRSoundMgr._playSound(44);
					int num = Preferences._getIntForKey("PREFS_WALL_CLIMBER") + 1;
					Preferences._setIntforKey(num, "PREFS_WALL_CLIMBER", comit: false);
					if (num >= 50)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acNoviceWallClimber"));
					}
					if (num >= 400)
					{
						CTRRootController.postAchievementName(NSObject.NSS("acVeteranWallClimber"));
					}
				}
				grab.kickActive = false;
			}
		}
		return true;
	}

	public virtual bool touchMoveXYIndex(float tx, float ty, int ti)
	{
		if (drawingPoppedUp != null && drawingPoppedUp.onTouchMoveXY(tx, ty))
		{
			return true;
		}
		if (ignoreTouches)
		{
			return true;
		}
		Vector vector = MathHelper.vect(tx, ty);
		if (ti >= 5)
		{
			return true;
		}
		if (!updatePhysics)
		{
			return true;
		}
		slastTouch = vector;
		Vector vector2 = MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y);
		if (rockets != null)
		{
			foreach (Rocket rocket in rockets)
			{
				if (rocket != null && rocket.isOperating == ti)
				{
					rocket.handleRotate(vector2);
					return true;
				}
			}
		}
		if (pumps != null)
		{
			for (int i = 0; i < pumps.Count; i++)
			{
				Pump pump = pumps[i];
				if (pump != null && pump.pumpTouch == ti && (double)pump.pumpTouchTimer != 0.0 && (double)MathHelper.vectDistance(startPos[ti], vector) > 10.0)
				{
					pump.pumpTouchTimer = 0f;
				}
			}
		}
		if (hands != null)
		{
			foreach (MechanicalHand hand in hands)
			{
				if (hand == null)
				{
					continue;
				}
				List<MechanicalHandSegment> segments = hand.segments;
				if (segments == null)
				{
					continue;
				}
				foreach (MechanicalHandSegment item in segments)
				{
					if (item != null && item.button != null)
					{
						item.button.onTouchMoveXY(vector2.x, vector2.y);
					}
				}
			}
		}
		int count = bungees.Count;
		for (int j = 0; j < count; j++)
		{
			Grab grab = bungees[j];
			if (grab == null)
			{
				continue;
			}
			if (grab.wheel && grab.wheelOperating == ti)
			{
				grab.handleWheelRotate(vector2);
				return true;
			}
			if ((double)grab.moveLength > 0.0 && grab.moverDragging == ti)
			{
				if (grab.moveVertical)
				{
					grab.y = MathHelper.FIT_TO_BOUNDARIES(vector2.y, grab.minMoveValue, grab.maxMoveValue);
				}
				else
				{
					grab.x = MathHelper.FIT_TO_BOUNDARIES(vector2.x, grab.minMoveValue, grab.maxMoveValue);
				}
				if (grab.rope != null)
				{
					grab.rope.bungeeAnchor.pos = MathHelper.vect(grab.x, grab.y);
					grab.rope.bungeeAnchor.pin = grab.rope.bungeeAnchor.pos;
				}
				if (grab.radius != -1f)
				{
					grab.reCalcCircle();
				}
				return true;
			}
			if (grab.kickable && grab.kicked && grab.rope != null && MathHelper.vectLength(MathHelper.vectSub(startPos[ti], vector)) > 10f)
			{
				grab.stickTimer = -1f;
			}
		}
		if (dragging[ti])
		{
			Vector start = MathHelper.vectAdd(startPos[ti], camera.pos);
			Vector end = vector2;
			int num = 0;
			if (fingerCutCreation)
			{
				FingerCut fingerCut = new FingerCut();
				fingerCut.start = start;
				fingerCut.end = end;
				fingerCut.startSize = 5f;
				fingerCut.endSize = 5f;
				fingerCut.c = RGBAColor.whiteRGBA;
				fingerCuts[ti].Add(fingerCut);
				List<FingerCut> list = fingerCuts[ti];
				if (list != null)
				{
					for (int k = 0; k < list.Count; k++)
					{
						FingerCut fingerCut2 = list[k];
						if (fingerCut2 != null)
						{
							num += cutWithRazorOrLine1Line2Immediate(null, fingerCut2.start, fingerCut2.end, im: false);
						}
					}
				}
			}
			if (num > 0)
			{
				freezeCamera = false;
				if (ropesCutAtOnce > 0 && (double)ropeAtOnceTimer > 0.0)
				{
					ropesCutAtOnce += num;
				}
				else
				{
					ropesCutAtOnce = num;
				}
				ropeAtOnceTimer = 0.05f;
				int num2 = Preferences._getIntForKey("PREFS_ROPES_CUT") + 1;
				Preferences._setIntforKey(num2, "PREFS_ROPES_CUT", comit: false);
				if (num2 >= 50)
				{
					CTRRootController.postAchievementName(NSObject.NSS("acRopeCollector"));
				}
				if (num2 >= 600)
				{
					CTRRootController.postAchievementName(NSObject.NSS("acRopeExpert"));
				}
			}
			ref Vector reference = ref prevStartPos[ti];
			reference = startPos[ti];
			startPos[ti] = vector;
		}
		return true;
	}

	public virtual void restart()
	{
		hide();
		show();
	}

	public virtual void spiderBusted(Grab g)
	{
		CTRSoundMgr._playSound(40);
		g.hasSpider = false;
		Image image = Image.Image_createWithResIDQuad(113, 11);
		image.doRestoreCutTransparency();
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, g.spider.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, (double)g.spider.y - 50.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
		timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, g.spider.y + FrameworkTypes.SCREEN_HEIGHT, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
		timeline.addKeyFrame(KeyFrame.makeRotation(0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
		timeline.addKeyFrame(KeyFrame.makeRotation(MathHelper.RND_RANGE(-120, 120), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1f));
		image.addTimelinewithID(timeline, 0);
		image.playTimeline(0);
		image.x = g.spider.x;
		image.y = g.spider.y;
		image.anchor = 18;
		timeline.delegateTimelineDelegate = aniPool;
		aniPool.addChild(image);
	}

	public virtual void spiderWon(Grab sg)
	{
		CTRSoundMgr._playSound(41);
		int count = bungees.Count;
		for (int i = 0; i < count; i++)
		{
			Grab grab = bungees[i];
			Bungee rope = grab.rope;
			if (rope != null && rope.tail == star)
			{
				if (rope.cut == -1)
				{
					rope.setCut(rope.parts.Count - 2);
					rope.forceWhite = false;
				}
				if (grab.hasSpider && grab.spiderActive && sg != grab)
				{
					spiderBusted(grab);
				}
			}
			if (grab.gun && RGBAColor.RGBAEqual(RGBAColor.solidOpaqueRGBA, grab.gunCup.color))
			{
				grab.gunCup.playTimeline(2);
			}
		}
		sg.hasSpider = false;
		spiderTookCandy = true;
		noCandy = true;
		Image image = Image.Image_createWithResIDQuad(113, 12);
		image.doRestoreCutTransparency();
		candy.anchor = (candy.parentAnchor = 18);
		candy.x = 0f;
		candy.y = -5f;
		image.addChild(candy);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
		timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, (double)sg.spider.y - 10.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, (double)sg.spider.y - 70.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
		timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, sg.spider.y + FrameworkTypes.SCREEN_HEIGHT, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
		image.addTimelinewithID(timeline, 0);
		image.playTimeline(0);
		image.x = sg.spider.x;
		image.y = sg.spider.y - 10f;
		image.anchor = 18;
		timeline.delegateTimelineDelegate = aniPool;
		aniPool.addChild(image);
		if (activeRocket != null)
		{
			activeRocket.state = 3;
			activeRocket.stopAnimation();
		}
		detachActiveSnails();
		if (restartState != 0)
		{
			dd.callObjectSelectorParamafterDelay(selector_gameLost, null, 2f);
		}
	}

	public virtual void operatePump(Pump p)
	{
		p.playTimeline(0);
		CTRSoundMgr._playSound(MathHelper.RND_RANGE(35, 38));
		Image grid = Image.Image_createWithResID(152);
		PumpDirt pumpDirt = new PumpDirt().initWithTotalParticlesAngleandImageGrid(5, MathHelper.RADIANS_TO_DEGREES((float)p.angle) - 90f, grid);
		pumpDirt.particlesDelegate = aniPool.particlesFinished;
		Vector v = MathHelper.vect(p.x + 25f, p.y);
		v = MathHelper.vectRotateAround(v, p.angle - Math.PI / 2.0, p.x, p.y);
		pumpDirt.x = v.x;
		pumpDirt.y = v.y;
		pumpDirt.startSystem(5);
		aniPool.addChild(pumpDirt);
		if (!noCandy)
		{
			handlePumpFlowPtSkin(p, star, candy);
		}
		if (bungees == null)
		{
			return;
		}
		foreach (Grab bungee in bungees)
		{
			if (bungee != null && bungee.rope != null && bungee.kickable && bungee.kicked)
			{
				handlePumpFlowPtSkin(p, bungee.rope.bungeeAnchor, bungee);
			}
		}
	}

	public virtual int cutWithRazorOrLine1Line2Immediate(Razor r, Vector v1, Vector v2, bool im)
	{
		int num = 0;
		for (int i = 0; i < bungees.Count; i++)
		{
			Grab grab = bungees[i];
			Bungee rope = grab.rope;
			if (rope == null || rope.cut != -1)
			{
				continue;
			}
			for (int j = 0; j < rope.parts.Count - 1; j++)
			{
				ConstraintedPoint constraintedPoint = rope.parts[j];
				ConstraintedPoint constraintedPoint2 = rope.parts[j + 1];
				bool flag = false;
				if (r == null)
				{
					flag = (!grab.wheel || !MathHelper.lineInRect(v1.x, v1.y, v2.x, v2.y, grab.x - 30f, grab.y - 30f, 60f, 60f)) && (!grab.gun || !MathHelper.lineInRect(v1.x, v1.y, v2.x, v2.y, grab.x - 15f, grab.y - 15f, 30f, 30f)) && (!grab.kickable || !MathHelper.lineInRect(v1.x, v1.y, v2.x, v2.y, grab.x - 15f, grab.y - 15f, 30f, 30f)) && lineInLine(v1.x, v1.y, v2.x, v2.y, constraintedPoint.pos.x, constraintedPoint.pos.y, constraintedPoint2.pos.x, constraintedPoint2.pos.y);
				}
				else if (constraintedPoint.prevPos.x != 2.1474836E+09f)
				{
					float x1l = minOf4(constraintedPoint.pos.x, constraintedPoint.prevPos.x, constraintedPoint2.pos.x, constraintedPoint2.prevPos.x);
					float y1t = minOf4(constraintedPoint.pos.y, constraintedPoint.prevPos.y, constraintedPoint2.pos.y, constraintedPoint2.prevPos.y);
					float x1r = maxOf4(constraintedPoint.pos.x, constraintedPoint.prevPos.x, constraintedPoint2.pos.x, constraintedPoint2.prevPos.x);
					float y1b = maxOf4(constraintedPoint.pos.y, constraintedPoint.prevPos.y, constraintedPoint2.pos.y, constraintedPoint2.prevPos.y);
					flag = MathHelper.rectInRect(x1l, y1t, x1r, y1b, r.drawX, r.drawY, r.drawX + (float)r.width, r.drawY + (float)r.height);
				}
				if (flag)
				{
					num++;
					if (grab.hasSpider && grab.spiderActive)
					{
						spiderBusted(grab);
					}
					GameController.postFlurryLevelEvent("LEVSCR_ROPE_CUT");
					CTRSoundMgr._playSound(27 + rope.relaxed);
					rope.setCut(j);
					if (im)
					{
						rope.cutTime = 0f;
						rope.removePart(j);
					}
					if (grab.gun)
					{
						grab.gunCup.playTimeline(1);
					}
					break;
				}
			}
		}
		return num;
	}

	public virtual void detachActiveHands()
	{
		if (hands == null)
		{
			return;
		}
		foreach (MechanicalHand hand in hands)
		{
			if (hand != null && hand.state == 1)
			{
				hand.cPoint.removeConstraint(star);
				hand.state = 2;
				hand.doRotateCandy = false;
				hand.animateReleaseWithAnimationsPool(aniPool);
			}
		}
	}

	public virtual void gameWon()
	{
		dd.cancelAllDispatches();
		stopTargetExAnims();
		if (activeRocket != null)
		{
			activeRocket.state = 3;
			activeRocket.stopAnimation();
		}
		detachActiveSnails();
		detachActiveHands();
		playAnimation(8);
		CTRSoundMgr._playSound(23);
		if (candyBubble != null)
		{
			popCandyBubble(left: false);
		}
		noCandy = true;
		candy.passTransformationsToChilds = true;
		candyMain.scaleX = (candyMain.scaleY = 1f);
		candyTop.scaleX = (candyTop.scaleY = 1f);
		Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
		timeline.addKeyFrame(KeyFrame.makePos(candy.x, candy.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makePos(target.x, (double)target.y + 10.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
		timeline.addKeyFrame(KeyFrame.makeScale(0.71, 0.71, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeScale(0.0, 0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
		timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
		candy.addTimelinewithID(timeline, 0);
		candy.playTimeline(0);
		timeline.delegateTimelineDelegate = aniPool;
		aniPool.addChild(candy);
		dd.callObjectSelectorParamafterDelay(selector_gameWonDelegate, null, 2f);
		calculateScore();
		releaseAllRopes(left: false);
		int num = Preferences._getIntForKey("PREFS_CANDIES_WON") + 1;
		Preferences._setIntforKey(num, "PREFS_CANDIES_WON", comit: false);
		if (num >= 150)
		{
			CTRRootController.postAchievementName(NSObject.NSS("acTummyDelight"));
		}
	}

	public virtual void gameLost()
	{
		dd.cancelAllDispatches();
		stopTargetExAnims();
		playAnimation(7);
		CTRSoundMgr._playSound(26);
		dd.callObjectSelectorParamafterDelay(selector_animateLevelRestart, null, 1f);
		gameSceneDelegate_gameLost();
	}

	public virtual void releaseAllRopes(bool left)
	{
		int count = bungees.Count;
		for (int i = 0; i < count; i++)
		{
			Grab grab = bungees[i];
			if (grab == null)
			{
				continue;
			}
			Bungee rope = grab.rope;
			if (rope != null && rope.tail == star)
			{
				if (rope.cut == -1)
				{
					rope.setCut(rope.parts.Count - 2);
				}
				else
				{
					rope.hideTailParts = true;
				}
				if (grab.hasSpider && grab.spiderActive)
				{
					spiderBusted(grab);
				}
				if (grab.gun && RGBAColor.RGBAEqual(RGBAColor.solidOpaqueRGBA, grab.gunCup.color))
				{
					grab.gunCup.playTimeline(2);
				}
			}
		}
	}

	public virtual void calculateScore()
	{
		timeBonus = (int)Math.Max(0f, 30f - time) * 100;
		timeBonus /= 10;
		timeBonus *= 10;
		starBonus = 1000 * starsCollected;
		score = timeBonus + starBonus;
	}

	public virtual void doCandyBlink()
	{
		candyBlink.playTimeline(0);
	}

	public virtual void startCamera()
	{
		if (mapWidth > FrameworkTypes.SCREEN_WIDTH || mapHeight > FrameworkTypes.SCREEN_HEIGHT)
		{
			ignoreTouches = true;
			fastenCamera = false;
			camera.type = CAMERA_TYPE.CAMERA_SPEED_PIXELS;
			camera.speed = 10f;
			cameraMoveMode = 0;
			ConstraintedPoint constraintedPoint = star;
			float num;
			float num2;
			if (mapWidth > FrameworkTypes.SCREEN_WIDTH)
			{
				if ((double)constraintedPoint.pos.x > (double)mapWidth / 2.0)
				{
					num = 0f;
					num2 = 0f;
				}
				else
				{
					num = mapWidth - FrameworkTypes.SCREEN_WIDTH;
					num2 = 0f;
				}
			}
			else if ((double)constraintedPoint.pos.y > (double)mapHeight / 2.0)
			{
				num = 0f;
				num2 = 0f;
			}
			else
			{
				num = 0f;
				num2 = mapHeight - FrameworkTypes.SCREEN_HEIGHT;
			}
			float v = constraintedPoint.pos.x - FrameworkTypes.SCREEN_WIDTH / 2f;
			float v2 = constraintedPoint.pos.y - FrameworkTypes.SCREEN_HEIGHT / 2f;
			float num3 = MathHelper.FIT_TO_BOUNDARIES(v, 0f, mapWidth - FrameworkTypes.SCREEN_WIDTH);
			float num4 = MathHelper.FIT_TO_BOUNDARIES(v2, 0f, mapHeight - FrameworkTypes.SCREEN_HEIGHT);
			camera.moveToXYImmediate(num, num2, immediate: true);
			initialCameraToStarDistance = MathHelper.vectDistance(camera.pos, MathHelper.vect(num3, num4));
		}
		else
		{
			ignoreTouches = false;
			camera.moveToXYImmediate(0f, 0f, immediate: true);
		}
	}

	public virtual void drawCuts()
	{
		for (int i = 0; i < 5; i++)
		{
			int count = fingerCuts[i].Count;
			if (count <= 0)
			{
				continue;
			}
			float num = 6f;
			float num2 = 1f;
			int num3 = 0;
			int j = 0;
			FingerCut fingerCut = null;
			Vector[] array = new Vector[count + 1];
			int num4 = 0;
			for (; j < count; j++)
			{
				fingerCut = fingerCuts[i][j];
				if (j == 0)
				{
					ref Vector reference = ref array[num4++];
					reference = fingerCut.start;
				}
				ref Vector reference2 = ref array[num4++];
				reference2 = fingerCut.end;
			}
			int num5 = count * 2;
			float[] array2 = new float[num5 * 2];
			float num6 = 1f / (float)num5;
			float num7 = 0f;
			int num8 = 0;
			while (true)
			{
				if ((double)num7 > 1.0)
				{
					num7 = 1f;
				}
				Vector vector = GLDrawer.calcPathBezier(array, count + 1, num7);
				if (num8 > array2.Count() - 2)
				{
					break;
				}
				array2[num8++] = vector.x;
				array2[num8++] = vector.y;
				if ((double)num7 == 1.0)
				{
					break;
				}
				num7 += num6;
			}
			float num9 = num / (float)num5;
			float[] array3 = new float[num5 * 4];
			for (int k = 0; k < num5 - 1; k++)
			{
				float s = num2;
				float s2 = ((k == num5 - 2) ? 1f : (num2 + num9));
				Vector vector2 = MathHelper.vect(array2[k * 2], array2[k * 2 + 1]);
				Vector v = MathHelper.vect(array2[(k + 1) * 2], array2[(k + 1) * 2 + 1]);
				Vector v2 = MathHelper.vectSub(v, vector2);
				Vector v3 = MathHelper.vectNormalize(v2);
				Vector v4 = MathHelper.vectRperp(v3);
				Vector v5 = MathHelper.vectPerp(v3);
				if (num3 == 0)
				{
					Vector vector3 = MathHelper.vectAdd(vector2, MathHelper.vectMult(v4, s));
					Vector vector4 = MathHelper.vectAdd(vector2, MathHelper.vectMult(v5, s));
					array3[num3++] = vector4.x;
					array3[num3++] = vector4.y;
					array3[num3++] = vector3.x;
					array3[num3++] = vector3.y;
				}
				Vector vector5 = MathHelper.vectAdd(v, MathHelper.vectMult(v4, s2));
				Vector vector6 = MathHelper.vectAdd(v, MathHelper.vectMult(v5, s2));
				array3[num3++] = vector6.x;
				array3[num3++] = vector6.y;
				array3[num3++] = vector5.x;
				array3[num3++] = vector5.y;
				num2 += num9;
			}
			OpenGL.SetWhiteColor();
			OpenGL.glVertexPointer(2, 5, 0, array3);
			OpenGL.glDrawArrays(8, 0, num3 / 2);
		}
	}

	public virtual void showGreeting()
	{
		playAnimation(12);
	}

	public virtual bool shouldShowHandAnimation()
	{
		CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
		if (twoParts == 2 && cTRRootController.getLevel() == 0 && restartState == -1)
		{
			return !cTRRootController.isPicker();
		}
		return false;
	}

	public virtual bool shouldSkipTutorialElement(XMLNode c)
	{
		NSString @string = Application.sharedAppSettings().getString(8);
		NSString nSString = c["locale"];
		if (@string.isEqualToString(NSObject.NSS("en")) || @string.isEqualToString(NSObject.NSS("ru")) || @string.isEqualToString(NSObject.NSS("de")) || @string.isEqualToString(NSObject.NSS("fr")) || @string.isEqualToString(NSObject.NSS("zh")) || @string.isEqualToString(NSObject.NSS("ja")) || @string.isEqualToString(NSObject.NSS("ko")) || @string.isEqualToString(NSObject.NSS("es")) || @string.isEqualToString(NSObject.NSS("it")) || @string.isEqualToString(NSObject.NSS("br")) || @string.isEqualToString(NSObject.NSS("nl")))
		{
			if (!nSString.isEqualToString(@string))
			{
				return true;
			}
		}
		else if (!nSString.isEqualToString(NSObject.NSS("en")))
		{
			return true;
		}
		return false;
	}

	public virtual void onButtonPressed(int n)
	{
		if (n != 1 || spikes == null)
		{
			return;
		}
		foreach (Spikes spike in spikes)
		{
			if (spike != null && spike.toggled != 0)
			{
				spike.rotateSpikes();
			}
		}
	}

	public virtual void handlePumpFlowPtSkin(Pump p, ConstraintedPoint s, GameObject c)
	{
		float num = 200f;
		if (GameObject.rectInObject(p.x - num, p.y - num, p.x + num, p.y + num, c))
		{
			Vector v = MathHelper.vect(c.x, c.y);
			Vector vector = default(Vector);
			vector.x = p.x - p.bb.w / 2f;
			Vector vector2 = default(Vector);
			vector2.x = p.x + p.bb.w / 2f;
			vector.y = (vector2.y = p.y);
			if (p.angle != 0.0)
			{
				v = MathHelper.vectRotateAround(v, 0.0 - p.angle, p.x, p.y);
			}
			if (v.y < vector.y && MathHelper.rectInRect(v.x - c.bb.w / 2f, v.y - c.bb.h / 2f, v.x + c.bb.w / 2f, v.y + c.bb.h / 2f, vector.x, vector.y - num, vector2.x, vector2.y))
			{
				float num2 = num * 2f;
				float num3 = num2 * (num - (vector.y - v.y)) / num;
				Vector v2 = MathHelper.vect(0f, 0f - num3);
				v2 = MathHelper.vectRotate(v2, p.angle);
				s.applyImpulseDelta(v2, 0.016f);
			}
		}
	}

	public virtual void handleBouncePtDelta(Bouncer b, ConstraintedPoint s, float delta)
	{
		if (!b.skip)
		{
			b.skip = true;
			Vector v = MathHelper.vectSub(s.prevPos, s.pos);
			int num = ((!(MathHelper.vectRotateAround(s.prevPos, 0f - b.angle, b.x, b.y).y < b.y)) ? 1 : (-1));
			float s2 = Math.Max(MathHelper.vectLength(v) * 40f, 300f) * (float)num;
			Vector v2 = MathHelper.vectForAngle(b.angle);
			Vector impulse = MathHelper.vectMult(MathHelper.vectPerp(v2), s2);
			s.pos = MathHelper.vectRotateAround(s.pos, 0f - b.angle, b.x, b.y);
			s.prevPos = MathHelper.vectRotateAround(s.prevPos, 0f - b.angle, b.x, b.y);
			s.prevPos.y = s.pos.y;
			s.pos = MathHelper.vectRotateAround(s.pos, b.angle, b.x, b.y);
			s.prevPos = MathHelper.vectRotateAround(s.prevPos, b.angle, b.x, b.y);
			s.applyImpulseDelta(impulse, delta);
			b.playTimeline(0);
			CTRSoundMgr._playSound(43);
			int num2 = Preferences._getIntForKey("PREFS_ROPES_JUMPS") + 1;
			Preferences._setIntforKey(num2, "PREFS_ROPES_JUMPS", comit: false);
			if (num2 >= 100)
			{
				CTRRootController.postAchievementName(NSObject.NSS("acHighJumper"));
			}
			if (num2 >= 400)
			{
				CTRRootController.postAchievementName(NSObject.NSS("acCrazyJumper"));
			}
		}
	}

	public virtual bool handleBubbleTouchXY(ConstraintedPoint s, float tx, float ty)
	{
		if (MathHelper.pointInRect(tx + camera.pos.x, ty + camera.pos.y, s.pos.x - 30f, s.pos.y - 30f, 60f, 60f))
		{
			popCandyBubble(left: false);
			return true;
		}
		return false;
	}

	public virtual bool pointOutOfScreen(ConstraintedPoint p)
	{
		if (!((double)p.pos.y > (double)mapHeight + 100.0) && !((double)p.pos.y < -100.0) && !(p.pos.x < 0f - FrameworkTypes.SCREEN_WIDTH))
		{
			return p.pos.x > mapWidth + FrameworkTypes.SCREEN_WIDTH;
		}
		return true;
	}

	public virtual void startTargetExAnims()
	{
		playAnimation(4);
	}

	public virtual void stopTargetExAnims()
	{
		target.visible = true;
		targetex.visible = false;
		if (targetex.getCurrentTimeline() != null)
		{
			targetex.stopCurrentTimeline();
		}
	}

	public virtual void drawDrawing()
	{
		if (drawings == null)
		{
			return;
		}
		foreach (Drawing drawing in drawings)
		{
			if (drawing != null && drawing.drawingVisible)
			{
				drawing.drawDrawing();
			}
		}
	}

	public virtual void drawingHidden(Drawing d)
	{
		drawingPoppedUp = null;
		NSObject.NSRET(d);
		drawings.Remove(d);
		CTRPreferences.setDrawingUnlocked(d.dg, 1);
		d = null;
		CTRPreferences.increaseNewDrawingsCounter();
		int num = 0;
		for (int i = 0; i <= 7; i++)
		{
			if (CTRPreferences.getDrawingUnlocked(i) != 0)
			{
				num++;
			}
		}
		if (num >= 4)
		{
			CTRRootController.postAchievementName(NSObject.NSS("acPhotoObserver"));
		}
	}

	public virtual void drawingShowing(Drawing d)
	{
	}

	public virtual void exhausted(Rocket r)
	{
		if (activeRocket == r)
		{
			activeRocket = null;
			star.disableGravity = false;
		}
	}

	public virtual void detachActiveSnails()
	{
		if (loads.Count() <= 0)
		{
			return;
		}
		for (int num = loads.Count() - 1; num >= 0; num--)
		{
			Snail snail = loads[num];
			if (snail.state == 1)
			{
				snail.detach();
			}
		}
	}

	private void selector_gameLost(NSObject param)
	{
		gameLost();
	}

	private void selector_gameWonDelegate(NSObject param)
	{
		CTRSoundMgr.EnableLoopedSounds(bEnable: false);
		gameSceneDelegate_gameWon();
	}

	private void selector_animateLevelRestart(NSObject param)
	{
		animateLevelRestart();
	}

	private void selector_showGreeting(NSObject param)
	{
		showGreeting();
	}

	private void selector_doCandyBlink(NSObject param)
	{
		doCandyBlink();
	}

	private void selector_teleport(NSObject param)
	{
	}

	private void drawCut(Vector fls, Vector frs, Vector start, Vector end, float startSize, float endSize, RGBAColor c, ref Vector le, ref Vector re)
	{
		Vector v = MathHelper.vectSub(end, start);
		Vector v2 = MathHelper.vectNormalize(v);
		Vector v3 = MathHelper.vectRperp(v2);
		Vector v4 = MathHelper.vectPerp(v2);
		Vector vector = (MathHelper.vectEqual(frs, MathHelper.vectUndefined) ? MathHelper.vectAdd(start, MathHelper.vectMult(v3, startSize)) : frs);
		Vector vector2 = (MathHelper.vectEqual(fls, MathHelper.vectUndefined) ? MathHelper.vectAdd(start, MathHelper.vectMult(v4, startSize)) : fls);
		Vector vector3 = MathHelper.vectAdd(end, MathHelper.vectMult(v3, endSize));
		Vector vector4 = MathHelper.vectAdd(end, MathHelper.vectMult(v4, endSize));
		float[] vertices = new float[8] { vector2.x, vector2.y, vector.x, vector.y, vector3.x, vector3.y, vector4.x, vector4.y };
		GLDrawer.drawSolidPolygonWOBorder(vertices, 4, c);
		le = vector4;
		re = vector3;
	}

	private float maxOf4(float v1, float v2, float v3, float v4)
	{
		if (v1 >= v2 && v1 >= v3 && v1 >= v4)
		{
			return v1;
		}
		if (v2 >= v1 && v2 >= v3 && v2 >= v4)
		{
			return v2;
		}
		if (v3 >= v2 && v3 >= v1 && v3 >= v4)
		{
			return v3;
		}
		if (v4 >= v2 && v4 >= v3 && v4 >= v1)
		{
			return v4;
		}
		return -1f;
	}

	private float minOf4(float v1, float v2, float v3, float v4)
	{
		if (v1 <= v2 && v1 <= v3 && v1 <= v4)
		{
			return v1;
		}
		if (v2 <= v1 && v2 <= v3 && v2 <= v4)
		{
			return v2;
		}
		if (v3 <= v2 && v3 <= v1 && v3 <= v4)
		{
			return v3;
		}
		if (v4 <= v2 && v4 <= v3 && v4 <= v1)
		{
			return v4;
		}
		return -1f;
	}

	private void recalcCandyRotateDelta(ref float delta)
	{
		while (Math.Abs(delta - 360f * (delta / Math.Abs(delta))) < Math.Abs(delta))
		{
			delta -= 360f * (delta / Math.Abs(delta));
		}
	}

	private float nearestAngleTofrom(float ta, float fa)
	{
		float num = fa - 360f;
		float num2 = fa + 360f;
		if (Math.Abs(fa - ta) < Math.Abs(num - ta) && Math.Abs(fa - ta) < Math.Abs(num2 - ta))
		{
			return fa;
		}
		if (Math.Abs(num - ta) < Math.Abs(num2 - ta))
		{
			return num;
		}
		return nearestAngleTofrom(ta, num2);
	}

	private float minAngleBetweenAandB(float a, float b)
	{
		float num;
		for (num = Math.Abs(a - b); num > 360f; num -= 360f)
		{
		}
		num = Math.Abs(num);
		if (num > 180f)
		{
			num -= 360f;
		}
		return Math.Abs(num);
	}
}
