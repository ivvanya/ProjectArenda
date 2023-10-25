using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class RoomListSql
    {
        private SQLConnection connection;
        public RoomListSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static RoomListSql _instanse;

        public static RoomListSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new RoomListSql();
            }
            return _instanse;
        }

        public List<Room> SelectRoomsToAdd()
        {
            List<Room> rooms = new List<Room>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT r.roomid, d.name, b.address, r.number FROM Room r 
                JOIN Building b ON r.buildingid=b.buildingid JOIN District d ON b.districtid=d.districtid 
                ORDER BY d.districtid, b.buildingid, r.number", connection.Get()))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Room room = new Room();
                    room.Id = reader.GetInt32(0);
                    room.building.district.Name = reader.GetString(1);
                    room.building.Address = reader.GetString(2);
                    room.Number = reader.GetInt32(3);
                    rooms.Add(room);
                }
            }
            connection.Close();
            return rooms;
        }

        public List<RoomList> SelectRoomsToUpdate(int contractid)
        {
            List<RoomList> roomlists = new List<RoomList>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT rl.roomid, d.name, b.address, r.number, o.objectiveid, o.name, rl.price, rl.rentalperiod 
                FROM RoomList rl JOIN Room r ON r.roomid=rl.roomid JOIN Objective o ON o.objectiveid=rl.objectiveid JOIN Building b ON r.buildingid=b.buildingid 
                JOIN District d ON b.districtid=d.districtid  WHERE rl.contractid=@contractid ORDER BY d.districtid, b.buildingid, r.number", connection.Get()))
            {
                command.Parameters.AddWithValue("contractid", contractid);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    RoomList roomList = new RoomList();
                    roomList.room.Id = reader.GetInt32(0);
                    roomList.room.building.district.Name = reader.GetString(1);
                    roomList.room.building.Address = reader.GetString(2);
                    roomList.room.Number = reader.GetInt32(3);
                    roomList.objective.Id = reader.GetInt32(4);
                    roomList.objective.Name = reader.GetString(5);
                    roomList.Price = reader.GetInt32(6);
                    roomList.RentalPeriod = reader.GetInt32(7);
                    roomlists.Add(roomList);
                }
            }
            connection.Close();
            return roomlists;
        }

        public int GetLastContractId()
        {
            int id;
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM Contract ORDER BY contractid DESC LIMIT 1",
                connection.Get()))
            {
                id = (int)command.ExecuteScalar();
            }
            connection.Close();
            return id;
        }

        public void WriteToDb(List<RoomList> roomLists)
        {
            foreach (RoomList rl in roomLists)
            {
                Objective objective = GetObjective(rl.objective.Name);
                rl.objective.Id = objective.Id;
            }

            connection.Open();
            foreach (RoomList roomList in roomLists)
            {
                using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO RoomList(roomid, contractid, price, rentalperiod, objectiveid) 
                    VALUES (@roomid, @contractid, @price, @rentalperiod, @objectiveid)", connection.Get()))
                {
                    command.Parameters.AddWithValue("roomid", roomList.room.Id);
                    command.Parameters.AddWithValue("contractid", roomList.contract.Id);
                    command.Parameters.AddWithValue("price", roomList.Price);
                    command.Parameters.AddWithValue("rentalperiod", roomList.RentalPeriod);
                    command.Parameters.AddWithValue("objectiveid", roomList.objective.Id);
                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }

        public void DeleteFromDb(int contractid)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM RoomList WHERE contractid=@contractid",
                    connection.Get()))
                {
                    command.Parameters.AddWithValue("contractid", contractid);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка удаления");
            }
        }

        public Objective GetObjective(string name)
        {
            Objective objective = new Objective();
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM Objective WHERE Name=@Name", connection.Get()))
            {
                command.Parameters.AddWithValue("Name", name);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    objective.Id = reader.GetInt32(0);
                    objective.Name = reader.GetString(1);
                    objective.Description = reader.GetString(2);
                }
            }
            connection.Close();
            return objective;
        }
    }
}
