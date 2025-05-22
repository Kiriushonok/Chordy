using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;
using System.Text;
using System.Text.Json;

namespace Chordy.BusinessLogic.Mappers
{
    public static class ChordVariationMapper
    {
        public static ChordVariationDto ToDto(ChordVariation entity)
        {
            return new ChordVariationDto
            {
                Id = entity.Id,
                ChordId = entity.ChordId,
                Applicatura = JsonSerializer.Deserialize<ApplicaturaModel>(entity.Applicatura)!,
                StartFret = entity.StartFret,
                Bare = entity.Bare,
                FingeringSVG = Encoding.UTF8.GetString(entity.FingeringSVG),
                UserId = entity.UserId,
                ChordName = entity.Chord.Name,
            };
        }

        public static ChordVariation ToEntity(ChordVariationCreateDto dto, Guid userId)
        {
            return new ChordVariation
            {
                ChordId = dto.ChordId,
                Applicatura = JsonSerializer.Serialize(dto.Applicatura),
                StartFret = dto.StartFret,
                Bare = dto.Bare,
                FingeringSVG = Encoding.UTF8.GetBytes(dto.FingeringSVG),
                UserId = userId
            };
        }
    }
}
