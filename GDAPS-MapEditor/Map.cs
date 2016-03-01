using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace GDAPSMapEditor
{
	public class Map
	{
		private Tile[,] tiles;
		private int width;
		private int height;
		private String superForeground;
		private String parallax;
		private String background;
		List<Entity> entities;

		public int Width
		{
			get
			{
				return width;
			}
		}

		public int Height
		{
			get
			{
				return height;
			}
		}

		public String SuperForeground
		{
			get
			{
				return superForeground;
			}
			set
			{
				superForeground = value;
			}
		}

		public String Parallax
		{
			get
			{
				return parallax;
			}
			set
			{
				parallax = value;
			}
		}

		public String Background
		{
			get
			{
				return background;
			}
			set
			{
				background = value;
			}
		}

		public const uint MAPEDITOR_VERSION = 3;

		public Map(int width = 32, int height = 32)
		{
			this.width = width;
			this.height = height;
			tiles = new Tile[width, height];
			for(int i = 0; i < width; ++i)
			{
				for(int j = 0; j < height; ++j)
				{
					tiles[i,j] = new Tile(MovementFlags.UP | MovementFlags.DOWN | MovementFlags.LEFT | MovementFlags.RIGHT,
					                      "transparent");
				}
			}
		}

		public void SetTile(int x, int y, Tile tile)
		{
			tiles[x,y] = tile;
		}

		public Tile GetTile(int x, int y)
		{
			return tiles[x,y];
		}

		public void AddEntity(Entity e)
		{
			entities.Add(e);
		}

		public void DeleteEntity(int x, int y)
		{
			for(int i = 0; i < entities.Count; ++i)
			{
				if(entities[i].X == x && entities[i].Y == y)
				{
					entities.RemoveAt(i);
				}
			}
		}

		public Tile this[int x, int y]
		{
			get
			{
				return GetTile(x, y);
			}
			set
			{
				SetTile(x, y, value);
			}
		}

		public void WriteMap(String name)
		{
			BinaryWriter output = new BinaryWriter(File.OpenWrite(name + ".map"));
			output.Write(MAPEDITOR_VERSION);
			output.Write(width);
			output.Write(height);
			for(int i = 0; i < width; ++i)
			{
				for(int j = 0; j < height; ++j)
				{
					output.Write((int)tiles[i,j].Flags);
					output.Write(tiles[i,j].Filename);
				}
			}
			output.Close();
		}

		public void LoadMap(String name)
		{
			BinaryReader input = new BinaryReader(File.OpenRead(name + ".map"));
			if(input.ReadUInt32() != MAPEDITOR_VERSION)
			{
				Console.WriteLine("Error: MapEditor version is wrong, map may not load correctly");
			}
			width = input.ReadInt32();
			height = input.ReadInt32();
			tiles = new Tile[width, height];
			for(int i = 0; i < width; ++i)
			{
				for(int j = 0; j < height; ++j)
				{
					tiles[i,j] = new Tile((MovementFlags)input.ReadInt32(), input.ReadString());
				}
			}
			input.Close();
		}
	}
}

