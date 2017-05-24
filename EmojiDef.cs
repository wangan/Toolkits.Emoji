using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toolkits.Log;

namespace Toolkits.Emoji {
    public class EmojiDef {
        private static string _emojiLocation = $"{AppDomain.CurrentDomain.BaseDirectory}/emoji-ios/unicode/";
        private static Dictionary<string, Image> _cache = new Dictionary<string, Image>();
        public Regex Range = new Regex("[\ud83d\ude00]-[\ud83d\ude00]|[\u2700]-[\u2700]|[\ud83d\ude80]-[\ud83d\ude80]|[\u2460]-[\u2460]|[\ud83c\udd00]-[\ud83c\udd00]|[\ud83c\ude00]-[\ud83c\ude00]|[\u2000]-[\u2000]|[\u20d0]-[\u20d0]|[\u2100]-[\u2100]|[\u2190]-[\u2190]|[\u2300]-[\u2300]|[\u25a0]-[\u25a0]|[\u2600]-[\u2600]|[\u2900]-[\u2900]|[\u2b00]-[\u2b00]|[\u3000]-[\u3000]|[\u3200]-[\u3200]|[\ud83c\udc00]-[\ud83c\udc00]|[\ud83c\udf00]-[\ud83c\udf00]");
        /* 1. Emoticons ( 1F600-1F64F ) */
        /* 2. Dingbats ( 2700-27BF ) */
        /* 3. Transport and map symbols (1F680-1F6FF ) */
        /* 4. Enclosed characters ( 2460-24FF) + (1F100-1F1FF) + (1F200-1F2FF)*/
        /* 5. Uncategorized 
                (0080-00FF) Latin-1 Supplement
                (2000-206F) General Punctuation
                (0000-007F) Basic Latin 
                (20D0-20FF) Combining Diacritical Marks for Symbols
                (2100-214F) Letterlike Symbols
                (2190-21FF) Arrows
                (2300-23FF) Miscellaneous Technical
                (25A0-25FF) Geometric Shapes
                (2600-26FF) Miscellaneous Symbols
                (2900-297F) Supplemental Arrows-B
                (2B00-2BFF) Miscellaneous Symbols and Arrows
                (3000-303F) CJK Symbols and Punctuation
                (3200-32FF) Enclosed CJK Letters and Months
                (1F000-1F02F) Mahjong Tiles
                (1F300-1F5FF) Miscellaneous Symbols and Pictographs */

        public static Image GetEmoji(string emojiId, float scale) {
            Image image = GetEmoji(emojiId);
            if (null != image) {
                try {
                    var width = image.Width * scale;
                    var height = image.Height * scale;

                    var bitmap = new Bitmap((int)width, (int)height);
                    bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    using (var gfx = Graphics.FromImage(bitmap)) {
                        gfx.CompositingMode = CompositingMode.SourceCopy;
                        gfx.CompositingQuality = CompositingQuality.HighQuality;
                        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfx.SmoothingMode = SmoothingMode.HighQuality;
                        gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        var destRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        using (var wrapMode = new ImageAttributes()) {
                            wrapMode.SetWrapMode(WrapMode.Tile);
                            gfx.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    return bitmap;

                } catch (Exception ex) {
                    LogHelper.Instance.Error("[EmojiDef][GetEmoji]", ex);
                }
            } else {
                LogHelper.Instance.Info($"[EmojiDef][GetEmoji] : 未获取到Emoji - {emojiId} - {scale}");
            }

            return null;
        }

        public static Image GetEmoji(string emojiId) {
            Image emoji = null;
            try {
                var emojiImg = $"{_emojiLocation}/{emojiId}.png";
                if (File.Exists(emojiImg)) {
                    emoji = Bitmap.FromFile(emojiImg);
                }
            } catch (Exception ex) {
                LogHelper.Instance.Error($"[EmojiDef][GetEmoji][FromFile] :{emojiId}", ex);
            }

            return emoji;
        }
    }
}
