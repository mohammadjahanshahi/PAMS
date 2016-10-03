using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSIActivityManagement.Extensions;

namespace BSIActivityManagement.Authorization
{
    public class AMAuthorizationAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        // Custom property
        public string AccessKey { get; set; }
        private Logic.DML DmlObj = new Logic.DML();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
            if (DmlObj.CheckUserAccessKeyByUserId(AccessKey, httpContext.User.GetAmUser())) return true;
            return false;               
        }
    }


}