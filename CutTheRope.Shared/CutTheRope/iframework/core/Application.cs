using CutTheRope.ctr_original;
using CutTheRope.game;
using CutTheRope.iframework.media;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.utils;

namespace CutTheRope.iframework.core;

internal class Application : NSObject
{
	private static CTRPreferences prefs;

	private static CTRResourceMgr resourceMgr;

	protected static RootController root;

	private static ApplicationSettings appSettings;

	private static GLCanvas canvas;

	private static SoundMgr soundMgr;

	private static MovieMgr movieMgr;

	public static CTRPreferences sharedPreferences()
	{
		return prefs;
	}

	public static CTRResourceMgr sharedResourceMgr()
	{
		return resourceMgr;
	}

	public static RootController sharedRootController()
	{
		if (root == null)
		{
			root = (CTRRootController)new CTRRootController().initWithParent(null);
		}
		return root;
	}

	public static ApplicationSettings sharedAppSettings()
	{
		return appSettings;
	}

	public static GLCanvas sharedCanvas()
	{
		return canvas;
	}

	public static SoundMgr sharedSoundMgr()
	{
		if (soundMgr == null)
		{
			soundMgr = new SoundMgr().init();
		}
		return soundMgr;
	}

	public static MovieMgr sharedMovieMgr()
	{
		if (movieMgr == null)
		{
			movieMgr = new MovieMgr();
		}
		return movieMgr;
	}

	public virtual ApplicationSettings createAppSettings()
	{
		return (ApplicationSettings)new ApplicationSettings().init();
	}

	public virtual GLCanvas createCanvas()
	{
		return new GLCanvas().initWithFrame(new Rectangle(0f, 0f, FrameworkTypes.SCREEN_WIDTH, FrameworkTypes.SCREEN_HEIGHT));
	}

	public virtual CTRResourceMgr createResourceMgr()
	{
		return (CTRResourceMgr)new CTRResourceMgr().init();
	}

	public virtual SoundMgr createSoundMgr()
	{
		return new SoundMgr().init();
	}

	public virtual CTRPreferences createPreferences()
	{
		return (CTRPreferences)new CTRPreferences().init();
	}

	public virtual RootController createRootController()
	{
		return (CTRRootController)new CTRRootController().initWithParent(null);
	}

	public virtual void applicationDidFinishLaunching(UIApplication application)
	{
		appSettings = createAppSettings();
		canvas = createCanvas();
		resourceMgr = createResourceMgr();
		root = createRootController();
		soundMgr = createSoundMgr();
		prefs = createPreferences();
		canvas.touchDelegate = root;
		canvas.show();
		root.activate();
	}

	internal static FontGeneric getFont(int fontResID)
	{
		/*
		if ((fontResID == 5 || fontResID == 6) && (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO || ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA))
		{
			return new FontWP7(fontResID);
		}
		*/
		return (FontGeneric)sharedResourceMgr().loadResource(fontResID, ResourceMgr.ResourceType.FONT);
	}

	internal static FontGeneric getFontEN(int fontResID)
	{
		return (FontGeneric)sharedResourceMgr().loadResource(fontResID, ResourceMgr.ResourceType.FONT);
	}

	internal static Texture2D getTexture(int textureResID)
	{
		return (Texture2D)sharedResourceMgr().loadResource(textureResID, ResourceMgr.ResourceType.IMAGE);
	}

	internal static NSString getString(int strResID)
	{
		return (NSString)sharedResourceMgr().loadResource(strResID, ResourceMgr.ResourceType.STRINGS);
	}
}
