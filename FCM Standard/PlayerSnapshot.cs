using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;
using Terraria.DataStructures;
using Shockah.Base;
using FluentPath;

namespace Shockah.FCM.Standard
{
	public class PlayerSnapshot
	{
		public const int FORMAT = 1;

		public static PlayerSnapshot Create(string name)
		{
			return Create(name, DateTime.Now);
		}
		public static PlayerSnapshot Create(string name, DateTime date)
		{
			PlayerSnapshot temp = new PlayerSnapshot(null);
			temp.player = Main.localPlayer;
			BinBuffer bb = new BinBuffer();
			temp.Save(bb);

			PlayerSnapshot snap = new PlayerSnapshot(name, date);
			bb.Pos = 0;
			snap.Load(bb);
			return snap;
		}

		public static void Restore(Player player, PlayerSnapshot snap)
		{
			PlayerSnapshot temp = new PlayerSnapshot(null);
			temp.player = player;
			BinBuffer bb = new BinBuffer();
			snap.Save(bb);

			bb.Pos = 0;
			temp.Load(bb);
		}
		
		public string name;
		public DateTime date;
		public Player player = new Player();

		public PlayerSnapshot(string name) : this(name, DateTime.Now) { }
		public PlayerSnapshot(string name, DateTime date)
		{
			this.name = name;
			this.date = date;
		}

		public void Load(BinBuffer bb)
		{
			/*int format = */bb.ReadByte();
			
			#region basic
			player.statLife = bb.ReadInt();
			player.statLifeMax = bb.ReadInt();
			player.statMana = bb.ReadInt();
			player.statManaMax = bb.ReadInt();
			player.hideVisual = bb.ReadByte();
			#endregion

			#region inventory
			for (int i = 0; i < 16; i++) player.armor[i] = bb.ReadItem(true);
			for (int i = 0; i < 8; i++) player.dye[i] = bb.ReadItem(true);
			for (int i = 0; i < 58; i++) player.inventory[i] = bb.ReadItem(true);
			for (int i = 0; i < Chest.maxItems; i++) player.bank.item[i] = bb.ReadItem(true);
			for (int i = 0; i < Chest.maxItems; i++) player.bank2.item[i] = bb.ReadItem(true);
			#endregion

			#region buffs
			player.maxBuffs = Player.defaultMaxBuffs;
			player.buffType = new int[Player.defaultMaxBuffs];
			player.buffTime = new int[Player.defaultMaxBuffs];
			player.buffCode = new ModBuff[Player.defaultMaxBuffs];
			int count = bb.ReadUShort();
			if (player.buffType.Length != count)
			{
				player.maxBuffs = count;
				player.buffType = new int[player.maxBuffs];
			}
			for (int i = 0; i < player.maxBuffs; i++)
			{
				int buffType = bb.ReadUShort();
				if (buffType == UInt16.MaxValue)
				{
					string buffName = bb.ReadString();
					if (BuffDef.byName.ContainsKey(buffName)) buffType = BuffDef.byName[buffName];
					else
					{
						//clean out the extra data since this buff is unloaded
						player.buffType[i] = 0;
						player.buffTime[i] = 0;
						int dummyTime = bb.ReadUShort();
						int dummyBytes = bb.ReadUShort();
						byte[] dummyByteArray = (dummyBytes == 0 ? new byte[0] : bb.ReadBytes(dummyBytes));
						continue;
					}
				}
				player.buffType[i] = buffType;
				player.buffTime[i] = bb.ReadUShort();
				if (BuffDef.buffs.ContainsKey(player.buffType[i]) && BuffDef.buffs[player.buffType[i]].modBuffType != null)
				{
					player.buffCode[i] = (ModBuff)Activator.CreateInstance(BuffDef.buffs[player.buffType[i]].modBuffType);
					player.buffCode[i].modBase = BuffDef.buffs[player.buffType[i]].modBase;
				}
				int codeSize = bb.ReadUShort();
				if (codeSize > 0)
				{
					BinBuffer bb2 = new BinBuffer(new BinBufferByte(codeSize));
					bb2.Write(bb.ReadBytes(codeSize));
					bb2.Pos = 0;
					if (player.buffCode[i] != null) { player.buffCode[i].Load(player, bb2); }
				}
			}
			if (player.maxBuffs <= 0)
			{
				player.maxBuffs = Player.defaultMaxBuffs;
				player.buffType = new int[Player.defaultMaxBuffs];
				player.buffTime = new int[Player.defaultMaxBuffs];
				player.buffCode = new ModBuff[Player.defaultMaxBuffs];
			}
			#endregion

			#region mod data
			player.SetupModEntities();
			int modsCount = bb.ReadInt();
			while (modsCount-- > 0)
			{
				string modName = bb.ReadString();
				int bytes = bb.ReadInt();

				foreach (ModPlayer mp in player.modEntities)
				{
					if (mp.modBase.mod.InternalName == modName)
					{
						BinBuffer bb2 = new BinBuffer();
						bb2.Write(bb.ReadBytes(bytes));
						bb2.Pos = 0;
						mp.Load(bb2);
						break;
					}
				}
			}
			#endregion

			for (int i = 3; i < 8; i++)
			{
				int type = player.armor[i].type;
				if (type == 908) player.lavaMax += 420;
				if (type == 906) player.lavaMax += 420;
				if (player.wingsLogic == 0 && player.armor[i].wingSlot >= 0) player.wingsLogic = player.armor[i].wingSlot;
				if (type == 158 || type == 396 || type == 1250 || type == 1251 || type == 1252) player.noFallDmg = true;
				player.lavaTime = player.lavaMax;
			}

			player.PlayerFrame();
		}

