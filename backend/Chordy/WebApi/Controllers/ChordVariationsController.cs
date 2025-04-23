using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/chord-variations")]
    public class ChordVariationController(IChordVariationService chordVariationService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] ChordVariationCreateDto dto, CancellationToken cancellationToken)
        {
            var chordVariation = await chordVariationService.AddAsync(dto, cancellationToken);
            return CreatedAtAction("GetById", new { id = chordVariation.Id }, chordVariation);
        }

        [HttpGet("by-chord/{chordId:int}")]
        public async Task<IActionResult> GetByChordIdAsync([FromRoute] int chordId, CancellationToken cancellationToken)
        {
            var variations = await chordVariationService.GetByChordIdAsync(chordId, cancellationToken);
            return Ok(variations);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var variation = await chordVariationService.GetByIdAsync(id, cancellationToken);
            return Ok(variation);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] ChordVariationCreateDto dto, CancellationToken cancellationToken)
        {
            await chordVariationService.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await chordVariationService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}