using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TewiBoard.Web.Models;
using TewiBoard.Web.Repositories;

namespace TewiBoard.Web.Controllers
{
    public class CardController : DbController
    {
        public static string bedUrl = "https://sm.ms/";
        public static string defaultTitle = "Untitled";
        public static string defaultNick = "Anonymous";
        public static string deletepwd = "deletepwdhere";

        public CardController(CardDb db, ILogger<DbController> logger) : base(db, logger) { }

        public IActionResult Index(long? pid, int? page)
        {
            if (pid == null)
            {
                return RedirectToAction("Index", "Timeline");
            }

            var result = GetCardsToPage(GetCardThread(pid.Value), ref page, out int maxpage);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;

            ViewBag.PrevText = CardDb.PrevText;

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm]CardWithImageModel cardwithfile)
        {
            // If card not exist
            if (cardwithfile == null)
            {
                return RedirectToAction("Index", "Timeline");
            }

            // If image type error
            if (cardwithfile.Image != null && !cardwithfile.Image.ContentType.StartsWith("image"))
            {
                return RedirectToAction("Index", "Timeline");
            }

            CardModel card = new CardModel(cardwithfile);

            // Get Genre
            card.Genre = Generator.GetGenreByModule(card.Module);
            // Check if no
            if (card.Genre == "NONONO!!")
            {
                return RedirectToAction("Index", "Timeline");
            }
            // Generate Post time
            card.PostTime = DateTime.Now;
            // Generate User Id
            card.UserId = Generator.GetUserId(Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            // Generate User Nick & title
            if (string.IsNullOrEmpty(card.Title) || string.IsNullOrWhiteSpace(card.Title))
            {
                card.Title = defaultTitle;
            }
            if (string.IsNullOrEmpty(card.UserNick) || string.IsNullOrWhiteSpace(card.UserNick))
            {
                card.UserNick = defaultNick;
            }

            // Check if op
            if (card.Content.Substring(0, 5) == "[key=")
            {
                string key;
                try
                {
                    key = card.Content.Substring(5, card.Content.IndexOf(']') - 5);

                    card.Content = card.Content.Substring(card.Content.IndexOf(']') + 1);
                    if (card.Content.StartsWith("\r\n"))
                    {
                        card.Content = card.Content.Substring(2);
                    }
                    else if (card.Content.StartsWith("\n"))
                    {
                        card.Content = card.Content.Substring(1);
                    }
                }
                catch (Exception)
                {
                    key = null;
                }

                if (Genres.OpKeys.Contains(key))
                {
                    card.IsRed = 1;
                }
                else
                {
                    card.IsRed = 0;
                }
            }

            _logger.LogInformation("\r\n-----------------------\r\n  NewCard Get Ready  \r\n-----------------------\r\n");

            // Check if img
            IFormFile image = cardwithfile.Image;
            if (image != null)
            {
                // Save file name
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                // Get file path
                var filePath = Path.Combine("wwwroot/img", filename);

                try
                {
                    // Cache img file to path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("\r\n-----------------------\r\n  ImageCopying Error!  \r\n-----------------------\r\n", e);
                }

                // Create new post request
                HttpClient http = new HttpClient();

                try
                {
                    // To avoid 403
                    http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                    // Base URL
                    http.BaseAddress = new Uri(bedUrl);
                    // Keep alive
                    http.DefaultRequestHeaders.Connection.Clear();
                    http.DefaultRequestHeaders.ConnectionClose = false;
                }
                catch (Exception e)
                {
                    _logger.LogError("\r\n-----------------------\r\n  Upload Image Request Error!  \r\n-----------------------\r\n", e);
                }

                // Post form body
                MultipartFormDataContent form = new MultipartFormDataContent();

                try
                {
                    // Add file to form
                    var imagefile = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                    imagefile.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"smfile\"",
                        FileName = "\"" + filename + "\""
                    };
                    form.Add(imagefile);
                }
                catch (Exception e)
                {
                    _logger.LogError("\r\n-----------------------\r\n  Add File To Post Form Error!  \r\n-----------------------\r\n", e);
                }


                // Get post response
                var response = await http.PostAsync("api/upload", form);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    _logger.LogError("\r\n-----------------------\r\n  Upload Response Error!  \r\n-----------------------\r\n", e);
                }

                // Dispose request
                http.Dispose();
                // Get response content to json
                string content = response.Content.ReadAsStringAsync().Result;

                try
                {
                    JObject json = JsonConvert.DeserializeObject(content) as JObject;
                    // Check status
                    if (json["code"].ToString() == "success")
                    {
                        card.ImageUrl = json["data"]["url"].ToString();
                    }
                    else
                    {
                        card.ImageUrl = null;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("\r\n-----------------------\r\n  Get Image Url Error!  \r\n-----------------------\r\n", e);
                }

                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception e)
                {
                    _logger.LogError("\r\n-----------------------\r\n  Image Delete Error!  \r\n-----------------------\r\n", e);
                }

            }
            // Add new card
            Cards.Add(card);

            // Check reply top
            if (card.ReplyTop == null)
            {
                Cards.UpdateReplyTop(card);
            }

            return RedirectToAction("Index", "Card", new
            {
                pid = card.Pid
            });
        }

        public IActionResult Delete(long pid, string pwd)
        {
            if (pwd != deletepwd)
            {
                return RedirectToAction("Index", "Timeline");
            }

            Cards.Delete(pid);

            return Json(new
            {
                Status = "Success",
                pid
            });
        }
    }
}