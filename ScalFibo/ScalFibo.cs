using cAlgo.API;
using System.Threading;
using System;
using System.Windows.Forms;
using cAlgo.API.Internals;
using System.Text.RegularExpressions;
using System.Net;

namespace cAlgo
{
    public static class Extensions
    {

        #region Enum

        public enum ColorNameEnum
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

        public enum CapitalTo
        {

            Balance,
            Equity

        }

        #endregion

        #region Class

        public class Monitor
        {

            private readonly Positions _allPositions = null;

            public class Information
            {

                public double TotalNetProfit = 0;
                public double MinVolumeInUnits = 0;
                public double MaxVolumeInUnits = 0;
                public double MidVolumeInUnits = 0;
                public int BuyPositions = 0;
                public int SellPositions = 0;
                public Position FirstPosition = null;
                public Position LastPosition = null;
                public double HighestHighAfterFirstOpen = 0;
                public double LowestLowAfterFirstOpen = 0;

            }

            public class PauseTimes
            {

                public double Over = 0;
                public double Under = 0;

            }

            public class BreakEvenData
            {

                public bool OnlyFirst = false;
                public bool Negative = false;
                public double Activation = 0;
                public int LimitBar = 0;
                public double Distance = 0;

            }

            public class TrailingData
            {

                public bool OnlyFirst = false;
                public bool ProActive = false;
                public double Activation = 0;
                public double Distance = 0;

            }

            public bool OpenedInThisBar = false;

            public bool OpenedInThisTrigger = false;

            public readonly string Label;

            public readonly Symbol Symbol;

            public readonly Bars Bars;

            public readonly PauseTimes Pause;

            public Information Info { get; private set; }

            public Position[] Positions { get; private set; }

            public Monitor(string NewLabel, Symbol NewSymbol, Bars NewBars, Positions AllPositions, PauseTimes NewPause)
            {

                Label = NewLabel;
                Symbol = NewSymbol;
                Bars = NewBars;
                Pause = NewPause;

                Info = new Information();

                _allPositions = AllPositions;

                Update(false, null, null, 0);

            }

            public Information Update(bool closeall, BreakEvenData breakevendata, TrailingData trailingdata, double SafeLoss, TradeType? filtertype = null)
            {

                Positions = _allPositions.FindAll(Label, Symbol.Name);

                double highestHighAfterFirstOpen = (Positions.Length > 0) ? Info.HighestHighAfterFirstOpen : 0;
                double lowestLowAfterFirstOpen = (Positions.Length > 0) ? Info.LowestLowAfterFirstOpen : 0;

                Info = new Information 
                {

                    HighestHighAfterFirstOpen = highestHighAfterFirstOpen,
                    LowestLowAfterFirstOpen = lowestLowAfterFirstOpen

                };

                double tmpVolume = 0;

                foreach (Position position in Positions)
                {

                    if (Info.HighestHighAfterFirstOpen == 0 || Symbol.Ask > Info.HighestHighAfterFirstOpen)
                        Info.HighestHighAfterFirstOpen = Symbol.Ask;
                    if (Info.LowestLowAfterFirstOpen == 0 || Symbol.Bid < Info.LowestLowAfterFirstOpen)
                        Info.LowestLowAfterFirstOpen = Symbol.Bid;

                    if (closeall && (filtertype == null || position.TradeType == filtertype))
                    {

                        position.Close();
                        continue;

                    }

                    if (SafeLoss > 0 && position.StopLoss == null)
                    {

                        TradeResult result = position.ModifyStopLossPips(SafeLoss);

                        if (result.Error == ErrorCode.InvalidRequest || result.Error == ErrorCode.InvalidStopLossTakeProfit)
                        {

                            position.Close();

                        }

                        continue;

                    }

                    if ((breakevendata != null && !breakevendata.OnlyFirst) || Positions.Length == 1)
                        CheckBreakEven(position, breakevendata);

                    if ((trailingdata != null && !trailingdata.OnlyFirst) || Positions.Length == 1)
                        CheckTrailing(position, trailingdata);

                    Info.TotalNetProfit += position.NetProfit;
                    tmpVolume += position.VolumeInUnits;

                    switch (position.TradeType)
                    {
                        case TradeType.Buy:

                            Info.BuyPositions++;
                            break;

                        case TradeType.Sell:

                            Info.SellPositions++;
                            break;

                    }

                    if (Info.FirstPosition == null || position.EntryTime < Info.FirstPosition.EntryTime)
                        Info.FirstPosition = position;

                    if (Info.LastPosition == null || position.EntryTime > Info.LastPosition.EntryTime)
                        Info.LastPosition = position;

                    if (Info.MinVolumeInUnits == 0 || position.VolumeInUnits < Info.MinVolumeInUnits)
                        Info.MinVolumeInUnits = position.VolumeInUnits;

                    if (Info.MaxVolumeInUnits == 0 || position.VolumeInUnits > Info.MaxVolumeInUnits)
                        Info.MaxVolumeInUnits = position.VolumeInUnits;

                }

                Info.MidVolumeInUnits = Math.Round(tmpVolume / Positions.Length, 0);

                return Info;

            }

