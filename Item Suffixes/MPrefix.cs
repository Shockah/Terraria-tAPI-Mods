using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	[GlobalMod] public class MPrefix : ModPrefix
	{
		public MPrefix(ModBase modBase) : base(modBase) { }

		public override void AdditionalTooltips(Item item, ref List<Tuple<string, bool>> current)
		{
			MItem m = item.GetSubClass<MItem>();
			if (m != null)
			{
				if (m.suffix != null)
				{
					List<string> ttips = m.suffix.AddTooltips();
					foreach (string ttip in ttips) current.Add(new Tuple<string, bool>(ttip, true));
				}
			}
		}
	}
}