using Autofac;
using DI.Ideas.Rehersal;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.ThreeLayers;
public interface IAreaRepository {}
public interface IBookRepository {}
public interface IBusinessRepository {}


public class AreaRepository : IAreaRepository
{


  public AreaRepository()
  {

  }
}

public class BookRepository : IBookRepository
{


  public BookRepository()
  {

  }
}

public class BusinessRepository : IBusinessRepository
{


  public BusinessRepository()
  {

  }
}
public interface IAreaService {}
public interface IBookService {}
public interface IBusinessService {}
public interface ICaseService {}


public class AreaService : IAreaService
{
  private readonly IAreaRepository _areaRepository;
  private readonly IBookRepository _bookRepository;
  private readonly IBusinessRepository _businessRepository;

  public AreaService(IAreaRepository areaRepository, IBookRepository bookRepository, IBusinessRepository businessRepository)
  {
    _areaRepository = areaRepository;
    _bookRepository = bookRepository;
    _businessRepository = businessRepository;
  }
}

public class BookService : IBookService
{
  private readonly IAreaRepository _areaRepository;
  private readonly IBusinessRepository _businessRepository;
  private readonly IBookRepository _bookRepository;

  public BookService(IAreaRepository areaRepository, IBusinessRepository businessRepository, IBookRepository bookRepository)
  {
    _areaRepository = areaRepository;
    _businessRepository = businessRepository;
    _bookRepository = bookRepository;
  }
}

public class BusinessService : IBusinessService
{
  private readonly IBusinessRepository _businessRepository;
  private readonly IBookRepository _bookRepository;
  private readonly IAreaRepository _areaRepository;

  public BusinessService(IBusinessRepository businessRepository, IBookRepository bookRepository, IAreaRepository areaRepository)
  {
    _businessRepository = businessRepository;
    _bookRepository = bookRepository;
    _areaRepository = areaRepository;
  }
}

public class CaseService : ICaseService
{
  private readonly IAreaRepository _areaRepository;
  private readonly IBookRepository _bookRepository;
  private readonly IBusinessRepository _businessRepository;

  public CaseService(IAreaRepository areaRepository, IBookRepository bookRepository, IBusinessRepository businessRepository)
  {
    _areaRepository = areaRepository;
    _bookRepository = bookRepository;
    _businessRepository = businessRepository;
  }
}
public interface IAreaController {}
public interface IBookController {}
public interface IBusinessController {}
public interface ICaseController {}
public interface IChildController {}


public class AreaController : IAreaController
{
  private readonly IBookService _bookService;
  private readonly IBusinessService _businessService;
  private readonly IAreaService _areaService;

  public AreaController(IBookService bookService, IBusinessService businessService, IAreaService areaService)
  {
    _bookService = bookService;
    _businessService = businessService;
    _areaService = areaService;
  }
}

public class BookController : IBookController
{
  private readonly IAreaService _areaService;
  private readonly ICaseService _caseService;
  private readonly IBusinessService _businessService;

  public BookController(IAreaService areaService, ICaseService caseService, IBusinessService businessService)
  {
    _areaService = areaService;
    _caseService = caseService;
    _businessService = businessService;
  }
}

public class BusinessController : IBusinessController
{
  private readonly IBookService _bookService;
  private readonly ICaseService _caseService;
  private readonly IBusinessService _businessService;

  public BusinessController(IBookService bookService, ICaseService caseService, IBusinessService businessService)
  {
    _bookService = bookService;
    _caseService = caseService;
    _businessService = businessService;
  }
}

public class CaseController : ICaseController
{
  private readonly ICaseService _caseService;
  private readonly IAreaService _areaService;
  private readonly IBookService _bookService;

  public CaseController(ICaseService caseService, IAreaService areaService, IBookService bookService)
  {
    _caseService = caseService;
    _areaService = areaService;
    _bookService = bookService;
  }
}

public class ChildController : IChildController
{
  private readonly ICaseService _caseService;
  private readonly IBookService _bookService;
  private readonly IAreaService _areaService;
  private readonly IBusinessService _businessService;

  public ChildController(ICaseService caseService, IBookService bookService, IAreaService areaService, IBusinessService businessService)
  {
    _caseService = caseService;
    _bookService = bookService;
    _areaService = areaService;
    _businessService = businessService;
  }
}

static class Registration
{
    public static void Register(IContainerBuilder container)
    {
container.RegisterTransient<IAreaRepository, AreaRepository>();
container.RegisterTransient<IBookRepository, BookRepository>();
container.RegisterTransient<IBusinessRepository, BusinessRepository>();
container.RegisterTransient<IAreaService, AreaService>();
container.RegisterTransient<IBookService, BookService>();
container.RegisterTransient<IBusinessService, BusinessService>();
container.RegisterTransient<ICaseService, CaseService>();
container.RegisterTransient<IAreaController, AreaController>();
container.RegisterTransient<IBookController, BookController>();
container.RegisterTransient<IBusinessController, BusinessController>();
container.RegisterTransient<ICaseController, CaseController>();
container.RegisterTransient<IChildController, ChildController>();
    }                           
}
static class AutofacRegistration
{
    public static void Register(Autofac.ContainerBuilder container)
    {
container.RegisterType<AreaRepository>().As<IAreaRepository>().InstancePerDependency();
container.RegisterType<BookRepository>().As<IBookRepository>().InstancePerDependency();
container.RegisterType<BusinessRepository>().As<IBusinessRepository>().InstancePerDependency();
container.RegisterType<AreaService>().As<IAreaService>().InstancePerDependency();
container.RegisterType<BookService>().As<IBookService>().InstancePerDependency();
container.RegisterType<BusinessService>().As<IBusinessService>().InstancePerDependency();
container.RegisterType<CaseService>().As<ICaseService>().InstancePerDependency();
container.RegisterType<AreaController>().As<IAreaController>().InstancePerDependency();
container.RegisterType<BookController>().As<IBookController>().InstancePerDependency();
container.RegisterType<BusinessController>().As<IBusinessController>().InstancePerDependency();
container.RegisterType<CaseController>().As<ICaseController>().InstancePerDependency();
container.RegisterType<ChildController>().As<IChildController>().InstancePerDependency();
    }                           
}
static class MSDIRegistration
{
    public static void Register(ServiceCollection container)
    {
container.AddTransient<IAreaRepository, AreaRepository>();
container.AddTransient<IBookRepository, BookRepository>();
container.AddTransient<IBusinessRepository, BusinessRepository>();
container.AddTransient<IAreaService, AreaService>();
container.AddTransient<IBookService, BookService>();
container.AddTransient<IBusinessService, BusinessService>();
container.AddTransient<ICaseService, CaseService>();
container.AddTransient<IAreaController, AreaController>();
container.AddTransient<IBookController, BookController>();
container.AddTransient<IBusinessController, BusinessController>();
container.AddTransient<ICaseController, CaseController>();
container.AddTransient<IChildController, ChildController>();
    }                           
}
