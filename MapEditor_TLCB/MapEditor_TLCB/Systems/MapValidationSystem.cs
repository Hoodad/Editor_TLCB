﻿using System;
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
			else
				return false;

			if(walkables.Count == 0)
				return true;
			return false;
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
		}

		ComponentMapper<Tilemap> m_tilemapMapper;
		ComponentMapper<TilemapValidate> m_validateMapper;
		Manager m_manager;
		Window m_validateControl;
		Label m_pathsValidLabel;
		Label m_playerValidLabel;
	}
}