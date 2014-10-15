using Shockah.Base;
using System;
using TAPI;

namespace Shockah.ItemSuffixes
{
	public class MBase : APIModBase
	{
		public static ModBase me = null;

		public override void OnLoad()
		{
			me = this;
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
				Func<Item, int> getSuffixID = (item) -> {
					return item.GetSubClass<MItem>().suffix.id;
				};
				Func<Item, bool> hasSuffix = (item) -> {
					return item.GetSubClass<MItem>().suffix.id != 0;
				};
				Func<Item, bool> canGetSuffixes = (item) -> {
					return item.GetSubClass<MItem>().CanGetSuffixes();
				};
				Action<Item> setRandomSuffix = (item) -> {
					item.GetSubClass<MItem>().SetRandomSuffix();
				};
				Func<int, Tuple<string, string, int[], int[], int, int, int>> getSuffixInfo = (suffixID) -> {
					MItem mi = item.GetSubClass<MItem>();
					if (mi.suffix.id == 0) return null;
					return new Tuple<string, string, int[], int[], int, int, int>(
						mi.suffix.displayName,
						mi.suffix.format
						new int[] { mi.suffix.damageMelee, mi.suffix.damageRanged, mi.suffix.damageMagic },
						new int[] { mi.suffix.critMelee, mi.suffix.critRanged, mi.suffix.critMagic },
						mi.suffix.defense,
						mi.suffix.threat,
						mi.suffix.regenHP
					);
				}

				if (args.Length == 2)
				{
					var callback = args[1] as Action<
						Func<Item, int>,
						Func<Item, bool>,
						Func<Item, bool>,
						Action<Item>,
						Func<int, Tuple<string, string, int[], int[], int, int, int>>
					>;
					if (callback == null)
					{
						return new object[] {
							getSuffixID,
							hasSuffix,
							canGetSuffixes,
							setRandomSuffix,
							getSuffixInfo
						};
					}
					else
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
				}
			}

			return base.OnModCall(mod, args);
		}
	}
}