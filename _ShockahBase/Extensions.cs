using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public static class Extensions
	{
		public static void DrawFull(this Player player)
		{
			if (player.ghost)
			{
				for (int num69 = 0; num69 < 3; num69++)
				{
					API.main.DrawGhost(player, player.shadowPos[num69], 0.5f + 0.2f * (float)num69);
				}
				API.main.DrawGhost(player, player.position, 0f);
			}
			else
			{
				if (player.inventory[player.selectedItem].flame || player.head == 137 || player.wings == 22)
				{
					player.itemFlameCount--;
					if (player.itemFlameCount <= 0)
					{
						player.itemFlameCount = 5;
						for (int num70 = 0; num70 < 7; num70++)
						{
							player.itemFlamePos[num70].X = (float)Main.rand.Next(-10, 11) * 0.15f;
							player.itemFlamePos[num70].Y = (float)Main.rand.Next(-10, 1) * 0.35f;
						}
					}
				}
				if (player.head == 111 && player.body == 73 && player.legs == 62)
				{
					player.drawGlow = true;
					player.drawAura = true;
				}
				if (player.head == 134 && player.body == 95 && player.legs == 79)
				{
					player.drawGlow = true;
					player.drawAura = true;
				}
				if (player.head == 107 && player.body == 69 && player.legs == 58)
				{
					player.drawGlow = true;
					player.drawAfterimage = true;
				}
				if (player.head == 108 && player.body == 70 && player.legs == 59)
				{
					player.drawGlow = true;
					player.drawAfterimage = true;
				}
				if (player.head == 109 && player.body == 71 && player.legs == 60)
				{
					player.drawGlow = true;
					player.drawAfterimage = true;
				}
				if (player.head == 110 && player.body == 72 && player.legs == 61)
				{
					player.drawGlow = true;
					player.drawAfterimage = true;
				}
				if (player.body == 67 && player.legs == 56 && player.head >= 103 && player.head <= 105)
				{
					player.drawAfterimage = true;
				}
				if ((player.head == 78 || player.head == 79 || player.head == 80) && player.body == 51 && player.legs == 47)
				{
					player.drawGlow = true;
				}
				if (player.dashDelay < 0)
				{
					player.drawAfterimage = true;
				}
				if (player.head == 5 && player.body == 5 && player.legs == 5)
				{
					player.drawAfterimage = true;
				}
				if (player.head == 74 && player.body == 48 && player.legs == 44)
				{
					player.drawAfterimage = true;
				}
				if (player.head == 76 && player.body == 49 && player.legs == 45)
				{
					player.drawAfterimage = true;
				}
				if (player.head == 7 && player.body == 7 && player.legs == 7)
				{
					player.drawAfterimage = true;
				}
				if (player.head == 22 && player.body == 14 && player.legs == 14)
				{
					player.drawAfterimage = true;
				}
				if (player.dye[0].dye == 30 && player.dye[1].dye == 30 && player.dye[2].dye == 30 && player.head == 4 && player.body == 27 && player.legs == 26)
				{
					player.drawAfterimage = true;
					player.drawAura = true;
				}
				if (player.body == 17 && player.legs == 16 && (player.head == 29 || player.head == 30 || player.head == 31))
				{
					player.drawAfterimage = true;
				}
				if (player.body == 19 && player.legs == 18 && (player.head == 35 || player.head == 36 || player.head == 37))
				{
					player.drawAura = true;
				}
				bool flag5 = false;
				if (player.wings == 3 || (player.wings >= 16 && player.wings <= 19))
				{
					flag5 = true;
				}
				else if (player.head == 45 || (player.head >= 106 && player.head <= 110))
				{
					flag5 = true;
				}
				else if (player.body == 26 || (player.body >= 68 && player.body <= 74))
				{
					flag5 = true;
				}
				else if (player.legs == 25 || (player.legs >= 57 && player.legs <= 63))
				{
					flag5 = true;
				}
				if (flag5)
				{
					Player expr_5147_cp_0 = player;
					expr_5147_cp_0.velocity.X = expr_5147_cp_0.velocity.X * 0.9f;
					if (player.velocity.Y < 0f)
					{
						Player expr_5172_cp_0 = player;
						expr_5172_cp_0.velocity.Y = expr_5172_cp_0.velocity.Y + 0.2f;
					}
					player.jump = 0;
					player.statDefense = -1000;
					player.AddBuff(23, 2, false);
					player.AddBuff(80, 2, false);
					player.AddBuff(67, 2, false);
					player.AddBuff(32, 2, false);
					player.AddBuff(31, 2, false);
					player.AddBuff(33, 2, false);
				}
				if (player.body == 26 && player.legs == 25 && player.head == 45)
				{
					player.drawAura = true;
					player.drawAfterimage = true;
				}
				if (player.body == 26 && player.legs == 25 && player.head == 63)
				{
					player.drawAura = true;
					player.drawAfterimage = true;
				}
				if (player.body == 24 && player.legs == 23 && (player.head == 41 || player.head == 42 || player.head == 43))
				{
					player.drawAura = true;
					player.drawAfterimage = true;
				}
				if (player.head == 157 && player.legs == 98 && player.body != 105)
				{
					int arg_5292_0 = player.body;
				}
				if (player.body == 36 && player.head == 56)
				{
					player.drawAura = true;
				}
				Vector2 position;
				if (player.drawAura)
				{
					if (!Main.gamePaused)
					{
						player.ghostFade += player.ghostDir * 0.075f;
					}
					if ((double)player.ghostFade < 0.1)
					{
						player.ghostDir = 1f;
						player.ghostFade = 0.1f;
					}
					else if ((double)player.ghostFade > 0.9)
					{
						player.ghostDir = -1f;
						player.ghostFade = 0.9f;
					}
					float num71 = player.ghostFade * 5f;
					for (int num72 = 0; num72 < 4; num72++)
					{
						float num73 = 0;
						float num74 = 0;
						switch (num72)
						{
							case 0:
								num73 = num71;
								num74 = 0f;
								goto IL_539B;
							case 1:
								num73 = -num71;
								num74 = 0f;
								goto IL_539B;
							case 2:
								num73 = 0f;
								num74 = num71;
								goto IL_539B;
							case 3:
								num73 = 0f;
								num74 = -num71;
								goto IL_539B;
						}
					IL_539B:
						position = new Vector2(player.position.X + num73, player.position.Y + player.gfxOffY + num74);
						API.main.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, player.ghostFade);
					}
				}
				if (player.drawAfterimage)
				{
					for (int num75 = 0; num75 < 3; num75++)
					{
						API.main.DrawPlayer(player, player.shadowPos[num75], player.shadowRotation[num75], player.shadowOrigin[num75], 0.5f + 0.2f * (float)num75);
					}
				}
				if (player.drawGlow)
				{
					for (int num76 = 0; num76 < 4; num76++)
					{
						position.X = player.position.X + (float)Main.rand.Next(-20, 21) * 0.1f;
						position.Y = player.position.Y + (float)Main.rand.Next(-20, 21) * 0.1f + player.gfxOffY;
						API.main.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.9f);
					}
				}
				if (player.shadowDodge)
				{
					player.shadowDodgeCount += 1f;
					if (player.shadowDodgeCount > 30f)
					{
						player.shadowDodgeCount = 30f;
					}
				}
				else
				{
					player.shadowDodgeCount -= 1f;
					if (player.shadowDodgeCount < 0f)
					{
						player.shadowDodgeCount = 0f;
					}
				}
				if (player.shadowDodgeCount > 0f)
				{
					Vector2 arg_555E_0 = player.position;
					position.X = player.position.X + player.shadowDodgeCount;
					position.Y = player.position.Y + player.gfxOffY;
					API.main.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
					position.X = player.position.X - player.shadowDodgeCount;
					API.main.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
				}
				position = player.position;
				position.Y += player.gfxOffY;
				API.main.DrawPlayer(player, position, player.fullRotation, player.fullRotationOrigin, 0f);
			}
		}
	}
}