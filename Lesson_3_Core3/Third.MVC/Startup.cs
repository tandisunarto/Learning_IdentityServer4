using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static System.Console;

namespace Third.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddControllersWithViews();

            services.AddAuthentication(options => {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options => {
                options.Authority = "http://localhost:8000";
                options.RequireHttpsMetadata = false;
                options.ClientId = "mvc";
                options.ClientSecret = "mvc-secret";
                options.ResponseType = "code";
                options.UsePkce = true;

                options.SaveTokens = true;

                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("address");
                options.ClaimActions.MapJsonKey("address", "address");
                options.Scope.Add("security_info");
                options.ClaimActions.MapJsonKey("sec_zone", "sec_zone");
                options.ClaimActions.MapJsonKey("sec_exp", "sec_exp");

                options.Scope.Add("offline_access");

                options.Events = new OpenIdConnectEvents {
                    OnRemoteFailure = ctx => {
                        WriteLine(".....OnRemoteFailure");
                        WriteLine(ctx.Failure.Message);

                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    },
                    OnRedirectToIdentityProvider = ctx => {
                        // only modify requests to the authorization endpoint
                        if (ctx.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                        {
                            // generate code_verifier
                            var codeVerifier = CryptoRandom.CreateUniqueId(32);
                        
                            // store codeVerifier for later use
                            if (ctx.Properties.Items.ContainsKey("code_verifier"))
                                ctx.Properties.Items["code_verifier"] = codeVerifier;
                            else
                                ctx.Properties.Items.Add("code_verifier", codeVerifier);
                        
                            // create code_challenge
                            string codeChallenge;
                            using (var sha256 = SHA256.Create())
                            {
                                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                                codeChallenge = Base64Url.Encode(challengeBytes);
                            }
                        
                            // add code_challenge and code_challenge_method to request
                            if (ctx.ProtocolMessage.Parameters.ContainsKey("code_challenge")) 
                            {
                                ctx.ProtocolMessage.Parameters["code_challenge"] = codeChallenge;
                                ctx.ProtocolMessage.Parameters["code_challenge_method"] = "S256";
                            }
                            else 
                            {
                                ctx.ProtocolMessage.Parameters.Add("code_challenge", codeChallenge);
                                ctx.ProtocolMessage.Parameters.Add("code_challenge_method", "S256");
                            }
                        }
                        
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = ctx => {
                        // only when authorization code is being swapped for tokens
                        if (ctx.TokenEndpointRequest?.GrantType == OpenIdConnectGrantTypes.AuthorizationCode)
                        {
                            // get stored code_verifier
                            if (ctx.Properties.Items.TryGetValue("code_verifier", out var codeVerifier))
                            {
                                // add code_verifier to token request
                                ctx.TokenEndpointRequest.Parameters.Add("code_verifier", codeVerifier);
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
