using System;
using System.Data;

namespace ACDClinicManagement.AppHelpers.AppServices.Calculation
{
    public static class CalculateBaseHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Objects

        public static DataTable DataTableAllVehicles, DataTableValuesAdded, DataTableVehicleTollsInformation;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static string GetValuAdded(object year)
        public static string GetValueAdded(object year)
        {
            var result = "";
            foreach (DataRow valueAdded in DataTableValuesAdded.Rows)
            {
                if (valueAdded["Year"].ToString() == year.ToString())
                {
                    result = valueAdded["ValueAdded"].ToString();
                    break;
                }
            }
            return result;
        }

        #endregion

        #region public static void AddSumRow(this DataTable dataTable)
        public static void AddSumRow(this DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0) return;
            decimal allToll = 0, allExhaustion = 0, allPenalty = 0, allAmount = 0;
            foreach (DataRow item in dataTable.Rows)
            {
                allToll += Convert.ToDecimal(item["Toll"]);
                allExhaustion += Convert.ToDecimal(item["Exhaustion"]);
                allPenalty += Convert.ToDecimal(item["Penalty"]);
                allAmount += Convert.ToDecimal(item["Amount"]);
            }
            dataTable.Rows.Add("مجموع", allToll, allExhaustion, allPenalty, allAmount);
        }

        #endregion

        #region public static (decimal allToll, decimal allExhaustion, decimal allPenalty, decimal allAmount) GetCalculatedInfo(this DataTable dataTable)
        public static (decimal allToll, decimal allExhaustion, decimal allPenalty, decimal allAmount) GetCalculatedInfo(this DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0) return(0, 0, 0, 0);
            decimal allTollAmount = 0, allExhaustionAmount = 0, allPenaltyAmount = 0, allAmount = 0;
            foreach (DataRow item in dataTable.Rows)
            {
                if (item["Year"].ToString() == "مجموع") continue;
                allTollAmount += Convert.ToDecimal(item["Toll"]);
                allExhaustionAmount +=Convert.ToDecimal(item["Exhaustion"]);
                allPenaltyAmount += Convert.ToDecimal(item["Penalty"]);
                allAmount += Convert.ToDecimal(item["Amount"]);
            }
            return (allTollAmount, allExhaustionAmount, allPenaltyAmount, allAmount);
        }

        #endregion


    }
}
