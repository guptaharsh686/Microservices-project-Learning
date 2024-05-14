using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDTO>> GetAllCommandsForPlatform([FromRoute] int platformId)
        {
            Console.WriteLine($"--> Hit the GetAllCommandsForPlatform {platformId}");

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            var commandItems = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commandItems));
        }


        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDTO> GetCommandForPlatform([FromRoute] int platformID, [FromRoute] int commandId)
        {
            Console.WriteLine($"--> GetCommandForPlatform platformId = {platformID}, commandId = {commandId}");
            if(!_repository.PlatformExists(platformID))
            {
                return NotFound();
            }

            var command = _repository.GetCommand(platformID, commandId);

            if(command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDTO>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDTO> CreateCommandForPlatform([FromRoute] int platformID,CommandCreateDTO commandDTO)
        {
            Console.WriteLine($"--> Creating New command for a platform = {platformID}");
            if(!_repository.PlatformExists(platformID))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandDTO);

            _repository.CreateCommand(platformID, command);
            _repository.SaveChanges();

            var commandReadDTO = _mapper.Map<CommandReadDTO>(command);

            return CreatedAtRoute(
                nameof(GetCommandForPlatform),
                new { PlatformID = platformID, commandID = commandReadDTO.Id, commandReadDTO });
        }
    }
}
