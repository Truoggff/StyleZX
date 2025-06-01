using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using StyleZX.Models;

namespace StyleZX.Controllers
{
    public class FashionAIController : Controller
    {
        private readonly Model1 db = new Model1();

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Ask(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return Json(new { reply = "Vui lòng nhập câu hỏi!" });

            string input = " " + userMessage.ToUpper().Trim() + " ";
            string inputLower = " " + userMessage.ToLower().Trim() + " ";



            var keywordResponses = new Dictionary<string[], string>
            {
                {
                    new[] { "xin chào", "hello", "hi", "good morning", "chào buổi sáng", "ngày mới" },
                    "👋 Chào bạn! StyleZX rất vui được hỗ trợ bạn. Bạn cần tư vấn gì về thời trang hôm nay? 😊"
                },
                {
                    new[] { "khuyến mãi", "giảm giá", "ưu đãi", "sale" },
                    "🎁 Hiện tại cửa hàng đang có nhiều chương trình khuyến mãi hấp dẫn! Bạn có thể truy cập trang chủ hoặc mục \"Khuyến mãi\" để xem chi tiết các ưu đãi đang diễn ra nhé!"
                },
                {
                    new[] { "có sẵn", "còn hàng", "phải đặt", "đặt trước" },
                    "📦 Các sản phẩm hiển thị trên website đều là hàng có sẵn trong kho. Bạn chỉ cần chọn và đặt hàng, không cần chờ đặt trước nhé!"
                },
                {
                    new[] { "bao lâu", "nhận được", "giao hàng trong", "mấy ngày" },
                    "🚚 Thời gian giao hàng thường từ 1-3 ngày nội thành, 3-5 ngày với tỉnh xa. Bạn sẽ nhận được mã vận đơn để theo dõi đơn hàng ngay sau khi đặt."
                },
                {
                    new[] { "toàn quốc", "giao hàng toàn quốc", "ship toàn quốc", "tỉnh khác" },
                    "✅ Bên mình có giao hàng toàn quốc nhé! Dù bạn ở tỉnh thành nào cũng có thể đặt hàng và nhận tại nhà."
                },
                {
                    new[] { "phí ship", "phí vận chuyển", "tiền giao hàng" },
                    "💸 Phí ship thường dao động từ 25.000đ - 35.000đ tùy khu vực. Đơn từ 500.000đ sẽ được *Miễn phí giao hàng toàn quốc!*"
                },
                {
                    new[] { "không vừa", "đổi size", "đổi hàng", "đổi đồ" },
                    "🔄 Bạn hoàn toàn có thể đổi sản phẩm nếu mặc không vừa. Vui lòng giữ sản phẩm còn mới và liên hệ shop trong vòng 3 ngày sau khi nhận hàng để được hỗ trợ đổi size nhé!"
                },
                {
                    new[] { "chính sách", "đổi trả", "trả hàng", "hoàn trả" },
                    "📋 Chính sách đổi trả của shop: hỗ trợ đổi size, màu hoặc sản phẩm lỗi trong vòng 3 ngày. Sản phẩm cần còn nguyên tag, chưa sử dụng và không bị hư hỏng."
                },
                {
                    new[] { "sản phẩm lỗi", "lỗi", "rách", "bị hư" },
                    "⚠️ Rất xin lỗi nếu bạn nhận được sản phẩm bị lỗi! Vui lòng chụp ảnh sản phẩm và liên hệ fanpage hoặc hotline để được đổi mới miễn phí nhé."
                },
                {
                    new[] { "đổi màu", "màu khác", "muốn màu khác" },
                    "🎨 Nếu bạn muốn đổi sang màu khác, shop hỗ trợ đổi trong 3 ngày với điều kiện sản phẩm còn mới, chưa sử dụng và còn hàng trong kho."
                },
                {
                    new[] { "liên hệ", "số điện thoại", "hotline", "inbox" },
                    "📞 Bạn có thể liên hệ với shop qua hotline: 0964.068.959, hoặc nhắn tin trực tiếp trên fanpage để được hỗ trợ nhanh chóng!"
                },
                {
                    new[] { "cửa hàng", "shop", "mua trực tiếp", "offline", "địa chỉ" },
                    "🏬 Hiện tại StyleZX vừa bán cả trực tuyến và trực tiếp tại của hàng. Địa Chỉ: Số nhà 68, Khu Xóm Giữa, Xã Ngô Xá, Huyện Cẩm Khê, Tỉnh Phú Thọ."
                },
                {
                    new[] { "outfit đi chơi", "cuối tuần", "đi chơi", "phối đồ cuối tuần" },
                    "✨ Gợi ý outfit đi chơi cuối tuần:\n- Áo phông hoặc áo polo phối cùng quần short hoặc quần jogger\n- Kết hợp với giày sneaker trắng và túi đeo chéo để tạo cảm giác năng động, trẻ trung\n- Nếu trời mát, có thể khoác thêm áo khoác gió mỏng nhé!"
                },
                {
                    new[] { "hot trend", "mốt năm nay", "xu hướng", "đang thịnh hành" },
                    "🔥 Hot trend hiện nay:\n- Áo croptop, áo 2 dây phối cùng váy chữ A hoặc quần jogger\n- Set áo sơ mi tay ngắn + quần jean đơn giản nhưng thời thượng\n- Phụ kiện như túi đeo vai mini và sneaker trắng rất được yêu thích năm nay!"
                },
                {
                    new[] { "set đồ tết", "mặc tết", "outfit tết", "tết mặc gì", "mặc gì dịp tết" },
                    "🎉 Gợi ý set đồ mặc Tết:\n- Nữ: Váy body hoặc váy suông dài phối cùng áo croptop hoặc áo sơ mi tay ngắn\n- Nam: Áo sơ mi dài tay hoặc polo + quần âu hoặc jean\n- Gợi ý màu đỏ, be, trắng để tạo cảm giác tươi mới, may mắn ngày Tết!"
                },
                {
                    new[] { "đi làm", "đi học", "công sở", "văn phòng" },
                    "👔 Gợi ý trang phục đi làm/đi học:\n- Áo sơ mi dài tay + quần âu (nam/nữ)\n- Áo polo mix quần tây hoặc jean\n- Nữ có thể phối váy suông và balo nhẹ để tạo nét thanh lịch"
                },
                {
                    new[] { "mùa hè", "nắng nóng", "mát mẻ" },
                    "☀️ Gợi ý đồ mặc mùa hè:\n- Áo phông, sơ mi cộc tay, áo 2 dây, croptop\n- Quần short, chân váy ngắn hoặc váy suông nhẹ\n- Giày sandal, sneaker và mũ rộng vành rất hợp cho đi chơi/mặc mát"
                },
                {
                    new[] { "mùa đông", "lạnh", "giữ ấm" },
                    "❄️ Gợi ý đồ mùa đông:\n- Áo len hoặc áo khoác dày\n- Quần jean hoặc jogger giữ nhiệt\n- Giày thể thao hoặc boot cổ ngắn, khăn choàng để vừa ấm vừa thời trang"
                },
                {
                    new[] { "cảm ơn", "thank you", "thanks", "cảm ơn bạn", "cảm ơn nhiều" },
                    "😊 Không có gì bạn nhé! Nếu cần hỗ trợ gì thêm cứ hỏi tôi nha!"
                },
                {
                    new[] { "đồng ý", "oke", "ok", "okie", "được", "đồng ý rồi","đúng","vâng","dạ" },
                    "👍 Vâng ạ!❤️"
                }

            };

            foreach (var pair in keywordResponses)
            {
                if (pair.Key.Any(k => inputLower.Contains(k)))
                {
                    return Json(new { reply = pair.Value });
                }
            }


            var heightMatch = Regex.Match(userMessage.ToLower(), @"(\d+(?:[.,]?\d+)?)(?:m|cm)");
            var weightMatch = Regex.Match(userMessage.ToLower(), @"(\d{1,3})\s?kg");

            if (heightMatch.Success && weightMatch.Success)
            {
                int heightCm = 0;
                string heightStr = heightMatch.Value.ToLower();

                if (heightStr.Contains("cm"))
                {
                    heightCm = int.Parse(Regex.Match(heightStr, @"\d{2,3}").Value);
                }
                else if (heightStr.Contains("m"))
                {
                    if (heightStr.Contains(".") || heightStr.Contains(","))
                    {
                        double meters = double.Parse(Regex.Match(heightStr, @"\d+[.,]?\d+").Value.Replace(',', '.'));
                        heightCm = (int)(meters * 100);
                    }
                    else
                    {
                        var mParts = Regex.Match(heightStr, @"(\d)(?:m)(\d{1,2})");
                        if (mParts.Success)
                        {
                            int meters = int.Parse(mParts.Groups[1].Value);
                            int cm = int.Parse(mParts.Groups[2].Value);
                            heightCm = meters * 100 + cm;
                        }
                    }
                }

                int weightKg = int.Parse(weightMatch.Groups[1].Value);

                string size = CalculateSize(heightCm, weightKg);

                string reply = $"✨ Với chiều cao {heightCm}cm và cân nặng {weightKg}kg, size phù hợp với bạn có thể là {size}.\n" +
                               $"Bạn có thể tham khảo các sản phẩm theo size này nhé! ❤️";

                return Json(new { reply });
            }


            var sizeList = db.ProductSizes.Select(s => s.NameSize.ToUpper()).ToList();
            var colorList = db.ProductColors.Select(c => c.ColorName.ToLower()).ToList();
            var categoryList = db.Categories.Select(c => c.Name.ToLower()).ToList();

            string matchedSize = sizeList
                .FirstOrDefault(size => Regex.IsMatch(input, $@"\bSIZE\s*{Regex.Escape(size)}\b|\b{Regex.Escape(size)}\b"));

            string matchedColor = colorList.FirstOrDefault(color => inputLower.Contains(" " + color + " "));
            string matchedCategory = categoryList.FirstOrDefault(cat => inputLower.Contains(" " + cat + " "));

            int? priceLimit = null;
            var priceMaxMatch = Regex.Match(userMessage, @"(?<number>\d{3,7})");
            if (priceMaxMatch.Success && (inputLower.Contains("dưới") || inputLower.Contains("<") || inputLower.Contains("<=")))
            {
                if (int.TryParse(priceMaxMatch.Groups["number"].Value, out int val))
                    priceLimit = val;
            }

            int? priceMin = null;
            var priceMinMatch = Regex.Match(userMessage, @"(?<number>\d{3,7})");
            if (priceMinMatch.Success && (inputLower.Contains("trên") || inputLower.Contains(">") || inputLower.Contains(">=")
                || inputLower.Contains("lớn hơn") || inputLower.Contains("cao hơn")))
            {
                if (int.TryParse(priceMinMatch.Groups["number"].Value, out int val))
                    priceMin = val;
            }


            bool hasFilter = !string.IsNullOrEmpty(matchedCategory)
                             || !string.IsNullOrEmpty(matchedSize)
                             || !string.IsNullOrEmpty(matchedColor)
                             || priceMin.HasValue
                             || priceLimit.HasValue;

            if (!hasFilter)
            {
                return Json(new
                {
                    reply = "🤖 Xin lỗi, tôi chưa hiểu câu hỏi của bạn. Bạn có thể hỏi về size, màu, giá hoặc chính sách của cửa hàng nhé!"
                });
            }


            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(matchedCategory))
                products = products.Where(p => p.Category.Name.ToLower() == matchedCategory);

