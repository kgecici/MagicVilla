using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepostiory;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MagicVilla_VillaAPI.Controllers;

//[Route("api/[controller]")]
[Route("api/villas")]
[ApiController]
public class VillaApiController : ControllerBase
{
    private readonly IVillaRepository _dbVilla;
    private readonly ILogger<VillaApiController> _logger;
    private readonly IMapper _mapper;

    public VillaApiController(ILogger<VillaApiController> logger, IVillaRepository dbVilla
        , IMapper mapper)
    {
        _logger = logger;
        _dbVilla = dbVilla;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        _logger.LogInformation("Getting all villas");
        IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
        return Ok(_mapper.Map<List<VillaDTO>>(villaList));
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = await _dbVilla.GetAsync(v => v.Id == id);
        if (villa == null) return NotFound();

        return Ok(_mapper.Map<Villa>(villa));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createVillaDTO)
    {
        if (await _dbVilla.GetAsync(v => v.Name.ToLower() == createVillaDTO.Name.ToLower()) != null)
        {
            ModelState.AddModelError("VILLA-0001", "Villa name already exists");
            return BadRequest(ModelState);
        }

        var villa = _mapper.Map<Villa>(createVillaDTO);

        await _dbVilla.CreateAsync(villa);

        return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
    }


    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villaDto = await _dbVilla.GetAsync(v => v.Id == id);
        if (villaDto == null)
            return NotFound();
        await _dbVilla.RemoveAsync(villaDto);
        return NoContent();
    }


    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaDTO updateVillaDTO)
    {
        if (id == 0 || id != updateVillaDTO.Id) return BadRequest();

        var villa = _mapper.Map<Villa>(updateVillaDTO);
        await _dbVilla.UpdateAsync(villa);
        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (id == 0) return BadRequest();

        var existingVilla = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
        if (existingVilla == null) return BadRequest();

        var villaUpdateDto = _mapper.Map<VillaUpdateDTO>(existingVilla);
        patchDTO.ApplyTo(villaUpdateDto, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var villa = _mapper.Map<Villa>(villaUpdateDto);
        await _dbVilla.UpdateAsync(villa);
        return NoContent();
    }
}