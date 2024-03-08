using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class UserDataSchema
    {
        public string Name { get; private set; }
        public string Bio { get; private set; }
        public string Image { get; private set; }

        public UserDataSchema(string name, string bio, string image)
        {
            Name = name;
            Bio = bio;
            Image = image;
        }
    }
}
