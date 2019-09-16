using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonApi.Configurations
{
    /// <summary>
    /// Represents the strongly-typed version of the ProjectConfigurations branch of appsettings.json.
    /// By using an instance of this class, controllers can read values from appsettings.json. 
    /// </summary>
    /// <remarks>
    /// The name of the class maps to a top level property in the JSON file. Additionally, while every member is technically
    /// allowed to be Get and Set, IOptions will not write any Sets to appsettings.json, just change the value at runtime.
    /// </remarks>
    public class ProjectConfigurations
    {
        public string ApplicationName { get; set; }
        public bool MentionsChocolate { get; set; }
        public Limits Limits { get; set; }
    }
}
