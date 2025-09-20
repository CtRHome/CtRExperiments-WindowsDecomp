using System.Collections.Generic;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.ctr_original;

internal class CTRResourceMgr : ResourceMgr
{
	private static Dictionary<int, string> resNames_;

	public override NSObject init()
	{
		base.init();
		return this;
	}

	public static int handleResource(int r)
	{
		return handleLocalizedResource(handleWvgaResource(r));
	}

	public override float getNormalScaleX(int r)
	{
		return 1f;
	}

	public override float getNormalScaleY(int r)
	{
		return 1f;
	}

	public override float getWvgaScaleX(int r)
	{
		return 1.5f;
	}

	public override float getWvgaScaleY(int r)
	{
		switch (r)
		{
		case 95:
		case 96:
		case 189:
		case 191:
		case 193:
		case 195:
		case 266:
		case 269:
		case 313:
			return 1.6666666f;
		default:
			return 1.5f;
		}
	}

	public override bool isWvgaResource(int r)
	{
		if (!FrameworkTypes.IS_WVGA)
		{
			return false;
		}
		switch (r)
		{
		default:
			switch (r)
			{
			case 77:
			case 83:
			case 84:
			case 85:
			case 86:
			case 90:
			case 95:
			case 96:
			case 97:
			case 98:
			case 99:
			case 100:
			case 101:
			case 102:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
			case 108:
			case 109:
			case 110:
			case 123:
			case 124:
			case 125:
			case 126:
			case 127:
			case 128:
			case 129:
			case 130:
			case 131:
			case 132:
			case 133:
			case 134:
			case 155:
			case 159:
			case 160:
			case 161:
			case 162:
			case 163:
			case 164:
			case 165:
			case 166:
			case 167:
			case 168:
			case 169:
			case 170:
			case 171:
			case 172:
			case 173:
			case 174:
			case 175:
			case 176:
			case 177:
			case 178:
			case 179:
			case 180:
			case 189:
			case 190:
			case 191:
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 214:
			case 215:
			case 216:
			case 217:
			case 218:
			case 219:
			case 220:
			case 221:
			case 222:
			case 223:
			case 224:
			case 225:
			case 226:
			case 227:
			case 235:
			case 236:
			case 237:
			case 238:
			case 239:
			case 240:
			case 241:
			case 247:
			case 248:
			case 249:
			case 250:
			case 251:
			case 253:
			case 255:
			case 261:
			case 263:
			case 266:
			case 267:
			case 269:
			case 271:
			case 273:
			case 289:
			case 290:
			case 291:
			case 292:
			case 293:
			case 294:
			case 295:
			case 296:
			case 297:
			case 298:
			case 299:
			case 300:
			case 301:
			case 306:
			case 307:
			case 308:
			case 309:
			case 311:
			case 313:
				break;
			default:
				goto end_IL_000e;
			}
			goto case 2;
		case 2:
		case 3:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
			return true;
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
			break;
			end_IL_000e:
			break;
		}
		return false;
	}

