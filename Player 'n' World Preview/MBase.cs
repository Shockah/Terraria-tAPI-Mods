using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.IO;
using TAPI;
using Terraria;

namespace Shockah.PNWPreview
{
	public class MBase : ModBase
	{
		public const int VERSION = 1;

		public static MBase me;
		public static readonly Dictionary<string, WorldInfo> infos = new Dictionary<string, WorldInfo>();
		public static bool shouldSave = false, wasInGame = false;

		private MenuButton btnPlayer = null, btnWorld = null;

		public static WorldInfo Current
		{
			get
			{
				if (!infos.ContainsKey(Main.worldName)) infos[Main.worldName] = new WorldInfo();
				return infos[Main.worldName];
			}
		}

		internal class FakeMenuButton : MenuButton
		{
			public readonly Action<SpriteBatch> draw;
			
			public FakeMenuButton(Action<SpriteBatch> draw) : base(0, "", "")
			{
				this.draw = draw;
			}

			public override bool MouseOver(Vector2 mouse)
			{
				return false;
			}

			public override void Draw(SpriteBatch sb, bool mouseOver)
			{
				draw(sb);
			}
		}

		public override void OnLoad()
		{
			me = this;
			if (File.Exists(Main.WorldPath + "/" + modName + ".dat"))
			{
				BinBuffer bb = new BinBuffer(new BinBufferByte(File.ReadAllBytes(Main.WorldPath + "/" + modName + ".dat")));
				if (bb.ReadUShort() > VERSION) return;

				while (bb.BytesLeft() > 0)
				{
					string wname = bb.ReadString();
					infos[wname] = WorldInfo.Load(bb);
				}
			}

			#region Player
			btnPlayer = new FakeMenuButton((sb) =>
				{
					MenuPage mp = Menu.menuPages[Menu.currentPage];
					for (int i = 0; i < 5; i++)
					{
						if (Menu.currentPage != "Player Select") break;
						MenuButton mb = mp.buttons[i];
						if (mb.displayText == "<Create Player>") continue;
						Player p = Main.loadPlayer[i + Menu.skip];
						if (p == null) continue;
						p.position = mb.position + new Vector2(mb.size.X + 40, 0) + Main.screenPosition;

						#region Drawing Player
						sb.End();
						sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, API.main.Rasterizer, null, API.main.Transform);
						if (p.head == 137 || p.wings == 22)
						{
							p.itemFlameCount--;
							if (p.itemFlameCount <= 0)
							{
								p.itemFlameCount = 5;
								for (int num70 = 0; num70 < 7; num70++)
								{
									p.itemFlamePos[num70].X = (float)Main.rand.Next(-10, 11) * 0.15f;
									p.itemFlamePos[num70].Y = (float)Main.rand.Next(-10, 1) * 0.35f;
								}
							}
						}
						bool flag2 = false;
						bool flag3 = false;
						bool flag4 = false;
						if (p.head == 111 && p.body == 73 && p.legs == 62)
						{
							flag3 = true;
							flag4 = true;
						}
						if (p.head == 134 && p.body == 95 && p.legs == 79)
						{
							flag3 = true;
							flag4 = true;
						}
						if (p.head == 107 && p.body == 69 && p.legs == 58)
						{
							flag3 = true;
							flag2 = true;
						}
						if (p.head == 108 && p.body == 70 && p.legs == 59)
						{
							flag3 = true;
							flag2 = true;
						}
						if (p.head == 109 && p.body == 71 && p.legs == 60)
						{
							flag3 = true;
							flag2 = true;
						}
						if (p.head == 110 && p.body == 72 && p.legs == 61)
						{
							flag3 = true;
							flag2 = true;
						}
						if (p.body == 67 && p.legs == 56 && p.head >= 103 && p.head <= 105)
						{
							flag2 = true;
						}
						if ((p.head == 78 || p.head == 79 || p.head == 80) && p.body == 51 && p.legs == 47)
						{
							flag3 = true;
						}
						if (p.dashDelay < 0)
						{
							flag2 = true;
						}
						if (p.head == 5 && p.body == 5 && p.legs == 5)
						{
							flag2 = true;
						}
						if (p.head == 74 && p.body == 48 && p.legs == 44)
						{
							flag2 = true;
						}
						if (p.head == 76 && p.body == 49 && p.legs == 45)
						{
							flag2 = true;
						}
						if (p.head == 7 && p.body == 7 && p.legs == 7)
						{
							flag2 = true;
						}
						if (p.head == 22 && p.body == 14 && p.legs == 14)
						{
							flag2 = true;
						}
						if (p.dye[0].dye == 30 && p.dye[1].dye == 30 && p.dye[2].dye == 30 && p.head == 4 && p.body == 27 && p.legs == 26)
						{
							flag2 = true;
							flag4 = true;
						}
						if (p.body == 17 && p.legs == 16 && (p.head == 29 || p.head == 30 || p.head == 31))
						{
							flag2 = true;
						}
						if (p.body == 19 && p.legs == 18 && (p.head == 35 || p.head == 36 || p.head == 37))
						{
							flag4 = true;
						}
						if (p.body == 26 && p.legs == 25 && p.head == 45)
						{
							flag4 = true;
							flag2 = true;
						}
						if (p.body == 26 && p.legs == 25 && p.head == 63)
						{
							flag4 = true;
							flag2 = true;
						}
						if (p.body == 24 && p.legs == 23 && (p.head == 41 || p.head == 42 || p.head == 43))
						{
							flag4 = true;
							flag2 = true;
						}
						if (p.body == 36 && p.head == 56)
						{
							flag4 = true;
						}
						if (flag4)
						{
							Vector2 position2 = p.position;
							if (p.ghostDir == 0 || p.ghostFade < .1f)
							{
								p.ghostDir = 1;
								p.ghostFade = .1f;
							}
							p.ghostFade += p.ghostDir * 0.075f;
							if ((double)p.ghostFade < 0.1)
							{
								p.ghostDir = 1f;
								p.ghostFade = 0.1f;
							}
							if ((double)p.ghostFade > 0.9)
							{
								p.ghostDir = -1f;
								p.ghostFade = 0.9f;
							}
							p.position.X = position2.X - p.ghostFade * 5f;
							p.position.Y = p.position.Y + p.gfxOffY;
							p.shadow = p.ghostFade;
							API.main.DrawPlayer(p);
							p.position.X = position2.X + p.ghostFade * 5f;
							p.shadow = p.ghostFade;
							API.main.DrawPlayer(p);
							p.position.X = position2.X;
							p.position.Y = position2.Y - p.ghostFade * 5f + p.gfxOffY;
							p.shadow = p.ghostFade;
							API.main.DrawPlayer(p);
							p.position.Y = position2.Y + p.ghostFade * 5f + p.gfxOffY;
							p.shadow = p.ghostFade;
							API.main.DrawPlayer(p);
							p.position = position2;
							p.shadow = 0f;
						}
						if (flag2)
						{
							Vector2 position3 = p.position;
							p.position = p.shadowPos[0];
							p.shadow = 0.5f;
							API.main.DrawPlayer(p);
							p.position = p.shadowPos[1];
							p.shadow = 0.7f;
							API.main.DrawPlayer(p);
							p.position = p.shadowPos[2];
							p.shadow = 0.9f;
							API.main.DrawPlayer(p);
							p.position = position3;
							p.shadow = 0f;
						}
						if (flag3)
						{
							for (int num71 = 0; num71 < 4; num71++)
							{
								Vector2 position4 = p.position;
								p.position.X = p.position.X + (float)Main.rand.Next(-20, 21) * 0.1f;
								p.position.Y = p.position.Y + (float)Main.rand.Next(-20, 21) * 0.1f + p.gfxOffY;
								p.shadow = 0.9f;
								API.main.DrawPlayer(p);
								p.position = position4;
								p.shadow = 0f;
							}
						}
						if (p.shadowDodge)
						{
							p.shadowDodgeCount += 1f;
							if (p.shadowDodgeCount > 30f) p.shadowDodgeCount = 30f;
						}
						else
						{
							p.shadowDodgeCount -= 1f;
							if (p.shadowDodgeCount < 0f) p.shadowDodgeCount = 0f;
						}
						if (p.shadowDodgeCount > 0f)
						{
							Vector2 position5 = p.position;
							p.position.X = p.position.X + p.shadowDodgeCount;
							p.position.Y = p.position.Y + p.gfxOffY;
							p.shadow = 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f;
							API.main.DrawPlayer(p);
							p.position.X = p.position.X - p.shadowDodgeCount * 2f;
							p.shadow = 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f;
							API.main.DrawPlayer(p);
							p.shadow = 0f;
							p.position = position5;
						}
						API.main.DrawPlayer(p);
						sb.End();
						sb.Begin();
						#endregion

						Vector2 pos = new Vector2(mb.position.X + mb.size.X + 80, mb.position.Y);
						if (mb.MouseOver(Main.mouse))
						{
							float w = (800 - mb.size.X) / 2 - 96;
							string text;
							const float scale = .75f;
							Drawing.DrawBox(sb, pos.X, pos.Y, w, Main.fontMouseText.LineSpacing * 2 * scale + 12);
							pos.X += 6;
							pos.Y += 6;

							text = "Life: " + p.statLifeMax;
							SDrawing.StringShadowed(sb, Main.fontMouseText, text, pos, Color.White, scale);
							pos.Y += Main.fontMouseText.LineSpacing * scale;

							if (p.statManaMax > 0)
							{
								text = "Mana: " + p.statManaMax;
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, pos, Color.White, scale);
								pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
						}
					}
				}
			);
			Menu.menuPages["Player Select"].buttons.Add(btnPlayer);
			#endregion

