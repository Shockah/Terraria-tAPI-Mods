using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class ILPreBuffs : InterfaceLayer
	{
		public ILPreBuffs() : base(MBase.me.modName + ":PreBuffs") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			CopyPastedModifiedBuffsCode(sb);
		}

		private void CopyPastedModifiedBuffsCode(SpriteBatch sb)
		{
			Main.buffString = "";
			if (!Main.recBigList)
			{
				Main.recStart = 0;
			}
			if (!Main.playerInventory)
			{
				Main.recBigList = false;
				int num28 = -1;
				int num29 = 11;
				for (int l = 0; l < 22; l++)
				{
					if (Main.localPlayer.buffType[l] > 0)
					{
						int num30 = Main.localPlayer.buffType[l];
						int num31 = 32 + l * 38;
						int num32 = 76;
						if (l >= num29)
						{
							num31 = 32 + (l - num29) * 38;
							num32 += 50;
						}
						Color color = new Color(Main.buffAlpha[l], Main.buffAlpha[l], Main.buffAlpha[l], Main.buffAlpha[l]);
						sb.Draw(Main.buffTexture[num30], new Vector2((float)num31, (float)num32), new Rectangle?(new Rectangle(0, 0, Main.buffTexture[num30].Width, Main.buffTexture[num30].Height)), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
						if (!Defs.buffNoTimer[num30] && !Main.vanityPet[num30] && !Main.lightPet[num30] && num30 != 64 && num30 != 60 && num30 != 49 && num30 != 28 && num30 != 34 && num30 != 37 && num30 != 38 && num30 != 40 && num30 != 41 && num30 != 27 && num30 != 19 && num30 != 42 && num30 != 43 && num30 != 45 && (!Main.localPlayer.honeyWet || num30 != 48) && num30 != 62 && num30 != 67 && num30 != 68 && num30 != 81 && num30 != 82 && num30 != 83 && num30 != 90 && Main.localPlayer.buffTime[l] > 2)
						{
							string text2;
							if (Main.localPlayer.buffTime[l] / 60 >= 3600)
							{
								text2 = Math.Round((double)(Main.localPlayer.buffTime[l] / 60) / 3600.0) + " h";
							}
							else if (Main.localPlayer.buffTime[l] / 60 >= 60)
							{
								text2 = Math.Round((double)(Main.localPlayer.buffTime[l] / 60) / 60.0) + " m";
							}
							else
							{
								text2 = Math.Round((double)Main.localPlayer.buffTime[l] / 60.0) + " s";
							}
							sb.DrawString(Main.fontItemStack, text2, new Vector2((float)num31, (float)(num32 + Main.buffTexture[num30].Height)), color, 0f, default(Vector2), 0.8f, SpriteEffects.None, 0f);
						}
						if (Main.mouseX < num31 + Main.buffTexture[num30].Width && Main.mouseY < num32 + Main.buffTexture[num30].Height && Main.mouseX > num31 && Main.mouseY > num32)
						{
							num28 = l;
							Main.buffAlpha[l] += 0.1f;
							if (Main.mouseRight && Main.mouseRightRelease && !Main.debuff[num30] && num30 != 60)
							{
								if (num30 == 90)
								{
									Main.localPlayer.Dismount();
								}
								Main.PlaySound(12, -1, -1, 1);
								Main.localPlayer.DelBuff(l);
							}
						}
						else
						{
							Main.buffAlpha[l] -= 0.05f;
						}
						if (Main.buffAlpha[l] > 1f)
						{
							Main.buffAlpha[l] = 1f;
						}
						else if ((double)Main.buffAlpha[l] < 0.4)
						{
							Main.buffAlpha[l] = 0.4f;
						}
					}
					else
					{
						Main.buffAlpha[l] = 0.4f;
					}
				}
				if (num28 >= 0)
				{
					int num33 = Main.localPlayer.buffType[num28];
					if (num33 > 0)
					{
						MBase.me.FillTooltipBuff(num33);
						Main.buffString = Main.buffTip[num33];
					}
				}
			}
		}
	}
}