	public static int handleWvgaResource(int r)
	{
		if (!FrameworkTypes.IS_WVGA)
		{
			return r;
		}
		return r switch
		{
			5 => 12, 
			6 => 13, 
			119 => 131, 
			181 => 189, 
			183 => 191, 
			185 => 193, 
			187 => 195, 
			264 => 266, 
			268 => 269, 
			91 => 107, 
			92 => 108, 
			93 => 109, 
			94 => 110, 
			149 => 159, 
			150 => 160, 
			114 => 128, 
			0 => 2, 
			244 => 249, 
			10 => 17, 
			228 => 235, 
			229 => 236, 
			230 => 237, 
			231 => 238, 
			232 => 239, 
			233 => 240, 
			234 => 241, 
			8 => 15, 
			243 => 248, 
			122 => 134, 
			254 => 255, 
			111 => 123, 
			121 => 132, 
			283 => 295, 
			211 => 219, 
			213 => 222, 
			282 => 294, 
			210 => 220, 
			212 => 221, 
			145 => 161, 
			1 => 3, 
			67 => 95, 
			68 => 96, 
			69 => 97, 
			71 => 99, 
			4 => 11, 
			9 => 16, 
			245 => 250, 
			242 => 247, 
			78 => 104, 
			79 => 83, 
			80 => 84, 
			81 => 85, 
			82 => 86, 
			75 => 103, 
			76 => 77, 
			281 => 293, 
			87 => 105, 
			278 => 290, 
			202 => 214, 
			201 => 215, 
			279 => 291, 
			204 => 218, 
			277 => 289, 
			280 => 292, 
			200 => 216, 
			203 => 217, 
			73 => 101, 
			7 => 14, 
			252 => 253, 
			72 => 100, 
			74 => 102, 
			117 => 124, 
			70 => 98, 
			118 => 125, 
			288 => 300, 
			120 => 133, 
			285 => 297, 
			207 => 223, 
			206 => 224, 
			286 => 298, 
			209 => 227, 
			284 => 296, 
			287 => 299, 
			205 => 225, 
			208 => 226, 
			88 => 106, 
			89 => 90, 
			156 => 178, 
			157 => 179, 
			135 => 174, 
			154 => 155, 
			262 => 263, 
			260 => 261, 
			141 => 162, 
			137 => 163, 
			138 => 164, 
			112 => 126, 
			116 => 130, 
			115 => 129, 
			142 => 165, 
			143 => 166, 
			139 => 167, 
			151 => 168, 
			158 => 180, 
			152 => 169, 
			113 => 127, 
			148 => 170, 
			147 => 171, 
			146 => 172, 
			140 => 173, 
			136 => 175, 
			144 => 176, 
			246 => 251, 
			182 => 190, 
			184 => 192, 
			186 => 194, 
			188 => 196, 
			265 => 267, 
			270 => 271, 
			153 => 177, 
			272 => 273, 
			310 => 311, 
			312 => 313, 
			_ => r, 
		};
	}

