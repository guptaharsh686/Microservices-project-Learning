using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
        {
            Console.WriteLine("---> Getting Platforms");

            var platformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItem));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDTO> GetPlatformById(int id)
        {
            var platformItem = _repository.GetPlatformById(id);

            if(platformItem != null) 
            { 
                return Ok(_mapper.Map<Platform, PlatformReadDTO>(platformItem));
            }

            return NotFound();
        }

    }
}
