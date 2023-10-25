using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class BuildingSql
    {
        private SQLConnection connection;
        public BuildingSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static BuildingSql _instanse;

        public static BuildingSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new BuildingSql();
            }
            return _instanse;
        }

        public List<Building> BuildingsWithDistricts()
        {
            List<Building> buildings = new List<Building>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT buildingid, d.districtid, d.name, address, floors, rooms, 
                comnumber FROM building b JOIN district d ON b.districtid=d.districtid ORDER BY buildingid;", connection.Get()))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Building building = new Building();
                    building.Id = reader.GetInt32(0);
                    building.district.Id = reader.GetInt32(1);
                    building.district.Name = reader.GetString(2);
                    building.Address = reader.GetString(3);
                    building.Floors = reader.GetInt32(4);
                    building.Rooms = reader.GetInt32(5);
                    building.ComNumb = reader.GetString(6);
                    buildings.Add(building);
                }
            }
            connection.Close();
            return buildings;
        }

        public List<Building> SelectAllBuildings(int buildid = 0)
        {
            List<Building> buildings = new List<Building>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Building WHERE buildingid!=@id", connection.Get()))
            {
                command.Parameters.AddWithValue("id", buildid);
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

        public void DeleteFromDb(int id)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Building WHERE buildingid=@ID", connection.Get()))
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

        public void WriteToDb(Building building)
        {
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO Building(districtid, address, floors, rooms, comnumber) 
                VALUES(@districtid, @address, @floors, @rooms, @comnumber)", connection.Get()))
            {
                command.Parameters.AddWithValue("districtid", building.district.Id);
                command.Parameters.AddWithValue("address", building.Address);
                command.Parameters.AddWithValue("floors", building.Floors);
                command.Parameters.AddWithValue("rooms", building.Rooms);
                command.Parameters.AddWithValue("comnumber", building.ComNumb);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public void UpdateInDb(Building building)
        {
            try
            {
                DistrictSql districtSql = new DistrictSql();
                District district = new District();
                district = districtSql.GetDistrict(building.district.Name);
                building.district.Id = district.Id;
                building.district.Name = district.Name;

                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Building SET districtid=@districtid, address=@address, 
                    floors=@floors, rooms=@rooms, comnumber=@comnumber WHERE buildingid=@buildingid", connection.Get()))
                {
                    command.Parameters.AddWithValue("districtid", building.district.Id);
                    command.Parameters.AddWithValue("address", building.Address);
                    command.Parameters.AddWithValue("floors", building.Floors);
                    command.Parameters.AddWithValue("rooms", building.Rooms);
                    command.Parameters.AddWithValue("comnumber", building.ComNumb);
                    command.Parameters.AddWithValue("buildingid", building.Id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка Обновления");
            }
        }
    }
}
