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

		public static MBase me { get; private set; }
		
		public int optMaxSlots = 0;
		public string optUnlockMode = null;

		public override void OnLoad()
		{
			me = this;
			ID.Fill(this);

			optMaxSlots = (int)options["maxSlots"].Value;
			optUnlockMode = (string)options["unlockMode"].Value;

			for (int i = MBase.ACC_SLOT_1; i <= MBase.ACC_SLOT_5; i++) ItemSlot.equip[i].active = false;
			for (int i = MBase.ACC_SOCIAL_1; i <= MBase.ACC_SOCIAL_5; i++) ItemSlot.equip[i].active = false;
			for (int i = MBase.ACC_DYE_1; i <= MBase.ACC_DYE_5; i++) ItemSlot.dye[i].active = false;
		}

		public override void OnUnload()
		{
			for (int i = ACC_SLOT_1; i <= ACC_SLOT_5; i++) ItemSlot.equip[i].active = true;
			for (int i = ACC_SOCIAL_1; i <= ACC_SOCIAL_5; i++) ItemSlot.equip[i].active = true;
			for (int i = ACC_DYE_1; i <= ACC_DYE_5; i++) ItemSlot.dye[i].active = true;
		}

		public override void OptionChanged(Option option)
		{
			switch (option.name)
			{
				case "maxSlots":
					optMaxSlots = (int)option.Value;
					break;
				case "unlockMode":
					optUnlockMode = (string)option.Value;
					break;
			}
		}
	}
}