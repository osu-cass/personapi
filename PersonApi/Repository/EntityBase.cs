using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Repository
{
    public abstract class EntityBase
    {
        public long Id { get; protected set; }
    }
}
