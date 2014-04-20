using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.ItemRanks
{
	[GlobalMod] public class MItem : ModItem
	{
		public static bool CanGetRanks(Item item)
		{
			return !item.IsBlank() && (Prefix.CanHavePrefix(item) || item.bodySlot != -1 || item.headSlot != -1 || item.legSlot != -1) && (!item.accessory || item.defense != 0) && item.maxStack == 1;
		}
		public static int RandomRank()
		{
			float f = 1;
			WeightedRandom<int> wr = new WeightedRandom<int>();
			for (int i = 0; i <= 10; i++)
			{
				wr.Add(10 - i, f);
				f *= 2.5f;
			}
			return wr.Get();
		}
		public static float GetRankModifier(int rank)
		{
			return 1f + (rank * .05f);
		}
		
		public int rank = 0;
		
		public MItem(ModBase modBase, Item item) : base(modBase, item) { }

		public bool CanGetRanks()
		{
			return CanGetRanks(item);
		}
		public void SetRandomRank()
		{
			rank = RandomRank();
		}
		public void SetupRank()
		{
			if (rank == 0) return;
			float mod = GetRankModifier(rank);

			item.damage = (int)(item.damage * mod);
			item.defense = (int)((item.defense + rank / 2) * mod);
			item.crit = (int)(item.crit * mod);
			item.knockBack *= mod;
		}

		public override void Save(BinBuffer bb)
		{
			if (!CanGetRanks()) return;
			bb.Write((byte)rank);
		}
		public override void Load(BinBuffer bb)
		{
			if (!CanGetRanks()) return;
			rank = bb.ReadByte();
			SetupRank();
		}

		public override void OnCraft(Recipe recipe)
		{
			if (!CanGetRanks()) return;
			SetRandomRank();
			SetupRank();
		}

		public override string OnAffixName(string currentName, string oldName)
		{
			if (rank > 0) currentName += " +" + rank;
			return currentName;
		}
	}
}
