using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TemporalOrdersController : GenericController<TemporalOrder>
    {
        private readonly ITemporalOrdersUnitOfWork _temporalOrdersUnitOfWork;

        public TemporalOrdersController(IGenericUnitOfWork<TemporalOrder> unitOfWork, ITemporalOrdersUnitOfWork temporalOrdersUnitOfWork) : base(unitOfWork)
        {
            _temporalOrdersUnitOfWork=temporalOrdersUnitOfWork;
        }

        [HttpPost("full")]
        public async Task<IActionResult> PostAsync(TemporalOrderDTO temporalOrderDTO)
        {
            var email = User.Identity!.Name!;
            var result = await _temporalOrdersUnitOfWork.AddFullAsync(email, temporalOrderDTO);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("my")]
        public async override Task<IActionResult> GetAsync()
        {
            var email = User.Identity!.Name!;
            var result = await _temporalOrdersUnitOfWork.GetAsync(email);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCountAsync()
        {
            var email = User.Identity!.Name!;
            var result = await _temporalOrdersUnitOfWork.GetCountAsync(email);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.Message);
        }
    }
}
