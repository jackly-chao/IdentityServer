using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.Quickstart.ApiResource
{
    public class ApiResourceController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public ApiResourceController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        #region MVCs
        // GET: ApiResourceController
        public async Task<IActionResult> Index()
        {
            var vm = await GetAllAsync();
            return View(vm);
        }

        // GET: ApiResourceController/Details/5
        public async Task<ApiResourceViewModel> Details(int id)
        {
            return await GetByIdAsync(id);
        }

        // GET: ApiResourceController/Create
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.Id = id;
            var vm = await GetByIdAsync(id);
            return View("CreateOrEdit", vm);
        }

        [HttpPost]
        public async Task<Response<string>> Save([FromBody] ApiResourceViewModel viewModel)
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

        // POST: ApiResourceController/Create
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

        // GET: ApiResourceController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Id = id;
            var vm = await GetByIdAsync(id);
            return View("CreateOrEdit", vm);
        }

        // POST: ApiResourceController/Edit/5
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

        // GET: ApiResourceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ApiResourceController/Delete/5
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
        private async Task<List<ApiResourceViewModel>> GetAllAsync()
        {
            var viewModels = new List<ApiResourceViewModel>();
            foreach (var resource in _configurationDbContext.ApiResources)
            {
                viewModels.Add(new ApiResourceViewModel
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                });
            }
            return viewModels;
        }

        private async Task<ApiResourceViewModel> GetByIdAsync(int id)
        {
            var viewModel = new ApiResourceViewModel();
            var model = await _configurationDbContext.ApiResources.FirstOrDefaultAsync(d => d.Id == id);
            if (model != null)
            {
                viewModel = new ApiResourceViewModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    Description = model.Description,
                };
            }
            return viewModel;
        }

        private async Task<Response<string>> AddAsync(ApiResourceViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.ApiResources.FirstOrDefaultAsync(d => d.Name == viewModel.Name);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "Api资源名称已存在!") { Data = "Api资源名称已存在!" };
            }
            identityResource = new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                Name = viewModel.Name,
                DisplayName = viewModel.DisplayName,
                Description = viewModel.Description,
            };
            await _configurationDbContext.ApiResources.AddAsync(identityResource);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result < 1)
            {
                return new Response<string>(success: false, msg: "注册失败,请重试!") { Data = "注册失败,请重试!" };
            }
            return new Response<string>(msg: "注册成功!") { Data = "注册成功!" };
        }

        private async Task<Response<string>> UpdateAsync(int id, ApiResourceViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.ApiResources.FirstOrDefaultAsync(d => d.Name == viewModel.Name && d.Id != id);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "Api资源名称已存在!") { Data = "Api资源名称已存在!" };
            }
            identityResource = viewModel;
            _configurationDbContext.ApiResources.Update(identityResource);
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
