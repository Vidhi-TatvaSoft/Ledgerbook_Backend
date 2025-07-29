using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

public class ViewRenderService : IViewRenderService
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public ViewRenderService(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderToStringAsync(string viewName, object model)
    {
        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };

        ActionContext actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor()
        );
        ViewEngineResult viewResult = _viewEngine.GetView(null, viewName, false);

        if (!viewResult.Success)
        {
            viewResult = _viewEngine.FindView(actionContext, viewName, false);
        }

        if (viewResult.View == null)
        {
            throw new ArgumentNullException($"{viewName} does not match any available view");
        }

        using StringWriter sw = new StringWriter();

        ViewDataDictionary viewDictionary = new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
        {
            Model = model
        };

        TempDataDictionary tempData = new TempDataDictionary(httpContext, _tempDataProvider);

        ViewContext viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            tempData,
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}