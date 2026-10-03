using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace ExtractKindleVoca
{
    public partial class Form1 : Form
    {
        private DataTable vocaTable = new DataTable();
        private String currentDbPath;

        public Form1()
        {
            InitializeComponent();
            SetupDataTable();
        }

        private void SetupDataTable()
        {
            vocaTable.Columns.Add("단어");
            vocaTable.Columns.Add("원형(Stem)");
            vocaTable.Columns.Add("문맥 문장(Usage)");
            vocaTable.Columns.Add("책 제목");
            vocaTable.Columns.Add("저자");
            dgvWords.DataSource = vocaTable;
            dgvWords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // 1. vocab.db 파일 불러오기
        private void btnOpen_Click(object sender, EventArgs e)
        {
            string detectedPath = AutoDetectKindleVocabPath();

            if (!string.IsNullOrEmpty(detectedPath))
            {
                currentDbPath = detectedPath;
                lblStatus.Text = $"킨들 감지 성공: {detectedPath}";
                LoadKindleVocab(currentDbPath);
                MessageBox.Show($"킨들을 찾았습니다!\n경로: {detectedPath}", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var result = MessageBox.Show(
                    "킨들 단어장(vocab.db)을 자동으로 찾지 못했습니다.\n수동으로 파일을 선택하시겠습니까?",
                    "감지 실패",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    btnOpen_Click(sender, e); // 기존 파일 다이얼로그 호출
                }
            }
        }

        private void LoadKindleVocab(string dbPath)
        {
            vocaTable.Rows.Clear();

            string connectionString = $"Data Source={dbPath};Mode=ReadOnly;";

            // WORDS, LOOKUPS, BOOK_INFO 테이블 조인 쿼리
            string query = @"
                SELECT 
                    w.word AS Word,
                    w.stem AS Stem,
                    l.usage AS Usage,
                    b.title AS Title,
                    b.authors AS Authors
                FROM LOOKUPS l
                JOIN WORDS w ON l.word_key = w.id
                LEFT JOIN BOOK_INFO b ON l.book_key = b.id
                ORDER BY l.timestamp DESC;";

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vocaTable.Rows.Add(
                                reader["Word"]?.ToString(),
                                reader["Stem"]?.ToString(),
                                reader["Usage"]?.ToString(),
                                reader["Title"]?.ToString(),
                                reader["Authors"]?.ToString()
                            );
                        }
                    }
                }

                lblStatus.Text = $"총 {vocaTable.Rows.Count}개의 단어를 불러왔습니다.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DB 로드 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. CSV로 내보내기 (Anki / 엑셀 호환)
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (vocaTable.Rows.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV 파일 (*.csv)|*.csv";
                sfd.FileName = $"Kindle_Voca_{DateTime.Now:yyyyMMdd}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportToCsv(sfd.FileName);
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            try
            {
                var sb = new StringBuilder();

                // 헤더
                sb.AppendLine("단어,원형,문맥,책제목,저자");

                // 데이터 작성 (쉼표/줄바꿈 포함 대비 쌍따옴표 처리)
                foreach (DataRow row in vocaTable.Rows)
                {
                    string word = EscapeCsv(row["단어"].ToString());
                    string stem = EscapeCsv(row["원형(Stem)"].ToString());
                    string usage = EscapeCsv(row["문맥 문장(Usage)"].ToString());
                    string title = EscapeCsv(row["책 제목"].ToString());
                    string authors = EscapeCsv(row["저자"].ToString());

                    sb.AppendLine($"{word},{stem},{usage},{title},{authors}");
                }

                // UTF-8 BOM으로 저장해야 엑셀에서 한글/특수문자가 깨지지 않음
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

                MessageBox.Show("성공적으로 CSV 파일을 내보냈습니다!", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // [추가] DB 삭제/초기화 여부 확인
                AskAndDeleteDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"내보내기 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AskAndDeleteDatabase()
        {
            if (string.IsNullOrEmpty(currentDbPath) || !File.Exists(currentDbPath)) return;

            var result = MessageBox.Show(
                "내보낸 단어를 킨들 DB에서 모두 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.",
                "단어장 초기화 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                DeleteDatabaseFile(currentDbPath);
            }
        }

        private void DeleteDatabaseFile(string dbPath)
        {
            lblStatus.Text = "보카 DB를 삭제합니다.. 단말기를 제거하지 마세요..";
            try
            {
                // 1. 메모리에 잡혀있는 SQLite 파일 핸들 해제
                SqliteConnection.ClearAllPools();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // 2. 만약을 위해 백업 생성 (.bak)
                string backupPath = dbPath + $".backup_{DateTime.Now:yyyyMMddHHmmss}";
                File.Copy(dbPath, backupPath, true);

                // 3. 실제 vocab.db 파일 삭제
                File.Delete(dbPath);

                // (부수 파일이 존재할 경우 함께 삭제: wal, shm)
                if (File.Exists(dbPath + "-wal")) File.Delete(dbPath + "-wal");
                if (File.Exists(dbPath + "-shm")) File.Delete(dbPath + "-shm");

                // 4. 상태 초기화
                vocaTable.Rows.Clear();
                lblStatus.Text = "vocab.db 파일이 삭제되었습니다.";
                currentDbPath = string.Empty;

                MessageBox.Show($"vocab.db 파일이 삭제되었습니다.\n(안전을 위해 백업 파일이 생성됨: {Path.GetFileName(backupPath)})", "삭제 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 삭제 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCsv(string text)
        {
            if (string.IsNullOrEmpty(text)) return "\"\"";
            return $"\"{text.Replace("\"", "\"\"")}\"";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 킨들이 이미 꽂혀있는지 자동 확인
            string detectedPath = AutoDetectKindleVocabPath();
            if (!string.IsNullOrEmpty(detectedPath))
            {
                currentDbPath = detectedPath;
            //    lblStatus.Text = $"킨들 감지됨: {detectedPath}";
                LoadKindleVocab(currentDbPath);
            }
            else
            {
            //    lblStatus.Text = "킨들이 감지되지 않았습니다. USB를 연결하거나 [자동 감지/열기]를 누르세요.";
            }
        }

        /// <summary>
        /// 연결된 드라이브 중 킨들 vocab.db를 자동으로 찾아 경로를 반환합니다.
        /// </summary>
        private string AutoDetectKindleVocabPath()
        {
            try
            {
                // PC에 연결된 모든 드라이브 순회
                foreach (var drive in DriveInfo.GetDrives())
                {
                    // 이동식 드라이브(Removable) 또는 사용 가능한 드라이브 확인
                    if (drive.IsReady)
                    {
                        // 예상되는 킨들 경로 조합
                        string candidatePath = Path.Combine(drive.RootDirectory.FullName, "system", "vocabulary", "vocab.db");

                        if (File.Exists(candidatePath))
                        {
                            return candidatePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"드라이브 검색 중 오류: {ex.Message}", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            AskAndDeleteDatabase();
        }
    }


}

