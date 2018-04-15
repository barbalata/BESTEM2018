using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrojanManager.Models
{
    public class User
    {
        public User(string userDetails)
        {
            if(!String.IsNullOrEmpty(userDetails))
            {
                var userSplit = userDetails.Split(',');

                Guid = Guid.Parse(userSplit[0]);
                Name = userSplit[1];
                Ip = userSplit[2];
                Port = userSplit[3];
            }

        }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
    }
}
