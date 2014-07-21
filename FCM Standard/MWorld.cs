using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class MWorld : ModWorld
	{
		public bool? lockDayTime = null;
		public int? lockDayRate = null;
		public bool lockDayTimeSave = false;
		public bool blockNPCSpawn = false;
		public bool blockNPCSpawnSave = false;
		
		public MWorld(ModBase modBase) : base(modBase) { }

		public override void Initialize()
		{
			lockDayTime = null;
			lockDayRate = null;
			lockDayTimeSave = false;
			blockNPCSpawn = false;
			blockNPCSpawnSave = false;
			SetupWorld();
		}

		public override void Load(BinBuffer bb)
		{
			if (bb.ReadBool())
			{
				lockDayTimeSave = true;
				lockDayTime = bb.ReadBool();
			}
			if (bb.ReadBool()) lockDayRate = bb.ReadInt();
			if (bb.ReadBool()) blockNPCSpawnSave = blockNPCSpawn = true;
			SetupWorld();
		}
		public override void Save(BinBuffer bb)
		{
			if (lockDayTimeSave && lockDayTime.HasValue)
			{
				bb.Write(true);
				bb.Write(lockDayTime.Value);
			}
			else bb.Write(false);

			bb.Write(lockDayRate.HasValue);
			if (lockDayRate.HasValue) bb.Write(lockDayRate.Value);
			bb.Write(blockNPCSpawnSave && blockNPCSpawnSave);
		}

		public void SetupWorld()
		{
			Main.dayRate = 1;
			
			if (lockDayTime.HasValue)
			{
				if (lockDayTime.Value != Main.dayTime)
				{
					Main.dayTime = !Main.dayTime;
					Main.time = 0;
				}
			}
			if (lockDayRate.HasValue) Main.dayRate = lockDayRate.Value;
		}

		public override void PostUpdate()
		{
			if (!Main.dedServ)
			{
				if (!(Interface.current is InterfaceFCMNPCs) && InterfaceFCMNPCs.spawning != null)
				{
					InterfaceFCMBase.resetInterface = false;
					InterfaceFCMNPCs.spawning = null;
					InterfaceFCMNPCs.me.Open();
				}
			}
			
			if (Main.netMode != 0)
			{
				if (InterfaceFCMMisc.throttleTimeUpdate > 0)
				{
					InterfaceFCMMisc.throttleTimeUpdate--;
					if (InterfaceFCMMisc.throttleTimeUpdate == 0 && InterfaceFCMMisc.timeUpdateSend)
					{
						InterfaceFCMMisc.timeUpdateSend = false;
						InterfaceFCMMisc.SendTimeUpdate(-1, -1);
					}
				}
			}

			if (lockDayTime.HasValue)
			{
				bool lockDay = lockDayTime.Value;
				if (Main.dayTime != lockDay)
				{
					Main.dayTime = lockDay;
					Main.time = 0;
				}
				else
				{
					double nextTime = Main.time + Main.dayRate;
					if (nextTime > (Main.dayTime ? 54000 : 32400))
					{
						Main.time = 0;
						if (!Main.dayTime) Main.moonPhase = (Main.moonPhase + 1) % 8;
					}
				}
			}
		}

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
			NetMessage.SendModData(MBase.me, MBase.MSG_CHEAT, index, -1, bb);
		}
	}
}