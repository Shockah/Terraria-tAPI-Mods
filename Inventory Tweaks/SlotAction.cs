using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.InvTweaks
{
	public class SlotAction
	{
		public enum MButton
		{
			Disabled,
			Left,
			Right,
			Middle;
		}
		public enum KKeys
		{
			None,
			Ctrl,
			Shift,
			Both;
		}

		public MButton mbutton;
		public KKeys kkeys;

		public SlotAction(MButton mouseButton, KKeys kboardKeys)
		{
			mbutton = mouseButton;
			kkeys = kboardKeys;
		}

		public virtual bool Applies(ItemSlot slot, bool release)
		{
			return true;
		}

		public virtual bool Call(ItemSlot slot, bool release)
		{
			return false;
		}
	}
}