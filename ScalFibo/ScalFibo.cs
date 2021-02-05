using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ScalFibo : Indicator
    {

        #region Enums & Class

        public enum CandleMode
        {
            HighLow,
            OpenClose
        }

        public enum MyColors
        {

            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Transparent,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen

        }

        #endregion

        #region Identity

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "ScalFibo";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.0";

        #endregion

        #region Params

        /// <summary>
        /// Identità del prodotto nel contesto di ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/scalfibo-indicator/")]
        public string ProductInfo { get; set; }
        /*
        /// <summary>
        /// Il numero di giorni da visualizzare
        /// </summary>
        [Parameter("Candle TimeFrame", Group = "Params", DefaultValue = 8, Step = 1)]
        public TimeFrame CandleTimeFrame { get; set; }
       
        /// <summary>
        /// Il numero di giorni da visualizzare
        /// </summary>
        [Parameter("Candle Mode", Group = "Params", DefaultValue = CandleMode.HighLow)]
        public CandleMode MyCandleMode { get; set; }
        
            /// <summary>
        /// Il numero di canele da visualizzare
        /// </summary>
        [Parameter("Candles To Show", Group = "Params", DefaultValue = 10, MinValue = 1, Step = 1)]
        public int CandleShow { get; set; }
        */

        [Parameter("Average Period", Group = "Strategy", DefaultValue = 50, MinValue = 1, Step = 1)]
        public int AveragePeriod { get; set; }

        [Parameter("Min Average", Group = "Strategy", DefaultValue = 40, MinValue = 1, Step = 1)]
        public int MinAverage { get; set; }

        [Parameter("Min Bars for Activation", Group = "Strategy", DefaultValue = 10, MinValue = 1, Step = 1)]
        public int MinBarsActivation { get; set; }

        [Parameter("MAx Bars for Activation", Group = "Strategy", DefaultValue = 35, MinValue = 1, Step = 1)]
        public int MaxBarsActivation { get; set; }

        /// <summary>
        /// Il Box, lo stile del bordo
        /// </summary>
        [Parameter("Fibo Number Margin", Group = "Styles", DefaultValue = 0.5)]
        public double FiboMargin { get; set; }

        [Parameter("Line Style Box", Group = "Styles", DefaultValue = LineStyle.Solid)]
        public LineStyle LineStyleBox { get; set; }

        /// <summary>
        /// Il Box, lo spessore del bordo
        /// </summary>
        [Parameter("Tickness", Group = "Styles", DefaultValue = 1, MaxValue = 5, MinValue = 1, Step = 1)]
        public int TicknessBox { get; set; }

        /// <summary>
        /// Il Box, il colore del time range
        /// </summary>
        [Parameter("Time Range Color", Group = "Styles", DefaultValue = MyColors.Violet)]
        public MyColors ColorTimeRange { get; set; }

        /// <summary>
        /// Il Box, il colore del massimo
        /// </summary>
        [Parameter("High/Open/Long Color", Group = "Styles", DefaultValue = MyColors.DodgerBlue)]
        public MyColors ColorHigh { get; set; }

        /// <summary>
        /// Il Box, il colore del minimo
        /// </summary>
        [Parameter("Low/Close/Short Color", Group = "Styles", DefaultValue = MyColors.Red)]
        public MyColors ColorLow { get; set; }

        /// <summary>
        /// Il Box, il colore del testo
        /// </summary>
        [Parameter("Text Color", Group = "Styles", DefaultValue = MyColors.Green)]
        public MyColors ColorText { get; set; }

        [Parameter("Fibonacci Color", Group = "Styles", DefaultValue = MyColors.Black)]
        public MyColors ColorFibo { get; set; }

        /// <summary>
        /// Il Box, l'opacità
        /// </summary>
        [Parameter("Opacity", Group = "Styles", DefaultValue = 50, MinValue = 1, MaxValue = 100, Step = 1)]
        public int Opacity { get; set; }

        /*
        /// <summary>
        /// Il Box, il riempimento
        /// </summary>
        [Parameter("Boxed ?", Group = "Styles", DefaultValue = true)]
        public bool Boxed { get; set; }

        /// <summary>
        /// Il Box, il riempimento
        /// </summary>
        [Parameter("Fill Box ?", Group = "Styles", DefaultValue = true)]
        public bool FillBox { get; set; }
        */

        #endregion

        #region Property

        private const double OpenStrategy = 8;
        private const double CloseStrategy = 19;
        private const string YES = "✔";
        private const string NO = "❌";

        private double[] BodyAverage = new double[] 
        {
            0,
            0,
            0
        };

        private double StrategyHight = 0;
        private double StrategyLow = 0;

        private int LastIndex5CheckedHigh = 0;
        private int LastIndex5CheckedLow = 0;
        bool trategyEnd = false;
        bool checked50 = false;
        bool checked38 = false;

        #endregion

        #region Indicator Events

        /// <summary>
        /// Viene generato all'avvio dell'indicatore, si inizializza l'indicatore
        /// </summary>
        protected override void Initialize()
        {

            // --> Se il timeframe è superiore o uguale al corrente devo uscire
            if (TimeFrame != TimeFrame.Minute5)
                Chart.DrawStaticText("Alert", string.Format("{0} : USE THIS INDICATOR ONLY WITH TIMEFRAME 5m", NAME.ToUpper()), VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

        }

        /// <summary>
        /// Generato ad ogni tick, vengono effettuati i calcoli dell'indicatore
        /// </summary>
        /// <param name="index">L'indice della candela in elaborazione</param>
        public override void Calculate(int index)
        {

            if (TimeFrame != TimeFrame.Minute5)
                return;

            try
            {

                _drawLevelFromCustomBar(index);

            } catch (Exception exp)
            {

                Chart.DrawStaticText("Alert", string.Format("{0} : error, {1}", NAME, exp), VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);

            }

        }

        #endregion

        #region Private Methods

        private void _drawLevelFromCustomBar(int index5m)
        {

            // --> Prelevo le candele daily
            Bars BarsCustom = MarketData.GetBars(TimeFrame.Daily);

            int index = BarsCustom.Count - 1;

            // --> Potrei non avere un numero sufficiente di candele
            if (index < 1)
                return;

            DateTime thisCandle = BarsCustom[index].OpenTime;
            DateTime nextCandle = thisCandle.AddDays(1);

            DateTime strategyZoneOpen = thisCandle.AddHours(OpenStrategy);
            DateTime strategyZoneClose = thisCandle.AddHours(CloseStrategy);

            DateTime beforeNextCandle = nextCandle.AddHours(-1);
            DateTime afterNextCandle = nextCandle;

            string RangeColor = (BarsCustom[index].Close > BarsCustom[index].Open) ? ColorHigh.ToString("G") : ColorLow.ToString("G");
            double treuLevelDiffInDigits = Math.Round(1.5 * Symbol.PipSize, Symbol.Digits);
            bool straetgyTZone = (Bars[index5m].OpenTime >= strategyZoneOpen && Bars[index5m].OpenTime <= strategyZoneClose);


            // --> Disegno l'apertura e la chiusura
            ChartRectangle MyBoxOC = Chart.DrawRectangle("ScalFibo OpenClose", beforeNextCandle, BarsCustom[index].Open, afterNextCandle, BarsCustom[index].Close, Color.FromName(RangeColor), TicknessBox, LineStyleBox);
            MyBoxOC.IsFilled = true;

            // --> Disegno il minimo e il massimo
            ChartRectangle MyBoxHL = Chart.DrawRectangle("ScalFibo HighLow", thisCandle, BarsCustom[index].High, nextCandle, BarsCustom[index].Low, Color.FromArgb(Opacity, Color.FromName(RangeColor)), TicknessBox, LineStyleBox);
            MyBoxHL.IsFilled = false;

            // --> Disegno il range temporale
            ChartRectangle MyBoxTimeRange = Chart.DrawRectangle("ScalFibo Time Range", strategyZoneOpen, BarsCustom[index].High, strategyZoneClose, BarsCustom[index].Low, Color.FromArgb(Opacity, Color.FromName(ColorTimeRange.ToString("G"))), TicknessBox, LineStyleBox);
            MyBoxTimeRange.IsFilled = true;

            // --> Disegno la linea dell'apertura
            ChartTrendLine MyBoxOpen = Chart.DrawTrendLine("ScalFibo Open Line", thisCandle.AddHours(OpenStrategy), BarsCustom[index].Open, thisCandle.AddHours(CloseStrategy), BarsCustom[index].Open, Color.FromArgb(Opacity, Color.FromName(RangeColor)), TicknessBox, LineStyleBox);
            MyBoxOpen.IsInteractive = false;
            /*
            // --> Disegno la linea della shadow
            ChartTrendLine MyBoxShadow = Chart.DrawTrendLine("ScalFibo Shadow Line" + rangeFlag, nextCandle, BarsCustom[index].High, nextCandle, BarsCustom[index].Low, Color.FromArgb(Opacity, Color.FromName(RangeColor)), 5, LineStyle.Solid);
            MyBoxShadow.IsInteractive = false;
            */

            // --> Ad ogni cambio giorno eseguo dei controlli
            if (BodyAverage[2] != index)
            {

                BodyAverage = _getDailyBodyAverage(index);
                StrategyHight = BarsCustom[index].Open;
                StrategyLow = BarsCustom[index].Open;
                trategyEnd = false;
                checked50 = false;
                checked38 = false;

            }

            // --> Aggiorno i realtive levels solo se nella daily
            if (Bars[index5m].OpenTime >= BarsCustom[index].OpenTime)
            {

                // --> Aggiorno il minimo e il massimo relativo alla strategia
                if (Bars[index5m].High >= StrategyHight + treuLevelDiffInDigits || (index5m == LastIndex5CheckedHigh && Bars[index5m].High > StrategyHight) || (Bars[LastIndex5CheckedHigh].OpenTime < BarsCustom[index].OpenTime))
                {

                    if (!trategyEnd && Bars[index5m].OpenTime <= strategyZoneClose)
                    {

                        ChartIcon realHightIcon = Chart.DrawIcon("ScalFibo Real Hight", ChartIconType.Circle, Bars[index5m].OpenTime, Bars[index5m].High, Color.FromName(ColorHigh.ToString("G")));
                        realHightIcon.IsInteractive = false;

                        StrategyHight = Bars[index5m].High;
                        LastIndex5CheckedHigh = index5m;

                    }


                }

                if (Bars[index5m].Low <= StrategyLow - treuLevelDiffInDigits || (index5m == LastIndex5CheckedLow && Bars[index5m].Low < StrategyLow) || (Bars[LastIndex5CheckedLow].OpenTime < BarsCustom[index].OpenTime))
                {

                    if (!trategyEnd && Bars[index5m].OpenTime <= strategyZoneClose)
                    {

                        ChartIcon realLowIcon = Chart.DrawIcon("ScalFibo Real Low", ChartIconType.Circle, Bars[index5m].OpenTime, Bars[index5m].Low, Color.FromName(ColorLow.ToString("G")));
                        realLowIcon.IsInteractive = false;

                        StrategyLow = Bars[index5m].Low;
                        LastIndex5CheckedLow = index5m;

                    }


                }

            }

            // --> Calcolo il minimo indispensabile per avviare la strategia, 15%
            double minAverage = BodyAverage[1] > 0 ? Math.Round(BodyAverage[0] - ((BodyAverage[1] / 100) * 15), 2) : 0;
            double currentAverage = Math.Round(Math.Abs(BarsCustom[index].Open - BarsCustom[index].Close) / Symbol.PipSize, 2);
            double MaxAverageHight = Math.Round((StrategyHight - BarsCustom[index].Open) / Symbol.PipSize, 2);
            double MaxAverageLow = Math.Round((BarsCustom[index].Open - StrategyLow) / Symbol.PipSize, 2);
            double MaxAverage = 0;

            string MaxAverageDirection = "";

            if (MaxAverageHight >= MaxAverageLow)
            {

                MaxAverage = MaxAverageHight;
                MaxAverageDirection = "Up";

            }
            else
            {

                MaxAverage = MaxAverageLow;
                MaxAverageDirection = "Down";

            }

            // --> Raccolgo le informazioni
            string padding = "\t";
            string info = string.Format(padding + "{0} ({1})\r\n\r\n", NAME.ToUpper(), VERSION);
            info += string.Format(padding + "Body Average\t: {0} ({1} Daily Bars)\r\n", BodyAverage[0], BodyAverage[1]);
            info += string.Format(padding + "... Activation\t: {0} (15% of Body Average)\r\n\r\n", minAverage);
            info += string.Format(padding + "Day Average\t: {0}\r\n", currentAverage);
            info += string.Format(padding + "... Max\t\t: {0} ({1})\r\n\r\n\r\n", MaxAverage, MaxAverageDirection);
            info += string.Format(padding + "CHECKLIST\r\n\r\n", MaxAverage, MaxAverageDirection);

            info += string.Format(padding + "{0} Strategy Time Zone\r\n\r\n", straetgyTZone ? "✔" : "❌");

            bool minxBody = (BodyAverage[0] >= MinAverage && MaxAverage >= minAverage);
            bool activationExtra = BodyAverage[0] < MinAverage && MaxAverage >= MinAverage;
            bool globalAverageActivation = minxBody || activationExtra;

            info += string.Format(padding + "{0} Average Activation (Body Average Min {1})\r\n", globalAverageActivation ? YES : NO, MinAverage);
            info += string.Format(padding + "... {0} Min Average (if Body Average >={1} then Day Max >=15%)\r\n", minxBody ? YES : NO, MinAverage);
            info += string.Format(padding + "... {0} Day Average (if Body Average <{1} then Day Max >= {1})\r\n\r\n", activationExtra ? YES : NO, MinAverage);

            int indexForPeriodZone = (LastIndex5CheckedHigh > LastIndex5CheckedLow) ? LastIndex5CheckedHigh : LastIndex5CheckedLow;
            int currentScart = index5m - indexForPeriodZone;
            bool periodZone = currentScart >= MinBarsActivation && currentScart <= MaxBarsActivation;

            info += string.Format(padding + "{0} Period Zone (>={1} && <={2})({3})\r\n\r\n", periodZone ? YES : NO, MinBarsActivation, MaxBarsActivation, currentScart);

            bool canDrawFibonacci = (straetgyTZone && globalAverageActivation);
            trategyEnd = checked50 || checked38;

            // --> && periodZone; aggiungere anche il check a 38.2 e disegnerà fibo fino a un certo punto e non oltre
            // --> Se ci sono le condizioni disegno Fibonacci, lo ricorstruisco perchè il classico non mi fa mettere altri livelli
            if (!trategyEnd && canDrawFibonacci)
            {

                // --> Ricavo i livelli
                double Fibo19tmp = Math.Round((((Bars[LastIndex5CheckedHigh].High - Bars[LastIndex5CheckedLow].Low) / 100) * 19.1), Symbol.Digits);
                double Fibo26tmp = Math.Round((((Bars[LastIndex5CheckedHigh].High - Bars[LastIndex5CheckedLow].Low) / 100) * 26.4), Symbol.Digits);
                double Fibo38tmp = Math.Round((((Bars[LastIndex5CheckedHigh].High - Bars[LastIndex5CheckedLow].Low) / 100) * 38.2), Symbol.Digits);
                double Fibo50tmp = Math.Round((((Bars[LastIndex5CheckedHigh].High - Bars[LastIndex5CheckedLow].Low) / 100) * 50), Symbol.Digits);
                double Fibo61tmp = Math.Round((((Bars[LastIndex5CheckedHigh].High - Bars[LastIndex5CheckedLow].Low) / 100) * 61.8), Symbol.Digits);
                double Fibo76tmp = Math.Round((((Bars[LastIndex5CheckedHigh].High - Bars[LastIndex5CheckedLow].Low) / 100) * 76.4), Symbol.Digits);

                double Fibo19 = (LastIndex5CheckedHigh < LastIndex5CheckedLow) ? Bars[LastIndex5CheckedLow].Low + Fibo19tmp : Bars[LastIndex5CheckedHigh].High - Fibo19tmp;
                double Fibo26 = (LastIndex5CheckedHigh < LastIndex5CheckedLow) ? Bars[LastIndex5CheckedLow].Low + Fibo26tmp : Bars[LastIndex5CheckedHigh].High - Fibo26tmp;
                double Fibo38 = (LastIndex5CheckedHigh < LastIndex5CheckedLow) ? Bars[LastIndex5CheckedLow].Low + Fibo38tmp : Bars[LastIndex5CheckedHigh].High - Fibo38tmp;
                double Fibo50 = (LastIndex5CheckedHigh < LastIndex5CheckedLow) ? Bars[LastIndex5CheckedLow].Low + Fibo50tmp : Bars[LastIndex5CheckedHigh].High - Fibo50tmp;
                double Fibo61 = (LastIndex5CheckedHigh < LastIndex5CheckedLow) ? Bars[LastIndex5CheckedLow].Low + Fibo61tmp : Bars[LastIndex5CheckedHigh].High - Fibo61tmp;
                double Fibo76 = (LastIndex5CheckedHigh < LastIndex5CheckedLow) ? Bars[LastIndex5CheckedLow].Low + Fibo76tmp : Bars[LastIndex5CheckedHigh].High - Fibo76tmp;

                DateTime dorsale = (LastIndex5CheckedHigh < LastIndex5CheckedLow) ? Bars[LastIndex5CheckedLow].OpenTime : Bars[LastIndex5CheckedHigh].OpenTime;

                if (dorsale >= strategyZoneOpen && dorsale <= strategyZoneClose)
                {
                    // --> Devo valutare meglio come fare a checkare questo flag
                    if(!checked50) checked50 = ((LastIndex5CheckedHigh < LastIndex5CheckedLow && Bars[index5m].High >= Fibo50) || (LastIndex5CheckedHigh > LastIndex5CheckedLow && Bars[index5m].Low <= Fibo50));
                    if(!checked50) checked38 = periodZone && ((LastIndex5CheckedHigh < LastIndex5CheckedLow && Bars[index5m].High >= Fibo38) || (LastIndex5CheckedHigh > LastIndex5CheckedLow && Bars[index5m].Low <= Fibo38));

                    double margin = Math.Round(FiboMargin * Symbol.PipSize, Symbol.Digits);

                    // --> La dorsale
                    ChartTrendLine RealFiboLine = Chart.DrawTrendLine("ScalFibo Fibo Line", dorsale, Bars[LastIndex5CheckedHigh].High, dorsale, Bars[LastIndex5CheckedLow].Low, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFiboLine.IsInteractive = false;

                    // --> Disegno il Fibo50
                    ChartTrendLine RealFibo19 = Chart.DrawTrendLine("ScalFibo Fibo 19", dorsale, Fibo19, dorsale.AddHours(1), Fibo19, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo19.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 19.1 txt", "19.1", dorsale.AddHours(1), Fibo19 + margin, Color.FromName(ColorFibo.ToString("G")));

                    // --> Disegno il Fibo50
                    ChartTrendLine RealFibo26 = Chart.DrawTrendLine("ScalFibo Fibo 26", dorsale, Fibo26, dorsale.AddHours(1), Fibo26, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo26.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 26.4 txt", "26.4", dorsale.AddHours(1), Fibo26 + margin, Color.FromName(ColorFibo.ToString("G")));

                    // --> Disegno il Fibo50
                    ChartTrendLine RealFibo38 = Chart.DrawTrendLine("ScalFibo Fibo 38", dorsale, Fibo38, dorsale.AddHours(1), Fibo38, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo38.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 38.2 txt", "38.2", dorsale.AddHours(1), Fibo38 + margin, Color.FromName(ColorFibo.ToString("G")));

                    // --> Disegno il Fibo50
                    ChartTrendLine RealFibo50 = Chart.DrawTrendLine("ScalFibo Fibo 50", dorsale, Fibo50, dorsale.AddHours(1), Fibo50, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo50.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 50.0 txt", "50.0", dorsale.AddHours(1), Fibo50 + margin, Color.FromName(ColorFibo.ToString("G")));

                    // --> Disegno il Fibo50
                    ChartTrendLine RealFibo61 = Chart.DrawTrendLine("ScalFibo Fibo 61", dorsale, Fibo61, dorsale.AddHours(1), Fibo61, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo61.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 61.8 txt", "61.8", dorsale.AddHours(1), Fibo61 + margin, Color.FromName(ColorFibo.ToString("G")));

                    // --> Disegno il Fibo50
                    ChartTrendLine RealFibo76 = Chart.DrawTrendLine("ScalFibo Fibo 76", dorsale, Fibo76, dorsale.AddHours(1), Fibo76, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo76.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 76.4 txt", "76.4", dorsale.AddHours(1), Fibo76 + margin, Color.FromName(ColorFibo.ToString("G")));

                }


            }

            info += string.Format(padding + "{0} Strategy Hit!\r\n", trategyEnd ? YES : NO);
            info += string.Format(padding + "... {0} Fibo 50\r\n", checked50 ? YES : NO);
            info += string.Format(padding + "... {0} Trigger\r\n", checked38 ? YES : NO);

            // --> Il box info
            Chart.DrawText("ScalFibo Info", info, nextCandle, BarsCustom[index].High, Color.FromName(ColorText.ToString("G")));

        }

        private double[] _getDailyBodyAverage(int index)
        {

            Bars BarsDaily = MarketData.GetBars(TimeFrame.Daily);

            if (BarsDaily.Count < AveragePeriod)
                return new double[] 
                {
                    0,
                    0,
                    0
                };

            double total = 0;
            double count = 0;

            // --> Controllo ogni candela e registro i punti che mi interessano
            for (int i = 1; i <= AveragePeriod; i++)
            {

                count++;

                // --> Potrebbe essere una candela rialzista, restituisco sempre un numero positivo
                total += Math.Abs(BarsDaily[index - i].Open - BarsDaily[index - i].Close);

            }

            double average = Math.Round((total / count) / Symbol.PipSize, 2);

            // --> Restituisco il numero di pips
            return new double[] 
            {
                average,
                count,
                index
            };

        }

        #endregion

    }
}
