using cAlgo.API;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Windows.Forms;
using cAlgo.API.Internals;
using System.Collections.Generic;
using System.Linq;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

// --> LICENZA : RIFERIMENTI

using NM_CTG_Licenza;

// <-- LICENZA : RIFERIMENTI

// --> UPDATES : RIFERIMENTI

using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;

// <-- UPDATES : RIFERIMENTI


namespace cAlgo
{
    /// <summary>
    /// Estensioni che rendono il codice più scorrevole con metodi non previsti dalla libreria cAlgo
    /// </summary>
    public static class Extensions
    {

        #region Enum

        /// <summary>
        /// Enumeratore per esporre il nome del colore nelle opzioni
        /// </summary>
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

        /// <summary>
        /// Enumeratore per esporre nei parametri una scelta con menu a tendina
        /// </summary>
        public enum CapitalTo
        {

            Balance,
            Equity

        }

        #endregion

        #region Class

        /// <summary>
        /// Classe per monitorare le posizioni di una specifica strategia
        /// </summary>
        public class Monitor
        {

            private Positions _allPositions = null;

            /// <summary>
            /// Standard per la raccolta di informazioni nel Monitor
            /// </summary>
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

            /// <summary>
            /// Standard per l'interpretazione dell'orario in double
            /// </summary>
            public class PauseTimes
            {

                public double Over = 0;
                public double Under = 0;

            }

            /// <summary>
            /// Standard per la gestione del break even
            /// </summary>
            public class BreakEvenData
            {

                // --> In caso di operazioni multiple sarebbe bene evitare la gestione di tutte
                public bool OnlyFirst = false;
                public bool Negative = false;
                public double Activation = 0;
                public int LimitBar = 0;
                public double Distance = 0;

            }

            /// <summary>
            /// Standard per la gestione del trailing
            /// </summary>
            public class TrailingData
            {

                // --> In caso di operazioni multiple sarebbe bene evitare la gestione di tutte
                public bool OnlyFirst = false;
                public bool ProActive = false;
                public double Activation = 0;
                public double Distance = 0;

            }

            /// <summary>
            /// Memorizza lo stato di apertura di una operazione nella Bar corrente
            /// </summary>
            public bool OpenedInThisBar = false;

            /// <summary>
            /// Memorizza lo stato di apertura di una operazione con il trigger corrente
            /// </summary>
            public bool OpenedInThisTrigger = false;

            /// <summary>
            /// Valore univoco che identifica la strategia
            /// </summary>
            public readonly string Label;

            /// <summary>
            /// Il Simbolo da monitorare in relazione alla Label
            /// </summary>
            public readonly Symbol Symbol;

            /// <summary>
            /// Le Bars con il quale la strategia si muove ed elabora le sue condizioni
            /// </summary>
            public readonly Bars Bars;

            /// <summary>
            /// Il riferimento temporale della pausa
            /// </summary>
            public readonly PauseTimes Pause;

            /// <summary>
            /// Le informazioni raccolte dopo la chiamata .Update()
            /// </summary>
            public Information Info { get; private set; }

            /// <summary>
            /// Le posizioni filtrate in base al simbolo e alla label
            /// </summary>
            public Position[] Positions { get; private set; }

            /// <summary>
            /// Monitor per la raccolta d'informazioni inerenti la strategia in corso
            /// </summary>
            public Monitor(string NewLabel, Symbol NewSymbol, Bars NewBars, Positions AllPositions, PauseTimes NewPause)
            {

                Label = NewLabel;
                Symbol = NewSymbol;
                Bars = NewBars;
                Pause = NewPause;

                Info = new Information();

                _allPositions = AllPositions;

                // --> Rendiamo sin da subito disponibili le informazioni
                Update(false, null, null, 0);

            }

