using codegeneratorlib.Helpers;
using EventMonitoring.ph.Data.Entities;
using EventMonitoring.ph.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventMonitoring.ph.Middleware
{
    public static class WEbUIServiceContainer
    {
        public static IServiceCollection WebUIAddInfrastractureService(this IServiceCollection services,
                                                                       IConfiguration configuration)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddDbContext<AppDB2Context>(o =>
            //            o.UseSqlServer(configuration.GetConnectionString("Default"), b=),
            //ServiceLifetime.Scoped);

            services.AddDbContext<db_ab9d6a_dbrenzContext>(o => o.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultScheme = IdentityConstants.ExternalScheme;
            });

            //services.AddIdentity<User, Role>(cfg =>
            //{
            //    cfg.User.RequireUniqueEmail = true;
            //    cfg.Password.RequiredLength = 6;
            //    cfg.Password.RequireNonAlphanumeric = false;
            //    cfg.Password.RequireLowercase = false;
            //    cfg.Password.RequireUppercase = false;
            //    cfg.Password.RequireDigit = false;

            //}).AddEntityFrameworkStores<db_ab9d6a_dbrenzContext>()
            //  .AddSignInManager()
            //  .AddDefaultTokenProviders();

            //services.ConfigureApplicationCookie(opts =>
            //{
            //    opts.LoginPath = "/Account/Login";
            //    opts.AccessDeniedPath = "/Account/AccessDenied";
            //});

            //services.AddAuthentication()
            //     .AddCookie()
            //     .AddJwtBearer(cfg =>
            //     {
            //         cfg.TokenValidationParameters = new TokenValidationParameters()
            //         {
            //             ValidIssuer = _config["Tokens:Issuer"],
            //             ValidAudience = _config["Tokens:Audience"],
            //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
            //         };
            //     });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(30);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddAuthorizationBuilder()
                    .AddPolicy("AdministrationPolicy", adp =>
                    {
                        adp.RequireAuthenticatedUser();
                        adp.RequireRole("Admin", "Manager");
                    }).AddPolicy("UserPolicy", adp =>
                    {
                        adp.RequireAuthenticatedUser();
                        adp.RequireRole("User");
                    });

            services.AddCascadingAuthenticationState();

            //services.AddScoped<Data.IDbContextFactory<AppDB2Context>, DbContextFactory<AppDB2Context>>();
            services.AddTransient<AppHelper>();

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            //services.AddTransient<seeder>();
            //services.AddTransient<AccessRoleHelper>();
            //services.AddTransient<AuthServices>();
            //services.AddScoped<IAccount, Account>();
            //services.AddScoped<IFtpManager, FtpManager>();
            //services.AddScoped<IMenuManager, MenuManager>();
            //services.AddScoped<IEmailManager, EmailManager>();
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreatePulloutHandler).Assembly));
            return services;
        }
    }
}