using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.CavernsOfTime
{
	public class WGTCaverns : WorldGenTask
	{
		private static readonly string TemplateTiles =
			@"                 +                 " + '\n' +
			@"            ###########            " + '\n' +
			@"           XX....|....XX           " + '\n' +
			@"          XX.....|.....XX          " + '\n' +
			@"         XX....*.|.*....XX         " + '\n' +
			@"        XX.......|.......XX        " + '\n' +
			@"       XX...._...|..._....XX       " + '\n' +
			@"      ##########.|.##########      " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #*|*#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #*|*#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"               #.|.#               " + '\n' +
			@"################.|.################" + '\n' +
			@"#......#####.....|.....#####......#" + '\n' +
			@"#................|................#" + '\n' +
			@"#.*..*...........|...........*..*.#" + '\n' +
			@"#..OO...._.......|......._....OO..#" + '\n' +
			@"#..OO../###\....._...../###\..OO..#" + '\n' +
			@"###################################";
		private static readonly string TemplateWalls =
			@"                 +                 " + '\n' +
			@"            ###########            " + '\n' +
			@"           XX.........XX           " + '\n' +
			@"          XX...........XX          " + '\n' +
			@"         XX.............XX         " + '\n' +
			@"        XX...............XX        " + '\n' +
			@"       XX.................XX       " + '\n' +
			@"      ##########...##########      " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"               #...#               " + '\n' +
			@"################...################" + '\n' +
			@"#111111#####...........#####222222#" + '\n' +
			@"#111111.....................222222#" + '\n' +
			@"#111111.....................222222#" + '\n' +
			@"#111111.....................222222#" + '\n' +
			@"#111111.###.............###.222222#" + '\n' +
			@"###################################";
		private static readonly string TemplateWires =
			@"                 *                 " + '\n' +
			@"            ***********            " + '\n' +
			@"           **         **           " + '\n' +
			@"          **           **          " + '\n' +
			@"         **  ***   ***  **         " + '\n' +
			@"        **   *       *   **        " + '\n' +
			@"       **    *       *    **       " + '\n' +
			@"      ##########   ##########      " + '\n' +
			@"               #   #               " + '\n' +
			@"               #   #               " + '\n' +
			@"               #***#               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"               #***#               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"               # * #               " + '\n' +
			@"################ * ################" + '\n' +
			@"#      #####     *     #####      #" + '\n' +
			@"#                *                #" + '\n' +
			@"# ********       *       ******** #" + '\n' +
			@"#        *       *       *        #" + '\n' +
			@"#      /###\     *     /###\      #" + '\n' +
			@"###################################";
		private const int BaseLevel = 7;

		private readonly ModBase modBase;

		public WGTCaverns(ModBase modBase) : base(modBase.modName)
		{
			this.modBase = modBase;
		}
		
		public override void Generate()
		{
			Main.statusText = "Building the Caverns of Time...";

			const int TILE_BASE = 175, TILE_PRESSURE = 135, TILE_SWITCH = 136, TILE_TORCH = 4, TILE_GRAVE = 85, TILE_ROPE = 213;
			const int FRAMEX_TORCH_OFF = 3, FRAMEX_TORCH_OFFL = 4, FRAMEX_TORCH_OFFR = 5, FRAMEX_GRAVE = 10;
			const int FRAMEY_PRESSURE = 2, FRAMEY_TORCH = 6;
			const int WALL_BASE = 45, WALL_ROOM1 = 113, WALL_ROOM2 = 31;

			string[] splTiles = TemplateTiles.Split('\n'), splWalls = TemplateWalls.Split('\n'), splWires = TemplateWires.Split('\n');

			int x = Main.maxTilesX / 2 + 50;
			x -= TemplateTiles.Split('\n')[0].Length / 2;

			int y = (int)Main.worldSurface;
			const int ISFINE_START = BaseLevel;
			int _isfine = ISFINE_START;
			while (--y > 0)
			{
				if (Main.tile[x, y] == null) Main.tile[x, y] = new Tile();
				if (Main.tile[x, y].active() || Main.tile[x, y].wall != 0 || Main.tile[x, y].liquid != 0)
				{
					_isfine = ISFINE_START;
					continue;
				}
				if (--_isfine == 0) break;
			}

			List<Tuple<int, int>> listFindFrames = new List<Tuple<int, int>>(), listTorches = new List<Tuple<int, int>>();
			for (int yy = splTiles.Length - 1; yy >= 0; yy--) for (int xx = 0; xx < splTiles[yy].Length; xx++)
			{
				char cTile = splTiles[yy][xx], cWall = splWalls[yy][xx], cWire = splWires[yy][xx];
				if (Main.tile[x + xx, y + yy] == null) Main.tile[x + xx, y + yy] = new Tile();

				if (cTile != ' ' && cTile != 'O')
				{
					WorldGen.KillTile(x + xx, y + yy, false, false, true);
					Main.tile[x + xx, y + yy].liquid = 0;
				}

				switch (cWall)
				{
					case '.':
						WorldGen.KillWall(x + xx, y + yy);
						WorldGen.PlaceWall(x + xx, y + yy, WALL_BASE, true);
						break;
					case '1':
						WorldGen.KillWall(x + xx, y + yy);
						WorldGen.PlaceWall(x + xx, y + yy, WALL_ROOM1, true);
						break;
					case '2':
						WorldGen.KillWall(x + xx, y + yy);
						WorldGen.PlaceWall(x + xx, y + yy, WALL_ROOM2, true);
						break;
				}

				switch (cTile)
				{
					case '#':
						WorldGen.PlaceTile(x + xx, y + yy, TILE_BASE, true, true);
						break;
					case '/':
						WorldGen.PlaceTile(x + xx, y + yy, TILE_BASE, true, true);
						Main.tile[x + xx, y + yy].slope(2);
						break;
					case '\\':
						WorldGen.PlaceTile(x + xx, y + yy, TILE_BASE, true, true);
						Main.tile[x + xx, y + yy].slope(1);
						break;
					case 'X':
						WorldGen.PlaceTile(x + xx, y + yy, TILE_BASE, true, true);
						WorldGen.PlaceActuator(x + xx, y + yy);
						break;
					case '*':
						listTorches.Add(new Tuple<int, int>(x + xx, y + yy));
						break;
					case '|':
						WorldGen.PlaceTile(x + xx, y + yy, TILE_ROPE, true, true);
						listFindFrames.Add(new Tuple<int, int>(x + xx, y + yy));
						break;
					case '_':
						WorldGen.PlaceTile(x + xx, y + yy, TILE_PRESSURE, true, true);
						Main.tile[x + xx, y + yy].frameY = (short)(FRAMEY_PRESSURE * 18);
						break;
					case '+':
						WorldGen.PlaceTile(x + xx, y + yy, TILE_SWITCH, true, true);
						break;
					case 'O':
						if (xx != splTiles[yy].Length - 1 && yy != splTiles.Length - 1 && splTiles[yy + 1][xx + 1] == 'O')
						{
							for (int yyy = 1; yyy >= 0; yyy--) for (int xxx = 0; xxx <= 1; xxx++) WorldGen.KillTile(x + xx + xxx, y + yy + yyy, false, false, true);
							WorldGen.PlaceTile(x + xx, y + yy + 1, TILE_GRAVE);
							for (int yyy = 1; yyy >= 0; yyy--) for (int xxx = 0; xxx <= 1; xxx++) Main.tile[x + xx + xxx, y + yy + yyy].frameX += (short)(FRAMEX_GRAVE * 18);
							for (int i = 0; i < Main.sign.Length; i++)
							{
								if (Main.sign[i] == null)
								{
									Main.sign[i] = new Sign();
									Main.sign[i].text = "";
								}
								if (Main.sign[i].x <= 0)
								{
									Main.sign[i].x = x + xx;
									Main.sign[i].y = y + yy;
									MWorld modWorld = (MWorld)modBase.modWorld;
									if (Main.tile[x + xx, y + yy].wall == WALL_ROOM1)
									{
										modWorld.room1X = x + xx;
										modWorld.room1Y = y + yy;
									}
									else if (Main.tile[x + xx, y + yy].wall == WALL_ROOM2)
									{
										modWorld.room2X = x + xx;
										modWorld.room2Y = y + yy;
									}
									break;
								}
							}
						}
						break;
				}

				switch (cWire)
				{
					case '*':
						if (Main.tile[x + xx, y + yy] == null) Main.tile[x + xx, y + yy] = new Tile();
						Main.tile[x + xx, y + yy].wire(true);
						break;
				}
			}

			foreach (Tuple<int, int> pos in listFindFrames) WorldGen.TileFrame(pos.Item1, pos.Item2);
			foreach (Tuple<int, int> pos in listTorches)
			{
				if (Main.tile[pos.Item1 - 1, pos.Item2] == null) Main.tile[pos.Item1 - 1, pos.Item2] = new Tile();
				if (Main.tile[pos.Item1 + 1, pos.Item2] == null) Main.tile[pos.Item1 + 1, pos.Item2] = new Tile();

				WorldGen.PlaceTile(pos.Item1, pos.Item2, TILE_TORCH, true, true);
				Main.tile[pos.Item1, pos.Item2].frameY = (short)(FRAMEY_TORCH * 22);

				int side = (Main.tile[pos.Item1 + 1, pos.Item2].active() ? 1 : 0) - (Main.tile[pos.Item1 - 1, pos.Item2].active() ? 1 : 0);
				Main.tile[pos.Item1, pos.Item2].frameX = (short)((side == 0 ? FRAMEX_TORCH_OFF : (side == -1 ? FRAMEX_TORCH_OFFL : FRAMEX_TORCH_OFFR)) * 22);
			}
		}
	}
}