using System;
using System.Collections.Generic;
using System.Text;
using BoatModels;
using Dapper;

namespace BoatService
{
    public class BoatService : IBoatService
    {
        DynamicData objDynamicData = new DynamicData();
        DynamicParameters objDynamicParameters = new DynamicParameters();
        ISqlDBHelper objSqlDBHelper = new SqlDBHelper();
        DynamicData IBoatService.RegisterBoat(BoatRegisterModel objBoatRegisterModel)
        {
            objDynamicParameters.Add("@Sptype", 1);
            objDynamicParameters.Add("@BoatName", objBoatRegisterModel.BoatName);
            objDynamicParameters.Add("@HourlyRate", objBoatRegisterModel.HourlyRate);
            objDynamicParameters.Add("@OutputValue", 0,null,System.Data.ParameterDirection.Output);
            objDynamicData.data = objSqlDBHelper.Execute("@prcBoat", objDynamicParameters);
            objDynamicData.info.Status= objDynamicParameters.Get<int>("@Outputvalue");
            return objDynamicData;

        }

        DynamicData IBoatService.RentBoat(BoatRentedModel objBoatRentedModel)
        {
            objDynamicParameters.Add("@Sptype", 2);
            objDynamicParameters.Add("@CustomerName", objBoatRentedModel.CustomerName);
            objDynamicParameters.Add("@ID", objBoatRentedModel.BoatNumber);
            objDynamicParameters.Add("@OutputValue", 0, null, System.Data.ParameterDirection.Output);
            objDynamicData.data = objSqlDBHelper.Execute("@prcBoat", objDynamicParameters);
            objDynamicData.info.Status = objDynamicParameters.Get<int>("@Outputvalue");
            return objDynamicData;

        }
    }
}
