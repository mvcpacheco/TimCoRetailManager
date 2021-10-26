using System.Collections.Generic;

namespace TRMDesktopUI.Library.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();

        public string DisplayRoles
        {
            get
            {
                return string.Join(", ", Roles.Values);
            }
        }

    }
}
