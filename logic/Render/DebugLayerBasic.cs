using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Game;
using model.Interfaces.Service;
using scenes.Manager;

namespace model.Models.Render.Debug
{
    public class DebugLayerBasic : DebugLayer
    {
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _fpsCounter = 0;
        private int _displayedFpsCounter = 0;
        private Vector2 _playerCoordinates = Vector2.Zero;
        private string _currentSceneName = string.Empty;
        private readonly ISceneManager _sceneManager;

        public DebugLayerBasic(
            string version,
            ISceneManager sceneManager, 
            SpriteBatch worldBatch, 
            SpriteBatch overlayBatch, 
            SpriteFont debugFont, 
            Texture2D debugTex
            ) 
            : base(version, worldBatch, overlayBatch, debugFont, debugTex)
        {
            _sceneManager = sceneManager;
        }

        public override void CalculateDataLayer(GameTime gameTime, State currentState)
        {
            var scene = _sceneManager.GetScene();
            var playerPos = scene?.GetState()?.Player?.Position;
            _playerCoordinates = playerPos == null ? Vector2.Zero : new Vector2(playerPos.Value.X, playerPos.Value.Y);
            var sceneType = scene?.GetType();
            _currentSceneName = sceneType?.FullName ?? string.Empty;
        }

        public override void DrawInWorldLayer()
        {
        }

        public override void DrawOverlayLayer()
        {
            string[] lines =
            [
                $"Build: {_version}",
                $"FPS: {_displayedFpsCounter}",
                $"Scene: {_currentSceneName}",
                $"POS: ({_playerCoordinates.X}, {_playerCoordinates.Y})"
            ];
            var displayCoordinates = Vector2.Zero;
            foreach(var line in lines)
            {
                DrawOverlayText(displayCoordinates, line);
                displayCoordinates = new Vector2(displayCoordinates.X, displayCoordinates.Y + _debugFont.LineSpacing);
            }
        }

        private void DrawOverlayText(Vector2 place, string text)
        {
            _overlayBatch.DrawString(_debugFont, text, place, Color.White);
        }

        public override void CalculateRenderDataLayer(GameTime gameTime, State currentState)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            if(_elapsedTime.TotalSeconds >= 1.0)
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _displayedFpsCounter = _fpsCounter;
                _fpsCounter = 0;
            } 
            else 
            {
                _fpsCounter += 1;
            }
        }
    }
}