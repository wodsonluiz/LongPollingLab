using ExampleLongPollingWithTaskCompletionSource.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController: ControllerBase
    {
        public OrdersController()
        {
            
        }

        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return OrderHelper.GetFakeOrders(10);
        }
    }
}
