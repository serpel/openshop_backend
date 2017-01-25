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


        [HttpGet]
        [HttpPost]
        public HttpResponseMessage LoginByEmail(string username = "", string password = "")
        {
            if(username.Count() > 0 && password.Count() > 0)
            {
                var user = db.DeviceUser
                  .Where(w => w.Username == username.Trim() && w.Password == password.Trim())
                  .ToList()
                  .Select(s => new {
                      id = s.DeviceUserId,
                      access_token = s.AccessToken,
                      name = s.Name,
                      username = s.Username
                  }).FirstOrDefault();

                return Request.CreateResponse(HttpStatusCode.OK, user, Configuration.Formatters.JsonFormatter);
            }     

            return Request.CreateResponse(HttpStatusCode.NotFound, new { error = "User not found" }, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
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
                .Where(w => w.PartentId == 0)
                .Select(s => new { id = s.CategoryId, original_id = s.RemoteId, name = s.Name,
                    children = db.Categories.Where(w => w.PartentId == s.RemoteId).ToList().Select(sc => new { id = sc.CategoryId, original_id = sc.RemoteId, name = sc.Name, children = new String[] { }, type = sc.Type }),
                    type = s.Type });

            var result = new { navigation = categories };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
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

        [HttpGet]
        //TODO: fix search related products
        public HttpResponseMessage GetProduct(int id, string include = "")
        {
            var product = db.Products
                .Where(w => w.ProductId == id)
                .ToList()
                .Select(s =>new
                {
                    id = s.ProductId,
                    remote_id = s.RemoteId,
                    url = "",
                    name = s.Name,
                    category = s.CategoryId,
                    brand = s.Brand.Name,
                    season = s.Season,
                    code = s.Code,
                    description = s.Description,
                    main_image = s.MainImage,
                    main_image_high_res = s.MainImageHighRes,
                    images = new String[] { },
                    variants = s.Variants
                                .ToList()
                                .Select(v => new
                                {
                                    id = v.ProductVariantId,
                                    color = new {          
                                                id = v.Color.ColorId,
                                                remote_id = v.Color.RemoteId,
                                                value = v.Color.Value,
                                                code = v.Color.Code,
                                                img = v.Color.Image,
                                                description = v.Color.Description
                                    },
                                    size = new {
                                        id = v.Size.SizeId,
                                        remote_id = v.Size.RemoteId,
                                        value = v.Size.Value,
                                        description = v.Size.Description
                                    },
                                    images = v.Images,
                                    code = v.Code,
                                    quantity = v.Quantity,
                                    is_committed = v.IsCommitted,
                                    price = v.Price,
                                    currency = v.Currency
                                })
                }).FirstOrDefault();

            if (include.Trim() == "related")
            {

            }

            return Request.CreateResponse(HttpStatusCode.OK, product, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage GetProducts(int category = -1, string sort = "", string search = "")
        {
            var products = db.Products
                .ToList()
                .Select(s => new
                {
                    id = s.ProductId,
                    remote_id = s.RemoteId,
                    url = @"http:\/\/img.bfashion.com\/products\/presentation\/0d5b86cca1cd7f09172526e2ffe3022408c4f727.jpg",
                    name = s.Name,
                    category = s.Category.RemoteId,
                    categoryCode = s.Category.Code,
                    brand = s.Brand.Name,
                    brandCode = s.Brand.Code,
                    season = s.Season,
                    code = s.Code,
                    description = s.Description,
                    main_image = s.MainImage,
                    main_image_high_res = s.MainImageHighRes,
                    images = new String[] { },
                    variants = new List<ProductVariant>(),
                    related = new String[] { }
                });

            if(category >= 0)
            {
                products = products.Where(w => w.category == category);
            }

            if(search.Count() > 0)
            {
                products = products.Where(w => w.code.Contains(search.ToUpper())
                || w.brand.Contains(search.ToUpper())
                || w.categoryCode == search.ToUpper()
                || w.description.ToUpper().Contains(search.ToUpper())
                );
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
                        //products = products.OrderByDescending(o => o.price);
                        break;
                   case "price_asc":
                        //products = products.OrderBy(o => o.price);
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

        [HttpGet]
        [HttpDelete]
        public HttpResponseMessage DeleteToCart(int userId, int id)
        {
            bool success = true;
            String message = "";

            var cart_item = db.CartProductItems
                .ToList()
                .Where(w => w.Cart.DeviceUserId == userId && w.CartProductItemId == id)
                .FirstOrDefault();

            try
            {
                db.CartProductVariants.Remove(cart_item.CartProductVariant);
                db.CartProductItems.Remove(cart_item);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                success = false;
                message = e.Message;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = success, message = message }, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AddToCart(int userId = -1, int product_variant_id = -1, int quantity = 0)
        {
            bool success = false;

            if(userId > 0 && product_variant_id > 0)
            {
                var product_variant = db.ProductVariants
                    .ToList()
                    .Where(w => w.ProductVariantId == product_variant_id)
                    .FirstOrDefault();

                //load a user profile is not needed at all
                //var user = db.DeviceUser
                //    .Where(w => w.DeviceUserId == userId)
                //    .FirstOrDefault();

                var cart = db.Carts
                    .ToList()
                    .Where(w => w.DeviceUserId == userId)
                    .FirstOrDefault();

                //create a new cart if not exist
                if(cart == null && product_variant != null)
                {
                    var new_cart = new Cart()
                    {
                        DeviceUserId = userId,
                        Currency = product_variant.Currency,
                        TotalPrice = 0,
                        TotalPriceFormatted = product_variant.Currency + ' ' + 0
                    };

                    db.Carts.Add(new_cart);
                    db.SaveChanges();
                    cart = new_cart;
                }

                if(cart != null && product_variant != null)
                {
                    var cart_item_variant = new CartProductVariant()
                    {
                        CategoryId = product_variant.Product.CategoryId,
                        ColorId = product_variant.ColorId,
                        SizeId = product_variant.SizeId,
                        MainImage = product_variant.Product.MainImage,
                        Name = product_variant.Code,
                        ProductVariantId = product_variant.ProductVariantId,
                        Url = "",
                        Price = product_variant.Price,
                        PriceFormatted = product_variant.GetPriceTotalFormated()
                    };

                    db.CartProductVariants.Add(cart_item_variant);
                    db.SaveChanges();

                    var cart_item = new CartProductItem()
                    {
                        CartProductVariantId = cart_item_variant.CartProductVariantId,
                        CartId = cart.CartId,
                        Expiration = 0,
                        Quantity = quantity,
                        RemoteId = product_variant.Product.RemoteId,
                        TotalItemPrice = (quantity * product_variant.Price),
                        TotalItemPriceFormatted = product_variant.Currency + ' ' + (quantity * product_variant.Price)
                    };

                    db.CartProductItems.Add(cart_item);
                    db.SaveChanges();

                    success = true;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = success }, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage CartInfo(int userId = -1)
        {
            var cart = db.Carts
               .Where(w => w.DeviceUserId == userId)
               .ToList()
               .Select(s => new
               {
                   product_count = s.GetProductCount(),
                   total_price = s.GetProductTotalPrice(),
                   total_price_formatted = (s.Currency + s.GetProductTotalPrice().ToString()),
                   currency = s.Currency,
                   discounts = new String[] { },
                   products = s.CartProductItems.ToList()
                   .Select(p => new {
                       id = p.CartProductItemId,
                       quantity = p.Quantity,
                       product = new
                       {
                            //p.CartProductVariant
                        }
                   })
               }).FirstOrDefault();

            if (cart == null)
            {
                var empy_cart = new
                {
                    product_count = 0,
                    total_price = 0,
                    currency = "L",
                    total_price_formatted = "L 0.0",
                    discounts = new String[] { },
                    products = new String[] { }
                };

                return Request.CreateResponse(HttpStatusCode.OK, empy_cart, Configuration.Formatters.JsonFormatter);
            }

            return Request.CreateResponse(HttpStatusCode.OK, cart, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage Cart(int userId = -1)
        {
            var cart = db.Carts
                .Where(w => w.DeviceUserId == userId)
                .ToList()
                .Select(s => new
                {
                    product_count = s.GetProductCount(),
                    total_price = s.GetProductTotalPrice(),
                    currency =  s.Currency,
                    discounts =  new String[] {},
                    items = s.CartProductItems.ToList()
                    .Select(p => new {
                        id = p.CartProductItemId,
                        quantity = p.Quantity,
                        totalItemPrice = p.TotalItemPrice,
                        total_item_price_formatted = p.TotalItemPriceFormatted,
                        variant =  new
                        {
                            //TODO: fix remoteId it should be the original remoteId from product variant
                            id = p.CartProductVariant.CartProductVariantId,
                            remoteId = p.CartProductVariant.CartProductVariantId,
                            product_variant_id = p.CartProductVariant.ProductVariantId,
                            url = "",
                            name = p.CartProductVariant.Name,
                            price = p.CartProductVariant.Price,
                            price_formatted = p.CartProductVariant.Price,
                            category = p.CartProductVariant.CategoryId,
                            currency = "",
                            code = p.CartProductVariant.ProductVariant.Code,
                            description = p.CartProductVariant.Name,
                            main_image = p.CartProductVariant.ProductVariant.Product.MainImage,
                            color = new
                            {
                                id = p.CartProductVariant.ProductVariant.Color.ColorId,
                                remote_id = p.CartProductVariant.Color.RemoteId,
                                value = p.CartProductVariant.ProductVariant.Color.Value,
                                code = p.CartProductVariant.ProductVariant.Color.Code,
                                img = p.CartProductVariant.ProductVariant.Color.Image
                            },
                            size = new
                            {
                                id = p.CartProductVariant.ProductVariant.Size.SizeId,
                                remote_id = p.CartProductVariant.ProductVariant.Size.RemoteId,
                                value = p.CartProductVariant.ProductVariant.Size.Value,
                                description = p.CartProductVariant.ProductVariant.Size.Description
                            }
                        }
                    })
                }).FirstOrDefault();

            if(cart == null)
            {
                var empy_cart = new
                {
                    product_count = 0,
                    total_price = 0,
                    currency = "L",
                    discounts = new String[] { },
                    products = new String[] { }
                };

                return Request.CreateResponse(HttpStatusCode.OK, empy_cart, Configuration.Formatters.JsonFormatter);
            }

            return Request.CreateResponse(HttpStatusCode.OK, cart, Configuration.Formatters.JsonFormatter);
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