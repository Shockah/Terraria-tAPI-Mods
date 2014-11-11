using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class ItemSlotFCM : ItemSlot, El
	{
		protected readonly InterfaceFCMItems gui;
		public Item item = new Item();

		public ItemSlotFCM(InterfaceFCMItems gui, int index) : base(MBase.me, "Slot", index, (slot, item) => { ((ItemSlotFCM)slot).item = item; }, (slot) => { return ((ItemSlotFCM)slot).item; })
		{
			this.gui = gui;
		}

		public override void Update(Vector2 offset)
		{
			if (!MyItem.IsBlank() && IsMouseOnItemSlot() && KState.Special.Ctrl.Down()) InterfaceFCMItems.displayIds = true;
			base.Update(offset);
		}

		public override bool AllowsItem(Item item)
		{
			return item.IsBlank() || (!this.item.IsBlank() && item.IsTheSameAs(this.item) || (this.item.IsBlank() && ShouldHold() != null && item.IsTheSameAs(ShouldHold())));
		}

		public override void OnLeftClick(ref bool release)
		{
			if (InterfaceFCMBase.lockSlotInteraction) return;
			if (release && KState.Special.Shift.Down() && !MyItem.IsBlank())
			{
				Main.localPlayer.GetItem(Main.myPlayer, (Item)MyItem.Clone());
			}
			else
			{
				base.OnLeftClick(ref release);
			}
		}
		public override void OnRightClick(ref bool release)
		{
			if (InterfaceFCMBase.lockSlotInteraction) return;
			base.OnRightClick(ref release);
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
