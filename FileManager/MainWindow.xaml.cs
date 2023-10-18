using CommunityToolkit.Mvvm.DependencyInjection;
using HtmlAgilityPack;
using ReadSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFindAndReplace_Click(object sender, RoutedEventArgs e)
        {
            string searchString = txtA.Text.Trim();
            string replaceString = txtB.Text.Trim();
            if (searchString == "" || replaceString == "") return;

            var folderName = txtFolder.Text;

            var files = Directory.GetFiles(folderName);

            int finded = 0;
            foreach ( var filePath in files)
            {
                if (File.Exists(filePath))
                {

                    string[] lines = File.ReadAllLines(filePath);

                    // Perform the find and replace operation
                    for (int i = 0; i < lines.Length; i++)
                    {
                        // Replace all occurrences of the search string with the replace string
                        lines[i] = lines[i].Replace(searchString, replaceString);
                        //Debug.Print(filePath +" :| "+i.ToString());
                        finded++;
                    }

                    // Write the modified lines back to the text file
                    File.WriteAllLines(filePath, lines);
                }
            }
            MessageBox.Show("Finished 변경된 갯수:" + finded.ToString());

        }

        private void btnHtmlToText_Click(object sender, RoutedEventArgs e)
        {
            var folderName = txtFolder.Text;

            var files = Directory.GetFiles(folderName);

            int finded = 0;
            foreach (var filePath in files)
            {
                if (File.Exists(filePath))
                {
                    // HTML 파일 로드
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(filePath);

                    StringWriter sw = new StringWriter();
                    ConvertTo(doc.DocumentNode, sw);
                    sw.Flush();
                    var temp = sw.ToString();
                }
            }
        }
        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }
        private static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;
                        case "br":
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }

        }
        /// <summary>
        /// HTML 에서 Text만 추출한다.
        /// </summary>
        /// <param name="Html"></param>
        /// <returns></returns>
        public string StripHtml(string Html)
        {
            string output = Html;

            output = System.Text.RegularExpressions.Regex.Replace(output, "<br>", Environment.NewLine);
            output = System.Text.RegularExpressions.Regex.Replace(output, "<br/>", Environment.NewLine);
            output = System.Text.RegularExpressions.Regex.Replace(output, "<br />", Environment.NewLine);

            //get rid of HTML tags
            output = System.Text.RegularExpressions.Regex.Replace(output, "<[^>]*>", string.Empty);
            //get rid of multiple blank lines
            output = System.Text.RegularExpressions.Regex.Replace(output, @"^\s*$\n", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);

            output = System.Text.RegularExpressions.Regex.Replace(output, "&nbsp;", " ");
            return output;
        }
    }
}
