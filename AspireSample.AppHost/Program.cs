using System.Diagnostics;
using AspireSample.AppHost.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var apiservicedb=  builder.AddSqlServer("sqlserver").AddDatabase("apiservicedb");

var apiService = builder.AddProject<AspireSample_ApiService>("apiservice")
    // db
    .WithReference(apiservicedb)
    .WaitFor(apiservicedb)
    //open api
    .WithSwaggerUi()
    .WithRedoc()
    .WithScalar();




builder.AddProject<AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();