using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using MapEditor_TLCB.CustomControls;
using Microsoft.Xna.Framework.Input;

namespace MapEditor_TLCB.Systems
{
    struct QueuedForRender
    {
        public Texture2D tex;
        public Vector2 pos;
        public Rectangle sourceRect;
        public Color color;

    }

	class DrawCanvasSystem: EntitySystem
	{
		public DrawCanvasSystem(Dictionary<string, Texture2D> p_textures,
			GraphicsDevice p_graphicsDevice, RenderTarget2D p_canvasRender,
			Manager p_manager)
			: base(typeof(Tilemap), typeof(Transform), typeof(TilemapRender))
		{
			m_textures = p_textures;
			m_spriteBatch = new SpriteBatch(p_graphicsDevice);
			m_graphicsDevice = p_graphicsDevice;
			m_canvasRender = p_canvasRender;
			m_lastMovedMousePos = Vector2.Zero;
		}

		protected override void ProcessEntities(Dictionary<int, Entity> entities)
		{
			StartupDialogSystem dialogSystem = (StartupDialogSystem)(world.SystemManager.GetSystem<StartupDialogSystem>()[0]);

			m_spriteBatch.Draw(m_textures["canvas_shadow_30px"], new Vector2(-30.0f, -30.0f), new Color(0, 0, 0, 0.5f));

			Color transparent = new Color(0.5f, 0.5f, 0.8f, 0.5f);
			Color gridColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
			foreach (Entity e in entities.Values)
			{
				Transform transform = m_transformMapper.Get(e);
				Tilemap tilemap = m_tilemapMapper.Get(e);
				TilemapRender render = m_tilemapRenderMapper.Get(e);

				Texture2D texture = dialogSystem.tilemap;
                List<QueuedForRender> queued = new List<QueuedForRender>();
				for (int y = 0; y < tilemap.getRows(); y++)
				{
					for (int x = 0; x < tilemap.getColumns(); x++)
					{
                        int state = tilemap.getState(x, y);
                        Vector2 mapPosition = tilemap.getPosition(x, y);

                        //Ugly as hell but works
                        if (checkAddQueueItem(state, transform.position + mapPosition, queued, Color.White))
                            state = 8;


						if (state >= 0 && !render.overlay)
						{
							int sourceX = state % 30;
							int sourceY = state / 30;
							m_spriteBatch.Draw(texture, transform.position + mapPosition,
								new Rectangle(sourceX * 32, sourceY * 32, 32, 32), Color.White);
						}
						else if(state >= 0 && render.overlay)
						{
							int sourceX = state % 30;
							int sourceY = state / 30;
							m_spriteBatch.Draw(texture, transform.position + mapPosition,
								new Rectangle(sourceX * 32, sourceY * 32, 32, 32), transparent);
						}
						else if(!render.overlay)
						{
							//m_spriteBatch.Draw(texture, transform.position + mapPosition,
							//	new Rectangle(0 * 32, 0 * 32, 32, 32), Color.White);
						}
					}
				}

				if (m_lastMovedMousePos.X >= 0.0f &&
					m_lastMovedMousePos.X < tilemap.getColumns() * 32.0f &&
					m_lastMovedMousePos.Y >= 0.0f &&
					m_lastMovedMousePos.Y < tilemap.getRows() * 32.0f)
				{
					int[] mouseTilePos = tilemap.getTilePosition(m_lastMovedMousePos);
					Vector2 mouseGridPosition = tilemap.getPosition(mouseTilePos[0], mouseTilePos[1]);

					if (m_toolSys.GetCurrentTool() == Tool.ROAD_TOOL ||
						m_toolSys.GetCurrentTool() == Tool.ERASE_TOOL)
					{
						m_spriteBatch.Draw(m_textures["TileSelector"], mouseGridPosition, transparent);
					}
					else if (m_toolSys.GetCurrentTool() == Tool.PAINT_TOOL)
					{
                        IntPair toDraw = m_toolSys.GetCurrentDrawTileIndex();
                        Vector2 min = new Vector2(toDraw.i1 - 30 * (toDraw.i1 / 30), toDraw.i1 / 30);
                        Vector2 max = new Vector2(toDraw.i2 - 30 * (toDraw.i2 / 30), toDraw.i2 / 30);
                        for (int i = (int)min.X; i <= max.X; i++)
                        {
                            for (int j = (int)min.Y; j <= max.Y; j++)
                            {
                                Vector2 mapPosition = tilemap.getPosition(mouseTilePos[0]+(int)(i-min.X), mouseTilePos[1]+(int)(j-min.Y));
                                int state = j * 30 + i;
                                if (checkAddQueueItem(state, mapPosition, queued, transparent))
                                {
                                    state = 8;
                                }
                                int sourceX = state % 30;
                                int sourceY = state / 30;
                                m_spriteBatch.Draw(texture, transform.position + mapPosition,
                                    new Rectangle(sourceX * 32, sourceY * 32, 32, 32), transparent);
                            }
                        }


						//m_spriteBatch.Draw(texture, mouseGridPosition, m_toolSys.getTilemapIconRectangle(), transparent);
					}
				}
				if (m_camTransform.scale >= 1.0f) {
					m_spriteBatch.Draw(m_textures["canvas_grid"], Vector2.Zero, gridColor);
				}
                for (int i = 0; i < queued.Count; i++)
                {
                    m_spriteBatch.Draw(queued[i].tex, queued[i].pos,
                         queued[i].sourceRect, queued[i].color);
                }
			}

		}

