using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System.Collections.Generic;

namespace ProjectArenda.Tools
{
    public class RoomListTools
    {
        private RoomListSql roomListSql;
        public RoomListTools()
        {
            roomListSql = RoomListSql.GetInstanse();
        }

        public void CreateRoomList(List<RoomList> roomLists)
        {
            int contractid = roomListSql.GetLastContractId();
            foreach (RoomList roomList in roomLists)
            {
                roomList.contract.Id = contractid;
            }
            roomListSql.WriteToDb(roomLists);
        }

        public void UpdateRoomList(List<RoomList> roomLists)
        {
            int contractid = roomLists[0].contract.Id;
            roomListSql.DeleteFromDb(contractid);
            roomListSql.WriteToDb(roomLists);
        }
    }
}
