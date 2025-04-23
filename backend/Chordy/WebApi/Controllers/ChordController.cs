using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/chords")]
    public class ChordController(IChordService chordService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateChordAsync([FromBody] ChordCreateDto chordCreateDto, CancellationToken cancellationToken)
        {
            var chord = await chordService.CreateAsync(chordCreateDto, cancellationToken);
            return CreatedAtAction("GetChordById", new { id = chord.Id }, chord);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetChordByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var chord = await chordService.GetByIdAsync(id, cancellationToken);
            return Ok(chord);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChordsAsync(CancellationToken cancellationToken)
        {
            var chords = await chordService.GetAllAsync(cancellationToken);
            return Ok(chords);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateChordAsync([FromRoute] int id, [FromBody] ChordCreateDto chordCreateDto, CancellationToken cancellationToken)
        {
            await chordService.UpdateAsync(id, chordCreateDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCollectionAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await chordService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