        private bool checkAddQueueItem(int state, Vector2 position, List<QueuedForRender> queued, Color color)
        {
            if (state >= 270 && state < 302)
            {
                QueuedForRender toQueue = new QueuedForRender();

                toQueue.pos = position - new Vector2(16, 32);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 128;
                toQueue.sourceRect.Width = 64;
                toQueue.sourceRect.Height = 64;
                toQueue.color = color;

                if (state == 270)
                    toQueue.tex = m_textures["player"];
                else if (state == 300)
                    toQueue.tex = m_textures["rat"];
                else if (state == 301)
                    toQueue.tex = m_textures["rat2"];
                else
                    return false;
                queued.Add(toQueue);           

                return true;
            }
            else if (state == 302)
            {
                QueuedForRender toQueue = new QueuedForRender();
                toQueue.pos = position - new Vector2(37.5f, 32);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 200;
                toQueue.sourceRect.Width = 100;
                toQueue.sourceRect.Height = 100;
                toQueue.tex = m_textures["robot"];
                toQueue.color = color;
                queued.Add(toQueue);

                return true;
            }
            else if (state == 450)
            {
                QueuedForRender toQueue = new QueuedForRender();
                toQueue.pos = position - new Vector2(16, 16);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 0;
                toQueue.sourceRect.Width = 64;
                toQueue.sourceRect.Height = 64;
                toQueue.tex = m_textures["trap"];
                toQueue.color = color;
                queued.Add(toQueue);

                return true;
            }
            else if (state >= 180 && state <= 185)
            {
                QueuedForRender toQueue = new QueuedForRender();

                toQueue.pos = position - new Vector2(16, 16);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 64 * (state - 180);
                toQueue.sourceRect.Width = 64;
                toQueue.sourceRect.Height = 64;
                toQueue.tex = m_textures["switch"];
                toQueue.color = color;
                queued.Add(toQueue);

                return true;
            }
            else if (state >= 210 && state <= 215)
            {
                QueuedForRender toQueue = new QueuedForRender();

                toQueue.pos = position - new Vector2(16, 16);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 64 * (state - 210);
                toQueue.sourceRect.Width = 64;
                toQueue.sourceRect.Height = 64;
                toQueue.tex = m_textures["block"];
                toQueue.color = color;
                queued.Add(toQueue);

                return true;
            }
            else if (state == 360)
            {
                QueuedForRender toQueue = new QueuedForRender();

                toQueue.pos = position - new Vector2(16, 16);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 0;
                toQueue.sourceRect.Width = 64;
                toQueue.sourceRect.Height = 64;
                toQueue.tex = m_textures["speed"];
                toQueue.color = color;
                queued.Add(toQueue);

                state = 8;
            }
            else if (state == 390)
            {
                QueuedForRender toQueue = new QueuedForRender();

                toQueue.pos = position - new Vector2(16, 16);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 0;
                toQueue.sourceRect.Width = 64;
                toQueue.sourceRect.Height = 64;
                toQueue.tex = m_textures["bomb"];
                toQueue.color = color;
                queued.Add(toQueue);

                return true;
            }
            else if (state == 420)
            {
                QueuedForRender toQueue = new QueuedForRender();

                toQueue.pos = position - new Vector2(16, 16);
                toQueue.sourceRect.X = 0;
                toQueue.sourceRect.Y = 0;
                toQueue.sourceRect.Width = 64;
                toQueue.sourceRect.Height = 64;
                toQueue.tex = m_textures["super"];
                toQueue.color = color;
                queued.Add(toQueue);

                return true;
            }
            return false;
        }

        public override void Initialize()
		{
			m_transformMapper = new ComponentMapper<Transform>(world);
			m_tilemapMapper = new ComponentMapper<Tilemap>(world);
			m_tilemapRenderMapper = new ComponentMapper<TilemapRender>(world);
			m_toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
		}

		protected override void Begin()
		{
			m_graphicsDevice.SetRenderTarget(m_canvasRender);
			m_graphicsDevice.Clear(Color.LightGray);

			Matrix cameraMatrix = Matrix.Identity;
			Entity camera = world.TagManager.GetEntity("mainCamera");
			if (camera != null)
			{
				m_camTransform = camera.GetComponent<Transform>();
				if (m_camTransform != null)
				{
					cameraMatrix = m_camTransform.getMatrix();
				}
			}
			
			m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
				SamplerState.PointClamp, null, null, null, cameraMatrix);
		}

		protected override void End()
		{
			m_spriteBatch.End();
			m_graphicsDevice.SetRenderTarget(null);
			if (Keyboard.GetState().IsKeyDown(Keys.F5))
			{
				System.IO.FileStream file = new System.IO.FileStream("asd.png", System.IO.FileMode.OpenOrCreate);
				m_canvasRender.SaveAsPng(file, m_canvasRender.Width, m_canvasRender.Height);
				file.Close();
			}
		}

		public void setLastMousePos(Vector2 p_mousePos)
		{
			m_lastMovedMousePos = p_mousePos;
		}

        public void updateWindowSize(RenderTarget2D p_newCanvas)
        {
            m_canvasRender = p_newCanvas;
        }

		Dictionary<string, Texture2D> m_textures;
		SpriteBatch m_spriteBatch;
		GraphicsDevice m_graphicsDevice;
		RenderTarget2D m_canvasRender;
		ComponentMapper<Transform> m_transformMapper;
		ComponentMapper<Tilemap> m_tilemapMapper;
		ComponentMapper<TilemapRender> m_tilemapRenderMapper;
		Vector2 m_lastMovedMousePos;
		CurrentToolSystem m_toolSys;
		Transform m_camTransform;
	}
}
