using System.Collections.Generic;
using Microsoft.Xna.Framework;
using model.Game;
using model.Models.Game.View;

namespace model.Interfaces.Manager
{
    public interface ICameraManager
    {
        public Camera? ActiveCamera { get; }
        public void AddCamera(Camera camera);
        public void AddCameras(IEnumerable<Camera> cameras);
        public void SetViewportSize(int width, int height);
        public Matrix GetActiveCameraMatrix();
        public void StartFollowCameras(State gameState);
        public void TryChangeActiveCamera(string name);
        public void TryChangeActiveCamera(int id);
        public void TryChangeActiveCameraByIid(string? iid);
        public void ClearCameras();
    }
}