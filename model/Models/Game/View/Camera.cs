using Microsoft.Xna.Framework;

namespace model.Models.Game.View
{
    public abstract class Camera
    {
        public readonly int Id;
        public readonly string Iid;
        public readonly string Name;
        public bool Enabled = false;
        public virtual Vector2 Coordinates { get; protected set; } = Vector2.Zero;
        public float X { get { return Coordinates.X; }}
        public float Y { get { return Coordinates.Y; }}
        public float Rotation { get; protected set; } = 0;
        public float Zoom { get; protected set; } = 1;

        public Camera(int id, string iid, string name, bool active)
        {
            Id = id;
            Iid = iid;
            Name = name;
            Enabled = active;
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void ApplyRotation(float value)
        {
            Rotation += value;
        }

        public void SetZoom(float zoom)
        {
            Zoom = zoom;
        }

        public void ApplyZoom(float value)
        {
            Zoom += value;
        }
    }
}