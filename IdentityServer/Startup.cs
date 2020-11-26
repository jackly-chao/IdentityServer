using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerAspNetIdentity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //添加控制器的服务
            services.AddControllersWithViews();

            #region ClientCredentials
            ////参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/1_ClientCredentials/src/IdentityServer/Startup.cs
            //var builder = services.AddIdentityServer()
            //    .AddInMemoryApiScopes(Config.GetApiScopes())
            //    .AddInMemoryClients(Config.GetClients());

            //builder.AddDeveloperSigningCredential();
            #endregion

            #region Code
            ////参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/2_InteractiveAspNetCore/src/IdentityServer/Startup.cs
            //var builder = services.AddIdentityServer()
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddInMemoryApiScopes(Config.GetApiScopes())
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddTestUsers(Config.GetUsers());

            //builder.AddDeveloperSigningCredential();
            #endregion

            #region 使用EF保存到数据库
            ////参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/5_EntityFramework/src/IdentityServer/Startup.cs
            //var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            //const string connectionString = @"Server=127.0.0.1;User ID=django;Password=django;Database=IdentityServer;";
            //services.AddIdentityServer()
            //    .AddTestUsers(Config.GetUsers())
            //    //AddConfigurationStore()需要添加IdentityServer4.EntityFramework包
            //    .AddConfigurationStore(options =>
            //    {
            //        //UseSqlServer()需要添加Microsoft.EntityFrameworkCore.SqlServer包
            //        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            //            sql => sql.MigrationsAssembly(migrationsAssembly));
            //    })
            //    .AddOperationalStore(options =>
            //    {
            //        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            //            sql => sql.MigrationsAssembly(migrationsAssembly));
            //    })
            //    .AddDeveloperSigningCredential();
            #endregion

            #region 使用AspNetIdentity
            ////参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/6_AspNetIdentity/src/IdentityServerAspNetIdentity/Startup.cs
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            //var builder = services.AddIdentityServer(options =>
            //{
            //    options.Events.RaiseErrorEvents = true;
            //    options.Events.RaiseInformationEvents = true;
            //    options.Events.RaiseFailureEvents = true;
            //    options.Events.RaiseSuccessEvents = true;

            //    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
            //    options.EmitStaticAudienceClaim = true;
            //})
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddInMemoryApiScopes(Config.GetApiScopes())
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddAspNetIdentity<ApplicationUser>();

            //if (env.IsDevelopment())
            //{
            //    // not recommended for production - you need to store your key material somewhere secure
            //    builder.AddDeveloperSigningCredential();
            //}
            #endregion

            #region 最终版本
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var builder = services.AddIdentityServer()
                //AddConfigurationStore()需要添加IdentityServer4.EntityFramework包
                .AddConfigurationStore(options =>
                {
                    //UseSqlServer()需要添加Microsoft.EntityFrameworkCore.SqlServer包
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                //AddAspNetIdentity需要添加IdentityServer4.AspNetIdentity包
                .AddAspNetIdentity<ApplicationUser>()
                .AddSigningCredential(new X509Certificate2(Path.Combine(basePath,
                Configuration["Certificates:Path"]),
                Configuration["Certificates:Password"]));
            //.AddDeveloperSigningCredential();//测试环境可以不用证书
            //.AddSigningCredential(Certificate.Certificate.Get())//生产环境考虑使用证书

            //if (Environment.IsDevelopment())
            //{
            //    builder.AddDeveloperSigningCredential();
            //}
            //builder.AddSigningCredential(new X509Certificate2("C:\\Users\\Administrator\\Desktop\\IdentityServer\\IdentityServer\\bin\\Debug\\netcoreapp3.1\\Certificate\\idsrv4.pfx", "jc335191"));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //初始化数据库
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //启用静态文件服务
            app.UseStaticFiles();

            app.UseRouting();

            //添加认证服务（UseIdentityServer()需要添加IdentityServer4包）
            app.UseIdentityServer();

            //添加授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        /// <summary>
        /// 初始化数据库
        /// 参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/5_EntityFramework/src/IdentityServer/Startup.cs
        /// </summary>
        /// <param name="app"></param>
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //初始化数据库结构
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                
                //初始化配置
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    context.Database.Migrate();
                    if (!context.Clients.Any())
                    {
                        foreach (var client in Config.GetClients())
                        {
                            context.Clients.Add(client.ToEntity());
                        }
                        context.SaveChanges();
                    }

                    if (!context.IdentityResources.Any())
                    {
                        foreach (var resource in Config.GetIdentityResources())
                        {
                            context.IdentityResources.Add(resource.ToEntity());
                        }
                        context.SaveChanges();
                    }

                    if (!context.ApiScopes.Any())
                    {
                        foreach (var resource in Config.GetApiScopes().ToList())
                        {
                            context.ApiScopes.Add(resource.ToEntity());
                        }
                        context.SaveChanges();
                    }

                    if (!context.ApiResources.Any())
                    {
                        foreach (var resource in Config.GetApiResources().ToList())
                        {
                            context.ApiResources.Add(resource.ToEntity());
                        }
                        context.SaveChanges();
                    }
                }

                //初始化用户,参考：https://github.com/IdentityServer/IdentityServer4/blob/2b5d9b634aa5195e3f7867129836691c5b68bd7d/samples/Quickstarts/6_AspNetIdentity/src/IdentityServerAspNetIdentity/SeedData.cs
                {
                    var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();
                    var userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    if (!userMgr.Users.Any())
                    {
                        foreach (var user in Config.GetUsers().ToList())
                        {
                            var newUser = new ApplicationUser
                            {
                                UserName = user.Username,
                            };

                            var result = userMgr.CreateAsync(newUser, user.Password).Result;
                            if (result.Succeeded)
                            {
                                result = userMgr.AddClaimsAsync(newUser, new Claim[]{
                                    new Claim(JwtClaimTypes.Name, newUser.UserName),
                                    new Claim(JwtClaimTypes.GivenName, newUser.UserName),
                                    new Claim(JwtClaimTypes.FamilyName, newUser.UserName),
                                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                                }).Result;
                            }
                        }
                    }
                }
            }
        }
    }
}
