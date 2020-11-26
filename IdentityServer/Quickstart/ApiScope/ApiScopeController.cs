using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Quickstart.ApiScope
{
    public class ApiScopeController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public ApiScopeController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        #region MVCs
        // GET: ApiScopeController
        public async Task<IActionResult> Index()
        {
            var vm = await GetAllAsync();
            return View(vm);
        }

        // GET: ApiScopeController/Details/5
        public async Task<ApiScopeViewModel> Details(int id)
        {
            return await GetByIdAsync(id);
        }

        // GET: ApiScopeController/Create
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.Id = id;
            var vm = await GetByIdAsync(id);
            return View("CreateOrEdit", vm);
        }

        [HttpPost]
        public async Task<Response<string>> Save([FromBody] ApiScopeViewModel viewModel)
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

        // POST: ApiScopeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ApiScopeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ApiScopeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ApiScopeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ApiScopeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Services
        private async Task<List<ApiScopeViewModel>> GetAllAsync()
        {
            var viewModels = new List<ApiScopeViewModel>();
            foreach (var resource in _configurationDbContext.ApiScopes)
            {
                viewModels.Add(new ApiScopeViewModel
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                });
            }
            return viewModels;
        }

        private async Task<ApiScopeViewModel> GetByIdAsync(int id)
        {
            var viewModel = new ApiScopeViewModel();
            var model = await _configurationDbContext.ApiScopes.FirstOrDefaultAsync(d => d.Id == id);
            if (model != null)
            {
                viewModel = new ApiScopeViewModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    Description = model.Description,
                };
            }
            return viewModel;
        }

        private async Task<Response<string>> AddAsync(ApiScopeViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.ApiScopes.FirstOrDefaultAsync(d => d.Name == viewModel.Name);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "认证资源名称已存在!") { Data = "认证资源名称已存在!" };
            }
            identityResource = new IdentityServer4.EntityFramework.Entities.ApiScope
            {
                Name = viewModel.Name,
                DisplayName = viewModel.DisplayName,
                Description = viewModel.Description,
            };
            await _configurationDbContext.ApiScopes.AddAsync(identityResource);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result < 1)
            {
                return new Response<string>(success: false, msg: "注册失败,请重试!") { Data = "注册失败,请重试!" };
            }
            return new Response<string>(msg: "注册成功!") { Data = "注册成功!" };
        }

        private async Task<Response<string>> UpdateAsync(int id, ApiScopeViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.ApiScopes.FirstOrDefaultAsync(d => d.Name == viewModel.Name && d.Id != id);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "认证资源名称已存在!") { Data = "认证资源名称已存在!" };
            }
            identityResource = viewModel;
            _configurationDbContext.ApiScopes.Update(identityResource);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result < 1)
            {
                return new Response<string>(success: false, msg: "修改失败,请重试!") { Data = "修改失败,请重试!" };
            }
            return new Response<string>(msg: "修改成功!") { Data = "修改成功!" };
        }
        #endregion
    }
}
