using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();

        public string RoleList
        {
            get
            {
                // sort by value
                Roles = Roles.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                // comma separated list
                return string.Join(", ", Roles.Select(x => x.Value)); 
            }
        }

    }
}
