using CutTheRope.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutTheRope.iframework;

internal partial class OpenGL
{
    private static partial Microsoft.Xna.Framework.Rectangle GetScreenRectangle()
    {
        //return Global.ScreenSizeManager.ScaledViewRect;
        var viewport = Global.GraphicsDevice.Viewport;
        return viewport.Bounds;
    }
}