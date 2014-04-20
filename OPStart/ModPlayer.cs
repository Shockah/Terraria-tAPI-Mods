using System;
using System.Collections.Generic;
using System.Text;

using TAPI;

namespace TAPI.OPStart
{
	public class ModPlayer : TAPI.ModPlayer
	{
		public ModPlayer(TAPI.ModBase modBase,Player player) : base(modBase,player) {}
		
		public override void OnInventoryReset(bool onMediumcoreRespawn)
		{
			if (!onMediumcoreRespawn)
			{
				player.statLifeMax = 500;
				player.statManaMax = 200;
			}
			
			int slot = 3;
			player.inventory[slot].  SetDefaults("Ankh Shield");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Magic Cuffs");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Magnet Sphere");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Vampire Knives");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("S.D.M.G.");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Chlorophyte Bullet");
			player.inventory[slot++].stack = 999;
			
			player.inventory[slot].  SetDefaults("Chlorophyte Bullet");
			player.inventory[slot++].stack = 999;
			
			player.inventory[slot].  SetDefaults("Terra Blade");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Fairy Wings");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Nimbus Rod");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Bundle of Balloons");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Rainbow Gun");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Rod of Discord");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Golden Shower");
			player.inventory[slot++].Prefix(-1);
			
			player.inventory[slot].  SetDefaults("Sparkfrost Boots");
			player.inventory[slot++].Prefix(-1);
		}
	}
}