            if (priceLimit.HasValue)
                products = products.Where(p => p.Price <= priceLimit.Value);

            if (priceMin.HasValue)
                products = products.Where(p => p.Price >= priceMin.Value);

            var productList = products.ToList();

            var variants = from p in productList
                           join v in db.ProductVariants on p.ProductId equals v.ProductId
                           where v.Quantity > 0
                           select new
                           {
                               ProductName = p.Name,
                               p.Price,
                               SizeName = v.ProductSize.NameSize.ToUpper(),
                               ColorName = v.ProductColor.ColorName.ToLower(),
                               v.Quantity
                           };

            if (!string.IsNullOrEmpty(matchedSize))
                variants = variants.Where(v => v.SizeName == matchedSize);

            if (!string.IsNullOrEmpty(matchedColor))
                variants = variants.Where(v => v.ColorName == matchedColor);

            var results = variants
                .GroupBy(v => new { v.ProductName, v.Price })
                .Select(g => new
                {
                    Name = g.Key.ProductName,
                    Price = g.Key.Price,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            if (results.Any())
            {
                var sb = new StringBuilder("📌 Các sản phẩm còn hàng");
                var conditions = new List<string>();
                if (!string.IsNullOrEmpty(matchedCategory))
                    conditions.Add($"danh mục {matchedCategory}");
                if (!string.IsNullOrEmpty(matchedSize))
                    conditions.Add($"size {matchedSize}");
                if (!string.IsNullOrEmpty(matchedColor))
                    conditions.Add($"màu {matchedColor}");
                if (priceMin.HasValue)
                    conditions.Add($"trên {priceMin.Value:N0}đ");
                if (priceLimit.HasValue)
                    conditions.Add($"dưới {priceLimit.Value:N0}đ");

                if (conditions.Any())
                    sb.Append(" " + string.Join(", ", conditions));

                sb.Append(":\n");
                foreach (var r in results)
                    sb.AppendLine($"~ {r.Name} ({r.Price:N0}đ) còn {r.TotalQuantity} sản phẩm");

                return Json(new { reply = sb.ToString().Replace("\r\n", "\n") });
            }
            else
            {
                var sb = new StringBuilder("❌ Không tìm thấy sản phẩm");
                var nf = new List<string>();
                if (!string.IsNullOrEmpty(matchedCategory))
                    nf.Add($"danh mục {matchedCategory}");
                if (!string.IsNullOrEmpty(matchedSize))
                    nf.Add($"size {matchedSize}");
                if (!string.IsNullOrEmpty(matchedColor))
                    nf.Add($"màu {matchedColor}");
                if (priceMin.HasValue)
                    nf.Add($"trên {priceMin.Value:N0}đ");
                if (priceLimit.HasValue)
                    nf.Add($"dưới {priceLimit.Value:N0}đ");

                if (nf.Any())
                    sb.Append(" " + string.Join(", ", nf));
                sb.Append(" còn hàng.");

                return Json(new { reply = sb.ToString() });
            }          
        }
        private string CalculateSize(int heightCm, int weightKg)
        {
            if (heightCm <= 155)
            {
                if (weightKg < 45) return "XS";
                if (weightKg <= 52) return "S";
                return "M";
            }
            else if (heightCm <= 165)
            {
                if (weightKg < 50) return "S";
                if (weightKg <= 58) return "M";
                return "L";
            }
            else if (heightCm <= 175)
            {
                if (weightKg < 60) return "M";
                if (weightKg <= 70) return "L";
                return "XL";
            }
            else if (heightCm <= 185)
            {
                if (weightKg < 70) return "L";
                if (weightKg <= 80) return "XL";
                return "XXL";
            }
            else
            {
                if (weightKg < 80) return "XL";
                if (weightKg <= 90) return "XXL";
                return "XXXL";
            }
        }


    }
}
