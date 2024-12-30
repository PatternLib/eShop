using System.Net;
using EShopOnContainers.Basket.Domain;
using EShopOnContainers.Basket.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.Basket.Controllers;

[Route(template: "api/v1/[controller]")]
[Authorize]
public class BasketController : Controller
{
    private readonly ILogger<BasketController> _logger;
    private readonly IBasketRepository _repository;

    public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger)
    {
        _repository = basketRepository;
        _logger = logger;
    }

    // GET /id
    [HttpGet(template: "{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get(string id)
    {
        var basket = (await _repository.GetBasketAsync(customerId: id))
            ?? new CustomerBasket(buyerId: id);

        return Ok(value: basket);
    }

    // POST /value
    [HttpPost]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Post([FromBody] CustomerBasket value)
    {
        var basket = await _repository.UpdateBasketAsync(basket: value);
        
        return Ok(value: basket);
    }
}
