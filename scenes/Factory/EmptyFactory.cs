using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box2D.NET;
using model.Game;
using model.Interfaces.Factory;

namespace scenes.Factory
{
    public class EmptyFactory<Type> : IFactory<Type>
    {
        public Type Get(State state)
        {
            throw new NotImplementedException();
        }
    }
}