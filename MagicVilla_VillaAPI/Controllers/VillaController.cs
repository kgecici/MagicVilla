using System.Net;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepostiory;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

//[Route("api/[controller]")]
[Route("api/villas")]
[ApiController]
public class VillaController : ControllerBase
{
    private ApiResponse _response;
    private readonly IVillaRepository _dbVilla;
    private readonly ILogger<VillaController> _logger;
    private readonly IMapper _mapper;

    public VillaController(ILogger<VillaController> logger, IVillaRepository dbVilla
        , IMapper mapper)
    {
        _logger = logger;
        _dbVilla = dbVilla;
        _mapper = mapper;
        _response = new();
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetVillas()
    {
        _logger.LogInformation("Getting all villas");
        IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
        _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetVilla(int id)
    {
        try
        {
            if (id == 0) return BadRequest();

            var villa = await _dbVilla.GetAsync(v => v.Id == id);
            if (villa == null) return NotFound(); // TODO: villa null olamiyor hicbir zaman??

            _response.Result = _mapper.Map<Villa>(villa);
            _response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            // FIXME: Burada catch etmek yerine ortak bir noktada catch nasıl yapılır araştır
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }
        return Ok(_response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody] VillaCreateDTO createVillaDTO)
    {
        if (await _dbVilla.GetAsync(v => v.Name.ToLower() == createVillaDTO.Name.ToLower()) != null)
        {
            ModelState.AddModelError("VILLA-0001", "Villa name already exists");
            return BadRequest(ModelState);
        }

        var villa = _mapper.Map<Villa>(createVillaDTO);

        await _dbVilla.CreateAsync(villa);
        _response.Result = _mapper.Map<VillaDTO>(villa);
        _response.StatusCode = HttpStatusCode.Created;
        return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
    }


    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteVilla(int id)
    {
        if (id == 0)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { "Id cannot be null" };
;            return BadRequest(_response);
        }

        var villa = await _dbVilla.GetAsync(v => v.Id == id);
        if (villa == null)
            return NotFound();
        await _dbVilla.RemoveAsync(villa);
        _response.IsSuccess = true;
        _response.StatusCode = HttpStatusCode.NoContent;
        return Ok(_response);
    }


    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateVillaDTO)
    {
        if (id == 0 || id != updateVillaDTO.Id) return BadRequest();

        var villa = _mapper.Map<Villa>(updateVillaDTO);
        await _dbVilla.UpdateAsync(villa);
        _response.IsSuccess = true;
        _response.StatusCode = HttpStatusCode.NoContent;
        return Ok(_response);
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