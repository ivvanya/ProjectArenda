using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class PeriodicitySql
    {
        private SQLConnection connection;
        public PeriodicitySql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static PeriodicitySql _instanse;

        public static PeriodicitySql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new PeriodicitySql();
            }
            return _instanse;
        }

        public List<Periodicity> SelectAll(int id = 0)
        {
            List<Periodicity> periodicityList = new List<Periodicity>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Periodicity WHERE periodicityid!=@id", connection.Get()))
            {
                command.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Periodicity periodicity = new Periodicity()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2)
                    };
                    periodicityList.Add(periodicity);
                }
            }
            connection.Close();
            return periodicityList;
        }

        public void WriteToDb(Periodicity periodicity)
        {
            if (!CheckNameExists(periodicity.Name, periodicity.Id))
            {
                MessageBox.Show("Периодичность выплат с таким названием уже существует");
                return;
            }
            else
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO Periodicity(name, description) 
                VALUES(@name, @disc)", connection.Get()))
                {
                    command.Parameters.AddWithValue("name", periodicity.Name);
                    command.Parameters.AddWithValue("disc", periodicity.Description);
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
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Periodicity WHERE periodicityid=@ID", connection.Get()))
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

        public void UpdateInDb(Periodicity periodicity)
        {
            try
            {
                if (!CheckNameExists(periodicity.Name, periodicity.Id))
                {
                    MessageBox.Show("Периодичность выплат с таким названием уже существует");
                    return;
                }
                else
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Periodicity SET Name=@Name, Description=@Description 
                    WHERE periodicityid=@ID", connection.Get()))
                    {
                        command.Parameters.AddWithValue("Name", periodicity.Name);
                        command.Parameters.AddWithValue("Description", periodicity.Description);
                        command.Parameters.AddWithValue("ID", periodicity.Id);
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
                
        private bool CheckNameExists(string name, int id)
        {
            foreach (Periodicity periodicity in SelectAll(id))
            {
                if (periodicity.Name == name)
                    return false;
            }
            return true;
        }
    }
}
