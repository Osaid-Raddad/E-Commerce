namespace E_Commerce.PL.Extensions
{
    public static class AddCorsPolicy
    {
        public const string PolicyName = "_myAllowSpecificOrigins";
        public static IServiceCollection AddCorsPolicyServices(this IServiceCollection Services)
        {
            Services.AddCors(options =>
            {
                options.AddPolicy(name: PolicyName,
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });

            return Services;
        }
    }
}
