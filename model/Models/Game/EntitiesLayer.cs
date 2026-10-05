using System;
using ldtk;
using model.Models.Map;

namespace model.Models.Game
{
    public class EntitiesLayer : Layer
    {
        public EntityInstance[] Entities;

        public string[] RequiredTags { set; get; }

        public int Order { set; get; }

        public EntitiesLayer(EntityInstance[] entities, string[] tags, int order)
        {
            Entities = entities;
            RequiredTags = tags;
            Order = order;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                _disposedValue = true;
            }
        }
    }
}