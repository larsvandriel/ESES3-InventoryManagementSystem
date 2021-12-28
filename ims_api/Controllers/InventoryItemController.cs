using InventoryManagementSystem.API.Filters;
using InventoryManagementSystem.Contracts;
using InventoryManagementSystem.Entities.Extensions;
using InventoryManagementSystem.Entities.Models;
using InventoryManagementSystem.Entities.Parameters;
using InventoryManagementSystem.Entities.ShapedEntities;
using LoggingService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace InventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryItemController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public InventoryItemController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetAllInventoryItems([FromQuery] InventoryItemParameters inventoryItemParameters)
        {
            try
            {

                var inventoryItems = _repository.InventoryItem.GetAllInventoryItems(inventoryItemParameters);

                var metadata = new
                {
                    inventoryItems.TotalCount,
                    inventoryItems.PageSize,
                    inventoryItems.CurrentPage,
                    inventoryItems.TotalPages,
                    inventoryItems.HasNext,
                    inventoryItems.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {inventoryItems.Count} inventory items from database.");

                var shapedInventoryItems = inventoryItems.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedInventoryItems);
                }

                for (var index = 0; index < inventoryItems.Count; index++)
                {
                    var supplierLinks = CreateLinksForInventoryItem(((ShapedInventoryItemEntity)inventoryItems[index]).InventoryId, ((ShapedInventoryItemEntity)inventoryItems[index]).ProductId, inventoryItemParameters.Fields);
                    shapedInventoryItems[index].Add("Links", supplierLinks);
                }

                var inventoryItemWrapper = new LinkCollectionWrapper<Entity>(shapedInventoryItems);

                return Ok(CreateLinksForInventoryItems(inventoryItemWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllInventoryItems action: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{inventoryId}/{productId}")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetInventoryItemByInventoryIdAndProductId(Guid inventoryId, Guid productId, [FromQuery] string fields)
        {
            try
            {
                var inventoryItem = (ShapedInventoryItemEntity)_repository.InventoryItem.GetInventoryItemByInventoryIdAndProductId(inventoryId, productId, fields);

                if (inventoryItem.InventoryId == Guid.Empty)
                {
                    _logger.LogError($"InventoryItem with SupplierId: {inventoryId}, hasn't been found in db.");
                    return NotFound();
                }

                if (inventoryItem.ProductId == Guid.Empty)
                {
                    _logger.LogError($"InventoryItem with SupplierId: {inventoryId} and ProductId: {productId}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped InventoryItem with supplierId: {inventoryId} and productId: {productId}");
                    return Ok(inventoryItem.Entity);
                }

                inventoryItem.Entity.Add("Links", CreateLinksForInventoryItem(inventoryItem.InventoryId, inventoryItem.ProductId, fields));

                return Ok(inventoryItem.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryItemByInventoryIdAndProductId action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateInventoryItem([FromBody] InventoryItem inventoryItem)
        {
            try
            {
                if (inventoryItem.IsObjectNull())
                {
                    _logger.LogError("InventoryItem object sent from client is null.");
                    return BadRequest("InventoryItem object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid InventoryItem object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.InventoryItem.CreateInventoryItem(inventoryItem);
                _repository.Save();

                return CreatedAtRoute("GetInventoryItemByInventoryIdAndProductId", new { inventoryId = inventoryItem.InventoryId, productId = inventoryItem.ProductId }, inventoryItem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateInventoryItem action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{inventoryId}/{productId}")]
        public IActionResult UpdateInventoryItem(Guid inventoryId, Guid productId, [FromBody] InventoryItem inventoryItem)
        {
            try
            {
                if (inventoryItem.IsObjectNull())
                {
                    _logger.LogError("InventoryItem object sent from client is null.");
                    return BadRequest("InventoryItem object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid InventoryItem object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var dbInventoryItem = _repository.InventoryItem.GetInventoryItemByInventoryIdAndProductId(inventoryId, productId);
                if (dbInventoryItem.IsEmptyObject())
                {
                    _logger.LogError($"InventoryItem with inventoryId: {inventoryId} and productId: {productId}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.InventoryItem.UpdateInventoryItem(dbInventoryItem, inventoryItem);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateInventoryItem action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{inventoryId}/{productId}")]
        public IActionResult DeleteInventoryItem(Guid inventoryId, Guid productId)
        {
            try
            {
                var inventoryItem = _repository.InventoryItem.GetInventoryItemByInventoryIdAndProductId(inventoryId, productId);
                if (inventoryItem.IsEmptyObject())
                {
                    _logger.LogError($"InventoryItem with inventoryId: {inventoryId} and productId: {productId}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.InventoryItem.DeleteInventoryItem(inventoryItem);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInventoryItem action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForInventoryItem(Guid supplierId, Guid productId, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetInventoryItemByInventoryIdAndProductId), values: new {supplierId, productId, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteInventoryItem), values: new {supplierId, productId}), "delete_inventoryItem", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateInventoryItem), values: new {supplierId, productId}), "update_inventoryItem", "PUT")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForInventoryItems(LinkCollectionWrapper<Entity> inventoryItemsWrapper)
        {
            inventoryItemsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAllInventoryItems), values: new { }), "self", "GET"));

            return inventoryItemsWrapper;
        }
    }
}
