﻿using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnlineStore.Core.CrossCuttingConcerns.Caching.Redis
{
    public class RedisCacheManager : ICacheManager
    {
        private readonly IDatabase _redisDb;

        public RedisCacheManager()
        {
            _redisDb = RedisConnectionFactory.Connection.GetDatabase();
        }

        public void Add(string key, object data, int expireAsMinute)
        {
            var newDate = DateTime.Now.AddMinutes(expireAsMinute);
            var expireTimeSpan = newDate.Subtract(DateTime.Now);
            _redisDb.StringSet(key, JsonConvert.SerializeObject(data), expireTimeSpan);
        }

        public void Clear()
        {
            var endpoints = _redisDb.Multiplexer.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var keys = _redisDb.Multiplexer.GetServer(endpoint).Keys();
                foreach (var key in keys)
                {
                    Remove(key);
                }
            }
        }

        public T Get<T>(string key)
        {
            var redisObject = _redisDb.StringGet(key);

            var val = JsonConvert.DeserializeObject<T>(redisObject);
            return val;
            //return redisObject.HasValue ? JsonConvert.DeserializeObject<T>(redisObject) : Activator.CreateInstance<T>();
        }

        public bool IsExist(string key)
        {
            return _redisDb.KeyExists(key);
        }

        public void Remove(string key)
        {
            _redisDb.KeyDelete(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var endpoints = _redisDb.Multiplexer.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var keys = _redisDb.Multiplexer.GetServer(endpoint).Keys(pattern: pattern);

                foreach (var key in keys)
                {
                    Remove(key);
                }
            }
        }
    }
}
