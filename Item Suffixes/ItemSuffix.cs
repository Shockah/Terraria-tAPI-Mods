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
			blacklistThreat = new List<Func<Item, bool>>(),
			blacklistRegenHP = new List<Func<Item,bool>>();

		static ItemSuffix()
		{
			new ItemSuffix(null);
			
			new ItemSuffix("Bear").Melee(1).Defense(1);
			new ItemSuffix("Gorilla").Melee(1).Threat(1);
			new ItemSuffix("Tiger").Melee(1, 1);
			new ItemSuffix("Champion").Melee(2);
			new ItemSuffix("Mercenary").Melee(0, 2);
			new ItemSuffix("Ancestor").Melee(0, 1).Defense(1);
			new ItemSuffix("Soldier").Melee(0, 1).Threat(1);

			new ItemSuffix("Monkey").Ranged(1).Defense(1);
			new ItemSuffix("Wolf").Ranged(1).Threat(1);
			new ItemSuffix("Falcon").Ranged(1, 1);
			new ItemSuffix("Hunt").Ranged(2);
			new ItemSuffix("Marksman").Ranged(0, 2);
			new ItemSuffix("Wild").Ranged(0, 1).Defense(1);
			new ItemSuffix("Beast").Ranged(0, 1).Threat(1);

			new ItemSuffix("Eagle").Magic(1).Defense(1);
			new ItemSuffix("Whale").Magic(1).Threat(1);
			new ItemSuffix("Owl").Magic(1, 1);
			new ItemSuffix("Sorcerer").Magic(2);
			new ItemSuffix("Nightmare").Magic(0, 2);
			new ItemSuffix("Moon").Magic(0, 1).Defense(1);
			new ItemSuffix("Sun").Magic(0, 1).Threat(1);

			new ItemSuffix("Knight").Defense(2);
			new ItemSuffix("Crusade").Threat(2);
			new ItemSuffix("Squire").Defense(1).Threat(1);

			new ItemSuffix("Regeneration", "{0} of {1}").RegenHP(2);
			new ItemSuffix("Boar").Melee(1).RegenHP(1);
			new ItemSuffix("Bandit").Melee(0, 1).RegenHP(1);
			new ItemSuffix("Marksmanship", "{0} of {1}").Ranged(1).RegenHP(1);
			new ItemSuffix("Eluding", "{0} of {1}").Ranged(0, 1).RegenHP(1);
			new ItemSuffix("Necromancer").Magic(1).RegenHP(1);
			new ItemSuffix("Concentration", "{0} of {1}").Magic(0, 1).RegenHP(1);
			new ItemSuffix("Paladin").Defense(1).RegenHP(1);
			new ItemSuffix("Elder").Threat(1).RegenHP(1);

			Func<Item, bool>[] fs;

			//Melee
			fs = new Func<Item, bool>[]{
				(item) => (item.type >= 100 && item.type <= 102) || (item.type >= 956 && item.type <= 958), //Shadow set
				(item) => item.type >= 231 && item.type <= 233, //Molten set
				(item) => new List<int>(new int[] { 372, 1205, 377, 1210, 401, 1215, 559, 1001 }).Contains(item.type), //Cobalt / Palladium / Mythril / Orichalcum / Adamantite / Titanium / Hallowed / Chlorophyte set
				(item) => item.type >= 1316 && item.type <= 1318, //Turtle set
				(item) => item.type >= 2199 && item.type <= 2202, //Beetle set
				(item) => new List<int>(new int[] { 211, 490, 536, 485, 897, 936, 1322, 1343 }).Contains(item.type) //accessories
			};
			foreach (Func<Item, bool> f in fs) { blacklistRanged.Add(f); blacklistMagic.Add(f); }

			//Ranged
			fs = new Func<Item, bool>[]{
				(item) => (item.type >= 151 && item.type <= 153) || item.type == 959, //Necro set
				(item) => new List<int>(new int[] { 373, 1206, 378, 1211, 402, 1216, 553, 1002 }).Contains(item.type), //Cobalt / Palladium / Mythril / Orichalcum / Adamantite / Titanium / Hallowed / Chlorophyte set
				(item) => item.type >= 1546 && item.type <= 1550, //Shroomite set
				(item) => new List<int>(new int[] { 491, 1300, 1321, 1858 }).Contains(item.type) //accessories
			};
			foreach (Func<Item, bool> f in fs) { blacklistMelee.Add(f); blacklistMagic.Add(f); }

			//Magic
			fs = new Func<Item, bool>[]{
				(item) => (item.type >= 228 && item.type <= 230) || (item.type >= 960 && item.type <= 962), //Jungle set
				(item) => item.type >= 123 && item.type <= 125, //Meteor set
				(item) => new List<int>(new int[] { 371, 1207, 376, 1212, 400, 1217, 558, 1003 }).Contains(item.type), //Cobalt / Palladium / Mythril / Orichalcum / Adamantite / Titanium / Hallowed / Chlorophyte set
				(item) => item.type >= 1832 && item.type <= 1834, //Spooky set
				(item) => item.type >= 1159 && item.type <= 1161, //Tiki set
				(item) => (item.type >= 1503 && item.type <= 1505) || item.type == 2189, //Spectre set
				(item) => item.type >= 2361 && item.type <= 2363, //Bee set
				(item) => item.type >= 2370 && item.type <= 2372, //Spider set
				(item) => new List<int>(new int[] { 111, 223, 489, 555, 982, 1158, 1167, 1595, 1845, 1864, 2219, 2220, 2221 }).Contains(item.type), //accessories
				(item) => new List<int>(new int[] { 238, 1282, 1283, 1284, 1285, 1286, 1287, 2275, 2279 }).Contains(item.type) //armor parts
			};
			foreach (Func<Item, bool> f in fs) { blacklistMelee.Add(f); blacklistRanged.Add(f); }

			//Melee + Ranged
			fs = new Func<Item, bool>[]{
				(item) => item.type >= 684 && item.type <= 686 //Frost set
			};
			foreach (Func<Item, bool> f in fs) { blacklistMagic.Add(f); }
		}

		public static List<ItemSuffix> AllowedSuffixes(Item item)
		{
			List<ItemSuffix> ret = new List<ItemSuffix>();
			foreach (ItemSuffix suffix in list)
			{
				if (suffix.displayName == null) continue;
				if (suffix.damageMelee != 0 || suffix.critMelee != 0) foreach (Func<Item, bool> f in blacklistMelee) if (f(item)) goto L;
				if (suffix.damageRanged != 0 || suffix.critRanged != 0) foreach (Func<Item, bool> f in blacklistRanged) if (f(item)) goto L;
				if (suffix.damageMagic != 0 || suffix.critMagic != 0) foreach (Func<Item, bool> f in blacklistMagic) if (f(item)) goto L;
				if (suffix.defense != 0) foreach (Func<Item, bool> f in blacklistDefense) if (f(item)) goto L;
				if (suffix.threat != 0) foreach (Func<Item, bool> f in blacklistThreat) if (f(item)) goto L;
				if (suffix.regenHP != 0) foreach (Func<Item, bool> f in blacklistRegenHP) if (f(item)) goto L;
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
		public readonly string displayName, format;

		public int damageMelee, damageRanged, damageMagic;
		public int critMelee, critRanged, critMagic;
		public int defense, threat, regenHP;

		private ItemSuffix(string displayName, string format = "{0} of the {1}")
		{
			id = list.Count;
			this.displayName = displayName;
			this.format = format;
			list.Add(this);
		}

		public bool IsAllowed(Item item)
		{
			List<ItemSuffix> list = AllowedSuffixes(item);
			foreach (ItemSuffix suffix in list) if (object.ReferenceEquals(suffix, this)) return true;
			return false;
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
		public ItemSuffix RegenHP(int regenHP)
		{
			this.regenHP = regenHP;
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
		public int BonusRegenHP(int initial) { return BonusMult(initial, regenHP, 2, 1.1f); }

		public virtual List<string> AddTooltips()
		{
			List<string> ret = new List<string>();
			string ttip;

			if (damageMelee > 0 && damageMelee == damageRanged && damageMelee == damageMagic)
			{
				ttip = BonusPowerString(damageMelee) + " increases damage";
				ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
				ret.Add(ttip);
			}
			else
			{
				if (damageMelee > 0)
				{
					ttip = BonusPowerString(damageMelee) + " increases melee damage";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					ret.Add(ttip);
				}
				if (damageRanged > 0)
				{
					ttip = BonusPowerString(damageRanged) + " increases ranged damage";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					ret.Add(ttip);
				}
				if (damageMagic > 0)
				{
					ttip = BonusPowerString(damageMagic) + " increases magic damage";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					ret.Add(ttip);
				}
			}

			if (critMelee > 0 && critMelee == critRanged && critMelee == critMagic)
			{
				ttip = BonusPowerString(critMelee) + " increases critical strike chance";
				ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
				ret.Add(ttip);
			}
			else
			{
				if (critMelee > 0)
				{
					ttip = BonusPowerString(critMelee) + " increases melee critical strike chance";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					ret.Add(ttip);
				}
				if (critRanged > 0)
				{
					ttip = BonusPowerString(critRanged) + " increases ranged critical strike chance";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					ret.Add(ttip);
				}
				if (critMagic > 0)
				{
					ttip = BonusPowerString(critMagic) + " increases magic critical strike chance";
					ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
					ret.Add(ttip);
				}
			}

			if (defense > 0)
			{
				ttip = BonusPowerString(defense) + " increases defense";
				ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
				ret.Add(ttip);
			}

			if (threat > 0)
			{
				ttip = BonusPowerString(threat) + " increases the likeliness of enemies attacking you";
				ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
				ret.Add(ttip);
			}

			if (regenHP > 0)
			{
				ttip = BonusPowerString(regenHP) + " increases life regeneration rate";
				ttip = ("" + ttip[0]).ToUpper() + ttip.Substring(1);
				ret.Add(ttip);
			}

			return ret;
		}
	}
}