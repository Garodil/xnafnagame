using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using model.Game;
using model.Interfaces.Manager;

namespace model.Models.Game.View
{
    public class CameraManager : ICameraManager
    {
        private IList<Camera> _cameras = [];
        public Camera? ActiveCamera { get; private set; }
        private Vector2 _viewportSize;
        private Vector3 _viewportTranslation;

        private Camera? TryGetCamera(string name)
        {
            return _cameras.SingleOrDefault(x => x.Name == name);
        }

        private Camera? TryGetCamera(int id)
        {
            return _cameras.SingleOrDefault(x => x.Id == id);
        }

        private Camera? TryGetCameraIid(string? iid)
        {
            return _cameras.SingleOrDefault(x => x.Iid == iid);
        }

        public void SetViewportSize(int width, int height)
        {
            _viewportSize = new Vector2(width, height);
            _viewportTranslation = new Vector3(width / 2.0f, height / 2.0f, 0.0f);
        }

        public Matrix GetActiveCameraMatrix()
        {
            if (ActiveCamera == null) { return Matrix.Identity; }
            return Matrix.CreateTranslation(new Vector3(-ActiveCamera.Coordinates, 0)) *
            Matrix.CreateScale(ActiveCamera.Zoom) *
            Matrix.CreateRotationZ(ActiveCamera.Rotation) *
            Matrix.CreateTranslation(_viewportTranslation);
        }

        public void StartFollowCameras(State gameState)
        {
            var followCameras = _cameras.OfType<FollowCamera>();
            foreach (var followCamera in followCameras)
            {
                if (followCamera.TryGetFollowEntityIid(out var iid)) {
                    followCamera.SetFollowEntity(gameState.Entities.Single(entity => entity.Iid == iid)); 
                }
            }
        }

        public void AddCamera(Camera camera)
        {
            _cameras = [.. _cameras, camera];
        }

        public void AddCameras(IEnumerable<Camera> cameras)
        {
            _cameras = [.. _cameras, .. cameras];
        }

        public void ClearCameras()
        {
            _cameras = [];
        }

        public void TryChangeActiveCamera(string name)
        {
            ActiveCamera = TryGetCamera(name);
        }

        public void TryChangeActiveCamera(int id)
        {
            ActiveCamera = TryGetCamera(id);
        }
        
        public void TryChangeActiveCameraByIid(string? iid)
        {
            ActiveCamera = TryGetCameraIid(iid);
        }
    }
}
