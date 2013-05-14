using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Components
{
	class Tilemap: Component
	{
		public enum TilemapType {SingleTilemap, RoadTilemap, FinalTilemap, WallTilemap};

		public Tilemap(int p_columns, int p_rows, int p_tilewidth, int p_tileheight, TilemapType p_tilemapType, int p_fill = -1)
		{
			columns = p_columns;
			rows = p_rows;
			tilewidth = p_tilewidth;
			tileheight = p_tileheight;
			map = new int[p_columns, p_rows];
			for (int y = 0; y < p_rows; y++)
			{
				for (int x = 0; x < p_columns; x++)
				{
					map[x, y] = p_fill;
				}
			}

			type = p_tilemapType;
		}

		public int[] getTilePosition(float p_x, float p_y)
		{
			int[] tilePosition = new int[2];
			if (p_x <  + (float)(columns * tilewidth) &&
				p_y <  + (float)(rows * tileheight))
			{
				tilePosition[0] = (int)((p_x) / (float)(tilewidth));
				tilePosition[1] = (int)((p_y) / (float)(tileheight));
			}
			else
			{
				tilePosition[0] = -1;
				tilePosition[1] = -1;
			}

			return tilePosition;
		}

		public int[] getTilePosition(Vector2 p_pos)
		{
			return getTilePosition(p_pos.X, p_pos.Y);
		}

		public Vector2 getPosition(int p_tileX, int p_tileY)
		{
			Vector2 position = new Vector2(0.0f, 0.0f);
			if (p_tileX >= 0 && p_tileX < columns &&
				p_tileY >= 0 && p_tileY < rows)
			{
				position.X = p_tileX * tilewidth;
				position.Y = p_tileY * tileheight;
			}
			return position;
		}

		public int getColumns()
		{
			return columns;
		}

		public int getRows()
		{
			return rows;
		}

		public int getState(int p_x, int p_y)
		{
			int state = -1;
			if (p_x >= 0 && p_x < columns &&
				p_y >= 0 && p_y < rows)
			{
				state = map[p_x, p_y];
			}
			return state;
		}

		public void setState(int p_x, int p_y, int p_state)
		{
			if (p_x >= 0 && p_x < columns &&
				p_y >= 0 && p_y < rows)
			{
				map[p_x, p_y] = p_state;
			}
		}

		public void clear()
		{
			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < columns; x++)
				{
					map[x, y] = -1;
				}
			}
		}

		public bool isWalkable(int p_x, int p_y)
		{
			int state = getState(p_x, p_y);
			if(state >= 90 && state < 30*17)
				return true;
			return false;
		}

		public  bool connectedTo(int p_x, int p_y, int p_rangeFrom, int p_rangeTo)
		{
			int[] states = new int[4] {
				getState(p_x, p_y - 1),
				getState(p_x + 1, p_y),
				getState(p_x, p_y + 1),
				getState(p_x - 1, p_y)
			};

			for (int i = 0; i < 4; i++)
			{
				if(states[i] >= p_rangeFrom && states[i] < p_rangeTo)
					return true;
			}

			return false;
		}
		public TilemapType getType()
		{
			return type;
		}
		
		private int columns;
		private int rows;
		private int tilewidth;
		private int tileheight;
		private int[,] map;
		private TilemapType type;
	}
}
