using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.ChestContents
{
	public class MPlayer : ModPlayer
	{
		public Dictionary<int, List<Point>> visited = new Dictionary<int, List<Point>>();
		
		public MPlayer(ModBase modBase, Player p) : base(modBase, p) { }

		public void MarkAsVisited()
		{
			Chest c = player.chestObj;
			if (c == null) return;
			
			if (!visited.ContainsKey(Main.worldID)) visited.Add(Main.worldID, new List<Point>());
			List<Point> points = visited[Main.worldID];
			if (!points.Contains(new Point(c.x, c.y))) points.Add(new Point(c.x, c.y));
		}
		public bool IsVisited(Chest c)
		{
			if (!visited.ContainsKey(Main.worldID)) return false;
			List<Point> points = visited[Main.worldID];
			return points.Contains(new Point(c.x, c.y));
		}

		public override void Save(BinBuffer bb)
		{
			bb.Write(visited.Count);
			foreach (KeyValuePair<int, List<Point>> kvp in visited)
			{
				bb.Write(kvp.Key);
				bb.Write(kvp.Value.Count);
				foreach (Point point in kvp.Value) bb.WriteX((ushort)point.X, (ushort)point.Y);
			}
		}

		public override void Load(BinBuffer bb)
		{
			visited.Clear();
			try
			{
				int count = bb.ReadInt();
				while (count-- > 0)
				{
					int key = bb.ReadInt();
					List<Point> points = new List<Point>();
					int count2 = bb.ReadInt();
					while (count2-- > 0) points.Add(new Point(bb.ReadUShort(), bb.ReadUShort()));
					visited.Add(key, points);
				}
			}
			catch (Exception) { }
		}

		public override void PostUpdate()
		{
			MarkAsVisited();
		}
	}
}