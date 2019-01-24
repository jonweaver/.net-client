﻿using Newtonsoft.Json;
using Splitio.Domain;
using Splitio.Redis.Services.Cache.Interfaces;
using Splitio.Services.Shared.Interfaces;
using System.Collections.Generic;

namespace Splitio.Redis.Services.Cache.Classes
{
    public class RedisImpressionsCache : RedisCacheBase, ISimpleCache<KeyImpression>
    {
        private const string impressionKeyPrefix = "impressions.";

        public RedisImpressionsCache(IRedisAdapter redisAdapter, string machineIP, string sdkVersion, string userPrefix = null)
            : base(redisAdapter, machineIP, sdkVersion, userPrefix) 
        {}

        public void AddItem(KeyImpression item)
        {
            var key = redisKeyPrefix + impressionKeyPrefix + item.feature;
            var impressionJson = JsonConvert.SerializeObject(item);
            redisAdapter.SAdd(key, impressionJson);
        }

        //public void AddItem(KeyImpression item)
        //{
        //    var key = string.Format("(0)SPLITIO.impressions", string.IsNullOrEmpty(UserPrefix) ? string.Empty : $"{UserPrefix}.");

        //    var impressions = new List<object>();

        //    //foreach (var item in items)
        //    //{
        //        impressions.Add(new
        //        {
        //            m = new { s = SdkVersion, i = MachineIp, n = "" },
        //            i = new { k = item.keyName, b = item.bucketingKey, f = item.feature, t = item.treatment, r = item.label, c = item.changeNumber, m = item.time }
        //        });
        //    //}

        //    redisAdapter.ListRightPush(key, JsonConvert.SerializeObject(impressions));
        //}
    }
}
