using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class InventoryData
    {
        public List<InvertoryModel> GetInventory()
        {
            SqlDataAccess sql = new SqlDataAccess();

            var output = sql.LoadData<InvertoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");

            return output;
        }

        public void SaveInventoryRecord(InvertoryModel item)
        {
            SqlDataAccess sql = new SqlDataAccess();

            sql.SaveData("dbo.spInventory_Insert", item, "TRMData");
        }
    }
}
