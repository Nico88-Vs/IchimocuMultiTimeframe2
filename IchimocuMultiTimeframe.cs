// Copyright QUANTOWER LLC. © 2017-2021. All rights reserved.

using System;
using System.Drawing;
using System.Globalization;
using TradingPlatform.BusinessLayer;

namespace Custom
{
    /// <summary>
    /// this inidicator display 2 new Ichimoku Indicator based on 2 different timeframes resulted by a moltiplication of chart time-frame
    /// (if multiplaier 2 = 0 sencond timeframe will be ignored); multiplaier can't be negative.
    /// </summary>
    public class IchimocuMultiTimeframe : Indicator
    {
        #region Global var

        // Const line ==>> Just assigne a name to the line Index in order to avoid mistacke
        private int Tenkan_Sen = 0;

        private int Kijun_Sen = 1;
        private int Chikou_Span = 2;
        private int Senkou_SpanA = 3;
        private int Senkou_SpanB = 4;

        // Const line second Time Frame
        private int Tenkan_Sen2 = 5;

        private int Kijun_Sen2 = 6;
        private int Chikou_Span2 = 7;
        private int Senkou_SpanA2 = 8;
        private int Senkou_SpanB2 = 9;

        // Declaring a multiplaier for each new Time Frame
        private int multiplaier;

        private int multiplaier2;

        // Declaring tow variable of type trend just for set the clouds color
        private Trend currenTrend;

        private Trend currenTrend2;

        //Define min history dep as the longest period
        public int MinHistoryDepth => Math.Max(this.TenkanPeriod, Math.Max(this.KijounPeriod, this.SekuSpanBPeriod));

        #endregion Global var

        #region input

        //Setting Users imput
        [InputParameter("Tenkan Sen", 0, 1, 999, 1, 0)]     //nome,indice,valore.min,valore.max,precision
        public int TenkanPeriod = 9;

        [InputParameter("Kijoun Sen", 1, 1, 999, 1, 0)]
        public int KijounPeriod = 26;

        [InputParameter("SekuSpanB", 2, 1, 999, 1, 0)]
        public int SekuSpanBPeriod = 52;

        [InputParameter("CloudUp", 3)]
        public Color CloudUpColor = Color.FromArgb(50, Color.Green);

        [InputParameter("CloudUp2", 3)]
        public Color CloudUpColor2 = Color.FromArgb(50, Color.Green);

        [InputParameter("CloudDown", 3)]
        public Color CloudDownColor = Color.FromArgb(50, Color.Red);   // ATTENZIONE VERIFICARE FUNZIONAMENTO INDICE

        [InputParameter("CloudDown2", 3)]
        public Color CloudDownColor2 = Color.FromArgb(50, Color.Red);

        [InputParameter("Multiplaier", 4, 1, 20, 0)]
        public int Moltiplicatore = 5;

        [InputParameter("MultiplaierSecondo", 4, 1, 60, 0)]
        public int Moltiplicatore2 = 1;

        #endregion input

        #region costructors

        //indicator costructor
        public IchimocuMultiTimeframe()
            : base()
        {
            // Defines indicator's name and description.
            Name = "Ichi MultiTF ";
            Description = "Get ichi from  higer Tf based on a multiple of the current Tf";

            // Defines line on demand with particular parameters.
            AddLineSeries("Tenkan-sen", Color.Blue, 1, LineStyle.Solid);
            AddLineSeries("Kijun-sen", Color.Red, 1, LineStyle.Solid);
            AddLineSeries("Chikou-span", Color.Orange, 1, LineStyle.Solid);
            AddLineSeries("SenkunSpan-a", Color.Green, 1, LineStyle.Solid);
            AddLineSeries("SenkunSpan-b", Color.Red, 1, LineStyle.Solid);

            //second indicator
            AddLineSeries("Tenkan-sen2", Color.Blue, 1, LineStyle.Solid);
            AddLineSeries("Kijun-sen2", Color.Red, 1, LineStyle.Solid);
            AddLineSeries("Chikou-span2", Color.Orange, 1, LineStyle.Solid);
            AddLineSeries("SenkunSpan-a2", Color.Green, 1, LineStyle.Solid);
            AddLineSeries("SenkunSpan-b2", Color.Red, 1, LineStyle.Solid);

            // By default indicator will be applied on main window of the chart
            SeparateWindow = false;
        }

        // Costructor with parameters ==>> sets periods as user selected
        public IchimocuMultiTimeframe(int tenkanPeriod, int kijounPeriod, int sekuSpanBPeriod, int firstMultiplaier, int secondMultiplaier) : this()
        {
            this.TenkanPeriod = tenkanPeriod;
            this.KijounPeriod = kijounPeriod;
            this.SekuSpanBPeriod = sekuSpanBPeriod;
            this.multiplaier = firstMultiplaier;
            this.multiplaier2 = secondMultiplaier;
        }

        #endregion costructors

        #region execution

