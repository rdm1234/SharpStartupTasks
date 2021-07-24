using Examples.AspNetCoreWebApi.Services;
using Examples.SeparatedStartupTasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;

namespace Examples.AspNetCoreWebApi.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        private readonly MainStartupDependentService _mainDependentService;
        private readonly SomeStartupDependentService _someDependentService;

        public TestController(
            MainStartupDependentService mainDependentService,
            SomeStartupDependentService otherDependentService)
        {
            this._mainDependentService = mainDependentService;
            this._someDependentService = otherDependentService;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            var result =
                AllBoolPropertiesEqualsTrue(typeof(MainStartupDependentService), _mainDependentService)
                && AllBoolPropertiesEqualsTrue(typeof(SomeStartupDependentService), _someDependentService);

            return Ok(result);
        }

        private bool AllBoolPropertiesEqualsTrue(Type type, object instance)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.PropertyType == typeof(bool))
                .Select(p => (bool)p.GetValue(instance))
                .All(p => p == true);
        }
    }
}
