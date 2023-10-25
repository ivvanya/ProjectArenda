using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class DistrictSql
    {
        private SQLConnection connection;
        public DistrictSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static DistrictSql _instanse;

        public static DistrictSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new DistrictSql();
            }
            return _instanse;
        }

        public List<District> SelectAllDistricts(int disid = 0)
        {
            List<District> districts = new List<District>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM District WHERE districtid!=@id", connection.Get()))
            {
                command.Parameters.AddWithValue("id", disid);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    District district = new District()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2)
                    };
                    districts.Add(district);
                }
            }
            connection.Close();
            return districts;
        }

        public void WriteToDb(District district)
        {
            if (!CheckNameExists(district.Name, district.Id))
            {
                MessageBox.Show("Район с таким названием уже существует");
                return;
            }
            else if (!CheckName(district.Name))
            {
                MessageBox.Show("Некорректное название района");
                return;
            }
            else
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO District(name, description) 
                VALUES(@name, @disc)", connection.Get()))
                {
                    command.Parameters.AddWithValue("name", district.Name);
                    command.Parameters.AddWithValue("disc", district.Description);
                    command.ExecuteNonQuery();
                }
                connection.Close();
                MessageBox.Show("Строка добавлена в базу данных");
            }

        }

        public void DeleteFromDb(int id)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM District WHERE DistrictID=@ID", connection.Get()))
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

        public void UpdateInDb(District district)
        {
            try
            {
                if (!CheckNameExists(district.Name, district.Id))
                {
                    MessageBox.Show("Район с таким названием уже существует");
                    return;
                }
                else if (!CheckName(district.Name))
                {
                    MessageBox.Show("Некорректное название района");
                    return;
                }
                else
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE District SET Name=@Name, Description=@Description 
                    WHERE DistrictID=@DistrictID", connection.Get()))
                    {
                        command.Parameters.AddWithValue("Name", district.Name);
                        command.Parameters.AddWithValue("Description", district.Description);
                        command.Parameters.AddWithValue("DistrictID", district.Id);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    MessageBox.Show("Строка обновлена в базе данных");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка Обновления");
            }
        }

        private bool CheckName(string name)
        {
            string pattern = "^(([а-яa-z])+ ?)*$";
            if (Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase))
                return true;
            return false;
        }

        private bool CheckNameExists(string name, int id)
        {
            foreach (District district in SelectAllDistricts(id))
            {
                if (district.Name == name)
                    return false;
            }
            return true;
        }

        public District GetDistrict(string name)
        {
            District district = new District();
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM District WHERE Name=@Name", connection.Get()))
            {
                command.Parameters.AddWithValue("Name", name);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    district.Id = reader.GetInt32(0);
                    district.Name = reader.GetString(1);
                }
            }
            connection.Close();
            return district;
        }
    }
}
