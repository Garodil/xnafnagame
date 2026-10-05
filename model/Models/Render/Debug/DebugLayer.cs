using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Game;

namespace model.Models.Render.Debug
{
    public abstract class DebugLayer
    {
        protected readonly string _version;
        protected readonly SpriteBatch _worldBatch, _overlayBatch;
        protected readonly SpriteFont _debugFont;
        protected readonly Texture2D _debugTex;

        protected DebugLayer(string version, SpriteBatch worldBatch, SpriteBatch overlayBatch, SpriteFont debugFont, Texture2D debugTex)
        {
            _worldBatch = worldBatch;
            _overlayBatch = overlayBatch;
            _debugFont = debugFont;
            _debugTex = debugTex;
            _version = version;
        }

        public abstract void DrawOverlayLayer();
        public abstract void DrawInWorldLayer();
        public abstract void CalculateDataLayer(GameTime gameTime, State currentState);
        public abstract void CalculateRenderDataLayer(GameTime gameTime, State currentState);
    }
}