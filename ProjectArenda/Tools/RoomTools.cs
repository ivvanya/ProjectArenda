using ProjectArenda.DataBase;
using ProjectArenda.Models;

namespace ProjectArenda.Tools
{
    public class RoomTools
    {
        private RoomSql roomSql;
        public RoomTools()
        {
            roomSql = RoomSql.GetInstanse();
        }

        public void AddRoom(Room room)
        {
            RoomSql roomSql = new RoomSql();
            roomSql.WriteToDb(room);
            roomSql.AddRoomsBuilding(room.building.Id);
        }

        public void UpdateRoom(Room room)
        {
            roomSql.UpdateInDb(room);
        }

        public string checkData(Room room)
        {
            string status;
            if (room.Number < 1 || room.Square < 0 || room.decoration.Name == "" || room.building.Address == "")
                status = "Заполните все поля";
            else if (!CkeckBuildingExists(room.building.Address))
                status = "Здание не существует";
            else if (!CheckDecorationExists(room.decoration.Name))
                status = "Вид отделки не существует";
            else
                status = "good";

            return status;
        }

        private bool CkeckBuildingExists(string address)
        {
            RoomSql roomSql = new RoomSql();
            Building building = roomSql.GetBuilding(address);
            if (building.Id == 0)
                return false;
            else
                return true;

        }
        private bool CheckDecorationExists(string name)
        {
            RoomSql roomSql = new RoomSql();
            Decoration decoration = roomSql.GetDecoration(name);
            if (decoration.Id == 0)
                return false;
            else
                return true;

        }
    }
}
