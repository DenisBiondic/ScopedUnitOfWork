# ScopedUnitOfWork

ScopedUnitOfWork is a lightweight .NET Standard implementation of commonly used Unit of Work and Repository patterns, extended with scoped functionality to improve read performance and in-built with transactions in respect to underlying Entity Framework Core ORM.

Three key features are:

 - very **simple** and ***extensible*** repository / uow implementation, which ***integrates*** with any **IoC container** out there
 - **scoped (ambient) contexts**
 - **scoped (ambient) transactions**

## Simple extensible repository / uow implementation

Altough there are about a milion other repository / uow implementations, I never found any that was simple enough, without the bloat of 100s of methods, that was IoC / DI friendly and that was easily extensible (essentially respecting the open / closed principle).

A pattern that worked for me for years is having a injectable *factory*, which creates units of work, which are then used to resolve the repositories. Something like:

```c#
using (IUnitOfWork unitOfWork = factory.Create())
{
    unitOfWork.GetRepository<ICustomerRepository>().Add(...);
    unitOfWork.Commit();
}
```

What I also needed is that the framework uses the underlying IoC container, for example when resolving repositories like in example above. This removes any need of having concrete repository properties on UnitOfWork class which is a common anti-pattern.

## Scoped contexts

Main reason for scoped / ambient contexts is usage of so called **L1 cache**. This is the in-built cache in the EFCore DbContext which is checked when retriving entities by the identifier (for example when using context.Find()). To take advantage of this caching you simply reuse the same database context for multiple operations. But, as soon as you start "sharing" these context you have to manage their lifecycles as well. There are many patterns to go about this issue, session-per-web-request or session-per-controller to just name a few. I would strongly recommend reading this excellent acricle for more info: http://mehdi.me/ambient-dbcontext-in-ef6/

How does scoped units of work look like? Like this:

```c#
using (IUnitOfWork parent = factory.Create())
{
    using (IUnitOfWork child = factory.Create())
    {
        ...
    }
}
```

Scoped / ambient functionality takes care that both parent and child have actually
the **same** DbContext instance.

### Scoped / ambient transactions

Very similarly to the scoped contexts, this library offers easy to use Transaction management. Here is an example:

```c#
using (IUnitOfWork unitOfWork = factory.Create(ScopeType.Transactional))
{
    // everything here, as well as any other unit of work are now transactional
    using (IUnitOfWork unitOfWork = factory.Create())
    {
        ...
    }

    // commits everything
    unitOfWork.Commit();
}
```

# Setup

TODO

<!--## Packages and configuration

Main *modules* (NuGet packages / assemblies) are:

  - ScopedUnitOfWork.Core - basic types IRepository and IUnitOfWork, which can be safely referenced
    from a domain assembly in let's say "onion" architecture (so that you maintain the *persistence ignorance* pattern).
  - ScopedUnitOfWork.**EF6** - Unit of Work and Repository implementations for Entity Framework 6.
  - ScopedUnitOfWork.**EF.Core** - Unit of Work and Repository implementations for Entity Framework Core.-->

<!--The framework is avaiable on NuGet for download.

Full Version | NuGet | NuGet Install
------------ | :-------------: | :-------------:
ScopedUnitOfWork.Interfaces | <a href="https://www.nuget.org/packages/ScopedUnitOfWork.Interfaces/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/CoffeeApplied.Core.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/CoffeeApplied.Core/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/CoffeeApplied.Core.svg?style=flat-square" /></a> | ```PM> Install-Package ScopedUnitOfWork.Interfaces```
ScopedUnitOfWork.EF6 | <a href="https://www.nuget.org/packages/ScopedUnitOfWork.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/CoffeeApplied.PersistenceFramework.EF6.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/CoffeeApplied.PersistenceFramework.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/CoffeeApplied.PersistenceFramework.EF6.svg?style=flat-square" /></a> | ```PM> Install-Package ScopedUnitOfWork.EF6```
ScopedUnitOfWork.EF.Core | <a href="https://www.nuget.org/packages/ScopedUnitOfWork.EF.Core/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/CoffeeApplied.PersistenceFramework.EF7.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/CoffeeApplied.PersistenceFramework.EF7/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/CoffeeApplied.PersistenceFramework.EF7.svg?style=flat-square" /></a> | ```PM> Install-Package ScopedUnitOfWork.EF.Core```-->
<!-- 

