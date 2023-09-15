﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        [HttpGet]
        public UserModel GetById()
        {
            // gets userId from the user who is loged in
            // passing userId by method is security issue
            string userId = RequestContext.Principal.Identity.GetUserId();

            UserData data = new UserData();

            return data.GetUserById(userId).First();
        }
    }
}