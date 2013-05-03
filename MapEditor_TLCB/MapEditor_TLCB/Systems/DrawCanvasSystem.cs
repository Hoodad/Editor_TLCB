using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
	class DrawCanvasSystem: EntitySystem
	{
		public DrawCanvasSystem(Dictionary<string, Texture2D> p_textures,
			SpriteBatch p_spriteBatch, GraphicsDevice p_graphicsDevice,
			RenderTarget2D p_canvasRender)
			: base(typeof(Tilemap), typeof(Transform), typeof(TilemapRender))
		{
			m_textures = p_textures;
			//m_spriteBatch = p_spriteBatch;
			m_spriteBatch = new SpriteBatch(p_graphicsDevice);
			m_graphicsDevice = p_graphicsDevice;
			m_canvasRender = p_canvasRender;
		}

		protected override void ProcessEntities(Dictionary<int, Entity> entities)
		{
			Color transparent = new Color(0.3f, 0.3f, 0.3f, 0.2f);
			foreach (Entity e in entities.Values)
			{
				Transform transform = m_transformMapper.Get(e);
				Tilemap tilemap = m_tilemapMapper.Get(e);
				TilemapRender render = m_tilemapRenderMapper.Get(e);
				
				Texture2D texture = m_textures[render.theme];
				for (int y = 0; y < tilemap.getRows(); y++)
				{
					for (int x = 0; x < tilemap.getColumns(); x++)
					{
						int state = tilemap.getState(x, y);
						Vector2 mapPosition = tilemap.getPosition(x, y);
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
							m_spriteBatch.Draw(texture, transform.position + mapPosition,
								new Rectangle(1 * 32, 1 * 32, 32, 32), Color.White);
						}
					}
				}
			}
		}

		public override void Initialize()
		{
			m_transformMapper = new ComponentMapper<Transform>(world);
			m_tilemapMapper = new ComponentMapper<Tilemap>(world);
			m_tilemapRenderMapper = new ComponentMapper<TilemapRender>(world);
		}

		protected override void Begin()
		{
			m_graphicsDevice.SetRenderTarget(m_canvasRender);
			m_graphicsDevice.Clear(Color.DarkGray);

			Matrix cameraMatrix = Matrix.Identity;
			Entity camera = world.TagManager.GetEntity("mainCamera");
			if (camera != null)
			{
				Transform camTransform = camera.GetComponent<Transform>();
				if (camTransform != null)
				{
					cameraMatrix = camTransform.getMatrix();
				}
			}
			
			m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
				null, null, null, null, cameraMatrix);
		}

		protected override void End()
		{
			m_spriteBatch.End();
			m_graphicsDevice.SetRenderTarget(null);
		}

		Dictionary<string, Texture2D> m_textures;
		SpriteBatch m_spriteBatch;
		GraphicsDevice m_graphicsDevice;
		RenderTarget2D m_canvasRender;
		ComponentMapper<Transform> m_transformMapper;
		ComponentMapper<Tilemap> m_tilemapMapper;
		ComponentMapper<TilemapRender> m_tilemapRenderMapper;
	}
}
