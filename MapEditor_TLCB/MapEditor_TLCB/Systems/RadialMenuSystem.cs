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
using MapEditor_TLCB.CustomControls;



namespace MapEditor_TLCB.Systems
{
	class RadialMenuSystem : EntitySystem
	{
		RadialMenuContext m_context;

		GraphicsDevice m_device;
		ContentManager m_content;

        List<EventData> m_events;

        Texture2D m_tilemap;

        int customID;


        RadialWindow m_radialWindow;
        Manager m_manager;

		public RadialMenuSystem(GraphicsDevice p_device, ContentManager p_content, Manager p_manager)
		{
			m_device = p_device;
			m_content = p_content;
            m_manager = p_manager;
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

            m_tilemap = m_content.Load<Texture2D>("TileSheets/tilemap_garden");

            Texture2D question = m_content.Load<Texture2D>("Question");

            m_events = new List<EventData>();
            int start = 3 * 30;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    IntPair pair;
                    pair.i1 = start + i + j * 30;
                    pair.i2 = start + i + j * 30;
                    m_events.Add(new EventData(setToolCallback, pair));
                }
            }
            IntPair pair2;
            pair2.i1 = start + 7;
            pair2.i2 = start + 7;
            m_events.Add(new EventData(setToolCallback, pair2));

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
            roadTileList.Add(new RadialMenuItem("Upper Left", m_tilemap, m_events[0], new Rectangle(0, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Vertical", m_tilemap, m_events[1], new Rectangle(0, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Lower Left", m_tilemap, m_events[2], new Rectangle(0, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Horizontal", m_tilemap, m_events[3], new Rectangle(32, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Single Dot", m_tilemap, m_events[4], new Rectangle(32, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Horizontal", m_tilemap, m_events[5], new Rectangle(32, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Upper Right", m_tilemap, m_events[6], new Rectangle(64, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Vertical", m_tilemap, m_events[7], new Rectangle(64, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Lower Right", m_tilemap, m_events[8], new Rectangle(64, 160, 32, 32), 0.4f));

            roadTileList.Add(new RadialMenuItem("Dead End Left", m_tilemap, m_events[9], new Rectangle(96, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Dead End Up", m_tilemap, m_events[10], new Rectangle(96, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Dead End Down", m_tilemap, m_events[11], new Rectangle(96, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Dead End Right", m_tilemap, m_events[12], new Rectangle(128, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("T Up Right Down", m_tilemap, m_events[13], new Rectangle(128, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Crossing", m_tilemap, m_events[14], new Rectangle(128, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("T Left Right Down", m_tilemap, m_events[15], new Rectangle(160, 96, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("T Left Up Down", m_tilemap, m_events[16], new Rectangle(160, 128, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("T Left Up Right", m_tilemap, m_events[17], new Rectangle(160, 160, 32, 32), 0.4f));
            roadTileList.Add(new RadialMenuItem("Horizontal", m_tilemap, m_events[18], new Rectangle(224, 96, 32, 32), 0.4f));
            RadialMenu RoadTileMenu = new RadialMenu(m_device, m_content, roadTileList, road, null);

            //CUSTOM MENU
            List<RadialMenuItem> customList = new List<RadialMenuItem>();
            RadialMenu customMenu = new RadialMenu(m_device, m_content, customList, question, null);

            //COLLECTION MENU
			List<RadialMenuItem> collectionList = new List<RadialMenuItem>();
            collectionList.Add(new RadialMenuItem("Characters", characters, charactersMenu));
            collectionList.Add(new RadialMenuItem("Power-Ups", powerups, powerupsMenu));
            collectionList.Add(new RadialMenuItem("Road Tiles", m_tilemap, RoadTileMenu, new Rectangle(0, 96, 96, 96)));
            collectionList.Add(new RadialMenuItem("Custom Selections", question, customMenu));
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
            customMenu.setParent(collectionMenu);


            //ADD ALL MENUS
			m_context.addRadialMenu(menu);
            m_context.addRadialMenu(collectionMenu);
            m_context.addRadialMenu(charactersMenu);
            m_context.addRadialMenu(powerupsMenu);
            m_context.addRadialMenu(RoadTileMenu);
            m_context.addRadialMenu(customMenu);
            customID = 5;



            m_radialWindow = new RadialWindow(m_manager);
            m_radialWindow.Init();
            m_radialWindow.Left = 0;
            m_radialWindow.Top = 0;
            m_radialWindow.Width = m_manager.Window.Width;
            m_radialWindow.Height = m_manager.Window.Height;
            m_radialWindow.Parent = null;
            //m_canvasWindow.BorderVisible = false;
            m_radialWindow.Resizable = false;
            m_radialWindow.StayOnTop = true;
            m_radialWindow.RadialContext = m_context;
            m_radialWindow.DoubleClicks = false;
            m_radialWindow.Click += new TomShane.Neoforce.Controls.EventHandler(radialWindow_MouseClick);
            m_manager.Add(m_radialWindow);
            m_radialWindow.Hide();
		}

		public override void Process()
		{
            for (int i = 0; i < m_events.Count; i++)
            {
                if (KeyDelta.getDelta(m_events[i].hotkey) > 0.0f)
                {
                    m_events[i].callback(m_events[i].data);
                }
            }

			float dt = World.Delta / 1000.0f;
			m_context.update(dt);

            if (m_context.isActive())
                m_radialWindow.Show();
            else
                m_radialWindow.Hide();
		}
		public void Render(SpriteBatch p_spriteBatch)
		{
			m_context.draw5(p_spriteBatch);
		}
        public bool isRadialActive()
        {
            return m_context.isActive();
        }
        public void setToolCallback(Object p_toolIndex)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)world.SystemManager.GetSystem<CurrentToolSystem>()[0];
            toolSys.SetCurrentTool(CustomControls.Tool.PAINT_TOOL);

            MapEditor_TLCB.CustomControls.IntPair pair;
            pair = (IntPair)p_toolIndex;
            toolSys.SetCurrentDrawToolIndex(pair);
        }
        public void addCustomSelection(IntPair p_selection)
        {
            EventData ev = new EventData(setToolCallback, p_selection);
            Vector2 min = new Vector2(p_selection.i1 - 30 * (p_selection.i1 / 30), p_selection.i1 / 30);
            Vector2 max = new Vector2(p_selection.i2 - 30 * (p_selection.i2 / 30), p_selection.i2 / 30);

            Rectangle rect;
            rect.X = (int)(32 * min.X);
            rect.Width = 32 * (int)(max.X - min.X + 1);
            rect.Y = (int)(32 * min.Y);
            rect.Height = 32 * (int)(max.Y - min.Y + 1);

            RadialMenu customMenu = m_context.getRadialMenu(customID);
            RadialMenuItem newItem = new RadialMenuItem("Custom Selection", m_tilemap, ev, rect);
            customMenu.addItem(newItem);
            m_events.Add(ev);

            NotificationBarSystem note = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
            note.AddNotification(new Notification("Added custom selection to radial menu", NotificationType.INFO));
        }
        public void toggleRadialMenu()
        {
            m_context.toggleRadialMenu();
        }
        private void radialWindow_MouseClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TomShane.Neoforce.Controls.MouseEventArgs ev = (TomShane.Neoforce.Controls.MouseEventArgs)(e);
            if (ev.Button == MouseButton.Right)
            {
                toggleRadialMenu();
            }
        }
	}
}
