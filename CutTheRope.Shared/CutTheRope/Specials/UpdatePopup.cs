using CutTheRope.ctr_commons;
using CutTheRope.ctr_original;
using CutTheRope.game;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using Microsoft.Xna.Framework;

namespace CutTheRope.Specials;

internal class UpdatePopup : Popup, ButtonDelegate
{
	private const int buttonNo = 0;

	private const int buttonYes = 1;

	public void onButtonPressed(int n)
	{
		switch (n)
		{
		case 0:
			hidePopup();
			break;
		case 1:
			break;
		}
	}

	public static void showUpdatePopup()
	{
		bool flag = false;
		switch (Application.sharedRootController().activeChildID)
		{
		case 0:
			//App.NeedsUpdate = true;
			return;
		case 2:
			//App.NeedsUpdate = true;
			return;
		case 1:
			flag = false;
			break;
		case 3:
			flag = true;
			break;
		}
		//App.NeedsUpdate = false;
		UpdatePopup updatePopup = (UpdatePopup)new UpdatePopup().init();
		updatePopup.setName(NSObject.NSS("popup"));
		Button button = MenuController.createButtonWithTextIDDelegate(Application.getString(1179675), 0, updatePopup);
		button.anchor = (button.parentAnchor = 18);
		button.setTouchIncreaseLeftRightTopBottom(15f, 15f, 0f, 0f);
		Button button2 = MenuController.createButtonWithTextIDDelegate(Application.getString(1179674), 1, updatePopup);
		button2.anchor = 33;
		button2.parentAnchor = 9;
		button2.setTouchIncreaseLeftRightTopBottom(15f, 15f, 0f, 0f);
		button.addChild(button2);
		FontGeneric font = Application.getFont(5);
		float num = 300f;
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
		{
			num /= 0.85f;
		}
		Text text = new Text().initWithFont(font);
		text.setAlignment(2);
		text.setStringandWidth(Application.getString(1179729), num);
		text.y = -34f;
		text.scaleX = (text.scaleY = 0.8f);
		if (ResDataPhoneFullExperiments.LANGUAGE == Language.LANG_KO)
		{
			text.scaleX *= 0.85f;
			text.scaleY *= 0.85f;
		}
		Image image = Image.Image_createWithResIDQuad(70, 0);
		image.anchor = (image.parentAnchor = 18);
		updatePopup.addChild(image);
		text.anchor = (text.parentAnchor = 18);
		image.addChild(text);
		button.y += -14f;
		button.anchor = 18;
		button.parentAnchor = 34;
		image.addChild(button);
		updatePopup.showPopup();
		ViewController currentController = Application.sharedRootController().getCurrentController();
		View view = currentController.activeView();
		view.addChild(updatePopup);
		if (flag)
		{
			((GameController)currentController).setPaused(p: true);
		}
		else
		{
			MenuController.ep = updatePopup;
		}
	}
}
