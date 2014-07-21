using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class MBase : ModBase
	{
		public const int
			MSG_SPAWN_NPCS = 1,
			MSG_CHEAT = 2,
			MSG_TIME = 3;
		
		public static ModBase me { get; private set; }
		
		public override void OnLoad()
		{
			me = this;
		}

		public override void OnAllModsLoaded()
		{
			InterfaceFCMItems.Reset();
			InterfaceFCMNPCs.Reset();
			InterfaceFCMPrefixes.Reset();
			InterfaceFCMBuffs.Reset();
			InterfaceFCMMisc.Reset();

			new InterfaceFCMItems();
			new InterfaceFCMNPCs();
			new InterfaceFCMPrefixes();
			new InterfaceFCMBuffs();
			new InterfaceFCMMisc();

			FrameFCMButtons.EventCreatingButtonList += (list) =>
			{
				list.Add(new LittleButton("Items", textures["Images/ModuleItems.png"], () => Interface.current is InterfaceFCMItems, () => InterfaceFCMItems.me.Open(), 0f));
				list.Add(new LittleButton("NPCs", textures["Images/ModuleNPCs.png"], () => Interface.current is InterfaceFCMNPCs, () => InterfaceFCMNPCs.me.Open(), -1f));
				list.Add(new LittleButton("Prefixes", textures["Images/ModulePrefixes.png"], () => Interface.current is InterfaceFCMPrefixes, () => InterfaceFCMPrefixes.me.Open(), -2f));
				list.Add(new LittleButton("Buffs", textures["Images/ModuleBuffs.png"], () => Interface.current is InterfaceFCMBuffs, () => InterfaceFCMBuffs.me.Open(), -3f));
				list.Add(new LittleButton("Misc", textures["Images/ModuleMisc.png"], () => Interface.current is InterfaceFCMMisc, () => InterfaceFCMMisc.me.Open(), -4f));
			};
		}

		public override void PreGameDraw(SpriteBatch sb)
		{
			if (!Main.gameMenu)
			{
				MPlayer mp = (MPlayer)Main.localPlayer.GetSubClass<MPlayer>();
				if (mp.lastCameraPos.X != -1 || mp.lastCameraPos.Y != -1) Main.screenPosition = mp.lastCameraPos;

				if (InterfaceFCMMisc.fullBright)
				{
					for (int i = 0; i < Lighting.color.GetLength(0); i++) for (int j = 0; j < Lighting.color.GetLength(1); j++)
					{
						Lighting.color[i, j] = 1f;
						Lighting.colorG[i, j] = 1f;
						Lighting.colorB[i, j] = 1f;
						Lighting.color2[i, j] = 1f;
						Lighting.colorG2[i, j] = 1f;
						Lighting.colorB2[i, j] = 1f;
					}
					Main.renderNow = false;
				}
				if (InterfaceFCMMisc.flashlight)
				{
					if (Util.KeyPressed(Keys.Tab)) InterfaceFCMMisc.flashlightOff = !InterfaceFCMMisc.flashlightOff;
					if (!InterfaceFCMMisc.flashlightOff) Lighting.AddLight(Main.screenPosition + Main.mouse, 50f, 50f, 50f);
				}
			}
		}

		public override void NetReceive(int msgType, BinBuffer bb)
		{
			int ignore;
			BinBuffer copybb;
			
			switch (msgType)
			{
				case MSG_SPAWN_NPCS:
					if (Main.netMode != 2) return;
					InterfaceFCMNPCs.SpawnNPCs(bb.ReadShort(), bb.ReadInt(), bb.ReadInt(), bb.ReadFloat(), bb.ReadUShort(), bb.ReadInt());
					break;
				case MSG_CHEAT:
					ignore = 0;
					copybb = null;
					if (Main.netMode == 2)
					{
						ignore = bb.ReadByte();
						copybb = SBase.CopyFurther(bb);
					}
					
					int count = bb.ReadByte();
					while (count-- > 0)
					{
						int pid = bb.ReadByte();
						BitsByte bbyte = bb.ReadByte();
						MPlayer m = Main.player[pid].GetSubClass<MPlayer>();
						if (m != null) bbyte.Retrieve(ref m.cheatGod, ref m.cheatNoclip, ref m.cheatUsage, ref m.cheatRange, ref m.cheatTileSpeed, ref m.cheatTileUsage);
					}

					if (Main.netMode == 2) NetMessage.SendModData(this, MSG_CHEAT, -1, ignore, copybb);
					break;
				case MSG_TIME:
					ignore = 0;
					copybb = null;
					if (Main.netMode == 2)
					{
						ignore = bb.ReadByte();
						copybb = SBase.CopyFurther(bb);
					}

					Main.dayTime = bb.ReadBool();
					Main.time = bb.ReadFloat();
					Main.dayRate = bb.ReadUShort();
					Main.moonPhase = bb.ReadByte();
					((BitsByte)bb.ReadByte()).Retrieve(ref Main.hardMode, ref Main.bloodMoon, ref Main.eclipse);

					MWorld mw = (MWorld)modWorld;
					mw.blockNPCSpawn = bb.ReadBool();
					mw.blockNPCSpawnSave = bb.ReadBool();
					mw.lockDayTime = null;
					if (bb.ReadBool()) mw.lockDayTime = bb.ReadBool();
					mw.lockDayTimeSave = bb.ReadBool();
					mw.lockDayRate = bb.ReadBool() ? new int?(Main.dayRate) : null;

					if (Main.netMode == 2) NetMessage.SendModData(this, MSG_TIME, -1, ignore, copybb);
					break;
				default: break;
			}
		}

		public override bool TamperSend(BinBuffer bb, int msgId, int remoteClient, int ignoreClient, string text, int arg1, float arg2, float arg3, float arg4, float arg5)
		{
			if (msgId == 23 && InterfaceFCMNPCs.fakeUpdating) return false;
			return true;
		}
	}
}