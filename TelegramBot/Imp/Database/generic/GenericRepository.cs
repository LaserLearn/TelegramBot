using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;
using TelegramBot.config.mongo;
using TelegramBot.contract.Database.generic;
using TelegramBot.model.attribute;
using MongoDB.Driver.Linq;

namespace TelegramBot.Imp.Database.generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IMongoClient _mongoClient;
        protected readonly string _developDbName;
        protected readonly string _collectionName;

        public GenericRepository(
            IMongoClient mongoClient,
            MongoDBSettings settings)
        {
            _mongoClient = mongoClient;
            _developDbName = settings.Develop_DataBase.DatabaseName;
            _collectionName = GetCollectionName();
        }

        // ======== Utility Methods ========
        private string GetCollectionName()
        {
            var collectionNameAttribute = typeof(T).GetCustomAttribute<CollectionNameAttribute>();

            return collectionNameAttribute?.TableName ??
                   typeof(T).Name.ToLower() + "s";
        }

        public IMongoCollection<T> GetCollection(string? databaseName = null) =>
            _mongoClient
                .GetDatabase(string.IsNullOrWhiteSpace(databaseName) ? _developDbName : databaseName)
                .GetCollection<T>(_collectionName);



        // ======== Default Operations (Auto Read/Write Separation) ========
        public async Task<T> GetByIdAsync(string id)
            => await GetByIdAsync(id, _developDbName);

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
            => await GetAllAsync(_developDbName, filter, orderBy);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
            => await ExistsAsync(_developDbName, predicate);

        public async Task AddAsync(T entity)
            => await AddAsync(entity, _developDbName);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await AddRangeAsync(entities, _developDbName);

        public async Task UpdateAsync(T entity)
            => await UpdateAsync(entity, _developDbName);

        public async Task DeleteByIdAsync(string id)
            => await DeleteByIdAsync(id, _developDbName);

        public async Task DeleteByIdsAsync(IEnumerable<string> ids)
            => await DeleteByIdsAsync(ids, _developDbName);

        // ======== Overloads with Database Name Parameter ========
        public async Task<T> GetByIdAsync(string id, string databaseName)
        {
            var filter = BuildIdFilter(id);
            return await GetCollection(databaseName).Find(filter).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<T>> GetAllAsync(
            string databaseName,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            var query = GetCollection(databaseName).AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        public async Task<bool> ExistsAsync(
            string databaseName,
            Expression<Func<T, bool>> predicate)
        {
            return await GetCollection(databaseName)
                .Find(predicate)
                .AnyAsync();
        }

        public async Task AddAsync(T entity, string databaseName)
        {
            await GetCollection(databaseName)
                .InsertOneAsync(entity);
        }

        public async Task AddRangeAsync(
            IEnumerable<T> entities,
            string databaseName)
        {
            await GetCollection(databaseName)
                .InsertManyAsync(entities);
        }

        public async Task UpdateAsync(T entity, string databaseName)
        {
            var id = typeof(T).GetProperty("Id")?.GetValue(entity)?.ToString();
            var filter = BuildIdFilter(id);

            await GetCollection(databaseName).ReplaceOneAsync(filter, entity);
        }


        public async Task DeleteByIdAsync(string id, string databaseName)
        {
            var filter = BuildIdFilter(id);
            await GetCollection(databaseName).DeleteOneAsync(filter);
        }


        public async Task DeleteByIdsAsync(IEnumerable<string> ids, string databaseName)
        {
            var idProperty = typeof(T).GetProperty("Id");
            var bsonIdAttr = idProperty?.GetCustomAttribute<BsonRepresentationAttribute>();

            FilterDefinition<T> filter;

            if (bsonIdAttr != null && bsonIdAttr.Representation == BsonType.ObjectId)
            {
                var objectIds = ids.Select(x => new ObjectId(x));
                filter = Builders<T>.Filter.In("_id", objectIds);
            }
            else
            {
                filter = Builders<T>.Filter.In("_id", ids);
            }

            await GetCollection(databaseName).DeleteManyAsync(filter);
        }



        //private function 
        private FilterDefinition<T> BuildIdFilter(string id)
        {
            var idProperty = typeof(T).GetProperty("Id");
            var bsonIdAttr = idProperty?.GetCustomAttribute<BsonRepresentationAttribute>();

            if (bsonIdAttr != null && bsonIdAttr.Representation == BsonType.ObjectId)
            {
                return Builders<T>.Filter.Eq("_id", new ObjectId(id));
            }

            return Builders<T>.Filter.Eq("_id", id);
        }


    }
}
