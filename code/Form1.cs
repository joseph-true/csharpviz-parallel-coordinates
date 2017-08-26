// -------------------------------------------------
// C-Sharp DataViz Parallel Coordinates
//
// License:
// Copyright (c) 2008-2017 Joseph True
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// -------------------------------------------------
// Author:            Joseph True
// Email:             jtrueprojects@gmail.com
// Original Date:     10/15/2008
// Updated:           Aug, 2017
// History:           Originally created as a Parallel Coordinates programming project for CS 525D Fall, 2008
//
// Overall Design:    Reads CSV file with car data.
//                    Gets min and max for each dimension.
//                    Plots eight numerical dimensions across multiple parallel Y axis.
//                    1. Retail Price
//                    2. Dealer Cost
//                    3. Engine Size (l)
//                    4. Cyl
//                    5. HP
//                    6. City MPG
//                    7. Hwy MPG
//                    8. Weight
//
//Additional Files:   Reads in mydata.csv data file with 8 columns of car data
//
// -------------------------------------------------
//
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace WinAppDataVizParallelCoords
{
	// .NET genertared code for form
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		//---------------------------------------
		// Application variables
		//---------------------------------------
		// String data values read from file
		ArrayList m_array1 = new ArrayList();
		ArrayList m_array2 = new ArrayList();
		ArrayList m_array3 = new ArrayList();
		ArrayList m_array4 = new ArrayList();
		ArrayList m_array5 = new ArrayList();
		ArrayList m_array6 = new ArrayList();
		ArrayList m_array7 = new ArrayList();
		ArrayList m_array8 = new ArrayList();

		// Axis points
		static int offsetX = 20;
		static int offsetY = 420;
		
		static int m_xAxisStart = offsetX;
		static int m_yAxisStart = offsetY;
		static int m_yAxisEnd =100;
		static int m_pAxisInterval = 110;
		static int m_yAxisHeight = m_yAxisStart - m_yAxisEnd;
		
		string[] m_DimNames = new string[8];

		//imported data
		float[,] m_XYdata;
		float[] m_yMins = new float[8];
		float[] m_yMaxs = new float[8];
		private System.Windows.Forms.Label lblMsg;
        private Label lblFileName;
        private LinkLabel lnkAbout;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblMsg = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lnkAbout = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.Location = new System.Drawing.Point(8, 30);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(680, 24);
            this.lblMsg.TabIndex = 0;
            this.lblMsg.Text = "description";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.Location = new System.Drawing.Point(8, 9);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(81, 16);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "(file name)";
            // 
            // lnkAbout
            // 
            this.lnkAbout.AutoSize = true;
            this.lnkAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkAbout.Location = new System.Drawing.Point(761, 8);
            this.lnkAbout.Name = "lnkAbout";
            this.lnkAbout.Size = new System.Drawing.Size(110, 15);
            this.lnkAbout.TabIndex = 2;
            this.lnkAbout.TabStop = true;
            this.lnkAbout.Text = "About this program";
            this.lnkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAbout_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(892, 466);
            this.Controls.Add(this.lnkAbout);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.lblMsg);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SharpViz - Parallel Coordinates";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// --- JTrue --------------------------------------------
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			importData();

			// Dimension names.
			m_DimNames[0]="Retail Price";
			m_DimNames[1]="Dealer Cost";
			m_DimNames[2]="Engine Size (l)";
			m_DimNames[3]="Cyl";
			m_DimNames[4]="HP";
			m_DimNames[5]="City MPG";
			m_DimNames[6]="Hwy MPG";
			m_DimNames[7]="Weight";

			string msgText;
			msgText = "This program imports 8 dimensions from the cars data set ";
			msgText = msgText + "and then plots the data using parallel coordinates.";
			lblMsg.Text =msgText;
		}

		// C#
		protected override void OnPaint(PaintEventArgs e) 
		{			
			// Redraw screen when form re-paints
			drawParallelCordinates();
			drawDataLines();
		}

		// Draw parallel coordinates
		private void drawParallelCordinates()
		{
			using (Graphics g = this.CreateGraphics())
			{
				Pen myPen = new Pen(Color.Black,1);
				// x axis - use next line if want to display x axis.
				// g.DrawLine(myPen, new Point(m_xAxisStart,m_yAxisStart), new Point(m_xAxisEnd,m_yAxisStart));
				
				Font fnt = new Font("Verdana", 10);

				// y axis start point
				int xLoc = m_xAxisStart;

				string strMin;
				string strMax;
				
				// Draw each y axis.
				for (int i=1;i<=8;i++)
				{	
					// Draw line and text label for axis.
					g.DrawLine(myPen, new Point(xLoc,m_yAxisStart), new Point(xLoc,m_yAxisEnd));
					g.DrawString(m_DimNames[i-1], fnt, new SolidBrush(Color.Black),xLoc,m_yAxisEnd-35);

					// Display min max values as labels on each y axis.
					Font myFnt = new Font("Verdana", 10);
					strMin = System.Convert.ToString(m_yMins[i-1]); 
					strMax = System.Convert.ToString(m_yMaxs[i-1]);
					g.DrawString(strMax, fnt, new SolidBrush(Color.Black),xLoc,m_yAxisEnd-20);
					g.DrawString(strMin, fnt, new SolidBrush(Color.Black),xLoc,m_yAxisStart+10);
					
					xLoc = xLoc + m_pAxisInterval;
				}
			}
		}

		// Read data from csv text file.
		private void importData()
		{
            string dataFile = "mydata.csv";

            lblFileName.Text = "File: " + dataFile;

            // Check for data file
            if (File.Exists(dataFile) == false)
            {
                MessageBox.Show("Missing data file " + dataFile + ".  Program will exit.", "File Error");
                Application.Exit();
            }

            // Read and parse the file
            try
            {
                FileStream aFile = new FileStream(dataFile, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(aFile);

                string strLine;
                strLine = sr.ReadLine();
                string[] strData;
                char[] chrDelimeter = new char[] { ',' };

			    // Read line from file and split into 8 dimensions
			    while (strLine !=null)
			    {
				    strData = strLine.Split(chrDelimeter,10);

				    m_array1.Add (strData[0]);
				    m_array2.Add (strData[1]);
				    m_array3.Add (strData[2]);
				    m_array4.Add (strData[3]);
				    m_array5.Add (strData[4]);
				    m_array6.Add (strData[5]);
				    m_array7.Add (strData[6]);
				    m_array8.Add (strData[7]);
				
				    strLine = sr.ReadLine();
			    }
			    sr.Close();			

			    m_XYdata = new float[8,m_array1.Count];

			    // Convert data from string
			    for(int i=0;i<m_array1.Count;i++)
			    {
				    m_XYdata[0,i]=System.Convert.ToSingle( m_array1[i]);
				    m_XYdata[1,i]=(System.Convert.ToSingle (m_array2[i]));
				    m_XYdata[2,i]=(System.Convert.ToSingle (m_array3[i]));
				    m_XYdata[3,i]=(System.Convert.ToSingle (m_array4[i]));
				    m_XYdata[4,i]=(System.Convert.ToSingle (m_array5[i]));
				    m_XYdata[5,i]=(System.Convert.ToSingle (m_array6[i]));
				    m_XYdata[6,i]=(System.Convert.ToSingle (m_array7[i]));
				    m_XYdata[7,i]=(System.Convert.ToSingle (m_array8[i]));
			    }

			    // Get min and max for each dimension.
			    for(int a=0;a<=7;a++)
			    {
				    m_yMins[a]= getMinVal(a);
				    m_yMaxs[a]= getMaxVal(a);
			    }
            }
            catch (Exception e)
            {
                MessageBox.Show( e.Message, "Error Reading Data File");
                Application.Exit();
            }

		}

		// ===================================
		// Cycle thru data and draw line across dimension for each data row
		private void drawDataLines()
		{
			if (m_XYdata != null)
			{
				//adjust to coordinate system
				int xLoc=0;

				int y1,y2;
				// Step thru all data rows
				for(int i=0; i<=m_XYdata.GetUpperBound(1);i++)
				//for(int i=10; i<=200;i++)
				{
					xLoc=20;
					// Step thru each column (dimension)
					using (Graphics g = this.CreateGraphics())
					{
						Pen myPen = new Pen(Color.Blue,1);
						GraphicsPath myPath = new GraphicsPath();
						for(int c=0;c<=6;c++)
						{
							// Draw line for one row of data.
							// NOTE: Could change these equations to allow for different or custom
							// min max values for each axis so the data plots better vertically.
							//y1 = System.Convert.ToInt16((m_yAxisHeight)*(m_XYdata[c,i]/(m_yMaxs[c]-m_yMins[c])));
							//y1 = System.Convert.ToInt16((m_yAxisHeight)*((m_XYdata[c,i]-m_yMins[c])/(m_yMaxs[c]-m_yMins[c])));
							y1 = System.Convert.ToInt16((m_yAxisHeight)*((m_XYdata[c,i])/(m_yMaxs[c]+.1*m_yMaxs[c])));

							//adjust to y offset
							y1 = m_yAxisStart  - y1;
							//y2 = System.Convert.ToInt16((m_yAxisHeight)*((m_XYdata[c+1,i]-m_yMins[c+1])/(m_yMaxs[c+1]-m_yMins[c+1])));
							y2 = System.Convert.ToInt16((m_yAxisHeight)*((m_XYdata[c+1,i])/(m_yMaxs[c+1]+.1*m_yMaxs[c+1])));

							//adjust to y offset
							y2 = m_yAxisStart - y2;

							myPath.AddLine(xLoc,y1,xLoc+m_pAxisInterval,y2); 
							//myPath.AddLine(20,400,600,400); 
							xLoc=xLoc+m_pAxisInterval;
						}
						g.DrawPath(myPen,myPath);
						myPath.Dispose();
					}
				}
			}  
		}
					
		// Get min value for a given dimension.
		private float getMinVal(int c)
		{
			float myVal=0;
			if (m_XYdata != null)
			{
				myVal = m_XYdata[c,0];
				for(int i=1; i<=m_XYdata.GetUpperBound(1);i++)
				{
					if (m_XYdata[c,i] < myVal)
					{
						myVal = m_XYdata[c,i];
					}
				}
			}
			return myVal;
		}

		// Get max value for a given dimension.
		private float getMaxVal(int c)
		{
			float myVal=0;
			if (m_XYdata != null)
			{
				myVal = m_XYdata[c,0];
				for(int i=1; i<=m_XYdata.GetUpperBound(1);i++)
				{
					if (m_XYdata[c,i] > myVal)
					{
						myVal = m_XYdata[c,i];
					}
				}			
			}
			return myVal;
		}

        private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string txtMessage = "Joseph True\njtrueprojects@gmail.com\nCopyright 2008-2017";
            string txtTitle = "About";
            MessageBox.Show(txtMessage, txtTitle);
        }

	}
}
