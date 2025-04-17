namespace AstarApp;

public partial class Form1 : Form
{
    private GamePanel gp;

    public Form1()
    {
        InitializeComponent();

        this.ClientSize         = new Size(960, 800);
        this.FormBorderStyle    = FormBorderStyle.FixedSingle;
        this.StartPosition      = FormStartPosition.CenterScreen;

        gp      = new GamePanel();
        gp.Dock = DockStyle.Fill;

        this.Controls.Add(gp);
    }
}