            public void CloseAllPositions(TradeType? filtertype = null)
            {

                Update(true, null, null, 0, filtertype);

            }

            public bool InGAP(double distance)
            {

                return Symbol.DigitsToPips(Bars.LastGAP()) >= distance;

            }

            public bool InPause(DateTime timeserver)
            {

                string nowHour = (timeserver.Hour < 10) ? string.Format("0{0}", timeserver.Hour) : string.Format("{0}", timeserver.Hour);
                string nowMinute = (timeserver.Minute < 10) ? string.Format("0{0}", timeserver.Minute) : string.Format("{0}", timeserver.Minute);

                double adesso = Convert.ToDouble(string.Format("{0},{1}", nowHour, nowMinute));

                if (Pause.Over < Pause.Under && adesso >= Pause.Over && adesso <= Pause.Under)
                {

                    return true;

                }
                else if (Pause.Over > Pause.Under && ((adesso >= Pause.Over && adesso <= 23.59) || adesso <= Pause.Under))
                {

                    return true;

                }

                return false;

            }

            private void CheckBreakEven(Position position, BreakEvenData breakevendata)
            {

                if (breakevendata == null || breakevendata.Activation == 0)
                    return;

                double activation = Symbol.PipsToDigits(breakevendata.Activation);

                int currentMinutes = Bars.TimeFrame.ToMinutes();
                DateTime limitTime = position.EntryTime.AddMinutes(currentMinutes * breakevendata.LimitBar);
                bool limitActivation = (breakevendata.LimitBar > 0 && Bars.Last(0).OpenTime >= limitTime);

                double distance = Symbol.PipsToDigits(breakevendata.Distance);

                switch (position.TradeType)
                {

                    case TradeType.Buy:

                        double breakevenpointbuy = Math.Round(position.EntryPrice + distance, Symbol.Digits);

                        if ((Symbol.Bid >= (position.EntryPrice + activation) || limitActivation) && (position.StopLoss == null || position.StopLoss < breakevenpointbuy))
                        {

                            position.ModifyStopLossPrice(breakevenpointbuy);

                        }
                        else if (breakevendata.Negative && (Symbol.Bid <= (position.EntryPrice - activation) || limitActivation) && (position.TakeProfit == null || position.TakeProfit > breakevenpointbuy))
                        {

                            position.ModifyTakeProfitPrice(breakevenpointbuy);

                        }

                        break;

                    case TradeType.Sell:

                        double breakevenpointsell = Math.Round(position.EntryPrice - distance, Symbol.Digits);

                        if ((Symbol.Ask <= (position.EntryPrice - activation)) && (position.StopLoss == null || position.StopLoss > breakevenpointsell))
                        {

                            position.ModifyStopLossPrice(breakevenpointsell);

                        }
                        else if (breakevendata.Negative && (Symbol.Ask >= (position.EntryPrice + activation)) && (position.TakeProfit == null || position.TakeProfit < breakevenpointsell))
                        {

                            position.ModifyTakeProfitPrice(breakevenpointsell);

                        }

                        break;

                }

            }

