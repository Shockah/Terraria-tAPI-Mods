using TAPI;
using TAPI.UIKit;
using Terraria;
using Shockah.Base;
using Shockah.InvTweaks;

namespace Shockah.InvTweaks.SlotActions
{
	public class OverrideTrashAction : SlotAction
	{
		public override bool Applies(ItemSlot slot, bool release)
		{
			return !slot.MyItem.IsBlank() && (slot.type == "Inventory" || slot.type == "Coin" || slot.type == "Ammo") && release;
		}

		public override bool Call(ItemSlot slot, bool release)
		{
			return true;
		}

		public override void Load(OptionList options)
		{
			bool enabled = (bool)options[GetType().Name].Value;
			mbutton = enabled ? MButton.Left : MButton.Disabled;
			if (enabled) kkeys = KKeys.Shift;
		}
	}
}