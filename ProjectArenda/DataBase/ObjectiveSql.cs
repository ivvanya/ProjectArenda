using Npgsql;
using ProjectArenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class ObjectiveSql
    {
        private SQLConnection connection;
        public ObjectiveSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static ObjectiveSql _instanse;

        public static ObjectiveSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new ObjectiveSql();
            }
            return _instanse;
        }

        public List<Objective> SelectAll(int id = 0)
        {
            List<Objective> objectives = new List<Objective>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Objective WHERE ObjectiveID!=@id", connection.Get()))
            {
                command.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Objective objective = new Objective()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2)
                    };
                    objectives.Add(objective);
                }
            }
            connection.Close();
            return objectives;
        }

        public void WriteToDb(Objective objective)
        {
            if (!CheckNameExists(objective.Name, objective.Id))
            {
                MessageBox.Show("Цель аренды с таким названием уже существует");
                return;
            }
            else if (!CheckName(objective.Name))
            {
                MessageBox.Show("Некорректное название вида отделки");
                return;
            }
            else
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO Objective(name, description) 
                VALUES(@name, @desc)", connection.Get()))
                {
                    command.Parameters.AddWithValue("name", objective.Name);
                    command.Parameters.AddWithValue("desc", objective.Description);
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
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Objective WHERE ObjectiveID=@ID", connection.Get()))
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

        public void UpdateInDb(Objective objective)
        {
            try
            {
                if (!CheckNameExists(objective.Name, objective.Id))
                {
                    MessageBox.Show("Вид отделки с таким названием уже существует");
                    return;
                }
                else if (!CheckName(objective.Name))
                {
                    MessageBox.Show("Некорректное название вида отделки");
                    return;
                }
                else
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Objective SET Name=@Name, Description=@Description 
                    WHERE ObjectiveID=@ID", connection.Get()))
                    {
                        command.Parameters.AddWithValue("Name", objective.Name);
                        command.Parameters.AddWithValue("Description", objective.Description);
                        command.Parameters.AddWithValue("ID", objective.Id);
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
            foreach (Objective objective in SelectAll(id))
            {
                if (objective.Name == name)
                    return false;
            }
            return true;
        }
    }
}
