using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.AccSlots
{
	public class PLDyeAccessories : PlayerLayer
	{
		public PLDyeAccessories(ModBase modBase) : base(modBase.mod.InternalName + ":DyeAccessories") { }

		protected override void OnDraw(Player player, SpriteBatch sb)
		{
			ExtraDrawInfo edi = PlayerLayer.extraDrawInfo;

			MPlayer mp = player.GetSubClass<MPlayer>();
			for (int i = 0; i < mp.extraItem.Length * 2; i++)
			{
				bool vanity = i >= mp.extraItem.Length;
				Item item = mp.extraItem[i % mp.extraItem.Length];
				Item social = mp.extraSocial[i % mp.extraItem.Length];
				Item dye = mp.extraDye[i % mp.extraItem.Length];
				Item current = vanity ? social : item;
				bool hideVisual = vanity ? false : !mp.visibility[i];

				if (!dye.IsBlank() && !current.IsBlank() && (vanity || !hideVisual || current.wingSlot > 0 || current.type == 934))
				{
					if (current.handOnSlot > 0)
					{
						edi.dyeHandOn = dye.dye;
					}
					if (current.handOffSlot > 0)
					{
						edi.dyeHandOff = dye.dye;
					}
					if (current.backSlot > 0)
					{
						edi.dyeBackSlot = dye.dye;
					}
					if (current.frontSlot > 0)
					{
						edi.dyeFrontSlot = dye.dye;
					}
					if (current.shoeSlot > 0)
					{
						edi.dyeShoeSlot = dye.dye;
					}
					if (current.waistSlot > 0)
					{
						edi.dyeWaistSlot = dye.dye;
					}
					if (current.shieldSlot > 0)
					{
						edi.dyeShieldSlot = dye.dye;
					}
					if (current.neckSlot > 0)
					{
						edi.dyeNeckSlot = dye.dye;
					}
					if (current.faceSlot > 0)
					{
						edi.dyeFaceSlot = dye.dye;
					}
					if (current.balloonSlot > 0)
					{
						edi.dyeBalloonSlot = dye.dye;
					}
					if (current.wingSlot > 0)
					{
						edi.dyeWingSlot = dye.dye;
					}
					if (current.type == 934)
					{
						edi.dyeCarpet = dye.dye;
					}
				}
			}
		}
	}
}