using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InvertoryModel> GetInventory();
        void SaveInventoryRecord(InvertoryModel item);
    }
}