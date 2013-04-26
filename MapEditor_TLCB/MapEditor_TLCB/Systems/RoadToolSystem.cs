using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MapEditor_TLCB.CustomControls;

namespace MapEditor_TLCB.Systems
{
	class RoadToolSystem: EntitySystem
	{
		public RoadToolSystem(): base(typeof(Tilemap))
		{
		}

		public override void Initialize()
		{
			m_tilemapMapper = new ComponentMapper<Tilemap>(world);
			m_toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
		}

		public override void OnRemoved(Artemis.Entity e)
		{
			if (e.Tag == "mainTilemap") {
				mainTilemap = null;
			}
			else if (e.Tag == "singlesTilemap")
			{
				singlesTilemap = null;
			}
			else if (e.Tag == "roadTilemap")
			{
				roadTilemap = null;
			}
			else if (e.Tag == "wallTilemap")
			{
				wallTilemap = null;
			}
		}

		public override void Added(Entity e)
		{
			if (e.Tag == "mainTilemap") {
				mainTilemap = m_tilemapMapper.Get(e);
				canvasTransform = e.GetComponent<Transform>();
			}
			else if (e.Tag == "singlesTilemap")
			{
				singlesTilemap = m_tilemapMapper.Get(e);
			}
			else if (e.Tag == "roadTilemap") {
				roadTilemap = m_tilemapMapper.Get(e);
			}
			else if (e.Tag == "wallTilemap") {
				wallTilemap = m_tilemapMapper.Get(e);
			}
		}

		public override void Process()
		{
			Tool currentTool = m_toolSys.GetCurrentTool();
			if (mainTilemap != null && roadTilemap != null && wallTilemap != null &&
				canvasTransform != null)
			{
				Vector2 mousePos = new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y);
				mousePos -= canvasTransform.position;
				if (Mouse.GetState().LeftButton == ButtonState.Pressed)
				{
					if (currentTool == Tool.ROAD_TOOL)
					{
						int[] mapPos = roadTilemap.getTilePosition(mousePos);
						roadTilemap.setState(mapPos[0], mapPos[1], 0);
					}
					else if (currentTool == Tool.PAINT_TOOL)
					{
						int[] mapPos = roadTilemap.getTilePosition(mousePos);
						singlesTilemap.setState(mapPos[0], mapPos[1], 0);
						roadTilemap.setState(mapPos[0], mapPos[1], -1);
					}
					else if (currentTool == Tool.ERASE_TOOL)
					{
						int[] mapPos = roadTilemap.getTilePosition(mousePos);
						singlesTilemap.setState(mapPos[0], mapPos[1], -1);
						roadTilemap.setState(mapPos[0], mapPos[1], -1);
					}
				}

				generateWallmapFromRoadmap(wallTilemap, roadTilemap);

				RoadAndWallMapperSystem mapperSystem =
					(RoadAndWallMapperSystem)world.SystemManager.GetSystem<
					RoadAndWallMapperSystem>()[0];
				// Write to the resulting tilemap
				updateTilemapUsingRoadmap(mainTilemap, roadTilemap, 
					mapperSystem.getRoadMapper());
				updateTilemapUsingWallmap(mainTilemap, wallTilemap, roadTilemap, 
					mapperSystem.getWallMapper());
				updateTilemapUsingSingles(mainTilemap, singlesTilemap);
			}
		}

		private void updateTilemapUsingSingles(Tilemap p_mainTilemap, Tilemap p_singlesTilemap)
		{
			for (int y = 0; y < p_mainTilemap.getRows(); y++)
			{
				for (int x = 0; x < p_mainTilemap.getColumns(); x++)
				{
					if (p_singlesTilemap.getState(x, y) >= 0)
					{
						p_mainTilemap.setState(x, y, p_singlesTilemap.getState(x, y));
					}
				}
			}
		}

		private void updateTilemapUsingRoadmap(Tilemap p_tilemap, Tilemap p_roadMap, RoadMapper p_roadMapper)
		{
			for (int y = 0; y < p_roadMap.getRows(); y++)
			{
				for (int x = 0; x < p_roadMap.getColumns(); x++)
				{
					if (p_roadMap.getState(x, y) >= 0)
					{
						p_tilemap.setState(x, y, p_roadMapper.getContactType(x, y, p_roadMap));
					}
					else if (p_roadMap.getState(x, y) == -1)
					{
						p_tilemap.setState(x, y, -1);
					}
				}
			}
		}

		private void updateTilemapUsingWallmap(Tilemap p_tilemap, Tilemap p_wallMap,
			Tilemap p_roadMap, WallMapper p_wallMapper)
		{
			for (int y = 0; y < p_wallMap.getRows(); y++)
			{
				for (int x = 0; x < p_wallMap.getColumns(); x++)
				{
					if (p_wallMap.getState(x, y) >= 0)
					{
						p_tilemap.setState(x, y, p_wallMapper.getContactType(x, y, p_roadMap));
					}
				}
			}
		}

		private void generateWallmapFromRoadmap(Tilemap p_wallMap, Tilemap p_roadMap)
		{
			p_wallMap.clear();
			// Sweep adding walls
			for (int y = 0; y < p_roadMap.getRows(); y++)
			{
				for (int x = 0; x < p_roadMap.getColumns(); x++)
				{
					if (p_roadMap.getState(x, y) >= 0)
					{
						p_wallMap.setState(x - 1, y - 1, 1);
						p_wallMap.setState(x, y - 1, 1);
						p_wallMap.setState(x + 1, y - 1, 1);
						p_wallMap.setState(x + 1, y, 1);
						p_wallMap.setState(x + 1, y + 1, 1);
						p_wallMap.setState(x, y + 1, 1);
						p_wallMap.setState(x - 1, y + 1, 1);
						p_wallMap.setState(x - 1, y, 1);
					}
				}
			}
			// Sweep removing on roads
			for (int y = 0; y < p_roadMap.getRows(); y++)
			{
				for (int x = 0; x < p_roadMap.getColumns(); x++)
				{
					if (p_roadMap.getState(x, y) >= 0)
					{
						p_wallMap.setState(x, y, -1);
					}
				}
			}
		}
		
		Tilemap mainTilemap;
		Transform canvasTransform;
		Tilemap singlesTilemap;
		Tilemap roadTilemap;
		Tilemap wallTilemap;
		ComponentMapper<Tilemap> m_tilemapMapper;
		CurrentToolSystem m_toolSys;
	}
}
