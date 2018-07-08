using System;
using System.Collections.Generic;
using System.Linq;
using TestWork.Models;
using System.Threading.Tasks;
using TestWork.Salter;

namespace TestWork
{
    public class InitializeFirstRegistrator
    {
        public static void Initialize(DBContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Roles
                    {
                        RolesName = "Registrator",
                        RolesExistedFlag = true
                    },
                    new Roles
                    {
                        RolesName = "SimpleUser",
                        RolesExistedFlag = true
                    }
                );
                context.SaveChanges();
            }
            if (!context.Users.Any(r => r.UserRoles.RolesName == "Registrator"))
            {
                context.Users.Add(
                    new Users
                    {
                        UsersName = "FirstRegistrator",
                        UsersAddress = "FirstRegistratorAddress",
                        UsersEmail = "firstregistratoremail@testwork.com",
                        UsersPassword = Salter.Salter.GetHashString("testpass"),
                        UsersPhoneNumber = "+78001234567",
                        UsersExistedFlag = true,
                        RolesId = context.Roles.Where(r => r.RolesName == "Registrator").FirstOrDefault().RolesId
                    }
                    );
                context.SaveChanges();
            }
        }
    }
}
