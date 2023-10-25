using ProjectArenda.DataBase;
using ProjectArenda.Models;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ProjectArenda.Tools
{
    public class ContractTools
    {
        private ContractSql contractSql;
        public ContractTools()
        {
            contractSql = ContractSql.GetInstanse();
        }

        public bool addContract(Contract contract, int statusid)
        {
            if (statusid == 0)
                contract.tenant.Id = contractSql.GetIndividualId(contract.individual.PassSeries, contract.individual.PassNumber);
            else
                contract.tenant.Id = contractSql.GetEntitieId(contract.entity.INN);

            string status = checkData(contract);
            if (status == "good")
            {
                contractSql.WriteToDb(contract);
                return true;
            }
            else
            {
                MessageBox.Show(status);
                return false;
            }
        }

        public void updateContract(Contract contract)
        {
            Periodicity periodicity = contractSql.GetPeriodicity(contract.periodicity.Name);
            if (periodicity.Id == 0)
            {
                MessageBox.Show("Введите корректную периодичность выплат");
            }
            else
            {
                contract.periodicity.Id = periodicity.Id;
                contractSql.UpdateInDb(contract);
                MessageBox.Show("Данные обновлены");
            }
        }

        public string checkData(Contract contract)
        {
            string status;
            if (!CheckINN(contract.entity.INN))
                status = "Неправильный формат номера ИНН";
            else if (!CheckPassNumber(contract.individual.PassNumber) || !CheckPassSeries(contract.individual.PassSeries))
                status = "Неправильный формат паспортных данных";
            else if (contract.tenant.Id == 0)
                status = "В базе данных нет клиента с таким номером документа";
            else
                status = "good";

            return status;
        }

        private bool CheckINN(string number)
        {
            string reg = @"^\d{10}$";
            Regex regex = new Regex(reg);
            MatchCollection match = regex.Matches(number);
            if (match.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool CheckPassNumber(string number)
        {
            string reg = @"^\d{6}$";
            Regex regex = new Regex(reg);
            MatchCollection match = regex.Matches(number);
            if (match.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool CheckPassSeries(string account)
        {
            string emailreg = @"^\d{4}$";
            Regex regex = new Regex(emailreg);
            MatchCollection match = regex.Matches(account);
            if (match.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
