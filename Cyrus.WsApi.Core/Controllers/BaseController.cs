using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cyrus.WsApi.Core.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected ILog Logger { get; }

        public BaseController()
        {
            Logger = LogManager.GetLogger(Startup.Repository.Name, this.GetType());
        }
    }
}
