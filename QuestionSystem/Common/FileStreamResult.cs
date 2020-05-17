using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace QuestionSystem.Common
{
    /// <summary>
    /// Class FileStreamResult.
    /// </summary>
    /// <seealso cref="System.Web.Http.IHttpActionResult" />
    public class FileStreamResult : IHttpActionResult
    {
        readonly Stream _stream;
        readonly string _mediaType;
        readonly string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamResult"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="mediaType">Type of the media.</param>
        public FileStreamResult(Stream stream, string mediaType) : this(stream, mediaType, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamResult"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="fileName">Name of the file.</param>
        public FileStreamResult(Stream stream, string mediaType, string fileName)
        {
            _stream = stream;
            _mediaType = mediaType;
            _fileName = fileName;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpResponseMessage>(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                httpResponseMessage.Content = new StreamContent(_stream);
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(_mediaType);
                if (!string.IsNullOrEmpty(_fileName))
                {
                    var browser = String.Empty;
                    if (HttpContext.Current.Request.UserAgent != null)
                    {
                        browser = HttpContext.Current.Request.UserAgent.ToUpper();
                    }
                    var encodeFileName = !browser.Contains("FIREFOX");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = encodeFileName ? HttpUtility.UrlEncode(_fileName, Encoding.UTF8) : _fileName
                    };
                }
                return httpResponseMessage;
            }
            catch
            {
                httpResponseMessage.Dispose();
                throw;
            }
        }
    }
}