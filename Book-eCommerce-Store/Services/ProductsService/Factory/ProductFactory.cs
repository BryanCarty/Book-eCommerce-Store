using Book_eCommerce_Store.DTOs.Products;

namespace Book_eCommerce_Store.Services.ProductsService.Factory
{
    public class ProductFactory : IProductFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ProductFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IProductsService GetProductsService(ProductCategory category)
        {
            if (category == ProductCategory.Books)
            {
                return (IProductsService)serviceProvider.GetService(typeof(BooksService));
            }
            else
            {
                return (IProductsService)serviceProvider.GetService(typeof(StationaryService));
            }
        }
    }
}
