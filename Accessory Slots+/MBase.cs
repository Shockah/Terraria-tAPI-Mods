using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;
using Terraria;
using Shockah.Base;

namespace Shockah.AccSlots
{
	public class MBase : ModBase
	{
		public const int
			ACC_SLOT_1 = 3,
			ACC_SLOT_5 = 7,
			ACC_SOCIAL_1 = 11,
			ACC_SOCIAL_5 = 15,
			ACC_DYE_1 = 3,
			ACC_DYE_5 = 7;

		public static MBase me { get; private set; }
		public static SEvent<Action<Player, string, int, Item, Item>> EventExtraSlotChange;
		public static SEvent<Action<Player, int, bool>> EventExtraVisibilityChange;
		public static SEvent<Action<Player, int>> EventUnlockSlot;
		
		public int optMaxSlots = 0;
		public string optUnlockMode = null;

		public static int counterSlot = 0;

		public override void OnLoad()
		{
			me = this;
			ID.Fill(this);
			EventExtraSlotChange = new SEvent<Action<Player, string, int, Item, Item>>();
			EventExtraVisibilityChange = new SEvent<Action<Player, int, bool>>();
			EventUnlockSlot = new SEvent<Action<Player, int>>();

			optMaxSlots = (int)options["maxSlots"].Value;
			optUnlockMode = (string)options["unlockMode"].Value;

			for (int i = MBase.ACC_SLOT_1; i <= MBase.ACC_SLOT_5; i++) ItemSlot.equip[i].active = false;
			for (int i = MBase.ACC_SOCIAL_1; i <= MBase.ACC_SOCIAL_5; i++) ItemSlot.equip[i].active = false;
			for (int i = MBase.ACC_DYE_1; i <= MBase.ACC_DYE_5; i++) ItemSlot.dye[i].active = false;
		}

		public override void OnUnload()
		{
			for (int i = ACC_SLOT_1; i <= ACC_SLOT_5; i++) ItemSlot.equip[i].active = true;
			for (int i = ACC_SOCIAL_1; i <= ACC_SOCIAL_5; i++) ItemSlot.equip[i].active = true;
			for (int i = ACC_DYE_1; i <= ACC_DYE_5; i++) ItemSlot.dye[i].active = true;
		}

		public override void OnAllModsLoaded()
		{
			SBase.EventMenuStateChange += (menu) =>
			{
				counterSlot = 0;
			};

			EventExtraSlotChange += (player, type, index, oldItem, newItem) =>
			{
				BinBuffer bb = new BinBuffer();
				bb.Write((byte)(type == "Item" ? ILSlots.MODE_ITEM : (type == "Social" ? ILSlots.MODE_SOCIAL : ILSlots.MODE_DYE)));
				bb.Write((byte)index);
				bb.Write(newItem);
				NetMessage.SendModData(this, MNet.MSG_UPDATESLOT, -1, -1, bb);
			};

			EventExtraVisibilityChange += (player, index, state) =>
			{
				BinBuffer bb = new BinBuffer();
				bb.Write((byte)index);
				bb.Write(state);
				NetMessage.SendModData(this, MNet.MSG_UPDATEVISIBILITY, -1, -1, bb);
			};
		}

		public override void OptionChanged(Option option)
		{
			switch (option.name)
			{
				case "maxSlots":
					optMaxSlots = (int)option.Value;
					break;
				case "unlockMode":
					optUnlockMode = (string)option.Value;
					break;
			}
		}

		public override object OnModCall(ModBase mod, params object[] args)
		{
			if (args.Length == 0)
			{
				return base.OnModCall(mod, args);
			}

			string call = (string)args[0];
			if (call.StartsWith("RegisterEvent") && args.Length == 2)
			{
				call = call.Substring("RegisterEvent".Length);
				switch (call)
				{
					case "ExtraSlotChange":
					{
						var ev = args[1] as Action<Player, string, int, Item, Item>;
						if (ev != null) EventExtraSlotChange += ev;
						break;
					}
					case "ExtraVisibilityChange":
					{
						var ev = args[1] as Action<Player, int, bool>;
						if (ev != null) EventExtraVisibilityChange += ev;
						break;
					}
					case "UnlockSlot":
					{
						var ev = args[1] as Action<Player, int>;
						if (ev != null) EventUnlockSlot += ev;
						break;
					}
				}
			}
			else if (args.Length <= 2 && call == "RequestHooks")
			{
				Func<Player, int> getAvailableSlots = (player) => {
					return player.GetSubClass<MPlayer>().currentSlots;
				};
				Func<Player, int> getAvailableExtraSlots = (player) => {
					return player.GetSubClass<MPlayer>().currentExtraSlots;
				};

				Func<Player, int, Item> getItemAt = (player, slot) => {
					return slot < 5 ? player.armor[ACC_SLOT_1 + slot] : player.GetSubClass<MPlayer>().extraItem[slot - 5];
				};
				Func<Player, int, Item> getExtraItemAt = (player, slot) => {
					return player.GetSubClass<MPlayer>().extraItem[slot];
				};

				Func<Player, int, Item> getSocialItemAt = (player, slot) => {
					return slot < 5 ? player.armor[ACC_SOCIAL_1 + slot] : player.GetSubClass<MPlayer>().extraSocial[slot - 5];
				};
				Func<Player, int, Item> getExtraSocialItemAt = (player, slot) => {
					return player.GetSubClass<MPlayer>().extraSocial[slot];
				};

				Func<Player, int, Item> getDyeItemAt = (player, slot) => {
					return slot < 5 ? player.dye[ACC_DYE_1 + slot] : player.GetSubClass<MPlayer>().extraDye[slot - 5];
				};
				Func<Player, int, Item> getExtraDyeItemAt = (player, slot) => {
					return player.GetSubClass<MPlayer>().extraDye[slot];
				};

				Func<Player, int, bool> isVisibleAt = (player, slot) => {
					return slot < 5 ? !player.hideVisual[ACC_SLOT_1 + slot] : player.GetSubClass<MPlayer>().visibility[slot - 5];
				};
				Func<Player, int, bool> isExtraVisibleAt = (player, slot) => {
					return player.GetSubClass<MPlayer>().visibility[slot];
				};

				if (args.Length == 2)
				{
					var callback = args[1] as Action<
						Func<Player, int>,
						Func<Player, int>,
						Func<Player, int, Item>,
						Func<Player, int, Item>,
						Func<Player, int, Item>,
						Func<Player, int, Item>,
						Func<Player, int, Item>,
						Func<Player, int, Item>,
						Func<Player, int, bool>,
						Func<Player, int, bool>
					>;
					if (callback != null)
					{
						callback(
							getAvailableSlots,
							getAvailableExtraSlots,

							getItemAt,
							getExtraItemAt,

							getSocialItemAt,
							getExtraSocialItemAt,

							getDyeItemAt,
							getExtraDyeItemAt,

							isVisibleAt,
							isExtraVisibleAt
						);
						return null;
					}
				}

				return new object[] {
					getAvailableSlots,
					getAvailableExtraSlots,

					getItemAt,
					getExtraItemAt,

					getSocialItemAt,
					getExtraSocialItemAt,

					getDyeItemAt,
					getExtraDyeItemAt,

					isVisibleAt,
					isExtraVisibleAt
				};
			}

			return base.OnModCall(mod, args);
		}
	}
}