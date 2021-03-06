﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;
using Terraria.DataStructures;

namespace Shockah.AccSlots
{
	public class MPlayer : ModPlayer
	{
		public const int MAX_EXTRA_SLOTS = 10;

		public static PLDyeAccessories layer = null;

		private static void CopyFromItem(Item target, Item source)
		{
			target.netDefaults(source.netID);
			target.stack = source.stack;
		}
		
		public int currentSlots
		{
			get
			{
				int slots = MBase.me.optMaxSlots;
				if (MBase.me.optUnlockMode == "Shopping")
				{
					for (int i = 0; i < 8; i++)
					{
						if (boughtSlots[i]) slots++;
					}
					return slots;
				}
				return Math.Min(slots, MAX_EXTRA_SLOTS + 5);
			}
		}
		public int currentExtraSlots
		{
			get { return currentSlots - 5; }
		}
		public Item[][] extraSlots;
		public Item[] extraItem, extraSocial, extraDye;
		public BitsBytes visibility;
		public BitsByte boughtSlots;

		protected Item[] cacheItem = null, cacheSocial = null, cacheDye = null;

		public override void Initialize()
		{
			boughtSlots = new BitsByte();
			visibility = new BitsBytes();

			extraItem = new Item[MAX_EXTRA_SLOTS];
			extraSocial = new Item[MAX_EXTRA_SLOTS];
			extraDye = new Item[MAX_EXTRA_SLOTS];
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				extraItem[i] = new Item();
				extraSocial[i] = new Item();
				extraDye[i] = new Item();
				visibility[i] = true;
			}
			extraSlots = new Item[][] { extraItem, extraSocial, extraDye };
		}

