using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly ICatalogContext _context;

        //injecting the _context class in the repository layer.
        //_context is responsible for creating DB connections
        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string Id)
        {
            //Mongo DB filters
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, Id);

            DeleteResult deleteResult = await _context
                                                .Products
                                                .DeleteOneAsync(filter);

            // To check whether the delete operation done properly or not using the IsAcknowledged & DeletedCount 
            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }

        public async Task<Product> GetProduct(string Id)
        {
            return await _context.Products.Find(p => p.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string productCategory)
        {
            //return await _context.Products.Find(p => p.Category.Equals(productCategory, StringComparison.OrdinalIgnoreCase)).ToListAsync();

            //mongo DB filters
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, productCategory);

            return await _context
                            .Products
                            .Find(filter)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string productName)
        {
            //return await _context.Products.Find(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase)).ToListAsync();

            //mongo DB filters
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, productName);

            return await _context
                            .Products
                            .Find(filter)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.Find(p => true).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            //The whole json object in the DB is replaced based on the product id.
            // as we know that Mongo DB stroes the information in Json format.

            var updateResult = await _context
                                        .Products
                                        .ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);

            // To check whether the delete operation done properly or not using the IsAcknowledged & ModifiedCount 
            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
        }
    }
}
