using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.utils;

internal partial class Images
{
	private static partial ContentManager getContentManager(string imgName);

	public static Texture2D get(string imgName)
	{
		ContentManager contentManager = getContentManager(imgName);
		Texture2D result = null;
        result = contentManager.Load<Texture2D>(imgName);
        return result;
	}

	public static void free(string imgName)
	{
		ContentManager contentManager = getContentManager(imgName);
		contentManager.Unload();
	}
}
