using System;
using Microsoft.Xna.Framework;
using model.Models.Entities;

namespace model.Models.Game.View
{
    public class FollowCamera : Camera
    {
        private string? _followEntityIid;
        private readonly WeakReference<Entity> _followEntity;
        public override Vector2 Coordinates => GetEntityPosition() ?? Vector2.Zero;

        public FollowCamera(int id, string iid, string name, Entity? followEntity, bool active) : base(id, iid, name, active)
        {
            _followEntity = new WeakReference<Entity>(followEntity);
        }

        public FollowCamera(int id, string iid, string name, string? followEntityIid, bool active) : base(id, iid, name, active)
        {
            _followEntity = new WeakReference<Entity>(null);
            _followEntityIid = followEntityIid;
        }

        public void SetFollowEntity(Entity followEntity)
        {
            _followEntity.SetTarget(followEntity);
        }

        public bool TryGetFollowEntityIid(out string iid)
        {
            iid = _followEntityIid!;
            return iid != null;
        }

        private Vector2? GetEntityPosition()
        {
            return _followEntity.TryGetTarget(out var entity) ? entity.GetTexturePosition() : null;
        }
    }
}