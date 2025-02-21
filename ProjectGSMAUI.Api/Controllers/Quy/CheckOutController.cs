using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using AutoMapper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Data.Entities;
using Azure.Core;

namespace ProjectGSMAUI.Api.Controllers.Quy
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : Controller
    {
        private readonly ICheckOutServices service;
        private readonly IMapper mapper;
        public CheckOutController(ICheckOutServices service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }
        [HttpPost("CreateDatVe")]
        public async Task<IActionResult> CreateDatVe(List<DatVeCreator> datVe)
        {
            Console.WriteLine(datVe);
            var data = await this.service.CreateDatVe(datVe);
            return Ok(data);
        }
    }
}
