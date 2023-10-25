using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class RoleSql
    {
        private SQLConnection connection;
        public RoleSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static RoleSql _instanse;

        public static RoleSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new RoleSql();
            }
            return _instanse;
        }

        public List<Role> SelectAllRoles()
        {
            List<Role> roles = new List<Role>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Role", connection.Get()))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Role role = new Role();
                    role.Id = reader.GetInt32(0);
                    role.Name = reader.GetString(1);
                    roles.Add(role);
                }
            }
            connection.Close();
            return roles;
        }

        public void WriteToDb(Role role)
        {
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO Roles(Name) VALUES(@Name)", connection.Get()))
            {
                command.Parameters.AddWithValue("Name", role.Name);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public void DeleteFromDb(int id)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Role WHERE RoleID=@ID", connection.Get()))
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

        public void UpdateInDb(int id)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Role SET Name=@Name WHERE RoleID=@ID", connection.Get()))
                {
                    command.Parameters.AddWithValue("ID", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка Обновления");
            }
        }

        public Role GetRole(string name)
        {
            Role role = new Role();
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM Role WHERE Name=@Name", connection.Get()))
            {
                command.Parameters.AddWithValue("Name", name);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    role.Id = reader.GetInt32(0);
                    role.Name = reader.GetString(1);
                }
            }
            connection.Close();
            return role;
        }
    }
}
