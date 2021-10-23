using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class ProductData
    {
        public List<ProductModel> GetProducts()
        {
            var sql = new SqlDataAccess();

            var retorno = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", null, "TRMData");

            return retorno;
        }

        public ProductModel GetProductById(int productId)
        {
            var sql = new SqlDataAccess();

            var retorno = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

            return retorno;
        }
    }
}