		public override void Load(BinBuffer bb)
		{
			boughtSlots = bb.ReadByte();
			visibility = bb.ReadBytes(2);
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				extraItem[i] = bb.ReadItem();
				extraSocial[i] = bb.ReadItem();
				extraDye[i] = bb.ReadItem();
			}
		}

		public override void Save(BinBuffer bb)
		{
			bb.Write(boughtSlots);
			bb.Write(visibility);
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				bb.Write(extraItem[i]);
				bb.Write(extraSocial[i]);
				bb.Write(extraDye[i]);
			}
		}

		public override void MidUpdate()
		{
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				if (extraItem[i].IsBlank()) continue;
				player.UpdateEquipNormal(player.whoAmI, extraItem[i]);
			}
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				if (extraItem[i].IsBlank()) continue;
				player.UpdateEquipAccessory(player.whoAmI, extraItem[i], !visibility[i]);
			}
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				if (extraSocial[i].IsBlank()) continue;
				player.UpdateEquipAccessoryVanity(player.whoAmI, extraSocial[i]);
			}
		}

		public override void PostUpdate()
		{
			if (Main.netMode == 2 || player.whoAmI != Main.myPlayer) return;
			if (MBase.EventExtraSlotChange.Count == 0) return;

			bool wasNull = false;
			if (cacheItem == null)
			{
				cacheItem = new Item[extraItem.Length];
				for (int i = 0; i < cacheItem.Length; i++) cacheItem[i] = new Item();
				cacheSocial = new Item[extraSocial.Length];
				for (int i = 0; i < cacheSocial.Length; i++) cacheSocial[i] = new Item();
				cacheDye = new Item[extraDye.Length];
				for (int i = 0; i < cacheDye.Length; i++) cacheDye[i] = new Item();
				wasNull = true;
			}

			for (int i = 0; i < cacheItem.Length && (i < currentExtraSlots || wasNull); i++)
			{
				if (!extraItem[i].IsTheSameAs(cacheItem[i]) || extraItem[i].stack != cacheItem[i].stack || wasNull)
				{
					if (!wasNull) foreach (Action<Player, string, int, Item, Item> h in MBase.EventExtraSlotChange) h(player, "Item", i, cacheItem[i], extraItem[i]);
					CopyFromItem(cacheItem[i], extraItem[i]);
				}
			}
			for (int i = 0; i < cacheSocial.Length && (i < currentExtraSlots || wasNull); i++)
			{
				if (!extraSocial[i].IsTheSameAs(cacheSocial[i]) || extraSocial[i].stack != cacheSocial[i].stack || wasNull)
				{
					if (!wasNull) foreach (Action<Player, string, int, Item, Item> h in MBase.EventExtraSlotChange) h(player, "Social", i, cacheSocial[i], extraSocial[i]);
					CopyFromItem(cacheSocial[i], extraSocial[i]);
				}
			}
			for (int i = 0; i < cacheDye.Length && (i < currentExtraSlots || wasNull); i++)
			{
				if (!extraDye[i].IsTheSameAs(cacheDye[i]) || extraDye[i].stack != cacheDye[i].stack || wasNull)
				{
					if (!wasNull) foreach (Action<Player, string, int, Item, Item> h in MBase.EventExtraSlotChange) h(player, "Dye", i, cacheDye[i], extraDye[i]);
					CopyFromItem(cacheDye[i], extraDye[i]);
				}
			}
		}

		public override void FrameEffects()
		{
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				Item item = extraItem[i];
				if (item.IsBlank()) continue;
				bool hideVisual = !visibility[i];
				if ((player.shield <= 0 || item.frontSlot < 1 || item.frontSlot > 4) && (player.front < 1 || player.front > 4 || item.shieldSlot <= 0))
				{
					if (item.wingSlot > 0)
					{
						if (hideVisual && (player.velocity.Y == 0f || player.mount.Active))
						{
							continue;
						}
						player.wings = item.wingSlot;
					}
					if (!hideVisual)
					{
						if (item.handOnSlot > 0)
						{
							player.handon = item.handOnSlot;
						}
						if (item.handOffSlot > 0)
						{
							player.handoff = item.handOffSlot;
						}
						if (item.backSlot > 0)
						{
							player.back = item.backSlot;
							player.front = -1;
						}
						if (item.frontSlot > 0)
						{
							player.front = item.frontSlot;
						}
						if (item.shoeSlot > 0)
						{
							player.shoe = item.shoeSlot;
						}
						if (item.waistSlot > 0)
						{
							player.waist = item.waistSlot;
						}
						if (item.shieldSlot > 0)
						{
							player.shield = item.shieldSlot;
						}
						if (item.neckSlot > 0)
						{
							player.neck = item.neckSlot;
						}
						if (item.faceSlot > 0)
						{
							player.face = item.faceSlot;
						}
						if (item.balloonSlot > 0)
						{
							player.balloon = item.balloonSlot;
						}
					}
				}
			}
			for (int i = 0; i < MAX_EXTRA_SLOTS; i++)
			{
				Item item = extraSocial[i];
				if (item.IsBlank()) continue;
				if (item.handOnSlot > 0)
				{
					player.handon = item.handOnSlot;
				}
				if (item.handOffSlot > 0)
				{
					player.handoff = item.handOffSlot;
				}
				if (item.backSlot > 0)
				{
					player.back = item.backSlot;
					player.front = -1;
				}
				if (item.frontSlot > 0)
				{
					player.front = item.frontSlot;
				}
				if (item.shoeSlot > 0)
				{
					player.shoe = item.shoeSlot;
				}
				if (item.waistSlot > 0)
				{
					player.waist = item.waistSlot;
				}
				if (item.shieldSlot > 0)
				{
					player.shield = item.shieldSlot;
				}
				if (item.neckSlot > 0)
				{
					player.neck = item.neckSlot;
				}
				if (item.faceSlot > 0)
				{
					player.face = item.faceSlot;
				}
				if (item.balloonSlot > 0)
				{
					player.balloon = item.balloonSlot;
				}
				if (item.wingSlot > 0)
				{
					player.wings = item.wingSlot;
				}
			}
		}

		[CallPriority(-1000000f)] public override void ModifyDrawLayerList(List<PlayerLayer> list)
		{
			if (layer == null)
			{
				layer = new PLDyeAccessories(modBase);
			}
			list.Insert(0, layer);
		}

		[CallPriority(-1000000f)] public override void ModifyDrawLayerHeadList(List<PlayerLayer> list)
		{
			ModifyDrawLayerList(list);
		}
	}
}