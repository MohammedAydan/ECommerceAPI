using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Model.Entities
{
    public class UserModel : IdentityUser
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
}
