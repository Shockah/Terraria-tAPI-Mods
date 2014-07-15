using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shockah.ETooltip
{
	public class ETipStyle
	{
		internal static readonly Dictionary<string, ETipStyle> map = new Dictionary<string, ETipStyle>();
		internal static readonly ETipStyle
			Vanilla = new ETipStyle("Vanilla"),
			TwoCols = new ETipStyle("2 columns");
		
		private ETipStyle(string optionValueName)
		{
			map[optionValueName] = this;
		}
	}
}