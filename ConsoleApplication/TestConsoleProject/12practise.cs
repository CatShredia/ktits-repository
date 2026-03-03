namespace practise12
{
    class Cup
    {
        // высота
        public double Height { get; set; }
        // диаметр
        public double Diameter { get; set; }

        // стакан
        public Cup(double height, double diameter)
        {
            Height = height;
            Diameter = diameter;
        }

        // определение объема
        public double Volume()
        {
            return Math.PI * Math.Pow(Diameter / 2, 2) * Height;
        }

        // определение заполненого объекма в %
        public double FillPercent(double fillHeight)
        {
            if (fillHeight > Height) fillHeight = Height;
            if (fillHeight < 0) fillHeight = 0;
            return (fillHeight / Height) * 100;
        }

        // масса воды
        public double LiquidMass(double fillHeight, double density)
        {
            if (fillHeight > Height) fillHeight = Height;
            if (fillHeight < 0) fillHeight = 0;
            return density * Math.PI * Math.Pow(Diameter / 2, 2) * fillHeight;
        }
    }
}