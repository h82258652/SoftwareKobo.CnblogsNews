using AngleSharp;
using AngleSharp.DOM;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace SoftwareKobo.CnblogsNews.Service
{
    public class NewsDetailService
    {
        public async static Task<string> DownloadNewsDetailHtml(Uri uri)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(uri);
            }
        }

        public static string GetTitle(IDocument document)
        {
            var titleNode = document.QuerySelector("b");
            return titleNode.InnerHtml;
        }

        public static IDocument ParseHtmlToDocument(string html)
        {
            return DocumentBuilder.Html(html);
        }

        public static void RenderBorder(StackPanel panel, INode node, StringBuilder textBuffer)
        {
            var border = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 5, 0, 5),
                Padding = new Thickness(5, 0, 5, 0)
            };
            var innerPanel = new StackPanel();
            border.Child = innerPanel;
            RenderNode(node, innerPanel, textBuffer);
            RenderText(innerPanel, textBuffer);
            panel.Children.Add(border);
        }

        public static void RenderImage(StackPanel panel, Uri imageUri)
        {
            var image = new Image()
            {
                Source = new BitmapImage(imageUri),
                Margin = new Thickness(0, 5, 0, 5)
            };
            panel.Children.Add(image);
        }

        public static UIElement RenderNewsDetail(IDocument document)
        {
            var newsDetailNode = document.QuerySelector(@"div[style=""line-height:1.6;""]");
            var panel = new StackPanel()
            {
                Margin = new Thickness(20, 0, 20, 10)
            };
            RenderNode(newsDetailNode, panel);
            return panel;
        }

        public static async void RenderNode(INode node, StackPanel panel, StringBuilder textBuffer)
        {
            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.NodeName == "div")
                {
                    RenderNode(childNode, panel, textBuffer);
                }
                else if (childNode.NodeName == "p")
                {
                    RenderNode(childNode, panel, textBuffer);
                    RenderText(panel, textBuffer);
                }
                else if (childNode.NodeName == "a")
                {
                    if (childNode.TextContent == "[图片]")
                    {
                        var href = ((IElement)childNode).GetAttribute("href");
                        RenderImage(panel, new Uri(href, UriKind.Absolute));
                    }
                    else
                    {
                        textBuffer.Append(childNode.TextContent);
                    }
                }
                else if (childNode.NodeName == "strong")
                {
                    RenderText(panel, textBuffer);
                    textBuffer.Append(childNode.TextContent);
                    RenderText(panel, textBuffer, true);
                }
                else if (childNode.NodeName == "blockquote")
                {
                    RenderBorder(panel, childNode, textBuffer);
                }
                else if (childNode.NodeName == "ol")
                {
                    var index = 1;
                    foreach (var olChildNode in childNode.ChildNodes)
                    {
                        if (olChildNode.NodeName == "li")
                        {
                            textBuffer.Append(index + ". " + olChildNode.TextContent);
                            RenderText(panel, textBuffer);
                            index++;
                        }
                        else if (olChildNode.NodeType == NodeType.Text
                            && olChildNode.TextContent.Replace("\r", string.Empty).Replace("\n", string.Empty).Length <= 0)
                        {
                            // Skip empty line break.
                        }
                        else
                        {
                            await new MessageDialog(olChildNode.NodeName, "unknow html tag under ol").ShowAsync();
                            Debugger.Break();
                        }
                    }
                }
                else if (childNode.NodeName == "ul")
                {
                    foreach (var ulChildNode in childNode.ChildNodes)
                    {
                        if (ulChildNode.NodeName == "li")
                        {
                            textBuffer.Append("● " + childNode.TextContent);
                            RenderText(panel, textBuffer);
                        }
                        else if (ulChildNode.NodeType == NodeType.Text && ulChildNode.TextContent.Replace("\r", string.Empty).Replace("\n", string.Empty).Length <= 0)
                        {
                            // Skip empty line break.
                        }
                        else
                        {
                            await new MessageDialog(ulChildNode.NodeName, "unkonw html tag under ul").ShowAsync();
                            Debugger.Break();
                        }
                    }
                }
                else if (childNode.NodeName == "br")
                {
                    RenderText(panel, textBuffer);
                }
                else if (childNode.NodeName == "center")
                {
                    RenderNode(childNode, panel);
                }
                else if (childNode.NodeType == NodeType.Text)
                {
                    textBuffer.Append(childNode.TextContent);
                }
                else
                {
                    await new MessageDialog(childNode.NodeName, "unknow html tag").ShowAsync();
                    Debugger.Break();
                }
            }
        }

        public static void RenderNode(INode node, StackPanel panel)
        {
            var textBuffer = new StringBuilder();
            RenderNode(node, panel, textBuffer);

            // 确保内容完全输出。
            RenderText(panel, textBuffer);
        }

        public static void RenderText(StackPanel panel, StringBuilder textBuffer, bool bold = false)
        {
            var text = textBuffer.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty);
            if (text.Length > 0)
            {
                var textBlock = new TextBlock()
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap
                };

                #region 是否粗体

                if (bold)
                {
                    textBlock.FontWeight = FontWeights.Bold;
                }

                #endregion 是否粗体

                #region 字体大小

                if (bold == false)
                {
                    object textStyleMediumFontSize;
                    if (Application.Current.Resources.TryGetValue("TextStyleMediumFontSize", out  textStyleMediumFontSize) == false)
                    {
                        textStyleMediumFontSize = 16;
                    }
                    textBlock.FontSize = (double)textStyleMediumFontSize;
                }
                else
                {
                    object textStyleLargeFontSize;
                    if (Application.Current.Resources.TryGetValue("TextStyleLargeFontSize", out textStyleLargeFontSize) == false)
                    {
                        textStyleLargeFontSize = 18.14;
                    }
                    textBlock.FontSize = (double)textStyleLargeFontSize;
                }

                #endregion 字体大小

                #region 边距

                if (bold == false)
                {
                    textBlock.Margin = new Thickness(0, 5, 0, 5);
                }
                else
                {
                    textBlock.Margin = new Thickness(0, 10, 0, 5);
                }

                #endregion 边距

                panel.Children.Add(textBlock);
            }
            textBuffer.Clear();
        }
    }
}