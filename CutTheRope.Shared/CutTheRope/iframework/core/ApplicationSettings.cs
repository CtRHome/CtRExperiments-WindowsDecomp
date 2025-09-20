using System;
using CutTheRope.ctr_original;
using CutTheRope.ios;

namespace CutTheRope.iframework.core;

internal class ApplicationSettings : NSObject
{
	public enum AppSettings
	{
		APP_SETTING_INTERACTION_ENABLED,
		APP_SETTING_MULTITOUCH_ENABLED,
		APP_SETTING_STATUSBAR_HIDDEN,
		APP_SETTING_MAIN_LOOP_TIMERED,
		APP_SETTING_FPS_METER_ENABLED,
		APP_SETTING_FPS,
		APP_SETTING_ORIENTATION,
		APP_SETTING_LOCALIZATION_ENABLED,
		APP_SETTING_LOCALE,
		APP_SETTING_RETINA_SUPPORT,
		APP_SETTINGS_CUSTOM
	}

	public virtual int getInt(int s)
	{
		if (s == 5)
		{
			return 50;
		}
		throw new NotImplementedException();
	}

	public virtual NSString getString(int s)
	{
		if (s == 8)
		{
			return ResDataPhoneFullExperiments.LANGUAGE switch
			{
				Language.LANG_EN => NSObject.NSS("en"), 
				Language.LANG_RU => NSObject.NSS("ru"), 
				Language.LANG_DE => NSObject.NSS("de"), 
				Language.LANG_FR => NSObject.NSS("fr"), 
				Language.LANG_IT => NSObject.NSS("it"), 
				Language.LANG_ES => NSObject.NSS("es"), 
				Language.LANG_NL => NSObject.NSS("nl"), 
				Language.LANG_BR => NSObject.NSS("br"), 
				Language.LANG_KO => NSObject.NSS("ko"), 
				Language.LANG_ZH => NSObject.NSS("zh"), 
				Language.LANG_JA => NSObject.NSS("ja"), 
				_ => NSObject.NSS("en"), 
			};
		}
		return NSObject.NSS("");
	}
}
