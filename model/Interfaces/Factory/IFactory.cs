using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box2D.NET;
using model.Game;

namespace model.Interfaces.Factory
{
    public interface IFactory<Type>
    {
        public Type Get(State state);
    }
}