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
using Newtonsoft.Json;
using Hangfire;
using OpenshopBackend.BussinessLogic;
using System.Web.Script.Serialization;
using System.Text;
using OpenshopBackend.Controllers;
using OpenshopBackend.BussinessLogic.SAP;

namespace OpenshopBackend.Api
{
    public class ListController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private String default_currency = "L";
        private Double default_tax = 0.15;

        [AutomaticRetry(Attempts = 0)]
        public void CreatePaymentOnSAP(int paymentId)
        {
            IncomingPayment incomingPayment = new IncomingPayment();
            incomingPayment.MakePayment(paymentId);
        }

        //[HttpGet]
        //public HttpResponseMessage CreatePayment(String cash, String transfer)
        //{

        //}

        [HttpGet]
        public HttpResponseMessage ProcessPayment(int paymentId)
        {
            bool success = true;

            BackgroundJob.Enqueue(() => CreatePaymentOnSAP(paymentId));

            return Request.CreateResponse(HttpStatusCode.OK, success, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage Users()
        {
            var users = db.DeviceUser
                .ToList()
                .Select(s => new
                {
                    id = s.DeviceUserId,
                    sales_person_id = s.SalesPersonId,
                    name = s.Name,
                });

            var result = new
            {
                records = users
            };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }


        [HttpGet]
        [HttpPost]
        public HttpResponseMessage UpdateUser(int userId, String bluetooth)
        {
            bool success = true;
            string message = "";

            try
            {
                DeviceUser myUser = db.DeviceUser
                    .Where(w => w.DeviceUserId == userId)
                    .ToList()
                    .FirstOrDefault();

                myUser.PrintBluetoothAddress = bluetooth;

                db.Entry(myUser).State = EntityState.Modified;
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, myUser, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Configuration.Formatters.JsonFormatter);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetUser(int id)
        {
            var user = db.DeviceUser
                  .Where(w => w.DeviceUserId == id)
                  .ToList()
                  .Select(s => new
                  {
                      id = s.DeviceUserId,
                      access_token = s.AccessToken,
                      sales_person_id = s.SalesPersonId,
                      name = s.Name,
                      email = s.Username,
                      print_bluetooth_address = s.PrintBluetoothAddress
                  }).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, user, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage LoginByEmail(string username = "", string password = "")
        {
            if (username.Count() > 0 && password.Count() > 0)
            {
                var user = db.DeviceUser
                  .Where(w => w.Username == username.Trim() && w.Password == password.Trim())
                  .ToList()
                  .Select(s => new
                  {
                      id = s.DeviceUserId,
                      access_token = s.AccessToken,
                      sales_person_id = s.SalesPersonId,
                      name = s.Name,
                      username = s.Username,
                      print_bluetooth_address = s.PrintBluetoothAddress
                  }).FirstOrDefault();

                MyLogger.GetInstance.Debug(String.Format("LoginByEmail - username: {0}, pass: {1}", user.username, user.name));

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

            var categoryModels = db.Categories
                .ToList()
                .Select(v => new CategoryViewModel()
                {
                    CategoryId = v.CategoryId,
                    Id = v.Id,
                    Name = v.Name,
                    PartentId = v.PartentId,
                    Type = v.Type,
                    RemoteId = v.RemoteId
                 }
                )
                .ToList();

            var categories = categoryModels
                .Where(w => w.PartentId == 0)
                .Select(s => new {
                    id = s.Id,
                    name = s.Name,
                    original_id = s.RemoteId,
                    type = s.Type,
                    children = GetChildrens(categoryModels, s.Id)
                }).OrderBy(o => o.id);

            var result = new { navigation = categories };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        private Object GetChildrens(List<CategoryViewModel> categories, int parentId)
        {
            return categories
                .Where(w => w.PartentId == parentId)
                .Select(s => new
                {
                    id = s.Id,
                    original_id = s.RemoteId,
                    type = s.Type,
                    name = s.Name,
                    children = GetChildrens(categories, s.Id)
                })
                .ToList();
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
            var product = db.Products.AsQueryable();
            product = product.Where(w => w.ProductId == id);

            var result = product
                .ToList()
                .Select(s => new
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
                                .OrderBy(o => o.Code)
                                .ToList()
                                .Select(v => new
                                {
                                    id = v.ProductVariantId,
                                    color = new
                                    {
                                        id = v.Color.ColorId,
                                        remote_id = v.Color.RemoteId,
                                        value = v.Color.Value,
                                        code = v.Color.Code,
                                        img = v.Color.Image,
                                        description = v.Color.Description
                                    },
                                    size = new
                                    {
                                        id = v.Size.SizeId,
                                        remote_id = v.Size.RemoteId,
                                        value = v.Size.Value,
                                        description = v.Size.Description
                                    },
                                    warehouse = v.WareHouseCode,
                                    images = v.Images,
                                    code = v.Code,
                                    quantity = v.Quantity,
                                    is_committed = v.IsCommitted,
                                    price = v.Price,
                                    currency = v.Currency
                                })
                }).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage GetProducts2(int category = -1, string sort = "", string search = "")
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
                    //categoryCode = s.Category.Code,
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

            if (category >= 0)
            {
                products = products.Where(w => w.category == category);
            }

            if (search.Count() > 0)
            {
                products = products.Where(w => w.code.Contains(search.ToUpper())
                || w.brand.Contains(search.ToUpper())
                || w.name.ToUpper().Contains(search.ToUpper())
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
                    records_count = products.Count(),
                    sorting = "newest"
                },
                records = products
            };

            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage GetProducts(int category = -1, string sort = "", string search = "")
        {
            var result = db.Products.AsQueryable();

            if(category > 0)
                result = result.Where(w => w.Category.RemoteId == category);
            if (search.Count() > 0)
                result = result.Where(w => w.Code.Contains(search.ToUpper())
                        || w.Brand.Name.Contains(search.ToUpper())
                        || w.Name.ToUpper().Contains(search.ToUpper())
                );
            if (sort.ToLower().Count() > 0)
            {
                switch (sort.ToLower())
                {
                    case "newest":
                        result = result.OrderByDescending(o => o.ProductId);
                        break;
                    case "popularity":
                        //products = products.OrderByDescending(o => o.rank);
                        break;
                    case "price_desc":
                        result = result.OrderByDescending(o => o.Variants != null ? o.Variants.FirstOrDefault().Price : o.ProductId);
                        break;
                    case "price_asc":
                        result = result.OrderBy(o => o.Variants != null ? o.Variants.FirstOrDefault().Price : o.ProductId);
                        break;
                }
            }

            var products = result
                .ToList()
                .Select(s => new
                {
                    id = s.ProductId,
                    remote_id = s.RemoteId,
                    url = "",
                    name = s.Name,
                    category = s.Category.RemoteId,
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

            var links = new Link()
            {
                first = "",
                next = "",
                last = "",
                prev = "",
                self = ""
            };

            var respond = new Record()
            {
                metadata = new Metadata()
                {
                    links = links,
                    records_count = products.Count(),
                    sorting = "newest"
                },
                records = products
            };

            return Request.CreateResponse(HttpStatusCode.OK, respond, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage GetClients(String search = "")
        {
            var clients = db.Clients.AsQueryable();

            if (search.Count() > 0)
                clients = clients.Where(w => w.Name.ToLower().Contains(search.ToLower())
                || w.CardCode.ToLower().Contains(search.ToLower()));

            var result = clients
                .ToList()
                .Select(s => new
                {
                    id = s.ClientId,
                    card_code = s.CardCode,
                    phone = s.PhoneNumber,
                    address = s.Address,
                    name = s.Name
                });

            var records = new { records = result };

            return Request.CreateResponse(HttpStatusCode.OK, records, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage GetDocuments(string card_code = "")
        {
            var client = db.Clients
                .Where(w => w.CardCode == card_code)
                .ToList()
                .FirstOrDefault();

            var documents = db.Documents
                .Where(w => w.Client.CardCode == card_code)
                .ToList()
                .Select(s => new
                {
                    id = s.DocumentId,
                    document_code = s.DocumentCode,
                    created_date = s.CreatedDate,
                    due_date = s.DueDate,
                    total_amount = s.TotalAmount,
                    payed_amount = s.PayedAmount
                });

            var result = new
            {
                records = documents,
                client_card_code = client.CardCode,
                client_name = client.Name,
                credit_limit = client.CreditLimit,
                balance = client.Balance,
                in_orders = client.InOrders,
                pay_condition = client.PayCondition
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
                .Where(w => w.Cart.DeviceUserId == userId && w.CartProductItemId == id)
                .ToList()
                .FirstOrDefault();

            try
            {
                db.CartProductVariants.Remove(cart_item.CartProductVariant);
                db.CartProductItems.Remove(cart_item);
                db.SaveChanges();

                MyLogger.GetInstance.Debug(String.Format("DeleteToCart - userId: {0}, cartProductItem: {1}", userId, id));
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
                MyLogger.GetInstance.Error(e.Message, e);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = success, message = message }, Configuration.Formatters.JsonFormatter);
        }

        //[HttpGet]
        //[HttpPost]
        //public HttpResponseMessage AddToCart(int userId = -1, int product_variant_id = -1, int quantity = 0)
        //{
        //    bool success = false;

        //    if (userId > 0 && product_variant_id > 0)
        //    {
        //        var product_variant = db.ProductVariants
        //            .ToList()
        //            .Where(w => w.ProductVariantId == product_variant_id)
        //            .FirstOrDefault();

        //        var cart = db.Carts
        //            .ToList()
        //            .Where(w => w.DeviceUserId == userId)
        //            .FirstOrDefault();

        //        //create a new cart if not exist
        //        if (cart == null && product_variant != null)
        //        {
        //            var new_cart = new Cart()
        //            {
        //                DeviceUserId = userId,
        //                Currency = product_variant.Currency,
        //                TotalPrice = 0,
        //                TotalPriceFormatted = product_variant.Currency + ' ' + 0
        //            };

        //            db.Carts.Add(new_cart);
        //            db.SaveChanges();
        //            cart = new_cart;
        //        }

        //        if (cart != null && product_variant != null)
        //        {
        //            var cart_item_variant = new CartProductVariant()
        //            {
        //                CategoryId = product_variant.Product.CategoryId,
        //                ColorId = product_variant.ColorId,
        //                SizeId = product_variant.SizeId,
        //                MainImage = product_variant.Product.MainImage,
        //                Name = product_variant.Code,
        //                ProductVariantId = product_variant.ProductVariantId,
        //                WareHouseCode = product_variant.WareHouseCode,
        //                Url = "",
        //                Price = product_variant.Price,
        //                Discount = 0.0,
        //                PriceFormatted = product_variant.GetPriceTotalFormated()
        //            };

        //            db.CartProductVariants.Add(cart_item_variant);
        //            db.SaveChanges();

        //            var cart_item = new CartProductItem()
        //            {
        //                CartProductVariantId = cart_item_variant.CartProductVariantId,
        //                CartId = cart.CartId,
        //                Expiration = 0,
        //                Quantity = quantity,
        //                Discount = 0.0,
        //                RemoteId = product_variant.Product.RemoteId,
        //                TotalItemPrice = (quantity * product_variant.Price),
        //                TotalItemPriceFormatted = product_variant.Currency + ' ' + (quantity * product_variant.Price)
        //            };

        //            db.CartProductItems.Add(cart_item);
        //            db.SaveChanges();

        //            MyLogger.GetInstance.Debug(String.Format("AddToCart - userId: {0}, product_variant_id: {1}, quantity: {2}", userId, product_variant_id, quantity));

        //            success = true;
        //        }
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, new { success = success }, Configuration.Formatters.JsonFormatter);
        //}

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AddToCart(int userId = -1, int product_variant_id = -1, int quantity = 0, String cardcode = "")
        {
            bool success = false;

            if (userId > 0 && product_variant_id > 0)
            {
                var product_variant = db.ProductVariants
                     .Where(w => w.ProductVariantId == product_variant_id)
                    .ToList()
                    .FirstOrDefault();

                var cart = db.Carts
                    .Where(w => w.DeviceUserId == userId)
                    .ToList()
                    .FirstOrDefault();

                //create a new cart if not exist
                if (cart == null && product_variant != null)
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

                if (cart != null && product_variant != null)
                {
                    var discount = db.ClientDiscounts
                        .Where(w => w.CardCode == cardcode && w.ItemGroup == product_variant.ItemGroup)
                        .ToList()
                        .FirstOrDefault();

                    var cart_item_variant = new CartProductVariant()
                    {
                        CategoryId = product_variant.Product.CategoryId,
                        ColorId = product_variant.ColorId,
                        SizeId = product_variant.SizeId,
                        MainImage = product_variant.Product.MainImage,
                        Name = product_variant.Code,
                        ProductVariantId = product_variant.ProductVariantId,
                        WareHouseCode = product_variant.WareHouseCode,
                        Url = "",
                        Price = product_variant.Price,
                        Discount = (discount != null ? (product_variant.Price * discount.Discount)/100 : 0.0),
                        PriceFormatted = product_variant.GetPriceTotalFormated()
                    };

                    db.CartProductVariants.Add(cart_item_variant);
                    db.SaveChanges();

                    var setting = db.Settings.ToList().FirstOrDefault();
                    var isv = setting != null ? setting.ISV : 0.0;

                    var discountvalue = (discount != null ? ((product_variant.Price * discount.Discount) / 100) * quantity : 0.0);
                    var cart_item = new CartProductItem()
                    {
                        CartProductVariantId = cart_item_variant.CartProductVariantId,
                        CartId = cart.CartId,
                        Expiration = 0,
                        Quantity = quantity,
                        ISV = ((quantity * product_variant.Price) - discountvalue) * isv,
                        Discount = discountvalue,
                        RemoteId = product_variant.Product.RemoteId,
                        TotalItemPrice = (quantity * product_variant.Price),
                        TotalItemPriceFormatted = product_variant.Currency + ' ' + (quantity * product_variant.Price)
                    };

                    db.CartProductItems.Add(cart_item);
                    db.SaveChanges();

                    MyLogger.GetInstance.Debug(String.Format("AddToCart - userId: {0}, product_variant_id: {1}, quantity: {2}", userId, product_variant_id, quantity));

                    success = true;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = success }, Configuration.Formatters.JsonFormatter);
        }

        public HttpResponseMessage GetOrders(int userId)
        {
            bool success = false;

            if (userId > 0)
            {
                var orders = db.Orders
                    .Where(w => w.DeviceUser.DeviceUserId == userId)
                    .OrderByDescending(o => o.OrderId)
                    .ToList()
                    .Select(s => new
                    {
                        id = s.OrderId,
                        remote_id = s.RemoteId,
                        series = s.Series,
                        comment = s.Comment,
                        status = s.Status,
                        date_created = s.DateCreated,
                        item_count = s.GetItemCount(),
                        subtotal = s.GetSubtotal(),
                        discount = s.GetDiscount(),
                        IVA = s.GetIVA(),
                        total = s.GetTotal(),
                        total_formatted = default_currency + s.GetTotal(),
                        items = s.OrderItems
                                .ToList()
                                .Select(i => new {
                                    code = i.SKU,
                                    quantity = i.Quantity,
                                    price = i.Price,
                                    discount = i.Discount,
                                    tax = i.TaxValue,
                                    warehouse_code = i.WarehouseCode
                                }),
                        client = new
                        {
                            name = s.Client.Name,
                            card_code = s.Client.CardCode,
                            phone = s.Client.PhoneNumber,
                            address = s.Client.Address,
                            RTN = s.Client.RTN
                        },
                        seller = s.DeviceUser.Name
                    });

                var result = new
                {
                    metadata = new Object(),
                    records = orders
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { success = success }, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage Order(int id)
        {
            var order = db.Orders
                .ToList()
                .Where(w => w.OrderId == id)
                .FirstOrDefault();

            var result = new
            {
                id = order.OrderId,
                remote_id = order.RemoteId,
                series = order.Series,
                comment = order.Comment,
                status = order.Status,
                date_created = order.DateCreated,
                item_count = order.GetItemCount(),
                subtotal = order.GetSubtotal(),
                discount = order.GetDiscount(),
                IVA = order.GetIVA(),
                total = order.GetTotal(),
                total_formatted = default_currency + order.GetTotal(),
                items = order.OrderItems
                                .ToList()
                                .Select(i => new {
                                    code = i.SKU,
                                    quantity = i.Quantity,
                                    price = i.Price,
                                    discount = i.Discount,
                                    tax = i.TaxValue,
                                    warehouse_code = i.WarehouseCode
                                }),
                client = new
                {
                    name = order.Client.Name,
                    card_code = order.Client.CardCode,
                    phone = order.Client.PhoneNumber,
                    address = order.Client.Address,
                    RTN = order.Client.RTN
                },
                seller = order.DeviceUser.Name
            };

            if (order != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, order, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage ReCreateOrder(int orderId)
        {
            bool success = true;

            if (orderId > 0)
            {
                BackgroundJob.Enqueue(() => CreateQuotationOrderOnSap(orderId));
            }

            return Request.CreateResponse(HttpStatusCode.OK, success, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage CreateOrder(int userId, int cartId, String jo)
        {
            bool success = true;
            string message = "";

            try
            {
                OrderViewModel myOrder = JsonConvert.DeserializeObject<OrderViewModel>(jo);

                var deviceuser = db.DeviceUser
                    .Where(w => w.DeviceUserId == userId)
                    .ToList()
                    .FirstOrDefault();

                var cart = db.Carts
                    .Where(w => w.CartId == cartId)
                    .ToList()
                    .FirstOrDefault();

                var cartItems = cart.CartProductItems;

                var client = db.Clients
                    .Where(w => w.CardCode == myOrder.CardCode)
                    .ToList()
                    .FirstOrDefault();

                if (client != null && deviceuser != null)
                {
                    var order = new Order()
                    {
                        DateCreated = DateTime.Now.ToString(),
                        Series = myOrder.Series,
                        //RemoteId = order.RemoteId,
                        Comment = myOrder.Comment,
                        Status = OrderStatus.CreadoEnAplicacion.ToString(),
                        DeviceUserId = userId,
                        ClientId = client.ClientId
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    foreach (var item in cartItems)
                    {
                        var orderItem = new OrderItem()
                        {
                            OrderId = order.OrderId,
                            Quantity = item.Quantity,
                            SKU = item.CartProductVariant.Name,
                            Price = item.CartProductVariant.Price,
                            TaxValue = 0.15,
                            TaxCode = "IVA",
                            Discount = item.CartProductVariant.Discount,
                            WarehouseCode = item.CartProductVariant.WareHouseCode                  
                        };

                        //This line update the product iscommited
                        item.CartProductVariant.ProductVariant.IsCommitted += item.Quantity;
                        db.Entry(item).State = EntityState.Modified;
                        db.OrderItems.Add(orderItem);
                    }
                    db.SaveChanges();

                    db.Carts.Remove(cart);
                    db.SaveChanges();

                    int id = order.OrderId;

                    BackgroundJob.Enqueue(() => CreateQuotationOrderOnSap(id));

                    var result = new
                    {
                        id = order.OrderId,
                        remote_id = order.RemoteId,
                        series = order.Series,
                        comment = order.Comment,
                        status = order.Status,
                        date_created = order.DateCreated,
                        item_count = order.GetItemCount(),
                        subtotal = order.GetSubtotal(),
                        discount = order.GetDiscount(),
                        IVA = order.GetIVA(),
                        total = order.GetTotal(),
                        total_formatted = default_currency + order.GetTotal(),
                        items = order.OrderItems
                                .ToList()
                                .Select(i => new {
                                    code = i.SKU,
                                    quantity = i.Quantity,
                                    price = i.Price,
                                    discount = i.Discount,
                                    tax = i.TaxValue,
                                    warehouse_code = i.WarehouseCode
                                }),
                        client = new
                        {
                            name = order.Client.Name,
                            card_code = order.Client.CardCode,
                            phone = order.Client.PhoneNumber,
                            address = order.Client.Address,
                            RTN = order.Client.RTN
                        },
                        seller = deviceuser.Name
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, new { }, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
                MyLogger.GetInstance.Error(e.Message, e);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = success, message = message }, Configuration.Formatters.JsonFormatter);
        }

        [AutomaticRetry(Attempts = 0)]
        public void CreateQuotationOrderOnSap(int orderId)
        {
            String message = "";

            try
            {
                Quotation salesorder = new Quotation();
                message = salesorder.AddQuotation(orderId);
            }
            catch (Exception e)
            {
                MyLogger.GetInstance.Error(e.Message, e);
            }
        }


        [AutomaticRetry(Attempts = 0)]
        public void CreateOrderOnSap(int orderId)
        {
            var order = db.Orders
                .Where(w => w.OrderId == orderId)
                .ToList()
                .FirstOrDefault();

            String message = "";

            try
            {
                SalesOrder salesorder = new SalesOrder();
                if (salesorder.ServerConnection.Connect() == 0)
                {
                    message = salesorder.AddSalesOrder(order, order.OrderItems.ToList());
                    salesorder.ServerConnection.Disconnect();
                }

                if (message != "")
                {
                    order.RemoteId = message;
                    order.Status = OrderStatus.PreliminarEnSAP.ToString();
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MyLogger.GetInstance.Error(e.Message, e);
            }
        }


        //userId=%d&productCartId=%d%newQuantity=%d&newVariantId=%d
        [HttpGet]
        [HttpPut]
        public HttpResponseMessage UpdateToCart(int userId, int productCartItemId, int newQuantity, int newProductVariantId)
        {
            bool success = true;
            String message = "";

            if (userId > 0 && productCartItemId > 0 && newQuantity > 0 && newProductVariantId > 0)
            {
                var product_variant = db.ProductVariants
                    .Where(w => w.ProductVariantId == newProductVariantId)
                    .ToList()
                    .FirstOrDefault();

                var product_cart = db.CartProductItems
                    .Where(w => w.CartProductItemId == productCartItemId)
                    .ToList()
                    .FirstOrDefault();

                var cart_product_variant = product_cart.CartProductVariant;

                //update the new variant
                cart_product_variant.ProductVariantId = product_variant.ProductVariantId;
                cart_product_variant.Price = product_variant.Price;
                cart_product_variant.PriceFormatted = product_variant.GetPriceTotalFormated();
                cart_product_variant.SizeId = product_variant.SizeId;
                cart_product_variant.ColorId = product_variant.ColorId;
                cart_product_variant.WareHouseCode = product_variant.WareHouseCode;

                //Update the visual of Total Price
                product_cart.Quantity = newQuantity;
                product_cart.TotalItemPrice = newQuantity * cart_product_variant.Price;
                product_cart.TotalItemPriceFormatted = product_variant.Currency + " " + product_cart.TotalItemPrice;

                try
                {
                    db.Entry(product_cart).State = EntityState.Modified;
                    db.Entry(cart_product_variant).State = EntityState.Modified;
                    db.SaveChanges();

                    MyLogger.GetInstance.Debug(String.Format("UpdateToCart - username: {0}, cart: {1}, cart_product_variant_id: {1}", userId, product_cart.CartId, cart_product_variant.CartProductVariantId));
                }
                catch (Exception e)
                {
                    MyLogger.GetInstance.Error(e.Message, e);
                    success = false;
                    message = e.Message;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success = success, message = message }, Configuration.Formatters.JsonFormatter);
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
                   .Select(p => new
                   {
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
                    id = s.CartId,
                    product_count = s.GetProductCount(),
                    total_price = s.GetProductTotalPrice(),
                    discount = s.GetProductDiscountPrice(),
                    ISV = s.GetProductISVPrice(),
                    subtotal = s.GetProductSubtotalPrice(),
                    currency = s.Currency,
                    discounts = new String[] { },
                    items = s.CartProductItems
                    .OrderBy(o => o.CartProductVariant.Name)
                    .ToList()
                    .Select(p => new
                    {
                        id = p.CartProductItemId,
                        quantity = p.Quantity,
                        discount = p.Discount,
                        totalItemPrice = p.TotalItemPrice,
                        total_item_price_formatted = p.TotalItemPriceFormatted,
                        variant = new
                        {
                            //TODO: fix remoteId it should be the original remoteId from product variant
                            id = p.CartProductVariant.CartProductVariantId,
                            remoteId = p.CartProductVariant.CartProductVariantId,
                            product_id = p.CartProductVariant.ProductVariant.ProductId,
                            product_variant_id = p.CartProductVariant.ProductVariantId,
                            url = "",
                            name = p.CartProductVariant.Name,
                            discount_price = p.Discount,
                            price = p.CartProductVariant.Price,
                            price_formatted = p.CartProductVariant.Price,
                            category = p.CartProductVariant.CategoryId,
                            currency = "",
                            code = p.CartProductVariant.ProductVariant.Code,
                            description = p.CartProductVariant.Name,
                            main_image = p.CartProductVariant.ProductVariant.Product.MainImage,
                            warehouse_code = p.CartProductVariant.WareHouseCode,
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

            if (cart == null)
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

        [HttpGet]
        public HttpResponseMessage GetMenuBadgeCount()
        {
            var products_count = db.Products.Count();

            var clients_count = db.Clients.Count();

            int reports_count = 0;

            var result = new
            {
                products_count = products_count,
                clients_count = clients_count,
                reports_count = reports_count
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