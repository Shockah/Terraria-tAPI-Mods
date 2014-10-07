using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Text;
using TAPI;
using Terraria;
using Terraria.DataStructures;

namespace Shockah.FCM.Standard
{
	public class InterfaceFCMMisc : InterfaceFCMBase
	{
		public const int POS_X = 20, POS_Y = 300;
		public static readonly string[] MOON_PHASES = new string[] { "Full Moon", "Waning Gibbous", "Third Quarter", "Waning Crescent", "New Moon", "Waxing Crescent", "First Quarter", "Waxing Gibbous" };

		public static InterfaceFCMMisc me = null;
		public static int throttleTimeUpdate = 0;
		public static bool timeUpdateSend = false;
		public static bool freeCamera = false, fullBright = false, flashlight = false, flashlightOff = false;

		public static void Reset()
		{
			freeCamera = false;
			fullBright = false;
			flashlight = false;
			flashlightOff = false;
			throttleTimeUpdate = 0;
			timeUpdateSend = false;
		}

		public static void QueueTimeUpdate()
		{
			if (Main.netMode != 1) return;
			if (throttleTimeUpdate > 0) return;
			throttleTimeUpdate = 5;
			timeUpdateSend = true;
		}
		public static void SendTimeUpdate(int player, int ignorePlayer)
		{
			SendTimeUpdate(player, ignorePlayer, Main.netMode == 1);
		}
		public static void SendTimeUpdate(int remote, int ignore, bool addMyId)
		{
			if (Main.netMode == 0) return;

			BinBuffer bb = new BinBuffer();
			if (addMyId) bb.Write((byte)Main.myPlayer);

			bb.Write(Main.dayTime);
			bb.Write((float)Main.time);
			bb.Write((ushort)Main.dayRate);
			bb.Write((byte)Main.moonPhase);
			bb.Write(new BitsByte(Main.hardMode, Main.bloodMoon, Main.eclipse));

			MWorld mw = (MWorld)MBase.me.modWorld;
			bb.Write(mw.blockNPCSpawn);
			bb.Write(mw.blockNPCSpawnSave);
			bb.Write(mw.lockDayTime.HasValue);
			if (mw.lockDayTime.HasValue) bb.Write(mw.lockDayTime.Value);
			bb.Write(mw.lockDayTimeSave);
			bb.Write(mw.lockDayRate.HasValue);

			bb.Pos = 0;
			NetMessage.SendModData(MBase.me, MBase.MSG_TIME, remote, ignore, bb);
		}
		public static void SendCheatUpdate(int player, int ignorePlayer)
		{
			SendCheatUpdate(player, ignorePlayer, Main.netMode == 1);
		}
		public static void SendCheatUpdate(int remote, int ignore, bool addMyId)
		{
			if (Main.netMode == 0) return;

			BinBuffer bb = new BinBuffer();
			if (addMyId) bb.Write((byte)Main.myPlayer);

			MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
			bb.Write((byte)1);
			bb.Write((byte)Main.myPlayer);
			bb.Write(new BitsByte(mp.cheatGod, mp.cheatNoclip, mp.cheatUsage, mp.cheatRange, mp.cheatTileSpeed, mp.cheatTileUsage));

			bb.Pos = 0;
			NetMessage.SendModData(MBase.me, MBase.MSG_CHEAT, remote, ignore, bb);
		}

		public static void SetAbsoluteTime(int time)
		{
			time += 27000;
			time = FixAbsoluteTime(time);
			Main.time = time;
			Main.dayTime = time < 54000;
			if (time >= 54000) Main.time -= 54000;
		}
		public static int GetAbsoluteTime()
		{
			int ret = (int)Main.time;
			if (!Main.dayTime) ret += 54000;
			ret -= 27000;
			return FixAbsoluteTime(ret);
		}
		public static int FixAbsoluteTime(int time)
		{
			while (time < 0) time += 86400;
			while (time >= 86400) time -= 86400;
			return time;
		}
		public static int[] GetTime(int time = -1)
		{
			if (time < 0) time = GetAbsoluteTime();
			time = FixAbsoluteTime(time);

			int h = 0, m = 0, s = 0;
			s = time % 60; time /= 60;
			m = time % 60; time /= 60;
			h = time % 24;

			return new int[] { h, m, s };
		}
		public static string GetTimeText(bool system24h, int time = -1)
		{
			int[] l = GetTime(time);
			if (system24h)
			{
				string ret = "", s;

				s = "" + l[0];
				while (s.Length < 2) s = "0" + s;
				ret += s;

				s = "" + l[1];
				while (s.Length < 2) s = "0" + s;
				ret += ":" + s;

				return ret;
			}
			else
			{
				string ret = "", s;
				bool am = false;

				if (l[0] >= 12 && l[0] < 24) am = true;
				l[0] %= 12;
				if (l[0] == 0) l[0] = 12;

				s = "" + l[0];
				ret += s;

				s = "" + l[1];
				while (s.Length < 2) s = "0" + s;
				ret += ":" + s;

				ret += " " + (am ? "AM" : "PM");

				return ret;
			}
		}

