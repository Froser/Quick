using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Froser.Automaton.Network;

namespace Froser.Quick.WebSite
{
    public static class DatabaseCenter
    {
        public static DatabaseInformation GetDatabaseTable(String tableName)
        {
            return new DatabaseInformation("10.20.133.13", "db_quick", "quick", "quick", tableName, "", "3306");
        }
    }
}