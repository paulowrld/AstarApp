public class MapaGrid : Mapa
{
    public int[,] mapa;
    public int[,] mapa2;

    public MapaGrid(int Largura, int Altura, int tilestelaX, int tilestelaY) : base(null, tilestelaX, tilestelaY)
    {
        this.Altura = Altura;
        this.Largura = Largura;

        mapa = new int[Altura, Largura];
    }

    public void Loadmapfromimage(string filename)
    {
        string path = System.IO.Path.Combine(Application.StartupPath, "Images", filename);
        Console.WriteLine($"Tentando carregar imagem do caminho: {path}");

        if (!System.IO.File.Exists(path))
        {
            throw new Exception("Arquivo de imagem não encontrado em: " + path);
        }

        Bitmap imgtmp = null;
        Bitmap imagem = null;
        try
        {
            imgtmp = new Bitmap(path);
            imagem = new Bitmap(imgtmp.Width, imgtmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using Graphics g = Graphics.FromImage(imagem);
            g.DrawImage(imgtmp, 0, 0);
        }
        catch (IOException e)
        {
            Console.WriteLine(e.StackTrace);
        }

        this.Altura = imagem.Width;
        this.Largura = imagem.Height;

        mapa = new int[Altura, Largura];

        int cor = imagem.GetPixel(0, 0).ToArgb();
        for (int j = 0; j < imagem.Height; j++)
        {
            for (int i = 0; i < imagem.Width; i++)
            {
                int cor1 = imagem.GetPixel(i, j).ToArgb();
                if (cor == cor1)
                    mapa[j, i] = 0;
                else
                    mapa[j, i] = 1;
            }
        }
    }

    public override void DesenhaSe(Graphics dbg)
    {
        int offx = MapX & 0x0f;
        int offy = MapY & 0x0f;
        int somax, somay;
        if (offx > 0)
            somax = 1;
        else
            somax = 0;
        if (offy > 0)
            somay = 1;
        else
            somay = 0;

        dbg.DrawLine(Pens.LightGray, 0, 0, 0, 0); // exemplo para manter a ordem (linhas de grade)
        for (int j = 0; j < NumeroTilesY + somay; j++)
        {
            dbg.DrawLine(Pens.LightGray, 0, (j << 4) - offy, NumeroTilesX * 16, (j << 4) - offy);
        }
        for (int i = 0; i < NumeroTilesX + somax; i++)
        {
            dbg.DrawLine(Pens.LightGray, (i << 4) - offx, 0, (i << 4) - offx, NumeroTilesY * 16);
        }

        dbg.FillRectangle(Brushes.Black, 0, 0, 1, 1);
        for (int j = 0; j < NumeroTilesY + somay; j++)
        {
            for (int i = 0; i < NumeroTilesX + somax; i++)
            {
                if (mapa[(MapY >> 4) + j, (MapX >> 4) + i] > 0)
                {
                    dbg.FillRectangle(Brushes.Black, (i << 4) - offx, (j << 4) - offy, 16, 16);
                }
            }
        }
    }

    public void Posiciona(int x, int y)
    {
        int Xtemp = x >> 4;
        int Ytemp = y >> 4;

        if (Xtemp < 0)
            MapX = 0;
        else if (Xtemp >= (Largura - NumeroTilesX))
            MapX = (Largura - NumeroTilesX) << 4;
        else
            MapX = x;

        if (Ytemp < 0)
            MapY = 0;
        else if (Ytemp >= (Altura - NumeroTilesY))
            MapY = (Altura - NumeroTilesY) << 4;
        else
            MapY = y;
    }

    public int ReadCInt(System.IO.BinaryReader data)
    {
        int b1 = data.ReadByte();
        int b2 = data.ReadByte();
        int b3 = data.ReadByte();
        int b4 = data.ReadByte();
        int dado = (b1 & 0x00ff) | ((b2 & 0x00ff) << 8) | ((b3 & 0x00ff) << 16) | ((b4 & 0x00ff) << 24);
        return dado;
    }
}