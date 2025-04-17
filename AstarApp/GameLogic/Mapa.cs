using System;
using System.Drawing;
using System.IO;

public class Mapa
{
    public int Altura = 70;
    public int Largura = 100;
    public int NumeroTilesX = 0;
    public int NumeroTilesY = 0;
    public int MapX = 0;
    public int MapY = 0;
    public int TilePLinhaTileset = 0;

    public void CarregaTiles() { }

    public Image TileSet = null;

    public int[][] mapa;
    public int[][] mapa2;

    public Mapa(Image tileset, int tilestelaX, int tilestelaY)
    {
        NumeroTilesX = tilestelaX;
        NumeroTilesY = tilestelaY;
        TileSet = tileset;
        MapX = 0;
        MapY = 0;
    }

    public void AbreMapa(string nomemapa)
    {
        try
        {
            Stream In = GetType().Assembly.GetManifestResourceStream(nomemapa);
            BinaryReader data = new BinaryReader(In);

            int Versao = data.ReadInt32(); // lê Versao
            Largura = ReadCInt(data);    // lê Largura
            Altura = ReadCInt(data);     // lê Altura

            Console.WriteLine(" Largura " + Largura);
            Console.WriteLine(" Altura " + Altura);

            int ltilex = ReadCInt(data); // lê Larg Tile
            int ltiley = ReadCInt(data); // lê Altura Tile

            Console.WriteLine(" ltilex " + ltilex);
            Console.WriteLine(" ltiley " + ltiley);

            byte[] nome = new byte[32];
            data.Read(nome, 0, 32);       // lê Nome Tilemap
            data.Read(nome, 0, 32);

            int numLayers = ReadCInt(data); // lê numero de Layers
            int numTiles = ReadCInt(data);  // lê numero de Tiles

            Console.WriteLine(" numLayers " + numLayers);
            Console.WriteLine(" numTiles " + numTiles);

            int BytesPorTiles = ReadCInt(data); // lê numero de bytes por tile;
            Console.WriteLine(" BytesPorTiles " + BytesPorTiles);

            int vago1 = ReadCInt(data); // lê vago;
            int vago2 = ReadCInt(data); // lê vago;

            // Inicializa os arrays jagged
            mapa = new int[Altura][];
            mapa2 = new int[Altura][];
            for (int j = 0; j < Altura; j++)
            {
                mapa[j] = new int[Largura];
                mapa2[j] = new int[Largura];
            }

            if (BytesPorTiles == 1)
            {
                for (int j = 0; j < Altura; j++)
                {
                    for (int i = 0; i < Largura; i++)
                    {
                        int b1 = data.ReadByte();
                        int b2 = data.ReadByte();
                        int dado = (b1 & 0x00ff) | ((b2 & 0x00ff) << 8);
                        mapa[j][i] = dado;
                    }
                }
                if (numLayers == 2)
                {
                    for (int j = 0; j < Altura; j++)
                    {
                        for (int i = 0; i < Largura; i++)
                        {
                            int b1 = data.ReadByte();
                            int b2 = data.ReadByte();
                            int dado = (b1 & 0x00ff) | ((b2 & 0x00ff) << 8);
                            mapa2[j][i] = dado;
                        }
                    }
                }
            }
            else if (BytesPorTiles == 2)
            {
                for (int j = 0; j < Altura; j++)
                {
                    for (int i = 0; i < Largura; i++)
                    {
                        int b1 = data.ReadByte();
                        int b2 = data.ReadByte();
                        int b3 = data.ReadByte();
                        int b4 = data.ReadByte();
                        int dado = (b1 & 0x00ff) | ((b2 & 0x00ff) << 8) | ((b3 & 0x00ff) << 16) | ((b4 & 0x00ff) << 24);
                        mapa[j][i] = dado;
                    }
                }
                if (numLayers == 2)
                {
                    for (int j = 0; j < Altura; j++)
                    {
                        for (int i = 0; i < Largura; i++)
                        {
                            int b1 = data.ReadByte();
                            int b2 = data.ReadByte();
                            int b3 = data.ReadByte();
                            int b4 = data.ReadByte();
                            int dado = (b1 & 0x00ff) | ((b2 & 0x00ff) << 8) | ((b3 & 0x00ff) << 16) | ((b4 & 0x00ff) << 24);
                            mapa2[j][i] = dado;
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < Altura; j++)
                {
                    for (int i = 0; i < Largura; i++)
                    {
                        int b1 = data.ReadByte();
                        int dado = (b1 & 0x00ff);
                        mapa[j][i] = dado;
                    }
                }
                if (numLayers == 2)
                {
                    for (int j = 0; j < Altura; j++)
                    {
                        for (int i = 0; i < Largura; i++)
                        {
                            int b1 = data.ReadByte();
                            int dado = (b1 & 0x00ff);
                            mapa2[j][i] = dado;
                        }
                    }
                }
            }

            data.Close();
            In.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + "  abreaMapaPau!!!");
        }
    }

    public virtual void DesenhaSe(Graphics dbg)
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

        for (int j = 0; j < NumeroTilesY + somay; j++)
        {
            for (int i = 0; i < NumeroTilesX + somax; i++)
            {
                int tilex = (mapa[j + (MapY >> 4)][i + (MapX >> 4)] % TilePLinhaTileset) << 4;
                int tiley = (mapa[j + (MapY >> 4)][i + (MapX >> 4)] / TilePLinhaTileset) << 4;
                dbg.DrawImage(TileSet, new Rectangle((i << 4) - offx, (j << 4) - offy, 16, 16),
                              tilex, tiley, 16, 16, GraphicsUnit.Pixel);
            }
        }
        for (int j = 0; j < NumeroTilesY + somay; j++)
        {
            for (int i = 0; i < NumeroTilesX + somax; i++)
            {
                int tilex = (mapa2[j + (MapY >> 4)][i + (MapX >> 4)] % TilePLinhaTileset) << 4;
                int tiley = (mapa2[j + (MapY >> 4)][i + (MapX >> 4)] / TilePLinhaTileset) << 4;
                dbg.DrawImage(TileSet, new Rectangle((i << 4) - offx, (j << 4) - offy, 16, 16),
                              tilex, tiley, 16, 16, GraphicsUnit.Pixel);
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

    public int ReadCInt(BinaryReader data)
    {
        int b1 = data.ReadByte();
        int b2 = data.ReadByte();
        int b3 = data.ReadByte();
        int b4 = data.ReadByte();
        return (b1 & 0x00ff) | ((b2 & 0x00ff) << 8) | ((b3 & 0x00ff) << 16) | ((b4 & 0x00ff) << 24);
    }
}
