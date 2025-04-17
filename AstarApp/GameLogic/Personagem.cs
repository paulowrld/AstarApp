public class Personagem : Agente
{
    private Bitmap AnimeSet;

    private int frame;
    private int timeranimacao;
    private int animacao;
    private int tempoentreframes;

    private int velocidade = 0;
    private int velx, vely;

    private int sizeX = 24;
    private int sizeY = 32;

    // Construtor
    public Personagem(Bitmap _AnimeSet)
    {
        AnimeSet        = _AnimeSet;
        frame           = 0;
        animacao        = 0;
        timeranimacao   = 0;

        velx = 0;
        vely = 0;

        tempoentreframes = 200;
    }

    public override void DesenhaSe(Graphics dbg, int XMundo, int YMundo)
    {
        Rectangle destRect  = new Rectangle((int)X - XMundo, (int)Y - YMundo, sizeX, sizeY);
        Rectangle srcRect   = new Rectangle(sizeX * frame, sizeY * animacao, sizeX, sizeY);
        dbg.DrawImage(AnimeSet, destRect, srcRect, GraphicsUnit.Pixel);
    }

    public override void SimulaSe(int DiffTime) { }
}