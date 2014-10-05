using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Insight
{
	public class MPlayer : ModPlayer
	{
		public Dictionary<int, List<ChestCache>> visited = new Dictionary<int,List<ChestCache>>();

		public void MarkAsVisited()
		{
			MarkAsVisited(player.chestObj);
		}
		public void MarkAsVisited(Chest c)
		{
			if (c == null) return;
			if (!visited.ContainsKey(Main.worldID)) visited.Add(Main.worldID, new List<ChestCache>());
			List<ChestCache> caches = visited[Main.worldID];
			foreach (ChestCache cache in caches) if (cache.x == c.x && cache.y == c.y) return;
			ChestCache cacheNew = new ChestCache(c.x, c.y);
			cacheNew.Prepare();
			caches.Add(cacheNew);
		}
		public ChestCache VisitedCache(Chest c)
		{
			if (c == null) return null;
			if (!(bool)MBase.me.options["visitedOnly"].Value) MarkAsVisited(c);
			if (!visited.ContainsKey(Main.worldID)) return null;
			List<ChestCache> caches = visited[Main.worldID];
			foreach (ChestCache cache in caches) if (cache.x == c.x && cache.y == c.y) return cache;
			return null;
		}
		public bool IsVisited(Chest c)
		{
			return VisitedCache(c) != null;
		}

		public override void Save(BinBuffer bb)
		{
			if (visited == null)
			{
				bb.Write(0);
				return;
			}
			
			bb.Write(visited.Count);
			foreach (KeyValuePair<int, List<ChestCache>> kvp in visited)
			{
				bb.Write(kvp.Key);
				bb.Write(kvp.Value.Count);
				foreach (ChestCache cache in kvp.Value)
				{
					bb.WriteX((ushort)cache.x, (ushort)cache.y);
					List<Item> cachedItems = cache.GetItems();
					if (cachedItems == null)
					{
						bb.Write((byte)0);
						goto L;
					}
					bb.Write((byte)cachedItems.Count);
					foreach (Item cached in cachedItems)
					{
						bb.Write(cached.name);
						bb.Write(cached.stack);
					}
					L: { }
				}
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
					List<ChestCache> caches = new List<ChestCache>();
					int count2 = bb.ReadInt();
					while (count2-- > 0)
					{
						ChestCache cacheNew = new ChestCache(bb.ReadUShort(), bb.ReadUShort());
						int count3 = bb.ReadByte();
						while (count3-- > 0)
						{
							Item itemNew = new Item();
							itemNew.SetDefaults(bb.ReadString());
							itemNew.stack = bb.ReadInt();
							cacheNew.AddItemAtLoad(itemNew);
						}
						caches.Add(cacheNew);
					}
					visited.Add(key, caches);
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