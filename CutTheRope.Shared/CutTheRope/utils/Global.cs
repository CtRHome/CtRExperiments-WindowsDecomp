using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.utils;

internal partial class Global
{
	private static SpriteBatch spriteBatch_;

	private static GraphicsDevice graphicsDevice_;

	public static SpriteBatch SpriteBatch
	{
		get
		{
			return spriteBatch_;
		}
		set
		{
			spriteBatch_ = value;
		}
	}

	public static GraphicsDevice GraphicsDevice
	{
		get
		{
			return graphicsDevice_;
		}
		set
		{
			graphicsDevice_ = value;
		}
	}
}