        protected override void OnInit()
        {
            //set indicator multiplaier as user selected
            multiplaier = Moltiplicatore;
            multiplaier2 = Moltiplicatore2;

            // Shifts first time frame's line by the multiplaier in order to get the right x cordinate of the value
            this.LinesSeries[Chikou_Span].TimeShift = -this.KijounPeriod * multiplaier;
            this.LinesSeries[Senkou_SpanA].TimeShift = this.KijounPeriod * multiplaier;
            this.LinesSeries[Senkou_SpanB].TimeShift = this.KijounPeriod * multiplaier;

            // Shifts second time frame's line by the multiplaier in order to get the right x cordinate of the value
            this.LinesSeries[Chikou_Span2].TimeShift = -this.KijounPeriod * multiplaier2;
            this.LinesSeries[Senkou_SpanA2].TimeShift = this.KijounPeriod * multiplaier2;
            this.LinesSeries[Senkou_SpanB2].TimeShift = this.KijounPeriod * multiplaier2;
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            // Min History Condition
            if (this.Count < MinHistoryDepth) { return; }

            //                     !!!!!!! FINITO !!!!!!
            //cikou_span
            //SetValue(Close(), Chikou_Span);

            //tenkansen
            double tenkan = GetAvarage(TenkanPeriod, multiplaier);
            SetValue(tenkan, Tenkan_Sen);

            //kijoun
            double kijoun = GetAvarage(KijounPeriod, multiplaier);
            SetValue(kijoun, Kijun_Sen);

            //spanA
            double spana = (GetAvarage(TenkanPeriod, multiplaier) + GetAvarage(KijounPeriod, multiplaier)) / 2;
            SetValue(spana, Senkou_SpanA);

            //spanB
            double spanb = GetAvarage(SekuSpanBPeriod, multiplaier);
            SetValue(spanb, Senkou_SpanB);

            //seconde linee
            if (multiplaier2 > 1)
            {
                //cikou_span
                SetValue(Close(), Chikou_Span);

                //tenkansen
                double tenkan2 = GetAvarage(TenkanPeriod, multiplaier2);
                SetValue(tenkan2, Tenkan_Sen2);

                //kijoun
                double kijoun2 = GetAvarage(KijounPeriod, multiplaier2);
                SetValue(kijoun2, Kijun_Sen2);

                //spanA
                double spana2 = (GetAvarage(TenkanPeriod, multiplaier2) + GetAvarage(KijounPeriod, multiplaier2)) / 2;
                SetValue(spana2, Senkou_SpanA2);

                //spanB
                double spanb2 = GetAvarage(SekuSpanBPeriod, multiplaier2);
                SetValue(spanb2, Senkou_SpanB2);

                //gestione della nuvola
                var newTrend2 = spana2 == spanb2 ? Trend.Unknown : spana2 > spanb2 ? Trend.Up : Trend.Down;

                if (this.currenTrend2 != newTrend2)
                {
                    this.EndCloud(Senkou_SpanA2, Senkou_SpanB2, this.GetColorByTrend2(this.currenTrend2));
                    this.BeginCloud(Senkou_SpanA2, Senkou_SpanB2, this.GetColorByTrend2(newTrend2));
                }

                this.currenTrend2 = newTrend2;
            }

            //gestione della nuvola
            var newTrend = spana == spanb ? Trend.Unknown : spana > spanb ? Trend.Up : Trend.Down;

            if (this.currenTrend != newTrend)
            {
                this.EndCloud(Senkou_SpanA, Senkou_SpanB, this.GetColorByTrend(this.currenTrend));
                this.BeginCloud(Senkou_SpanA, Senkou_SpanB, this.GetColorByTrend(newTrend));
            }

            this.currenTrend = newTrend;
        }

        #endregion execution

        #region paint

        public override void OnPaintChart(PaintChartEventArgs args)
        {
            var gr = args.Graphics;
            Font f = new Font("Arial", 10);

            //try draw
            gr.DrawString("First multiplaier : x " + multiplaier, f, Brushes.LightGreen, 10, 75);
            gr.DrawString("Second multiplaier : x " + multiplaier2, f, Brushes.LightGreen, 10, 100);
        }

        #endregion paint

        #region GetValue

        //Get Ichimoku values
        public double GetAvarage(int period, int multiplaier)
        {
            double high = this.GetPrice(PriceType.High);
            double low = this.GetPrice(PriceType.Low);

            for (int i = 1; i < period * multiplaier; i++)
            {
                double price = this.GetPrice(PriceType.High, i);
                if (price > high) { high = price; }

                double priceLow = this.GetPrice(PriceType.Low, i);
                if (priceLow < low) { low = priceLow; }
            }

            double avarage = (high + low) / 2;

            return avarage;
        }

        private enum Trend
        {
            Unknown,
            Up,
            Down,
        }

        private Color GetColorByTrend(Trend trend) => trend switch
        {
            Trend.Up => this.CloudUpColor,
            Trend.Down => this.CloudDownColor,
            _ => Color.Empty
        };

        private Color GetColorByTrend2(Trend trend) => trend switch
        {
            Trend.Up => this.CloudUpColor2,
            Trend.Down => this.CloudDownColor2,
            _ => Color.Empty
        };

        #endregion GetValue
    }
}