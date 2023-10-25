using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class DecorationSql
    {
        private SQLConnection connection;
        public DecorationSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static DecorationSql _instanse;

        public static DecorationSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new DecorationSql();
            }
            return _instanse;
        }

        public List<Decoration> SelectAll(int id = 0)
        {
            List<Decoration> decorations = new List<Decoration>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Decoration WHERE decorationid!=@id", connection.Get()))
            {
                command.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Decoration decoration = new Decoration()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2)
                    };
                    decorations.Add(decoration);
                }
            }
            connection.Close();
            return decorations;
        }

        public void WriteToDb(Decoration decoration)
        {
            if (!CheckNameExists(decoration.Name, decoration.Id))
            {
                MessageBox.Show("Вид отделки с таким названием уже существует");
                return;
            }
            else if (!CheckName(decoration.Name))
            {
                MessageBox.Show("Некорректное название вида отделки");
                return;
            }
            else
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO Decoration(name, description) 
                VALUES(@name, @disc)", connection.Get()))
                {
                    command.Parameters.AddWithValue("name", decoration.Name);
                    command.Parameters.AddWithValue("disc", decoration.Description);
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
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Decoration WHERE DecorationID=@ID", connection.Get()))
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

        public void UpdateInDb(Decoration decoration)
        {
            try
            {
                if (!CheckNameExists(decoration.Name, decoration.Id))
                {
                    MessageBox.Show("Вид отделки с таким названием уже существует");
                    return;
                }
                else if (!CheckName(decoration.Name))
                {
                    MessageBox.Show("Некорректное название вида отделки");
                    return;
                }
                else
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Decoration SET Name=@Name, Description=@Description 
                    WHERE DecorationID=@ID", connection.Get()))
                    {
                        command.Parameters.AddWithValue("Name", decoration.Name);
                        command.Parameters.AddWithValue("Description", decoration.Description);
                        command.Parameters.AddWithValue("ID", decoration.Id);
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
            foreach (Decoration decoration in SelectAll(id))
            {
                if (decoration.Name == name)
                    return false;
            }
            return true;
        }
    }
}
