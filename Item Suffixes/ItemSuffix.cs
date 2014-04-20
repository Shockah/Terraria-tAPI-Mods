using System;
using System.Collections.Generic;
using Terraria;

namespace Shockah.ItemSuffixes
{
	public class ItemSuffix
	{
		public static List<ItemSuffix> list = new List<ItemSuffix>();
		public static List<Func<Item, bool>>
			blacklistMelee = new List<Func<Item, bool>>(),
			blacklistRanged = new List<Func<Item, bool>>(),
			blacklistMagic = new List<Func<Item, bool>>(),
			blacklistDefense = new List<Func<Item, bool>>(),
			blacklistThreat = new List<Func<Item, bool>>();

		static ItemSuffix()
		{
			new ItemSuffix(null);
			
			new ItemSuffix("of the Bear").Melee(1).Defense(1);
			new ItemSuffix("of the Gorilla").Melee(1).Threat(1);
			new ItemSuffix("of the Tiger").Melee(1, 1);
			new ItemSuffix("of the Champion").Melee(2);
			new ItemSuffix("of the Mercenary").Melee(0, 2);
			new ItemSuffix("of the Ancestor").Melee(0, 1).Defense(1);
			new ItemSuffix("of the Soldier").Melee(0, 1).Threat(1);

			new ItemSuffix("of the Monkey").Ranged(1).Defense(1);
			new ItemSuffix("of the Wolf").Ranged(1).Threat(1);
			new ItemSuffix("of the Falcon").Ranged(1, 1);
			new ItemSuffix("of the Hunt").Ranged(2);
			new ItemSuffix("of the Marksman").Ranged(0, 2);
			new ItemSuffix("of the Wild").Ranged(0, 1).Defense(1);
			new ItemSuffix("of the Beast").Ranged(0, 1).Threat(1);

			new ItemSuffix("of the Eagle").Magic(1).Defense(1);
			new ItemSuffix("of the Whale").Magic(1).Threat(1);
			new ItemSuffix("of the Owl").Magic(1, 1);
			new ItemSuffix("of the Sorcerer").Magic(2);
			new ItemSuffix("of the Nightmare").Magic(0, 2);
			new ItemSuffix("of the Moon").Magic(0, 1).Defense(1);
			new ItemSuffix("of the Sun").Magic(0, 1).Threat(1);

			new ItemSuffix("of the Knight").Defense(2);
			new ItemSuffix("of the Crusade").Threat(2);
			new ItemSuffix("of the Squire").Defense(1).Threat(1);

			Func<Item, bool> f;

			//Jungle set
			f = (item) => { return item.type >= 228 && item.type <= 230; };
			blacklistMelee.Add(f); blacklistRanged.Add(f);

			//Ancient Cobalt set
			f = (item) => { return item.type >= 960 && item.type <= 962; };
			blacklistMelee.Add(f); blacklistRanged.Add(f);

			//Meteor set
			f = (item) => { return item.type >= 123 && item.type <= 125; };
			blacklistMelee.Add(f); blacklistRanged.Add(f);

			//Necro set
			f = (item) => { return item.type >= 151 && item.type <= 153; };
			blacklistMelee.Add(f); blacklistMagic.Add(f);

			//Shadow set
			f = (item) => { return item.type >= 100 && item.type <= 102; };
			blacklistRanged.Add(f); blacklistMagic.Add(f);

			//Molten set
			f = (item) => { return item.type >= 231 && item.type <= 233; };
			blacklistRanged.Add(f); blacklistMagic.Add(f);

			//Cobalt / Palladium / Mythril / Orichalcum / Adamantite / Titanium / Hallowed / Chlorophyte melee set
			f = (item) => { return new List<int>(new int[] { 372, 1205, 377, 1210, 401, 1215, 559, 1001 }).Contains(item.type); };
			blacklistRanged.Add(f); blacklistMagic.Add(f);

			//Cobalt / Palladium / Mythril / Orichalcum / Adamantite / Titanium / Hallowed / Chlorophyte ranged set
			f = (item) => { return new List<int>(new int[] { 373, 1206, 378, 1211, 402, 1216, 553, 1002 }).Contains(item.type); };
			blacklistMelee.Add(f); blacklistMagic.Add(f);

			//Cobalt / Palladium / Mythril / Orichalcum / Adamantite / Titanium / Hallowed / Chlorophyte magic set
			f = (item) => { return new List<int>(new int[] { 371, 1207, 376, 1212, 400, 1217, 558, 1003 }).Contains(item.type); };
			blacklistMelee.Add(f); blacklistMagic.Add(f);

			//Spooky set
			f = (item) => { return item.type >= 1832 && item.type <= 1834; };
			blacklistMelee.Add(f); blacklistRanged.Add(f);

			//Tiki set
			f = (item) => { return item.type >= 1159 && item.type <= 1161; };
			blacklistMelee.Add(f); blacklistRanged.Add(f);

			//Frost set
			f = (item) => { return item.type >= 684 && item.type <= 686; };
			blacklistMagic.Add(f);

			//Turtle set
			f = (item) => { return item.type >= 1316 && item.type <= 1318; };
			blacklistRanged.Add(f); blacklistMagic.Add(f);

			//Shroomite set
			f = (item) => { return item.type >= 1546 && item.type <= 1550; };
			blacklistMelee.Add(f); blacklistMagic.Add(f);

			//Spectre set
			f = (item) => { return item.type >= 1503 && item.type <= 1505; };
			blacklistMelee.Add(f); blacklistRanged.Add(f);

			//melee accessories
			f = (item) => { return new List<int>(new int[] { 211, 490, 536, 485, 897, 936, 1322, 1343 }).Contains(item.type); };
			blacklistRanged.Add(f); blacklistMagic.Add(f);

			//ranged accessories
			f = (item) => { return new List<int>(new int[] { 491, 1300, 1321, 1858 }).Contains(item.type); };
			blacklistMelee.Add(f); blacklistMagic.Add(f);

			//magic accessories
			f = (item) => { return new List<int>(new int[] { 111, 223, 489, 555, 982, 1158, 1167, 1595, 1845, 1864 }).Contains(item.type); };
			blacklistMelee.Add(f); blacklistRanged.Add(f);
		}

