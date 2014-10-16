using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.AccSlots
{
	public class MNet : ModNet
	{
		public const int
			MSG_SYNCPLAYERS = 1,
			MSG_REQUEST = 2,
			MSG_UPDATESLOT = 3,
			MSG_UPDATEVISIBILITY = 4;
		
		public override void PlayerConnected(int index)
		{
			if (Main.netMode == 2)
			{
				NetMessage.SendModData(modBase, MSG_REQUEST, index, -1);
				
				int count = 0;
				BinBuffer bb2 = new BinBuffer();
				for (int i = 0; i < Main.player.Length; i++)
				{
					Player player = Main.player[i];
					if (!player.active) continue;

					bb2.Write((byte)i);
					MPlayer mp = player.GetSubClass<MPlayer>();
					mp.Save(bb2);
					count++;
				}

				bb2.Pos = 0;
				BinBuffer bb = new BinBuffer();
				bb.Write((byte)count);
				bb.Write(bb2);

				bb.Pos = 0;
				NetMessage.SendModData(modBase, MSG_SYNCPLAYERS, index, -1, bb);
			}
		}
		
		public override void NetReceive(BinBuffer bb, int messageID, MessageBuffer buffer)
		{
			if (Main.netMode == 2)
			{
				switch (messageID)
				{
					case MSG_SYNCPLAYERS:
					{
						int count = bb.ReadByte();
						while (count-- > 0)
						{
							int id = bb.ReadByte();
							Player player = Main.player[id];
							MPlayer mp = player.GetSubClass<MPlayer>();
							mp.Load(bb);
						}
						break;
					}
					case MSG_UPDATESLOT:
					{
						BinBuffer bb2 = new BinBuffer();
						bb2.Write((byte)buffer.whoAmI);
						int cachePos = bb.Pos;
						bb2.Write(bb);
						bb.Pos = cachePos;

						int slotMode = bb.ReadByte();
						int slotIndex = bb.ReadByte();
						Main.player[buffer.whoAmI].GetSubClass<MPlayer>().extraSlots[slotMode][slotIndex] = bb.ReadItem();

						bb2.Pos = 0;
						NetMessage.SendModData(modBase, MSG_UPDATESLOT, -1, buffer.whoAmI, bb2);
						break;
					}
					case MSG_UPDATEVISIBILITY:
					{
						BinBuffer bb2 = new BinBuffer();
						bb2.Write((byte)buffer.whoAmI);
						int cachePos = bb.Pos;
						bb2.Write(bb);
						bb.Pos = cachePos;

						int slotIndex = bb.ReadByte();
						Main.player[buffer.whoAmI].GetSubClass<MPlayer>().visibility[slotIndex] = bb.ReadBool();

						bb2.Pos = 0;
						NetMessage.SendModData(modBase, MSG_UPDATEVISIBILITY, -1, buffer.whoAmI, bb2);
						break;
					}
				}
			}
			else
			{
				switch (messageID)
				{
					case MSG_SYNCPLAYERS:
						int count = bb.ReadByte();
						while (count-- > 0)
						{
							int id = bb.ReadByte();
							Player player = Main.player[id];
							MPlayer mp = player.GetSubClass<MPlayer>();
							mp.Load(bb);
						}
						break;
					case MSG_REQUEST:
					{
						bb = new BinBuffer();
						bb.Write((byte)1);
						bb.Write((byte)Main.myPlayer);
						Main.localPlayer.GetSubClass<MPlayer>().Save(bb);
						break;
					}
					case MSG_UPDATESLOT:
					{
						int playerID = bb.ReadByte();
						int slotMode = bb.ReadByte();
						int slotIndex = bb.ReadByte();
						Main.player[playerID].GetSubClass<MPlayer>().extraSlots[slotMode][slotIndex] = bb.ReadItem();
						break;
					}
					case MSG_UPDATEVISIBILITY:
					{
						int playerID = bb.ReadByte();
						int slotIndex = bb.ReadByte();
						Main.player[playerID].GetSubClass<MPlayer>().visibility[slotIndex] = bb.ReadBool();
						break;
					}
				}
			}
		}
	}
}