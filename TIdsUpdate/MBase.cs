using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TAPI;

namespace TAPI.TIdsUpdate
{
	public class MBase : ModBase
	{
		public override void OnLoad()
		{
			StringBuilder sb = new StringBuilder();
			
			foreach (KeyValuePair<string, Item> kvp in Defs.items)
			{
				if (!kvp.Key.StartsWith("vanilla:")) continue;
				if (sb.Length != 0) sb.Append("\n");
				sb.Append(kvp.Key.Substring(8) + " = " + kvp.Value.netID);
			}
			
			File.WriteAllText("C:\\result.txt", sb.ToString());
			TConsole.Track("Finished!", "tidsupdate", 300);
		}
	}
}