		public static List<ItemSuffix> AllowedSuffixes(Item item)
		{
			List<ItemSuffix> ret = new List<ItemSuffix>();
			foreach (ItemSuffix suffix in list)
			{
				if (suffix.displayName == null) continue;
				if (suffix.damageMelee != 0) foreach (Func<Item, bool> f in blacklistMelee) if (f(item)) goto L;
				if (suffix.damageRanged != 0) foreach (Func<Item, bool> f in blacklistRanged) if (f(item)) goto L;
				if (suffix.damageMagic != 0) foreach (Func<Item, bool> f in blacklistMagic) if (f(item)) goto L;
				if (suffix.defense != 0) foreach (Func<Item, bool> f in blacklistDefense) if (f(item)) goto L;
				if (suffix.threat != 0) foreach (Func<Item, bool> f in blacklistThreat) if (f(item)) goto L;
				ret.Add(suffix);
				L: { }
			}
			return ret;
		}
		public static ItemSuffix RandomAllowedSuffix(Item item)
		{
			List<ItemSuffix> allowed = AllowedSuffixes(item);
			return allowed[Main.rand.Next(allowed.Count)];
		}

		public static int BonusMult(int initial, int times, int min, float scale)
		{
			for (int i = 0; i < times; i++)
			{
				initial = (int)Math.Max(initial + min, initial * scale);
				scale *= scale;
			}
			return initial;
		}
		public static string BonusPowerString(int value)
		{
			switch (value)
			{
				case 1: return "slightly";
				case 2: return "severely";
				default: return null;
			}
		}

		public readonly int id;
		public readonly string displayName;

		public int damageMelee, damageRanged, damageMagic;
		public int critMelee, critRanged, critMagic;
		public int defense, threat;

		private ItemSuffix(string displayName)
		{
			id = list.Count;
			this.displayName = displayName;
			list.Add(this);
		}

