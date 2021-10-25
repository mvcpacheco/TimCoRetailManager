using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Route("GetSalesReport")]
        [Authorize(Roles = "Manager,Admin")]
        public List<SaleReportModel> GetSalesReport()
        {
            var data = new SaleData();
            return data.GetSaleReport();
        }

        [HttpPost]

        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            var data = new SaleData();
            data.SaveSale(sale, userId);
        }
    }
}