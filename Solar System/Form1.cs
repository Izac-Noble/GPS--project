using System;
using System.Collections.Generic;
using System.Collections; // need to use ArrayList
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Solar_System
{
    public partial class Form1 : Form
    {
        private class Planet
        {
            string name;
            float Angle;
            Bitmap image;
            bool staticcenter; // true if orbit center never changes
            float aboutX; // static center X
            float aboutY; // static center Y
            Planet center; // centered about moving planet
            float majoraxis;
            float minoraxis;
            double ec; // eccentricity
            double p; // ellipse parameter: numerator of polar equation
            double rotation;
            double speed;

            public Planet(string Name, float OrbitRadius, float Location, float AngularVelocity, Bitmap Image, float OrbitCenterX, float OrbitCenterY)
            {
                name = Name;
                Angle = Location;
                speed = AngularVelocity;
                image = Image;
                staticcenter = true;
                aboutX = OrbitCenterX;
                aboutY = OrbitCenterY;
                majoraxis = OrbitRadius;
                minoraxis = OrbitRadius;
                p = minoraxis * minoraxis / majoraxis; // constant parameter: numerator of polar equation
                ec = Math.Sqrt(majoraxis * majoraxis - minoraxis * minoraxis) / majoraxis; // constant
                rotation = 0;
            }
            public Planet(string Name, float OrbitRadius, float Location, float AngularVelocity, Bitmap Image, Planet Center)
            {
                name = Name;
                Angle = Location;
                speed = AngularVelocity;
                image = Image;
                staticcenter = false;
                center = Center;
                majoraxis = OrbitRadius;
                minoraxis = OrbitRadius;
                p = minoraxis * minoraxis / majoraxis; // constant parameter: numerator of polar equation
                ec = Math.Sqrt(majoraxis * majoraxis - minoraxis * minoraxis) / majoraxis; // constant
                rotation = 0;
            }
            public Planet(string Name, float PlanetRadius, float OrbitRadius, Color PlanetColor, Random R, float AngularVelocity, ArrayList Planets, string Center)
            {
                name = Name;
                image = Sphere(PlanetColor, PlanetRadius, 0.50);
                Angle = (float)((Math.PI / 1000.0) * (R.Next() % 2000));
                speed = AngularVelocity;
                image = Image;
                staticcenter = false;
                center = (Planet)Planets[0];
                foreach (Planet planet in Planets)
                {
                    if (Center == planet.Name)
                    {
                        center = planet;
                        break;
                    }
                }
                majoraxis = OrbitRadius;
                minoraxis = OrbitRadius;
                p = minoraxis * minoraxis / majoraxis; // constant parameter: numerator of polar equation
                ec = Math.Sqrt(majoraxis * majoraxis - minoraxis * minoraxis) / majoraxis; // constant
                rotation = 0;
            }
            public Planet(string Name, float PlanetRadius, float OrbitRadius, Color PlanetColor, Random R, float AngularVelocity, ArrayList Planets)
            {
                name = Name;
                image = Sphere(PlanetColor, PlanetRadius, 0.50);
                Angle = (float)((Math.PI / 1000.0) * (R.Next() % 2000));
                speed = AngularVelocity;
                image = Image;
                staticcenter = false;
                center = (Planet)Planets[0];
                majoraxis = OrbitRadius;
                minoraxis = OrbitRadius;
                p = minoraxis * minoraxis / majoraxis; // constant parameter: numerator of polar equation
                ec = Math.Sqrt(majoraxis * majoraxis - minoraxis * minoraxis) / majoraxis; // constant
                rotation = 0;
            }
            public Planet(float PlanetRadius, float MajorAxis, float MinorAxis, double RotationDegrees, Color PlanetColor, ArrayList Planets)
            {
                image = Sphere(PlanetColor, PlanetRadius, 0.50);
                majoraxis = MajorAxis;
                minoraxis = MinorAxis;
                staticcenter = false;
                center = (Planet)Planets[0]; // Assumes Sun is first planet
                p = minoraxis * minoraxis / majoraxis; // constant parameter: numerator of polar equation
                ec = Math.Sqrt(majoraxis * majoraxis - minoraxis * minoraxis) / majoraxis; // constant
                rotation = RotationDegrees * Math.PI/180.0;
                speed = 0.001;
                Angle = 0;
            }
            public string Name // Read Only
            {
                get { return name; }
            }
            public double Radius(double angle)
            {
                // Polar equation source: http://math.etsu.edu/MultiCalc/Chap3/Chap3-2/part4.htm
                return p / (1 - ec * Math.Cos(angle + rotation));
            }
            public float X // Read Only
            {
                get
                {
                    if (StaticCenter)
                        return aboutX;
                    else
                    {
                        return AboutX + Convert.ToInt32(Radius(Angle) * Math.Cos(Angle));
                    }
                }
            }
            public float Y // Read Only
            {
                get
                {
                    if (StaticCenter)
                        return aboutY;
                    else
                    {
                        return AboutY + Convert.ToInt32(Radius(Angle) * Math.Sin(Angle));
                    }
                }
            }
            public bool StaticCenter
            {
                get { return staticcenter; }
            }
            public float MajorAxis
            {
                get { return majoraxis; }
            }
            public float MinorAxis
            {
                get { return minoraxis; }
            }
            public float Location
            {
                get { return Angle; }
                set { Angle = value; }
            }
            public double Rotation // Read Only
            {
                get { return rotation; }
            }
            public Bitmap Image
            {
                get { return image; }
                set { image = value; }
            }
            public float AboutX // Read Only
            {
                get
                {
                    if (StaticCenter)
                        return aboutX;
                    else
                        return center.X;
                }
            }
            public float AboutY // Read Only
            {
                get
                {
                    if (StaticCenter)
                        return aboutY;
                    else
                        return center.Y;
                }
            }
            public void MovePlanet()
            {
                double r = p / (1 - ec * Math.Cos(Angle + rotation)); // polar equation source: http://math.etsu.edu/MultiCalc/Chap3/Chap3-2/part4.htm
                double EllipseArea = Math.PI * majoraxis * minoraxis;
                double DeltaTheta = 2 * (EllipseArea * speed) / (r * r);
                Angle += (float)DeltaTheta; // Move Planetoid
            }
            private Bitmap Sphere(Color PlanetColor, double PlanetRadius, double FadeFactor)
            {
                int Side = Convert.ToInt32(Math.Ceiling(2 * PlanetRadius));
                if (Side % 2 == 0) ++Side; // ensures Side is odd, so there is a center pixel
                int xc = Side / 2; int yc = xc; // xc,yc are coordinates for center pixel
                Color TransparentColor = Color.Black; // White might be used as planet color
                Bitmap B = new Bitmap(Side, Side);
                for (int x = 0; x < B.Width; x++)
                    for (int y = 0; y < B.Height; y++)
                    {
                        B.SetPixel(x, y, TransparentColor); // make each pixel transparent
                        double d = Math.Sqrt((x - xc) * (x - xc) + (y - yc) * (y - yc));
                        if (d < PlanetRadius) // overwrite each pixel with visible, but fading color
                        {
                            double Ratio = (PlanetRadius - d) / PlanetRadius;
                            double Intensity = Math.Pow(Ratio, FadeFactor);
                            int Red = Convert.ToInt32(PlanetColor.R * Intensity); // Note: Convert rounds to nearest integer
                            int Green = Convert.ToInt32(PlanetColor.G * Intensity);
                            int Blue = Convert.ToInt32(PlanetColor.B * Intensity);
                            B.SetPixel(x, y, Color.FromArgb(Red, Green, Blue));
                        }
                    }
                B.MakeTransparent(TransparentColor);
                return B;
            }
        } // end class Planet

        Bitmap SolarSystem;
        ArrayList Planets;
        bool OutlineOrbit;
        Random R = new Random();
        Planet Earth;
        Planet Saturn;
        Planet Mercury;
        Planet Moon;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Solar System Simulation";
            SolarSystem = new Bitmap(pictureBox1.Width, pictureBox1.Height); // Create global bitmap
            button1.Text = "Save";
            checkBox1.Text = "Outline\nOrbits";
            OutlineOrbit = false;
            Planets = new ArrayList();
            timer1.Enabled = true;
            timer1.Interval = 25;

            // Create Sun
            Bitmap SunBmp = Sphere(Color.Blue, 40.0, 0.25);
            Planet Sun = new Planet("Sol", 0.0F, 0.0F, 0.0F, SunBmp, pictureBox1.Width / 2, pictureBox1.Height / 2);
            Planets.Add(Sun);
            // Create Earth
            Bitmap EarthBitmap = new Bitmap("SAT1.bmp");
            EarthBitmap.MakeTransparent(EarthBitmap.GetPixel(0, 0)); // or Earth.MakeTransparent(WhiteBrush.Color);
            Earth = new Planet("Earth", 100, 0.0F, 0.0008F, EarthBitmap, Sun);
            Planets.Add(Earth);

            // DEBUG
            Bitmap SaturnBitmap = new Bitmap("GyoSat.bmp");
            SaturnBitmap.MakeTransparent(SaturnBitmap.GetPixel(0, 0)); // or Earth.MakeTransparent(WhiteBrush.Color);
            Saturn = new Planet("Saturn", 200, 0.0F, 0.0004F, SaturnBitmap, Sun);
            Planets.Add(Saturn);

            //Create Earth's Moon
            Bitmap MoonBitmap = new Bitmap("Moon.bmp");
            MoonBitmap.MakeTransparent(MoonBitmap.GetPixel(0, 0));
            Moon = new Planet("Moon", 390, 0.0F, 0.0006F, MoonBitmap, Sun);
            Planets.Add(Moon);
            // Create Mercury
            Bitmap MercuryBitmap = new Bitmap("SAT3.bmp");
            MercuryBitmap.MakeTransparent(MercuryBitmap.GetPixel(0, 0));
            Planets.Add(new Planet("Mercury", 49, (float)Math.PI, 0.002F, MercuryBitmap, Sun));
            //// Create Mars
            //Bitmap MarsBitmap = Sphere(Color.Red, 4.5, 0.33);
            //Planet Mars = new Planet("Mars", 145, (float)(Math.PI / 2), 0.00025F, MarsBitmap, Sun);
            //Planets.Add(Mars);
            // Create Phobos (a moon of Mars)
            //Planets.Add(new Planet("Phobos", 1.5F, 12, Color.White, R, 0.0065F, Planets, "Mars"));
            // Create Deimos (a moon of Mars)
            Bitmap DeimosBitmap = new Bitmap(1, 1);
            //DeimosBitmap.SetPixel(0, 0, Color.White);
            //Planets.Add(new Planet("Deimos", 20, 0.0F, 0.004F, DeimosBitmap, Mars));
            // Create Venus
            //Planets.Add(new Planet("Venus", 4.5F, 70, Color.Magenta, R, 0.00075F, Planets));
            // Create Haley's Comet
            Planets.Add(new Planet(2.5F, 200, 125, 45, Color.Beige,Planets));
            // Create arbitrary comet
            Planets.Add(new Planet(2.0F, 210, 100, 140, Color.Violet, Planets));

            // Draw solar system & display
            DrawSolarSystem();
            pictureBox1.Image = SolarSystem;
        }



        public Bitmap Sphere(Color PlanetColor, double PlanetRadius, double FadeFactor)
        {
            int Side = Convert.ToInt32(Math.Ceiling(2 * PlanetRadius));
            if (Side % 2 == 0) ++Side; // ensures Side is odd, so there is a center pixel
            int xc = Side / 2; int yc = xc; // xc,yc are coordinates for center pixel
            Color TransparentColor = Color.Black; // White might be used as planet color
            Bitmap B = new Bitmap(Side, Side);
            for (int x = 0; x < B.Width; x++)
                for (int y = 0; y < B.Height; y++)
                {
                    B.SetPixel(x, y, TransparentColor); // make each pixel transparent
                    double d = Math.Sqrt((x - xc) * (x - xc) + (y - yc) * (y - yc));
                    if (d < PlanetRadius) // overwrite each pixel with visible, but fading color
                    {
                        double Ratio = (PlanetRadius - d) / PlanetRadius;
                        double Intensity = Math.Pow(Ratio, FadeFactor);
                        int Red = Convert.ToInt32(PlanetColor.R * Intensity); // Note: Convert rounds to nearest integer
                        int Green = Convert.ToInt32(PlanetColor.G * Intensity);
                        int Blue = Convert.ToInt32(PlanetColor.B * Intensity);
                        B.SetPixel(x, y, Color.FromArgb(Red, Green, Blue));
                    }
                }
            B.MakeTransparent(TransparentColor);
            return B;
        }



        private void DrawSolarSystem()
        {
            Graphics g = Graphics.FromImage(SolarSystem); // Create graphics object to draw on bitmap
            // Draw black background for space
            SolidBrush SpaceBrush = new SolidBrush(Color.Black);
            g.FillRectangle(SpaceBrush, 0, 0, SolarSystem.Width, SolarSystem.Height);
            // Draw each object in Planets list
            Pen OrbitPen = new Pen(Color.FromArgb(40, 40, 40));
            // Draw all orbits
            foreach (Planet P in Planets)
            {
                // Draw orbits
                if ((OutlineOrbit)&&(!P.StaticCenter)) // Don't draw orbit for Sun
                {
                    double theta = 0;
                    double r = P.Radius(theta);
                    int x1 = Convert.ToInt32(P.AboutX) + Convert.ToInt32(r * Math.Cos(theta));
                    int y1 = Convert.ToInt32(P.AboutY) + Convert.ToInt32(r * Math.Sin(theta));
                    while (theta < 2 * Math.PI)
                    {
                        theta = theta + Math.PI / 60; // + about 0.05 radians
                        r = P.Radius(theta);
                        int x2 = Convert.ToInt32(P.AboutX) + Convert.ToInt32(r * Math.Cos(theta));
                        int y2 = Convert.ToInt32(P.AboutY) + Convert.ToInt32(r * Math.Sin(theta));
                        g.DrawLine(OrbitPen, x1, y1, x2, y2);
                        x1 = x2;
                        y1 = y2;
                    }
                }
            }
            // Draw planetoids (after all orbits are drawn)
            foreach (Planet P in Planets)
            {
                if ((P.Image.Width == 1) && (P.Image.Height == 1))
                {
                    SolidBrush B = new SolidBrush(P.Image.GetPixel(0, 0));
                    g.FillRectangle(B, P.X, P.Y, 1, 1);
                    // Note: g has no SetPixel method
                }
                else
                    g.DrawImage(P.Image, P.X - P.Image.Width / 2, P.Y - P.Image.Height / 2);
            }
            // If broadcasting, draw line to station
            float BitmapCenterX = SolarSystem.Width / 2;
            float BitmapCenterY = SolarSystem.Height / 2;
            float SunStationX = BitmapCenterX + 20;
            float SunStationY = BitmapCenterY;
            // Is earth in range?
            float Tolerance = 82;
            float d = (float)Math.Sqrt((Earth.X - SunStationX) * (Earth.X - SunStationX) + (Earth.Y - SunStationY) * (Earth.Y - SunStationY));
            //float t = (float)Math.Sqrt((Mercury.X - SunStationX) * (Mercury.X - SunStationX) + (Mercury.Y - SunStationY) * (Mercury.Y - SunStationY));

            if (d<Tolerance)
            {
                g.DrawLine(new Pen(Color.White), SunStationX, SunStationY, Earth.X, Earth.Y);
               
            }
            //if (t < Tolerance)
            //{
            //    g.DrawLine(new Pen(Color.White), SunStationX, SunStationY, Mercury.X, Mercury.Y);
            //}
            g.DrawLine(new Pen(Color.White), BitmapCenterX, BitmapCenterY, Saturn.X, Saturn.Y);

        } // end DrawSolarSystem



        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the location of all planets in Planets list
            foreach (Planet P in Planets)
                P.MovePlanet();
            // Draw solar system & display
            DrawSolarSystem();
            pictureBox1.Image = SolarSystem;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            SolarSystem.Save("Solar System.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                OutlineOrbit = true;
            else
                OutlineOrbit = false;
        }

    }
}