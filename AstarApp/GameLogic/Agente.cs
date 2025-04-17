using System.Drawing;

public abstract class Agente
{
    public double X, Y;

    public abstract void SimulaSe(int DiffTime);
    public abstract void DesenhaSe(Graphics dbg, int XMundo, int YMundo);
}
