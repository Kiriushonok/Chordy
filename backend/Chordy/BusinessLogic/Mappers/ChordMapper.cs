using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Entities;

namespace Chordy.BusinessLogic.Mappers
{
    public static class ChordMapper
    {
        public static ChordDto ToDto(Chord chord)
        {
            return new ChordDto
            {
                Id = chord.Id,
                Name = chord.Name,
            };
        }

        public static Chord ToEntity(ChordCreateDto chordCreateDto)
        {
            return new Chord
            {
                Name = chordCreateDto.Name
            };
        }
    }
}
