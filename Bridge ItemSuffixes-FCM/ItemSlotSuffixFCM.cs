using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.FCM;
using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	public class ItemSlotSuffixFCM : Interface.ItemSlot, El
	{
		protected readonly InterfaceFCMSuffixes gui;
		public Item item = new Item();

		public ItemSlotSuffixFCM(InterfaceFCMSuffixes gui) : base(MBase.me, "SuffixSlot", 0, (slot, item) => { ((ItemSlotSuffixFCM)slot).item = item; }, (slot) => { return ((ItemSlotSuffixFCM)slot).item; })
		{
			this.gui = gui;
		}

		public override bool AllowsItem(Item item)
		{
			return item.IsBlank() || (item.maxStack == 1 && item.GetSubClass<MItem>().CanGetSuffixes());
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