            private void CheckTrailing(Position position, TrailingData trailingdata)
            {

                if (trailingdata == null || trailingdata.Activation == 0 || trailingdata.Distance == 0)
                    return;
                double distance = Symbol.PipsToDigits(trailingdata.Distance);
                double activation = Symbol.PipsToDigits(trailingdata.Activation);


                double trailing;
                switch (position.TradeType)
                {

                    case TradeType.Buy:

                        trailing = Math.Round(Symbol.Bid - distance, Symbol.Digits);

                        if ((Symbol.Bid >= (position.EntryPrice + activation)) && (position.StopLoss == null || position.StopLoss < trailing))
                        {

                            position.ModifyStopLossPrice(trailing);

                        }
                        else if (trailingdata.ProActive && Info.HighestHighAfterFirstOpen > 0 && position.StopLoss != null && position.StopLoss > 0)
                        {

                            double activationprice = position.EntryPrice + activation;
                            double firsttrailing = Math.Round(activationprice - distance, Symbol.Digits);

                            if (position.StopLoss >= firsttrailing)
                            {

                                double limitpriceup = Info.HighestHighAfterFirstOpen;
                                double limitpricedw = Math.Round(Info.HighestHighAfterFirstOpen - distance, Symbol.Digits);

                                double k = Math.Round(limitpriceup - Symbol.Ask, Symbol.Digits);

                                double newtrailing = Math.Round(limitpricedw + k, Symbol.Digits);

                                if (position.StopLoss < newtrailing)
                                    position.ModifyStopLossPrice(newtrailing);

                            }

                        }

                        break;

                    case TradeType.Sell:

                        trailing = Math.Round(Symbol.Ask + Symbol.PipsToDigits(trailingdata.Distance), Symbol.Digits);

                        if ((Symbol.Ask <= (position.EntryPrice - Symbol.PipsToDigits(trailingdata.Activation))) && (position.StopLoss == null || position.StopLoss > trailing))
                        {

                            position.ModifyStopLossPrice(trailing);

                        }
                        else if (trailingdata.ProActive && Info.LowestLowAfterFirstOpen > 0 && position.StopLoss != null && position.StopLoss > 0)
                        {

                            double activationprice = position.EntryPrice - activation;
                            double firsttrailing = Math.Round(activationprice + distance, Symbol.Digits);

                            if (position.StopLoss <= firsttrailing)
                            {

                                double limitpriceup = Math.Round(Info.LowestLowAfterFirstOpen + distance, Symbol.Digits);
                                double limitpricedw = Info.LowestLowAfterFirstOpen;

                                double k = Math.Round(Symbol.Bid - limitpricedw, Symbol.Digits);

                                double newtrailing = Math.Round(limitpriceup - k, Symbol.Digits);

                                if (position.StopLoss > newtrailing)
                                    position.ModifyStopLossPrice(newtrailing);

                            }

                        }

                        break;

                }

            }

        }

        public class MonenyManagement
        {

            private readonly double _minSize = 0.01;
            private double _percentage = 0;
            private double _fixedSize = 0;
            private double _pipToCalc = 30;

            private readonly IAccount _account = null;
            public readonly Symbol Symbol;

            public CapitalTo CapitalType = CapitalTo.Balance;

            public double Percentage
            {

                get { return _percentage; }


                set { _percentage = (value > 0 && value <= 100) ? value : 0; }
            }

            public double FixedSize
            {

                get { return _fixedSize; }



                set { _fixedSize = (value >= _minSize) ? value : 0; }
            }


            public double PipToCalc
            {

                get { return _pipToCalc; }

                set { _pipToCalc = (value > 0) ? value : 100; }
            }


            public double Capital
            {

                get
                {

                    switch (CapitalType)
                    {

                        case CapitalTo.Equity:

                            return _account.Equity;
                        default:


                            return _account.Balance;

                    }

                }
            }


            public MonenyManagement(IAccount NewAccount, CapitalTo NewCapitalTo, double NewPercentage, double NewFixedSize, double NewPipToCalc, Symbol NewSymbol)
            {

                _account = NewAccount;

                Symbol = NewSymbol;

                CapitalType = NewCapitalTo;
                Percentage = NewPercentage;
                FixedSize = NewFixedSize;
                PipToCalc = NewPipToCalc;

            }

