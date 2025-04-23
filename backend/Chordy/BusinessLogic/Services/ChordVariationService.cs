using Chordy.BusinessLogic.Exceptions;
using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Mappers;
using Chordy.BusinessLogic.Models;
using Chordy.DataAccess.Repositories.Interfaces;
using Chordy.BusinessLogic.Validators;

namespace Chordy.BusinessLogic.Services
{
    public class ChordVariationService(IChordVariationRepository chordVariationRepository, IChordRepository chordRepository) : IChordVariationService
    {
        public async Task<ChordVariationDto> AddAsync(ChordVariationCreateDto dto, CancellationToken cancellationToken = default)
        {
            ChordVariationValidator.Validate(dto);
            var chord = await chordRepository.GetByIdAsync(dto.ChordId, cancellationToken);
            if (chord == null)
                throw new KeyNotFoundException($"Аккорд с ID {dto.ChordId} не найден");

            var entity = ChordVariationMapper.ToEntity(dto);
            await chordVariationRepository.AddAsync(entity, cancellationToken);

            return ChordVariationMapper.ToDto(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await chordVariationRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException($"Вариация аккорда с ID {id} не найдена");

            await chordVariationRepository.DeleteAsync(entity, cancellationToken);
        }

        public async Task<List<ChordVariationDto>> GetByChordIdAsync(int chordId, CancellationToken cancellationToken = default)
        {
            var chord = await chordRepository.GetByIdAsync(chordId, cancellationToken);
            if (chord == null)
                throw new KeyNotFoundException($"Аккорд с ID {chordId} не найден");

            var variations = await chordVariationRepository.GetByChordIdAsync(chordId, cancellationToken);
            return variations.Select(ChordVariationMapper.ToDto).ToList();
        }

        public async Task<ChordVariationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await chordVariationRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException($"Вариация аккорда с ID {id} не найдена");

            return ChordVariationMapper.ToDto(entity);
        }

        public async Task UpdateAsync(int id, ChordVariationCreateDto dto, CancellationToken cancellationToken = default)
        {
            ChordVariationValidator.Validate(dto);

            var entity = await chordVariationRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException($"Вариация аккорда с ID {id} не найдена");

            // Проверка существования аккорда
            var chord = await chordRepository.GetByIdAsync(dto.ChordId, cancellationToken);
            if (chord == null)
                throw new KeyNotFoundException($"Аккорд с ID {dto.ChordId} не найден");

            // Обновляем поля
            entity.ChordId = dto.ChordId;
            entity.Applicatura = System.Text.Json.JsonSerializer.Serialize(dto.Applicatura);
            entity.StartFret = dto.StartFret;
            entity.Bare = dto.Bare;
            entity.FingeringSVG = System.Text.Encoding.UTF8.GetBytes(dto.FingeringSVG);

            await chordVariationRepository.UpdateAsync(entity, cancellationToken);
        }
    }
}