using TAPI;
using Terraria;

namespace Shockah.Base
{
	[GlobalMod] public class MItem : ModItem
	{
		internal static uint autoIncrement = 0;

		internal uint myId;

		public override void Initialize()
		{
			myId = ++autoIncrement;
		}
	}
}