		public void Save(BinBuffer bb)
		{
			bb.Write((byte)FORMAT);
			BinBuffer bb2;

			#region basic
			bb.WriteX(player.statLife, player.statLifeMax);
			bb.WriteX(player.statMana, player.statManaMax);
			bb.Write(player.hideVisual);
			#endregion

			#region inventory
			for (int i = 0; i < 16; i++) bb.Write(player.armor[i], true);
			for (int i = 0; i < 8; i++) bb.Write(player.dye[i], true);
			for (int i = 0; i < 58; i++) bb.Write(player.inventory[i], true);
			for (int i = 0; i < Chest.maxItems; i++) bb.Write(player.bank.item[i], true);
			for (int i = 0; i < Chest.maxItems; i++) bb.Write(player.bank2.item[i], true);
			#endregion

			#region buffs
			bb.Write((ushort)player.maxBuffs);
			for (int i = 0; i < player.maxBuffs; i++)
			{
				if (Main.buffNoSave[player.buffType[i]])
				{
					bb.Write((ushort)0);
					bb.Write((ushort)0);
					bb.Write((ushort)0);
				}
				else
				{
					if (player.buffType[i] < Main.maxBuffTypes)
					{
						bb.Write((ushort)player.buffType[i]);
					}
					else
					{
						bb.Write(UInt16.MaxValue);
						bb.Write(BuffDef.byType[player.buffType[i]]);
					}
					bb.Write((ushort)player.buffTime[i]);
					if (player.buffCode[i] != null)
					{
						bb2 = new BinBuffer();
						player.buffCode[i].Save(player, bb2);
						bb.Write((ushort)bb2.Size);
						if (!bb2.IsEmpty)
						{
							bb2.Pos = 0;
							bb.Write(bb2);
						}
					}
					else bb.Write((ushort)0);
				}
			}
			#endregion

			#region mod data
			int modsCount = 0;
			bb2 = new BinBuffer();
			foreach (ModPlayer mp in player.modEntities)
			{
				BinBuffer bb3 = new BinBuffer();
				mp.Save(bb3);
				if (bb3.Size != 0)
				{
					modsCount++;
					bb2.Write(mp.modBase.mod.InternalName);
					bb2.Write(bb3.Size);
					bb3.Pos = 0;
					bb2.Write(bb3);
				}
			}
			bb.Write(modsCount);
			bb2.Pos = 0;
			bb.Write(bb2);
			#endregion
		}

		public void Draw(Vector2 pos)
		{
			bool copyfullbright = Lighting.fullBright;
			Lighting.fullBright = true;
			Vector2 copypos = player.position;
			player.position = pos + Main.screenPosition;
			player.DrawFull();
			player.position = copypos;
			Lighting.fullBright = copyfullbright;
		}
	}
}