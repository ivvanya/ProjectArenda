using ProjectArenda.DataBase;

namespace ProjectArenda.Tools
{
    public class AccessRightsTools
    {
        private MenuStructureSql menuStructureSql;
        public AccessRightsTools()
        {
            menuStructureSql = MenuStructureSql.GetInstanse();
        }

    }
}
