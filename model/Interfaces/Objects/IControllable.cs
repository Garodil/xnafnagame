using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model.Interfaces.Objects
{
    public interface IControllable
    {
        public void ApplyWalk(int value);
        public void ApplyJump(int value);
    }
}