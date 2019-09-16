using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Configurations
{
    /// <summary>
    /// Represents the nested Limits properties inside of the ProjectConfigurations branch of appsettings.json.
    /// </summary>
    public class Limits
    {
        public int TotalNumberOfPeople { get; set; }
        public int TotalChocolateLovers { get; set; }
    }
}
