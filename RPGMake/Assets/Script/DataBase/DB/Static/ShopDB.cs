using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ShopDB", menuName = "DataBases/DataBase/ShopDB", order = 0)]
public class ShopDB : StaticDB
{

    [SerializeField] public List<ShopDBData> _dataList=new List<ShopDBData>();


    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }

    protected override void SetDataList_child(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as ShopDBData).ToList();
    }
    #region 非継承
    public static void BuyItem(ItemDBData item,int buyCount)
    {
        var db = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0];
        db._haveMoney -= item._data._price * buyCount;
        db.ChengeItemNum(item._serchId,buyCount);
        SaveDataController.Instance.SetData<PartyDB, SavedDBData_party>(db);
        
    }

    //場所がわかりにくい　そのうち治したい
    public static int GetBuyableCount(IShopContent item)
    {
        var haveMoney = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0]._haveMoney;
        int buyAbleCount = haveMoney / item.GetShopContentData().price;
        return buyAbleCount;
    }
    #endregion
}
