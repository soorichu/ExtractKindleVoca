
namespace ExtractKindleVoca
{
    public partial class Form1
    {
   
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Button btnOpen;
            System.Windows.Forms.Button btnExport;
            this.dgvWords = new System.Windows.Forms.DataGridView();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            btnOpen = new System.Windows.Forms.Button();
            btnExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWords)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.Location = new System.Drawing.Point(372, 12);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new System.Drawing.Size(328, 45);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "킨들 보카 가져오기";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnExport
            // 
            btnExport.Location = new System.Drawing.Point(12, 363);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(341, 45);
            btnExport.TabIndex = 1;
            btnExport.Text = "CSV로 내보내기";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // dgvWords
            // 
            this.dgvWords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWords.Location = new System.Drawing.Point(12, 75);
            this.dgvWords.Name = "dgvWords";
            this.dgvWords.RowTemplate.Height = 23;
            this.dgvWords.Size = new System.Drawing.Size(688, 272);
            this.dgvWords.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblStatus.Location = new System.Drawing.Point(24, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(249, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "킨들 단말기를 연결 후 버튼을 클릭해주세요..";
     //       this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(372, 364);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(327, 44);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "킨들에서 보카 삭제하기";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 429);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.dgvWords);
            this.Controls.Add(btnExport);
            this.Controls.Add(btnOpen);
            this.Name = "Form1";
            this.Text = "킨들 보카 추출하기";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvWords;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDelete;
    }


}

