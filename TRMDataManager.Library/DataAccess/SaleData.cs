using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            if (saleInfo?.SaleDetails.Count > 0)
            {
                var detais = new List<SaleDetailDBModel>();
                var products = new ProductData();

                foreach (var item in saleInfo.SaleDetails)
                {
                    var detail = new SaleDetailDBModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };

                    var productInfo = products.GetProductById(detail.ProductId);

                    if (productInfo == null)
                    {
                        throw new System.Exception($"O código do produto {detail.ProductId} não foi encontrado no banco de dados");
                    }

                    detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

                    if (productInfo.IsTaxable)
                    {
                        detail.Tax = productInfo.RetailPrice * detail.Quantity * (productInfo.TaxRate / 100);
                    }

                    detais.Add(detail);
                }

                var sale = new SaleDBModel
                {
                    SubTotal = detais.Sum(x => x.PurchasePrice),
                    Tax = detais.Sum(x => x.Tax),
                    CashierId = cashierId
                };

                sale.Total = sale.SubTotal + sale.Tax;

                using (var sql = new SqlDataAccess())
                {
                    try
                    {
                        sql.BeginTransaction("TRMData");

                        sale.Id = sql.SaveDataInTransaction("dbo.spSale_Insert", sale, "Id", System.Data.DbType.Int32);

                        foreach (var item in detais)
                        {
                            item.SaleId = sale.Id;
                            sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                        }

                        sql.CommitTransaction();
                    }
                    catch
                    {
                        sql.RollbackTransaction();
                        throw;
                    }
                }
            }
        }
    }
}