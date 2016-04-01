using System;
using System.Net;
using System.Runtime.Serialization.Json;

namespace Skcrwblr
{
    /// <summary>
    /// Makes requests to the KCRW API.
    /// </summary>
    static class KcrwApi
    {
        /// <summary>
        /// Fetches the first track in the tracklist returned by accessing the API at the
        /// specified URL.
        /// </summary>
        /// <param name="requestUrl">The API URL to access.</param>
        /// <returns>A <see cref="KcrwResponse"/> instance representing the result.</returns>
        public static KcrwResponse RequestNowPlaying(string requestUrl)
        {
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new WebException(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(KcrwResponse[]));
                object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                KcrwResponse[] jsonResponse = objResponse as KcrwResponse[];
                return jsonResponse[0];
            }
        }

        /// <summary>
        /// Fetches the tracklist returned by accessing the API at the specified URL.
        /// </summary>
        /// <param name="requestUrl">The API URL to access.</param>
        /// <returns>An array of <see cref="KcrwResponse"/> instances representing the tracklist.
        /// </returns>
        public static KcrwResponse[] RequestAll(string requestUrl)
        {
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new WebException(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(KcrwResponse[]));
                object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                KcrwResponse[] jsonResponse = objResponse as KcrwResponse[];
                return jsonResponse;
            }
        }
    }
}
