using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class ILPreMouseOver : InterfaceLayer
	{
		public ILPreMouseOver() : base(MBase.me.modName + ":PreMouseOver") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			CopyPastedModifiedMouseOverCode(sb);
		}

		private void CopyPastedModifiedMouseOverCode(SpriteBatch sb)
		{
			Rectangle rectangle = new Rectangle((int)((float)Main.mouseX + Main.screenPosition.X), (int)((float)Main.mouseY + Main.screenPosition.Y), 1, 1);
			if (Main.localPlayer.gravDir == -1f)
			{
				rectangle.Y = (int)Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
			}
			if (!Main.mouseText)
			{
				int num = 26 * Main.localPlayer.statLifeMax2 / (int)Main.heartLife;
				int num2 = 0;
				if (Main.localPlayer.statLifeMax2 > 200)
				{
					num = 260;
					num2 += 26;
				}
				if (Main.mouseX > 500 + Main.sX && Main.mouseX < 500 + num + Main.sX && Main.mouseY > 32 && Main.mouseY < 32 + Main.heartTexture.Height + num2)
				{
					Main.localPlayer.showItemIcon = false;
					string cursorText = Main.localPlayer.statLife + "/" + Main.localPlayer.statLifeMax2;
					MBase.me.FillTooltip(cursorText);
					Main.mouseText = true;
				}
			}
			if (!Main.mouseText)
			{
				int num3 = 24;
				int num4 = 28 * Main.localPlayer.statManaMax2 / Main.starMana;
				if (Main.mouseX > 762 + Main.sX && Main.mouseX < 762 + num3 + Main.sX && Main.mouseY > 30 && Main.mouseY < 30 + num4)
				{
					Main.localPlayer.showItemIcon = false;
					string cursorText2 = Main.localPlayer.statMana + "/" + Main.localPlayer.statManaMax2;
					MBase.me.FillTooltip(cursorText2);
					Main.mouseText = true;
				}
			}
			if (!Main.mouseText)
			{
				for (int i = 0; i < 400; i++)
				{
					if (Main.item[i].active)
					{
						Rectangle value = new Rectangle((int)((double)Main.item[i].position.X + (double)Main.item[i].width * 0.5 - (double)Main.itemTexture[Main.item[i].type].Width * 0.5), (int)(Main.item[i].position.Y + (float)Main.item[i].height - (float)Main.itemTexture[Main.item[i].type].Height), Main.itemTexture[Main.item[i].type].Width, Main.itemTexture[Main.item[i].type].Height);
						if (rectangle.Intersects(value))
						{
							Main.localPlayer.showItemIcon = false;
							string text = Main.item[i].AffixName();
							if (Main.item[i].stack > 1)
							{
								object obj = text;
								text = string.Concat(new object[]
								{
									obj,
									" (",
									Main.item[i].stack,
									")"
								});
							}
							if (Main.item[i].owner < 255 && Main.showItemOwner)
							{
								text = text + " <" + Main.player[Main.item[i].owner].name + ">";
							}
							Main.rare = Main.item[i].rare;
							MBase.me.FillTooltipItemInWorld(Main.item[i]);
							Main.mouseText = true;
							break;
						}
					}
				}
			}
			for (int j = 0; j < 255; j++)
			{
				if (Main.player[j].active && Main.myPlayer != j && !Main.player[j].dead)
				{
					Rectangle value2 = new Rectangle((int)((double)Main.player[j].position.X + (double)Main.player[j].width * 0.5 - 16.0), (int)(Main.player[j].position.Y + (float)Main.player[j].height - 48f), 32, 48);
					if (!Main.mouseText && rectangle.Intersects(value2))
					{
						Main.localPlayer.showItemIcon = false;
						MBase.me.FillTooltip(Main.player[j]);
					}
				}
			}
			Main.mouseOverNpc = false;
			if (!Main.mouseText)
			{
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active)
					{
						Main.LoadNPC(Main.npc[k].type);
						Rectangle value3 = new Rectangle((int)((double)Main.npc[k].position.X + (double)Main.npc[k].width * 0.5 - (double)Main.npcTexture[Main.npc[k].type].Width * 0.5), (int)(Main.npc[k].position.Y + (float)Main.npc[k].height - (float)(Main.npcTexture[Main.npc[k].type].Height / Main.npcFrameCount[Main.npc[k].type])), Main.npcTexture[Main.npc[k].type].Width, Main.npcTexture[Main.npc[k].type].Height / Main.npcFrameCount[Main.npc[k].type]);
						if (Main.npc[k].type >= 87 && Main.npc[k].type <= 92)
						{
							value3 = new Rectangle((int)((double)Main.npc[k].position.X + (double)Main.npc[k].width * 0.5 - 32.0), (int)((double)Main.npc[k].position.Y + (double)Main.npc[k].height * 0.5 - 32.0), 64, 64);
						}
						if (rectangle.Intersects(value3) && ((Main.npc[k].type != 85 && Main.npc[k].type != 341) || Main.npc[k].ai[0] != 0f))
						{
							bool flag = false;
							if (Main.npc[k].townNPC || Main.npc[k].type == 105 || Main.npc[k].type == 106 || Main.npc[k].type == 123)
							{
								Rectangle rectangle2 = new Rectangle((int)(Main.localPlayer.position.X + (float)(Main.localPlayer.width / 2) - (float)(Main.localPlayer.tileRangeX * 16)), (int)(Main.localPlayer.position.Y + (float)(Main.localPlayer.height / 2) - (float)(Main.localPlayer.tileRangeY * 16)), Main.localPlayer.tileRangeX * 16 * 2, Main.localPlayer.tileRangeY * 16 * 2);
								Rectangle value4 = new Rectangle((int)Main.npc[k].position.X, (int)Main.npc[k].position.Y, Main.npc[k].width, Main.npc[k].height);
								if (rectangle2.Intersects(value4))
								{
									flag = true;
								}
							}
							if (flag && !Main.localPlayer.dead)
							{
								Main.mouseOverNpc = true;
								int num6 = -(Main.npc[k].width / 2 + 8);
								SpriteEffects effects = SpriteEffects.None;
								if (Main.npc[k].spriteDirection == -1)
								{
									effects = SpriteEffects.FlipHorizontally;
									num6 = Main.npc[k].width / 2 + 8;
								}
								sb.Draw(Main.chatTexture, new Vector2(Main.npc[k].position.X + (float)(Main.npc[k].width / 2) - Main.screenPosition.X - (float)(Main.chatTexture.Width / 2) - (float)num6, Main.npc[k].position.Y - (float)Main.chatTexture.Height - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, Main.chatTexture.Width, Main.chatTexture.Height)), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, default(Vector2), 1f, effects, 0f);
								if (Main.mouseRight && Main.npcChatRelease)
								{
									Main.npcChatRelease = false;
									if (Main.localPlayer.talkNPC != k)
									{
										Main.npcShop = 0;
										Main.craftGuide = false;
										Main.localPlayer.dropItemCheck();
										Recipe.FindRecipes();
										Main.localPlayer.sign = -1;
										Main.editSign = false;
										Main.localPlayer.talkNPC = k;
										Main.playerInventory = false;
										Main.localPlayer.chest = -1;
										Main.npcChatText = Main.npc[k].GetChat();
										Main.PlaySound(24, -1, -1, 1);
									}
								}
							}
							Main.localPlayer.showItemIcon = false;
							MBase.me.FillTooltip(Main.npc[k]);
							return;
						}
					}
				}
			}
		}
	}
}