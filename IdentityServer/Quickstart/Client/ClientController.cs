using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Quickstart.Client
{
    public class ClientController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public ClientController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        #region MVCs
        // GET: ClientController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await GetAllAsync();
            return View(vm);
        }

        // GET: ClientController/Details/5
        public async Task<ClientViewModel> Details(int id)
        {
            return await GetByIdAsync(id);
        }

        // GET: ClientController/Create
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.Id = id;
            var vm = await GetByIdAsync(id);
            return View("CreateOrEdit", vm);
        }

        [HttpPost]
        public async Task<Response<string>> Save([FromBody] ClientViewModel viewModel)
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

        // POST: ClientController/Create
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

        // GET: ClientController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ClientController/Edit/5
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

        // GET: ClientController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClientController/Delete/5
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
        private async Task<List<ClientViewModel>> GetAllAsync()
        {
            var models = new List<ClientViewModel>();

            if (_configurationDbContext.Clients.Any())
            {
                foreach (var resource in _configurationDbContext.Clients)
                {
                    models.Add(new ClientViewModel
                    {
                        Id = resource.Id,
                        ClientId = resource.ClientId,
                        ClientName = resource.ClientName,
                        Description = resource.Description,
                    });
                }
            }
            return models;
        }

        private async Task<ClientViewModel> GetByIdAsync(int id)
        {
            var viewModel = new ClientViewModel();
            var model = await _configurationDbContext.Clients.FirstOrDefaultAsync(d => d.Id == id);
            if (model != null)
            {
                viewModel = new ClientViewModel
                {
                    Id = model.Id,
                    ClientId = model.ClientId,
                    ClientName = model.ClientName,
                    Description = model.Description,
                };
            }
            return viewModel;
        }

        private async Task<Response<string>> AddAsync(ClientViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.Clients.FirstOrDefaultAsync(d => d.ClientId == viewModel.ClientId);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "Api域名称已存在!") { Data = "Api域名称已存在!" };
            }
            identityResource = new IdentityServer4.EntityFramework.Entities.Client
            {
                ClientId = viewModel.ClientId,
                ClientName = viewModel.ClientName,
                Description = viewModel.Description,
            };
            await _configurationDbContext.Clients.AddAsync(identityResource);
            var result = await _configurationDbContext.SaveChangesAsync();
            if (result < 1)
            {
                return new Response<string>(success: false, msg: "注册失败,请重试!") { Data = "注册失败,请重试!" };
            }
            return new Response<string>(msg: "注册成功!") { Data = "注册成功!" };
        }

        private async Task<Response<string>> UpdateAsync(int id, ClientViewModel viewModel)
        {
            //判断
            var identityResource = await _configurationDbContext.Clients.FirstOrDefaultAsync(d => d.ClientId == viewModel.ClientId && d.Id != id);
            if (identityResource != null)
            {
                return new Response<string>(success: false, msg: "Api域名称已存在!") { Data = "Api域名称已存在!" };
            }
            identityResource = viewModel;
            _configurationDbContext.Clients.Update(identityResource);
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
