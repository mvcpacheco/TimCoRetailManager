using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [HttpPost]
        public void Post(SaleModel sale)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            var data = new SaleData();
            data.SaveSale(sale, userId);
        }
    }
}