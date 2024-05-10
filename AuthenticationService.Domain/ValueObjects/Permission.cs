using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.ValueObjects
{
    public class Permission
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Permission(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
