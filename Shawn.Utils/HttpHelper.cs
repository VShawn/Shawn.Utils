﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Shawn.Utils
{
    public static class HttpHelper
    {
        #region POST

        public static string Post(string url, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            var result = "";
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            var builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }

            var data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            var resp = (HttpWebResponse)req.GetResponse();
            var stream = resp.GetResponseStream();
            using (var reader = new StreamReader(stream, encoding))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public static string Post(string url, string content, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            string result = "";
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            var data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            var resp = (HttpWebResponse)req.GetResponse();
            var stream = resp.GetResponseStream();
            using (var reader = new StreamReader(stream, encoding))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        #endregion POST

        #region GET

        public static string Get(string url, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            string result = "";
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Headers["Accept-Language"] = "zh-CN,zh;q=0.8";
            req.Referer = "https://www.google.com/";
            req.Timeout = 5 * 1000;
            var resp = (HttpWebResponse)req.GetResponse();
            var stream = resp.GetResponseStream();
            try
            {
                using var reader = new StreamReader(stream, encoding);
                result = reader.ReadToEnd();
            }
            finally
            {
                stream.Close();
            }
            return result;
        }

        public static string Get(string url, Dictionary<string, string> dic, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            var req = (HttpWebRequest)WebRequest.Create(builder.ToString());
            var resp = (HttpWebResponse)req.GetResponse();
            var stream = resp.GetResponseStream();
            try
            {
                using var reader = new StreamReader(stream, encoding);
                result = reader.ReadToEnd();
            }
            finally
            {
                stream.Close();
            }
            return result;
        }

        #endregion GET
    }
}