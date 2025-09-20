using CutTheRope.windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutTheRope.utils;

internal partial class Global
{

    private static GraphicsDeviceManager graphicsDeviceManager_;

    public static GraphicsDeviceManager GraphicsDeviceManager
    {
        get
        {
            return graphicsDeviceManager_;
        }
        set
        {
            graphicsDeviceManager_ = value;
        }
    }

    private static ScreenSizeManager screenSizeManager_ = new ScreenSizeManager(500, 800);

    public static ScreenSizeManager ScreenSizeManager
    {
        get
        {
            return screenSizeManager_;
        }
        set
        {
            screenSizeManager_ = value;
        }
    }

    private static MouseCursor mouseCursor_ = new MouseCursor();

    public static MouseCursor MouseCursor => mouseCursor_;

    private static Game1 game_;

    public static Game1 XnaGame
    {
        get
        {
            return game_;
        }
        set
        {
            game_ = value;
        }
    }


}
