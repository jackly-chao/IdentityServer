using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Quickstart.User
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await GetAllAsync();
            return View(vm);
        }

        public async Task<IActionResult> Create(int id)
        {
            ViewBag.Id = id;
            var vm = await GetByIdAsync(id);
            return View("CreateOrEdit", vm);
        }

        public async Task<UserViewModel> Details(int id)
        {
            return await GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<Response<string>> Save([FromBody] UserViewModel viewModel)
        {
            var result = new Response<string>();
            if (viewModel.Id > 0)
            {
                result = await UpdateAsync(viewModel.Id, viewModel);
            }
            else
            {
                result = await AddAsync(viewModel);
            }
            return result;
        }

        private async Task<List<UserViewModel>> GetAllAsync()
        {
            var models = new List<UserViewModel>();

            if (_userManager.Users.Any())
            {
                foreach (var resource in _userManager.Users)
                {
                    models.Add(new UserViewModel
                    {
                        Id = resource.Id,
                        UserName = resource.UserName,
                        Email = resource.Email,
                        PhoneNumber = resource.PhoneNumber,
                        NickName = resource.NickName,
                        LastName = resource.LastName,
                        FirstName = resource.FirstName,
                        Sex = resource.Sex,
                        Birth = resource.Birth,
                        Address = resource.Address,
                    });
                }
            }
            return models;
        }

        private async Task<UserViewModel> GetByIdAsync(int id)
        {
            var viewModel = new UserViewModel();
            var model = await _userManager.Users.FirstOrDefaultAsync(d => d.Id == id);
            if (model != null)
            {
                viewModel = new UserViewModel
                {
                    Id = model.Id,
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
            }
            return viewModel;
        }

        private async Task<Response<string>> AddAsync(UserViewModel viewModel)
        {
            //判断
            var identityResource = await _userManager.Users.FirstOrDefaultAsync(d => d.UserName == viewModel.UserName);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "用户名称已存在!") { Data = "用户名称已存在!" };
            }
            identityResource = new ApplicationUser
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(identityResource);
            if (!result.Succeeded)
            {
                return new Response<string>(success: false, msg: "注册失败,请重试!") { Data = "注册失败,请重试!" };
            }
            return new Response<string>(msg: "注册成功!") { Data = "注册成功!" };
        }

        private async Task<Response<string>> UpdateAsync(int id, UserViewModel viewModel)
        {
            //判断
            var identityResource = await _userManager.Users.FirstOrDefaultAsync(d => d.UserName == viewModel.UserName && d.Id != id);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "用户名称已存在!") { Data = "用户名称已存在!" };
            }
            identityResource = viewModel;
            var result = await _userManager.UpdateAsync(identityResource);

            if (!result.Succeeded)
            {
                return new Response<string>(success: false, msg: "修改失败,请重试!") { Data = "修改失败,请重试!" };
            }
            return new Response<string>(msg: "修改成功!") { Data = "修改成功!" };
        }
    }
}
