using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chordy.WebApi.Controllers
{
    [ApiController]
    [Route("api/collections")]
    public class CollectionController(ICollectionService collectionService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCollectionAsync([FromBody] CollectionCreateDto collectionDto, CancellationToken cancellationToken) 
        {
            var collection = await collectionService.CreateAsync(collectionDto, cancellationToken);
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
        [Authorize]
        public async Task<IActionResult> UpdateCollectionAsync([FromRoute] int id, [FromBody] CollectionCreateDto collectionDto, CancellationToken cancellationToken)
        {
            await collectionService.UpdateAsync(id, collectionDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteCollectionAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await collectionService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
