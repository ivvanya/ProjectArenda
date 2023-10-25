using Npgsql;
using ProjectArenda.Models;
using System.Collections.Generic;

namespace ProjectArenda.DataBase
{
    public class DocumentsSql
    {
        private SQLConnection connection;
        public DocumentsSql()
        {
            connection = SQLConnection.GetInstanse();
        }

        private static DocumentsSql _instanse;

        public static DocumentsSql GetInstanse()
        {
            if (_instanse == null)
            {
                _instanse = new DocumentsSql();
            }
            return _instanse;
        }

        public List<Contract> GetContracts()
        {
            List<Contract> contracts = new List<Contract>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT contractid FROM contract ORDER BY contractid;", connection.Get()))
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contract contract = new Contract();
                    contract.Id = reader.GetInt32(0);
                    contracts.Add(contract);
                }
            }
            connection.Close();
            return contracts;
        }

        public Contract GetContract(int contractId)
        {
            Contract contract = new Contract();
            contract.Id = contractId;
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT c.starttime, e.name, r.name, t.tenantid, t.facestatus, c.endtime 
                FROM contract c JOIN employee e ON e.employeeid=c.employeeid JOIN tenant t ON t.tenantid=c.tenantid 
                JOIN role r ON r.roleid=e.roleid WHERE contractid=@contractid", connection.Get()))
            {
                command.Parameters.AddWithValue("contractid", contractId);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    contract.StartTime = reader.GetDateTime(0);
                    contract.employee.Name = reader.GetString(1);
                    contract.employee.role.Name = reader.GetString(2);
                    contract.tenant.Id = reader.GetInt32(3);
                    contract.tenant.FaceStatus = reader.GetBoolean(4);
                    contract.EndTime = reader.GetDateTime(5);
                }
            }
            connection.Close();

            connection.Open();
            if (contract.tenant.FaceStatus == true)
            {
                using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT i.name, i.number FROM tenant t JOIN individual i 
                    ON t.tenantid=i.individualid WHERE tenantid=@tenantid", connection.Get()))
                {
                    command.Parameters.AddWithValue("tenantid", contract.tenant.Id);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        contract.individual.Name = reader.GetString(0);
                        contract.individual.Number = reader.GetString(1);
                    }
                }
            }
            else
            {
                using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT e.name, e.headname, e.number, e.address, e.bank, 
                    e.bankaccount FROM tenant t JOIN entity e ON t.tenantid=e.entityid WHERE tenantid=@tenantid;", connection.Get()))
                {
                    command.Parameters.AddWithValue("tenantid", contract.tenant.Id);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        contract.entity.Name = reader.GetString(0);
                        contract.entity.HeadName = reader.GetString(1);
                        contract.entity.Number = reader.GetString(2);
                        contract.entity.Address = reader.GetString(3);
                        contract.entity.Bank = reader.GetString(4);
                        contract.entity.BankAccount = reader.GetString(5);
                    }
                }
            }
            connection.Close();
            return contract;
        }

        public List<RoomList> GetRoomLists(int contractid)
        {
            List<RoomList> roomlists = new List<RoomList>();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT rl.roomid, d.name, b.address, r.number, o.name, rl.price, rl.rentalperiod, r.square 
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
                    roomList.objective.Name = reader.GetString(4);
                    roomList.Price = reader.GetInt32(5);
                    roomList.RentalPeriod = reader.GetInt32(6);
                    roomList.room.Square = reader.GetInt32(7);
                    roomlists.Add(roomList);
                }
            }
            connection.Close();
            return roomlists;
        }
    }
}
