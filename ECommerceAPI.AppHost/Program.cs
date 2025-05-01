var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ECommerceAPI>("ecommerceapi");

builder.Build().Run();
