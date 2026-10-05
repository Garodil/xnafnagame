using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Interfaces.Service;
using model.Models.Render;

namespace service
{
    public class RenderTargetService : IRenderTargetService, IDisposable
    {
        private readonly Dictionary<RenderTargetEnum, RenderTarget2D> _renderTargets = new();

        private bool disposedValue = false;

        public RenderTargetService()
        {
        }

        public void DisposeAllRenderTargets()
        {
            foreach(var key in _renderTargets.Keys)
            {
                DisposeRenderTarget(key);
            }
        }

        public void DisposeRenderTarget(RenderTargetEnum targetTag)
        {
            if(_renderTargets.TryGetValue(targetTag, out var renderTarget))
            {
                renderTarget.Dispose();
                _renderTargets.Remove(targetTag);
            }
        }

        public RenderTarget2D? GetRenderTarget(RenderTargetEnum targetTag)
        {
            
            RenderTarget2D? renderTarget;
            _renderTargets.TryGetValue(targetTag, out renderTarget);
            return renderTarget;
        }

        public RenderTarget2D InitRenderTarget(RenderTargetEnum targetTag, int pixelizationScale, GraphicsDevice gd)
        {
            var renderTarget = new RenderTarget2D(
                gd, 
                gd.Viewport.Width / pixelizationScale, 
                gd.Viewport.Height / pixelizationScale,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents
            );

            _renderTargets.Add(targetTag, renderTarget);

            return renderTarget;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                DisposeAllRenderTargets();
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        ~RenderTargetService()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}