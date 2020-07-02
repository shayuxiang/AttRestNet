using AttRest;
using AttRest.CodeType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using viewmodel;

namespace test.Controllers
{
    [Area("AttTest")]
    [Route("[Area]/api/[controller]/[action]")]
    [ApiController]
    public class AttController : Controller
    {

        [HttpGet]
        [AttDescription("获取API基架")]
        public void GetSchema() 
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Response.ContentType = "text/html;";
            Response.WriteAsync(AttRestCore.Schema(), Encoding.GetEncoding("gbk"));
            Response.WriteAsync(AttRestCore.DefaultCss());
        }

        [HttpGet]
        [AttDescription("获取API基架")]
        public void GetVue()
        {
            AttRestCore.ResponseScript(HttpContext);
        }


        [HttpGet]
        [Route("Test/{id}")]
        [AttDescription("测试方法GetId")]
        public JsonResult GetId(int id)
        {
            return Json(id);
        }

        [HttpGet]
        [AttDescription("枚举参数测试")]
        public JsonResult GetEnum(EnumTest enumTest)
        {
            return Json(enumTest);
        }

        [HttpGet]
        [AttDescription("对象参数测试")]
        public JsonResult GetResult(Test test) {
            return Json(test);
        }

        [HttpPost]
        [AttDescription("对象参数测试")]
        public JsonResult GetResult2(Test test)
        {
            return Json(test);
        }
    }
}
