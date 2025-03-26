using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Chordy.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/collection")]
    public class CollectionController(ICollectionService collectionService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCollectionAsync([FromBody] CollectionDto collectionDto, CancellationToken cancellationToken) 
        {
            var collection = await collectionService.CreateAsync(collectionDto.Name, cancellationToken);
            return CreatedAtAction("GetCollectionById", new {id = collection.Id}, collection);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCollectionByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var collection = await collectionService.GetByIdAsync(id, cancellationToken);
            return Ok(collection);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCollectionsAsync(CancellationToken cancellationToken)
        {
            var collection = await collectionService.GetAllAsync(cancellationToken);
            return Ok(collection);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCollectionAsync([FromRoute] int id, [FromBody] string newName, CancellationToken cancellationToken)
        {
            await collectionService.UpdateAsync(id, newName, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCollectionAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await collectionService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
