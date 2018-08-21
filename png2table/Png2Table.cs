using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace png2table {

    internal class Png2Table {

        public string Convert(string imageFile, bool useColSpan = true, bool useStyleTag = false, bool antiAlias = true, string backgroundColor = "white") {
            var image = Image.FromFile(imageFile);

            Bitmap b = null;

            if (antiAlias) {
                b = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                var g = Graphics.FromImage(b);
                g.Clear(ColorTranslator.FromHtml(backgroundColor));
                g.DrawImage(image, 0, 0, image.Width, image.Height);

            } else {
                b = new Bitmap(image);
            }

            var sb = new StringBuilder();

            if (useStyleTag) {
                sb.AppendLine("<style>");
                sb.AppendLine(".png2table td { width: 1px; height: 1px; }");
                sb.AppendLine("</style>");
            }

            sb.AppendLine($"<table{(useStyleTag ? " class=\"png2table\"" : "")} cellspacing=\"0\" cellpadding=\"0\">");

            if (useColSpan) {

                for (int y = 0; y < image.Height; y++) {
                    sb.AppendLine("<tr>");

                    var sameColorCount = 1;
                    for (int x = 0; x < image.Width; x++) {
                        var thisC = b.GetPixel(x, y);
                        var nextC = x < image.Width - 1 ? b.GetPixel(x + 1, y) : Color.Empty;

                        if (nextC == thisC) {
                            sameColorCount++;
                            continue;
                        }

                        if (thisC != nextC) {
                            AddTdPixels(sb, !useStyleTag, sameColorCount, thisC);
                            sameColorCount = 1;
                        }
                    }

                    sb.AppendLine("</tr>");
                }

            } else {
                for (int y = 0; y < image.Height; y++) {
                    sb.AppendLine("<tr>");

                    for (int x = 0; x < image.Width; x++) {
                        AddTdPixels(sb, !useStyleTag, 1, b.GetPixel(x, y));
                    }

                    sb.AppendLine("</tr>");
                }
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        private void AddTdPixels(StringBuilder sb, bool useSizingAttributes, int numberOfPixels, Color c) {
            var bgColorString = c.A == 0 ? "" : $" bgcolor=\"{ColorTranslator.ToHtml(c)}\"";
            var sizingString = useSizingAttributes ? $" width=\"{numberOfPixels}\" height=\"1\"" : "";

            if (numberOfPixels > 1) {
                sb.AppendLine($"<td{sizingString} colspan=\"{numberOfPixels}\"{bgColorString}></td>");
            } else {
                sb.AppendLine($"<td{sizingString}{bgColorString}></td>");
            }
        }
    }
}