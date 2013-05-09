using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
	class MapValidationSystem: EntitySystem
	{
		public MapValidationSystem(Manager p_manager)
			: base(typeof(Tilemap), typeof(TilemapValidate))
		{
			m_manager = p_manager;
		}

		protected override void ProcessEntities(Dictionary<int, Entity> entities)
		{
			foreach (Entity e in entities.Values)
			{
				Tilemap tilemap = m_tilemapMapper.Get(e);
				TilemapValidate valid = m_validateMapper.Get(e);

				valid.pathsValid = validatePaths(tilemap);
				valid.playerValid = validatePlayer(tilemap);
				valid.switchesValid = validateSwitches(tilemap);

				if (e.Tag == "mainTilemap")
				{
					if(valid.pathsValid) {
						m_pathsValidLabel.TextColor = Color.DarkGreen;
					}
					else {
						m_pathsValidLabel.TextColor = Color.DarkRed;
					}
					
					if(valid.playerValid) {
						m_playerValidLabel.TextColor = Color.DarkGreen;
					}
					else {
						m_playerValidLabel.TextColor = Color.DarkRed;
					}
					
					if(valid.switchesValid) {
						m_switchesValidLabel.TextColor = Color.DarkGreen;
					}
					else {
						m_switchesValidLabel.TextColor = Color.DarkRed;
					}
				}
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
			else {
				return false;
			}

			int walkableWithoutPills = 0;
			foreach (int[] p in walkables)
			{
				int state = p_tilemap.getState(p[0], p[1]);
				if(state >= 6 * 30) {
					walkableWithoutPills++;
				}
			}

			if(walkables.Count - walkableWithoutPills == 0)
				return true;
			return false;
		}

		private bool validatePlayer(Tilemap p_tilemap)
		{
			int numPlayers = 0;
			bool connectedToRoad = false;
			for (int y = 0; y < p_tilemap.getRows(); y++)
			{
				for (int x = 0; x < p_tilemap.getColumns(); x++)
				{
					if (p_tilemap.getState(x, y) == 270)
					{
						numPlayers ++;
						if(p_tilemap.connectedTo(x, y, 3 * 30, 5 * 30))
							connectedToRoad = true;
					}
				}
			}

			if(numPlayers == 1 && connectedToRoad)
				return true;
			return false;
		}

		private bool validateSwitches(Tilemap p_tilemap)
		{
			int[] numSwitches = new int[8]{0,0,0,0,0,0,0,0};
			int[] numBlockades = new int[8]{0,0,0,0,0,0,0,0};

			for (int y = 0; y < p_tilemap.getRows(); y++)
			{
				for (int x = 0; x < p_tilemap.getColumns(); x++)
				{
					int state = p_tilemap.getState(x, y);
					if (state >= 6 * 30 && state < 6 * 30 + 8)
					{
						numSwitches[state - 6 * 30] += 1;
					}
					else if (state >= 7 * 30 && state < 7 * 30 + 8)
					{
						numBlockades[state - 7 * 30] += 1;
					}
				}
			}

			for (int i = 0; i < 8; i++)
			{
				if (numSwitches[i] > 0 || numBlockades[i] > 0)
				{
					if(numSwitches[i] == 0 || numBlockades[i] == 0)
						return false;
				}
			}

			return true;
		}

		public override void Initialize()
		{
			m_tilemapMapper = new ComponentMapper<Tilemap>(world);
			m_validateMapper = new ComponentMapper<TilemapValidate>(world);

			m_validateControl = new Window(m_manager);
			m_validateControl.Init();
			m_validateControl.Width = 150;
			m_validateControl.Height = 50;
			m_validateControl.Text = "Validation";
			m_validateControl.Center();
			m_validateControl.Top = 0;
			m_validateControl.BorderVisible = false;
			m_validateControl.Resizable = false;
			m_manager.Add(m_validateControl);

			m_pathsValidLabel = new Label(m_manager);
			m_pathsValidLabel.Init();
			m_pathsValidLabel.Parent = m_validateControl;
			m_pathsValidLabel.Text = "Paths";
			m_pathsValidLabel.Left = 10;
			m_pathsValidLabel.Top = 5;
			m_pathsValidLabel.TextColor = Color.DarkRed;

			m_playerValidLabel = new Label(m_manager);
			m_playerValidLabel.Init();
			m_playerValidLabel.Parent = m_validateControl;
			m_playerValidLabel.Text = "Player";
			m_playerValidLabel.Left = 10;
			m_playerValidLabel.Top = 20;
			m_playerValidLabel.TextColor = Color.DarkRed;

			m_switchesValidLabel = new Label(m_manager);
			m_switchesValidLabel.Init();
			m_switchesValidLabel.Parent = m_validateControl;
			m_switchesValidLabel.Text = "Switches";
			m_switchesValidLabel.Left = 10;
			m_switchesValidLabel.Top = 35;
			m_switchesValidLabel.TextColor = Color.DarkRed;
		}

		ComponentMapper<Tilemap> m_tilemapMapper;
		ComponentMapper<TilemapValidate> m_validateMapper;
		Manager m_manager;
		Window m_validateControl;
		Label m_pathsValidLabel;
		Label m_playerValidLabel;
		Label m_switchesValidLabel;
	}
}