///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	04-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.StaticClass;
using Domain.ViewModel;
using Newtonsoft.Json;
using NReJSON;
using StackExchange.Redis;

namespace Infrastracture.Repositories
{
    public class RedisCache : IDisposable
    {
        public ConnectionMultiplexer connector;

        public RedisCache()
        {
            connector = ConnectionMultiplexer.Connect(Connections.REDISCS);
            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);
        }


        #region==========|  Dispose Method  |==========
        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                connector.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========

        #region=============================| Syncronous Methods |=============================

        public bool SetCache(string collectionName, string dataKey, string newData)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                OperationResult result = db.JsonSet(collectionName, newData, dataKey);
                string resultValue = result.RawResult.ToString();
                return resultValue.ToLower() == "ok";
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "SetCache", collectionName, dataKey, newData));
                return false;
            }
            finally
            {
                connector?.Close();
            }
        }


        public bool UpdateCache(string collectionName, string dataKey, string updateData)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                OperationResult result = db.JsonSet(collectionName, updateData.ToJsonString(), dataKey);
                string resultValue = result.RawResult.ToString();
                return resultValue.ToLower() == "ok";
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "UpdateCache", collectionName, dataKey, updateData));
                return false;
            }
            finally
            {
                connector?.Close();
            }
        }


        public bool UpdateCache(string collectionName, string dataKey, dynamic updateData)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                string jsonBody = updateData.ToJsonString();
                OperationResult result = db.JsonSet(collectionName, jsonBody, dataKey);
                string resultValue = result.RawResult.ToString();
                return resultValue.ToLower() == "ok";
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "UpdateCache", collectionName, dataKey, updateData.ToJsonString()));
                return false;
            }
            finally
            {
                connector?.Close();
            }
        }


        public string GetCache(string collectionName, string dataKey)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                string[] parms = { "." + dataKey };
                RedisResult result = db.JsonGet(collectionName, parms);
                return (RedisValue)result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "GetCache", collectionName, dataKey));
                return string.Empty;
            }
            finally
            {
                connector?.Close();
            }
        }


        public string GetCache(string collectionName)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                RedisResult result = db.JsonGet(collectionName);
                return (RedisValue)result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "GetCache", collectionName));
                return string.Empty;
            }
            finally
            {
                connector?.Close();
            }
        }


        public string AppendInArray(string collectionName, string path, string jsonStringData)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                RedisResult result = db.Execute("JSON.ARRAPPEND", new object[] { collectionName, path, jsonStringData });
                return (RedisValue)result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "AppendInArray", collectionName, path, jsonStringData));
                return string.Empty;
            }
            finally
            {
                connector?.Close();
            }
        }

        #endregion

        #region=============================| Asyncronous Methods |=============================

        public async Task<bool> SetCacheAsync(string collectionName, string dataKey, string jsonStringData)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                OperationResult result = await db.JsonSetAsync(collectionName, jsonStringData, dataKey);
                string resultValue = result.RawResult.ToString();
                return resultValue.ToLower() == "ok";
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "SetCacheAsync", collectionName, dataKey, jsonStringData));
                return false;
            }
            finally
            {
                await connector?.CloseAsync();
            }

        }


        public async Task<bool> SetCacheAsync(string collectionName, string jsonStringData)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                OperationResult result = await db.JsonSetAsync(collectionName, jsonStringData);
                string resultValue = result.RawResult.ToString();
                bool res = resultValue.ToLower() == "ok";
                return res;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "SetCacheAsync", collectionName, jsonStringData));
                return false;
            }
            finally
            {
                await connector?.CloseAsync();
            }

        }


        /// <summary>
        /// Example:  await redis.UpdateCacheAsync("test", "$.Testkey", "testval_updated_V2".ToJsonString());
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="dataKey"></param>
        /// <param name="updateStr"></param>
        /// <returns></returns>
        public async Task<bool> UpdateCacheAsync(string collectionName, string dataKey, string updateStr)
        {
            try
            {
                IDatabase db = connector.GetDatabase();

                RedisResult resp = await db.ExecuteAsync("JSON.SET", collectionName, dataKey, updateStr);

                return resp.ToString().ToLower() == "ok" ? true : false;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "UpdateCacheAsync", collectionName, dataKey, updateStr));
                return false;
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        public async Task<string> GetCacheAsync(string collectionName, string dataKey)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                string[] parms = { "." + dataKey };
                RedisResult result = await db.JsonGetAsync(collectionName, parms);
                return (RedisValue)result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "GetCacheAsync", collectionName, dataKey));
                return string.Empty;
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        public async Task<string> GetCacheAsync(string collectionName)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                RedisResult result = await db.JsonGetAsync(collectionName);
                return (RedisValue)result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "GetCacheAsync", collectionName));
                return string.Empty;
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        public async Task<string> AppendInArrayAsync(string collectionName, string path, string jsonStringData)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                RedisResult result = await db.ExecuteAsync("JSON.ARRAPPEND", new object[] { collectionName, path, jsonStringData });
                return (RedisValue)result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "AppendInArrayAsync", collectionName, path, jsonStringData));
                return string.Empty;
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        public async Task<bool> DeleteAsync(string collectionName, string key)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                RedisResult resp = await db.ExecuteAsync("JSON.DEL", [collectionName, key]);
                return (int)resp > 0;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "DeleteAsync", key));
                return false;
            }
            finally
            {
                await connector.CloseAsync();
            }
        }


        public async Task<long> MultiDeleteAsync(string collectionName, List<string> keys)
        {
            try
            {
                int result = 0;
                IDatabase db = connector.GetDatabase();
                IBatch batch = db.CreateBatch();

                Parallel.ForEach(keys, async key =>
                {
                    RedisResult resp = await db.ExecuteAsync("JSON.DEL", new object[] { collectionName, key });
                    result = (int)resp > 0 ? result + 1 : result;
                });

                return result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "MultiDeleteAsync", keys.ToJsonString()));
                return 0;
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        /// <summary>
        /// This api gets all keys of a Redis Hash collection
        /// </summary>
        /// <returns>Returns List<string></returns>
        public async Task<List<string>> GetAllKeys(string collectionName)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                HashEntry[] allCollection = await db.HashGetAllAsync(collectionName);

                List<string> redisKeys = allCollection.Select(s => s.Name.ToString()).ToList();

                return redisKeys;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "GetAllKeys", collectionName));
                return new List<string>();
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        /// <summary>
        /// This api gets values by keys from a Redis Hash collection
        /// </summary>
        /// <returns>Returns string</returns>
        public async Task<string> GetValueByKey(string collectionName, string key)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                string redisValue = await db.HashGetAsync(collectionName, key);

                return redisValue;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "GetValueByKey", collectionName, key));
                return string.Empty;
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        /// <summary>
        /// This api gets Banner Details by multiple keys from a Redis Hash collection
        /// </summary>
        /// <returns>Returns string</returns>
        public async Task<List<BannerDetailsRedis>> GetBannersByKeys(string collectionName, List<string> keys)
        {
            try
            {
                IDatabase db = connector.GetDatabase();

                List<BannerDetailsRedis> bannerList = new();

                RedisValue[] redisFields = keys.Select(key => (RedisValue)key).ToArray();

                RedisValue[] redisValue = await db.HashGetAsync(collectionName, redisFields);

                Parallel.ForEach(redisValue, (banner, ct) =>
                {
                    BannerDetailsRedis bindBanner = JsonConvert.DeserializeObject<BannerDetailsRedis>(banner);
                    bannerList.Add(bindBanner);
                });

                return bannerList;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "GetBannersByKeys", collectionName, string.Join(",", keys)));
                return new List<BannerDetailsRedis>();
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        public async Task<string> ClearFullCollectionAsync(string collectionName)
        {
            try
            {
                IDatabase db = connector.GetDatabase();
                RedisResult result = await db.ExecuteAsync("JSON.DEL", new object[] { collectionName, "$.*", string.Empty });
                return (RedisValue)result;
            }
            catch (Exception ex)
            {
                TextLogWriter.WriteRedisErrorLog(HelperMethod.FormattedRedisError(ex, "ClearFullCollectionAsync", collectionName));
                return string.Empty;
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }


        /// <summary>
        /// Add or Update single key value inside a collection.
        /// <para>Ex: JSON.SET doc $ '{"bool":true}'</para>
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="dataKey"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteSetAsync(string collectionName, string dataKey, dynamic keyValue)
        {
            try
            {
                string _dataKey = $"$.{dataKey}";
                string _keyValue = keyValue.ToJsonString(); //JsonConvert.SerializeObject(keyValue, Formatting.None);

                IDatabase db = connector.GetDatabase();
                RedisResult result = await db.ExecuteAsync("JSON.SET", collectionName, dataKey, _keyValue);
                return (RedisValue)result == "ok";
            }
            finally
            {
                await connector?.CloseAsync();
            }
        }

        #endregion


        #region=============================| Asyncronous MISC Methods |=============================

        public async Task RemoveLoginProviderFromRedis(string collectionName, List<string> keys)
        {
            using(RedisCache redisCache = new())
            {
                await redisCache.MultiDeleteAsync(collectionName, keys);
            }
        }

        #endregion=============================| Asyncronous MISC Methods |=============================

    }
}
