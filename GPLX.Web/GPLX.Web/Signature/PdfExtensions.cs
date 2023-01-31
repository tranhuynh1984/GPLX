using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper.Internal;
using iTextSharp.text.pdf.parser;

namespace GPLX.Web.Signature
{
    public class LocationTextExtractionStrategyWithPosition : LocationTextExtractionStrategy
    {
        private readonly List<TextChunk> _locationalResult = new List<TextChunk>();

        private readonly ITextChunkLocationStrategy _tclStrategy;

        public LocationTextExtractionStrategyWithPosition() : this(new TextChunkLocationStrategyDefaultImp())
        {
        }

        /**
         * Creates a new text extraction renderer, with a custom strategy for
         * creating new TextChunkLocation objects based on the input of the
         * TextRenderInfo.
         * @param strategy the custom strategy
         */
        public LocationTextExtractionStrategyWithPosition(ITextChunkLocationStrategy strategy)
        {
            _tclStrategy = strategy;
        }


        private bool StartsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[0] == ' ';
        }


        private bool EndsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[str.Length - 1] == ' ';
        }

        /**
         * Filters the provided list with the provided filter
         * @param textChunks a list of all TextChunks that this strategy found during processing
         * @param filter the filter to apply.  If null, filtering will be skipped.
         * @return the filtered list
         * @since 5.3.3
         */

        private List<TextChunk> filterTextChunks(List<TextChunk> textChunks, ITextChunkFilter filter)
        {
            if (filter == null)
            {
                return textChunks;
            }

            var filtered = new List<TextChunk>();

            foreach (var textChunk in textChunks)
            {
                if (filter.Accept(textChunk))
                {
                    filtered.Add(textChunk);
                }
            }

            return filtered;
        }

        public override void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment segment = renderInfo.GetBaseline();
            if (renderInfo.GetRise() != 0.0f)
            {
                // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to 
                Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                segment = segment.TransformBy(riseOffsetTransform);
            }

            TextChunk tc = new TextChunk(renderInfo.GetText(), _tclStrategy.CreateLocation(renderInfo, segment));
            _locationalResult.Add(tc);
        }


        public IList<TextLocation> GetLocations()
        {

            var filteredTextChunks = filterTextChunks(_locationalResult, null);
            filteredTextChunks.Sort();

            TextChunk lastChunk = null;

            var textLocations = new List<TextLocation>();

            foreach (var chunk in filteredTextChunks)
            {

                if (lastChunk == null)
                {
                    //initial
                    textLocations.Add(new TextLocation
                    {
                        Text = chunk.Text,
                        X = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[0]),
                        Y = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[1])
                    });

                }
                else
                {
                    if (chunk.SameLine(lastChunk))
                    {
                        var text = "";
                        // we only insert a blank space if the trailing character of the previous string wasn‘t a space, and the leading character of the current string isn‘t a space
                        if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.Text) &&
                            !EndsWithSpace(lastChunk.Text))
                            text += ' ';

                        text += chunk.Text;

                        textLocations[textLocations.Count - 1].Text += text;

                    }
                    else
                    {

                        textLocations.Add(new TextLocation
                        {
                            Text = chunk.Text,
                            X = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[0]),
                            Y = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[1])
                        });
                    }
                }

                lastChunk = chunk;
            }

            //now find the location(s) with the given texts
            return textLocations;

        }
    }

    public class TextLocation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Text { get; set; }
    }

    public class TextWithFontExtractionStategy : ITextExtractionStrategy
    {
        //HTML buffer
        private StringBuilder result = new StringBuilder();

        //Store last used properties
        private Vector lastBaseLine;
        private string lastFont;
        private float lastFontSize;

        //http://api.itextpdf.com/itext/com/itextpdf/text/pdf/parser/TextRenderInfo.html
        private enum TextRenderMode
        {
            FillText = 0,
            StrokeText = 1,
            FillThenStrokeText = 2,
            Invisible = 3,
            FillTextAndAddToPathForClipping = 4,
            StrokeTextAndAddToPathForClipping = 5,
            FillThenStrokeTextAndAddToPathForClipping = 6,
            AddTextToPaddForClipping = 7
        }


        public void BeginTextBlock()
        {

        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            string curFont = renderInfo.GetFont().PostscriptFontName;
            //Check if faux bold is used
            if ((renderInfo.GetTextRenderMode() == (int)TextRenderMode.FillThenStrokeText))
            {
                curFont += "-Bold";
            }

            //This code assumes that if the baseline changes then we're on a newline
            Vector curBaseline = renderInfo.GetBaseline().GetStartPoint();
            Vector topRight = renderInfo.GetAscentLine().GetEndPoint();


            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(curBaseline[Vector.I1],
                curBaseline[Vector.I2], topRight[Vector.I1], topRight[Vector.I2]);
            Single curFontSize = rect.Height;

            //See if something has changed, either the baseline, the font or the font size
            if ((lastBaseLine == null) || (curBaseline[Vector.I2] != lastBaseLine[Vector.I2]) ||
                (curFontSize != lastFontSize) || (curFont != lastFont))
            {
                //if we've put down at least one span tag close it
                if ((lastBaseLine != null))
                {
                    result.AppendLine("</span>");
                }

                //If the baseline has changed then insert a line break
                if ((lastBaseLine != null) && curBaseline[Vector.I2] != lastBaseLine[Vector.I2])
                {
                    result.AppendLine("<br />");
                }

                //Create an HTML tag with appropriate styles
                result.AppendFormat("<span style=\"font-family:{0};font-size:{1}\">", curFont, curFontSize);
            }

            //Append the current text
            result.Append(renderInfo.GetText());

            //Set currently used properties
            lastBaseLine = curBaseline;
            lastFontSize = curFontSize;
            lastFont = curFont;
        }

        public void EndTextBlock()
        {

        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {

        }

        public string GetResultantText()
        {
            //If we wrote anything then we'll always have a missing closing tag so close it here
            if (result.Length > 0)
            {
                result.Append("</span>");
            }

            return result.ToString();
        }

    }


    public class MyLocationTextExtractionStrategy : LocationTextExtractionStrategy
    {
        //Hold each coordinate
        public List<RectAndText> myPoints = new List<RectAndText>();
        public List<Locator> matchLocators = new List<Locator>();
        public List<Locator> allLocators = new List<Locator>();

        //The string that we're searching for
        public string TextToSearchFor { get; set; }
        private string[] SearchSeparators => TextToSearchFor.Split(' ');

        private bool[] Matcher { get; set; }
        private int MatchAt { get; set; }
        public int Counter { get; set; }

        //How to compare strings
        public System.Globalization.CompareOptions CompareOptions { get; set; }

        public MyLocationTextExtractionStrategy(String textToSearchFor,
            System.Globalization.CompareOptions compareOptions = System.Globalization.CompareOptions.None)
        {
            this.TextToSearchFor = textToSearchFor;
            this.CompareOptions = compareOptions;

            this.Matcher = new bool[SearchSeparators.Length];
        }

        //Automatically called for each chunk of text in the PDF
        public override void RenderText(TextRenderInfo renderInfo)
        {
            base.RenderText(renderInfo);
            //See if the current chunk contains the text
            var startPosition = -1;

            string renderGetText = renderInfo.GetText();

            foreach (var sep in SearchSeparators)
            {
                startPosition = System.Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(renderGetText, sep, this.CompareOptions);
                if (startPosition >= 0)
                {
                    bool backMatch = Matcher[(MatchAt == 0 ? 0 : MatchAt - 1)];
                    MatchAt++;
                    if (MatchAt > 1 && !backMatch)
                        MatchAt = 0;
                    else
                        Matcher[MatchAt - 1] = true;
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(renderGetText) && startPosition < 0)
            {
                MatchAt = 0;
                Array.Clear(this.Matcher, 0, this.Matcher.Length);
            }

            var chars = renderInfo.GetCharacterRenderInfos().Skip(startPosition).Take(this.TextToSearchFor.Length)
                .ToList();

            //Grab the first and last character
            var firstChar = chars.First();
            var lastChar = chars.Last();


            //Get the bounding box for the chunk of text
            var bottomLeft = firstChar.GetDescentLine().GetStartPoint();
            var topRight = lastChar.GetAscentLine().GetEndPoint();

            if (!string.IsNullOrWhiteSpace(renderGetText))
            {
                Counter++;
                allLocators.Add(new Locator
                {
                    Word = renderGetText,
                    llx = bottomLeft[Vector.I1],
                    lly = bottomLeft[Vector.I2],
                    urx = topRight[Vector.I1],
                    ury = topRight[Vector.I2],
                    Counter = Counter
                });
            }


            //If not found bail
            if (startPosition < 0)
            {
                return;
            }

            //Grab the individual characters


            //Create a rectangle from it
            var rect = new iTextSharp.text.Rectangle(
                bottomLeft[Vector.I1],
                bottomLeft[Vector.I2],
                topRight[Vector.I1],
                topRight[Vector.I2]
            );
            matchLocators.Add(new Locator
            {
                llx = bottomLeft[Vector.I1],
                lly = bottomLeft[Vector.I2],
                urx = topRight[Vector.I1],
                ury = topRight[Vector.I2],
                Word = renderGetText,
                Counter = Counter
            });
            this.myPoints.Add(new RectAndText(rect, renderGetText));
            if (this.Matcher.All(x => x))
            {
                MatchAt = 0;
                Array.Clear(this.Matcher, 0, this.Matcher.Length);
            }
            //Add this to our main collection
        }
    }

    public class RectAndText
    {
        public iTextSharp.text.Rectangle Rect;
        public String Text;
        public RectAndText(iTextSharp.text.Rectangle rect, String text)
        {
            this.Rect = rect;
            this.Text = text;
        }
    }

    public class Locator
    {
        public float llx { get; set; }
        public float lly { get; set; }
        public float ury { get; set; }
        public float urx { get; set; }

        public string Word { get; set; }
        public int Counter { get; set; }
    }
}