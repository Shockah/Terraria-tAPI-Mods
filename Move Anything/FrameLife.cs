using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.MoveAnything
{
	public class FrameLife : FrameHiding
	{
		public bool hidden = false;
		
		public FrameLife() : base("Life", Anchor.TopLeft, new Vector2(500f + (Main.screenWidth - 800f), 32f), new Vector2(260, 52)) { }

		protected override void OnRender2(SpriteBatch sb)
		{
			Vector2 pos = FramePos1();
			Main.heartLife = 20f;
			if (Main.localPlayer.statLifeMax2 > 400)
			{
				Main.heartLife = (float)Main.localPlayer.statLifeMax2 / 20f;
			}
			int num2 = (Main.localPlayer.statLifeMax2 - 400) / 5;
			if (num2 < 0)
			{
				num2 = 0;
			}
			int num3 = 1;
			while ((float)num3 < (float)Main.localPlayer.statLifeMax2 / Main.heartLife + 1f)
			{
				float num4 = 1f;
				bool flag = false;
				int num5;
				if ((float)Main.localPlayer.statLife >= (float)num3 * Main.heartLife)
				{
					num5 = 255;
					if ((float)Main.localPlayer.statLife == (float)num3 * Main.heartLife)
					{
						flag = true;
					}
				}
				else
				{
					float num6 = ((float)Main.localPlayer.statLife - (float)(num3 - 1) * Main.heartLife) / Main.heartLife;
					num5 = (int)(30f + 225f * num6);
					if (num5 < 30)
					{
						num5 = 30;
					}
					num4 = num6 / 4f + 0.75f;
					if ((double)num4 < 0.75)
					{
						num4 = 0.75f;
					}
					if (num6 > 0f)
					{
						flag = true;
					}
				}
				if (flag)
				{
					num4 += Main.cursorScale - 1f;
				}
				int num7 = 0;
				int num8 = 0;
				if (num3 > 10)
				{
					num7 -= 260;
					num8 += 26;
				}
				int num9 = (int)((double)((float)num5) * 0.9);
				if (!Main.localPlayer.ghost)
				{
					if (num2 > 0)
					{
						num2--;
						sb.Draw(Main.heart2Texture, new Vector2(Main.heartTexture.Width * .5f + (float)(pos.X + 26 * (num3 - 1) + num7), Main.heartTexture.Width * .5f + pos.Y + ((float)Main.heartTexture.Height - (float)Main.heartTexture.Height * num4) / 2f + (float)num8), null, new Color(num5, num5, num5, num9), 0f, Main.heartTexture.Size() / 2f, num4, SpriteEffects.None, 0f);
					}
					else
					{
						sb.Draw(Main.heartTexture, new Vector2(Main.heartTexture.Width * .5f + (float)(pos.X + 26 * (num3 - 1) + num7), Main.heartTexture.Width * .5f + pos.Y + ((float)Main.heartTexture.Height - (float)Main.heartTexture.Height * num4) / 2f + (float)num8), null, new Color(num5, num5, num5, num9), 0f, Main.heartTexture.Size() / 2f, num4, SpriteEffects.None, 0f);
					}
				}
				num3++;
			}
		}
	}
}
