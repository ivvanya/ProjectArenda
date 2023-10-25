using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System.Text.RegularExpressions;

namespace ProjectArenda.Tools
{
    public class BuildingTools
    {
        private BuildingSql buildingSql;
        public BuildingTools()
        {
            buildingSql = BuildingSql.GetInstanse();
        }

        public void AddBuilding(Building building)
        {
            DistrictSql districtSql = new DistrictSql();
            District district = districtSql.GetDistrict(building.district.Name);
            building.district.Id = district.Id;
            buildingSql.WriteToDb(building);
        }

        public void UpdateBuilding(Building building)
        {
            buildingSql.UpdateInDb(building);
        }

        public string checkData(Building building)
        {
            string status;
            if (building.district.Name == "" || building.Address == "" || building.Floors == 0 || building.ComNumb == "")
                status = "Заполните все поля";
            if (!CheckNumber(building.ComNumb))
                status = "Неправильный формат номера телефона";
            else if (!CkeckDistrictExists(building.district.Name))
                status = "Введите корректное название района";
            else if (!CheckNumberExists(building.ComNumb, building.Id))
                status = "Данный номер уже присвоен одному из зданий";
            else
                status = "good";

            return status;
        }
        private bool CheckNumber(string phoneNumber)
        {
            string reg = @"^\d{11}$";
            Regex regex = new Regex(reg);
            MatchCollection match = regex.Matches(phoneNumber);
            if (match.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool CheckNumberExists(string number, int id)
        {
            foreach (Building employee in buildingSql.SelectAllBuildings(id))
            {
                if (employee.ComNumb == number)
                    return false;
            }
            return true;
        }
        private bool CkeckDistrictExists(string currentDistrict)
        {
            DistrictSql districtSql = new DistrictSql();
            District district = districtSql.GetDistrict(currentDistrict);
            if (district.Id == 0)
                return false;
            else
                return true;
        }
    }
}
