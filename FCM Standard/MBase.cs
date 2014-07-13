using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class MBase : ModBase
	{
		public const int
			MSG_SPAWN_NPCS = 1,
			MSG_CHEAT = 2;
		
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

		public override void PostGameDraw(SpriteBatch sb)
		{
			if (!(Interface.current is InterfaceFCMNPCs) && InterfaceFCMNPCs.spawning != null)
			{
				InterfaceFCMBase.resetInterface = false;
				InterfaceFCMNPCs.spawning = null;
				InterfaceFCMNPCs.me.Open();
			}
		}

		public override void NetReceive(int msgType, BinBuffer bb)
		{
			switch (msgType)
			{
				case MSG_SPAWN_NPCS:
					if (Main.netMode != 2) return;
					InterfaceFCMNPCs.SpawnNPCs(bb.ReadShort(), bb.ReadInt(), bb.ReadInt(), bb.ReadFloat(), bb.ReadUShort(), bb.ReadInt());
					break;
				case MSG_CHEAT:
					int ignore = 0;
					BinBuffer copybb = null;
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
						if (m != null) bbyte.Retrieve(ref m.cheatGod, ref m.cheatNoclip);
					}

					if (Main.netMode == 2) NetMessage.SendModData(this, MSG_CHEAT, -1, ignore, copybb);
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