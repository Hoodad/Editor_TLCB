using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB.Systems
{
	class MapValidationSystem: EntitySystem
	{
		public MapValidationSystem()
			: base(typeof(Tilemap), typeof(TilemapValidate))
		{
		}

		protected override void ProcessEntities(Dictionary<int, Entity> entities)
		{
			foreach (Entity e in entities.Values)
			{
				Tilemap tilemap = m_tilemapMapper.Get(e);
				TilemapValidate valid = m_validateMapper.Get(e);

				valid.pathsValid = validatePaths(tilemap);
			}
		}

		private bool validatePaths(Tilemap p_tilemap)
		{
			List<int[]> walkables = new List<int[]>();
			for (int y = 0; y < p_tilemap.getRows(); y++)
			{
				for (int x = 0; x < p_tilemap.getColumns(); x++)
				{
					if (p_tilemap.isWalkable(x, y))
					{
						walkables.Add(new int[2]{x, y});
					}
				}
			}

			if (walkables.Count > 0)
			{
				List<int[]> toCheck = new List<int[]>();
				toCheck.Add(walkables[0]);
				walkables.RemoveAt(0);

				while (toCheck.Count > 0)
				{
					int[] pos = toCheck[0];
					toCheck.RemoveAt(0);
					if (p_tilemap.isWalkable(pos[0], pos[1] - 1)) // North
					{
						int index = walkables.FindIndex(delegate(int[] p){return p[0]==pos[0] && p[1] == pos[1] - 1;});
						if (index != -1)
						{
							walkables.RemoveAt(index);
							toCheck.Add(new int[2]{pos[0], pos[1] - 1});
						}
					}
					if (p_tilemap.isWalkable(pos[0] + 1, pos[1])) // West
					{
						int index = walkables.FindIndex(delegate(int[] p){return p[0]==pos[0] + 1 && p[1] == pos[1];});
						if (index != -1)
						{
							walkables.RemoveAt(index);
							toCheck.Add(new int[2]{pos[0] + 1, pos[1]});
						}
					}
					if (p_tilemap.isWalkable(pos[0], pos[1] + 1)) // South
					{
						int index = walkables.FindIndex(delegate(int[] p){return p[0]==pos[0] && p[1] == pos[1] + 1;});
						if (index != -1)
						{
							walkables.RemoveAt(index);
							toCheck.Add(new int[2]{pos[0], pos[1] + 1});
						}
					}
					if (p_tilemap.isWalkable(pos[0] - 1, pos[1])) // West
					{
						int index = walkables.FindIndex(delegate(int[] p){return p[0]==pos[0] - 1 && p[1] == pos[1];});
						if (index != -1)
						{
							walkables.RemoveAt(index);
							toCheck.Add(new int[2]{pos[0] - 1, pos[1]});
						}
					}
				}
			}

			if(walkables.Count == 0)
				return true;
			return false;
		}

		public override void Initialize()
		{
			m_tilemapMapper = new ComponentMapper<Tilemap>(world);
			m_validateMapper = new ComponentMapper<TilemapValidate>(world);
		}

		ComponentMapper<Tilemap> m_tilemapMapper;
		ComponentMapper<TilemapValidate> m_validateMapper;
	}
}
