using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class ItemSlotFCM : Interface.ItemSlot, El
	{
		protected readonly InterfaceFCMItems gui;
		public Item item = new Item();

		public ItemSlotFCM(InterfaceFCMItems gui, int index) : base(MBase.me, "Slot", index, (slot, item) => { ((ItemSlotFCM)slot).item = item; }, (slot) => { return ((ItemSlotFCM)slot).item; })
		{
			this.gui = gui;
		}

		public override bool AllowsItem(Item item)
		{
			return item.IsBlank() || (!this.item.IsBlank() && item.IsTheSameAs(this.item) || (this.item.IsBlank() && ShouldHold() != null && item.IsTheSameAs(ShouldHold())));
		}

		public override void OnLeftClick(ref bool release)
		{
			if (release && Main.keyState.IsKeyDown(Keys.LeftShift) && !MyItem.IsBlank())
			{
				Main.localPlayer.GetItem(Main.myPlayer, (Item)MyItem.Clone());
			}
			else
			{
				base.OnLeftClick(ref release);
			}
		}

		public Item ShouldHold()
		{
			if (index >= 0 && index < gui.filtered.Count) return gui.filtered[index];
			return null;
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update) Update();
			if (draw) Draw(sb);
			return false;
		}
	}
}
