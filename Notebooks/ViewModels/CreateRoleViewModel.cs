using Microsoft.AspNetCore.Identity;
using Notebooks.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notebooks.ViewModels
{
    public class CreateRoleViewModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public bool IsActive { get; set; }
        public List<UserRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public CreateRoleViewModel()
        {
            AllRoles = new List<UserRole>();
            UserRoles = new List<string>();
        }
    }
}
