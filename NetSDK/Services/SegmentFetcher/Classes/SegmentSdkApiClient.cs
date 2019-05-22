﻿using Splitio.CommonLibraries;
using System;
using System.Net;
using Splitio.Services.SplitFetcher.Interfaces;
using Common.Logging;
using Splitio.Services.Metrics.Interfaces;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SegmentSdkApiClient : SdkApiClient, ISegmentSdkApiClient
    {
        private const string SegmentChangesUrlTemplate = "/api/segmentChanges/{segment_name}";
        private const string UrlParameterSince = "?since=";
        private const string SegmentFetcherTime = "segmentChangeFetcher.time";
        private const string SegmentFetcherStatus = "segmentChangeFetcher.status.{0}";
        private const string SegmentFetcherException = "segmentChangeFetcher.exception";

        private static readonly ILog Log = LogManager.GetLogger(typeof(SegmentSdkApiClient));

        public SegmentSdkApiClient(HTTPHeader header, string baseUrl, long connectionTimeOut, long readTimeout, IMetricsLog metricsLog = null) : base(header, baseUrl, connectionTimeOut, readTimeout, metricsLog) { }

        public async Task<string> FetchSegmentChanges(string name, long since, CancellationToken token)
        {
            var clock = new Stopwatch();
            clock.Start();
            try
            {
                var requestUri = GetRequestUri(name, since);
                var response = await ExecuteGet(requestUri, token);

                if ((int)response.statusCode >= (int)HttpStatusCode.OK && (int)response.statusCode < (int)HttpStatusCode.Ambiguous)
                {
                    if (metricsLog != null)
                    {
                        metricsLog.Time(SegmentFetcherTime, clock.ElapsedMilliseconds);
                        metricsLog.Count(string.Format(SegmentFetcherStatus, response.statusCode), 1);
                    }

                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug($"FetchSegmentChanges with name '{name}' took {clock.ElapsedMilliseconds} milliseconds using uri '{requestUri}'");
                    }

                    return response.content;
                }

                if (metricsLog != null)
                {
                    metricsLog.Count(string.Format(SegmentFetcherStatus, response.statusCode), 1);
                }

                Log.Error(response.statusCode == HttpStatusCode.Forbidden
                    ? "factory instantiation: you passed a browser type api_key, please grab an api key from the Split console that is of type sdk"
                    : string.Format("Http status executing FetchSegmentChanges: {0} - {1}", response.statusCode.ToString(), response.content));

                return string.Empty;
            }
            catch (Exception e)
            {
                Log.Error("Exception caught executing FetchSegmentChanges", e);
                
                if (metricsLog != null)
                {
                    metricsLog.Count(SegmentFetcherException, 1);
                }

                return string.Empty;
            }
        }

        private string GetRequestUri(string name, long since)
        {
            var segmentChangesUrl = SegmentChangesUrlTemplate.Replace("{segment_name}", name);
            return string.Concat(segmentChangesUrl, UrlParameterSince, Uri.EscapeDataString(since.ToString()));
        }
    }
}