## Configuration and first use


1. The framework has IoC container usage in its blood, so you need to provide a bridge to whatever container you are using. Internally, the [CommonServiceLocator](https://www.nuget.org/packages/commonservicelocator/) is used, so it should be pretty easy to configure your IoC container of choice.

For example, with Autofac you can use the Autofac.Extras.CommonServiceLocator NuGet package. 

2. Register the IServiceLocator in container itself

``` c#
// to update the Autofac container, we need to create a new ContainerBuilder and update the container with new registrations
builder = new ContainerBuilder();

// The AutofacServiceLocator is wrapper from Autofac.Extras.CommonServiceLocator
builder.Register(x => new AutofacServiceLocator(container)).As<IServiceLocator>();

builder.Update(container);
```

The registration above is fine as long as you have your DbContexts registered as "transient" (new instance each time when requested).
However, in some cases you might have the DbContext registered per scope (like when using ASP.NET Core and Entity Framework Core).
In that case, make sure you use the inner most scope as container when plugging in the CommonServiceLocator:

``` c#
builder = new ContainerBuilder();

// note the dynamic resolution of ILifetimeScope
builder.Register(x => new AutofacServiceLocator(x.Resolve<ILifetimeScope>()))
    .As<IServiceLocator>();

builder.Update(container);
```

For other IoC containers, syntax would be different of course.

#####For a working project, please check *ScopedUnitOfWork.Sample* project which uses Autofac and Entity Framework 7.

After IoC integration is set up, rest should be pretty easy

 1. Your DbContext needs to be registered with your IoC container as **self**, since it will be internally resolved from the IoC container upon UnitOfWork creation.
    For example with Autofac:

    ```
    builder.RegisterType<SampleContext>().As<SampleContext>();
    ```

    Note: in some frameworks, like ASP.NET Core with EF Core, this will already be done with configuration code in Startup class.

 2. You need to register a factory with your concrete context. With Autofac you would do something like:
    
    ```
    builder.RegisterType<UnitOfWorkFactory<SampleContext>>().As<IUnitOfWorkFactory>();
    ```

    The context type here (SampleContext) is the exact concrete type registered as self in the step 1.
    Factory interface *IUnitOfWorkFactory* should be used for constructor injection on your objects
    where you want to use UoW.
 3. Register your repositories for their interfaces. For example

    ```
    builder.RegisterType<InvoiceRepository>().As<IInvoiceRepository>();
    ```

    This is required because you will get the repositories through uow.GetRepository<T>() and this
    internally will use your IoC container.


## Realistic real-world example

More complex real world usage with transactions (each unit of work represents different components which would be contained in different classes):

```c#
// our controller or some other top level service (maybe on application services layer)
// opens a spanning context - every uow underneath will have the same context
using (var spanningUnitOfWork = factory.Create())
{
    var repository = spanningUnitOfWork.GetRepository<ICustomerRepository>();
    Customer customer = repository.FindByName(firstCustomerName);

    // let's say this was a authorization check or something
    customer.Name.Should().Be(firstCustomerName);

    // now we want to call a business service which would have its own transactional unit of work in there
    using (var businessUnitOfWork = factory.Create(ScopeType.Transactional))
    {
        using (var firstDomainService = factory.Create())
        {
            firstDomainService.GetRepository<ICustomerRepository>()
                .Add(new Customer { Name = secondCustomerName });

            firstDomainService.Commit();
        }

        using (var secondDomainService = factory.Create())
        {
            secondDomainService.GetRepository<ICustomerRepository>()
                .Add(new Customer { Name = thirdCustomerName });

            secondDomainService.Commit();
        }

        // this should commit the transaction as its the top most transactional uow
        businessUnitOfWork.Commit();
    }
}
```

Also check the acceptance tests in the solution for actual code and examples. -->
