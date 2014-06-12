using TAPI;
using Terraria;

namespace Shockah.FIRE
{
	public class FIRE : ModItem
	{
		public FIRE(ModBase modBase, Item item) : base(modBase, item) { }

		public override bool? UseItem(Player player)
		{
			if (MWorld.me.instances.Count >= (int)MWorld.me.modBase.options["maxInstances"].Value) return false;
			MWorld.me.instances.Add(new FIREInstance((int)(Main.mouseX + Main.screenPosition.X) / 16, (int)(Main.mouseY + Main.screenPosition.Y) / 16));
			return true;
		}
	}
}