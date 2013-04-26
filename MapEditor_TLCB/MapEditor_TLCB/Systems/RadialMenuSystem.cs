using System;
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

            //COLLECTION MENU
			List<RadialMenuItem> collectionList = new List<RadialMenuItem>();
            collectionList.Add(new RadialMenuItem("Characters", characters, charactersMenu));
            collectionList.Add(new RadialMenuItem("Power-Ups", powerups, powerupsMenu));
            collectionList.Add(new RadialMenuItem("Dummy", dummy, tempEvent));
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


            //ADD ALL MENUS
			m_context.addRadialMenu(menu);
            m_context.addRadialMenu(collectionMenu);
            m_context.addRadialMenu(charactersMenu);
            m_context.addRadialMenu(powerupsMenu);
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
	}
}
