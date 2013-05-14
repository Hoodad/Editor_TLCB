using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MapEditor_TLCB.CustomControls;
using MapEditor_TLCB.Systems.Interface;
using MapEditor_TLCB.Actions.Interface;
using MapEditor_TLCB.Actions;
using TomShane.Neoforce.Controls;

namespace MapEditor_TLCB.Systems
{
	class RoadToolSystem: EntitySystem, ActionSystemInterface
	{
		public RoadToolSystem(): base(typeof(Tilemap))
		{
			m_lmbPressed = false;
		}

		public override void Initialize()
		{
			m_tilemapMapper = new ComponentMapper<Tilemap>(world);
			m_toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
			m_drawCanvasSys = (DrawCanvasSystem)(world.SystemManager.GetSystem<DrawCanvasSystem>()[0]);
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
				Entity camera = world.TagManager.GetEntity("mainCamera");
				if (camera != null)
				{
					Transform camTransform = camera.GetComponent<Transform>();
					if (camTransform != null)
					{
						mousePos = Vector2.Transform(mousePos, Matrix.Invert(camTransform.getMatrix()));
					}
				}

				RadialMenuSystem rms = (RadialMenuSystem)(world.SystemManager.GetSystem<RadialMenuSystem>()[0]);

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

		public void canvasGroupBehavior(object sender, MouseEventArgs e)
		{
			if (e.State.LeftButton == ButtonState.Pressed && !m_lmbPressed)
			{
				m_lmbPressed = true;
				ActionSystem actionSys = ((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]);
                string actionstring = "Tool"; Tool currentTool = m_toolSys.GetCurrentTool();
                switch (currentTool)
                {
                    case Tool.ROAD_TOOL: actionstring = "Road"; break;
                    case Tool.ERASE_TOOL: actionstring = "Erase"; break;
                    case Tool.PAINT_TOOL: actionstring = "Paint"; break;
                    default: actionstring = "W-T-F!!!!!!"; break;
                }
				actionSys.StartGroupingActions(actionstring);
			}
			else if (e.State.LeftButton == ButtonState.Released && m_lmbPressed)
			{
				m_lmbPressed = false;
				ActionSystem actionSys = ((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]);
				actionSys.StopGroupingActions();
			}
		}
		public void canvasWindow_MouseMove(object sender, MouseEventArgs e)
		{
			Vector2 mousePos = new Vector2(e.Position.X, e.Position.Y);
			Entity camera = world.TagManager.GetEntity("mainCamera");
			if (camera != null)
			{
				Transform camTransform = camera.GetComponent<Transform>();
				if (camTransform != null)
				{
					mousePos = Vector2.Transform(mousePos, Matrix.Invert(camTransform.getMatrix()));
					m_drawCanvasSys.setLastMousePos(mousePos);
				}
			}

			if (mousePos.X >= 0.0f &&
				mousePos.X < mainTilemap.getColumns() * 32.0f &&
				mousePos.Y >= 0.0f &&
				mousePos.Y < mainTilemap.getRows() * 32.0f)
			{
				if (m_lmbPressed)
				{
					Tool currentTool = m_toolSys.GetCurrentTool();
					if (mainTilemap != null && roadTilemap != null && wallTilemap != null &&
						canvasTransform != null)
					{

						if (currentTool == Tool.ROAD_TOOL)
						{
							int[] mapPos = roadTilemap.getTilePosition(mousePos);
							ModifyTile changeTile = new ModifyTile(world.SystemManager);
							changeTile.col = mapPos[0];
							changeTile.row = mapPos[1];
							changeTile.state = 0;
							changeTile.affectedTilemap = roadTilemap.getType();

							if (roadTilemap.getState(mapPos[0], mapPos[1]) != 0)
							{
								ActionSystem actionSys = ((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]);
								actionSys.EnqueueAction(changeTile);
								overWriteSinglesWithWalls(mapPos);
							}
						}
						else if (currentTool == Tool.PAINT_TOOL)
						{
							int[] mapPos = roadTilemap.getTilePosition(mousePos);

							IntPair indexPair = m_toolSys.GetCurrentDrawTileIndex();
							Vector2 min = new Vector2(indexPair.i1 - 30 * (indexPair.i1 / 30), indexPair.i1 / 30);
							Vector2 max = new Vector2(indexPair.i2 - 30 * (indexPair.i2 / 30), indexPair.i2 / 30);

							for (int i = (int)min.X; i <= max.X; i++)
							{
								for (int j = (int)min.Y; j <= max.Y; j++)
								{
									int index = j * 30 + i;
									if (singlesTilemap.getState(mapPos[0]+i-(int)min.X, mapPos[1]+j-(int)min.Y) != index)
									{
										ActionSystem actionSys = ((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]);

										//actionSys.StartGroupingActions();
										ModifyTile changeTile = new ModifyTile(world.SystemManager);
										changeTile.col = mapPos[0] + i - (int)min.X;
										changeTile.row = mapPos[1] + j - (int)min.Y;
										changeTile.state = index;
										changeTile.affectedTilemap = singlesTilemap.getType();
										actionSys.EnqueueAction(changeTile);

										//ModifyTile roadChangeTile = new ModifyTile(world.SystemManager);
										//roadChangeTile.col = changeTile.col;
										//roadChangeTile.row = changeTile.row;
										//roadChangeTile.state = -1;
										//roadChangeTile.affectedTilemap = roadTilemap.getType();
										//actionSys.EnqueueAction(roadChangeTile);
									}
								}
							}
						}
						else if (currentTool == Tool.ERASE_TOOL)
						{
							int[] mapPos = roadTilemap.getTilePosition(mousePos);

							ModifyTile roadModify = new ModifyTile(world.SystemManager);
							roadModify.affectedTilemap = roadTilemap.getType();
							roadModify.col = mapPos[0];
							roadModify.row = mapPos[1];
							roadModify.state = -1;

							ModifyTile singlesModify = new ModifyTile(world.SystemManager);
							singlesModify.affectedTilemap = singlesTilemap.getType();
							singlesModify.col = mapPos[0];
							singlesModify.row = mapPos[1];
							singlesModify.state = -1;

							ActionSystem actionSys = ((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]);
							actionSys.EnqueueAction(roadModify);
							actionSys.EnqueueAction(singlesModify);
						}
					}
				}
			}
		}

		private void overWriteSinglesWithWalls(int[] p_mapPos)
		{
			ModifyTile[] changeSingles = new ModifyTile[9];
			changeSingles[0] = new ModifyTile(world.SystemManager);
			changeSingles[0].col = p_mapPos[0] - 1;
			changeSingles[0].row = p_mapPos[1] - 1;
			changeSingles[0].state = -1;
			changeSingles[0].affectedTilemap = singlesTilemap.getType();

			changeSingles[1] = new ModifyTile(world.SystemManager);
			changeSingles[1].col = p_mapPos[0];
			changeSingles[1].row = p_mapPos[1] - 1;
			changeSingles[1].state = -1;
			changeSingles[1].affectedTilemap = singlesTilemap.getType();

			changeSingles[2] = new ModifyTile(world.SystemManager);
			changeSingles[2].col = p_mapPos[0] + 1;
			changeSingles[2].row = p_mapPos[1] - 1;
			changeSingles[2].state = -1;
			changeSingles[2].affectedTilemap = singlesTilemap.getType();

			changeSingles[3] = new ModifyTile(world.SystemManager);
			changeSingles[3].col = p_mapPos[0] + 1;
			changeSingles[3].row = p_mapPos[1];
			changeSingles[3].state = -1;
			changeSingles[3].affectedTilemap = singlesTilemap.getType();
			
			changeSingles[4] = new ModifyTile(world.SystemManager);
			changeSingles[4].col = p_mapPos[0] + 1;
			changeSingles[4].row = p_mapPos[1] + 1;
			changeSingles[4].state = -1;
			changeSingles[4].affectedTilemap = singlesTilemap.getType();
			
			changeSingles[5] = new ModifyTile(world.SystemManager);
			changeSingles[5].col = p_mapPos[0];
			changeSingles[5].row = p_mapPos[1] + 1;
			changeSingles[5].state = -1;
			changeSingles[5].affectedTilemap = singlesTilemap.getType();
			
			changeSingles[6] = new ModifyTile(world.SystemManager);
			changeSingles[6].col = p_mapPos[0] - 1;
			changeSingles[6].row = p_mapPos[1] + 1;
			changeSingles[6].state = -1;
			changeSingles[6].affectedTilemap = singlesTilemap.getType();
			
			changeSingles[7] = new ModifyTile(world.SystemManager);
			changeSingles[7].col = p_mapPos[0] - 1;
			changeSingles[7].row = p_mapPos[1];
			changeSingles[7].state = -1;
			changeSingles[7].affectedTilemap = singlesTilemap.getType();
			
			changeSingles[8] = new ModifyTile(world.SystemManager);
			changeSingles[8].col = p_mapPos[0];
			changeSingles[8].row = p_mapPos[1];
			changeSingles[8].state = -1;
			changeSingles[8].affectedTilemap = singlesTilemap.getType();

			ActionSystem actionSys = ((ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0]);
			for (int i = 0; i < 9; i++)
			{
				actionSys.EnqueueAction(changeSingles[i]);
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
						p_tilemap.setState(x, y, 31);
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

		public void ReceiveAction(ActionInterface p_action)
		{
			if (p_action.GetType() == typeof(ModifyTile))
			{
				ModifyTile action = (ModifyTile)p_action;

				int oldState = -1;

				switch (action.affectedTilemap)
				{
					case Tilemap.TilemapType.RoadTilemap:
						oldState = roadTilemap.getState(action.col, action.row);
						roadTilemap.setState(action.col, action.row, action.state);
						break;

					case Tilemap.TilemapType.SingleTilemap:
						oldState = singlesTilemap.getState(action.col, action.row);
						singlesTilemap.setState(action.col, action.row, action.state);
						break;

					default:
						break;
				}

				action.state = oldState;
			}
		}
		
		Tilemap mainTilemap;
		Transform canvasTransform;
		Tilemap singlesTilemap;
		Tilemap roadTilemap;
		Tilemap wallTilemap;
		ComponentMapper<Tilemap> m_tilemapMapper;
		CurrentToolSystem m_toolSys;
		DrawCanvasSystem m_drawCanvasSys;
		bool m_lmbPressed;
	}
}
