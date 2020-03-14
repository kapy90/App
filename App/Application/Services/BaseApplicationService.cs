using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace App.Application.Services
{
    public abstract class BaseApplicationService : IApplicationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BaseApplicationService()
        {
            _httpContextAccessor = Infrastructure.EngineContext.Current.Resolve<IHttpContextAccessor>();
        }

        protected ClaimsPrincipal User
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null)
                {
                    var claimsIdentity = new ClaimsIdentity();
                    user = new ClaimsPrincipal(claimsIdentity);
                }
                return user;
            }
        }

    }
}
