using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using Shockah.ETooltip.ModuleItem;
using System;
using System.Collections.Generic;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class MBase : ModBase
	{
		public static MBase me { get; private set; }
		public static STooltip tip = new STooltip();

		public int maxPowerPick = 0, maxPowerAxe = 0, maxPowerHammer = 0, maxManaCost = 0;
		public List<Module<Item>> modulesItems = new List<Module<Item>>();
		
		public override void OnLoad()
		{
			me = this;

			modulesItems.Add(new ModuleItemName());
			modulesItems.Add(new ModuleItemDamage());
			modulesItems.Add(new ModuleItemCrit());
			modulesItems.Add(new ModuleItemSpeed());
			modulesItems.Add(new ModuleItemKnockback());
			modulesItems.Add(new ModuleItemEquipable());
			modulesItems.Add(new ModuleItemWand());
			modulesItems.Add(new ModuleItemDefense());
			modulesItems.Add(new ModuleItemPickaxe());
			modulesItems.Add(new ModuleItemAxe());
			modulesItems.Add(new ModuleItemHammer());
			modulesItems.Add(new ModuleItemRange());
			modulesItems.Add(new ModuleItemRestore());
			modulesItems.Add(new ModuleItemManaCost());
			modulesItems.Add(new ModuleItemPlaceable());
			modulesItems.Add(new ModuleItemAmmo());
			modulesItems.Add(new ModuleItemConsumable());
			modulesItems.Add(new ModuleItemMaterial());
			modulesItems.Add(new ModuleItemTipText());
			modulesItems.Add(new ModuleItemBuffDuration());
			modulesItems.Add(new ModuleItemPrefix());
			modulesItems.Add(new ModuleItemSetBonus());
			modulesItems.Add(new ModuleItemPrice());
		}

		public override void OnAllModsLoaded()
		{
			SBase.EventSTooltipDraw += () => { return tip; };

			foreach (KeyValuePair<string, Item> kvp in Defs.items)
			{
				if (kvp.Value.type == Defs.unloadedItem.type) continue;
				if (kvp.Value.pick > maxPowerPick) maxPowerPick = kvp.Value.pick;
				if (kvp.Value.axe > maxPowerAxe) maxPowerAxe = kvp.Value.axe;
				if (kvp.Value.hammer > maxPowerHammer) maxPowerHammer = kvp.Value.hammer;
				if (kvp.Value.mana > maxManaCost) maxManaCost = kvp.Value.mana;
			}
		}

		public void FillTooltip(string cursorText)
		{
			if (API.main.mouseNPC > -1) return;
			if (cursorText == null) return;

			tip.scale = (float)options["ttipScale"].Value;
			tip.alpha = null;
			if ((bool)options["ttipBackground"].Value) tip.alpha = .785f;

			if (!Main.toolTip.item.IsBlank())
			{
				ETipStyle style = ETipStyle.map[(string)options["itemStyle"].Value];
				foreach (Module<Item> module in modulesItems) module.ModifyTip(style, options, tip, Main.toolTip.item);
			}
		}
	}
}