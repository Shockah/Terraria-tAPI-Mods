using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.Insight
{
	public class ChestCache
	{
		public readonly int x, y;
		protected List<Item> items = new List<Item>();
		protected Chest chest = null;

		public ChestCache(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public void Prepare()
		{
			if (!InitChest()) return;
			CacheItems();
		}
		public List<Item> GetItems()
		{
			Prepare();
			return items;
		}
		internal void AddItemAtLoad(Item item)
		{
			items.Add(item);
		}

		protected bool InitChest()
		{
			if (chest != null) return true;
			if (Main.gameMenu) return true;

			Tile t = Main.tile[x, y];
			if (t.active() && t.type == ILChestContents.TILE_CHEST)
			{
				for (int i = 0; i < Main.chest.Length; i++)
				{
					Chest c = Main.chest[i];
					if (c == null) continue;
					if ((x == c.x || x == c.x + 1) && (y == c.y || y == c.y + 1) && Main.localPlayer.GetSubClass<MPlayer>().IsVisited(c))
					{
						chest = c;
						return true;
					}
				}
			}

			return chest != null;
		}
		protected void CacheItems()
		{
			if (Main.netMode != 0 && Main.localPlayer.chestObj == null) return;
			
			items.Clear();

			if (chest != null)
			{
				for (int i = 0; i < chest.item.Length; i++)
				{
					Item item = chest.item[i];
					if (item.IsBlank()) continue;

					foreach (Item item2 in items)
					{
						if (item.type == item2.type)
						{
							item2.stack += item.stack;
							goto L;
						}
					}

					Item itemNew = new Item().SetDefaults(item.name);
					itemNew.stack = item.stack;
					items.Add(itemNew);

					L: { }
				}
				HandleCoins();
			}
		}
		protected void HandleCoins()
		{
			int coins = 0;
			for (int i = 0; i < items.Count; i++)
			{
				Item item = items[i];
				int oldCoins = coins;
				if (item.name == "Copper Coin") coins += item.stack;
				else if (item.name == "Silver Coin") coins += item.stack * 100;
				else if (item.name == "Gold Coin") coins += item.stack * 100 * 100;
				else if (item.name == "Platinum Coin") coins += item.stack * 100 * 100 * 100;
				if (oldCoins != coins) items.RemoveAt(i--);
			}

			if (coins % 100 != 0) items.Add(new Item().SetDefaults("Vanilla:Copper Coin").SetStack(coins % 100));
			coins /= 100;

			if (coins % 100 != 0) items.Add(new Item().SetDefaults("Vanilla:Silver Coin").SetStack(coins % 100));
			coins /= 100;

			if (coins % 100 != 0) items.Add(new Item().SetDefaults("Vanilla:Gold Coin").SetStack(coins % 100));
			coins /= 100;

			if (coins % 100 != 0) items.Add(new Item().SetDefaults("Vanilla:Platinum Coin").SetStack(coins % 100));
			coins /= 100;
		}
	}
}