using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.CompanionBag
{
	public class MPlayer : ModPlayer
	{
		public List<Item> companions = new List<Item>();
		
		public MPlayer(ModBase modBase, Player player) : base(modBase, player) { }

		public override void Initialize()
		{
			companions.Clear();
			companions.Add(new Item());
			InterfaceCompanionBag.needUpdate = true;
		}

		public override void Save(BinBuffer bb)
		{
			bb.Write((byte)(companions.Count - 1));
			foreach (Item item in companions) if (!item.IsBlank()) bb.Write(item.name);
		}

		public override void Load(BinBuffer bb)
		{
			if (bb.BytesLeft() == 0) return;

			try
			{
				companions.Clear();
				int count = bb.ReadByte();
				while (count-- > 0) companions.Add(new Item().SetDefaults(bb.ReadString()));
				companions.Add(new Item());
				InterfaceCompanionBag.needUpdate = true;
			}
			catch (Exception)
			{ }
		}
	}
}