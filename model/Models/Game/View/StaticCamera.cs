using Microsoft.Xna.Framework;

namespace model.Models.Game.View
{
    public class StaticCamera : Camera
    {
        public StaticCamera(int id, string iid, string name, Vector2 coordinates, bool active) : base(id, iid, name, active)
        {
            Coordinates = coordinates;
        }

        public void SetCoordinates(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }

        public void ApplyCoordinates(Vector2 value)
        {
            Coordinates += value;
        }
    }
}