﻿using System.Collections.Generic;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class UserData
    {
        public List<UserModel> GetUserById(string Id)
        {
            var sql = new SqlDataAccess();

            var p = new { Id = Id };

            var retorno = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "TRMData");

            return retorno;
        }
    }
}