using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Entities
{
    public class Product
    {
        //Bson ID : To represent the id as Mongo DB ID. 
        [BsonId]
        //BsonType represents the objectId-> means the objectId is
        //generated in database.
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        public string Name { get; set; }
        public string  Category { get; set; }
        public string  Summary { get; set; }
        public string  Description { get; set; }
        public string  ImageFile { get; set; }
        public decimal Price { get; set; }
    }
}
