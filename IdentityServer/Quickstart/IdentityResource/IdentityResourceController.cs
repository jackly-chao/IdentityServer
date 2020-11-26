using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.Quickstart.IdentityResource
{
    public class IdentityResourceController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public IdentityResourceController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        #region MVCs
        // GET: IdentityResourceController
        public async Task<IActionResult> Index()
        {
            var vm = await GetAllAsync();
            return View(vm);
        }

        // GET: IdentityResourceController/Details/5
        public async Task<IdentityResourceViewModel> Details(int id)
        {
            return await GetByIdAsync(id);
        }

        // GET: IdentityResourceController/Create
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.Id = id;
            var vm = await GetByIdAsync(id);
            return View("CreateOrEdit", vm);
        }

        [HttpPost]
        public async Task<Response<string>> Save([FromBody] IdentityResourceViewModel viewModel)
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

        // POST: IdentityResourceController/Create
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

        // GET: IdentityResourceController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Id = id;
            var vm = await GetByIdAsync(id);
            return View("CreateOrEdit", vm);
        }

        // POST: IdentityResourceController/Edit/5
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

        // GET: IdentityResourceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IdentityResourceController/Delete/5
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

        #region APIs
        [HttpGet("{id}")]
        public async Task<IdentityResourceViewModel> Get(int id)
        {
            var viewModel = new IdentityResourceViewModel();
            var model = await _configurationDbContext.IdentityResources.FirstOrDefaultAsync(d => d.Id == id);
            if (model != null)
            {
                viewModel.Name = model.Name;
                viewModel.DisplayName = model.DisplayName;
                viewModel.Description = model.Description;
            }
            return viewModel;
        }

        [HttpPost]
        public async Task<Response<string>> Post([FromBody] IdentityResourceViewModel viewModel)
        {
            return await AddAsync(viewModel);
        }

        [HttpPut("{id}")]
        public async Task<Response<string>> Put(int id, [FromBody] IdentityResourceViewModel viewModel)
        {
            return await UpdateAsync(id, viewModel);
        }

        ///// <summary>
        ///// 查找数据列表
        ///// </summary>
        ///// <returns></returns>
        //// GET: api/<ValuesController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        ///// <summary>
        ///// 根据ID查看数据
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //// GET api/<ValuesController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        ///// <summary>
        ///// 新增数据
        ///// </summary>
        ///// <param name="value"></param>
        //// POST api/<ValuesController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        ///// <summary>
        ///// 根据ID修改数据
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="value"></param>
        //// PUT api/<ValuesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        ///// <summary>
        ///// 根据ID删除数据
        ///// </summary>
        ///// <param name="id"></param>
        //// DELETE api/<ValuesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
        #endregion

        #region Services
        private async Task<List<IdentityResourceViewModel>> GetAllAsync()
        {
            var viewModels = new List<IdentityResourceViewModel>();
            foreach (var resource in _configurationDbContext.IdentityResources)
            {
                var model = new IdentityResourceViewModel
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                };
                viewModels.Add(model);
            }
            return viewModels;
        }

        private async Task<IdentityResourceViewModel> GetByIdAsync(int id)
        {
            var viewModel = new IdentityResourceViewModel();
            var model = await _configurationDbContext.IdentityResources.FirstOrDefaultAsync(d => d.Id == id);
            if (model != null)
            {
                viewModel = new IdentityResourceViewModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    Description = model.Description,
                };
            }
            return viewModel;
        }

        private async Task<Response<string>> AddAsync(IdentityResourceViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.IdentityResources.FirstOrDefaultAsync(d => d.Name == viewModel.Name);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "认证资源名称已存在!") { Data = "认证资源名称已存在!" };
            }
            identityResource = new IdentityServer4.EntityFramework.Entities.IdentityResource
            {
                Name = viewModel.Name,
                DisplayName = viewModel.DisplayName,
                Description = viewModel.Description,
            };
            await _configurationDbContext.IdentityResources.AddAsync(identityResource);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result < 1)
            {
                return new Response<string>(success: false, msg: "注册失败,请重试!") { Data = "注册失败,请重试!" };
            }
            return new Response<string>(msg: "注册成功!") { Data = "注册成功!" };
        }

        private async Task<Response<string>> UpdateAsync(int id, IdentityResourceViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.IdentityResources.FirstOrDefaultAsync(d => d.Name == viewModel.Name && d.Id != id);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "认证资源名称已存在!") { Data = "认证资源名称已存在!" };
            }
            identityResource = viewModel;
            _configurationDbContext.IdentityResources.Update(identityResource);
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
