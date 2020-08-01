using BoatModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoatService
{
   public interface IBoatService
    {
        DynamicData RegisterBoat(BoatRegisterModel objBoatRegisterModel);
        DynamicData RentBoat(BoatRentedModel objBoatRentedModel);
    }
}
