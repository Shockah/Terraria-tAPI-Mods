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
		public MWorld(ModBase modBase) : base(modBase) { }

		public override void PlayerConnected(int index)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player p = Main.player[i];
				if (!p.active) continue;
				MPlayer m = p.GetSubClass<MPlayer>();
				if (m != null && (m.cheatGod || m.cheatNoclip)) list.Add(i);
			}
			
			BinBuffer bb = new BinBuffer();
			bb.Write((byte)list.Count);
			foreach (int pid in list)
			{
				bb.Write((byte)pid);
				MPlayer m = Main.player[pid].GetSubClass<MPlayer>();
				bb.Write(new BitsByte(m.cheatGod, m.cheatNoclip));
			}

			bb.Pos = 0;
			NetMessage.SendModData(MBase.me, MBase.MSG_CHEAT, index, -1, bb);
		}
	}
}