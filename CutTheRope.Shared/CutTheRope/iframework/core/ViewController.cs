using System;
using System.Collections.Generic;
using CutTheRope.ctr_commons;
using CutTheRope.game;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.iframework.core;

internal class ViewController : NSObject, TouchDelegate
{
	public enum ControllerState
	{
		CONTROLLER_DEACTIVE,
		CONTROLLER_ACTIVE,
		CONTROLLER_PAUSED
	}

	public const int FAKE_TOUCH_UP_TO_DEACTIVATE_BUTTONS = -10000;

	public ControllerState controllerState;

	public int activeViewID;

	public Dictionary<int, View> views;

	public int activeChildID;

	public Dictionary<int, ViewController> childs;

	public ViewController parent;

	public int pausedViewID;

	public bool shouldCutTouches;

	public ViewController()
	{
		views = new Dictionary<int, View>();
	}

	public virtual NSObject initWithParent(ViewController p)
	{
		if (base.init() != null)
		{
			controllerState = ControllerState.CONTROLLER_DEACTIVE;
			views = new Dictionary<int, View>();
			childs = new Dictionary<int, ViewController>();
			activeViewID = -1;
			activeChildID = -1;
			pausedViewID = -1;
			parent = p;
		}
		return this;
	}

	public virtual void activate()
	{
		controllerState = ControllerState.CONTROLLER_ACTIVE;
		Application.sharedRootController().onControllerActivated(this);
	}

	public virtual void deactivate()
	{
		Application.sharedRootController().onControllerDeactivationRequest(this);
	}

	public virtual void deactivateImmediately()
	{
		controllerState = ControllerState.CONTROLLER_DEACTIVE;
		if (activeViewID != -1)
		{
			hideActiveView();
		}
		Application.sharedRootController().onControllerDeactivated(this);
		parent.onChildDeactivated(parent.activeChildID);
	}

	public virtual void pause()
	{
		controllerState = ControllerState.CONTROLLER_PAUSED;
		Application.sharedRootController().onControllerPaused(this);
		if (activeViewID != -1)
		{
			pausedViewID = activeViewID;
			hideActiveView();
		}
	}

	public virtual void unpause()
	{
		controllerState = ControllerState.CONTROLLER_ACTIVE;
		if (activeChildID != -1)
		{
			activeChildID = -1;
		}
		Application.sharedRootController().onControllerUnpaused(this);
		if (pausedViewID != -1)
		{
			showView(pausedViewID);
		}
	}

	public virtual void update(float delta)
	{
		if (activeViewID != -1)
		{
			View view = activeView();
			view.update(delta);
		}
	}

	public virtual void addViewwithID(View v, int n)
	{
		views.TryGetValue(n, out var _);
		views[n] = v;
	}

	public virtual void deleteView(int n)
	{
		views[n] = null;
	}

	public virtual void hideActiveView()
	{
		View view = views[activeViewID];
		Application.sharedRootController().onControllerViewHide(view);
		view?.hide();
		activeViewID = -1;
	}

	public virtual void showView(int n)
	{
		if (activeViewID != -1)
		{
			hideActiveView();
		}
		activeViewID = n;
		View view = views[n];
		Application.sharedRootController().onControllerViewShow(view);
		view.show();
	}

	public virtual View activeView()
	{
		return views[activeViewID];
	}

	public virtual View getView(int n)
	{
		View value = null;
		views.TryGetValue(n, out value);
		return value;
	}

	public virtual void addChildwithID(ViewController c, int n)
	{
		((NSObject)null)?.dealloc();
		childs[n] = c;
	}

	public virtual void deleteChild(int n)
	{
		ViewController value = null;
		if (childs.TryGetValue(n, out value))
		{
			value.dealloc();
			childs[n] = null;
		}
	}

	public virtual void deactivateActiveChild()
	{
		ViewController viewController = childs[activeChildID];
		viewController.deactivate();
		activeChildID = -1;
	}

	public virtual void activateChild(int n)
	{
		if (activeChildID != -1)
		{
			deactivateActiveChild();
		}
		pause();
		activeChildID = n;
		ViewController viewController = childs[n];
		viewController.activate();
	}

	public virtual void onChildDeactivated(int n)
	{
		unpause();
	}

	public virtual ViewController activeChild()
	{
		return childs[activeChildID];
	}

	public virtual ViewController getChild(int n)
	{
		return childs[n];
	}

	private bool checkNoChildsActive()
	{
		foreach (KeyValuePair<int, ViewController> child in childs)
		{
			ViewController value = child.Value;
			if (value != null && value.controllerState != 0)
			{
				return false;
			}
		}
		return true;
	}

	public Vector convertTouchForLandscape(Vector t)
	{
		throw new NotImplementedException();
	}

	public void cutTouchCollection(TouchCollection touches)
	{
		for (int i = 1; i < touches.Count; i++)
		{
			touches.RemoveAt(i);
		}
	}

	public virtual bool touchesBeganwithEvent(TouchCollection touches)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (activeViewID == -1)
		{
			return false;
		}
		View view = activeView();
		int num = -1;
		for (int i = 0; i < touches.Count; i++)
		{
			num++;
			if (num > 1)
			{
				break;
			}
			TouchLocation touchLocation = touches[i];
			if (touchLocation.State == TouchLocationState.Pressed)
			{
				return view.onTouchDownXY(CtrRenderer.transformX(touchLocation.Position.X), CtrRenderer.transformY(touchLocation.Position.Y));
			}
		}
		return false;
	}

	public virtual bool touchesEndedwithEvent(TouchCollection touches)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (activeViewID == -1)
		{
			return false;
		}
		View view = activeView();
		int num = -1;
		for (int i = 0; i < touches.Count; i++)
		{
			num++;
			if (num > 1)
			{
				break;
			}
			TouchLocation touchLocation = touches[i];
			if (touchLocation.State == TouchLocationState.Released)
			{
				Drawing.touchUpHook = true;
				return view.onTouchUpXY(CtrRenderer.transformX(touchLocation.Position.X), CtrRenderer.transformY(touchLocation.Position.Y));
			}
		}
		return false;
	}

	public virtual bool touchesMovedwithEvent(TouchCollection touches)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (activeViewID == -1)
		{
			return false;
		}
		View view = activeView();
		int num = -1;
		for (int i = 0; i < touches.Count; i++)
		{
			num++;
			if (num > 1)
			{
				break;
			}
			TouchLocation touchLocation = touches[i];
			if (touchLocation.State == TouchLocationState.Moved)
			{
				return view.onTouchMoveXY(CtrRenderer.transformX(touchLocation.Position.X), CtrRenderer.transformY(touchLocation.Position.Y));
			}
		}
		return false;
	}

	public virtual bool touchesCancelledwithEvent(TouchCollection touches)
	{
		foreach (TouchLocation item in touches)
		{
			_ = item.State;
		}
		return false;
	}

	public override void dealloc()
	{
		views.Clear();
		views = null;
		childs.Clear();
		childs = null;
		base.dealloc();
	}

	public virtual bool backButtonPressed()
	{
		return false;
	}

	public virtual bool menuButtonPressed()
	{
		return false;
	}
}
