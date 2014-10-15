using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.AccSlots
{
	public class CustomItemSlotAccessory : ItemSlotAccessory
	{
		public readonly int customIndex;
		public readonly Action<CustomItemSlotAccessory, bool> VisibleSet;
		public readonly Func<CustomItemSlotAccessory, bool> VisibleGet;
		
		public CustomItemSlotAccessory(ModBase modBase, string type, int index, int accessoryIndex, int customIndex, Action<ItemSlot, Item> ActionSet, Func<ItemSlot, Item> ActionGet, Action<CustomItemSlotAccessory, bool> VisibleSet, Func<CustomItemSlotAccessory, bool> VisibleGet)
			: base(modBase, type, index, accessoryIndex, ActionSet, ActionGet)
		{
			this.customIndex = customIndex;
			this.VisibleSet = VisibleSet;
			this.VisibleGet = VisibleGet;
		}

		public override bool IsAccessoryVisible()
		{
			return VisibleGet(this);
		}

		public override void ToggleAccessoryVisibility()
		{
			bool newState = !VisibleGet(this);
			VisibleSet(this, newState);
			foreach (Action<Player, int, bool> h in MBase.EventExtraVisibilityChange) h(Main.localPlayer, customIndex, newState);
		}

		public override void Update(Vector2 offset)
		{
			pos = position + offset;

			if (!IsActive()) return;
			Vector2 btnVisibilityOffset = new Vector2(34 / 0.85f, 2 / 0.85f);
			Texture2D tex = IsAccessoryVisible() ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;

			if (new Rectangle((int)(pos.X + btnVisibilityOffset.X * scale), (int)(pos.Y + btnVisibilityOffset.Y * scale), tex.Width, tex.Height).Contains(Main.mouse))
			{
				Main.localPlayer.mouseInterface = true;
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					ToggleAccessoryVisibility();
					Main.PlaySound(12, -1, -1, 1);
				}
				Main.toolTip = new Item();
				Main.hoverItemName = Lang.inter[IsAccessoryVisible() ? 59 : 60];
			}
			else if (IsMouseOnItemSlot())
			{
				Main.armorHide = true;
				Main.localPlayer.mouseInterface = true;
				if (!MyItem.IsBlank()) { MyItem.social = MyItem.wornArmor = false; }
				if (Main.mouseLeft) UICore.clickAction = this._LeftClick;
				else if (Main.mouseRight) UICore.clickAction = this._RightClick;

				if (!MyItem.IsBlank())
				{
					Main.hoverItemName = MyItem.AffixName();
					if (MyItem.stack > 1) Main.hoverItemName += " (" + MyItem.stack + ")";
					Main.toolTip = MyItem;
				}
			}
		}
		public override bool AllowsItem(Item item)
		{
			bool? hb = Hooks.Interface.ItemSlotAllowsItem(this, item);
			if (hb.HasValue) return hb.Value;
			if (item.IsBlank()) return true;
			return item.accessory && item.CanEquip(Main.localPlayer, this);
		}
		public override void OnLeftClick(ref bool release)
		{
			if (Hooks.Interface.PreItemSlotLeftClick(this, ref release))
			{
				if (release)
				{
					if (Main.localPlayer.itemAnimation <= 0 && Main.localPlayer.itemTime == 0)
					{
						if (AllowsItem(Main.mouseItem))
						{
							Item _item = Main.mouseItem;
							Main.mouseItem = MyItem;
							MyItem = _item;
							if (MyItem.IsBlank()) MyItem = new Item();

							if (Main.mouseItem != null && Main.mouseItem.stack > 0) Main.mouseItem.OnEquip(Main.localPlayer, this);
							if (MyItem != null && MyItem.stack > 0) MyItem.OnUnEquip(Main.localPlayer, this);

							if (Main.mouseItem.IsTheSameAs(MyItem) && MyItem.stack != MyItem.maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack)
							{
								if (Main.mouseItem.stack + MyItem.stack <= Main.mouseItem.maxStack)
								{
									MyItem.stack += Main.mouseItem.stack;
									Main.mouseItem.SetDefaults(0);
								}
								else
								{
									int diff = Main.mouseItem.maxStack - MyItem.stack;
									MyItem.stack += diff;
									Main.mouseItem.stack -= diff;
								}
							}
							if (Main.mouseItem.IsBlank()) Main.mouseItem = new Item();
							if (!Main.mouseItem.IsBlank() || !MyItem.IsBlank()) Main.PlaySound(7, -1, -1, 1);
						}
					}
					Recipe.FindRecipes();
				}
			}
			Hooks.Interface.PostItemSlotLeftClick(this, release);
		}

		public override void DrawItemSlotBackground(SpriteBatch sb, Vector2 offset)
		{
			pos = position + offset;

			if (Hooks.Interface.PreDrawItemSlotBackground(sb, this))
			{
				Texture2D tex = Main.inventoryBack3Texture;
				sb.Draw(tex, pos, null, Main.inventoryBack * alpha, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
			}
			Hooks.Interface.PostDrawItemSlotBackground(sb, this);
		}
	}
}