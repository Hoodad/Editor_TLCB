﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Artemis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MapEditor_TLCB.Systems
{
	class RadialMenuSystem : EntitySystem
	{
		RadialMenuContext m_context;

		GraphicsDevice m_device;
		ContentManager m_content;

		public RadialMenuSystem(GraphicsDevice p_device, ContentManager p_content)
		{
			m_device = p_device;
			m_content = p_content;
		}

		public override void Initialize()
		{
			m_context = new RadialMenuContext(m_device, m_content);

			//Textures
			Texture2D dummy = m_content.Load<Texture2D>("radialTemp");
            Texture2D tileCollection = m_content.Load<Texture2D>("chest");
			Texture2D main = m_content.Load<Texture2D>("cheese");
            Texture2D characters = m_content.Load<Texture2D>("Radial/Characters/characters");
            Texture2D cheeseboy = m_content.Load<Texture2D>("Radial/Characters/player");
            Texture2D napoleon = m_content.Load<Texture2D>("Radial/Characters/napoleon");
            Texture2D rat1 = m_content.Load<Texture2D>("Radial/Characters/rat");
            Texture2D rat2 = m_content.Load<Texture2D>("Radial/Characters/rat2");

            
            Texture2D powerups = m_content.Load<Texture2D>("Radial/Powerups/powerups");
            Texture2D speed = m_content.Load<Texture2D>("Radial/Powerups/speedup");
            Texture2D powerful = m_content.Load<Texture2D>("Radial/Powerups/super");
            Texture2D bomb = m_content.Load<Texture2D>("Radial/Powerups/bomb");

            Texture2D road = m_content.Load<Texture2D>("RoadToolIcon");

            Texture2D tilemap = m_content.Load<Texture2D>("TileSheets/tilemap_garden");


			EventData tempEvent = new EventData(null, null);


            //CHARACTERS MENU
            List<RadialMenuItem> characterList = new List<RadialMenuItem>();
            characterList.Add(new RadialMenuItem("Avatar", cheeseboy, tempEvent));
            characterList.Add(new RadialMenuItem("Napoleon", napoleon, tempEvent));
            characterList.Add(new RadialMenuItem("Rat", rat1, tempEvent));
            characterList.Add(new RadialMenuItem("Infected Rat", rat2, tempEvent));
            RadialMenu charactersMenu = new RadialMenu(m_device, m_content, characterList, characters, null);

            //POWER-UPS MENU
            List<RadialMenuItem> powerupsList = new List<RadialMenuItem>();
            powerupsList.Add(new RadialMenuItem("Bomb", bomb, tempEvent));
            powerupsList.Add(new RadialMenuItem("Speed", speed, tempEvent));
            powerupsList.Add(new RadialMenuItem("Strength", powerful, tempEvent));
            RadialMenu powerupsMenu = new RadialMenu(m_device, m_content, powerupsList, powerups, null);

            //ROAD TILES MENU
            List<RadialMenuItem> roadTileList = new List<RadialMenuItem>();
            roadTileList.Add(new RadialMenuItem("Upper Left", tilemap, tempEvent, new Rectangle(0, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Vertical", tilemap, tempEvent, new Rectangle(0, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Lower Left", tilemap, tempEvent, new Rectangle(0, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Horizontal", tilemap, tempEvent, new Rectangle(32, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Single Dot", tilemap, tempEvent, new Rectangle(32, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Horizontal", tilemap, tempEvent, new Rectangle(32, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Upper Right", tilemap, tempEvent, new Rectangle(64, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Vertical", tilemap, tempEvent, new Rectangle(64, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Lower Right", tilemap, tempEvent, new Rectangle(64, 160, 32, 32), 0.4f));

            roadTileList.Add(new RadialMenuItem("Road1", tilemap, tempEvent, new Rectangle(96, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road2", tilemap, tempEvent, new Rectangle(96, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road3", tilemap, tempEvent, new Rectangle(96, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road4", tilemap, tempEvent, new Rectangle(128, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road5", tilemap, tempEvent, new Rectangle(128, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road6", tilemap, tempEvent, new Rectangle(128, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road7", tilemap, tempEvent, new Rectangle(160, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road8", tilemap, tempEvent, new Rectangle(160, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road9", tilemap, tempEvent, new Rectangle(160, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Road9", tilemap, tempEvent, new Rectangle(224, 96, 32, 32), 0.4f));
            RadialMenu RoadTileMenu = new RadialMenu(m_device, m_content, roadTileList, road, null);

            //COLLECTION MENU
			List<RadialMenuItem> collectionList = new List<RadialMenuItem>();
            collectionList.Add(new RadialMenuItem("Characters", characters, charactersMenu));
            collectionList.Add(new RadialMenuItem("Power-Ups", powerups, powerupsMenu));
            collectionList.Add(new RadialMenuItem("Road Tiles", tilemap, RoadTileMenu, new Rectangle(0, 96, 96, 96)));
            RadialMenu collectionMenu = new RadialMenu(m_device, m_content, collectionList, tileCollection, null);

            //MAIN MENU
			List<RadialMenuItem> items = new List<RadialMenuItem>();
            items.Add(new RadialMenuItem("Tile Collection", tileCollection, collectionMenu));
            items.Add(new RadialMenuItem("Dummy", dummy, tempEvent));
            items.Add(new RadialMenuItem("Dummy", dummy, tempEvent));
            items.Add(new RadialMenuItem("Dummy", dummy, tempEvent));

			RadialMenu menu = new RadialMenu(m_device, m_content, items, main, null);

            //DETERMINE ALL PARENTING
            collectionMenu.setParent(menu);
            charactersMenu.setParent(collectionMenu);
            powerupsMenu.setParent(collectionMenu);
            RoadTileMenu.setParent(collectionMenu);


            //ADD ALL MENUS
			m_context.addRadialMenu(menu);
            m_context.addRadialMenu(collectionMenu);
            m_context.addRadialMenu(charactersMenu);
            m_context.addRadialMenu(powerupsMenu);
            m_context.addRadialMenu(RoadTileMenu);
		}

		public override void Process()
		{
			float dt = World.Delta / 1000.0f;
			m_context.update(dt);
		}
		public void Render(SpriteBatch p_spriteBatch)
		{
			m_context.draw5(p_spriteBatch);
		}
        public bool isRadialActive()
        {
            return m_context.isActive();
        }
	}
}
