using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.AccSlots
{
	public class MBase : ModBase
	{
		public const int
			ACC_SLOT_1 = 3,
			ACC_SLOT_5 = 7,
			ACC_SOCIAL_1 = 11,
			ACC_SOCIAL_5 = 15,
			ACC_DYE_1 = 3,
			ACC_DYE_5 = 7;
		
		public static int currentSlots = 5;

		public static void UpdateSlots(int slots = -1)
		{
			if (slots >= 0)
			{
				currentSlots = slots;
			}
		}

		public override void OnUnload()
		{
			for (int i = ACC_SLOT_1; i <= ACC_SLOT_5; i++) ItemSlot.equip[i].active = true;
			for (int i = ACC_SOCIAL_1; i <= ACC_SOCIAL_5; i++) ItemSlot.equip[i].active = true;
			for (int i = ACC_DYE_1; i <= ACC_DYE_5; i++) ItemSlot.dye[i].active = true;
		}
	}
}