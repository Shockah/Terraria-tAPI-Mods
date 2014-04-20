using TAPI;
using Terraria;

namespace Shockah.Base
{
	[GlobalMod] public class MItem : ModItem
	{
		internal static uint autoIncrement = 0;

		internal uint myId;
		
		public MItem(ModBase modBase, Item item) : base(modBase, item) { }

		public override void Initialize()
		{
			myId = ++autoIncrement;
		}
	}
}