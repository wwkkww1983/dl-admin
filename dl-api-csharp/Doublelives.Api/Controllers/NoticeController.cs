﻿using Doublelives.Api.MockResponse;
using Doublelives.Api.Models.Notice;
using Doublelives.Service.WorkContextAccess;
using Microsoft.AspNetCore.Mvc;

namespace Doublelives.Api.Controllers
{
    public class NoticeController : AuthControllerBase
    {
        public NoticeController(IWorkContextAccessor workContextAccessor)
            : base(workContextAccessor)
        {
        }

        [HttpGet("list")]
        public ActionResult<NoticeViewModel> List()
        {
            var model = MockResponseHelper.GetMockModel<NoticeViewModel>("noticelist");

            return Ok(model);
        }
    }
}
