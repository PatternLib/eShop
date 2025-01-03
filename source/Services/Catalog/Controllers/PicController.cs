﻿using EShopOnContainers.Catalog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EShopOnContainers.Catalog.Controllers;

public class PicController : ControllerBase
{
    private readonly CatalogContext _catalogContext;
    private readonly IWebHostEnvironment _env;

    public PicController(CatalogContext catalogContext, IWebHostEnvironment env)
    {
        _catalogContext = catalogContext;
        _env = env;
    }

    // Get image by productId
    // GET api/v1/catalog/items/1/pic
    [HttpGet]
    [Route("api/v1/catalog/items/{catalogItemId:int}/pic")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetImage(int catalogItemId)
    {
        if (catalogItemId <= 0)
        {
            return BadRequest();
        }

        var item = await _catalogContext.CatalogItems
                .SingleOrDefaultAsync(ci => ci.Id == catalogItemId);

        if (item != null)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, item.PictureFileName);

            string imageFileExtension = Path.GetExtension(item.PictureFileName);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = System.IO.File.ReadAllBytes(path);

            return File(buffer, mimetype);
        }

        return NotFound();
    }

    private string GetImageMimeTypeFromImageFileExtension(string extension)
    {
        string mimetype;

        switch (extension)
        {
            case ".png":
                mimetype = "image/png";
                break;
            case ".gif":
                mimetype = "image/gif";
                break;
            case ".jpg":
            case ".jpeg":
                mimetype = "image/jpeg";
                break;
            case ".bmp":
                mimetype = "image/bmp";
                break;
            case ".tiff":
                mimetype = "image/tiff";
                break;
            case ".wmf":
                mimetype = "image/wmf";
                break;
            case ".jp2":
                mimetype = "image/jp2";
                break;
            case ".svg":
                mimetype = "image/svg+xml";
                break;
            default:
                mimetype = "application/octet-stream";
                break;
        }

        return mimetype;
    }

}
