---
layout: page
title: Dependency Injection
---

# What is Dependency Injection

Dependency injection (DI) is a software design pattern that implements inversion of control for resolving dependencies.

Orleans is using the abstraction written by the developers of [ASP.NET Core](https://docs.asp.net). For a detailed explanation about how does it work, check out the [official documentation](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html#dependency-injection).

# DI in Orleans

Dependency Injection is currently supported only on the server side within Orleans.

Orleans makes it possible to inject dependencies into application [Grains](../Getting-Started-With-Orleans/Grains.md).

However Orleans supports every container dependent injection mechanisms, one of the most commonly used method is constructor injection.

Theoretically any type can be injected which was previously registered in a [`IServiceCollection`](https://docs.asp.net/projects/api/en/latest/autoapi/Microsoft/Extensions/DependencyInjection/IServiceCollection/index.html) during Silo startup.

Orleans also makes it possible to inject the following service into an application object instance:
- `IGrainFactory`

**Note**:
As Orleans is evolving, as of the current plans it will be possible to leverage dependency injection in other application classes as well, like [`StreamProviders`](../Orleans-Streams/Stream-Providers.md). 

# Configuring DI

The DI configuration is a global configuration value and must be configured there.

Orleans is using a similar approach as ASP.NET Core to configure DI. You must have a `Startup` class within your application which must contain a `ConfigureServices` method. It must return an object instance of type: `IServiceProvider`.

Configuration is done by specifying the type of your `Startup` class via one of the methods described below.

**Note**:
Previously DI configuration was specified at the cluster node level, this was changed in the recent release. 

## Configuring from Code

It is possible to tell Orleans what `Startup` type you like to use with code based configuration. There is an extension method named `UseStartup` on the `ClusterConfiguration` class which you can use to do that.

``` csharp
var configuration = new ClusterConfiguration();

configuration.UseStartup<MyStartup>();
``` 

## Configuring via XML

To register your `Startup` class with Orleans you've to add a `Startup` element to the `Globals` section and in the `Type` attribute you've to specify the assembly qualified name for the type.

``` XML
<?xml version="1.0" encoding="utf-8" ?>
<tns:OrleansConfiguration xmlns:tns="urn:orleans">
  <tns:Globals>
    <tns:Startup Type="MyApplication.Configuration.Startup,MyApplication" />
  </tns:Globals>
</tns:OrleansConfiguration>
```
# Example

Here is a complete example `Startup` class:

``` csharp
public class Startup
{
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IInjectedService, InjectedService>();

        return services.BuildServiceProvider();
    }
}
```
