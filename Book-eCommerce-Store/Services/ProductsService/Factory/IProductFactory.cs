namespace Book_eCommerce_Store.Services.ProductsService.Factory
{
    public interface IProductFactory
    {
        public IProductsService GetProductsService(ProductCategory category);
    }
}
