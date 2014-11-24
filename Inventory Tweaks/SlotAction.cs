using System;
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
			Right
		}
		public enum KKeys
		{
			None,
			Ctrl,
			Shift,
			Both
		}

		public MButton mbutton = MButton.Disabled;
		public KKeys kkeys = KKeys.None;

		public virtual bool Applies(ItemSlot slot, bool release)
		{
			return true;
		}

		public virtual bool Call(ItemSlot slot, bool release)
		{
			return false;
		}

		public virtual void Load(OptionList options)
		{
			mbutton = (MButton)Enum.Parse(typeof(MButton), (string)options[GetType().Name + "_mouse"].Value);
			kkeys = (KKeys)Enum.Parse(typeof(KKeys), (string)options[GetType().Name + "_keys"].Value);
		}
	}
}