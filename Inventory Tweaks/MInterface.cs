using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using TAPI;
using TAPI.UIKit;
using Terraria;
using Shockah.InvTweaks.SlotActions;
using System.Collections.Generic;

namespace Shockah.InvTweaks
{
	public class MInterface : ModInterface
	{
		public readonly List<SlotAction> actions = new List<SlotAction>();
		private Texture2D texGlow = null, texCracked = null, texQuest = null;

		public override void PostDrawItemSlotBackground(SpriteBatch sb, ItemSlot slot)
		{
			if ((bool)modBase.options["colorRarity"].Value)
			{
				if (!slot.MyItem.IsBlank())
				{
					if (texGlow == null)
					{
						texGlow = modBase.textures["Images/ItemSlotGlow"];
						texCracked = modBase.textures["Images/ItemSlotCracked"];
						texQuest = modBase.textures["Images/ItemSlotQuest"];
					}
					Texture2D tex = slot.MyItem.questItem ? texQuest : (slot.MyItem.rare >= 0 ? texGlow : texCracked);
					Color c = slot.MyItem.GetRarityColor();
					if (slot.MyItem.rare == 0) c = Color.LightGray;
					if (slot.MyItem.rare < 0) c = Color.Gray;
					if (slot.MyItem.questItem) c = Color.White;
					sb.Draw(tex, slot.pos, null, c * slot.alpha, 0f, default(Vector2), slot.scale, SpriteEffects.None, 0f);
				}
			}
		}

		public bool HandleActions(ItemSlot slot, bool release, SlotAction.MButton mbutton)
		{
			SlotAction.KKeys kkeys = SlotAction.KKeys.None;
			if (KState.Special.Ctrl.Down())
				kkeys = (SlotAction.KKeys)(((int)kkeys) | ((int)SlotAction.KKeys.Ctrl));
			if (KState.Special.Shift.Down())
				kkeys = (SlotAction.KKeys)(((int)kkeys) | ((int)SlotAction.KKeys.Shift));

			foreach (SlotAction action in actions)
			{
				if (action.mbutton == mbutton && action.kkeys == kkeys && action.Applies(slot, release))
					return !action.Call(slot, release);
			}
			return true;
		}

		public override bool PreItemSlotLeftClick(ItemSlot slot, ref bool release)
		{
			return HandleActions(slot, release, SlotAction.MButton.Left);
		}
		public override bool PreItemSlotRightClick(ItemSlot slot, ref bool release)
		{
			return HandleActions(slot, release, SlotAction.MButton.Right);
		}
	}
}