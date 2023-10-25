using Npgsql;
using ProjectArenda.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectArenda.DataBase
{
    public class ContractSql
    {
        private SQLConnection connection;
        public ContractSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static ContractSql _instanse;

        public static ContractSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new ContractSql();
            }
            return _instanse;
        }

        public void WriteToDb(Contract contract)
        {
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO Contract(tenantid, employeeid, 
                periodicityid, penalty, additionally, starttime, endtime) VALUES(@tenantid, @employeeid, 
                @periodicityid, @penalty, @additionally, @starttime, @endtime)", connection.Get()))
            {
                command.Parameters.AddWithValue("tenantid", contract.tenant.Id);
                command.Parameters.AddWithValue("employeeid", contract.employee.Id);
                command.Parameters.AddWithValue("periodicityid", contract.periodicity.Id);
                command.Parameters.AddWithValue("penalty", contract.Penalty);
                command.Parameters.AddWithValue("additionally", contract.Additionaly);
                command.Parameters.AddWithValue("starttime", contract.StartTime);
                command.Parameters.AddWithValue("endtime", contract.EndTime);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public List<Contract> AllContractsIndividual()
        {
            List<Contract> contracts = new List<Contract>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT c.contractid, i.individualid, i.name, e.employeeid,
                e.name, p.periodicityid, p.name, c.penalty, c.additionally, c.starttime, c.endtime FROM contract c 
                JOIN individual i ON i.individualid=c.tenantid JOIN employee e ON e.employeeid=c.employeeid 
                JOIN periodicity p ON p.periodicityid=c.periodicityid ORDER BY c.contractid", connection.Get()))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contract contract = new Contract();
                    contract.Id = reader.GetInt32(0);
                    contract.individual.Id = reader.GetInt32(1);
                    contract.individual.Name = reader.GetString(2);
                    contract.employee.Id = reader.GetInt32(3);
                    contract.employee.Name = reader.GetString(4);
                    contract.periodicity.Id = reader.GetInt32(5);
                    contract.periodicity.Name = reader.GetString(6);
                    contract.Penalty = reader.GetInt32(7);
                    contract.Additionaly = reader.GetString(8);
                    contract.StartTime = reader.GetDateTime(9);
                    contract.EndTime = reader.GetDateTime(10);
                    contracts.Add(contract);
                }
            }
            connection.Close();
            return contracts;
        }

        public List<Contract> AllContractsEntity()
        {
            List<Contract> contracts = new List<Contract>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT c.contractid, en.entityid, en.name, e.employeeid,
                e.name, p.periodicityid, p.name, c.penalty, c.additionally, c.starttime, c.endtime FROM contract c 
                JOIN entity en ON en.entityid=c.tenantid JOIN employee e ON e.employeeid=c.employeeid 
                JOIN periodicity p ON p.periodicityid=c.periodicityid ORDER BY c.contractid", connection.Get()))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contract contract = new Contract();
                    contract.Id = reader.GetInt32(0);
                    contract.entity.Id = reader.GetInt32(1);
                    contract.entity.Name = reader.GetString(2);
                    contract.employee.Id = reader.GetInt32(3);
                    contract.employee.Name = reader.GetString(4);
                    contract.periodicity.Id = reader.GetInt32(5);
                    contract.periodicity.Name = reader.GetString(6);
                    contract.Penalty = reader.GetInt32(7);
                    contract.Additionaly = reader.GetString(8);
                    contract.StartTime = reader.GetDateTime(9);
                    contract.EndTime = reader.GetDateTime(10);
                    contracts.Add(contract);
                }
            }
            connection.Close();
            return contracts;
        }

        public int GetEntitieId(string number)
        {
            int id = 0;
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT entityid FROM Entity WHERE INN=@INN", connection.Get()))
            {
                command.Parameters.AddWithValue("INN", number);
                id = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
            return id;
        }

        public int GetIndividualId(string series, string number)
        {
            int id = 0;
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT individualid FROM Individual WHERE passseries=@series 
                and passnumber=@number", connection.Get()))
            {
                command.Parameters.AddWithValue("series", series);
                command.Parameters.AddWithValue("number", number);
                id = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
            return id;
        }

        public void DeleteFromDb(int id)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Contract WHERE contractid=@ID", connection.Get()))
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

        public void UpdateInDb(Contract contract)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"UPDATE Contract SET periodicityid=@periodicityid, 
                    penalty=@penalty, additionally=@additionally WHERE contractid=@contractid", connection.Get()))
                {
                    command.Parameters.AddWithValue("periodicityid", contract.periodicity.Id);
                    command.Parameters.AddWithValue("penalty", contract.Penalty);
                    command.Parameters.AddWithValue("additionally", contract.Additionaly);
                    command.Parameters.AddWithValue("contractid", contract.Id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка Обновления");
            }
        }

        public Periodicity GetPeriodicity(string name)
        {
            Periodicity periodicity = new Periodicity();
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM Periodicity WHERE Name=@Name", connection.Get()))
            {
                command.Parameters.AddWithValue("Name", name);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    periodicity.Id = reader.GetInt32(0);
                    periodicity.Name = reader.GetString(1);
                }
            }
            connection.Close();
            return periodicity;
        }
    }
}
