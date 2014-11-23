using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;
using Terraria.DataStructures;

namespace Shockah.FCM.Standard
{
	public class MNet : ModNet
	{
		public override void PlayerConnected(int index)
		{
			InterfaceFCMMisc.SendTimeUpdate(index, -1);

			List<int> list = new List<int>();
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player p = Main.player[i];
				if (!p.active) continue;
				MPlayer m = p.GetSubClass<MPlayer>();
				if (m != null && (m.cheatGod || m.cheatNoclip || m.cheatUsage || m.cheatRange || m.cheatTileSpeed || m.cheatTileUsage)) list.Add(i);
			}

			BinBuffer bb = new BinBuffer();
			bb.Write((byte)list.Count);
			foreach (int pid in list)
			{
				bb.Write((byte)pid);
				MPlayer m = Main.player[pid].GetSubClass<MPlayer>();
				bb.Write(new BitsByte(m.cheatGod, m.cheatNoclip, m.cheatUsage, m.cheatRange, m.cheatTileSpeed, m.cheatTileUsage));
			}

			bb.Pos = 0;
			NetMessage.SendModData(this, MBase.MSG_CHEAT, index, -1, bb);
		}
		
		public override void NetReceive(BinBuffer bb, int msgType, MessageBuffer buffer)
		{
			BinBuffer copybb;

			switch (msgType)
			{
				case MBase.MSG_SPAWN_NPCS:
					if (Main.netMode != 2) return;
					InterfaceFCMNPCs.SpawnNPCs(bb.ReadShort(), bb.ReadInt(), bb.ReadInt(), bb.ReadFloat(), bb.ReadUShort(), bb.ReadInt());
					break;
				case MBase.MSG_CHEAT:
					copybb = null;
					if (Main.netMode == 2)
						copybb = SBase.CopyFurther(bb);

					int count = bb.ReadByte();
					while (count-- > 0)
					{
						int pid = bb.ReadByte();
						BitsByte bbyte = bb.ReadByte();
						MPlayer m = Main.player[pid].GetSubClass<MPlayer>();
						if (m != null) bbyte.Retrieve(ref m.cheatGod, ref m.cheatNoclip, ref m.cheatUsage, ref m.cheatRange, ref m.cheatTileSpeed, ref m.cheatTileUsage);
					}

					if (Main.netMode == 2)
						NetMessage.SendModData(this, MBase.MSG_CHEAT, -1, buffer.whoAmI, copybb);
					break;
				case MBase.MSG_TIME:
					copybb = null;
					if (Main.netMode == 2)
						copybb = SBase.CopyFurther(bb);

					Main.dayTime = bb.ReadBool();
					Main.time = bb.ReadFloat();
					Main.dayRate = bb.ReadUShort();
					Main.moonPhase = bb.ReadByte();
					((BitsByte)bb.ReadByte()).Retrieve(ref Main.hardMode, ref Main.bloodMoon, ref Main.eclipse);

					MWorld mw = (MWorld)MBase.me.modWorld;
					mw.blockNPCSpawn = bb.ReadBool();
					mw.blockNPCSpawnSave = bb.ReadBool();

					mw.lockDayTime = null;
					if (bb.ReadBool()) mw.lockDayTime = bb.ReadBool();
					mw.lockDayTimeSave = bb.ReadBool();

					mw.lockDayRate = bb.ReadBool() ? new int?(Main.dayRate) : null;

					mw.lockChristmas = null;
					if (bb.ReadBool()) mw.lockChristmas = bb.ReadBool();
					mw.lockChristmasSave = bb.ReadBool();

					mw.lockHalloween = null;
					if (bb.ReadBool()) mw.lockHalloween = bb.ReadBool();
					mw.lockHalloweenSave = bb.ReadBool();

					Main.checkXMas();
					Main.checkHalloween();

					if (Main.netMode == 2)
						NetMessage.SendModData(this, MBase.MSG_TIME, -1, buffer.whoAmI, copybb);
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