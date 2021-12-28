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
    public class InventoryItemNotificationThresholdController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public InventoryItemNotificationThresholdController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetAllInventoryItemNotificationThresholds([FromQuery] InventoryItemNotificationThresholdParameters inventoryItemNotificationThresholdParameters)
        {
            try
            {

                var inventoryItemNotificationThresholds = _repository.InventoryItemNotificationThreshold.GetAllInventoryItemNotificationThresholds(inventoryItemNotificationThresholdParameters);

                var metadata = new
                {
                    inventoryItemNotificationThresholds.TotalCount,
                    inventoryItemNotificationThresholds.PageSize,
                    inventoryItemNotificationThresholds.CurrentPage,
                    inventoryItemNotificationThresholds.TotalPages,
                    inventoryItemNotificationThresholds.HasNext,
                    inventoryItemNotificationThresholds.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {inventoryItemNotificationThresholds.Count} InventoryItemNotificationThresholds from database.");

                var shapedInventoryItemNotificationThresholds = inventoryItemNotificationThresholds.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedInventoryItemNotificationThresholds);
                }

                for (var index = 0; index < inventoryItemNotificationThresholds.Count; index++)
                {
                    var supplierLinks = CreateLinksForInventoryItemNotificationThreshold(((ShapedInventoryItemNotificationThresholdEntity)inventoryItemNotificationThresholds[index]).InventoryId, ((ShapedInventoryItemNotificationThresholdEntity)inventoryItemNotificationThresholds[index]).ProductId, ((ShapedInventoryItemNotificationThresholdEntity)inventoryItemNotificationThresholds[index]).ThresholdAmount, ((ShapedInventoryItemNotificationThresholdEntity)inventoryItemNotificationThresholds[index]).ThresholdType, ((ShapedInventoryItemNotificationThresholdEntity)inventoryItemNotificationThresholds[index]).NotificationMethod, inventoryItemNotificationThresholdParameters.Fields);
                    shapedInventoryItemNotificationThresholds[index].Add("Links", supplierLinks);
                }

                var inventoryItemNotificationThresholdWrapper = new LinkCollectionWrapper<Entity>(shapedInventoryItemNotificationThresholds);

                return Ok(CreateLinksForInventoryItemNotificationThresholds(inventoryItemNotificationThresholdWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllInventoryItemNotificationThresholds action: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{inventoryId}/{productId}/{thresholdAmount}/{thresholdType}/{notificationMethod}")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod(Guid inventoryId, Guid productId, int thresholdAmount, ThresholdType thresholdType, string notificationMethod, [FromQuery] string fields)
        {
            try
            {
                var inventoryItemNotificationThreshold = (ShapedInventoryItemNotificationThresholdEntity)_repository.InventoryItemNotificationThreshold.GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod(inventoryId, productId, thresholdAmount, thresholdType, notificationMethod, fields);

                if (inventoryItemNotificationThreshold.InventoryId == Guid.Empty)
                {
                    _logger.LogError($"InventoryItemNotificationThreshold with SupplierId: {inventoryId}, hasn't been found in db.");
                    return NotFound();
                }

                if (inventoryItemNotificationThreshold.ProductId == Guid.Empty)
                {
                    _logger.LogError($"InventoryItemNotificationThreshold with ProductId: {productId}, hasn't been found in db.");
                    return NotFound();
                }

                if (inventoryItemNotificationThreshold.ThresholdAmount < 0)
                {
                    _logger.LogError($"InventoryItemNotificationThreshold with a thresholdAmount of {thresholdAmount}, hasn't been found in db.");
                    return NotFound();
                }

                if (inventoryItemNotificationThreshold.NotificationMethod == null)
                {
                    _logger.LogError($"InventoryItemNotificationThreshold with NotificationMethod: {notificationMethod}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped InventoryItemNotificationThreshold with supplierId: {inventoryId}, productId: {productId}, a thresholdAmount off {thresholdAmount}, ThresholdType: {thresholdType} and NotificationMethod: {notificationMethod}");
                    return Ok(inventoryItemNotificationThreshold.Entity);
                }

                inventoryItemNotificationThreshold.Entity.Add("Links", CreateLinksForInventoryItemNotificationThreshold(inventoryItemNotificationThreshold.InventoryId, inventoryItemNotificationThreshold.ProductId, inventoryItemNotificationThreshold.ThresholdAmount, inventoryItemNotificationThreshold.ThresholdType, inventoryItemNotificationThreshold.NotificationMethod, fields));

                return Ok(inventoryItemNotificationThreshold.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateInventoryItemNotificationThreshold([FromBody] InventoryItemNotificationThreshold inventoryItemNotificationThreshold)
        {
            try
            {
                if (inventoryItemNotificationThreshold.IsObjectNull())
                {
                    _logger.LogError("InventoryItemNotificationThreshold object sent from client is null.");
                    return BadRequest("InventoryItemNotificationThreshold object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid InventoryItemNotificationThreshold object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.InventoryItemNotificationThreshold.CreateInventoryItemNotificationThreshold(inventoryItemNotificationThreshold);
                _repository.Save();

                return CreatedAtRoute("GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod", new { inventoryId = inventoryItemNotificationThreshold.InventoryId, productId = inventoryItemNotificationThreshold.ProductId, thresholdAmount = inventoryItemNotificationThreshold.ThresholdAmount, thresholdType = inventoryItemNotificationThreshold.ThresholdType, notificationMethod = inventoryItemNotificationThreshold.NotificationMethod }, inventoryItemNotificationThreshold);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateInventoryItemNotificationThreshold action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{inventoryId}/{productId}/{thresholdAmount}/{thresholdType}/{notificationMethod}")]
        public IActionResult DeleteInventoryItemNotificationThreshold(Guid inventoryId, Guid productId, int thresholdAmount, ThresholdType thresholdType, string notificationMethod)
        {
            try
            {
                var inventoryItemNotificationThreshold = _repository.InventoryItemNotificationThreshold.GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod(inventoryId, productId, thresholdAmount, thresholdType, notificationMethod);
                if (inventoryItemNotificationThreshold.IsEmptyObject())
                {
                    _logger.LogError($"InventoryItemNotificationThreshold with inventoryId: {inventoryId}, productId: {productId}, thresholdAmount: {thresholdAmount}, thresholdType: {thresholdType} and notificationMethod {notificationMethod}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.InventoryItemNotificationThreshold.DeleteInventoryItemNotificationThreshold(inventoryItemNotificationThreshold);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInventoryItemNotificationThreshold action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForInventoryItemNotificationThreshold(Guid supplierId, Guid productId, int thresholdAmount, ThresholdType thresholdType, string notificationMethod, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetInventoryItemNotificationThresholdByInventoryIdProductIdThresholdAmountThresholdTypeAndNotificationMethod), values: new {supplierId, productId, thresholdAmount, thresholdType, notificationMethod, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteInventoryItemNotificationThreshold), values: new {supplierId, productId, thresholdAmount, thresholdType, notificationMethod}), "delete_inventoryItemNotificationAmount", "DELETE")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForInventoryItemNotificationThresholds(LinkCollectionWrapper<Entity> inventoryItemNotificationThresholdsWrapper)
        {
            inventoryItemNotificationThresholdsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAllInventoryItemNotificationThresholds), values: new { }), "self", "GET"));

            return inventoryItemNotificationThresholdsWrapper;
        }
    }
}
