using System.Collections.Generic;

namespace kroniiapi.DB.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<Administrator> Administrators { get; set; }
        public ICollection<Admin> Admins { get; set; }
        public ICollection<Trainer> Trainers { get; set; }
        public ICollection<Trainee> Trainees { get; set; }
        public ICollection<Company> Companies { get; set; }
    }
}