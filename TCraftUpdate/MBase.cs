using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TAPI;

namespace TAPI.TCraftUpdate
{
	public class MBase : ModBase
	{
		public override void OnLoad()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append("$data = array(\n");
			for (int i = 0; i < Recipe.numRecipes; i++)
			{
				Recipe r = Main.recipe[i];
				if (i != 0) sb.Append(",\n");
				sb.Append("\tarray(");
				sb.Append("\"result\" => \"" + r.createItem.displayName.Replace("\"", "\\\"") + "\", ");
				if (r.createItem.stack > 1) sb.Append("\"resultc\" => " + r.createItem.stack + ", ");
				
				sb.Append("\"items\" => array(");
				for (int j = 0; j < r.requiredItem.Length; j++)
				{
					if (r.requiredItem[j].IsBlank()) break;
					if (j != 0) sb.Append(", ");
					sb.Append("\"" + r.requiredItem[j].displayName.Replace("\"","\\\"") + "\" => " + r.requiredItem[j].stack);
				}
				sb.Append(")");
				
				List<string> listTiles = new List<string>();
				for (int j = 0; j < r.requiredTile.Length; j++)
				{
					if (r.requiredTile[j] == -1) break;
					listTiles.Add("\"" + TileDef.displayName[r.requiredTile[j]].Replace("\"","\\\"") + "\"");
				}
				if (r.needWater) listTiles.Add("\"Water\"");
				if (r.needLava) listTiles.Add("\"Lava\"");
				if (r.needHoney) listTiles.Add("\"Honey\"");
				if (listTiles.Count != 0)
				{
					sb.Append(", \"at\" => array(");
					for (int j = 0; j < listTiles.Count; j++)
					{
						if (j != 0) sb.Append(", ");
						sb.Append(listTiles[j]);
					}
					sb.Append(")");
				}
				
				sb.Append(")");
			}
			sb.Append("\n);");
			
			File.WriteAllText("C:\\result.txt", sb.ToString());
			TConsole.Track("Finished!", "tcraftupdate", 300);
		}
	}
}