using Shockah.Base;
using System;
using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	public class MBase : APIModBase
	{
		public static ModBase me = null;

		public static bool modAccSlots = false;
		public static Func<Player, int> hookAccSlotsGetAvailableExtraSlots = null;
		public static Func<Player, int, Item> hookAccSlotsGetExtraItemAt = null;

		public override void OnLoad()
		{
			me = this;

			modAccSlots = false;
			hookAccSlotsGetAvailableExtraSlots = null;
			hookAccSlotsGetExtraItemAt = null;
		}
		
		public override void OnAllModsLoaded()
		{
			SBase.EventNPCLoot += (npc, item) =>
			{
				MItem mitem = item.GetSubClass<MItem>();
				if (mitem == null) return;
				if (mitem.CanGetSuffixes()) mitem.SetRandomSuffix();
			};
			SBase.EventTileLoot += (point, item) =>
			{
				MItem mitem = item.GetSubClass<MItem>();
				if (mitem == null) return;
				if (mitem.CanGetSuffixes()) mitem.SetRandomSuffix();
			};

			if (Mods.IsModLoaded("Shockah.AccSlots"))
			{
				modAccSlots = true;
				object[] ret = (object[])CallInMod("Shockah.AccSlots", "RequestHooks");
				hookAccSlotsGetAvailableExtraSlots = (Func<Player, int>)ret[1];
				hookAccSlotsGetExtraItemAt = (Func<Player, int, Item>)ret[3];
			}
		}

		public override object OnModCall(ModBase mod, params object[] args)
		{
			if (args.Length == 0)
			{
				return base.OnModCall(mod, args);
			}

			string call = (string)args[0];
			if (args.Length <= 2 && call == "RequestHooks")
			{
				Func<Item, int> getSuffixID = (item) => {
					return item.GetSubClass<MItem>().suffix.id;
				};
				Func<Item, bool> hasSuffix = (item) => {
					return item.GetSubClass<MItem>().suffix.id != 0;
				};
				Func<Item, bool> canGetSuffixes = (item) => {
					return item.GetSubClass<MItem>().CanGetSuffixes();
				};
				Action<Item> setRandomSuffix = (item) => {
					item.GetSubClass<MItem>().SetRandomSuffix();
				};
				Func<int, Tuple<string, string, int[], int[], int, int, int>> getSuffixInfo = (suffixID) => {
					ItemSuffix suffix = ItemSuffix.list[suffixID];
					return new Tuple<string, string, int[], int[], int, int, int>(
						suffix.displayName,
						suffix.format,
						new int[] { suffix.damageMelee, suffix.damageRanged, suffix.damageMagic },
						new int[] { suffix.critMelee, suffix.critRanged, suffix.critMagic },
						suffix.defense,
						suffix.threat,
						suffix.regenHP
					);
				};

				if (args.Length == 2)
				{
					var callback = args[1] as Action<
						Func<Item, int>,
						Func<Item, bool>,
						Func<Item, bool>,
						Action<Item>,
						Func<int, Tuple<string, string, int[], int[], int, int, int>>
					>;
					if (callback != null)
					{
						callback(
							getSuffixID,
							hasSuffix,
							canGetSuffixes,
							setRandomSuffix,
							getSuffixInfo
						);
						return null;
					}

					return new object[] {
						getSuffixID,
						hasSuffix,
						canGetSuffixes,
						setRandomSuffix,
						getSuffixInfo
					};
				}
			}

			return base.OnModCall(mod, args);
		}
	}
}