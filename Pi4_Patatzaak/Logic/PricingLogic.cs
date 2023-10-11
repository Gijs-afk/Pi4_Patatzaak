using System.Linq;
using Pi4_Patatzaak.Data;
using Pi4_Patatzaak.Models;
using Pi4_Patatzaak.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Pi4_Patatzaak.Exceptions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace Pi4_Patatzaak.Logic
{
    public class PricingLogic
    {
        private readonly AppDbContext _dbContext; 

        public PricingLogic(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public decimal GetProductPrice(int productId)
        {
            // Zoek het product op basis van het productId
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product == null)
            {
                throw new BadRequestException("Product was not found");
            }

            // Controleer of er een korting beschikbaar is voor dit product
            var sale = _dbContext.Discounts.FirstOrDefault(s => s.ProductID == productId);

            if (sale != null)
            {
                return sale.DiscountPrice;
            }
            return product.Price;
        }

       



    }
}

