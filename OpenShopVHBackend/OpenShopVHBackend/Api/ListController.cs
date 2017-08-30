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
using OpenShopVHBackend.Models;
using OpenShopVHBackend.Models.Helper;
using Newtonsoft.Json;
using Hangfire;
using OpenShopVHBackend.BussinessLogic;
using System.Web.Script.Serialization;
using System.Text;
using OpenShopVHBackend.Controllers;
using OpenShopVHBackend.BussinessLogic.SAP;
using System.Data.Entity.Core.Objects;
using System.Globalization;

namespace OpenShopVHBackend.Api
{
    public class ListController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private String default_currency = "L";

        [HttpGet]
        public HttpResponseMessage GetInvoiceHistory(int userId, String search = "")
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {

                var invoices = db.InvoiceHistory
                    .Where(w => w.DocNum.ToLower().Contains(search) ||
                            w.CardCode.ToLower().Contains(search) ||
                            w.CardName.ToLower().Contains(search))
                    .ToList()
                    .Take(100)
                    .Select(s => new
                    {
                        id = s.InvoiceId,
                        doc_num = s.DocNum,
                        card_code = s.CardCode,
                        card_name = s.CardName,
                        total = s.Total
                    });

                var result = new
                {
                    invoices = invoices
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage MoveToWishList(int userId)
        {
            var cart = db.Carts
                .Where(w => w.DeviceUserId == userId 
                        && w.Type == 0)
                .ToList()
                .FirstOrDefault();

            if(cart != null && cart.CartProductItems.Count > 0)
            {
                //foreach (var item in cart.CartProductItems)
                //{
                //    var i = new CartProductItem()
                //    {
                //        CartId
                //    }
                //}
            }          

            return Request.CreateResponse(HttpStatusCode.OK, new object(), Configuration.Formatters.JsonFormatter);
        }
        //GetClientTransactions

        [HttpGet]
        public HttpResponseMessage AddToWishList(int userId, int variantId)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {

                var variant = db.ProductVariants
                .Where(w => w.ProductVariantId == variantId)
                .ToList()
                .FirstOrDefault();

                if (variant == null)
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new object(), Configuration.Formatters.JsonFormatter);

                var wishListVariant = new WishlistProductVariant()
                {
                    ProductId = variant.ProductId,
                    Name = variant.Product.Name,
                    Currency = variant.Currency,
                    Description = variant.Product.Description,
                    MainImage = variant.Product.MainImage,
                    Price = variant.Price,
                    Code = variant.Code,
                    CategoryId = variant.Product.CategoryId,
                    PriceFormatted = variant.GetPriceTotalFormated(),
                };

                db.WishlistProductVariant.Add(wishListVariant);
                db.SaveChanges();

                var wishlist = new WishlistItem()
                {
                    DeviceUserId = userId,
                    WishlistProductVariantId = wishListVariant.WishlistProductVariantId
                };

                db.WishlistItem.Add(wishlist);
                db.SaveChanges();

                var result = new
                {
                    success = true
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetWishlist(int userId)
        {
            var list = db.WishlistItem
                .Include(i => i.WishlistProductVariant)
                .Where(w => w.DeviceUserId == userId)
                .ToList()
                .Select(s => new {
                    user_id = s.DeviceUserId,
                    id = s.WishlistItemId,
                    variant = new {
                        category = s.WishlistProductVariant.CategoryId,
                        code = s.WishlistProductVariant.Code,
                        currency = s.WishlistProductVariant.Currency,
                        description = s.WishlistProductVariant.Description,
                        discount_price = s.WishlistProductVariant.DiscountPrice,
                        discount_price_formatted = s.WishlistProductVariant.DiscountPriceFormatted,
                        main_image = s.WishlistProductVariant.MainImage,
                        main_image_high_res = s.WishlistProductVariant.MainImageHighRes,
                        name = s.WishlistProductVariant.Name,
                        price = s.WishlistProductVariant.Price,
                        price_formatted = s.WishlistProductVariant.PriceFormatted,
                        product_id = s.WishlistProductVariant.ProductId
                    }                  
                });

            var records = new
            {
                id = 0,
                product_count = list.Count(),
                items = list
            };

            return Request.CreateResponse(HttpStatusCode.OK, records, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public HttpResponseMessage GetReportQuota(int userId, int year, int month)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var quotas = db.Fees
                .Where(w => w.DeviceUserId == userId
                       && w.Date.Year == year
                       && w.Date.Month == month)
                .GroupBy(g => g.DeviceUserId)
                .Select(s => new ReportEntry()
                {
                    Y = s.Sum(c => c.Amount),
                    Label = "Cuota"
                }).ToList()
                .FirstOrDefault();

                var orders = db.Orders
                    .Where(w => w.DeviceUserId == userId
                          && w.CreatedDate.Year == year
                          && w.CreatedDate.Month == month)
                    .GroupBy(g => g.DeviceUserId)
                    .ToList()
                    .Select(s => new ReportEntry()
                    {
                        Y = s.Sum(c => c.GetTotal()),
                        Label = "Ventas"
                    }).ToList()
                    .FirstOrDefault();

                if (quotas != null && orders != null)
                    quotas.Y = quotas.Y - orders.Y;

                List<ReportEntry> entries = new List<ReportEntry>();

                if (quotas != null)
                    entries.Add(quotas);
                if (orders != null)
                    entries.Add(orders);

                var records = new
                {
                    totalinvoiced = orders != null ? orders.Y : 0,
                    quota = quotas != null ? quotas.Y : 0,
                    entries = entries
                };

                return Request.CreateResponse(HttpStatusCode.OK, records, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetReportQuotaAccum(int userId, int year, int month, int day)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {

                DateTime currentDate = new DateTime(year, month, day);
                int days = currentDate.DayOfWeek - DayOfWeek.Monday;
                DateTime weekStart = currentDate.AddDays(-days);
                DateTime weekEnd = weekStart.AddDays(6);

                var quotas = db.Fees
                    .Where(w => w.DeviceUserId == userId
                           && w.Date >= weekStart && w.Date <= weekEnd)
                    .GroupBy(g => g.Date.Day)
                    .ToList()
                    .Select(s => new ReportEntry()
                    {
                        X = s.Key,
                        Y = s.Sum(c => c.Amount),
                        Label = s.Key.ToString()
                    }).ToList();

                var orders = db.Orders
                    .Where(w => w.DeviceUserId == userId
                           && w.CreatedDate >= weekStart && w.CreatedDate <= weekEnd)
                    .GroupBy(g => g.CreatedDate.Day)
                    .ToList()
                    .Select(s => new ReportEntry()
                    {
                        X = s.Key,
                        Y = s.Sum(c => c.GetTotal()),
                        Label = s.Key.ToString()
                    }).ToList();

                var records = new
                {
                    quotaaccum = quotas != null ? quotas.Sum(s => s.Y) : 0,
                    totalinvoiced = orders != null ? orders.Sum(s => s.Y) : 0,
                    firstlist = quotas,
                    secondlist = orders
                };

                return Request.CreateResponse(HttpStatusCode.OK, records, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetClientTransactions(int userId, String cardcode, DateTime begin, DateTime end)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                end = end.AddDays(1);

                var transactions = db.ClientTransactions
                .Where(w => w.CardCode == cardcode
                       && w.CreatedDate >= begin && w.CreatedDate <= end)
                 .OrderBy(o => o.CreatedDate)
                .ToList()
                .Select(s => new
                {
                    id = s.ClientTransactionId,
                    date = s.CreatedDate.ToShortDateString(),
                    description = s.Description,
                    reference_number = s.ReferenceNumber,
                    card_code = s.CardCode,
                    amount = s.Amount
                });

                var records = new
                {
                    transactions = transactions
                };

                return Request.CreateResponse(HttpStatusCode.OK, records, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage CancelPayment(int userId, int id)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var payment = db.Payments
                    .Where(w => w.PaymentId == id)
                    .ToList()
                    .FirstOrDefault();

                if (payment != null)
                {
                    payment.Status = PaymentStatus.Canceled;
                    db.Entry(payment).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var result = new
                {
                    success = true
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage SentPayment(int userId, int id)
        {
            bool success = true;

            BackgroundJob.Enqueue(() => CreatePaymentOnSAP(userId, id));

            return Request.CreateResponse(HttpStatusCode.OK, success, Configuration.Formatters.JsonFormatter);        
        }

        [HttpGet]
        public HttpResponseMessage GetPayments(int userId, DateTime begin, DateTime end)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                end = end.AddDays(1);

                var payments = db.Payments
                    .Include(i => i.Client)
                    .Include(i => i.Client.Invoices)
                    .Include(i => i.Cash)
                    .Include(i => i.Transfer)
                    .Include(i => i.Checks)
                    .Include(i => i.Invoices)
                    .Where(w => w.DeviceUserId == userId
                        && w.CreatedDate >= begin && w.CreatedDate <= end)
                    .OrderByDescending(o => o.PaymentId)
                    .ToList()
                    .Select(s => new
                    {
                        id = s.PaymentId,
                        userId = s.DeviceUserId,
                        doc_entry = s.DocEntry,
                        last_error = s.LastErrorMessage,
                        status = s.Status,
                        status_text = s.Status.ToString(),
                        created_date = s.CreatedDate.ToString(),
                        comment = s.Comment,
                        client = new
                        {
                            name = s.Client.Name,
                            card_code = s.Client.CardCode,
                            balance = s.Client.Balance,
                            RTN = s.Client.RTN,
                            invoices = s.Client.Invoices
                            .OrderBy(o => o.CreatedDate)
                            .ToList()
                            .Select(d => new
                            {
                                document_code = d.DocumentCode,
                                created_date = d.CreatedDate,
                                doc_entry = d.DocEntry,
                                dueDate = d.DueDate,
                                total_amount = d.TotalAmount,
                                payed_amount = d.PayedAmount,
                                balance_due = d.BalanceDue,
                                overdue_days = d.OverdueDays
                            })
                        },
                        total = s.TotalAmount,
                        cash = new
                        {
                            amount = s.Cash.Amount,
                            account = s.Cash.GeneralAccount
                        },
                        transfer = new
                        {
                            amount = s.Transfer.Amount,
                            account = s.Transfer.GeneralAccount,
                            number = s.Transfer.ReferenceNumber,
                            due_date = s.Transfer.Date,
                            bank = new
                            {
                                general_account = s.Transfer.GeneralAccount
                            }
                        },
                        checks = s.Checks
                        .ToList()
                        .Select(c => new
                        {
                            amount = c.Amount,
                            general_account = c.GeneralAccount,
                            due_date = c.DueDate,
                            bank = c.Bank.Name
                        }),
                        invoices = s.Invoices
                        .ToList()
                        .Select(i => new
                        {
                            document_number = i.DocumentNumber,
                            total_amount = i.TotalAmount,
                            total_payed = i.PayedAmount,
                            doc_entry = i.DocEntry
                        })
                    });

                var records = new
                {
                    payments = payments
                };

                return Request.CreateResponse(HttpStatusCode.OK, records, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage Payment(int userId, int id)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var payment = db.Payments
                    .Include(i => i.Client)
                    .Include(i => i.Cash)
                    .Include(i => i.Transfer)
                    .Include(i => i.Checks)
                    .Include(i => i.Invoices)
                .Where(w => w.PaymentId == id)
                .ToList()
                .Select(s => new
                {
                    id = s.PaymentId,
                    doc_entry = s.DocEntry,
                    client = new
                    {
                        name = s.Client.Name,
                        cardcode = s.Client.CardCode,
                        balance = s.Client.Balance
                    },
                    total = s.TotalAmount,
                    cash = new
                    {
                        amount = s.Cash.Amount,
                        account = s.Cash.GeneralAccount
                    },
                    transfer = new
                    {
                        amount = s.Transfer.Amount,
                        account = s.Transfer.GeneralAccount,
                        reference_number = s.Transfer.ReferenceNumber,
                        date = s.Transfer.Date
                    },
                    checks = s.Checks
                    .ToList()
                    .Select(c => new
                    {
                        amount = c.Amount,
                        general_account = c.GeneralAccount,
                        due_date = c.DueDate,
                        bank = c.Bank.Name
                    }),
                    invoices = s.Invoices
                    .Where(w => w.PaymentId == s.PaymentId)
                    .ToList()
                    .Select(i => new
                    {
                        document_code = i.DocumentNumber,
                        total_amount = i.TotalAmount,
                        total_payed = i.PayedAmount,
                        doc_entry = i.DocEntry
                    })
                });

                return Request.CreateResponse(HttpStatusCode.OK, payment, Configuration.Formatters.JsonFormatter);
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void CreatePaymentOnSAP(int userId, int paymentId)
        {
            DraftPayment draft = new DraftPayment();
            draft.MakePayment(userId, paymentId);
        }

        [HttpGet]
        [HttpPut]
        public HttpResponseMessage AddPayment(Int32 userId, Int32 clientId, Double totalPaid, String comment, String cash, String transfer = "", String checks = "", String invoices = "")
        {

            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                bool success = true;

                List<Check> myChecks;
                Transfer myTransfer;
                Cash myCash;
                List<InvoiceItem> invoicesItems;
                int cashId = 0, transferId = 0;
                int paymentId = 0;

                try
                {
                    if (cash.Count() > 0)
                    {
                        myCash = JsonConvert.DeserializeObject<Cash>(cash);
                        db.Cash.Add(myCash);
                        db.SaveChanges();
                        cashId = myCash.CashId;
                    }

                    if (transfer.Count() > 0)
                    {
                        myTransfer = JsonConvert.DeserializeObject<Transfer>(transfer);
                        db.Transfers.Add(myTransfer);
                        db.SaveChanges();
                        transferId = myTransfer.TransferId;
                    }

                    var myPayment = new Payment()
                    {
                        TotalAmount = totalPaid,
                        CashId = cashId,
                        TransferId = transferId,
                        DeviceUserId = userId,
                        ClientId = clientId,
                        CreatedDate = DateTime.Now,
                        Status = PaymentStatus.CreadoEnAplicacion,
                        DocEntry = "",
                        Comment = comment
                    };

                    db.Payments.Add(myPayment);
                    db.SaveChanges();

                    paymentId = myPayment.PaymentId;


                    if (checks.Count() > 0)
                    {
                        myChecks = JsonConvert.DeserializeObject<List<Check>>(checks);

                        if (myChecks != null)
                        {
                            foreach (var item in myChecks)
                            {
                                item.PaymentId = paymentId;
                                db.Checks.Add(item);
                            }
                            db.SaveChanges();
                        }
                    }

                    if (invoices.Count() > 0)
                    {
                        invoicesItems = JsonConvert.DeserializeObject<List<InvoiceItem>>(invoices);

                        if (invoicesItems != null)
                        {
                            foreach (var item in invoicesItems)
                            {
                                item.PaymentId = paymentId;
                                db.Invoices.Add(item);
                            }
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    MyLogger.GetInstance.Error(e.Message);
                }

                var result = new
                {
                    success = success
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        [HttpPut]
        public HttpResponseMessage SentPayment(Int32 userId, Int32 clientId, Double totalPaid, String comment, String cash, String transfer = "", String checks = "", String invoices = "", Int32 paymentId = 0)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                bool success = true;

                List<Check> myChecks;
                Transfer myTransfer;
                Cash myCash;
                List<InvoiceItem> invoicesItems;

                var payment = db.Payments
                    .Include(i => i.Cash).Include(i => i.Transfer).Include(i => i.Checks)
                    .Where(w => w.PaymentId == paymentId)
                    .ToList()
                    .FirstOrDefault();

                try
                {

                    if (cash.Count() > 0)
                    {
                        myCash = JsonConvert.DeserializeObject<Cash>(cash);
                        payment.Cash.Amount = myCash.Amount;
                    }

                    if (transfer.Count() > 0)
                    {
                        myTransfer = JsonConvert.DeserializeObject<Transfer>(transfer);
                        payment.Transfer.Amount = myTransfer.Amount;
                        payment.Transfer.Date = myTransfer.Date;
                        payment.Transfer.GeneralAccount = myTransfer.GeneralAccount;
                        payment.Transfer.ReferenceNumber = myTransfer.ReferenceNumber;
                    }

                    if (checks.Count() > 0)
                    {
                        myChecks = JsonConvert.DeserializeObject<List<Check>>(checks);
                        payment.Checks.RemoveRange(0, payment.Checks.Count);

                        if (myChecks != null)
                        {
                            foreach (var item in myChecks)
                            {
                                item.PaymentId = payment.PaymentId;
                                db.Checks.Add(item);
                            }
                            db.SaveChanges();
                        }
                    }

                    if (invoices.Count() > 0)
                    {
                        payment.Invoices.RemoveRange(0, payment.Invoices.Count);
                        db.SaveChanges();
                        invoicesItems = JsonConvert.DeserializeObject<List<InvoiceItem>>(invoices);

                        if (invoicesItems != null)
                        {
                            foreach (var item in invoicesItems)
                            {
                                item.PaymentId = payment.PaymentId;
                                db.Invoices.Add(item);
                            }
                        }

                        db.SaveChanges();
                    }

                    payment.CreatedDate = DateTime.Now;
                    payment.Comment = comment;
                    db.Entry(payment).State = EntityState.Modified;

                    if (db.SaveChanges() > 0)
                    {
                        BackgroundJob.Enqueue(() => CreatePaymentOnSAP(userId, paymentId));
                    }
                }
                catch (Exception e)
                {
                    success = false;
                    MyLogger.GetInstance.Error(e.Message);

                    var result2 = new
                    {
                        succes = false,
                        message = e.Message
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, result2, Configuration.Formatters.JsonFormatter);
                }

                var result = new
                {
                    success = success
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage Users(int userId)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            var users = db.DeviceUser
                    .Where(w => w.Shop.ConnectionString == user.Shop.ConnectionString)
                    .ToList()
                    .Select(s => new
                    {
                        id = s.DeviceUserId,
                        sales_person_id = s.SalesPersonId,
                        name = s.Name
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
                success = false;
                message = e.Message;
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = success }, Configuration.Formatters.JsonFormatter);
            }           
        }


        [HttpGet]
        public HttpResponseMessage GetUser(int id)
        {
            var user = db.DeviceUser
                  .Include( i => i.Shop )
                  .Where(w => w.DeviceUserId == id)
                  .ToList()
                  .Select(s => new
                  {
                      id = s.DeviceUserId,
                      access_token = s.AccessToken,
                      sales_person_id = s.SalesPersonId,
                      name = s.Name,
                      email = s.Username,
                      print_bluetooth_address = s.PrintBluetoothAddress,
                      country = s.Shop.Name
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
                      print_bluetooth_address = s.PrintBluetoothAddress,
                      country = s.Shop.Name
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

        // GET: api/List/GetNavigations
        public HttpResponseMessage GetNavigations(int userId, int shop = -1)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
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
                    .Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        original_id = s.RemoteId,
                        type = s.Type,
                        children = GetChildrens(categoryModels, s.Id)
                    }).OrderBy(o => o.id);

                var result = new { navigation = categories };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
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
        public HttpResponseMessage GetBanners(int userId)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
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
        }

        [HttpGet]
        public HttpResponseMessage GetDevices(int userId)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
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
        }

        [HttpPost]
        public HttpResponseMessage SetDevice(String message)
        {

            //return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        //TODO: fix search related products
        public HttpResponseMessage GetProduct(int userId, int id, string include = "")
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var product = db.Products.AsQueryable();
                product = product.Where(w => w.ProductId == id);

                //Fix lazy loading MVC
                //db.Entry(product).Collection("Variants").Load();

                var result = product
                    .Include(i => i.Variants.Select(s => s.Color))
                    .Include(i => i.Variants.Select(s => s.Size))
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
        }

        [HttpGet]
        public HttpResponseMessage GetProducts(int userId, int category = -1, string sort = "", string search = "", string brand = "")
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var result = db.Products.AsQueryable();

                if (category > 0)
                    result = result.Where(w => w.Category.RemoteId == category);
                if (search.Count() > 0)
                {
                    result = result.Where(w => w.Code.Contains(search.ToUpper())
                            || w.Brand.Name.Contains(search.ToUpper())
                            || w.Name.ToUpper().Contains(search.ToUpper())
                    );
                }
                if (brand.Count() > 0)
                {
                    int brandId = Int32.Parse(brand);
                    result = result.Where(w => w.BrandId.Equals(brandId));
                }

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
                    .Include(i => i.Category)
                    .Include(i => i.Brand)
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
                        sorting = "newest",
                        filters = generateFilters(userId)
                    },
                    records = products
                };

                return Request.CreateResponse(HttpStatusCode.OK, respond, Configuration.Formatters.JsonFormatter);
            }
        }

        //TODO: fix multi country fixed numbers
        public List<FilterType> generateFilters(int userId)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                List<FilterType> list = new List<FilterType>();

                var categories = db.Categories
                    .Where(w => w.Type == "category")
                    //.Where(w => w.PartentId == 0 && w.Id > 0)
                    .ToList()
                    .Select(s => new
                    {
                        id = s.Id,
                        value = s.Name,
                        parent = s.PartentId
                    });

                var subcategories = db.Categories
                    .Where(w => w.Type == "subcategory")
                   //.Where(w => w.Id >= 101 && w.Id <= 9999)
                   .ToList()
                   .Select(s => new
                   {
                       id = s.Id,
                       value = s.Name,
                       parent = s.PartentId
                   });

                var familia = db.Categories
                    .Where(w => w.Type == "family")
                  //.Where(w => w.Id >= 10101 && w.Id <= 999999)
                  .ToList()
                  .Select(s => new
                  {
                      id = s.Id,
                      value = s.Name,
                      parent = s.PartentId
                  });

                var subfamily = db.Categories
                    .Where(w => w.Type == "subfamily")
                  //.Where(w => w.Id >= 1010101 && w.Id <= 99999999)
                  .ToList()
                  .Select(s => new
                  {
                      id = s.RemoteId,
                      value = s.Name,
                      parent = s.PartentId
                  });


                list.Add(new FilterType()
                {

                    id = 1,
                    name = "Categoria",
                    label = "category",
                    type = "select",
                    values = categories
                });

                list.Add(new FilterType()
                {

                    id = 2,
                    name = "SubCategoria",
                    label = "subcategory",
                    type = "select",
                    values = subcategories
                });

                list.Add(new FilterType()
                {

                    id = 3,
                    name = "Familia",
                    label = "family",
                    type = "select",
                    values = familia
                });

                list.Add(new FilterType()
                {

                    id = 4,
                    name = "SubFamily",
                    label = "subfamily",
                    type = "select",
                    values = subfamily
                });


                var brands = db.Brands
                  .ToList()
                  .Select(s => new
                  {
                      id = s.BrandId,
                      value = s.Name
                  });


                list.Add(new FilterType()
                {

                    id = 5,
                    name = "Marca",
                    label = "brand",
                    type = "select",
                    values = brands
                });

                return list;

            }
        }

        [HttpGet]
        public HttpResponseMessage GetBanks(int userId)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var banks = db.Banks
                .ToList()
                .Select(s => new
                {
                    id = s.BankId,
                    name = s.Name + " | " + s.FormatCode,
                    general_account = s.GeneralAccount
                });

                var result = new
                {
                    banks = banks
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage Client(int userId, string cardcode)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var clients = db.Clients
                    .Include(i => i.Invoices)
                    .Where(w => w.CardCode == cardcode)
                    .ToList()
                    .Select(s => new
                    {
                        id = s.ClientId,
                        name = s.Name,
                        card_code = s.CardCode,
                        phone = s.PhoneNumber,
                        RTN = s.RTN,
                        invoices = s.Invoices
                            .ToList()
                            .Select(d => new
                            {
                                document_code = d.DocumentCode,
                                created_date = d.CreatedDate,
                                doc_entry = d.DocEntry,
                                dueDate = d.DueDate,
                                total_amount = d.TotalAmount,
                                payed_amount = d.PayedAmount,
                                balance_due = d.BalanceDue,
                                overdue_days = d.OverdueDays
                            })
                    })
                    .FirstOrDefault();

                return Request.CreateResponse(HttpStatusCode.OK, clients, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetClients(int userId, String search = "")
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var clients = db.Clients.AsQueryable();

                if (search.Count() > 0)
                    clients = clients.Where(w => w.Name.ToLower().Contains(search.ToLower())
                    || w.CardCode.ToLower().Contains(search.ToLower()));

                clients = clients.Take(30);

                var result = clients
                    .Include(i => i.Invoices)
                    .ToList()
                    .Select(s => new
                    {
                        id = s.ClientId,
                        card_code = s.CardCode,
                        phone = s.PhoneNumber,
                        address = s.Address,
                        name = s.Name,
                        RTN = s.RTN,
                        in_orders = s.PastDue,
                        invoices = s.Invoices
                            .OrderBy(o => o.DueDate)
                            .ToList()
                            .Select(d => new
                            {
                                document_code = d.DocumentCode,
                                created_date = d.CreatedDate,
                                dueDate = d.DueDate,
                                doc_entry = d.DocEntry,
                                total_amount = d.TotalAmount,
                                payed_amount = d.PayedAmount,
                                balance_due = d.BalanceDue,
                                overdue_days = d.OverdueDays
                            })
                    });

                var records = new { records = result };

                return Request.CreateResponse(HttpStatusCode.OK, records, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetDocuments(int userId, string card_code = "")
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var client = db.Clients
                    .Where(w => w.CardCode == card_code)
                    .ToList()
                    .FirstOrDefault();

                var documents = db.Documents
                    .Where(w => w.Client.CardCode == card_code)
                    .OrderBy(o => o.DueDate)
                    .ToList()
                    .Select(s => new
                    {
                        id = s.DocumentId,
                        document_code = s.DocumentCode,
                        created_date = s.CreatedDate,
                        due_date = s.DueDate,
                        total_amount = s.TotalAmount,
                        doc_entry = s.DocEntry,
                        payed_amount = s.PayedAmount,
                        balance_due = s.BalanceDue,
                        overdue_days = s.OverdueDays
                    });

                var result = new
                {
                    address = client.Address,
                    client_card_code = client.CardCode,
                    client_name = client.Name,
                    credit_limit = client.CreditLimit,
                    balance = client.Balance,
                    in_orders = client.PastDue,
                    pay_condition = client.PayCondition,
                    records = documents
                };

                return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        [HttpDelete]
        public HttpResponseMessage DeleteToCart(int userId, int id, int type)
        {
            bool success = true;
            String message = "";

            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {

                var cart_item = db.CartProductItems
                    .Where(w => w.Cart.DeviceUserId == userId
                        && w.CartProductItemId == id
                        && w.Type == type)
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
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AddToCart(int userId = -1, int product_variant_id = -1, int quantity = 0, String cardcode = "", int type = 0)
        {
            bool success = false;

            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;
            double isv = user.Shop.ISV / 100;

            using (var db = new ApplicationDbContext(connection))
            {

                if (userId > 0 && product_variant_id > 0 && quantity > 0)
                {
                    var product_variant = db.ProductVariants
                        .Include(i => i.Product)
                        .Where(w => w.ProductVariantId == product_variant_id)
                        .ToList()
                        .FirstOrDefault();

                    var cart = db.Carts
                        .Where(w => w.DeviceUserId == userId
                                && w.Type == type)
                        .ToList()
                        .FirstOrDefault();

                    //create a new cart if not exist
                    if (cart == null && product_variant != null)
                    {
                        var new_cart = new Cart()
                        {
                            DeviceUserId = userId,
                            Currency = product_variant.Currency,
                            Type = type,
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
                            .Where(w => w.CardCode == cardcode 
                                && w.ItemGroup == product_variant.ItemGroup)
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
                            DiscountPercent = (discount != null ? discount.Discount : 0.0),
                            Discount = (discount != null ? (((product_variant.Price * quantity) * discount.Discount) / 100) : 0.0),
                            PriceFormatted = product_variant.GetPriceTotalFormated()
                        };

                        db.CartProductVariants.Add(cart_item_variant);
                        db.SaveChanges();

                        var discountvalue = (discount != null ? (((product_variant.Price * quantity) * discount.Discount) / 100) : 0.0);
                        var cart_item = new CartProductItem()
                        {
                            CartProductVariantId = cart_item_variant.CartProductVariantId,
                            CartId = cart.CartId,
                            Expiration = 0,
                            Quantity = quantity,
                            ISV = ((quantity * product_variant.Price) - discountvalue) * isv,
                            DiscountPercent = (discount != null ? discount.Discount : 0.0),
                            Discount = discountvalue,
                            RemoteId = product_variant.Product.RemoteId,
                            TotalItemPrice = (quantity * product_variant.Price),
                            TotalItemPriceFormatted = product_variant.Currency + ' ' + (quantity * product_variant.Price),
                            Type = type
                        };

                        var tmp = db.CartProductItems
                            .Include(i => i.CartProductVariant)
                            .Where(w => w.CartProductVariant.ProductVariantId == product_variant.ProductVariantId && w.CartId == cart.CartId)
                            .ToList()
                            .FirstOrDefault();

                        if (tmp == null)
                        {
                            db.CartProductItems.Add(cart_item);
                            db.SaveChanges();
                        }
                        else
                        {
                            tmp.Quantity += quantity;
                            double dvalue = (discount != null ? (((product_variant.Price * tmp.Quantity) * discount.Discount) / 100) : 0.0);
                            tmp.ISV = ((tmp.Quantity * product_variant.Price) - dvalue) * isv;
                            tmp.Discount = dvalue;
                            tmp.TotalItemPrice = (tmp.Quantity * product_variant.Price);
                            tmp.TotalItemPriceFormatted = product_variant.Currency + ' ' + tmp.TotalItemPrice;
                            db.Entry(tmp).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        MyLogger.GetInstance.Info(String.Format("AddToCart - userId: {0}, product_variant_id: {1}, quantity: {2}", userId, product_variant_id, quantity));

                        success = true;
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = success }, Configuration.Formatters.JsonFormatter);
            }
        }

        public HttpResponseMessage GetOrders(int userId, DateTime begin, DateTime end)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                end = end.AddDays(1);

                var orders = db.Orders
                    .Include(i => i.OrderItems)
                    .Include(i => i.DeviceUser)
                    .Include(i => i.Client)
                    .Where(w => w.DeviceUser.SalesPersonId == user.SalesPersonId
                           && w.CreatedDate >= begin && w.CreatedDate <= end)
                    .OrderByDescending(o => o.OrderId)
                    .ToList()
                        .Select(s => new
                        {
                            id = s.OrderId,
                            remote_id = s.RemoteId,
                            series = s.Series,
                            comment = s.Comment,
                            status = s.Status.ToString(),
                            date_created = s.DateCreated,
                            item_count = s.GetItemCount(),
                            subtotal = s.GetSubtotal(),
                            discount = s.GetDiscount(),
                            IVA = s.GetIVA(),
                            total = s.GetTotal(),
                            total_formatted = s.GetTotal(),
                            items = s.OrderItems
                                    .ToList()
                                    .Select(i => new
                                    {
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

                var myrecords = new
                {
                    metadata = new Object(),
                    records = orders
                };

                return Request.CreateResponse(HttpStatusCode.OK, myrecords, Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpGet]
        public HttpResponseMessage Order(int userId, int id)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var order = db.Orders
                    .Include(i => i.DeviceUser)
                    .Include(i => i.Client)
                    .Include(i => i.OrderItems)
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
                    total_formatted = order.GetTotal(),
                    items = order.OrderItems
                                    .ToList()
                                    .Select(i => new
                                    {
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
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage ReCreateOrder(int userId, int orderId)
        {
            bool success = true;

            if (orderId > 0)
            {
                BackgroundJob.Enqueue(() => CreateDraftSalesOrderOnSap(userId, orderId));
            }

            return Request.CreateResponse(HttpStatusCode.OK, success, Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        [HttpPost]
        public HttpResponseMessage CreateOrder(int userId, int cartId, String jo)
        {
            bool success = true;
            string message = "";

            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                try
                {
                    OrderViewModel myOrder = JsonConvert.DeserializeObject<OrderViewModel>(jo);

                    var deviceuser = db.DeviceUser
                        .Where(w => w.DeviceUserId == userId)
                        .ToList()
                        .FirstOrDefault();

                    var cart = db.Carts
                        .Include(i => i.CartProductItems)
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
                            CreatedDate = DateTime.Now,
                            Series = myOrder.Series,
                            //RemoteId = order.RemoteId,
                            Comment = myOrder.Comment,
                            Status = OrderStatus.CreadoEnAplicacion,
                            DeviceUserId = userId,
                            ClientId = client.ClientId,
                            DeliveryDate = myOrder.DeliveryDate
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
                                TaxValue = user.Shop != null ? user.Shop.ISV : 0.0,
                                TaxCode = "IVA",
                                Discount = item.CartProductVariant.Discount,
                                DiscountPercent = item.CartProductVariant.DiscountPercent,
                                WarehouseCode = item.CartProductVariant.WareHouseCode
                            };

                            //This line update the product iscommited
                            item.CartProductVariant.ProductVariant.IsCommitted += item.Quantity;
                            db.Entry(item).State = EntityState.Modified;
                            db.OrderItems.Add(orderItem);
                        }
                        db.SaveChanges();

                        //TODO: Remove CartProductVariant
                        db.CartProductItems.RemoveRange(cartItems);
                        db.Carts.Remove(cart);
                        db.SaveChanges();

                        int id = order.OrderId;

                        BackgroundJob.Enqueue(() => CreateDraftSalesOrderOnSap(userId, id));

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
                            total_formatted = order.GetTotal(),
                            items = order.OrderItems
                                    .ToList()
                                    .Select(i => new
                                    {
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
        }

        [AutomaticRetry(Attempts = 0)]
        public void CreateQuotationOrderOnSap(int userId, int orderId)
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
        public void CreateDraftSalesOrderOnSap(int userId, int orderId)
        {
            //String message = "";
            try
            {
                PreliminarSalesOrder salesorder = new PreliminarSalesOrder();
                salesorder.AddSalesOrder(userId, orderId);
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


            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                if (user != null && productCartItemId > 0 && newQuantity > 0 && newProductVariantId > 0)
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
        }

        [HttpGet]
        public HttpResponseMessage CartInfo(int userId = -1)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
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
        }

        [HttpGet]
        public HttpResponseMessage Cart(int userId = -1, int type = 0)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                //fix lazy loading with .Include(i => i.CartProductItems.Select(s => s.CartProductVariant))
                var cart = db.Carts
                    .Include(i => i.CartProductItems.Select(s => s.CartProductVariant))
                    .Include(i => i.CartProductItems.Select(s => s.CartProductVariant.Size))
                    .Include(i => i.CartProductItems.Select(s => s.CartProductVariant.Color))
                    .Include(i => i.CartProductItems.Select(s => s.CartProductVariant.ProductVariant))
                    .Where(w => w.DeviceUserId == userId && w.Type == type)
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
                                code = p.CartProductVariant.Name,
                                description = p.CartProductVariant.Name,
                                main_image = p.CartProductVariant.MainImage,
                                warehouse_code = p.CartProductVariant.WareHouseCode,
                                color = new
                                {
                                    id = p.CartProductVariant.Color.ColorId,
                                    remote_id = p.CartProductVariant.Color.RemoteId,
                                    value = p.CartProductVariant.Color.Value,
                                    code = p.CartProductVariant.Color.Code,
                                    img = p.CartProductVariant.Color.Image
                                },
                                size = new
                                {
                                    id = p.CartProductVariant.Size.SizeId,
                                    remote_id = p.CartProductVariant.Size.RemoteId,
                                    value = p.CartProductVariant.Size.Value,
                                    description = p.CartProductVariant.Size.Description
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
        }

        [HttpGet]
        public HttpResponseMessage GetMenuBadgeCount(int userId)
        {
            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
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