			#region World
			btnWorld = new FakeMenuButton((sb) =>
				{
					MenuPage mp = Menu.menuPages[Menu.currentPage];
					int btnBack = -1;
					for (int i = mp.buttons.Count - 1; i >= 0; i--)
					{
						if (mp.buttons[i].displayText == "Back")
						{
							btnBack = i;
							break;
						}
					}
					if (btnBack == -1) return;
					
					for (int i = 0; i < 5; i++)
					{
						if (Menu.currentPage != "World Select") return;
						MenuButton mb = mp.buttons[i];
						if (mb.displayText == "<Create World>") continue;
						if (!infos.ContainsKey(mb.displayText)) continue;
						WorldInfo wi = infos[mb.displayText];

						Vector2 pos = new Vector2(mb.position.X + mb.size.X + 32, mp.buttons[0].position.Y);
						if (mb.MouseOver(Main.mouse))
						{
							float w = (800 - mb.size.X) / 2 - 48;
							Drawing.DrawBox(sb, pos.X, pos.Y, w, mp.buttons[btnBack].position.Y + mp.buttons[btnBack].size.Y - mp.buttons[0].position.Y);
							pos.X += 6;
							pos.Y += 6;
							string text;
							const float scale = .75f;
							int col2 = 0;
							Vector2 pos2;
							float itemOff = Main.fontMouseText.LineSpacing * scale * .5f;

							text = "" + wi.width + "x" + wi.height;
							if (wi.width == 4200 && wi.height == 1200) text += " (" + Lang.menu[92] + ")";
							if (wi.width == 6400 && wi.height == 1800) text += " (" + Lang.menu[93] + ")";
							if (wi.width == 8400 && wi.height == 2400) text += " (" + Lang.menu[94] + ")";
							SDrawing.StringShadowed(sb, Main.fontMouseText, text, pos, Color.White, scale);
							pos.Y += Main.fontMouseText.LineSpacing * scale;

							if (wi.hardmode)
							{
								text = "Entered hardmode";
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, pos, Color.Orange, scale);
								pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (wi.corruption)
							{
								text = "Corruption";
								DrawItem(sb, Defs.items["Vanilla:Demonite Bar"], new Vector2(pos.X + itemOff, pos.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos.X + 20, pos.Y), Color.White, scale);
								pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.crimson)
							{
								text = "Crimson";
								DrawItem(sb, Defs.items["Vanilla:Crimtane Bar"], new Vector2(pos.X + itemOff, pos.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos.X + 20, pos.Y), Color.White, scale);
								pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							pos.Y += itemOff;

							if (wi.oreEasy[(int)WorldInfo.Ore.Copper])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Copper") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.oreEasy[(int)WorldInfo.Ore.Tin])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Tin") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (wi.oreEasy[(int)WorldInfo.Ore.Iron])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Iron") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.oreEasy[(int)WorldInfo.Ore.Lead])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Lead") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (wi.oreEasy[(int)WorldInfo.Ore.Silver])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Silver") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.oreEasy[(int)WorldInfo.Ore.Tungsten])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Tungsten") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (wi.oreEasy[(int)WorldInfo.Ore.Gold])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Gold") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.oreEasy[(int)WorldInfo.Ore.Platinum])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:" + (text = "Platinum") + " Bar"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (col2 % 2 != 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							col2 = 0;
							pos.Y += itemOff;

							DrawItem(sb, Defs.items["Vanilla:" + (text = (wi.oreHard[0] ? "Palladium" : "Cobalt")) + " Bar"], new Vector2(pos.X + itemOff, pos.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
							SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos.X + 20, pos.Y), Color.White, scale);
							pos.Y += Main.fontMouseText.LineSpacing * scale;

							DrawItem(sb, Defs.items["Vanilla:" + (text = (wi.oreHard[1] ? "Orichalcum" : "Mythril")) + " Bar"], new Vector2(pos.X + itemOff, pos.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
							SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos.X + 20, pos.Y), Color.White, scale);
							pos.Y += Main.fontMouseText.LineSpacing * scale;

							DrawItem(sb, Defs.items["Vanilla:" + (text = (wi.oreHard[2] ? "Titanium" : "Adamantite")) + " Bar"], new Vector2(pos.X + itemOff, pos.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
							SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos.X + 20, pos.Y), Color.White, scale);
							pos.Y += Main.fontMouseText.LineSpacing * scale;

							pos.Y += itemOff;

							if (wi.bosses[(int)WorldInfo.Boss.EyeOfCthulhu])
							{
								text = "Eye of Cthulhu";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:Suspicious Looking Eye"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.bosses[(int)WorldInfo.Boss.EaterOfWorldsOrBrainOfCthulhu])
							{
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								if (wi.corruption && !wi.crimson)
								{
									text = "Eater of Worlds";
									DrawItem(sb, Defs.items["Vanilla:Worm Food"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								}
								else if (!wi.corruption && wi.crimson)
								{
									text = "Brain of Cthulhu";
									DrawItem(sb, Defs.items["Vanilla:Bloody Spine"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								}
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.bosses[(int)WorldInfo.Boss.Skeletron])
							{
								text = "Skeletron";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:Clothier Voodoo Doll"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.bosses[(int)WorldInfo.Boss.QueenBee])
							{
								text = "Queen Bee";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:Abeemination"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (col2 % 2 != 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							col2 = 0;

							if (wi.bosses[(int)WorldInfo.Boss.TheDestroyer])
							{
								text = "The Destroyer";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:Mechanical Worm"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.bosses[(int)WorldInfo.Boss.TheTwins])
							{
								text = "The Twins";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:Mechanical Eye"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.bosses[(int)WorldInfo.Boss.SkeletronPrime])
							{
								text = "Skeletron Prime";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:Mechanical Skull"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.bosses[(int)WorldInfo.Boss.Plantera])
							{
								text = "Plantera";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawTexture(sb, textures["Images/BossPlantera.png"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}
							if (wi.bosses[(int)WorldInfo.Boss.Golem])
							{
								text = "Golem";
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawItem(sb, Defs.items["Vanilla:Lihzahrd Power Cell"], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (col2 % 2 != 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							col2 = 0;
							pos.Y += itemOff;

							foreach (string townNPC in wi.townNPCs)
							{
								NPC def = null;
								if (Defs.npcs.ContainsKey(townNPC)) def = Defs.npcs[townNPC];
								if (def == null) continue;
								text = def.displayName;
								pos2 = new Vector2(pos.X + (col2++ % 2) * (w - 12) / 2, pos.Y);
								DrawTexture(sb, Main.npcHeadTexture[NPC.TypeToNum(def.type)], new Vector2(pos2.X + itemOff, pos2.Y + itemOff), Main.fontMouseText.LineSpacing * scale);
								SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(pos2.X + 20, pos2.Y), Color.White, scale);
								if (col2 % 2 == 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							}

							if (col2 % 2 != 0) pos.Y += Main.fontMouseText.LineSpacing * scale;
							col2 = 0;
							pos.Y += itemOff;
						}
					}
				}
			);
			Menu.menuPages["World Select"].buttons.Add(btnWorld);
			#endregion
		}

		public override void OnUnload()
		{
			Menu.menuPages["Player Select"].buttons.Remove(btnPlayer);
			Menu.menuPages["World Select"].buttons.Remove(btnWorld);
		}
		
		public override void PostGameDraw(SpriteBatch sb)
		{
			if (!Main.gameMenu)
			{
				shouldSave = true;
				wasInGame = true;
				return;
			}

			if (shouldSave)
			{
				WorldInfo _wi = Current;
				if (wasInGame)
				{
					wasInGame = false;
					if (WorldGen.oreTier1 != -1) _wi.oreHard[0] = WorldGen.oreTier1 != 221;
					if (WorldGen.oreTier2 != -1) _wi.oreHard[1] = WorldGen.oreTier2 != 222;
					if (WorldGen.oreTier3 != -1) _wi.oreHard[2] = WorldGen.oreTier3 != 223;
					_wi.hardmode = Main.hardMode;
					_wi.bosses[(int)WorldInfo.Boss.EyeOfCthulhu] = NPC.downedBoss1;
					_wi.bosses[(int)WorldInfo.Boss.EaterOfWorldsOrBrainOfCthulhu] = NPC.downedBoss2;
					_wi.bosses[(int)WorldInfo.Boss.Skeletron] = NPC.downedBoss3;
					_wi.bosses[(int)WorldInfo.Boss.QueenBee] = NPC.downedQueenBee;
					_wi.bosses[(int)WorldInfo.Boss.TheDestroyer] = NPC.downedMechBoss1;
					_wi.bosses[(int)WorldInfo.Boss.TheTwins] = NPC.downedMechBoss2;
					_wi.bosses[(int)WorldInfo.Boss.SkeletronPrime] = NPC.downedMechBoss3;
					_wi.bosses[(int)WorldInfo.Boss.Plantera] = NPC.downedPlantBoss;
					_wi.bosses[(int)WorldInfo.Boss.Golem] = NPC.downedGolemBoss;
				}
				
				shouldSave = false;
				if (File.Exists(Main.WorldPath + "/" + modName + ".dat")) File.Delete(Main.WorldPath + "/" + modName + ".dat");
				BinBuffer bb = new BinBuffer();
				bb.Write((ushort)VERSION);

				foreach (KeyValuePair<string, WorldInfo> kvp in infos)
				{
					if (!kvp.Value.read) continue;
					bb.Write(kvp.Key);
					WorldInfo.Save(bb, kvp.Value);
				}
				bb.Pos = 0;
				File.WriteAllBytes(Main.WorldPath + "/" + modName + ".dat", bb.ReadBytes());
			}
		}

		private void DrawItem(SpriteBatch sb, Item item, Vector2 pos, float maxSize)
		{
			float iscale = 1f;
			Texture2D texItem = item.GetTexture();
			if (texItem.Width > maxSize || texItem.Height > maxSize) iscale = texItem.Width > texItem.Height ? maxSize / texItem.Width : maxSize / texItem.Height;
			Vector2 origin = new Vector2(texItem.Width, texItem.Height) / 2;

			sb.Draw(texItem, pos, null, item.GetAlpha(item.GetTextureColor()), 0f, origin, iscale, SpriteEffects.None, 0f);
			if (item.color != default(Color)) sb.Draw(texItem, pos, null, item.GetColor(Color.White), 0f, origin, iscale, SpriteEffects.None, 0f);
		}
		private void DrawTexture(SpriteBatch sb, Texture2D tex, Vector2 pos, float maxSize)
		{
			float iscale = 1f;
			if (tex.Width > maxSize || tex.Height > maxSize) iscale = tex.Width > tex.Height ? maxSize / tex.Width : maxSize / tex.Height;
			Vector2 origin = new Vector2(tex.Width, tex.Height) / 2;
			sb.Draw(tex, pos, null, Color.White, 0f, origin, iscale, SpriteEffects.None, 0f);
		}
	}
}