		protected readonly ElButton
			bLockDayTime, bLockDayTimeSave, bLockDayRate,
			bHardmode, bBloodMoon, bEclipse,
			bGodmode, bNoclip, bUsage,
			bBlockSpawns, bBlockSpawnsSave,
			bRange, bTileSpeed, bTileUsage,
			bCamera, bFullBright/*, bFlashlight*/;
		protected string dragging = null;

		public InterfaceFCMMisc()
		{
			me = this;
			if (Main.dedServ) return;

			bLockDayTime = new ElButton(
				(b, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					bool newVal = mb == 0;
					if (mw.lockDayTime.HasValue)
					{
						mw.lockDayTime = newVal == mw.lockDayTime.Value ? null : new bool?(newVal);
					}
					else
					{
						mw.lockDayTime = newVal;
					}
					if (!mw.lockDayTime.HasValue) mw.lockDayTimeSave = false;
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					if (!mw.lockDayTime.HasValue) return;
					
					Texture2D tex = mw.lockDayTime.Value ? Main.sunTexture : Main.sun3Texture;
					float tscale = 1f;
					int dist = (tex.Width > tex.Height ? tex.Width : tex.Height);
					if (dist > b.size.X - 4) tscale = (b.size.X - 4) / dist;
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					StringBuilder sb = new StringBuilder();
					sb.Append(mw.lockDayTime.HasValue ? (mw.lockDayTime.Value ? "Day-only" : "Night-only") : "No time limiter");
					sb.Append("\nLeft click to " + (mw.lockDayTime.HasValue && mw.lockDayTime.Value ? "reset" : "set to day-only"));
					sb.Append("\nRight click to " + (mw.lockDayTime.HasValue && !mw.lockDayTime.Value ? "reset" : "set to night-only"));
					SBase.tip = sb.ToString();
				}
			);

			bLockDayTimeSave = new ElButton(
				(b, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					mw.lockDayTimeSave = !mw.lockDayTimeSave;
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					if (!mw.lockDayTimeSave) return;

					Texture2D tex = Shockah.FCM.MBase.me.textures["Images/Tick"];
					float tscale = 1f;
					int dist = (tex.Width > tex.Height ? tex.Width : tex.Height);
					if (dist > b.size.X - 4) tscale = (b.size.X - 4) / dist;
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					StringBuilder sb = new StringBuilder();
					sb.Append(mw.lockDayTimeSave ? "Save time limiter" : "Ignore time limiter when saving");
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bLockDayRate = new ElButton(
				(b, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					mw.lockDayRate = mw.lockDayRate.HasValue ? null : new int?(Main.dayRate);
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					if (!mw.lockDayRate.HasValue) return;

					Texture2D tex = Shockah.FCM.MBase.me.textures["Images/Tick"];
					float tscale = 1f;
					int dist = (tex.Width > tex.Height ? tex.Width : tex.Height);
					if (dist > b.size.X - 4) tscale = (b.size.X - 4) / dist;
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					StringBuilder sb = new StringBuilder();
					sb.Append(mw.lockDayRate.HasValue ? "Save day rate" : "Ignore day rate when saving");
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bHardmode = new ElButton(
				(b, mb) =>
				{
					Main.hardMode = !Main.hardMode;
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					Texture2D tex = Main.itemTexture[Main.hardMode ? 544 : 43];
					float tscale = 1f;
					int dist = (tex.Width > tex.Height ? tex.Width : tex.Height);
					if (dist > b.size.X - 4) tscale = (b.size.X - 4) / dist;
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Hardmode: " + (Main.hardMode ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bBloodMoon = new ElButton(
				(b, mb) =>
				{
					Main.bloodMoon = !Main.bloodMoon;
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					Texture2D tex = Main.moonTexture[0];
					float tscale = 1f;
					int dist = tex.Height / 8;
					if (dist > b.size.X - 4) tscale = (b.size.X - 4) / dist;
					sb.Draw(tex, b.pos + b.size / 2, new Rectangle?(new Rectangle(0, 0, tex.Width, dist)), Main.bloodMoon ? Color.Red : Color.White, 0f, new Vector2(tex.Width / 2, tex.Height / 8 / 2), tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Blood Moon: " + (Main.hardMode ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bEclipse = new ElButton(
				(b, mb) =>
				{
					Main.eclipse = !Main.eclipse;
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					Texture2D tex = Main.eclipse ? Main.sun3Texture : Main.sunTexture;
					float tscale = 1f;
					int dist = (tex.Width > tex.Height ? tex.Width : tex.Height);
					if (dist > b.size.X - 4) tscale = (b.size.X - 4) / dist;
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Solar Eclipse: " + (Main.hardMode ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bGodmode = new ElButton(
				(b, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					mp.cheatGod = !mp.cheatGod;
					SendCheatUpdate(-1, -1);
				},
				(b, sb, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					Texture2D tex = Main.buffTexture[10];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (mp.cheatGod ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					StringBuilder sb = new StringBuilder();
					sb.Append("Godmode: " + (mp.cheatGod ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bNoclip = new ElButton(
				(b, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					mp.cheatNoclip = !mp.cheatNoclip;
					SendCheatUpdate(-1, -1);
				},
				(b, sb, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					Texture2D tex = Main.buffTexture[18];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (mp.cheatNoclip ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					StringBuilder sb = new StringBuilder();
					sb.Append("Noclip: " + (mp.cheatNoclip ? "On" : "Off"));
					if (mp.cheatNoclip) sb.Append("\nHold Shift to move faster");
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bUsage = new ElButton(
				(b, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					mp.cheatUsage = !mp.cheatUsage;
					SendCheatUpdate(-1, -1);
				},
				(b, sb, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					Texture2D tex = MBase.me.textures["Images/IconCheatUsage"];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (mp.cheatUsage ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					StringBuilder sb = new StringBuilder();
					sb.Append("No ammo and mana usage: " + (mp.cheatUsage ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bRange = new ElButton(
				(b, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					mp.cheatRange = !mp.cheatRange;
					SendCheatUpdate(-1, -1);
				},
				(b, sb, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					Texture2D tex = MBase.me.textures["Images/IconCheatRange"];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (mp.cheatRange ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					StringBuilder sb = new StringBuilder();
					sb.Append("Extended range: " + (mp.cheatRange ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bTileSpeed = new ElButton(
				(b, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					mp.cheatTileSpeed = !mp.cheatTileSpeed;
					SendCheatUpdate(-1, -1);
				},
				(b, sb, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					Texture2D tex = MBase.me.textures["Images/IconCheatTileSpeed"];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (mp.cheatTileSpeed ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					StringBuilder sb = new StringBuilder();
					sb.Append("Increased building speed: " + (mp.cheatTileSpeed ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bTileUsage = new ElButton(
				(b, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					mp.cheatTileUsage = !mp.cheatTileUsage;
					SendCheatUpdate(-1, -1);
				},
				(b, sb, mb) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					Texture2D tex = MBase.me.textures["Images/IconCheatTileUsage"];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (mp.cheatTileUsage ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					StringBuilder sb = new StringBuilder();
					sb.Append("Don't consume tiles: " + (mp.cheatTileUsage ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bBlockSpawns = new ElButton(
				(b, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					mw.blockNPCSpawn = !mw.blockNPCSpawn;
					if (!mw.blockNPCSpawn) mw.blockNPCSpawnSave = false;
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					if (!mw.blockNPCSpawn) return;

					Texture2D tex = Main.cdTexture;
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					StringBuilder sb = new StringBuilder();
					sb.Append(mw.blockNPCSpawn ? "Block NPCs from spawning" : "Let NPCs spawn");
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bBlockSpawnsSave = new ElButton(
				(b, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					mw.blockNPCSpawnSave = !mw.blockNPCSpawnSave;
					QueueTimeUpdate();
				},
				(b, sb, mb) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					if (!mw.blockNPCSpawnSave) return;

					Texture2D tex = Shockah.FCM.MBase.me.textures["Images/Tick"];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					MWorld mw = (MWorld)MBase.me.modWorld;
					StringBuilder sb = new StringBuilder();
					sb.Append(mw.blockNPCSpawnSave ? "Save NPC spawning setting" : "Ignore NPC spawning setting when saving");
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bCamera = new ElButton(
				(b, mb) =>
				{
					freeCamera = !freeCamera;
				},
				(b, sb, mb) =>
				{
					Texture2D tex = Main.buffTexture[12];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (freeCamera ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(freeCamera ? "Free camera mode" : "Camera attached to player");
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			bFullBright = new ElButton(
				(b, mb) =>
				{
					fullBright = !fullBright;
				},
				(b, sb, mb) =>
				{
					Texture2D tex = Main.buffTexture[27];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (fullBright ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Full bright: " + (fullBright ? "On" : "Off"));
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);

			/*bFlashlight = new ElButton(
				(b, mb) =>
				{
					flashlight = !flashlight;
					flashlightOff = false;
				},
				(b, sb, mb) =>
				{
					Texture2D tex = Main.buffTexture[19];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White * (flashlight ? 1f : .5f), 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				},
				(b) =>
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Flashlight: " + (flashlight ? "On" : "Off"));
					if (flashlight) sb.Append("\nPress Tab to quick-toggle");
					sb.Append("\nClick to toggle");
					SBase.tip = sb.ToString();
				}
			);*/
		}

		public override void OnOpen()
		{
			base.OnOpen();
		}

		public override void Draw(InterfaceLayer layer, SpriteBatch sb)
		{
			if (dragging != null) Main.localPlayer.mouseInterface = true;
			base.Draw(layer, sb);
			bool blocked = false;
			MWorld mw = (MWorld)MBase.me.modWorld;

			Action<string, string, Vector2, Texture2D, float, Func<float, string>, Action<float>> drawSliderFloat = (name, tip, pos, sliderTex, ratio, textBuilder, codeSet) =>
			{
				sb.Draw(sliderTex, new Vector2(pos.X, pos.Y + 20), Color.White);
				sb.Draw(Main.colorSliderTexture, new Vector2(pos.X + 4 + (sliderTex.Width - 8) * ratio, pos.Y + 20 + sliderTex.Height / 2), null, Color.White, 0f, Main.colorSliderTexture.Size() * .5f, 1f, SpriteEffects.None, 0f);
				Drawing.StringShadowed(sb, Main.fontMouseText, tip, pos, Color.White, .8f);
				string valtext = textBuilder(ratio);
				float valscale = .8f;
				if (Main.fontMouseText.MeasureString(valtext).X * valscale > sliderTex.Width / 2) valscale = (sliderTex.Width / 2) / Main.fontMouseText.MeasureString(valtext).X;
				Drawing.StringShadowed(sb, Main.fontMouseText, valtext, new Vector2((float)Math.Round(pos.X + sliderTex.Width - Main.fontMouseText.MeasureString(textBuilder(ratio)).X * valscale), pos.Y), Color.White, valscale);
				if (dragging == name || (dragging == null && Math2.InRegion(Main.mouse, new Vector2(pos.X, pos.Y + 20), sliderTex.Width, sliderTex.Height)))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft)
					{
						dragging = name;
						int deltax = Main.mouseX - ((int)pos.X + 4);
						ratio = 1f * deltax / (sliderTex.Width - 8);
						ratio = Math.Min(Math.Max(ratio, 0f), 1f);
						codeSet(ratio);
					}
					else dragging = null;
				}
			};
			Action<string, string, Vector2, Texture2D, int, int, int, Func<int, string>, Action<int>> drawSliderInt = (name, tip, pos, sliderTex, value, vmin, vmax, textBuilder, codeSet) =>
			{
				float ratio = 1f * (value - vmin) / (vmax - vmin);
				sb.Draw(sliderTex, new Vector2(pos.X, pos.Y + 20), Color.White);
				sb.Draw(Main.colorSliderTexture, new Vector2(pos.X + 4 + (sliderTex.Width - 8) * ratio, pos.Y + 20 + sliderTex.Height / 2), null, Color.White, 0f, Main.colorSliderTexture.Size() * .5f, 1f, SpriteEffects.None, 0f);
				Drawing.StringShadowed(sb, Main.fontMouseText, tip, pos, Color.White, .8f);
				string valtext = textBuilder(value);
				float valscale = .8f;
				if (Main.fontMouseText.MeasureString(valtext).X * valscale > sliderTex.Width / 2) valscale = (sliderTex.Width / 2) / Main.fontMouseText.MeasureString(valtext).X;
				Drawing.StringShadowed(sb, Main.fontMouseText, valtext, new Vector2((float)Math.Round(pos.X + sliderTex.Width - Main.fontMouseText.MeasureString(textBuilder(value)).X * valscale), pos.Y), Color.White, valscale);
				if (dragging == name || (dragging == null && Math2.InRegion(Main.mouse, new Vector2(pos.X, pos.Y + 20), sliderTex.Width, sliderTex.Height)))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft)
					{
						dragging = name;
						int deltax = Main.mouseX - ((int)pos.X + 4);
						ratio = 1f * deltax / (sliderTex.Width - 8);
						ratio = Math.Min(Math.Max(ratio, 0f), 1f);
						codeSet((int)Math.Round(vmin + (vmax - vmin) * ratio));
					}
					else dragging = null;
				}
			};
			Action<Vector2, bool, Action<bool>, Func<bool, string>, Action<Vector2, bool>> drawButton = (pos, value, codeSet, textBuilder, codeDraw) =>
			{
				Drawing.DrawBox(sb, pos.X, pos.Y, 32, 32);
				if (Math2.InRegion(Main.mouse, new Vector2(pos.X, pos.Y), 32, 32))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						value = !value;
						codeSet(value);
					}
					SBase.tip = textBuilder(value);
				}
				codeDraw(pos, value);
			};

			drawSliderFloat("TimeSlider", "Current time", new Vector2(POS_X, POS_Y), MBase.me.textures["Images/TimeSlider"], 1f * FixAbsoluteTime(GetAbsoluteTime() - 43200) / 86400,
			(ratio) => { return GetTimeText(false, GetAbsoluteTime()); },
			(ratio) => { SetAbsoluteTime((int)(Math.Min(Math.Max(ratio, 0f), 1f) * 86400) - 43200); QueueTimeUpdate(); });

			bLockDayTime.pos = new Vector2(POS_X + 186, POS_Y + 8);
			bLockDayTime.size = new Vector2(24, 24);
			blocked = bLockDayTime.Draw(sb, true, !blocked && dragging == null) || blocked;

			if (mw.lockDayTime.HasValue)
			{
				bLockDayTimeSave.pos = new Vector2(POS_X + 214, POS_Y + 8);
				bLockDayTimeSave.size = new Vector2(24, 24);
				blocked = bLockDayTimeSave.Draw(sb, true, !blocked && dragging == null) || blocked;
			}

			drawSliderInt("TimeRateSlider", "Time rate", new Vector2(POS_X, POS_Y + 40), MBase.me.textures["Images/TimeRateSlider"], (int)Math.Ceiling(Math.Pow(Main.dayRate, 1f / 1.3545f)), 0, 50,
			(value) => { return "" + (int)Math.Pow(value, 1.3545f); },
			(value) => { Main.dayRate = (int)Math.Pow(value, 1.3545f); if (mw.lockDayRate.HasValue) mw.lockDayRate = Main.dayRate; QueueTimeUpdate(); });

			bLockDayRate.pos = new Vector2(POS_X + 186, POS_Y + 48);
			bLockDayRate.size = new Vector2(24, 24);
			blocked = bLockDayRate.Draw(sb, true, !blocked && dragging == null) || blocked;

			drawSliderInt("MoonPhase", "Moon phase", new Vector2(POS_X, POS_Y + 80), Main.colorBarTexture, Main.moonPhase, 0, 8,
			(value) => { return MOON_PHASES[value]; },
			(value) => { Main.moonPhase = value % 8; QueueTimeUpdate(); });

			bHardmode.pos = new Vector2(POS_X, POS_Y + 124);
			bHardmode.size = new Vector2(32, 32);
			blocked = bHardmode.Draw(sb, true, !blocked && dragging == null) || blocked;

			bBloodMoon.pos = new Vector2(POS_X + 40, POS_Y + 124);
			bBloodMoon.size = new Vector2(32, 32);
			blocked = bBloodMoon.Draw(sb, true, !blocked && dragging == null) || blocked;

			bEclipse.pos = new Vector2(POS_X + 80, POS_Y + 124);
			bEclipse.size = new Vector2(32, 32);
			blocked = bEclipse.Draw(sb, true, !blocked && dragging == null) || blocked;

			drawSliderInt("PlayerLifeMax", "Max life", new Vector2(POS_X + 244, POS_Y), MBase.me.textures["Images/LifeMaxSlider"], Main.localPlayer.statLifeMax / 5, 1, 100,
			(value) => { return "" + (value * 5); },
			(value) => { Main.localPlayer.statLifeMax = value * 5; });

			drawSliderInt("PlayerLife", "Life", new Vector2(POS_X + 244, POS_Y + 40), MBase.me.textures["Images/LifeSlider"], Main.localPlayer.statLife, 1, Main.localPlayer.statLifeMax2,
			(value) => { return "" + value; },
			(value) => { Main.localPlayer.statLife = value; });

			drawSliderInt("PlayerManaMax", "Max mana", new Vector2(POS_X + 244, POS_Y + 80), MBase.me.textures["Images/ManaSlider"], Main.localPlayer.statManaMax / 20, 0, 10,
			(value) => { return "" + (value * 20); },
			(value) => { Main.localPlayer.statManaMax = value * 20; });

			drawSliderInt("PlayerMana", "Mana", new Vector2(POS_X + 244, POS_Y + 120), MBase.me.textures["Images/ManaSlider"], Main.localPlayer.statMana, 1, Main.localPlayer.statManaMax2,
			(value) => { return "" + value; },
			(value) => { Main.localPlayer.statMana = value; });

			bGodmode.pos = new Vector2(POS_X + 434, POS_Y + 4);
			bGodmode.size = new Vector2(32, 32);
			blocked = bGodmode.Draw(sb, true, !blocked && dragging == null) || blocked;
			
			bNoclip.pos = new Vector2(POS_X + 474, POS_Y + 4);
			bNoclip.size = new Vector2(32, 32);
			blocked = bNoclip.Draw(sb, true, !blocked && dragging == null) || blocked;

			bUsage.pos = new Vector2(POS_X + 514, POS_Y + 4);
			bUsage.size = new Vector2(32, 32);
			blocked = bUsage.Draw(sb, true, !blocked && dragging == null) || blocked;

			bRange.pos = new Vector2(POS_X + 434, POS_Y + 44);
			bRange.size = new Vector2(32, 32);
			blocked = bRange.Draw(sb, true, !blocked && dragging == null) || blocked;

			bTileSpeed.pos = new Vector2(POS_X + 474, POS_Y + 44);
			bTileSpeed.size = new Vector2(32, 32);
			blocked = bTileSpeed.Draw(sb, true, !blocked && dragging == null) || blocked;

			bTileUsage.pos = new Vector2(POS_X + 514, POS_Y + 44);
			bTileUsage.size = new Vector2(32, 32);
			blocked = bTileUsage.Draw(sb, true, !blocked && dragging == null) || blocked;

			bCamera.pos = new Vector2(POS_X + 434, POS_Y + 84);
			bCamera.size = new Vector2(32, 32);
			blocked = bCamera.Draw(sb, true, !blocked && dragging == null) || blocked;

			bFullBright.pos = new Vector2(POS_X + 474, POS_Y + 84);
			bFullBright.size = new Vector2(32, 32);
			blocked = bFullBright.Draw(sb, true, !blocked && dragging == null) || blocked;

			/*bFlashlight.pos = new Vector2(POS_X + 514, POS_Y + 84);
			bFlashlight.size = new Vector2(32, 32);
			blocked = bFlashlight.Draw(sb, true, !blocked && dragging == null) || blocked;*/

			bBlockSpawns.pos = new Vector2(POS_X + 434, POS_Y + 124);
			bBlockSpawns.size = new Vector2(32, 32);
			blocked = bBlockSpawns.Draw(sb, true, !blocked && dragging == null) || blocked;

			if (mw.blockNPCSpawn)
			{
				bBlockSpawnsSave.pos = new Vector2(POS_X + 474, POS_Y + 124);
				bBlockSpawnsSave.size = new Vector2(32, 32);
				blocked = bBlockSpawnsSave.Draw(sb, true, !blocked && dragging == null) || blocked;
			}
		}
	}
}