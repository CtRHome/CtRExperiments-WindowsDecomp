using System.IO;
using Microsoft.Xna.Framework;

namespace CutTheRope.wp7utilities;

internal class ContentHelper
{
	internal static string OpenResourceAsString(string name)
	{
		return new StreamReader(OpenResourceAsStream(name)).ReadToEnd();
	}

	internal static Stream OpenResourceAsStream(string resPath)
	{
		Stream stream = null;
		return TitleContainer.OpenStream("Content/ctre/" + resPath);
	}
}
