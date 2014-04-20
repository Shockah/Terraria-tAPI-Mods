using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.CavernsOfTime
{
	public class MPlayer : ModPlayer
	{
		public const int FULL_DAY = 86400, DAYS = 3, DAYS_COOLDOWN = 14;
		
		public MPlayer(ModBase modBase, Player player) : base(modBase, player) { }

		public override void PostUpdate()
		{
			if (player.sign != -1)
			{
				Sign sign = Main.sign[player.sign];
				MWorld modWorld = (MWorld)modBase.modWorld;
				if (sign.x == modWorld.room1X && sign.y == modWorld.room1Y)
				{
					player.sign = -1;
					if (modWorld.cooldownHalloween == 0)
					{
						modWorld.forceHalloween = FULL_DAY * DAYS;
						modWorld.cooldownHalloween = FULL_DAY * DAYS_COOLDOWN;
						Main.NewText("The Keepers of Time have listened to your prayings!", 50, 255, 130);
					}
					else
					{
						if (modWorld.forceHalloween > 0)
						{
							int days = (int)Math.Ceiling(1f * modWorld.forceHalloween / FULL_DAY);
							Main.NewText("The magical force will be up for another " + days + " day" + (days == 1 ? "" : "s") + ".", 50, 255, 130);
						}
						else
						{
							int days = (int)Math.Ceiling(1f * modWorld.cooldownHalloween / FULL_DAY);
							Main.NewText("The Keepers of Time won't listen for another " + days + " day" + (days == 1 ? "" : "s") + ".", 50, 255, 130);
						}
					}
				}
				else if (sign.x == modWorld.room2X && sign.y == modWorld.room2Y)
				{
					player.sign = -1;
					if (modWorld.cooldownChristmas == 0)
					{
						modWorld.forceChristmas = FULL_DAY * DAYS;
						modWorld.cooldownChristmas = FULL_DAY * DAYS_COOLDOWN;
						Main.NewText("The Keepers of Time have listened to your prayings!", 50, 255, 130);
					}
					else
					{
						if (modWorld.forceChristmas > 0)
						{
							int days = (int)Math.Ceiling(1f * modWorld.forceChristmas / FULL_DAY);
							Main.NewText("The magical force will be up for another " + days + " day" + (days == 1 ? "" : "s") + ".", 50, 255, 130);
						}
						else
						{
							int days = (int)Math.Ceiling(1f * modWorld.cooldownChristmas / FULL_DAY);
							Main.NewText("The Keepers of Time won't listen for another " + days + " day" + (days == 1 ? "" : "s") + ".", 50, 255, 130);
						}
					}
				}
			}
		}
	}
}
