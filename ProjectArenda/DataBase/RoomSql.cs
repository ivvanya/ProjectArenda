using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class RoomSql
    {
        private SQLConnection connection;
        public RoomSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static RoomSql _instanse;

        public static RoomSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new RoomSql();
            }
            return _instanse;
        }

        public void UpdateInDb(Room room)
        {
            try
            {
                Decoration decoration = GetDecoration(room.decoration.Name);
                Building building = GetBuilding(room.building.Address);
                room.decoration.Id = decoration.Id;
                room.building.Id = building.Id;
                room.decoration.Name = decoration.Name;
                room.building.Address = building.Address;

                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Room SET buildingid=@buildingid, number=@number, 
                    floor=@floor, square=@square, decorationid=@decorationid, phone=@phone WHERE roomid=@roomid", connection.Get()))
                {
                    command.Parameters.AddWithValue("buildingid", room.building.Id);
                    command.Parameters.AddWithValue("number", room.Number);
                    command.Parameters.AddWithValue("floor", room.Floor);
                    command.Parameters.AddWithValue("square", room.Square);
                    command.Parameters.AddWithValue("decorationid", room.decoration.Id);
                    command.Parameters.AddWithValue("phone", room.Phone);
                    command.Parameters.AddWithValue("roomid", room.Id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка Обновления");
            }
        }

        public void DeleteFromDb(int id)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Room WHERE roomid=@ID", connection.Get()))
                {
                    command.Parameters.AddWithValue("ID", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка удаления");
            }
        }

        public void WriteToDb(Room room)
        {
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO Room(buildingid, number, floor, square, decorationid, phone) 
                VALUES(@buildingid, @number, @floor, @square, @decorationid, @phone)", connection.Get()))
            {
                command.Parameters.AddWithValue("buildingid", room.building.Id);
                command.Parameters.AddWithValue("number", room.Number);
                command.Parameters.AddWithValue("floor", room.Floor);
                command.Parameters.AddWithValue("square", room.Square);
                command.Parameters.AddWithValue("decorationid", room.decoration.Id);
                command.Parameters.AddWithValue("phone", room.Phone);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public void AddRoomsBuilding(int buildingid)
        {
            try
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Building SET rooms=rooms+1 WHERE buildingid=@buildingid", connection.Get()))
                {
                    command.Parameters.AddWithValue("buildingid", buildingid);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка Обновления");
            }
        }

        public List<Room> RoomBuildDecor()
        {
            List<Room> rooms = new List<Room>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT roomid, r.number, r.floor, r.square, r.phone, d.decorationid, 
                d.name, b.buildingid, b.address FROM room r JOIN building b ON r.buildingid=b.buildingid 
                JOIN decoration d ON r.decorationid=d.decorationid ORDER BY roomid", connection.Get()))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Room room = new Room();
                    room.Id = reader.GetInt32(0);
                    room.Number = reader.GetInt32(1);
                    room.Floor = reader.GetInt32(2);
                    room.Square = reader.GetInt32(3);
                    room.Phone = reader.GetBoolean(4);
                    room.decoration.Id = reader.GetInt32(5);
                    room.decoration.Name = reader.GetString(6);
                    room.building.Id = reader.GetInt32(7);
                    room.building.Address = reader.GetString(8);
                    rooms.Add(room);
                }
            }
            connection.Close();
            return rooms;
        }

        public List<Building> SelectBuildingsByDistrict(int district)
        {
            List<Building> buildings = new List<Building>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Building WHERE districtid=@id", connection.Get()))
            {
                command.Parameters.AddWithValue("id", district);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Building building = new Building();
                    building.Id = reader.GetInt32(0);
                    building.district.Id = reader.GetInt32(1);
                    building.Address = reader.GetString(2);
                    building.Floors = reader.GetInt32(3);
                    building.Rooms = reader.GetInt32(4);
                    building.ComNumb = reader.GetString(5);

                    buildings.Add(building);
                }
            }
            connection.Close();
            return buildings;
        }

        public Building GetBuilding(string name)
        {
            Building building = new Building();
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM Building WHERE Address=@Address", connection.Get()))
            {
                command.Parameters.AddWithValue("Address", name);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    building.Id = reader.GetInt32(0);
                    building.Address = reader.GetString(2);
                }
            }
            connection.Close();
            return building;
        }

        public Decoration GetDecoration(string name)
        {
            Decoration decoration = new Decoration();
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM Decoration WHERE Name=@Name", connection.Get()))
            {
                command.Parameters.AddWithValue("Name", name);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    decoration.Id = reader.GetInt32(0);
                    decoration.Name = reader.GetString(1);
                }
            }
            connection.Close();
            return decoration;
        }
    }
}
