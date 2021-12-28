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
    public class InventoryController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public InventoryController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetInventories([FromQuery] InventoryParameters inventoryParameters)
        {
            try
            {
                var inventories = _repository.Inventory.GetAllInventories(inventoryParameters);

                var metadata = new
                {
                    inventories.TotalCount,
                    inventories.PageSize,
                    inventories.CurrentPage,
                    inventories.TotalPages,
                    inventories.HasNext,
                    inventories.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {inventories.Count} inventories from database.");

                var shapedInventories = inventories.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedInventories);
                }

                for (var index = 0; index < inventories.Count; index++)
                {
                    var inventoryLinks = CreateLinksForInventory(((NormalShapedEntity)inventories[index]).Id, inventoryParameters.Fields);
                    shapedInventories[index].Add("Links", inventoryLinks);
                }

                var inventoriesWrapper = new LinkCollectionWrapper<Entity>(shapedInventories);

                return Ok(CreateLinksForInventories(inventoriesWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "InventoryById")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetInventoryById(Guid id, [FromQuery] string fields)
        {
            try
            {
                var inventory = (NormalShapedEntity)_repository.Inventory.GetInventoryById(id, fields);

                if (inventory.Id == Guid.Empty)
                {
                    _logger.LogError($"Inventory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped inventory with id: {id}");
                    return Ok(inventory.Entity);
                }

                inventory.Entity.Add("Links", CreateLinksForInventory(inventory.Id, fields));

                return Ok(inventory.Entity);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wring inside GetInventoryById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateInventory([FromBody] Inventory inventory)
        {
            try
            {
                if (inventory.IsObjectNull())
                {
                    _logger.LogError("Inventory object sent from client is null.");
                    return BadRequest("Inventory object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid inventory object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.Inventory.CreateInventory(inventory);
                _repository.Save();

                return CreatedAtRoute("InventoryById", new { id = inventory.Id }, inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateInventory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateInventory(Guid id, [FromBody] Inventory inventory)
        {
            try
            {
                if (inventory.IsObjectNull())
                {
                    _logger.LogError("Inventory object sent from client is null.");
                    return BadRequest("Inventory object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid inventory object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var dbInventory = _repository.Inventory.GetInventoryById(id);
                if (dbInventory.IsEmptyObject())
                {
                    _logger.LogError($"Inventory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Inventory.UpdateInventory(dbInventory, inventory);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteInventory(Guid id)
        {
            try
            {
                var inventory = _repository.Inventory.GetInventoryById(id);
                if (inventory.IsEmptyObject())
                {
                    _logger.LogError($"Inventory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Inventory.DeleteInventory(inventory);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForInventory(Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetInventoryById), values: new {id, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteInventory), values: new {id}), "delete_inventory", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateInventory), values: new {id}), "update_inventory", "PUT")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForInventories(LinkCollectionWrapper<Entity> inventoriesWrapper)
        {
            inventoriesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetInventories), values: new { }), "self", "GET"));

            return inventoriesWrapper;
        }
    }
}
