using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EasyBlog.SharedLibrary.HttpClients
{
    public class AuthHttpMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHttpMessageHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
