using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Achievements
{
	public class Notifier
	{
		public const int
			OFFSET_SCREEN_EDGE = 16,
			OFFSET_NOTIFIERS = 0,
			WIDTH = 300,
			HEIGHT = 56,
			ICON_WIDTH = 48,
			ICON_HEIGHT = 48;
		public static readonly Color
			COLOR_TOP = new Color(95, 95, 95),
			COLOR_BOTTOM = new Color(15, 15, 15);
		
		public readonly int index = 0;
		public readonly Achievement achievement;
		public readonly object icon;
		internal int anim = 0;

		public Notifier(int index, Achievement achievement, object icon = null)
		{
			this.index = index;
			this.achievement = achievement;
			this.icon = icon == null ? achievement.icon : icon;
		}

		public bool Draw(SpriteBatch sb)
		{
			anim++;
			if (anim == 6 * 60) return true;

			float alpha = 0f;
			if (anim < 30)
			{
				alpha = 1f * anim / 30;
			}
			else if (anim < 5 * 60)
			{
				alpha = 1f;
			}
			else
			{
				alpha = 1f - (1f * (anim - 5 * 60) / 60);
			}

			if (alpha > 0f)
			{
				Texture2D tex = MBase.me.textures["Notifier"];
				Vector2 pos = new Vector2((Main.screenWidth - WIDTH) / 2, Main.screenHeight - HEIGHT - OFFSET_SCREEN_EDGE - (index * (HEIGHT + OFFSET_NOTIFIERS)));
				sb.Draw(tex, new Rectangle((int)pos.X, (int)pos.Y, WIDTH, HEIGHT), Color.White * alpha);
				Vector2 pos2 = new Vector2(icon == null ? pos.X + WIDTH / 2 : pos.X + HEIGHT + (WIDTH - HEIGHT) / 2, pos.Y + HEIGHT / 2);
				Vector2 measure = Main.fontMouseText.MeasureString(achievement.name);
				Drawing.DrawStringShadow(sb, Main.fontMouseText, achievement.name, pos2, Color.Black * alpha, 0f, measure / 2);
				sb.DrawString(Main.fontMouseText, achievement.name, pos2, Color.White * alpha, 0f, measure / 2, 1f, SpriteEffects.None, 0f);

				if (icon != null)
				{
					if (icon is Texture2D)
					{
						Texture2D ic = (Texture2D)icon;
						float iscale = 1f;
						if (ic.Width > ICON_WIDTH) iscale = 1f * ICON_WIDTH / ic.Width;
						if (ic.Height * iscale > ICON_HEIGHT) iscale = 1f * ICON_HEIGHT / ic.Height;

						Vector2 pos3 = new Vector2(pos.X + HEIGHT / 2, pos.Y + HEIGHT / 2);
						sb.Draw(ic, pos3, null, Color.White * alpha, 0f, new Vector2(ic.Width, ic.Height) / 2, iscale, SpriteEffects.None, 0f);
					}
					else if (icon is Item)
					{
						Item ic = (Item)icon;
						float iscale = 1f;
						Texture2D texItem = ic.GetTexture();
						if (texItem.Width > ICON_WIDTH) iscale = 1f * ICON_WIDTH / texItem.Width;
						if (texItem.Height * iscale > ICON_HEIGHT) iscale = 1f * ICON_HEIGHT / texItem.Height;

						Vector2 pos3 = new Vector2(pos.X + HEIGHT / 2, pos.Y + HEIGHT / 2);
						Vector2 origin = new Vector2(texItem.Width, texItem.Height) / 2;
						sb.Draw(texItem, pos3, null, ic.GetAlpha(ic.GetTextureColor()) * alpha, 0f, origin, iscale, SpriteEffects.None, 0f);
						if (ic.color != default(Color)) sb.Draw(texItem, pos3, null, ic.GetColor(Color.White) * alpha, 0f, origin, iscale, SpriteEffects.None, 0f);
					}
				}
			}

			return false;
		}
	}
}