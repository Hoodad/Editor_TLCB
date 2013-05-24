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
	public class RadialMenuSystem : EntitySystem
	{
		RadialMenuContext m_context;

		GraphicsDevice m_device;
		ContentManager m_content;

        Texture2D m_tilemap;

        int customID;
        int toReplace = 0;

        List<RadialMenuItem> m_usesTilemap;


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
            Texture2D speed = m_content.Load<Texture2D>("OtherSheets/speedpowerup");
            Texture2D powerful = m_content.Load<Texture2D>("OtherSheets/Item_SuperCheesy");
            Texture2D bomb = m_content.Load<Texture2D>("OtherSheets/bombitem");

            Texture2D road = m_content.Load<Texture2D>("RoadToolIcon");

            Texture2D switches = m_content.Load<Texture2D>("OtherSheets/Switch_Tileset");
            Texture2D blocks = m_content.Load<Texture2D>("OtherSheets/Blockade_Tileset");

            m_tilemap = m_content.Load<Texture2D>("TileSheets/tilemap_garden");

            Texture2D question = m_content.Load<Texture2D>("Question");

            Texture2D switchPreview = m_content.Load<Texture2D>("OtherSheets/swiitchesPreview");
            Texture2D blockPreview = m_content.Load<Texture2D>("OtherSheets/Blockade_Preview");

            Texture2D trap = m_content.Load<Texture2D>("OtherSheets/Trap_Spikes");

            EventSystem ev = (EventSystem)world.SystemManager.GetSystem<EventSystem>()[0];
            int start = 3 * 30;

            List<EventData> events = new List<EventData>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    IntPair pair;
                    pair.i1 = start + i + j * 30;
                    pair.i2 = start + i + j * 30;
                    EventData eventData = new EventData(setToolCallback, pair);
                    ev.addEvent(eventData);
                    events.Add(eventData);
                }
            }

			EventData tempEvent = new EventData(null, null);

            //Character events
            EventData avatarEvent = new EventData(setToolCallback, new IntPair(270, 270));
            EventData robot = new EventData(setToolCallback, new IntPair(302, 302));
            EventData rat1E = new EventData(setToolCallback, new IntPair(300, 300));
            EventData rat2E = new EventData(setToolCallback, new IntPair(301, 301));
            events.Add(avatarEvent);
            events.Add(robot);
            events.Add(rat1E);
            events.Add(rat2E);

            //Power-Ups Events
            EventData speedE = new EventData(setToolCallback, new IntPair(360, 360));
            EventData bombE = new EventData(setToolCallback, new IntPair(390, 390));
            EventData superE = new EventData(setToolCallback, new IntPair(420, 420));
            events.Add(speedE);
            events.Add(bombE);
            events.Add(superE);

            //Switches Events
            EventData s1E = new EventData(setToolCallback, new IntPair(180, 180));
            EventData s2E = new EventData(setToolCallback, new IntPair(181, 181));
            EventData s3E = new EventData(setToolCallback, new IntPair(182, 182));
            EventData s4E = new EventData(setToolCallback, new IntPair(183, 183));
            EventData s5E = new EventData(setToolCallback, new IntPair(184, 184));
            EventData s6E = new EventData(setToolCallback, new IntPair(185, 185));
            events.Add(s1E);
            events.Add(s2E);
            events.Add(s3E);
            events.Add(s4E);
            events.Add(s5E);
            events.Add(s6E);

            //Blocks Events
            EventData b1E = new EventData(setToolCallback, new IntPair(210, 210));
            EventData b2E = new EventData(setToolCallback, new IntPair(211, 211));
            EventData b3E = new EventData(setToolCallback, new IntPair(212, 212));
            EventData b4E = new EventData(setToolCallback, new IntPair(213, 213));
            EventData b5E = new EventData(setToolCallback, new IntPair(214, 214));
            EventData b6E = new EventData(setToolCallback, new IntPair(215, 215));
            events.Add(b1E);
            events.Add(b2E);
            events.Add(b3E);
            events.Add(b4E);
            events.Add(b5E);
            events.Add(b6E);

            //Trap Event
            EventData trapE = new EventData(setToolCallback, new IntPair(450, 450));
            events.Add(trapE);



            //CHARACTERS MENU
            List<RadialMenuItem> characterList = new List<RadialMenuItem>();
            characterList.Add(new RadialMenuItem("Avatar", cheeseboy, avatarEvent));
            characterList.Add(new RadialMenuItem("Robotaparte", napoleon, robot));
            characterList.Add(new RadialMenuItem("Rat", rat1, rat1E));
            characterList.Add(new RadialMenuItem("Infected Rat", rat2, rat2E));
            RadialMenu charactersMenu = new RadialMenu(m_device, m_content, characterList, characters, null, ev, this);

            //POWER-UPS MENU
            List<RadialMenuItem> powerupsList = new List<RadialMenuItem>();
            powerupsList.Add(new RadialMenuItem("Bomb", bomb, bombE));
            powerupsList.Add(new RadialMenuItem("Speed", speed, speedE));
            powerupsList.Add(new RadialMenuItem("Strength", powerful, superE));
            RadialMenu powerupsMenu = new RadialMenu(m_device, m_content, powerupsList, powerups, null, ev, this);

            //SWITCHES MENU
            List<RadialMenuItem> switchesList = new List<RadialMenuItem>();
            switchesList.Add(new RadialMenuItem("Switch 1", switches, s1E, new Rectangle(0, 0, 64, 64)));
            switchesList.Add(new RadialMenuItem("Switch 2", switches, s2E, new Rectangle(0, 64, 64, 64)));
            switchesList.Add(new RadialMenuItem("Switch 3", switches, s3E, new Rectangle(0, 128, 64, 64)));
            switchesList.Add(new RadialMenuItem("Switch 4", switches, s4E, new Rectangle(0, 192, 64, 64)));
            switchesList.Add(new RadialMenuItem("Switch 5", switches, s5E, new Rectangle(0, 256, 64, 64)));
            switchesList.Add(new RadialMenuItem("Switch 6", switches, s6E, new Rectangle(0, 320, 64, 64)));
            RadialMenu switchesMenu = new RadialMenu(m_device, m_content, switchesList, switchPreview, null, ev, this);

            //BLOCKS MENU
            List<RadialMenuItem> blockList = new List<RadialMenuItem>();
            blockList.Add(new RadialMenuItem("Block 1", blocks, b1E, new Rectangle(0, 0, 64, 64)));
            blockList.Add(new RadialMenuItem("Block 2", blocks, b2E, new Rectangle(0, 64, 64, 64)));
            blockList.Add(new RadialMenuItem("Block 3", blocks, b3E, new Rectangle(0, 128, 64, 64)));
            blockList.Add(new RadialMenuItem("Block 4", blocks, b4E, new Rectangle(0, 192, 64, 64)));
            blockList.Add(new RadialMenuItem("Block 5", blocks, b5E, new Rectangle(0, 256, 64, 64)));
            blockList.Add(new RadialMenuItem("Block 6", blocks, b6E, new Rectangle(0, 320, 64, 64)));
            RadialMenu blockMenu = new RadialMenu(m_device, m_content, blockList, blockPreview, null, ev, this);

            //CUSTOM MENU
            List<RadialMenuItem> customList = new List<RadialMenuItem>();
            RadialMenu customMenu = new RadialMenu(m_device, m_content, customList, question, null, ev, this);

            //COLLECTION MENU
			List<RadialMenuItem> collectionList = new List<RadialMenuItem>();
            collectionList.Add(new RadialMenuItem("Characters", characters, charactersMenu));
            collectionList.Add(new RadialMenuItem("Power-Ups", powerups, powerupsMenu));
            collectionList.Add(new RadialMenuItem("Switches", switchPreview, switchesMenu));
            collectionList.Add(new RadialMenuItem("Blocks", blockPreview, blockMenu));
            collectionList.Add(new RadialMenuItem("Trap", trap, trapE));
            collectionList.Add(new RadialMenuItem("Custom Selections", question, customMenu));
            RadialMenu collectionMenu = new RadialMenu(m_device, m_content, collectionList, tileCollection, null, ev, this);

            //MAIN MENU
			List<RadialMenuItem> items = new List<RadialMenuItem>();
            items.Add(new RadialMenuItem("Tile Collection", tileCollection, collectionMenu));
            items.Add(new RadialMenuItem("Recent 1", m_tilemap, events[0], new Rectangle(0, 96, 32, 32)));
            items.Add(new RadialMenuItem("Recent 2", m_tilemap, events[1], new Rectangle(0, 128, 32, 32)));
            items.Add(new RadialMenuItem("Recent 3", m_tilemap, events[2], new Rectangle(0, 160, 32, 32)));
            items.Add(new RadialMenuItem("Recent 4", m_tilemap, events[3], new Rectangle(32, 96, 32, 32)));
            items.Add(new RadialMenuItem("Recent 5", m_tilemap, events[4], new Rectangle(32, 128, 32, 32)));
            items.Add(new RadialMenuItem("Recent 6", m_tilemap, events[5], new Rectangle(32, 160, 32, 32)));
            items.Add(new RadialMenuItem("Recent 7", m_tilemap, events[6], new Rectangle(64, 96, 32, 32)));
            items.Add(new RadialMenuItem("Recent 8", m_tilemap, events[7], new Rectangle(64, 128, 32, 32)));
            items.Add(new RadialMenuItem("Recent 9", m_tilemap, events[8], new Rectangle(64, 160, 32, 32)));
            m_usesTilemap = new List<RadialMenuItem>();
            for (int i = 1; i < items.Count; i++)
            {
                m_usesTilemap.Add(items[i]);
            }

            RadialMenu menu = new RadialMenu(m_device, m_content, items, main, null, ev, this);

            //DETERMINE ALL PARENTING
            collectionMenu.setParent(menu);
            charactersMenu.setParent(collectionMenu);
            powerupsMenu.setParent(collectionMenu);
            switchesMenu.setParent(collectionMenu);
            customMenu.setParent(collectionMenu);
            blockMenu.setParent(collectionMenu);


            //ADD ALL MENUS
			m_context.addRadialMenu(menu);
            m_context.addRadialMenu(collectionMenu);
            m_context.addRadialMenu(charactersMenu);
            m_context.addRadialMenu(powerupsMenu);
            m_context.addRadialMenu(customMenu);
            m_context.addRadialMenu(switchesMenu);
            m_context.addRadialMenu(blockMenu);
            customID = 4;



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
        public void requestMenu(Object p_menu)
        {
            m_context.requestMenu((RadialMenu)p_menu);
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
            EventSystem evSys = (EventSystem)world.SystemManager.GetSystem<EventSystem>()[0];
            evSys.addEvent(ev);

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
        public void setTilemap(Texture2D p_tilemap)
        {
            m_tilemap = p_tilemap;
            for (int i = 0; i < m_usesTilemap.Count; i++)
            {
                m_usesTilemap[i].texture = m_tilemap;
            }
        }
        public void currentToolChanged(IntPair p_current)
        {
            if (p_current.i1 < 0 || p_current.i2 < 0)
                return;
            for (int i = 0; i < m_usesTilemap.Count; i++)
            {
                EventData ev = m_usesTilemap[i].activateEvent;
                IntPair pair = (IntPair)ev.data;
                if (pair.i1 == p_current.i1 && pair.i2 == p_current.i2)
                    return;
            }
            m_usesTilemap[toReplace].activateEvent.data = p_current;
            Vector2 min = new Vector2(p_current.i1 - 30 * (p_current.i1 / 30), p_current.i1 / 30);
            Vector2 max = new Vector2(p_current.i2 - 30 * (p_current.i2 / 30), p_current.i2 / 30);
            m_usesTilemap[toReplace].sourceRect.X = (int)(32 * min.X);
            m_usesTilemap[toReplace].sourceRect.Width = 32 * (int)(max.X - min.X + 1);
            m_usesTilemap[toReplace].sourceRect.Y = (int)(32 * min.Y);
            m_usesTilemap[toReplace].sourceRect.Height = 32 * (int)(max.Y - min.Y + 1);

            toReplace++;
            if (toReplace >= m_usesTilemap.Count)
                toReplace = 0;
        }
	}
}
