using System.Collections.Generic;

public class PrintData
{
    public string Orientation { get; set; }

    public Printable Color { get; set; }

    public Printable Black { get; set; }
}

public class Printable
{

    public Printable()
    {
        Images = new List<PrinterImage>();
        Lines = new List<Line>();
    }

    public List<PrinterImage> Images { get; set; }

    public List<Line> Lines { get; set; }
}

public class Line
{
    public string Text { get; set; }

    public Position Position { get; set; }

    public PrinterFont Font { get; set; }

    public PrinterColor Color { get; set; }
}

public class PrinterImage
{
    public string Url { get; set; }
    public string UrlVariable { get; set; }
    public uint X { get; set; }
    public uint Y { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public string Unit { get; set; }
}

public class Position
{
    public Position()
    {
        Unit = "pixel";
    }
    public int X { get; set; }
    public int Y { get; set; }
    public uint FieldWidth { get; set; }
    public string Unit { get; set; }
}

public class PrinterFont
{
    public string Name { get; set; }
    public uint Size { get; set; }
    public int Weight { get; set; }
}

public class PrinterColor
{
    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }
}

