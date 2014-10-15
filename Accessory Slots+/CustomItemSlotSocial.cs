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
	public class CustomItemSlotSocial : ItemSlot
	{
		public readonly int customIndex;

		public CustomItemSlotSocial(ModBase modBase, string type, int index, int customIndex, Action<ItemSlot, Item> ActionSet, Func<ItemSlot, Item> ActionGet) : base(modBase, type, index, ActionSet, ActionGet)
		{
			this.customIndex = customIndex;
		}

		public override void Update(Vector2 offset)
		{
			base.Update(offset);
			Main.toolTip.social = true;
		}

		public override bool AllowsItem(Item item)
		{
			bool? hb = Hooks.Interface.ItemSlotAllowsItem(this, item);
			if (hb.HasValue) return hb.Value;
			if (item.IsBlank()) return true;
			return item.accessory && item.CanEquip(Main.localPlayer, this) && CustomItemSlotAccessory.CanEquip(item);
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
		public override void OnRightClick(ref bool release)
		{
			if (Hooks.Interface.PreItemSlotRightClick(this, ref release))
			{
				if (MyItem.CanEquip(Main.localPlayer, this))
				{
					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					Item result = mp.extraItem[index].Clone();
					if (result.stack > 0) result.OnUnEquip(Main.localPlayer, this);
					mp.extraItem[index] = MyItem.Clone();
					mp.extraItem[index].OnEquip(Main.localPlayer, this);
					MyItem = result;
					Main.PlaySound(7, -1, -1, 1);
				}
				Recipe.FindRecipes();
			}
			Hooks.Interface.PostItemSlotRightClick(this, release);
		}

		public override void DrawItemSlotBackground(SpriteBatch sb, Vector2 offset)
		{
			pos = position + offset;

			if (Hooks.Interface.PreDrawItemSlotBackground(sb, this))
			{
				Texture2D tex = Main.inventoryBack8Texture;
				sb.Draw(tex, pos, null, Main.inventoryBack * alpha, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
			}
			Hooks.Interface.PostDrawItemSlotBackground(sb, this);
		}

		public override void DrawItemSlotItem(SpriteBatch sb, Vector2 offset)
		{
			pos = position + offset;

			if (Hooks.Interface.PreDrawItemSlotItem(sb, this))
			{
				ItemSlot behind = MInterface.layer.slotsItem[customIndex];
				if (!behind.MyItem.IsBlank())
				{
					float alpha = this.alpha * 0.4f;
					float iscale = 1f;
					Texture2D texItem = behind.MyItem.GetTexture();
					if (texItem.Width > 32 || texItem.Height > 32) iscale = texItem.Width > texItem.Height ? 32f / texItem.Width : 32f / texItem.Height;
					iscale *= scale;

					Color cTexItem = behind.MyItem.GetTextureColor();
					sb.Draw(
						texItem,
						new Vector2(pos.X + 26f * scale - texItem.Width * 0.5f * iscale, pos.Y + 26f * scale - texItem.Height * 0.5f * iscale),
						null, behind.MyItem.GetAlpha(cTexItem) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
					);

					cTexItem = Color.White;
					if (behind.MyItem.color != default(Color))
					{
						sb.Draw(
							texItem,
							new Vector2(pos.X + 26f * scale - texItem.Width * 0.5f * iscale, pos.Y + 26f * scale - texItem.Height * 0.5f * iscale),
							null, behind.MyItem.GetColor(Color.White) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
						);
					}

					if (behind.MyItem.stack > 1)
					{
						sb.DrawString(Main.fontItemStack, "" + behind.MyItem.stack, new Vector2(pos.X + 10f * scale, pos.Y + 26f * scale), Color.White * alpha, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
					}
				}
				
				if (!MyItem.IsBlank())
				{
					float iscale = 1f;
					Texture2D texItem = MyItem.GetTexture();
					if (texItem.Width > 32 || texItem.Height > 32) iscale = texItem.Width > texItem.Height ? 32f / texItem.Width : 32f / texItem.Height;
					iscale *= scale;

					Color cTexItem = MyItem.GetTextureColor();
					sb.Draw(
						texItem,
						new Vector2(pos.X + 26f * scale - texItem.Width * 0.5f * iscale, pos.Y + 26f * scale - texItem.Height * 0.5f * iscale),
						null, MyItem.GetAlpha(cTexItem) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
					);

					cTexItem = Color.White;
					if (MyItem.color != default(Color))
					{
						sb.Draw(
							texItem,
							new Vector2(pos.X + 26f * scale - texItem.Width * 0.5f * iscale, pos.Y + 26f * scale - texItem.Height * 0.5f * iscale),
							null, MyItem.GetColor(Color.White) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
						);
					}

					if (MyItem.stack > 1)
					{
						sb.DrawString(Main.fontItemStack, "" + MyItem.stack, new Vector2(pos.X + 10f * scale, pos.Y + 26f * scale), Color.White * alpha, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
					}
				}
			}
			Hooks.Interface.PostDrawItemSlotItem(sb, this);
		}
	}
}