﻿using Splitio.Services.Cache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Classes
{
    public class RedisSegmentCache : ISegmentCache
    {
        private IRedisAdapter redisAdapter;
        private const string segmentKeyPrefix = "SPLITIO.segment.";
        private const string segmentNameKeyPrefix = "SPLITIO.segment.{segmentname}.";
        private const string segmentsKeyPrefix = "SPLITIO.segments.";

        public RedisSegmentCache(IRedisAdapter redisAdapter)
        {
            this.redisAdapter = redisAdapter;
        }

        public void AddToSegment(string segmentName, List<string> segmentKeys)
        {
            var valuesToAdd = segmentKeys.Select(x => (RedisValue)x).ToArray();
            redisAdapter.SAdd(segmentKeyPrefix + segmentName, valuesToAdd);
        }

        public void RemoveFromSegment(string segmentName, List<string> segmentKeys)
        {
            var valuesToRemove = segmentKeys.Select(x => (RedisValue)x).ToArray();
            redisAdapter.SRem(segmentKeyPrefix + segmentName, valuesToRemove);
        }

        public bool IsInSegment(string segmentName, string key)
        {
            return redisAdapter.SIsMember(segmentKeyPrefix + segmentName, key);
        }

        public void SetChangeNumber(string segmentName, long changeNumber)
        {
            var key = segmentNameKeyPrefix.Replace("{segmentname}", segmentName) + "till";
            redisAdapter.Set(key, changeNumber.ToString());
        }

        public long GetChangeNumber(string segmentName)
        {
            var key = segmentNameKeyPrefix.Replace("{segmentname}", segmentName) + "till";
            string changeNumberString = redisAdapter.Get(key);
            long changeNumberParsed;
            var result = long.TryParse(changeNumberString, out changeNumberParsed);

            return result ? changeNumberParsed : -1;
        }

        public long RegisterSegment(string segmentName)
        {
            return RegisterSegments(new List<string>() { segmentName });
        }

        public long RegisterSegments(List<string> segmentNames)
        {
            var key = segmentsKeyPrefix + "registered";
            var segments = segmentNames.Select(x => (RedisValue)x).ToArray();
            return redisAdapter.SAdd(key, segments);
        }

        public List<string> GetRegisteredSegments()
        {
            var key = segmentsKeyPrefix + "registered";
            var result = redisAdapter.SMembers(key);

            return result.Select(x => (string)x).ToList();
        }

        public void Flush()
        {
            redisAdapter.Flush();
        }
    }
}