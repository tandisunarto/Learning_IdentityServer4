using System;
using Microsoft.AspNetCore.Identity;

namespace Scarlet.Auth.Models
{
	public class ApplicationUser : IdentityUser
	{
		public Boolean Active { get; set; }
        public Boolean Approved { get; set; }
		public String FirstName { get; set; }
		public String LastName { get; set; }
        public DateTimeOffset LastPasswordChanged { get; set; }
        public DateTimeOffset LastLogin { get; set; }
        public DateTimeOffset Created { get; set; }
	}
}