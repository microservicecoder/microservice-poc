using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository: IBasketRepository
    {
        //IDistributedCache is used to communicate with Redis db, like we do in sql server 
        // dbContext.
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            //return type of the GetStringAsync is a string json.
            // user name is key. based on username we are retrieving the basket data

            var basket = await _redisCache.GetStringAsync(userName);
            if (string.IsNullOrEmpty(basket))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }
        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            //we dont need a create method as based on the key we will replace the entire basket
            //so update method works fine for first time insert or existing update.

            // SetStringAsync return nothing.its just a asynchronus taks operation.

            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);

        }
        public async Task DeleteBasket(string userName)
        {
            //RemoveAsync return nothing. its just a asynchronus taks operation.

            await _redisCache.RemoveAsync(userName);
        }
    }
}
