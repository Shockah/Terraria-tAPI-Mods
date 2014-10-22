using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;
using Terraria.DataStructures;

namespace Shockah.FCM.Standard
{
	public class MWorld : ModWorld
	{
		public bool? lockDayTime = null;
		public int? lockDayRate = null;
		public bool lockDayTimeSave = false;
		public bool blockNPCSpawn = false;
		public bool blockNPCSpawnSave = false;
		public bool? lockChristmas = null;
		public bool lockChristmasSave = false;
		public bool? lockHalloween = null;
		public bool lockHalloweenSave = false;
		
		public override void Initialize()
		{
			lockDayTime = null;
			lockDayRate = null;
			lockChristmas = null;
			lockHalloween = null;
			lockDayTimeSave = false;
			blockNPCSpawn = false;
			blockNPCSpawnSave = false;
			lockChristmasSave = false;
			lockHalloweenSave = false;
			SetupWorld();
		}

		public override void Load(BinBuffer bb)
		{
			try
			{
				if (bb.ReadBool())
				{
					lockDayTimeSave = true;
					lockDayTime = bb.ReadBool();
				}
				if (bb.ReadBool()) lockDayRate = bb.ReadInt();
				if (bb.ReadBool()) blockNPCSpawnSave = blockNPCSpawn = true;
				if (bb.ReadBool())
				{
					lockChristmasSave = true;
					lockChristmas = bb.ReadBool();
				}
				if (bb.ReadBool())
				{
					lockHalloweenSave = true;
					lockHalloween = bb.ReadBool();
				}
			}
			catch (Exception) { }
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

			if (lockChristmasSave && lockChristmas.HasValue)
			{
				bb.Write(true);
				bb.Write(lockChristmas.Value);
			}
			else bb.Write(false);

			if (lockHalloweenSave && lockHalloween.HasValue)
			{
				bb.Write(true);
				bb.Write(lockHalloween.Value);
			}
			else bb.Write(false);
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

			if (lockChristmas.HasValue)
			{
				Main.xMas = lockChristmas.Value;
			}
			if (lockHalloween.HasValue)
			{
				Main.halloween = lockHalloween.Value;
			}
		}

		public override void PostUpdate()
		{
			if (!Main.dedServ)
			{
				if (!(UICore.currentInterface is InterfaceFCMNPCs) && InterfaceFCMNPCs.spawning != null)
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

		public override bool? CheckChristmas()
		{
			return lockChristmas;
		}
		public override bool? CheckHalloween()
		{
			return lockHalloween;
		}
	}
}