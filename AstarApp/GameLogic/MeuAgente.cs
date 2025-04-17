public class MeuAgente : Agente
{
    private Color color;
    private double vel = 40;
    private double ang = 0;


    private double oldx = 0;
    private double oldy = 0;

    private int timeria = 0;

    private bool colidiu = false;

    public MeuAgente(int x, int y, Color color)
    {
        X = x;
        Y = y;
        this.color = color;
    }

    public override void SimulaSe(int DiffTime)
    {
        timeria += DiffTime;

        oldx = X;
        oldy = Y;

        if (timeria > 100)
        {
            CalculaIA(DiffTime);
            timeria = 0;
        }

        X += Math.Cos(ang) * vel * DiffTime / 1000.0;
        Y += Math.Sin(ang) * vel * DiffTime / 1000.0;

        for (int i = 0; i < GamePanel.listadeagentes.Count; i++)
        {
            Agente agente = GamePanel.listadeagentes[i];
            if (agente != this)
            {
                double dax = agente.X - X;
                double day = agente.Y - Y;
                double dista = dax * dax + day * day;

                if (dista < 400)
                {
                    X = oldx;
                    Y = oldy;

                    colidiu = true;

                    break;
                }
            }
        }
    }

    public override void DesenhaSe(Graphics dbg, int XMundo, int YMundo)
    {
        dbg.FillEllipse(new SolidBrush(color), (int)(X - 10) - XMundo, (int)(Y - 10) - YMundo, 20, 20);

        double linefx = X + 10 * Math.Cos(ang);
        double linefy = Y + 10 * Math.Sin(ang);
        dbg.DrawLine(Pens.Black, (int)X - XMundo, (int)Y - YMundo, (int)linefx - XMundo, (int)linefy - YMundo);
    }

    public void CalculaIA(int DiffTime)
    {
        vel = 0;
    }
}