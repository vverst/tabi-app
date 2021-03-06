﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TabiApiClient.Http
{
    public class JsonContent : HttpContent
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly object _value;

        public JsonContent(object value)
        {
            _value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                using(StreamWriter sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
                {
                    _serializer.Serialize(sw, _value);
                }
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
