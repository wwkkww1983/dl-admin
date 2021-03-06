﻿using AutoMapper;
using Doublelives.Api.Mappers;
using Doublelives.Api.Models.Account;
using Doublelives.Api.Models.Users;
using Doublelives.Api.Models.Users.Requests;
using Doublelives.Service.Users;
using Doublelives.Service.WorkContextAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Doublelives.Api.Controllers
{
    public class UserController : AuthControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(
            IWorkContextAccessor workContextAccessor,
            IUserService userService,
            IMapper mapper)
            : base(workContextAccessor)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取分页用户列表
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] UserListSearchRequest request)
        {
            var result = await _userService.GetPagedList(UserMapper.ToUserSearchDto(request));

            var viewModel = new UserPagedListViewModel
            {
                Current = result.PageNumber,
                Pages = result.PageCount,
                Size = result.PageSize,
                Sort = result.Sort,
                Total = result.TotalCount,
                Records = _mapper.Map<List<AccountProfileViewModel>>(result.Data)
            };

            return Ok(viewModel);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ModifyUser(UserUpdateRequest request)
        {
            // todo model validate
            await _userService.Update(UserMapper.ToUserUpdateDto(request, WorkContext.CurrentUser));

            return Ok("1");
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        [HttpPost]
        public IActionResult Add(UserUpdateRequest request)
        {
            return Ok("1");
        }
    }
}