	public static int handleLocalizedResource(int r)
	{
		switch (r)
		{
		case 87:
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_RU)
			{
				return 200;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_DE)
			{
				return 201;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR)
			{
				return 202;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH)
			{
				return 203;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				return 204;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
			{
				return 277;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ES)
			{
				return 278;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_IT)
			{
				return 279;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_NL)
			{
				return 280;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_BR)
			{
				return 281;
			}
			break;
		case 121:
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_RU)
			{
				return 210;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_DE)
			{
				return 211;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR)
			{
				return 121;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH)
			{
				return 212;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				return 213;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
			{
				return 282;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ES)
			{
				return 283;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_IT)
			{
				return 121;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_NL)
			{
				return 121;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_BR)
			{
				return 121;
			}
			break;
		case 120:
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_RU)
			{
				return 205;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_DE)
			{
				return 206;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR)
			{
				return 207;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH)
			{
				return 208;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				return 209;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
			{
				return 284;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ES)
			{
				return 285;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_IT)
			{
				return 286;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_NL)
			{
				return 287;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_BR)
			{
				return 288;
			}
			break;
		case 105:
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_RU)
			{
				return 216;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_DE)
			{
				return 215;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR)
			{
				return 214;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH)
			{
				return 217;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				return 218;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
			{
				return 289;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ES)
			{
				return 290;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_IT)
			{
				return 291;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_NL)
			{
				return 292;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_BR)
			{
				return 293;
			}
			break;
		case 132:
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_RU)
			{
				return 220;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_DE)
			{
				return 219;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR)
			{
				return 132;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH)
			{
				return 221;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				return 222;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
			{
				return 294;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ES)
			{
				return 295;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_IT)
			{
				return 132;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_NL)
			{
				return 132;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_BR)
			{
				return 132;
			}
			break;
		case 133:
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_RU)
			{
				return 225;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_DE)
			{
				return 224;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_FR)
			{
				return 223;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ZH)
			{
				return 226;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_JA)
			{
				return 227;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
			{
				return 296;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_ES)
			{
				return 297;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_IT)
			{
				return 298;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_NL)
			{
				return 299;
			}
			if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_BR)
			{
				return 300;
			}
			break;
		}
		return r;
	}

	public static string XNA_ResName(int resId)
	{
		if (resNames_ == null)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(0, "zeptolab");
			dictionary.Add(1, "loaderbar_full");
			dictionary.Add(2, "zeptolab_hd");
			dictionary.Add(3, "loaderbar_full_hd");
			dictionary.Add(4, "menu_button_default");
			dictionary.Add(5, "big_font");
			dictionary.Add(6, "small_font");
			dictionary.Add(7, "menu_loading");
			dictionary.Add(8, "drawing_canvas");
			dictionary.Add(9, "menu_button_short");
			dictionary.Add(10, "drawings_particles");
			dictionary.Add(11, "menu_button_default_hd");
			dictionary.Add(12, "big_font_hd");
			dictionary.Add(13, "small_font_hd");
			dictionary.Add(14, "menu_loading_hd");
			dictionary.Add(15, "drawing_canvas_hd");
			dictionary.Add(16, "menu_button_short_hd");
			dictionary.Add(17, "drawings_particles_hd");
			dictionary.Add(18, "menu_strings.xml");
			dictionary.Add(19, "tap");
			dictionary.Add(20, "bubble_break");
			dictionary.Add(21, "bubble");
			dictionary.Add(22, "candy_break");
			dictionary.Add(23, "monster_chewing");
			dictionary.Add(24, "monster_close");
			dictionary.Add(25, "monster_open");
			dictionary.Add(26, "monster_sad");
			dictionary.Add(27, "rope_bleak_1");
			dictionary.Add(28, "rope_bleak_2");
			dictionary.Add(29, "rope_bleak_3");
			dictionary.Add(30, "rope_bleak_4");
			dictionary.Add(31, "rope_get");
			dictionary.Add(32, "star_1");
			dictionary.Add(33, "star_2");
			dictionary.Add(34, "star_3");
			dictionary.Add(35, "pump_1");
			dictionary.Add(36, "pump_2");
			dictionary.Add(37, "pump_3");
			dictionary.Add(38, "pump_4");
			dictionary.Add(39, "spider_activate");
			dictionary.Add(40, "spider_fall");
			dictionary.Add(41, "spider_win");
			dictionary.Add(42, "win");
			dictionary.Add(43, "bouncer");
			dictionary.Add(44, "sucker_drop");
			dictionary.Add(45, "sucker_land");
			dictionary.Add(46, "gun");
			dictionary.Add(47, "voice_fail_01");
			dictionary.Add(48, "voice_fail_02");
			dictionary.Add(49, "voice_star_00a");
			dictionary.Add(50, "voice_star_00b");
			dictionary.Add(51, "voice_star_01a");
			dictionary.Add(52, "voice_star_01b");
			dictionary.Add(53, "voice_star_02a");
			dictionary.Add(54, "voice_star_02b");
			dictionary.Add(55, "voice_star_02c");
			dictionary.Add(56, "voice_star_03a");
			dictionary.Add(57, "voice_star_03b");
			dictionary.Add(58, "voice_star_03c");
			dictionary.Add(59, "voice_start_01");
			dictionary.Add(60, "voice_start_02");
			dictionary.Add(61, "voice_start_03");
			dictionary.Add(62, "rocket_fly");
			dictionary.Add(63, "rocket_start");
			dictionary.Add(64, "voice_star_03d");
			dictionary.Add(65, "voice_star_03e");
			dictionary.Add(66, "voice_star_03f");
			dictionary.Add(67, "menu_bgr");
			dictionary.Add(68, "menu_main_bgr");
			dictionary.Add(69, "menu_button_crystal");
			dictionary.Add(70, "menu_popup");
			dictionary.Add(71, "menu_button_crystal_icon");
			dictionary.Add(72, "menu_logo");
			dictionary.Add(73, "menu_level_selection");
			dictionary.Add(74, "menu_pack_selection");
			dictionary.Add(75, "menu_extra_buttons");
			dictionary.Add(76, "menu_extra_buttons_2");
			dictionary.Add(77, "menu_extra_buttons_2_hd");
			dictionary.Add(78, "menu_button_short_2");
			dictionary.Add(79, "menu_button_achiv_icon");
			dictionary.Add(80, "menu_button_album_icon");
			dictionary.Add(81, "menu_button_cup_icon");
			dictionary.Add(82, "menu_button_options_icon");
			dictionary.Add(83, "menu_button_achiv_icon_hd");
			dictionary.Add(84, "menu_button_album_icon_hd");
			dictionary.Add(85, "menu_button_cup_icon_hd");
			dictionary.Add(86, "menu_button_options_icon_hd");
			dictionary.Add(87, "menu_extra_buttons_en");
			dictionary.Add(88, "menu_shadow");
			dictionary.Add(89, "menu_audio_icons");
			dictionary.Add(90, "menu_audio_icons_hd");
			dictionary.Add(91, "menu_processing");
			dictionary.Add(92, "menu_promo");
			dictionary.Add(93, "menu_promo_banner");
			dictionary.Add(94, "menu_promo_button");
			dictionary.Add(95, "menu_bgr_hd");
			dictionary.Add(96, "menu_main_bgr_hd");
			dictionary.Add(97, "menu_button_crystal_hd");
			dictionary.Add(98, "menu_popup_hd");
			dictionary.Add(99, "menu_button_crystal_icon_hd");
			dictionary.Add(100, "menu_logo_hd");
			dictionary.Add(101, "menu_level_selection_hd");
			dictionary.Add(102, "menu_pack_selection_hd");
			dictionary.Add(103, "menu_extra_buttons_hd");
			dictionary.Add(104, "menu_button_short_2_hd");
			dictionary.Add(105, "menu_extra_buttons_en_hd");
			dictionary.Add(106, "menu_shadow_hd");
			dictionary.Add(107, "menu_processing_hd");
			dictionary.Add(108, "menu_promo_hd");
			dictionary.Add(109, "menu_promo_banner_hd");
			dictionary.Add(110, "menu_promo_button_hd");
			dictionary.Add(111, "hud_buttons");
			dictionary.Add(112, "obj_candy_01");
			dictionary.Add(113, "obj_spider");
			dictionary.Add(114, "confetti_particles");
			dictionary.Add(115, "obj_gun");
			dictionary.Add(116, "obj_sticker");
			dictionary.Add(117, "menu_pause");
			dictionary.Add(118, "menu_result");
			dictionary.Add(119, "font_numbers_big");
			dictionary.Add(120, "menu_result_en");
			dictionary.Add(121, "hud_buttons_en");
			dictionary.Add(122, "drawing_hidden");
			dictionary.Add(123, "hud_buttons_hd");
			dictionary.Add(124, "menu_pause_hd");
			dictionary.Add(125, "menu_result_hd");
			dictionary.Add(126, "obj_candy_01_hd");
			dictionary.Add(127, "obj_spider_hd");
			dictionary.Add(128, "confetti_particles_hd");
			dictionary.Add(129, "obj_gun_hd");
			dictionary.Add(130, "obj_sticker_hd");
			dictionary.Add(131, "font_numbers_big_hd");
			dictionary.Add(132, "hud_buttons_en_hd");
			dictionary.Add(133, "menu_result_en_hd");
			dictionary.Add(134, "drawing_hidden_hd");
			dictionary.Add(135, "hand");
			dictionary.Add(136, "obj_star_disappear");
			dictionary.Add(137, "obj_bubble_flight");
			dictionary.Add(138, "obj_bubble_pop");
			dictionary.Add(139, "obj_hook_auto");
			dictionary.Add(140, "obj_spikes_04");
			dictionary.Add(141, "obj_bubble_attached");
			dictionary.Add(142, "obj_hook_01");
			dictionary.Add(143, "obj_hook_02");
			dictionary.Add(144, "obj_star_idle");
			dictionary.Add(145, "hud_star");
			dictionary.Add(146, "obj_spikes_03");
			dictionary.Add(147, "obj_spikes_02");
			dictionary.Add(148, "obj_spikes_01");
			dictionary.Add(149, "char_animations");
			dictionary.Add(150, "char_animations2");
			dictionary.Add(151, "obj_hook_movable");
			dictionary.Add(152, "obj_pump");
			dictionary.Add(153, "tutorial_signs");
			dictionary.Add(154, "obj_rocket");
			dictionary.Add(155, "obj_rocket_hd");
			dictionary.Add(156, "obj_bouncer_01");
			dictionary.Add(157, "obj_bouncer_02");
			dictionary.Add(158, "obj_pollen");
			dictionary.Add(159, "char_animations_hd");
			dictionary.Add(160, "char_animations2_hd");
			dictionary.Add(161, "hud_star_hd");
			dictionary.Add(162, "obj_bubble_attached_hd");
			dictionary.Add(163, "obj_bubble_flight_hd");
			dictionary.Add(164, "obj_bubble_pop_hd");
			dictionary.Add(165, "obj_hook_01_hd");
			dictionary.Add(166, "obj_hook_02_hd");
			dictionary.Add(167, "obj_hook_auto_hd");
			dictionary.Add(168, "obj_hook_movable_hd");
			dictionary.Add(169, "obj_pump_hd");
			dictionary.Add(170, "obj_spikes_01_hd");
			dictionary.Add(171, "obj_spikes_02_hd");
			dictionary.Add(172, "obj_spikes_03_hd");
			dictionary.Add(173, "obj_spikes_04_hd");
			dictionary.Add(174, "hand_hd");
			dictionary.Add(175, "obj_star_disappear_hd");
			dictionary.Add(176, "obj_star_idle_hd");
			dictionary.Add(177, "tutorial_signs_hd");
			dictionary.Add(178, "obj_bouncer_01_hd");
			dictionary.Add(179, "obj_bouncer_02_hd");
			dictionary.Add(180, "obj_pollen_hd");
			dictionary.Add(181, "bgr_01");
			dictionary.Add(182, "char_support_01");
			dictionary.Add(183, "bgr_02");
			dictionary.Add(184, "char_support_02");
			dictionary.Add(185, "bgr_03");
			dictionary.Add(186, "char_support_03");
			dictionary.Add(187, "bgr_04");
			dictionary.Add(188, "char_support_04");
			dictionary.Add(189, "bgr_01_hd");
			dictionary.Add(190, "char_support_01_hd");
			dictionary.Add(191, "bgr_02_hd");
			dictionary.Add(192, "char_support_02_hd");
			dictionary.Add(193, "bgr_03_hd");
			dictionary.Add(194, "char_support_03_hd");
			dictionary.Add(195, "bgr_04_hd");
			dictionary.Add(196, "char_support_04_hd");
			dictionary.Add(197, "menu_music");
			dictionary.Add(198, "game_music");
			dictionary.Add(199, "game_music2");
			dictionary.Add(200, "menu_extra_buttons_ru");
			dictionary.Add(201, "menu_extra_buttons_gr");
			dictionary.Add(202, "menu_extra_buttons_fr");
			dictionary.Add(203, "menu_extra_buttons_zh");
			dictionary.Add(204, "menu_extra_buttons_ja");
			dictionary.Add(277, "menu_extra_buttons_ko");
			dictionary.Add(278, "menu_extra_buttons_es");
			dictionary.Add(279, "menu_extra_buttons_it");
			dictionary.Add(280, "menu_extra_buttons_nl");
			dictionary.Add(281, "menu_extra_buttons_br");
			dictionary.Add(205, "menu_result_ru");
			dictionary.Add(206, "menu_result_gr");
			dictionary.Add(207, "menu_result_fr");
			dictionary.Add(208, "menu_result_zh");
			dictionary.Add(209, "menu_result_ja");
			dictionary.Add(284, "menu_result_ko");
			dictionary.Add(285, "menu_result_es");
			dictionary.Add(286, "menu_result_it");
			dictionary.Add(287, "menu_result_nl");
			dictionary.Add(288, "menu_result_br");
			dictionary.Add(210, "hud_buttons_ru");
			dictionary.Add(211, "hud_buttons_gr");
			dictionary.Add(212, "hud_buttons_zh");
			dictionary.Add(213, "hud_buttons_ja");
			dictionary.Add(282, "hud_buttons_ko");
			dictionary.Add(283, "hud_buttons_es");
			dictionary.Add(214, "menu_extra_buttons_fr_hd");
			dictionary.Add(215, "menu_extra_buttons_gr_hd");
			dictionary.Add(216, "menu_extra_buttons_ru_hd");
			dictionary.Add(217, "menu_extra_buttons_zh_hd");
			dictionary.Add(218, "menu_extra_buttons_ja_hd");
			dictionary.Add(289, "menu_extra_buttons_ko_hd");
			dictionary.Add(290, "menu_extra_buttons_es_hd");
			dictionary.Add(291, "menu_extra_buttons_it_hd");
			dictionary.Add(292, "menu_extra_buttons_nl_hd");
			dictionary.Add(293, "menu_extra_buttons_br_hd");
			dictionary.Add(219, "hud_buttons_gr_hd");
			dictionary.Add(220, "hud_buttons_ru_hd");
			dictionary.Add(221, "hud_buttons_zh_hd");
			dictionary.Add(222, "hud_buttons_ja_hd");
			dictionary.Add(294, "hud_buttons_ko_hd");
			dictionary.Add(295, "hud_buttons_es_hd");
			dictionary.Add(223, "menu_result_fr_hd");
			dictionary.Add(224, "menu_result_gr_hd");
			dictionary.Add(225, "menu_result_ru_hd");
			dictionary.Add(226, "menu_result_zh_hd");
			dictionary.Add(227, "menu_result_ja_hd");
			dictionary.Add(296, "menu_result_ko_hd");
			dictionary.Add(297, "menu_result_es_hd");
			dictionary.Add(298, "menu_result_it_hd");
			dictionary.Add(299, "menu_result_nl_hd");
			dictionary.Add(300, "menu_result_br_hd");
			dictionary.Add(228, "drawing_01");
			dictionary.Add(229, "drawing_02");
			dictionary.Add(230, "drawing_03");
			dictionary.Add(231, "drawing_04");
			dictionary.Add(232, "drawing_05");
			dictionary.Add(233, "drawing_06");
			dictionary.Add(234, "drawing_08");
			dictionary.Add(235, "drawing_01_hd");
			dictionary.Add(236, "drawing_02_hd");
			dictionary.Add(237, "drawing_03_hd");
			dictionary.Add(238, "drawing_04_hd");
			dictionary.Add(239, "drawing_05_hd");
			dictionary.Add(240, "drawing_06_hd");
			dictionary.Add(241, "drawing_08_hd");
			dictionary.Add(242, "menu_drawings_thumb_page");
			dictionary.Add(243, "drawing_facebook");
			dictionary.Add(244, "drawings_menu_markers");
			dictionary.Add(245, "menu_drawings_bgr");
			dictionary.Add(246, "omnom_artist");
			dictionary.Add(247, "menu_drawings_thumb_page_hd");
			dictionary.Add(248, "drawing_facebook_hd");
			dictionary.Add(249, "drawings_menu_markers_hd");
			dictionary.Add(250, "menu_drawings_bgr_hd");
			dictionary.Add(251, "omnom_artist_hd");
			dictionary.Add(252, "menu_loading_bgr");
			dictionary.Add(253, "menu_loading_bgr_hd");
			dictionary.Add(254, "drawings_thumbnails");
			dictionary.Add(255, "drawings_thumbnails_hd");
			dictionary.Add(256, "snail_in");
			dictionary.Add(257, "snail_out");
			dictionary.Add(258, "water_splash");
			dictionary.Add(259, "rocket_in_water");
			dictionary.Add(260, "water_tile");
			dictionary.Add(261, "water_tile_hd");
			dictionary.Add(262, "obj_snail");
			dictionary.Add(263, "obj_snail_hd");
			dictionary.Add(264, "bgr_05");
			dictionary.Add(265, "char_support_05");
			dictionary.Add(266, "bgr_05_hd");
			dictionary.Add(267, "char_support_05_hd");
			dictionary.Add(268, "bgr_06");
			dictionary.Add(269, "bgr_06_hd");
			dictionary.Add(270, "char_support_06");
			dictionary.Add(271, "char_support_06_hd");
			dictionary.Add(272, "obj_robohand");
			dictionary.Add(273, "obj_robohand_hd");
			dictionary.Add(274, "hand_catch");
			dictionary.Add(275, "hand_drop");
			dictionary.Add(276, "hand_rotate");
			dictionary.Add(301, "menu_scrollbar");
			dictionary.Add(302, "menu_button_achiv_cup");
			dictionary.Add(303, "menu_leaderboard");
			dictionary.Add(304, "empty_achievement");
			dictionary.Add(305, "arrows");
			dictionary.Add(306, "scotch_tape_1");
			dictionary.Add(307, "scotch_tape_2");
			dictionary.Add(308, "scotch_tape_3");
			dictionary.Add(309, "scotch_tape_4");
			dictionary.Add(310, "menu_agepopup");
			dictionary.Add(311, "menu_agepopup_hd");
			dictionary.Add(312, "menu_agepopup_bgr");
			dictionary.Add(313, "menu_agepopup_bgr_hd");
			resNames_ = dictionary;
		}
		resNames_.TryGetValue(handleLocalizedResource(handleWvgaResource(resId)), out var value);
		return value;
	}

	public override NSObject loadResource(int resID, ResourceType resType)
	{
		return base.loadResource(handleLocalizedResource(handleWvgaResource(resID)), resType);
	}

	public override void freeResource(int resID)
	{
		base.freeResource(handleLocalizedResource(handleWvgaResource(resID)));
	}
}
