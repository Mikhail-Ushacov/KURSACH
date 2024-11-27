using System.Windows.Forms;

namespace KURSACH
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtVertexName;
        private Button btnAddVertex;
        private Button btnRemoveVertex;
        private ListBox lstVertices;
        private TextBox txtFromVertex;
        private TextBox txtToVertex;
        private TextBox txtEdgeCapacity;
        private Button btnAddEdge;
        private ListBox lstEdges;
        private Button btnRemoveEdge;
        private Button btnUpdateEdge;
        private TextBox txtSource;
        private TextBox txtSink;
        private Button btnFordFulkerson;
        private Button btnEdmondsKarp;
        private Label lblMaxFlow;
        private Panel panel1;
        private Button btnMathBench;
        private Button btnproalgoritm;
        private Button btnporivnyny;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtVertexName = new System.Windows.Forms.TextBox();
            this.btnAddVertex = new System.Windows.Forms.Button();
            this.btnRemoveVertex = new System.Windows.Forms.Button();
            this.lstVertices = new System.Windows.Forms.ListBox();
            this.txtFromVertex = new System.Windows.Forms.TextBox();
            this.txtToVertex = new System.Windows.Forms.TextBox();
            this.txtEdgeCapacity = new System.Windows.Forms.TextBox();
            this.btnAddEdge = new System.Windows.Forms.Button();
            this.lstEdges = new System.Windows.Forms.ListBox();
            this.btnRemoveEdge = new System.Windows.Forms.Button();
            this.btnUpdateEdge = new System.Windows.Forms.Button();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.txtSink = new System.Windows.Forms.TextBox();
            this.btnFordFulkerson = new System.Windows.Forms.Button();
            this.btnEdmondsKarp = new System.Windows.Forms.Button();
            this.lblMaxFlow = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMathBench = new System.Windows.Forms.Button();
            this.btnproalgoritm = new System.Windows.Forms.Button();
            this.btnporivnyny = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtVertexName
            // 
            this.txtVertexName.Location = new System.Drawing.Point(12, 12);
            this.txtVertexName.Name = "txtVertexName";
            this.txtVertexName.Size = new System.Drawing.Size(100, 20);
            this.txtVertexName.TabIndex = 0;
            // 
            // btnAddVertex
            // 
            this.btnAddVertex.Location = new System.Drawing.Point(118, 10);
            this.btnAddVertex.Name = "btnAddVertex";
            this.btnAddVertex.Size = new System.Drawing.Size(112, 23);
            this.btnAddVertex.TabIndex = 1;
            this.btnAddVertex.Text = "Додати вершину";
            this.btnAddVertex.UseVisualStyleBackColor = true;
            this.btnAddVertex.Click += new System.EventHandler(this.btnAddVertex_Click);
            // 
            // btnRemoveVertex
            // 
            this.btnRemoveVertex.Location = new System.Drawing.Point(118, 39);
            this.btnRemoveVertex.Name = "btnRemoveVertex";
            this.btnRemoveVertex.Size = new System.Drawing.Size(112, 23);
            this.btnRemoveVertex.TabIndex = 2;
            this.btnRemoveVertex.Text = "Видалити вершину";
            this.btnRemoveVertex.UseVisualStyleBackColor = true;
            this.btnRemoveVertex.Click += new System.EventHandler(this.btnRemoveVertex_Click);
            // 
            // lstVertices
            // 
            this.lstVertices.FormattingEnabled = true;
            this.lstVertices.Location = new System.Drawing.Point(12, 39);
            this.lstVertices.Name = "lstVertices";
            this.lstVertices.Size = new System.Drawing.Size(100, 95);
            this.lstVertices.TabIndex = 3;
            // 
            // txtFromVertex
            // 
            this.txtFromVertex.Location = new System.Drawing.Point(12, 140);
            this.txtFromVertex.Name = "txtFromVertex";
            this.txtFromVertex.Size = new System.Drawing.Size(100, 20);
            this.txtFromVertex.TabIndex = 4;
            // 
            // txtToVertex
            // 
            this.txtToVertex.Location = new System.Drawing.Point(12, 166);
            this.txtToVertex.Name = "txtToVertex";
            this.txtToVertex.Size = new System.Drawing.Size(100, 20);
            this.txtToVertex.TabIndex = 5;
            // 
            // txtEdgeCapacity
            // 
            this.txtEdgeCapacity.Location = new System.Drawing.Point(12, 192);
            this.txtEdgeCapacity.Name = "txtEdgeCapacity";
            this.txtEdgeCapacity.Size = new System.Drawing.Size(100, 20);
            this.txtEdgeCapacity.TabIndex = 6;
            // 
            // btnAddEdge
            // 
            this.btnAddEdge.Location = new System.Drawing.Point(118, 140);
            this.btnAddEdge.Name = "btnAddEdge";
            this.btnAddEdge.Size = new System.Drawing.Size(102, 23);
            this.btnAddEdge.TabIndex = 7;
            this.btnAddEdge.Text = "Додати ребро";
            this.btnAddEdge.UseVisualStyleBackColor = true;
            this.btnAddEdge.Click += new System.EventHandler(this.btnAddEdge_Click);
            // 
            // lstEdges
            // 
            this.lstEdges.FormattingEnabled = true;
            this.lstEdges.Location = new System.Drawing.Point(12, 218);
            this.lstEdges.Name = "lstEdges";
            this.lstEdges.Size = new System.Drawing.Size(181, 95);
            this.lstEdges.TabIndex = 8;
            // 
            // btnRemoveEdge
            // 
            this.btnRemoveEdge.Location = new System.Drawing.Point(118, 163);
            this.btnRemoveEdge.Name = "btnRemoveEdge";
            this.btnRemoveEdge.Size = new System.Drawing.Size(102, 23);
            this.btnRemoveEdge.TabIndex = 9;
            this.btnRemoveEdge.Text = "Видалити ребро";
            this.btnRemoveEdge.UseVisualStyleBackColor = true;
            this.btnRemoveEdge.Click += new System.EventHandler(this.btnRemoveEdge_Click);
            // 
            // btnUpdateEdge
            // 
            this.btnUpdateEdge.Location = new System.Drawing.Point(118, 189);
            this.btnUpdateEdge.Name = "btnUpdateEdge";
            this.btnUpdateEdge.Size = new System.Drawing.Size(102, 23);
            this.btnUpdateEdge.TabIndex = 10;
            this.btnUpdateEdge.Text = "Оновити ребро";
            this.btnUpdateEdge.UseVisualStyleBackColor = true;
            this.btnUpdateEdge.Click += new System.EventHandler(this.btnUpdateEdge_Click);
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(9, 340);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(100, 20);
            this.txtSource.TabIndex = 11;
            // 
            // txtSink
            // 
            this.txtSink.Location = new System.Drawing.Point(9, 366);
            this.txtSink.Name = "txtSink";
            this.txtSink.Size = new System.Drawing.Size(100, 20);
            this.txtSink.TabIndex = 12;
            // 
            // btnFordFulkerson
            // 
            this.btnFordFulkerson.Location = new System.Drawing.Point(115, 325);
            this.btnFordFulkerson.Name = "btnFordFulkerson";
            this.btnFordFulkerson.Size = new System.Drawing.Size(123, 35);
            this.btnFordFulkerson.TabIndex = 13;
            this.btnFordFulkerson.Text = "Алгоритм Форда-Фалкерсона";
            this.btnFordFulkerson.UseVisualStyleBackColor = true;
            this.btnFordFulkerson.Click += new System.EventHandler(this.btnFordFulkerson_Click);
            // 
            // btnEdmondsKarp
            // 
            this.btnEdmondsKarp.Location = new System.Drawing.Point(115, 366);
            this.btnEdmondsKarp.Name = "btnEdmondsKarp";
            this.btnEdmondsKarp.Size = new System.Drawing.Size(123, 35);
            this.btnEdmondsKarp.TabIndex = 14;
            this.btnEdmondsKarp.Text = "Алгоритм Eдмондса-Карпа";
            this.btnEdmondsKarp.UseVisualStyleBackColor = true;
            this.btnEdmondsKarp.Click += new System.EventHandler(this.btnEdmondsKarp_Click);
            // 
            // lblMaxFlow
            // 
            this.lblMaxFlow.AutoSize = true;
            this.lblMaxFlow.Location = new System.Drawing.Point(-1, 411);
            this.lblMaxFlow.Name = "lblMaxFlow";
            this.lblMaxFlow.Size = new System.Drawing.Size(122, 13);
            this.lblMaxFlow.TabIndex = 14;
            this.lblMaxFlow.Text = "Максимальний поток: ";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(244, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 600);
            this.panel1.TabIndex = 15;
            // 
            // btnMathBench
            // 
            this.btnMathBench.Location = new System.Drawing.Point(115, 434);
            this.btnMathBench.Name = "btnMathBench";
            this.btnMathBench.Size = new System.Drawing.Size(78, 34);
            this.btnMathBench.TabIndex = 0;
            this.btnMathBench.Text = "MathBench";
            this.btnMathBench.UseVisualStyleBackColor = true;
            this.btnMathBench.Click += new System.EventHandler(this.btnMathBench_Click);
            // 
            // btnproalgoritm
            // 
            this.btnproalgoritm.Location = new System.Drawing.Point(9, 434);
            this.btnproalgoritm.Name = "btnproalgoritm";
            this.btnproalgoritm.Size = new System.Drawing.Size(78, 34);
            this.btnproalgoritm.TabIndex = 0;
            this.btnproalgoritm.Text = "Про алгоритми";
            this.btnproalgoritm.UseVisualStyleBackColor = true;
            this.btnproalgoritm.Click += new System.EventHandler(this.proalgoritm);
            // 
            // btnporivnyny
            // 
            this.btnporivnyny.Location = new System.Drawing.Point(9, 474);
            this.btnporivnyny.Name = "btnporivnyny";
            this.btnporivnyny.Size = new System.Drawing.Size(78, 34);
            this.btnporivnyny.TabIndex = 1;
            this.btnporivnyny.Text = "Порівняння алгоритмів";
            this.btnporivnyny.UseVisualStyleBackColor = true;
            this.btnporivnyny.Click += new System.EventHandler(this.porivnyny);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1052, 617);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblMaxFlow);
            this.Controls.Add(this.btnFordFulkerson);
            this.Controls.Add(this.btnEdmondsKarp);
            this.Controls.Add(this.txtSink);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.btnUpdateEdge);
            this.Controls.Add(this.btnRemoveEdge);
            this.Controls.Add(this.lstEdges);
            this.Controls.Add(this.btnAddEdge);
            this.Controls.Add(this.txtEdgeCapacity);
            this.Controls.Add(this.txtToVertex);
            this.Controls.Add(this.txtFromVertex);
            this.Controls.Add(this.lstVertices);
            this.Controls.Add(this.btnRemoveVertex);
            this.Controls.Add(this.btnAddVertex);
            this.Controls.Add(this.txtVertexName);
            this.Controls.Add(this.btnMathBench);
            this.Controls.Add(this.btnproalgoritm);
            this.Controls.Add(this.btnporivnyny);
            this.Name = "Form1";
            this.Text = "Graph Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
