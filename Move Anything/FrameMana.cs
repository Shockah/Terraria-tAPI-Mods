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
	public class FrameMana : FrameHiding
	{
		public FrameMana() : base("Mana", Anchor.TopLeft, new Vector2(775f + (Main.screenWidth - 800f) - Main.manaTexture.Width * .5f, 30f), new Vector2(Main.manaTexture.Width, 0)) { }

		protected override bool IsVisible()
		{
			return Main.localPlayer.statManaMax2 > 0;
		}

		protected override void OnUpdate()
		{
			size.Y = (int)(Math.Ceiling(Main.localPlayer.statManaMax2 / 20f) * 28f);
		}

		protected override void OnRender2(SpriteBatch sb)
		{
			Vector2 pos = FramePos1();
			Main.starMana = 20;
			for (int i = 1; i < Main.localPlayer.statManaMax2 / Main.starMana + 1; i++)
			{
				bool flag2 = false;
				float num10 = 1f;
				int num11;
				if (Main.localPlayer.statMana >= i * Main.starMana)
				{
					num11 = 255;
					if (Main.localPlayer.statMana == i * Main.starMana)
					{
						flag2 = true;
					}
				}
				else
				{
					float num12 = (float)(Main.localPlayer.statMana - (i - 1) * Main.starMana) / (float)Main.starMana;
					num11 = (int)(30f + 225f * num12);
					if (num11 < 30)
					{
						num11 = 30;
					}
					num10 = num12 / 4f + 0.75f;
					if ((double)num10 < 0.75)
					{
						num10 = 0.75f;
					}
					if (num12 > 0f)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					num10 += Main.cursorScale - 1f;
				}
				int num13 = (int)((double)((float)num11) * 0.9);
				sb.Draw(Main.manaTexture, new Vector2(pos.X + Main.manaTexture.Width * .5f, (float)(pos.Y + Main.manaTexture.Height / 2) + ((float)Main.manaTexture.Height - (float)Main.manaTexture.Height * num10) / 2f + (float)(28 * (i - 1))), null, new Color(num11, num11, num11, num13), 0f, new Vector2((float)(Main.manaTexture.Width / 2), (float)(Main.manaTexture.Height / 2)), num10, SpriteEffects.None, 0f);
			}
		}
	}
}
