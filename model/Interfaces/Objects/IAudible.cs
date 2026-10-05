using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Sound;

namespace model.Interfaces.Objects
{
    public interface IAudibleEmitter
    {
        IEnumerable<Audible> GetSounds();
    }
}