            public double GetLotSize()
            {

                if (FixedSize > 0)
                    return FixedSize;

                double moneyrisk = Capital / 100 * Percentage;

                double sl_double = PipToCalc * Symbol.PipSize;
                double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);

                if (lots < _minSize)
                    return _minSize;

                return lots;

            }

        }

        #endregion

        #region Helper

        public static API.Color ColorFromEnum(ColorNameEnum colorName)
        {

            return API.Color.FromName(colorName.ToString("G"));

        }

        #endregion

        #region Bars

        public static int GetIndexByDate(this Bars thisBars, DateTime thisTime)
        {

            for (int i = thisBars.ClosePrices.Count - 1; i >= 0; i--)
            {

                if (thisTime == thisBars.OpenTimes[i])
                    return i;

            }

            return -1;

        }

        public static double LastGAP(this Bars thisBars)
        {

            double K = 0;

            if (thisBars.ClosePrices.Last(1) > thisBars.OpenPrices.LastValue)
            {

                K = Math.Round(thisBars.ClosePrices.Last(1) - thisBars.OpenPrices.LastValue, 5);

            }
            else if (thisBars.ClosePrices.Last(1) < thisBars.OpenPrices.LastValue)
            {

                K = Math.Round(thisBars.OpenPrices.LastValue - thisBars.ClosePrices.Last(1), 5);

            }

            return K;

        }

        #endregion

        #region Bar

        public static double Body(this Bar thisBar)
        {

            return thisBar.IsBullish() ? thisBar.Close - thisBar.Open : thisBar.Open - thisBar.Close;


        }

        public static bool IsBullish(this Bar thisBar)
        {

            return thisBar.Close > thisBar.Open;

        }

        public static bool IsBearish(this Bar thisBar)
        {

            return thisBar.Close < thisBar.Open;

        }

        public static bool IsDoji(this Bar thisBar)
        {

            return thisBar.Close == thisBar.Open;

        }

        #endregion

        #region Symbol

        public static double DigitsToPips(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips / thisSymbol.PipSize, 2);

        }

        public static double PipsToDigits(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips * thisSymbol.PipSize, thisSymbol.Digits);

        }

        public static double RealSpread(this Symbol thisSymbol)
        {

            return Math.Round(thisSymbol.Spread / thisSymbol.PipSize, 2);

        }

        #endregion

        #region TimeFrame

        public static int ToMinutes(this TimeFrame thisTimeFrame)
        {

            if (thisTimeFrame == TimeFrame.Daily)
                return 60 * 24;
            if (thisTimeFrame == TimeFrame.Day2)
                return 60 * 24 * 2;
            if (thisTimeFrame == TimeFrame.Day3)
                return 60 * 24 * 3;
            if (thisTimeFrame == TimeFrame.Hour)
                return 60;
            if (thisTimeFrame == TimeFrame.Hour12)
                return 60 * 12;
            if (thisTimeFrame == TimeFrame.Hour2)
                return 60 * 2;
            if (thisTimeFrame == TimeFrame.Hour3)
                return 60 * 3;
            if (thisTimeFrame == TimeFrame.Hour4)
                return 60 * 4;
            if (thisTimeFrame == TimeFrame.Hour6)
                return 60 * 6;
            if (thisTimeFrame == TimeFrame.Hour8)
                return 60 * 8;
            if (thisTimeFrame == TimeFrame.Minute)
                return 1;
            if (thisTimeFrame == TimeFrame.Minute10)
                return 10;
            if (thisTimeFrame == TimeFrame.Minute15)
                return 15;
            if (thisTimeFrame == TimeFrame.Minute2)
                return 2;
            if (thisTimeFrame == TimeFrame.Minute20)
                return 20;
            if (thisTimeFrame == TimeFrame.Minute3)
                return 3;
            if (thisTimeFrame == TimeFrame.Minute30)
                return 30;
            if (thisTimeFrame == TimeFrame.Minute4)
                return 4;
            if (thisTimeFrame == TimeFrame.Minute45)
                return 45;
            if (thisTimeFrame == TimeFrame.Minute5)
                return 5;
            if (thisTimeFrame == TimeFrame.Minute6)
                return 6;
            if (thisTimeFrame == TimeFrame.Minute7)
                return 7;
            if (thisTimeFrame == TimeFrame.Minute8)
                return 8;
            if (thisTimeFrame == TimeFrame.Minute9)
                return 9;
            if (thisTimeFrame == TimeFrame.Monthly)
                return 60 * 24 * 30;
            if (thisTimeFrame == TimeFrame.Weekly)
                return 60 * 24 * 7;

            return 0;

        }

        #endregion

    }

}


namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class ScalFibo : Indicator
    {

        #region Enums & Class

        public enum CandleMode
        {
            HighLow,
            OpenClose
        }

        #endregion

        #region Identity

        public const string NAME = "ScalFibo";

        public const string VERSION = "1.1.2";

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://www.google.com/search?q=ctrader+guru+scalfibo")]
        public string ProductInfo { get; set; }

        [Parameter("Average Period", Group = "Strategy", DefaultValue = 50, MinValue = 1, Step = 1)]
        public int AveragePeriod { get; set; }

        [Parameter("Min Average", Group = "Strategy", DefaultValue = 40, MinValue = 1, Step = 1)]
        public int MinAverage { get; set; }

        [Parameter("Min Bars for Activation", Group = "Strategy", DefaultValue = 10, MinValue = 1, Step = 1)]
        public int MinBarsActivation { get; set; }

        [Parameter("Max Bars for Activation", Group = "Strategy", DefaultValue = 35, MinValue = 1, Step = 1)]
        public int MaxBarsActivation { get; set; }

        [Parameter("True K", Group = "Strategy", DefaultValue = 1.5, MinValue = 0, Step = 0.1)]
        public double TrueDiff { get; set; }

        [Parameter("Time Start", Group = "Strategy", DefaultValue = 8, MinValue = 0, MaxValue = 23.59)]
        public double OpenStrategy { get; set; }

        [Parameter("Time Stop", Group = "Strategy", DefaultValue = 19, MinValue = 0, MaxValue = 23.59)]
        public double CloseStrategy { get; set; }

        [Parameter("On Trade Opportunity?", Group = "Alerts", DefaultValue = true)]
        public bool AlertOnOppo { get; set; }

        [Parameter("On Fibonacci Drawing?", Group = "Alerts", DefaultValue = false)]
        public bool AlertOnFibo { get; set; }

        [Parameter("On Strategy Hit?", Group = "Alerts", DefaultValue = false)]
        public bool AlertOnStrategyHit { get; set; }

        [Parameter("Enabled?", Group = "Webhook", DefaultValue = false)]
        public bool WebhookEnabled { get; set; }

        [Parameter("API", Group = "Webhook", DefaultValue = "https://api.telegram.org/bot[ YOUR TOKEN ]/sendMessage")]
        public string Webhook { get; set; }

        [Parameter("POST params", Group = "Webhook", DefaultValue = "chat_id=[ @CHATID ]&text={0}")]
        public string PostParams { get; set; }

        [Parameter("Fibo Number Margin", Group = "Styles", DefaultValue = 0.5)]
        public double FiboMargin { get; set; }

        [Parameter("Line Style Box", Group = "Styles", DefaultValue = LineStyle.Solid)]
        public LineStyle LineStyleBox { get; set; }

        [Parameter("Tickness", Group = "Styles", DefaultValue = 1, MaxValue = 5, MinValue = 1, Step = 1)]
        public int TicknessBox { get; set; }

        [Parameter("Time Range Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Violet)]
        public Extensions.ColorNameEnum ColorTimeRange { get; set; }

        [Parameter("High/Open/Long Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.DodgerBlue)]
        public Extensions.ColorNameEnum ColorHigh { get; set; }

        [Parameter("Low/Close/Short Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Red)]
        public Extensions.ColorNameEnum ColorLow { get; set; }

        [Parameter("Text Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Green)]
        public Extensions.ColorNameEnum ColorText { get; set; }

        [Parameter("Fibonacci Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Black)]
        public Extensions.ColorNameEnum ColorFibo { get; set; }

        [Parameter("Opacity", Group = "Styles", DefaultValue = 50, MinValue = 1, MaxValue = 100, Step = 1)]
        public int Opacity { get; set; }

        #endregion

        #region Property

        private const string YES = "✔";
        private const string NO = "❌";
        private const string ALERTON = "🔔";
        private const string ALERTOFF = "🔕";

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

        bool canAlert = false;

        bool alertHitOnFibo = false;
        bool alertHitOnStrategyHit = false;
        bool alertHitOnTradeOppo = false;
        bool fiboDrawded = false;

        #endregion

        #region Indicator Events

        protected override void Initialize()
        {

            if (TimeFrame != TimeFrame.Minute5)
            {

                AlertChart("USE THIS INDICATOR ONLY WITH TIMEFRAME 5 MINUTE");
                return;

            }

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

                DrawLevelFromCustomBar(index);

                if (IsLastBar)
                    canAlert = true;

            } catch (Exception exp)
            {

                Chart.DrawStaticText("Alert", string.Format("{0} : error, {1}", NAME, exp), VerticalAlignment.Center, API.HorizontalAlignment.Center, Color.Red);

            }

        }

        #endregion

        #region Private Methods

        private void DrawLevelFromCustomBar(int index5m)
        {

            Bars BarsCustom = MarketData.GetBars(TimeFrame.Daily);

            int index = BarsCustom.Count - 1;

            if (index < 1)
                return;

            DateTime thisCandle = BarsCustom[index].OpenTime;
            DateTime nextCandle = thisCandle.AddDays(1);

            DateTime strategyZoneOpen = thisCandle.AddHours(OpenStrategy);
            DateTime strategyZoneClose = thisCandle.AddHours(CloseStrategy);

            DateTime beforeNextCandle = nextCandle.AddHours(-1);
            DateTime afterNextCandle = nextCandle;

            string RangeColor = (BarsCustom[index].Close > BarsCustom[index].Open) ? ColorHigh.ToString("G") : ColorLow.ToString("G");
            double treuLevelDiffInDigits = Math.Round(TrueDiff * Symbol.PipSize, Symbol.Digits);
            bool straetgyTZone = (Bars[index5m].OpenTime >= strategyZoneOpen && Bars[index5m].OpenTime <= strategyZoneClose);


            ChartRectangle MyBoxOC = Chart.DrawRectangle("ScalFibo OpenClose", beforeNextCandle, BarsCustom[index].Open, afterNextCandle, BarsCustom[index].Close, Color.FromName(RangeColor), TicknessBox, LineStyleBox);
            MyBoxOC.IsFilled = true;

            ChartRectangle MyBoxHL = Chart.DrawRectangle("ScalFibo HighLow", thisCandle, BarsCustom[index].High, nextCandle, BarsCustom[index].Low, Color.FromArgb(Opacity, Color.FromName(RangeColor)), TicknessBox, LineStyleBox);
            MyBoxHL.IsFilled = false;

            ChartRectangle MyBoxTimeRange = Chart.DrawRectangle("ScalFibo Time Range", strategyZoneOpen, BarsCustom[index].High, strategyZoneClose, BarsCustom[index].Low, Color.FromArgb(Opacity, Color.FromName(ColorTimeRange.ToString("G"))), TicknessBox, LineStyleBox);
            MyBoxTimeRange.IsFilled = true;

            ChartTrendLine MyBoxOpen = Chart.DrawTrendLine("ScalFibo Open Line", thisCandle.AddHours(OpenStrategy), BarsCustom[index].Open, thisCandle.AddHours(CloseStrategy), BarsCustom[index].Open, Color.FromArgb(Opacity, Color.FromName(RangeColor)), TicknessBox, LineStyleBox);
            MyBoxOpen.IsInteractive = false;

            if (BodyAverage[2] != index)
            {

                BodyAverage = GetDailyBodyAverage(index);
                StrategyHight = BarsCustom[index].Open;
                StrategyLow = BarsCustom[index].Open;
                trategyEnd = false;
                checked50 = false;
                checked38 = false;

                fiboDrawded = false;

                canAlert = false;

                alertHitOnFibo = false;
                alertHitOnStrategyHit = false;
                alertHitOnTradeOppo = false;

            }

            if (Bars[index5m].OpenTime >= BarsCustom[index].OpenTime)
            {

                if (Bars[index5m].High >= StrategyHight + treuLevelDiffInDigits || (index5m == LastIndex5CheckedHigh && Bars[index5m].High > StrategyHight) || (Bars[LastIndex5CheckedHigh].OpenTime < BarsCustom[index].OpenTime))
                {

                    if (!trategyEnd && Bars[index5m].OpenTime <= strategyZoneClose)
                    {

                        ChartIcon realHightIcon = Chart.DrawIcon("ScalFibo Real Hight", ChartIconType.Circle, Bars[index5m].OpenTime, Bars[index5m].High, Color.FromName(ColorHigh.ToString("G")));
                        realHightIcon.IsInteractive = false;

                        StrategyHight = Bars[index5m].High;
                        LastIndex5CheckedHigh = index5m;

                        alertHitOnTradeOppo = false;

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

                        alertHitOnTradeOppo = false;

                    }


                }

            }

            double minAverage = BodyAverage[1] > 0 ? Math.Round(BodyAverage[0] - ((BodyAverage[1] / 100) * 15), 2) : 0;
            double currentAverage = Math.Round(Math.Abs(BarsCustom[index].Open - BarsCustom[index].Close) / Symbol.PipSize, 2);
            double MaxAverageHight = Math.Round((StrategyHight - BarsCustom[index].Open) / Symbol.PipSize, 2);
            double MaxAverageLow = Math.Round((BarsCustom[index].Open - StrategyLow) / Symbol.PipSize, 2);
            double MaxAverage;
            string MaxAverageDirection;
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


            if (AlertOnStrategyHit && trategyEnd && !alertHitOnStrategyHit)
            {

                alertHitOnStrategyHit = true;
                Alert("Strategy Hit!");

            }

            if (!trategyEnd && canDrawFibonacci)
            {

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

                    if (!checked50)
                        checked50 = ((LastIndex5CheckedHigh < LastIndex5CheckedLow && Bars[index5m].High >= Fibo50) || (LastIndex5CheckedHigh > LastIndex5CheckedLow && Bars[index5m].Low <= Fibo50));

                    if (!checked38)
                        checked38 = periodZone && ((LastIndex5CheckedHigh < LastIndex5CheckedLow && Bars[index5m].High >= Fibo38) || (LastIndex5CheckedHigh > LastIndex5CheckedLow && Bars[index5m].Low <= Fibo38));

                    double margin = Math.Round(FiboMargin * Symbol.PipSize, Symbol.Digits);

                    ChartTrendLine RealFiboLine = Chart.DrawTrendLine("ScalFibo Fibo Line", dorsale, Bars[LastIndex5CheckedHigh].High, dorsale, Bars[LastIndex5CheckedLow].Low, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFiboLine.IsInteractive = false;

                    ChartTrendLine RealFibo19 = Chart.DrawTrendLine("ScalFibo Fibo 19", dorsale, Fibo19, dorsale.AddHours(1), Fibo19, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo19.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 19.1 txt", "19.1", dorsale.AddHours(1), Fibo19 + margin, Color.FromName(ColorFibo.ToString("G")));

                    ChartTrendLine RealFibo26 = Chart.DrawTrendLine("ScalFibo Fibo 26", dorsale, Fibo26, dorsale.AddHours(1), Fibo26, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo26.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 26.4 txt", "26.4", dorsale.AddHours(1), Fibo26 + margin, Color.FromName(ColorFibo.ToString("G")));

                    ChartTrendLine RealFibo38 = Chart.DrawTrendLine("ScalFibo Fibo 38", dorsale, Fibo38, dorsale.AddHours(1), Fibo38, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo38.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 38.2 txt", "38.2", dorsale.AddHours(1), Fibo38 + margin, Color.FromName(ColorFibo.ToString("G")));

                    ChartTrendLine RealFibo50 = Chart.DrawTrendLine("ScalFibo Fibo 50", dorsale, Fibo50, dorsale.AddHours(1), Fibo50, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo50.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 50.0 txt", "50.0", dorsale.AddHours(1), Fibo50 + margin, Color.FromName(ColorFibo.ToString("G")));

                    ChartTrendLine RealFibo61 = Chart.DrawTrendLine("ScalFibo Fibo 61", dorsale, Fibo61, dorsale.AddHours(1), Fibo61, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo61.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 61.8 txt", "61.8", dorsale.AddHours(1), Fibo61 + margin, Color.FromName(ColorFibo.ToString("G")));

                    ChartTrendLine RealFibo76 = Chart.DrawTrendLine("ScalFibo Fibo 76", dorsale, Fibo76, dorsale.AddHours(1), Fibo76, Color.FromName(ColorFibo.ToString("G")), 1, LineStyle.Dots);
                    RealFibo76.IsInteractive = false;
                    Chart.DrawText("ScalFibo Fibo 76.4 txt", "76.4", dorsale.AddHours(1), Fibo76 + margin, Color.FromName(ColorFibo.ToString("G")));

                    if (AlertOnFibo && !alertHitOnFibo)
                    {

                        alertHitOnFibo = true;
                        Alert("Fibonacci retracement drawing!");

                    }

                    fiboDrawded = true;

                }


            }

            if (AlertOnOppo && !alertHitOnTradeOppo && !trategyEnd && fiboDrawded && periodZone)
            {

                alertHitOnTradeOppo = true;
                Alert("Trade Opportunity!");

            }

            info += string.Format(padding + "{0} Strategy Hit!\r\n", trategyEnd ? YES : NO);
            info += string.Format(padding + "... {0} Fibo 50\r\n", checked50 ? YES : NO);
            info += string.Format(padding + "... {0} Trigger\r\n\r\n", checked38 ? YES : NO);

            if (canAlert)
            {

                info += string.Format(padding + "{0} On Trade Opportunity {1}\r\n", AlertOnOppo ? ALERTON : ALERTOFF, alertHitOnTradeOppo ? YES : NO);
                info += string.Format(padding + "{0} On Fibonacci Drawing {1}\r\n", AlertOnFibo ? ALERTON : ALERTOFF, alertHitOnFibo ? YES : NO);
                info += string.Format(padding + "{0} On Strategy Hit {1}\r\n\r\n", AlertOnStrategyHit ? ALERTON : ALERTOFF, alertHitOnStrategyHit ? YES : NO);
                info += string.Format(padding + "{0} To Webhook\r\n", WebhookEnabled ? ALERTON : ALERTOFF);

            }


            Chart.DrawText("ScalFibo Info", info, nextCandle, BarsCustom[index].High, Color.FromName(ColorText.ToString("G")));

        }

        private double[] GetDailyBodyAverage(int index)
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

            for (int i = 1; i <= AveragePeriod; i++)
            {

                count++;

                total += Math.Abs(BarsDaily[index - i].Open - BarsDaily[index - i].Close);

            }

            double average = Math.Round((total / count) / Symbol.PipSize, 2);

            return new double[] 
            {
                average,
                count,
                index
            };

        }

        private void Alert(string mymex)
        {

            if (!canAlert || RunningMode != RunningMode.RealTime)
                return;

            string mex = string.Format("{0} : {1} {2}", NAME, SymbolName, mymex);

            new Thread(new ThreadStart(delegate { MessageBox.Show(mex, NAME, MessageBoxButtons.OK, MessageBoxIcon.Information); })).Start();
            ToWebHook(mex);
            Print(mex);

        }

        private void AlertChart(string mymex, bool withPrint = true)
        {

            if (RunningMode != RunningMode.RealTime)
                return;

            string mex = string.Format("{0} : {1}", NAME.ToUpper(), mymex);

            Chart.DrawStaticText("alert", mex, VerticalAlignment.Center, API.HorizontalAlignment.Center, Color.Red);
            if (withPrint)
                Print(mex);

        }

        public void ToWebHook(string custom)
        {

            if (!WebhookEnabled || custom == null || custom.Trim().Length < 1)
                return;

            string messageformat = custom.Trim();

            try
            {

                Uri myuri = new Uri(Webhook);

                string pattern = string.Format("{0}://{1}/.*", myuri.Scheme, myuri.Host);

                Regex urlRegEx = new Regex(pattern);
                WebPermission p = new WebPermission(NetworkAccess.Connect, urlRegEx);
                p.Assert();

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string HtmlResult = wc.UploadString(myuri, string.Format(PostParams, messageformat));
                }

            } catch (Exception exc)
            {

                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        #endregion

    }
}
