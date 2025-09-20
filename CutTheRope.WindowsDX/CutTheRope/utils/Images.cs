using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutTheRope.utils;

internal partial class Images
{
    private static Dictionary<string, ContentManager> _contentManagers = new Dictionary<string, ContentManager>();

    private static partial ContentManager getContentManager(string imgName)
    {
        ContentManager value = null;
        _contentManagers.TryGetValue(imgName, out value);
        if (value == null)
        {
            value = new ContentManager(Global.XnaGame.Services, "content");
            _contentManagers.Add(imgName, value);
        }
        return value;
    }

}