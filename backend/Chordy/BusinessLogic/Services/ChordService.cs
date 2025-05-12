using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Validators;
using Chordy.DataAccess.Repositories.Implementations;
using Chordy.DataAccess.Repositories.Interfaces;

namespace Chordy.BusinessLogic.Services
{
    public class ChordService(IChordRepository chordRepository) : IChordService
    {
        public async Task<ChordDto> CreateAsync(ChordCreateDto chordCreateDto, CancellationToken cancellationToken = default)
        {
            ChordValidator.Validate(chordCreateDto);
            var existingChord = await chordRepository.GetByNameAsync(chordCreateDto.Name, cancellationToken);
            if (existingChord != null)
            {
                throw new DuplicationConflictException($"Аккорд с названием {chordCreateDto.Name} уже существует");
            }
            var chord = ChordMapper.ToEntity(chordCreateDto);
            await chordRepository.CreateAsync(chord, cancellationToken);
            return ChordMapper.ToDto(chord);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var chord = await chordRepository.GetByIdAsync(id, cancellationToken);
            if (chord == null)
            {
                throw new KeyNotFoundException($"Аккорд с ID {id} не найден");
            }
            await chordRepository.DeleteAsync(chord, cancellationToken);
        }

        public async Task<List<ChordDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var chords = await chordRepository.GetAllAsync(cancellationToken);
            return chords.Select(ChordMapper.ToDto).ToList();
        }

        public async Task<ChordDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var chord = await chordRepository.GetByIdAsync(id, cancellationToken);
            if (chord == null)
            {
                throw new KeyNotFoundException($"Аккорд с ID {id} не найден");
            }
            return ChordMapper.ToDto(chord);
        }

        public async Task<ChordDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var chord = await chordRepository.GetByNameAsync(name, cancellationToken);
            if (chord == null)
            {
                throw new KeyNotFoundException($"Аккорд с названием {name} не найден");
            }
            return ChordMapper.ToDto(chord);
        }

        public async Task UpdateAsync(int id, ChordCreateDto chordCreateDto, CancellationToken cancellationToken = default)
        {
            ChordValidator.Validate(chordCreateDto);
            var chord = await chordRepository.GetByIdAsync(id, cancellationToken);
            if (chord == null)
            {
                throw new KeyNotFoundException($"Аккорд с ID {id} не найден");
            }
            chord.Name = chordCreateDto.Name;
            await chordRepository.UpdateAsync(chord, cancellationToken);
        }

        public async Task<List<ChordDto>> SearchChordsByNameAsync(string query, CancellationToken cancellationToken = default)
        {
            var chords = await chordRepository.GetAllAsync(cancellationToken);
            var filtered = chords
                .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return filtered.Select(ChordMapper.ToDto).ToList();
        }
    }
}
