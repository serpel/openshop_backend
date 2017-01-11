using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using OpenshopBackend.Models;
using OpenshopBackend.Models.Helper;

namespace OpenshopBackend.Api
{
    public class ListController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public HttpResponseMessage GetShops(int shop = -1)
        {
            var shops = db.Shops
                .ToList()
                .Select(s => new
                {
                    id = s.ShopId,
                    name = s.Name,
                    description = s.Description,
                    url = s.Url,
                    logo = s.Logo,
                    google_ua = s.GoogleUA,
                    language = s.Language,
                    currency = s.Currency,
                    flag_icon = s.FlagIcon
                });

            var result = new { metadata = new { }, records = shops };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        // GET: api/List
        public HttpResponseMessage GetNavigations(int shop = -1)
        {
            var categories = db.Categories
                .ToList()
                .Select(s => new { id = s.CategoryId, original_id = s.RemoteId, name = s.Name, children = new String[] { }, type = s.Type });

            var result = new { navigation = categories };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        public HttpResponseMessage GetBanners()
        {
            var banners = db.Banners
                .ToList()
                .Select(s => new
                {
                    id = s.BannerId,
                    name = s.Name,
                    target = s.Target,
                    imageUrl = s.ImageUrl
                });

            var result = new { metadata = new { }, records = banners };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage GetDevices()
        {
            var devices = db.Devices
                .ToList()
                .Select(s => new
                {
                    id = s.DeviceId,
                    device_token = s.DeviceToken,
                    platform = s.Platform
                });

            var result = new { metadata = new { }, records = devices };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        public HttpResponseMessage SetDevice(String message)
        {

            //return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //TODO: fix search related products
        public HttpResponseMessage GetProduct(int id, string include = "")
        {
            var product = db.Products
                .Where(w => w.ProductId == id)
                .ToList()
                .Select(s => new
                {
                    id = s.ProductId,
                    remote_id = s.RemoteId,
                    url = @"http:\/\/img.bfashion.com\/products\/presentation\/0d5b86cca1cd7f09172526e2ffe3022408c4f727.jpg",
                    name = s.Name,
                    price = s.Price,
                    price_formated = s.PriceFormated,
                    category = s.CategoryId,
                    brand = s.Brand.Name,
                    discounted_price = s.DisountedPrice,
                    discounted_price_formated = s.DisountedPriceFormated,
                    currency = s.Currency,
                    code = s.Code,
                    description = s.Description,
                    main_image = s.MainImage,
                    main_image_high_res = s.MainImageHighRes,
                    images = new String[] { },
                    variants = new List<ProductVariant>()
                }).FirstOrDefault();

            if (include.Trim() == "related")
            {

            }

            return Request.CreateResponse(HttpStatusCode.OK, product, Configuration.Formatters.JsonFormatter);
        }

        public HttpResponseMessage GetProducts(int category = -1, string sort = "")
        {
            var products = db.Products
                .ToList()
                .Select(s => new
                {
                    id = s.ProductId,
                    remote_id = s.RemoteId,
                    url = @"http:\/\/img.bfashion.com\/products\/presentation\/0d5b86cca1cd7f09172526e2ffe3022408c4f727.jpg",
                    name = s.Name,
                    price = s.Price,
                    price_formated = s.PriceFormated,
                    category = s.Category.RemoteId,
                    brand = s.Brand.Name,
                    discounted_price = s.DisountedPrice,
                    discounted_price_formated = s.DisountedPriceFormated,
                    currency = s.Currency,
                    code = s.Code,
                    description = s.Description,
                    main_image = s.MainImage,
                    main_image_high_res = s.MainImageHighRes,
                    images = new String[] { },
                    variants = new List<ProductVariant>()
                });

            if(category >= 0)
            {
                products = products.Where(w => w.category == category);
            }

            //TODO: fix sort polarity by rank field
            if (sort.ToLower().Count() > 0)
            {
                switch (sort.ToLower())
                {
                   case "newest":
                        products = products.OrderByDescending(o => o.id);
                        break;
                   case "popularity":
                        //products = products.OrderByDescending(o => o.rank);
                        break;
                   case "price_desc": 
                        products = products.OrderByDescending(o => o.price);
                        break;
                   case "price_asc":
                        products = products.OrderBy(o => o.price);
                        break;
                    default:
                        break;
                }
            }

            var links = new Link()
            {
                first = "http://localhost:64102/api/List/GetProducts",
                next = "http://localhost:64102/api/List/GetProducts",
                last = "http://localhost:64102/api/List/GetProducts",
                prev = "http://localhost:64102/api/List/GetProducts",
                self = "http://localhost:64102/api/List/GetProducts"
            };

            var result = new Record()
            {
                metadata = new Metadata()
                {
                    links = links,
                    records_count = products.ToList().Count,
                    sorting = "newest"
                },
                records = products
            };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}