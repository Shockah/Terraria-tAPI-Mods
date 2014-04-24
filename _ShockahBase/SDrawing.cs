using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI;

namespace Shockah.Base
{
	public static class SDrawing
	{
		public static void StringShadowed(SpriteBatch sb, SpriteFont font, string text, Vector2 pos)
		{
			StringShadowed(sb, font, text, pos, Color.White);
		}
		public static void StringShadowed(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color c, float scale = 1f, Vector2 origin = default(Vector2))
		{
			Drawing.DrawStringShadow(sb, font, text, pos, new Color(0, 0, 0, c.A), 0f, origin, scale);
			sb.DrawString(font, text, pos, c, 0f, origin, scale, SpriteEffects.None, 0f);
		}
	}
}