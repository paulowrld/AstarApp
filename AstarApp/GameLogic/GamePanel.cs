using AstarApp.GameLogic;

public class GamePanel : Panel, Runnable
{
    public static PathFindingAlgorithm CurrentAlgorithm = PathFindingAlgorithm.AStar;

    System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();


    private const int PWIDTH    = 960;
    private const int PHEIGHT   = 800;
    private bool running        = false;
    private bool gameOver       = false;

    int FPS, SFPS;
    int fpscount;

    public static Random rnd = new Random();

    bool LEFT, RIGHT, UP, DOWN;

    public static int mousex, mousey;

    public static List<Agente> listadeagentes = new List<Agente>();

    MapaGrid mapa;

    double posx, posy;

    MeuAgente meuHeroi = null;

    int[] caminho = null;

    float zoom = 1;

    int ntileW = 60;
    int ntileH = 50;

    Font f = new Font("Arial", 20f, FontStyle.Bold);

    public GamePanel()
    {
        this.BackColor = Color.White;
        this.Size = new Size(PWIDTH, PHEIGHT);
        this.DoubleBuffered = true;
        this.Focus();

        // Configuração dos eventos de teclado e mouse
        this.KeyDown    += GamePanel_KeyDown;
        this.KeyUp      += GamePanel_KeyUp;
        this.MouseMove  += GamePanel_MouseMove;
        this.MouseDown  += GamePanel_MouseDown;
        this.MouseWheel += GamePanel_MouseWheel;

        meuHeroi = new MeuAgente(10, 10, Color.Blue);
        listadeagentes.Add(meuHeroi);

        mousex = mousey = 0;

        mapa = new MapaGrid(100, 100, ntileW, ntileH);
        mapa.Loadmapfromimage("labirinto.png");

        gameTimer = new System.Windows.Forms.Timer();
        gameTimer.Interval = 16;
        gameTimer.Tick += GameTimer_Tick;
        gameTimer.Start();
    }

    LinkedList<Nodo> pilhaprofundidade = new();
    HashSet<int> nodosPercorridos = [];
    public bool JaPassei(int nX, int nY)
    {
        return nodosPercorridos.Contains(nX + nY * 1000);
    }


    public bool RodaBuscaProfundidade(int iniX, int iniY, int objX, int objY)
    {
        Nodo nodoAtivo = new Nodo(iniX, iniY);
        pilhaprofundidade.AddLast(nodoAtivo);

        while (pilhaprofundidade.Count > 0)
        {
            if (nodoAtivo.x == objX && nodoAtivo.y == objY)
            {
                caminho = new int[pilhaprofundidade.Count * 2];
                int index = 0;
                foreach (Nodo n in pilhaprofundidade)
                {
                    caminho[index] = n.x;
                    caminho[index + 1] = n.y;
                    index += 2;
                }
                return true;
            }

            lock (nodosPercorridos)
            {
                nodosPercorridos.Add(nodoAtivo.x + nodoAtivo.y * 1000);
            }

            Nodo[] t = new Nodo[4];
            t[0] = new Nodo(nodoAtivo.x, nodoAtivo.y + 1);
            t[1] = new Nodo(nodoAtivo.x + 1, nodoAtivo.y);
            t[2] = new Nodo(nodoAtivo.x, nodoAtivo.y - 1);
            t[3] = new Nodo(nodoAtivo.x - 1, nodoAtivo.y);

            bool ok = false;
            for (int i = 0; i < 4; i++)
            {
                if (t[i].y < 0 || t[i].y >= 1000 || t[i].x < 0 || t[i].x >= 1000)
                    continue;
                if (mapa.mapa[t[i].y, t[i].x] == 0 && !JaPassei(t[i].x, t[i].y))
                {
                    pilhaprofundidade.AddLast(t[i]);
                    nodoAtivo = t[i];
                    ok = true;
                    break;
                }
            }

            if (ok)
                continue;

            pilhaprofundidade.RemoveLast();
            if (pilhaprofundidade.Count > 0)
                nodoAtivo = pilhaprofundidade.Last.Value;
        }

        return false;
    }

    public void StopGame()
    {
        running = false;
    }

    public void run()
    {
        running = true;

        long DifTime, TempoAnterior;

        int segundo = 0;
        DifTime = 0;
        TempoAnterior = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        while (running)
        {
            GameUpdate((int)DifTime); // game state is updated

            Graphics g = this.CreateGraphics();
            GameRender((Graphics)g); // render to a buffer
            g.Dispose();

            try
            {
                Thread.Sleep(0);
            }
            catch (ThreadInterruptedException) { }

            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            DifTime = now - TempoAnterior;
            TempoAnterior = now;

            if (segundo != (int)(TempoAnterior / 1000))
            {
                FPS = SFPS;
                SFPS = 1;
                segundo = (int)(TempoAnterior / 1000);
            }
            else
            {
                SFPS++;
            }
        }
        System.Environment.Exit(0); // so enclosing Form exits
    }

    int timerfps = 0;
    private void GameUpdate(int DiffTime)
    {
        if (LEFT)
            posx -= 1000 * DiffTime / 1000.0;
        if (RIGHT)
            posx += 1000 * DiffTime / 1000.0;
        if (UP)
            posy -= 1000 * DiffTime / 1000.0;
        if (DOWN)
            posy += 1000 * DiffTime / 1000.0;

        if (posx > mapa.Largura * 16)
            posx = mapa.Largura * 16;
        if (posy > mapa.Altura * 16)
            posy = mapa.Altura * 16;
        if (posx < 0)
            posx = 0;
        if (posy < 0)
            posy = 0;

        mapa.Posiciona((int)posx, (int)posy);

        for (int i = 0; i < listadeagentes.Count; i++)
        {
            listadeagentes[i].SimulaSe(DiffTime);
        }
    }

