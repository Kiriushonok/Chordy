﻿using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Services;
using Chordy.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/chord-variations")]
    public class ChordVariationController(IChordVariationService chordVariationService, IAuthorizationService authorizationService, ChordyDbContext context) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] ChordVariationCreateDto dto, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var chordVariation = await chordVariationService.AddAsync(dto, userId, cancellationToken);
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

        [HttpGet("by-song/{songId}")]
        public async Task<IActionResult> GetChordVariationsForSong(int songId, CancellationToken cancellationToken)
        {
            // 1. Получить закреплённые вариации для этой песни (DefaultChords)
            var defaultVariations = await context.defaultChords
                .Where(dc => dc.SongId == songId)
                .Select(dc => dc.ChordVariation)
                .ToListAsync(cancellationToken);

            // 2. Для каждого аккорда получить все остальные вариации, кроме закреплённой
            var result = new Dictionary<int, List<ChordVariationDto>>(); // ChordId -> List<ChordVariationDto>
            foreach (var defaultVar in defaultVariations)
            {
                var all = await chordVariationService.GetByChordIdAsync(defaultVar.ChordId, cancellationToken);
                var others = all.Where(v => v.Id != defaultVar.Id).ToList();
                result[defaultVar.ChordId] = new List<ChordVariationDto> { ChordVariationMapper.ToDto(defaultVar) };
                result[defaultVar.ChordId].AddRange(others);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var chordVariations = await chordVariationService.GetAllAsync(cancellationToken);
            return Ok(chordVariations);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] ChordVariationCreateDto dto, CancellationToken cancellationToken)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "ChordVariationOwnerOrAdmin");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            await chordVariationService.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "ChordVariationOwnerOrAdmin");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            await chordVariationService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}