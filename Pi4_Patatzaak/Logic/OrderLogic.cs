using Pi4_Patatzaak.Data;
using Pi4_Patatzaak.Models;
using System.Security.Claims;

namespace Pi4_Patatzaak.Logic
{
    public class OrderLogic
    {
        private readonly AppDbContext _dbContext;
        private readonly DotEnvVariables _dotEnvVariables;
        private readonly AuthLogic _authLogic;

        public OrderLogic(AppDbContext dbContext, DotEnvVariables dotEnvVariables, AuthLogic authLogic)
        {
            _dbContext = dbContext;
            _dotEnvVariables = dotEnvVariables;
            _authLogic = authLogic;
        }
        public Order CreateOrder(Order order)
        {



            order.CustomerID = _authLogic.GetUserName();

            if(_dotEnvVariables.StandardOrderMessage == null)
            {
                order.Status = "Error getting status";
            }
            order.Status = _dotEnvVariables.StandardOrderMessage;

            return order;
        }
    }
}
