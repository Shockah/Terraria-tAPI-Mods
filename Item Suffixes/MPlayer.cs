using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	public class MPlayer : ModPlayer
	{
		public override void MidUpdate()
		{
			Item heldItem = player.heldItem, tooltip = Main.toolTip;
			MItem mheldItem = null, mtooltip = null;
			if (heldItem != null && !heldItem.IsBlank())
			{
				mheldItem = heldItem.GetSubClass<MItem>();
				if (mheldItem != null)
				{
					if (mheldItem.resetDamage != 0)
					{
						heldItem.damage = mheldItem.resetDamage;
						heldItem.crit = mheldItem.resetCrit;
					}

					mheldItem.resetDamage = heldItem.damage;
					mheldItem.resetCrit = heldItem.crit;
				}
			}

			if (tooltip != null && !tooltip.IsBlank())
			{
				mtooltip = tooltip.GetSubClass<MItem>();
				if (mtooltip != null)
				{
					if (mtooltip.resetDamage != 0)
					{
						tooltip.damage = mheldItem.resetDamage;
						tooltip.crit = mheldItem.resetCrit;
					}

					mtooltip.resetDamage = tooltip.damage;
					mtooltip.resetCrit = tooltip.crit;
				}
			}
			
			for (int i = 0; i < 8; i++)
			{
				Item item = player.armor[i];
				if (!item.IsBlank())
				{
					MItem mitem = item.GetSubClass<MItem>();
					if (mitem != null)
					{
						if (mitem.suffix != null)
						{
							if (!heldItem.IsBlank() && mheldItem != null)
							{
								if (heldItem.damage > 0)
								{
									if (heldItem.melee)
									{
										heldItem.damage = mitem.suffix.BonusDamageMelee(heldItem.damage);
										heldItem.crit = mitem.suffix.BonusCritMelee(heldItem.crit);
									}
									if (heldItem.ranged)
									{
										heldItem.damage = mitem.suffix.BonusDamageRanged(heldItem.damage);
										heldItem.crit = mitem.suffix.BonusCritRanged(heldItem.crit);
									}
									if (heldItem.magic || heldItem.summon)
									{
										heldItem.damage = mitem.suffix.BonusDamageMagic(heldItem.damage);
										heldItem.crit = mitem.suffix.BonusCritMagic(heldItem.crit);
									}
								}
							}
							if (!tooltip.IsBlank() && mtooltip != null)
							{
								if (tooltip.damage > 0)
								{
									if (tooltip.melee)
									{
										tooltip.damage = mtooltip.suffix.BonusDamageMelee(tooltip.damage);
										tooltip.crit = mtooltip.suffix.BonusCritMelee(tooltip.crit);
									}
									if (tooltip.ranged)
									{
										tooltip.damage = mtooltip.suffix.BonusDamageRanged(tooltip.damage);
										tooltip.crit = mtooltip.suffix.BonusCritRanged(tooltip.crit);
									}
									if (tooltip.magic || tooltip.summon)
									{
										tooltip.damage = mtooltip.suffix.BonusDamageMagic(tooltip.damage);
										tooltip.crit = mtooltip.suffix.BonusCritMagic(tooltip.crit);
									}
								}
							}

							player.statDefense = mitem.suffix.BonusDefense(player.statDefense);
							player.aggro = mitem.suffix.BonusThreat(player.aggro);
							player.lifeRegen = mitem.suffix.BonusRegenHP(player.lifeRegen);
						}
					}
				}
			}
		}
	}
}