            /// <summary>
            /// Filtra e rende disponibili le informazioni per la strategia monitorata. Eventualmente Chiude e gestisce le operazioni
            /// </summary>
            public Information Update(bool closeall, BreakEvenData breakevendata, TrailingData trailingdata, double SafeLoss, TradeType? filtertype = null)
            {

                // --> Raccolgo le informazioni che mi servono per avere il polso della strategia
                Positions = _allPositions.FindAll(Label, Symbol.Name);

                // --> Devo trascinarmi i vecchi dati prima di aggiornarli come massimali
                double highestHighAfterFirstOpen = (Positions.Length > 0) ? Info.HighestHighAfterFirstOpen : 0;
                double lowestLowAfterFirstOpen = (Positions.Length > 0) ? Info.LowestLowAfterFirstOpen : 0;

                // --> Resetto le informazioni
                Info = new Information
                {

                    // --> Inizializzo con i vecchi dati
                    HighestHighAfterFirstOpen = highestHighAfterFirstOpen,
                    LowestLowAfterFirstOpen = lowestLowAfterFirstOpen

                };

                double tmpVolume = 0;

                foreach (Position position in Positions)
                {

                    // --> Per il trailing proactive e altre feature devo conoscere lo stato attuale
                    if (Info.HighestHighAfterFirstOpen == 0 || Symbol.Ask > Info.HighestHighAfterFirstOpen)
                        Info.HighestHighAfterFirstOpen = Symbol.Ask;
                    if (Info.LowestLowAfterFirstOpen == 0 || Symbol.Bid < Info.LowestLowAfterFirstOpen)
                        Info.LowestLowAfterFirstOpen = Symbol.Bid;

                    // --> Per prima cosa devo controllare se chiudere la posizione
                    if (closeall && (filtertype == null || position.TradeType == filtertype))
                    {

                        position.Close();
                        continue;

                    }

                    // --> Il broker potrebbe non accettare lo stoploss e quindi non settarlo, intervengo ?
                    if (SafeLoss > 0 && position.StopLoss == null)
                    {

                        TradeResult result = position.ModifyStopLossPips(SafeLoss);

                        // --> Troppa voaltilità potrebbe portare a proporzioni e valori errati, comunque non andiamo oltre 
                        if (result.Error == ErrorCode.InvalidRequest || result.Error == ErrorCode.InvalidStopLossTakeProfit)
                        {

                            position.Close();

                        }

                        continue;

                    }

                    // --> Poi tocca al break even
                    if ((breakevendata != null && !breakevendata.OnlyFirst) || Positions.Length == 1)
                        _checkBreakEven(position, breakevendata);

                    // --> Poi tocca al trailing
                    if ((trailingdata != null && !trailingdata.OnlyFirst) || Positions.Length == 1)
                        _checkTrailing(position, trailingdata);

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

                // --> Restituisce una Exception Overflow di una operazione aritmetica, da approfondire
                //     Info.MidVolumeInUnits = Symbol.NormalizeVolumeInUnits(tmpVolume / Positions.Length,RoundingMode.ToNearest);
                Info.MidVolumeInUnits = Math.Round(tmpVolume / Positions.Length, 0);

                return Info;

            }

            /// <summary>
            /// Chiude tutte le posizioni del monitor
            /// </summary>
            public void CloseAllPositions(TradeType? filtertype = null)
            {

                Update(true, null, null, 0, filtertype);

            }

            /// <summary>
            /// Stabilisce se si è in GAP passando una certa distanza da misurare
            /// </summary>
            public bool InGAP(double distance)
            {

                return Symbol.DigitsToPips(Bars.LastGAP()) >= distance;

            }

            /// <summary>
            /// Controlla la fascia oraria per determinare se rientra in quella di pausa, utilizza dati double 
            /// perchè la ctrader non permette di esporre dati time, da aggiornare non appena la ctrader lo permette
            /// </summary>
            /// <returns>Conferma la presenza di una fascia oraria in pausa</returns>
            public bool InPause(DateTime timeserver)
            {

                // -->> Poichè si utilizzano dati double per esporre i parametri dobbiamo utilizzare meccanismi per tradurre l'orario
                string nowHour = (timeserver.Hour < 10) ? string.Format("0{0}", timeserver.Hour) : string.Format("{0}", timeserver.Hour);
                string nowMinute = (timeserver.Minute < 10) ? string.Format("0{0}", timeserver.Minute) : string.Format("{0}", timeserver.Minute);

                // --> Stabilisco il momento di controllo in formato double
                double adesso = Convert.ToDouble(string.Format("{0},{1}", nowHour, nowMinute));

                // --> Confronto elementare per rendere comprensibile la logica
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

            /// <summary>
            /// Controlla ed effettua la modifica in break-even se le condizioni le permettono
            /// </summary>
            private void _checkBreakEven(Position position, BreakEvenData breakevendata)
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

            /// <summary>
            /// Controlla ed effettua la modifica in trailing se le condizioni le permettono
            /// </summary>
            private void _checkTrailing(Position position, TrailingData trailingdata)
            {

                if (trailingdata == null || trailingdata.Activation == 0 || trailingdata.Distance == 0)
                    return;

                double trailing = 0;
                double distance = Symbol.PipsToDigits(trailingdata.Distance);
                double activation = Symbol.PipsToDigits(trailingdata.Activation);

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

                            // --> Devo determinare se è partita l'attivazione
                            double activationprice = position.EntryPrice + activation;
                            double firsttrailing = Math.Round(activationprice - distance, Symbol.Digits);

                            // --> Partito il trailing? Sono in retrocessione ?
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

                            // --> Devo determinare se è partita l'attivazione
                            double activationprice = position.EntryPrice - activation;
                            double firsttrailing = Math.Round(activationprice + distance, Symbol.Digits);

                            // --> Partito il trailing? Sono in retrocessione ?
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

        /// <summary>
        /// Classe per gestire il dimensionamento delle size
        /// </summary>
        public class MonenyManagement
        {

            private readonly double _minSize = 0.01;
            private double _percentage = 0;
            private double _fixedSize = 0;
            private double _pipToCalc = 30;

            // --> Riferimenti agli oggetti esterni utili per il calcolo
            private IAccount _account = null;
            public readonly Symbol Symbol;

            /// <summary>
            /// Il capitale da utilizzare per il calcolo
            /// </summary>
            public CapitalTo CapitalType = CapitalTo.Balance;

            /// <summary>
            /// La percentuale di rischio che si vuole investire
            /// </summary>
            public double Percentage
            {

                get { return _percentage; }


                set { _percentage = (value > 0 && value <= 100) ? value : 0; }
            }

            /// <summary>
            /// La size fissa da utilizzare, bypassa tutti i parametri di calcolo
            /// </summary>
            public double FixedSize
            {

                get { return _fixedSize; }



                set { _fixedSize = (value >= _minSize) ? value : 0; }
            }


            /// <summary>
            /// La distanza massima dall'ingresso con il quale calcolare le size
            /// </summary>
            public double PipToCalc
            {

                get { return _pipToCalc; }

                set { _pipToCalc = (value > 0) ? value : 100; }
            }


            /// <summary>
            /// Il capitale effettivo sul quale calcolare il rischio
            /// </summary>
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



            // --> Costruttore
            public MonenyManagement(IAccount NewAccount, CapitalTo NewCapitalTo, double NewPercentage, double NewFixedSize, double NewPipToCalc, Symbol NewSymbol)
            {

                _account = NewAccount;

                Symbol = NewSymbol;

                CapitalType = NewCapitalTo;
                Percentage = NewPercentage;
                FixedSize = NewFixedSize;
                PipToCalc = NewPipToCalc;

            }

            /// <summary>
            /// Restituisce il numero di lotti in formato 0.01
            /// </summary>
            public double GetLotSize()
            {

                // --> Hodeciso di usare una size fissa
                if (FixedSize > 0)
                    return FixedSize;

                // --> La percentuale di rischio in denaro
                double moneyrisk = Capital / 100 * Percentage;

                // --> Traduco lo stoploss o il suo riferimento in double
                double sl_double = PipToCalc * Symbol.PipSize;

                // --> In formato 0.01 = microlotto double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);
                // --> In formato volume 1K = 1000 Math.Round((moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);
                double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);

                if (lots < _minSize)
                    return _minSize;

                return lots;

            }

        }

        #endregion

        #region Helper

        /// <summary>
        /// Restituisce il colore corrispondente a partire dal nome
        /// </summary>
        /// <returns>Il colore corrispondente</returns>
        public static API.Color ColorFromEnum(ColorNameEnum colorName)
        {

            return API.Color.FromName(colorName.ToString("G"));

        }

        #endregion

        #region Bars

        /// <summary>
        /// Si ottiene l'indice della candela partendo dal suo orario di apertura
        /// </summary>
        /// <param name="MyTime">La data e l'ora di apertura della candela</param>
        /// <returns></returns>
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

        /// <summary>
        /// Misura la grandezza di una candela, tenendo conto della sua direzione
        /// </summary>
        /// <returns>Il corpo della candela, valore uguale o superiore a zero</returns>
        public static double Body(this Bar thisBar)
        {

            return thisBar.IsBullish() ? thisBar.Close - thisBar.Open : thisBar.Open - thisBar.Close;


        }

        /// <summary>
        /// Verifica la direzione rialzista di una candela
        /// </summary>
        /// <returns>True se la candela è rialzista</returns>        
        public static bool IsBullish(this Bar thisBar)
        {

            return thisBar.Close > thisBar.Open;

        }

        /// <summary>
        /// Verifica la direzione ribassista di una candela
        /// </summary>
        /// <returns>True se la candela è ribassista</returns>        
        public static bool IsBearish(this Bar thisBar)
        {

            return thisBar.Close < thisBar.Open;

        }

        /// <summary>
        /// Verifica se una candela ha un open uguale al close
        /// </summary>
        /// <returns>True se la candela è una doji con Open e Close uguali</returns>        
        public static bool IsDoji(this Bar thisBar)
        {

            return thisBar.Close == thisBar.Open;

        }

        #endregion

        #region Symbol

        /// <summary>
        /// Converte il numero di pips corrente da digits a double
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Digits</param>
        /// <returns></returns>
        public static double DigitsToPips(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips / thisSymbol.PipSize, 2);

        }

        /// <summary>
        /// Converte il numero di pips corrente da double a digits
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Double (2)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Restituisce in minuti il timeframe corrente
        /// </summary>
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

        /// <summary>
        /// ID prodotto, identificativo, viene fornito da ctrader.guru, 60886 è il riferimento del template in uso
        /// </summary>
        public const int ID = 177874;

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "ScalFibo Indicator";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.9";

        // --> UPDATES : VARIABILI E COSTANTI

        private const string UPDATESPAGE = "https://ctrader.guru/product/scalfibo-indicator/";
        private const string LICENSEPAGE = "https://ctrader.guru/licenze/";

        // <-- UPDATES : VARIABILI E COSTANTI

        // --> VARIABILI LICENZA

        string productName = NAME;
        readonly string endpoint = "https://ctrader.guru/_checkpoint_/";

        DateTime licenzaExpire;
        CL_CTG_Licenza.LicenzaInfo licenzaInfo;
        bool exitoncalculate = false;
        public Extensions.ColorNameEnum TextColor = Extensions.ColorNameEnum.Coral;

        // <-- VARIABILI LICENZA

        #endregion

        #region Params

        /// <summary>
        /// Identità del prodotto nel contesto di ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = UPDATESPAGE)]
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
        [Parameter("Time Range Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Violet)]
        public Extensions.ColorNameEnum ColorTimeRange { get; set; }

        /// <summary>
        /// Il Box, il colore del massimo
        /// </summary>
        [Parameter("High/Open/Long Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.DodgerBlue)]
        public Extensions.ColorNameEnum ColorHigh { get; set; }

        /// <summary>
        /// Il Box, il colore del minimo
        /// </summary>
        [Parameter("Low/Close/Short Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Red)]
        public Extensions.ColorNameEnum ColorLow { get; set; }

        /// <summary>
        /// Il Box, il colore del testo
        /// </summary>
        [Parameter("Text Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Green)]
        public Extensions.ColorNameEnum ColorText { get; set; }

        [Parameter("Fibonacci Color", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Black)]
        public Extensions.ColorNameEnum ColorFibo { get; set; }

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

        /// <summary>
        /// Viene generato all'avvio dell'indicatore, si inizializza l'indicatore
        /// </summary>
        protected override void Initialize()
        {

            // --> Se il timeframe è superiore o uguale al corrente devo uscire
            if (TimeFrame != TimeFrame.Minute5)
            {

                _alertChart("USE THIS INDICATOR ONLY WITH TIMEFRAME 5 MINUTE");
                exitoncalculate = true;
                return;

            }
            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

            // --> UPDATES : CONTROLLO

            _checkProductUpdate();

            // --> CONTROLLO LICENZA
            if (RunningMode == RunningMode.RealTime)
            {
                CL_CTG_Licenza.LicenzaConfig licConfig = new CL_CTG_Licenza.LicenzaConfig
                {
                    AccountBroker = Account.BrokerName,
                    AcconuntNumber = Account.Number.ToString()
                };

                CL_CTG_Licenza licenza = new CL_CTG_Licenza(endpoint, licConfig, productName);

                try
                {

                    licenzaInfo = licenza.GetLicenza(true);

                    if (licenzaInfo == null || licenzaInfo.ErrorProc == 2 || licenzaInfo.ErrorProc == 3)
                    {

                        frmLogin LoginForm = new frmLogin(Account.BrokerName, Account.Number.ToString());
                        LoginForm.FormClosed += delegate { return; };

                        LoginForm.ShowDialog();
                        exitoncalculate = true;
                        return;

                    }
                    else
                    {

                        // --> Ho inizializzato perchè non voglio la chiamata al server
                        if (licenzaInfo.Product == "")
                        {

                            
                            if (licenzaInfo.ErrorProc == 4)
                            {

                                _alertChart("Problem with server, please try again!".ToUpper());
                                _removeCookieAndLicense(licenza);
                                exitoncalculate = true;
                                return;

                            }
                            else
                            {

                                _alertChart("LICENSE NOT FOUND, please try again!".ToUpper());
                                _removeCookieAndLicense(licenza);
                                exitoncalculate = true;
                                return;

                            }

                        }
                        else if (!licenzaInfo.Login)
                        {

                            _alertChart("Email or Password wrong, please try again".ToUpper());
                            _removeCookieAndLicense(licenza);
                            exitoncalculate = true;
                            return;

                        }
                        else
                        {

                            if (licenzaInfo.Product.CompareTo(productName.ToUpper()) != 0)
                            {

                                if (MessageBox.Show("Not for this product, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                    _removeCookieAndLicense(licenza);

                                exitoncalculate = true;
                                return;

                            }
                            else
                            {

                                if ((licenzaInfo.AccountBroker.CompareTo("*") != 0 && licenzaInfo.AccountBroker.CompareTo(Account.BrokerName) != 0) || (licenzaInfo.AccountNumber.CompareTo("*") != 0 && licenzaInfo.AccountNumber.CompareTo(Account.Number.ToString()) != 0))
                                {

                                    if (MessageBox.Show("Not for this account, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                        _removeCookieAndLicense(licenza);

                                    exitoncalculate = true;
                                    return;

                                }
                                else
                                {

                                    if (licenzaInfo.Expire == null || licenzaInfo.Expire.Length < 1)
                                    {

                                        if (MessageBox.Show("Expired, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                            _removeCookieAndLicense(licenza);

                                        exitoncalculate = true;
                                        return;


                                    }
                                    else if (licenzaInfo.Expire.CompareTo("*") != 0)
                                    {

                                        try
                                        {

                                            String[] substringsExpire = licenzaInfo.Expire.Split(',');

                                            licenzaExpire = new DateTime(Int32.Parse(substringsExpire[0].Trim()), Int32.Parse(substringsExpire[1].Trim()), Int32.Parse(substringsExpire[2].Trim()), Int32.Parse(substringsExpire[3].Trim()), Int32.Parse(substringsExpire[4].Trim()), Int32.Parse(substringsExpire[5].Trim()));


                                            if (DateTime.Compare(licenzaExpire, Server.Time) > 0)
                                            {

                                                Print("Expire : " + licenzaExpire.ToString());

                                            }
                                            else
                                            {

                                                if (MessageBox.Show("Expired, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                                    _removeCookieAndLicense(licenza);

                                                exitoncalculate = true;
                                                return;

                                            }

                                        }
                                        catch
                                        {

                                            if (MessageBox.Show("Expired, remove cookie session?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                                                _removeCookieAndLicense(licenza);

                                            exitoncalculate = true;
                                            return;

                                        }

                                    }
                                    else
                                    {

                                        Print("Lifetime");

                                    }

                                }

                            }

                        }

                    }

                }
                catch (Exception exp)
                {

                    MessageBox.Show("Encryption issue, contact support@ctrader.guru", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    licenza.RemoveLicense();
                    exitoncalculate = true;

                    Print("Debug : " + exp.Message);

                    return;

                }

            }
            // <-- CONTROLLO LICENZA

        }

        /// <summary>
        /// Generato ad ogni tick, vengono effettuati i calcoli dell'indicatore
        /// </summary>
        /// <param name="index">L'indice della candela in elaborazione</param>
        public override void Calculate(int index)
        {

            if (TimeFrame != TimeFrame.Minute5)
                return;

            // --> CONTROLLO LICENZA
            if (RunningMode == RunningMode.RealTime)
            {

                try
                {

                    if (exitoncalculate)
                    {

                        return;

                    }
                    else if (licenzaInfo.Expire.CompareTo("*") != 0)
                    {

                        if (DateTime.Compare(licenzaExpire, Server.Time) > 0)
                        {

                            // TODO not expired

                        }
                        else
                        {
                            _alertChart("Expired");
                            return;

                        }

                    }

                }
                catch
                {

                    Chart.DrawStaticText("Expired", string.Format("{0} : Licence expired!", NAME), VerticalAlignment.Center, API.HorizontalAlignment.Center, Color.Red);

                    return;

                }


            }
            // <-- CONTROLLO LICENZA

            try
            {

                _drawLevelFromCustomBar(index);

                // --> Evito di mostrare l'alert all'avvio
                if (IsLastBar)
                    canAlert = true;

            }
            catch (Exception exp)
            {

                Chart.DrawStaticText("Alert", string.Format("{0} : error, {1}", NAME, exp), VerticalAlignment.Center, API.HorizontalAlignment.Center, Color.Red);

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
            double treuLevelDiffInDigits = Math.Round(TrueDiff * Symbol.PipSize, Symbol.Digits);
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

                fiboDrawded = false;

                canAlert = false;

                alertHitOnFibo = false;
                alertHitOnStrategyHit = false;
                alertHitOnTradeOppo = false;

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

                        // --> Ogni volta che si riaggiorna Fibo resetto l'alert flag per l'opportunity
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

                        // --> Ogni volta che si riaggiorna Fibo resetto l'alert flag per l'opportunity
                        alertHitOnTradeOppo = false;

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


            if (AlertOnStrategyHit && trategyEnd && !alertHitOnStrategyHit)
            {

                alertHitOnStrategyHit = true;
                _alert("Strategy Hit!");

            }

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
                    if (!checked50)
                        checked50 = ((LastIndex5CheckedHigh < LastIndex5CheckedLow && Bars[index5m].High >= Fibo50) || (LastIndex5CheckedHigh > LastIndex5CheckedLow && Bars[index5m].Low <= Fibo50));

                    if (!checked38)
                        checked38 = periodZone && ((LastIndex5CheckedHigh < LastIndex5CheckedLow && Bars[index5m].High >= Fibo38) || (LastIndex5CheckedHigh > LastIndex5CheckedLow && Bars[index5m].Low <= Fibo38));

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

                    if (AlertOnFibo && !alertHitOnFibo)
                    {

                        alertHitOnFibo = true;
                        _alert("Fibonacci retracement drawing!");

                    }

                    fiboDrawded = true;

                }


            }

            if (AlertOnOppo && !alertHitOnTradeOppo && !trategyEnd && fiboDrawded && periodZone)
            {

                alertHitOnTradeOppo = true;
                _alert("Trade Opportunity!");

            }

            info += string.Format(padding + "{0} Strategy Hit!\r\n", trategyEnd ? YES : NO);
            info += string.Format(padding + "... {0} Fibo 50\r\n", checked50 ? YES : NO);
            info += string.Format(padding + "... {0} Trigger\r\n\r\n", checked38 ? YES : NO);

            if (canAlert)
            {

                info += string.Format(padding + "{0} On Trade Opportunity {1}\r\n", AlertOnOppo ? ALERTON : ALERTOFF, alertHitOnTradeOppo ? YES : NO);
                info += string.Format(padding + "{0} On Fibonacci Drawing {1}\r\n", AlertOnFibo ? ALERTON : ALERTOFF, alertHitOnFibo ? YES : NO);
                info += string.Format(padding + "{0} On Strategy Hit {1}\r\n", AlertOnStrategyHit ? ALERTON : ALERTOFF, alertHitOnStrategyHit ? YES : NO);

            }


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

        /// <summary>
        /// Effettua un controllo sul sito ctrader.guru per mezzo delle API per verificare la presenza di aggiornamenti, solo in realtime
        /// </summary>
        private void _checkProductUpdate()
        {

            // --> Controllo solo se sono in realtime, evito le chiamate in backtest
            if (RunningMode != RunningMode.RealTime)
                return;

            // --> Organizzo i dati per la richiesta degli aggiornamenti
            Guru.API.RequestProductInfo Request = new Guru.API.RequestProductInfo
            {

                MyProduct = new Guru.Product
                {

                    ID = ID,
                    Name = NAME,
                    Version = VERSION

                },
                AccountBroker = Account.BrokerName,
                AccountNumber = Account.Number

            };

            // --> Effettuo la richiesta
            Guru.API Response = new Guru.API(Request);

            // --> Controllo per prima cosa la presenza di errori di comunicazioni
            if (Response.ProductInfo.Exception != "")
            {

                Print("{0} Exception : {1}", NAME, Response.ProductInfo.Exception);

            }
            // --> Chiedo conferma della presenza di nuovi aggiornamenti
            else if (Response.HaveNewUpdate())
            {

                string updatemex = string.Format("{0} : Updates available {1} ( {2} )", NAME, Response.ProductInfo.LastProduct.Version, Response.ProductInfo.LastProduct.Updated);

                // --> Informo l'utente con un messaggio sul grafico e nei log del cbot
                Chart.DrawStaticText(NAME + "Updates", updatemex, VerticalAlignment.Top, cAlgo.API.HorizontalAlignment.Left, Extensions.ColorFromEnum(TextColor));
                Print(updatemex);

            }

        }

        private void _removeCookieAndLicense(CL_CTG_Licenza licenza)
        {

            licenza.RemoveCookie();
            licenza.RemoveLicense();

        }

        private void _alertChart(string mymex)
        {

            if (RunningMode != RunningMode.RealTime)
                return;

            string mex = string.Format("{0} : {1}", NAME.ToUpper(), mymex);

            Chart.DrawStaticText("alert", mex, VerticalAlignment.Center, API.HorizontalAlignment.Center, Color.Red);
            Print(mex);

        }

        private void _alert(string mymex)
        {

            if (!canAlert || RunningMode != RunningMode.RealTime)
                return;

            string mex = string.Format("{0} : {1} {2}", NAME, SymbolName, mymex);

            new Thread(new ThreadStart(delegate { MessageBox.Show(mex, NAME, MessageBoxButtons.OK, MessageBoxIcon.Information); })).Start();
            Print(mex);

        }

        #endregion

    }
}


/// <summary>
/// NameSpace che racchiude tutte le feature ctrader.guru
/// </summary>
namespace Guru
{
    /// <summary>
    /// Classe che definisce lo standard identificativo del prodotto nel marketplace ctrader.guru
    /// </summary>
    public class Product
    {

        public int ID = 0;
        public string Name = "";
        public string Version = "";
        public string Updated = "";
        public string LastCheck = "";

    }

    /// <summary>
    /// Offre la possibilità di utilizzare le API messe a disposizione da ctrader.guru per verificare gli aggiornamenti del prodotto.
    /// Permessi utente "AccessRights = AccessRights.FullAccess" per accedere a internet ed utilizzare JSON
    /// </summary>
    public class API
    {
        /// <summary>
        /// Costante da non modificare, corrisponde alla pagina dei servizi API
        /// </summary>
        private const string Service = "https://ctrader.guru/api/product_info/";

        private static string MainPath = string.Format("{0}\\cAlgo\\cTraderGuru\\", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        private readonly string InfoFile = string.Format("{0}update", MainPath);

        /// <summary>
        /// Costante da non modificare, utilizzata per filtrare le richieste
        /// </summary>
        private const string UserAgent = "cTrader Guru";

        /// <summary>
        /// Variabile dove verranno inserite le direttive per la richiesta
        /// </summary>
        private RequestProductInfo RequestProduct = new RequestProductInfo();

        /// <summary>
        /// Variabile dove verranno inserite le informazioni identificative dal server dopo l'inizializzazione della classe API
        /// </summary>
        public ResponseProductInfo ProductInfo = new ResponseProductInfo();

        /// <summary>
        /// Classe che formalizza i parametri di richiesta, vengono inviate le informazioni del prodotto e di profilazione a fini statistici
        /// </summary>
        public class RequestProductInfo
        {

            /// <summary>
            /// Il prodotto corrente per il quale richiediamo le informazioni
            /// </summary>
            public Product MyProduct = new Product();

            /// <summary>
            /// Broker con il quale effettiamo la richiesta
            /// </summary>
            public string AccountBroker = "";

            /// <summary>
            /// Il numero di conto con il quale chiediamo le informazioni
            /// </summary>
            public int AccountNumber = 0;

        }

        /// <summary>
        /// Classe che formalizza lo standard per identificare le informazioni del prodotto
        /// </summary>
        public class ResponseProductInfo
        {

            /// <summary>
            /// Il prodotto corrente per il quale vengono fornite le informazioni
            /// </summary>
            public Product LastProduct = new Product();

            /// <summary>
            /// Eccezioni in fase di richiesta al server, da utilizzare per controllare l'esito della comunicazione
            /// </summary>
            public string Exception = "";

            /// <summary>
            /// La risposta del server
            /// </summary>
            public string Source = "";

        }

        /// <summary>
        /// Richiede le informazioni del prodotto richiesto
        /// </summary>
        /// <param name="Request"></param>
        public API(RequestProductInfo Request)
        {

            RequestProduct = Request;

            // --> Non controllo se non ho l'ID del prodotto
            if (Request.MyProduct.ID <= 0)
                return;

            string cleanedproduct = string.Join("-", Request.MyProduct.Name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            string fileToCheck = InfoFile + "-" + cleanedproduct.ToUpper() + ".json";

            // --> Controllo che siano passati almeno 30minuti tra una richiesta e l'altra
            try
            {

                string infodata = File.ReadAllText(fileToCheck);

                Product infolocal = JsonConvert.DeserializeObject<Product>(infodata);

                if (infolocal.LastCheck != "" && infolocal.ID == Request.MyProduct.ID)
                {

                    DateTime timeToTrigger = DateTime.Parse(infolocal.LastCheck).AddMinutes(60);

                    // --> Controllo se ci sono le condizioni per procedere
                    if (DateTime.Compare(timeToTrigger, DateTime.Now) > 0)
                    {

                        ProductInfo.LastProduct = infolocal;
                        return;

                    }

                }

            }
            catch
            {

            }

            // --> Dobbiamo supervisionare la chiamata per registrare l'eccexione
            try
            {

                // --> Strutturo le informazioni per la richiesta POST
                NameValueCollection data = new NameValueCollection
                {
                    {
                        "account_broker",
                        Request.AccountBroker
                    },
                    {
                        "account_number",
                        Request.AccountNumber.ToString()
                    },
                    {
                        "my_version",
                        Request.MyProduct.Version
                    },
                    {
                        "productid",
                        Request.MyProduct.ID.ToString()
                    }
                };

                // --> Autorizzo tutte le pagine di questo dominio
                Uri myuri = new Uri(Service);
                string pattern = string.Format("{0}://{1}/.*", myuri.Scheme, myuri.Host);

                Regex urlRegEx = new Regex(pattern);
                WebPermission p = new WebPermission(NetworkAccess.Connect, urlRegEx);
                p.Assert();

                // --> Protocollo di sicurezza https://
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

                // -->> Richiedo le informazioni al server
                using (var wb = new WebClient())
                {

                    wb.Headers.Add("User-Agent", UserAgent);

                    var response = wb.UploadValues(myuri, "POST", data);
                    ProductInfo.Source = Encoding.UTF8.GetString(response);

                }

                // -->>> Nel cBot necessita l'attivazione di "AccessRights = AccessRights.FullAccess"
                ProductInfo.LastProduct = JsonConvert.DeserializeObject<Product>(ProductInfo.Source);
                ProductInfo.LastProduct.LastCheck = DateTime.Now.ToString();

                // --> Aggiorno il file locale
                try
                {

                    Directory.CreateDirectory(MainPath);

                    File.WriteAllText(fileToCheck, JsonConvert.SerializeObject(ProductInfo.LastProduct));

                }
                catch
                {
                }

            }
            catch (Exception Exp)
            {

                // --> Qualcosa è andato storto, registro l'eccezione
                ProductInfo.Exception = Exp.Message;

            }

        }

        /// <summary>
        /// Esegue un confronto tra le versioni per determinare la presenza di aggiornamenti
        /// </summary>
        /// <returns></returns>
        public bool HaveNewUpdate()
        {

            // --> Voglio essere sicuro che stiamo lavorando con le informazioni giuste
            return (ProductInfo.LastProduct.ID == RequestProduct.MyProduct.ID && ProductInfo.LastProduct.Version != "" && RequestProduct.MyProduct.Version != "" && ProductInfo.LastProduct.Version != null && new Version(RequestProduct.MyProduct.Version).CompareTo(new Version(ProductInfo.LastProduct.Version)) < 0);

        }

    }

}