		public ItemSuffix Melee(int damage, int crit = 0)
		{
			damageMelee = damage;
			critMelee = crit;
			return this;
		}
		public ItemSuffix Ranged(int damage, int crit = 0)
		{
			damageRanged = damage;
			critRanged = crit;
			return this;
		}
		public ItemSuffix Magic(int damage, int crit = 0)
		{
			damageMagic = damage;
			critMagic = crit;
			return this;
		}
		public ItemSuffix Damage(int damage)
		{
			damageMelee = damageRanged = damageMagic = damage;
			return this;
		}
		public ItemSuffix Crit(int crit)
		{
			critMelee = critRanged = critMagic = crit;
			return this;
		}
		public ItemSuffix Defense(int defense)
		{
			this.defense = defense;
			return this;
		}
		public ItemSuffix Threat(int threat)
		{
			this.threat = threat;
			return this;
		}

		public int BonusDamageMelee(int initial) { return BonusMult(initial, damageMelee, 1, 1.05f); }
		public int BonusDamageRanged(int initial) { return BonusMult(initial, damageRanged, 1, 1.05f); }
		public int BonusDamageMagic(int initial) { return BonusMult(initial, damageMagic, 1, 1.05f); }
		public int BonusCritMelee(int initial) { return BonusMult(initial, critMelee, 2, 1.1f); }
		public int BonusCritRanged(int initial) { return BonusMult(initial, critRanged, 2, 1.1f); }
		public int BonusCritMagic(int initial) { return BonusMult(initial, critMagic, 2, 1.35f); }
		public int BonusDefense(int initial) { return BonusMult(initial, defense, 1, 1.05f); }
		public int BonusThreat(int initial) { return BonusMult(initial, threat, 125, 1.2f); }

		public virtual void AddTooltips(Item item)
		{
			MItem mitem = item.GetSubClass<MItem>();
			if (mitem != null)
			{
				string ttip;

				if (damageMelee > 0 && damageMelee == damageRanged && damageMelee == damageMagic)
				{
					ttip = BonusPowerString(damageMelee) + " increases damage";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					item.toolTips.Add(ttip); mitem.resetTooltip++;
				}
				else
				{
					if (damageMelee > 0)
					{
						ttip = BonusPowerString(damageMelee) + " increases melee damage";
						ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
						item.toolTips.Add(ttip); mitem.resetTooltip++;
					}
					if (damageRanged > 0)
					{
						ttip = BonusPowerString(damageRanged) + " increases ranged damage";
						ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
						item.toolTips.Add(ttip); mitem.resetTooltip++;
					}
					if (damageMagic > 0)
					{
						ttip = BonusPowerString(damageMagic) + " increases magic damage";
						ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
						item.toolTips.Add(ttip); mitem.resetTooltip++;
					}
				}

				if (critMelee > 0 && critMelee == critRanged && critMelee == critMagic)
				{
					ttip = BonusPowerString(critMelee) + " increases critical strike chance";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					item.toolTips.Add(ttip); mitem.resetTooltip++;
				}
				else
				{
					if (critMelee > 0)
					{
						ttip = BonusPowerString(critMelee) + " increases melee critical strike chance";
						ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
						item.toolTips.Add(ttip); mitem.resetTooltip++;
					}
					if (critRanged > 0)
					{
						ttip = BonusPowerString(critRanged) + " increases ranged critical strike chance";
						ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
						item.toolTips.Add(ttip); mitem.resetTooltip++;
					}
					if (critMagic > 0)
					{
						ttip = BonusPowerString(critMagic) + " increases magic critical strike chance";
						ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
						item.toolTips.Add(ttip); mitem.resetTooltip++;
					}
				}

				if (defense > 0)
				{
					ttip = BonusPowerString(defense) + " increases defense";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					item.toolTips.Add(ttip); mitem.resetTooltip++;
				}

				if (threat > 0)
				{
					ttip = BonusPowerString(threat) + " increases the likeliness of enemies attacking you";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					item.toolTips.Add(ttip); mitem.resetTooltip++;
				}
			}
		}
	}
}