    private void GameRender(Graphics dbg)
    {
        // clear the background
        dbg.Clear(Color.White);

        // Save current transform and apply scale (zoom)
        System.Drawing.Drawing2D.Matrix trans = dbg.Transform;
        dbg.ScaleTransform(zoom, zoom);

        try
        {
            mapa.DesenhaSe(dbg);
        }
        catch (Exception e)
        {
            Console.WriteLine("Erro ao desenhar mapa");
        }

        for (int i = 0; i < listadeagentes.Count; i++)
        {
            listadeagentes[i].DesenhaSe(dbg, mapa.MapX, mapa.MapY);
        }

        lock (nodosPercorridos)
        {
            foreach (int nxy in nodosPercorridos)
            {
                int px = nxy % 1000;
                int py = nxy / 1000;
                dbg.FillRectangle(Brushes.Green, px * 16 - mapa.MapX, py * 16 - mapa.MapY, 16, 16);
            }
        }

        if (caminho != null)
        {
            try
            {
                for (int i = 0; i < caminho.Length / 2; i++)
                {
                    int nx = caminho[i * 2];
                    int ny = caminho[i * 2 + 1];

                    dbg.FillRectangle(Brushes.Blue, nx * 16 - mapa.MapX, ny * 16 - mapa.MapY, 16, 16);
                }
            }
            catch (Exception e)
            {
            }
        }

        // Restore original transform
        dbg.ResetTransform();

        dbg.DrawString("FPS: " + FPS, f, Brushes.Blue, 10, 30);
        dbg.DrawString("N: " + nodosPercorridos.Count, f, Brushes.Blue, 100, 30);
    }

    private void GamePanel_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)
            LEFT = true;
        if (e.KeyCode == Keys.Right)
            RIGHT = true;
        if (e.KeyCode == Keys.Up)
            UP = true;
        if (e.KeyCode == Keys.Down)
            DOWN = true;
    }

    private void GamePanel_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)
            LEFT = false;
        if (e.KeyCode == Keys.Right)
            RIGHT = false;
        if (e.KeyCode == Keys.Up)
            UP = false;
        if (e.KeyCode == Keys.Down)
            DOWN = false;
    }

    private void GamePanel_MouseMove(object sender, MouseEventArgs e)
    {
        mousex = e.X;
        mousey = e.Y;
    }

    private void GamePanel_MouseDown(object sender, MouseEventArgs e)
    {
        int mousex = (int)((e.X + mapa.MapX) / zoom);
        int mousey = (int)((e.Y + mapa.MapY) / zoom);

        int mx = mousex / 16;
        int my = mousey / 16;

        if (mx > mapa.Altura || my > mapa.Largura)
            return;

        if (e.Button == MouseButtons.Right)
        {
            if (mapa.mapa[my, mx] == 0)
                mapa.mapa[my, mx] = 1;
            else
                mapa.mapa[my, mx] = 0;
        }
        if (e.Button == MouseButtons.Left)
        {
            if (mapa.mapa[my, mx] == 0)
            {
                caminho = null;
                long timeini = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                Console.WriteLine("" + my + " " + mx);
                Console.WriteLine("meueroi " + ((int)(meuHeroi.X / 16)) + " " + ((int)(meuHeroi.Y / 16)));

                bool found = CalcularCaminho((int)(meuHeroi.X / 16), (int)(meuHeroi.Y / 16), mx, my);

                long timefin = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - timeini;
                Console.WriteLine("Tempo Final: " + timefin);
            }
            else
            {
                Console.WriteLine("Caminho Final Bloqueado");
            }
        }
    }
    private void GameTimer_Tick(object sender, EventArgs e)
    {
        GameUpdate(16);
        this.Invalidate();
    }

    private void GamePanel_MouseWheel(object sender, MouseEventArgs e)
    {
        if (e.Delta < 0)
            zoom = zoom * 1.1f;
        else if (e.Delta > 0)
            zoom = zoom * 0.90f;

        ntileW = (int)((960 / zoom) / 16) + 1;
        ntileH = (int)((800 / zoom) / 16) + 1;

        if (ntileW >= 1000)
            ntileW = 1000;
        if (ntileH >= 1000)
            ntileH = 1000;
        mapa.NumeroTilesX = ntileW;
        mapa.NumeroTilesY = ntileH;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        GameRender(e.Graphics);
    }
    public bool CalcularCaminho(int inicioX, int inicioY, int destinoX, int destinoY)
    {
        if (CurrentAlgorithm == PathFindingAlgorithm.DFS)
        {
            return RodaBuscaProfundidade(inicioX, inicioY, destinoX, destinoY);
        }
        else if (CurrentAlgorithm == PathFindingAlgorithm.AStar)
        {
            nodosPercorridos.Clear();
            List<(int x, int y)> caminhoAStar = AStarPathFinder.Search(inicioX, inicioY, destinoX, destinoY, mapa, nodosPercorridos);
            if (caminhoAStar != null)
            {
                caminho = new int[caminhoAStar.Count * 2];
                int index = 0;
                foreach (var coord in caminhoAStar)
                {
                    caminho[index] = coord.x;
                    caminho[index + 1] = coord.y;
                    index += 2;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}