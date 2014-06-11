using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class ItemSlotPrefixFCM : Interface.ItemSlot, El
	{
		protected readonly InterfaceFCMPrefixes gui;
		public Item item = new Item();

		public ItemSlotPrefixFCM(InterfaceFCMPrefixes gui) : base(MBase.me, "PrefixSlot", 0, (slot, item) => { ((ItemSlotPrefixFCM)slot).item = item; }, (slot) => { return ((ItemSlotPrefixFCM)slot).item; })
		{
			this.gui = gui;
		}

		public override bool AllowsItem(Item item)
		{
			return item.IsBlank() || (item.maxStack == 1 && Prefix.CanHavePrefix(item));
		}

		public override void OnLeftClick(ref bool release)
		{
			if (release && Main.keyState.IsKeyDown(Keys.LeftShift) && !MyItem.IsBlank())
			{
				Main.localPlayer.GetItem(Main.myPlayer, (Item)MyItem.Clone());
				MyItem.SetDefaults(0);
			}
			else
			{
				base.OnLeftClick(ref release);
				gui.Refresh(true);
			}
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update) Update();
			if (draw) Draw(sb);
			return false;
		}
	}
}
