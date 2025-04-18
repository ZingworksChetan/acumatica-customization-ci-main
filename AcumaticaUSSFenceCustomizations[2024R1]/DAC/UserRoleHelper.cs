using System.Collections.Generic;
using PX.Data;
using PX.SM;
namespace DAC
{
    public class UserRoleHelper
    {
        public static List<string> GetCurrentUserRolesList()
        {
            List<string> acumaticaRolesList = new List<string>();
            string loggedInUsername = PXAccess.GetUserName();
            if (string.IsNullOrEmpty(loggedInUsername))
                return acumaticaRolesList;
            // Acuminator disable once PX1003 NonSpecificPXGraphCreateInstance [Justification]
            PXGraph graphToFetchUsernames = new PXGraph();
            foreach (UsersInRoles record in PXSelectJoin<UsersInRoles,
                    InnerJoin<Roles, On<UsersInRoles.rolename, Equal<Roles.rolename>>>,
                    Where<UsersInRoles.username, Equal<Required<UsersInRoles.username>>>>
                    .Select(graphToFetchUsernames, loggedInUsername))
                {
                    acumaticaRolesList.Add(record.Rolename);
                }
                    return acumaticaRolesList;
        }
    }
}

