using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KURSACH
{
    public partial class Form2 : Form
    {
        private Dictionary<int, Point> vertices;
        private List<Tuple<int, int, int>> edges;
        private string folderPath = "C:\\Users\\Owner\\Documents\\Visual Studio 2022\\Saves\\KURSACH\\"; //Путь ко всем файлам
        private Button runAlgorithmButton;
        private Button runEdmondsKarpButton;

        public Form2()
        {
            InitializeComponent();
            vertices = new Dictionary<int, Point>();
            edges = new List<Tuple<int, int, int>>();

            // Создаем ComboBox для выбора файла
            ComboBox fileComboBox = new ComboBox();
            fileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            fileComboBox.Location = new Point(10, 10); // Позиция ComboBox на форме
            fileComboBox.Size = new Size(100, 30);
            fileComboBox.SelectedIndexChanged += FileComboBox_SelectedIndexChanged;

            // Добавляем доступные файлы в ComboBox
            string[] files = { "graf1.txt", "graf2.txt", "graf3.txt" };
            fileComboBox.Items.AddRange(files);
            this.Controls.Add(fileComboBox);

            // Загрузим данные из первого файла по умолчанию
            if (files.Length > 0)
            {
                LoadCoordinatesFromFile(Path.Combine(folderPath, files[0]));
                CreateButtons(); // Создаём кнопки при инициализации формы
            }

            // Добавляем кнопку для запуска алгоритма Форда-Фалкерсона
            runAlgorithmButton = new Button
            {
                Text = "Запуск алгоритма Форда-Фалкерсона",
                Location = new Point(10, 50),
                Size = new Size(250, 30)
            };
            runAlgorithmButton.Click += RunFordFulkersonButton_Click;
            this.Controls.Add(runAlgorithmButton);

            // Добавляем кнопку для запуска алгоритма Эдмондса-Карпа
            runEdmondsKarpButton = new Button
            {
                Text = "Запуск алгоритма Эдмондса-Карпа",
                Location = new Point(10, 90),
                Size = new Size(250, 30)
            };
            runEdmondsKarpButton.Click += RunEdmondsKarpButton_Click;
            this.Controls.Add(runEdmondsKarpButton);
        }

        private void LoadCoordinatesFromFile(string filePath)
        {
            vertices.Clear();
            edges.Clear();

            if (File.Exists(filePath))
            {
                try
                {
                    var lines = File.ReadLines(filePath).ToList();
                    int vertexId = 1; // Идентификатор для вершины
                    bool isGraphSection = false; // Флаг для того, чтобы начать обрабатывать "Матрицу графа"

                    foreach (var line in lines)
                    {
                        var parts = line.Trim();
                        if (string.IsNullOrEmpty(parts))
                            continue;

                        // Если строка содержит координаты точки (в том числе название вершины)
                        if (!isGraphSection && parts.Contains(","))
                        {
                            var coordinates = parts.Split(',').Select(c => c.Trim()).ToArray();
                            if (coordinates.Length == 2)
                            {
                                if (int.TryParse(coordinates[0], out int x) && int.TryParse(coordinates[1], out int y))
                                {
                                    vertices[vertexId] = new Point(x, y);
                                    vertexId++;
                                }
                                else
                                {
                                    MessageBox.Show($"Неверный формат координат в строке: {line}");
                                }
                            }
                        }

                        // Если строка содержит информацию о соединении (Матрица графа)
                        else if (parts.StartsWith("//Матрица графа"))
                        {
                            isGraphSection = true;
                        }

                        // Если строка содержит информацию о рёбрах между вершинами
                        else if (parts.Contains(","))
                        {
                            var edgeInfo = parts.Split(',').Select(c => c.Trim()).ToArray();
                            if (edgeInfo.Length == 3)
                            {
                                if (int.TryParse(edgeInfo[0], out int vertex1) &&
                                    int.TryParse(edgeInfo[1], out int vertex2) &&
                                    int.TryParse(edgeInfo[2], out int weight))
                                {
                                    edges.Add(Tuple.Create(vertex1, vertex2, weight));
                                }
                                else
                                {
                                    MessageBox.Show($"Неверный формат рёбер в строке: {line}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при чтении файла: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Файл не найден: " + filePath);
            }
        }

        private void CreateButtons()
        {
            // Удаляем только старые кнопки, чтобы не дублировать их
            this.Controls.OfType<Button>().Where(b => b != runAlgorithmButton && b != runEdmondsKarpButton).ToList().ForEach(button => button.Dispose());

            // Создаём новые кнопки для каждой вершины
            foreach (var vertex in vertices)
            {
                Button button = new Button
                {
                    Width = 30,
                    Height = 30,
                    Location = new Point(vertex.Value.X - 15, vertex.Value.Y - 15),
                    BackColor = Color.Red,
                    FlatStyle = FlatStyle.Flat,
                    Text = vertex.Key.ToString(), // Номер вершины
                    FlatAppearance = { BorderSize = 0 }
                };
                this.Controls.Add(button);
            }

            this.Invalidate();
        }

        private void FileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedFile = (string)comboBox.SelectedItem;
            LoadCoordinatesFromFile(Path.Combine(folderPath, selectedFile));
            CreateButtons();  // Создаём кнопки заново для выбранного файла
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (vertices.Count > 0 && edges.Count > 0)
            {
                Graphics g = e.Graphics;
                Font font = new Font("Arial", 10);
                Brush brush = Brushes.Black;

                foreach (var edge in edges)
                {
                    Point vertex1 = vertices[edge.Item1];
                    Point vertex2 = vertices[edge.Item2];
                    int weight = edge.Item3;

                    g.DrawLine(Pens.Black, vertex1, vertex2);
                    float midX = (vertex1.X + vertex2.X) / 2f;
                    float midY = (vertex1.Y + vertex2.Y) / 2f;

                    float offsetX = 0;
                    float offsetY = -15;

                    g.DrawString(weight.ToString(), font, brush, midX + offsetX, midY + offsetY);
                }
            }
        }

        // Алгоритм Форда-Фалкерсона
        public class FordFulkerson
        {
            private Dictionary<int, Point> vertices;
            private List<Tuple<int, int, int>> edges;
            private Dictionary<int, Dictionary<int, int>> capacity;
            private Dictionary<int, int> flowThroughVertex;
            private Dictionary<int, Dictionary<int, int>> graph;

            public FordFulkerson(Dictionary<int, Point> vertices, List<Tuple<int, int, int>> edges)
            {
                this.vertices = vertices;
                this.edges = edges;
                capacity = new Dictionary<int, Dictionary<int, int>>();
                flowThroughVertex = new Dictionary<int, int>();
                graph = new Dictionary<int, Dictionary<int, int>>();

                foreach (var edge in edges)
                {
                    if (!graph.ContainsKey(edge.Item1))
                        graph[edge.Item1] = new Dictionary<int, int>();
                    if (!graph.ContainsKey(edge.Item2))
                        graph[edge.Item2] = new Dictionary<int, int>();

                    graph[edge.Item1][edge.Item2] = edge.Item3;
                    if (!capacity.ContainsKey(edge.Item1))
                        capacity[edge.Item1] = new Dictionary<int, int>();
                    capacity[edge.Item1][edge.Item2] = edge.Item3;
                }
            }

            public int FindMaxFlow(int source, int sink)
            {
                int maxFlow = 0;
                flowThroughVertex.Clear();

                while (true)
                {
                    var path = FindAugmentingPath(source, sink);
                    if (path == null)
                        break;

                    int pathFlow = int.MaxValue;
                    foreach (var edge in path)
                    {
                        pathFlow = Math.Min(pathFlow, capacity[edge.Item1][edge.Item2]);
                    }

                    foreach (var edge in path)
                    {
                        capacity[edge.Item1][edge.Item2] -= pathFlow;
                        if (!capacity.ContainsKey(edge.Item2))
                            capacity[edge.Item2] = new Dictionary<int, int>();
                        if (!capacity[edge.Item2].ContainsKey(edge.Item1))
                            capacity[edge.Item2][edge.Item1] = 0;
                        capacity[edge.Item2][edge.Item1] += pathFlow;

                        if (!flowThroughVertex.ContainsKey(edge.Item1))
                            flowThroughVertex[edge.Item1] = 0;
                        flowThroughVertex[edge.Item1] += pathFlow;
                    }

                    maxFlow += pathFlow;
                }

                return maxFlow;
            }

            private List<Tuple<int, int>> FindAugmentingPath(int source, int sink)
            {
                var parent = new Dictionary<int, int>();
                var visited = new HashSet<int>();
                var queue = new Queue<int>();
                queue.Enqueue(source);
                visited.Add(source);

                while (queue.Count > 0)
                {
                    int u = queue.Dequeue();
                    if (u == sink)
                        break;

                    foreach (var v in graph[u].Keys)
                    {
                        if (!visited.Contains(v) && capacity[u].ContainsKey(v) && capacity[u][v] > 0)
                        {
                            queue.Enqueue(v);
                            visited.Add(v);
                            parent[v] = u;
                        }
                    }
                }

                if (!visited.Contains(sink))
                    return null;

                var path = new List<Tuple<int, int>>();
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    path.Insert(0, Tuple.Create(u, v));
                }

                return path;
            }
            public Dictionary<int, int> GetFlowThroughVertex()
            {
                return flowThroughVertex;
            }
        }

        // Обработчик кнопки алгоритма Форда-Фалкерсона
        private void RunFordFulkersonButton_Click(object sender, EventArgs e)
        {
            FordFulkerson ff = new FordFulkerson(vertices, edges);

            int source = 1;
            int sink = vertices.Count;

            int maxFlow = ff.FindMaxFlow(source, sink);

            var resultForm = new Form
            {
                Text = "Результаты алгоритма Форда-Фалкерсона",
                Size = new Size(300, 300)
            };

            var resultLabel = new Label
            {
                Text = $"Максимальный поток: {maxFlow}",
                Location = new Point(10, 10),
                Size = new Size(250, 30)
            };

            resultForm.Controls.Add(resultLabel);

            int yOffset = 40; // Начальная позиция для вывода информации о потоке через вершины

            foreach (var vertex in vertices)
            {
                int flow = ff.GetFlowThroughVertex().ContainsKey(vertex.Key)
                    ? ff.GetFlowThroughVertex()[vertex.Key]
                    : 0;

                Label vertexLabel = new Label
                {
                    Text = $"Вершина {vertex.Key} - Поток: {flow}",
                    Location = new Point(10, yOffset),
                    Size = new Size(250, 30)
                };
                resultForm.Controls.Add(vertexLabel);
                yOffset += 30; // Смещение для следующей вершины
            }

            resultForm.Controls.Add(resultLabel);
            resultForm.ShowDialog();
        }



        // Алгоритм Эдмондса-Карпа
        public class EdmondsKarp
        {
            private Dictionary<int, Point> vertices;
            private List<Tuple<int, int, int>> edges;
            private Dictionary<int, Dictionary<int, int>> capacity;
            private Dictionary<int, Dictionary<int, int>> flow;
            private Dictionary<int, Dictionary<int, int>> graph;

            public EdmondsKarp(Dictionary<int, Point> vertices, List<Tuple<int, int, int>> edges)
            {
                this.vertices = vertices;
                this.edges = edges;
                capacity = new Dictionary<int, Dictionary<int, int>>();
                flow = new Dictionary<int, Dictionary<int, int>>(); // Ensure flow is a Dictionary<int, Dictionary<int, int>>.
                graph = new Dictionary<int, Dictionary<int, int>>();

                foreach (var edge in edges)
                {
                    if (!graph.ContainsKey(edge.Item1))
                        graph[edge.Item1] = new Dictionary<int, int>();
                    if (!graph.ContainsKey(edge.Item2))
                        graph[edge.Item2] = new Dictionary<int, int>();

                    graph[edge.Item1][edge.Item2] = edge.Item3;
                    if (!capacity.ContainsKey(edge.Item1))
                        capacity[edge.Item1] = new Dictionary<int, int>();
                    capacity[edge.Item1][edge.Item2] = edge.Item3;

                    // Initialize flow dictionary for each edge
                    if (!flow.ContainsKey(edge.Item1))
                        flow[edge.Item1] = new Dictionary<int, int>();
                    if (!flow[edge.Item1].ContainsKey(edge.Item2))
                        flow[edge.Item1][edge.Item2] = 0;
                }
            }

            public int FindMaxFlow(int source, int sink)
            {
                int maxFlow = 0;
                flow.Clear();

                while (true)
                {
                    var path = FindAugmentingPath(source, sink);
                    if (path == null)
                        break;

                    int pathFlow = int.MaxValue;
                    foreach (var edge in path)
                    {
                        pathFlow = Math.Min(pathFlow, capacity[edge.Item1][edge.Item2] - flow[edge.Item1][edge.Item2]);
                    }

                    foreach (var edge in path)
                    {
                        capacity[edge.Item1][edge.Item2] -= pathFlow;
                        if (!capacity.ContainsKey(edge.Item2))
                            capacity[edge.Item2] = new Dictionary<int, int>();
                        if (!capacity[edge.Item2].ContainsKey(edge.Item1))
                            capacity[edge.Item2][edge.Item1] = 0;
                        capacity[edge.Item2][edge.Item1] += pathFlow;

                        // Update the flow on the edge
                        flow[edge.Item1][edge.Item2] += pathFlow;
                    }

                    maxFlow += pathFlow;
                }

                return maxFlow;
            }

            private List<Tuple<int, int>> FindAugmentingPath(int source, int sink)
            {
                var parent = new Dictionary<int, int>();
                var visited = new HashSet<int>();
                var queue = new Queue<int>();
                queue.Enqueue(source);
                visited.Add(source);

                while (queue.Count > 0)
                {
                    int u = queue.Dequeue();
                    if (u == sink)
                        break;

                    foreach (var v in graph[u].Keys)
                    {
                        // Initialize flow[u][v] to 0 if not exists
                        if (!flow.ContainsKey(u))
                            flow[u] = new Dictionary<int, int>();
                        if (!flow[u].ContainsKey(v))
                            flow[u][v] = 0;

                        // Check the capacity and the flow to decide if we can augment
                        if (!visited.Contains(v) && capacity[u].ContainsKey(v) && flow[u][v] < capacity[u][v])
                        {
                            queue.Enqueue(v);
                            visited.Add(v);
                            parent[v] = u;
                        }
                    }
                }

                if (!visited.Contains(sink))
                    return null;

                var path = new List<Tuple<int, int>>();
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    path.Insert(0, Tuple.Create(u, v));
                }

                return path;
            }


            public Dictionary<int, Dictionary<int, int>> GetFlow()
            {
                return flow;
            }
        }

        // Обработчик кнопки алгоритма Эдмондса-Карпа
        private void RunEdmondsKarpButton_Click(object sender, EventArgs e)
        {
            EdmondsKarp ek = new EdmondsKarp(vertices, edges);

            int source = 1;
            int sink = vertices.Count;

            int maxFlow = ek.FindMaxFlow(source, sink);

            var resultForm = new Form
            {
                Text = "Результаты алгоритма Эдмондса-Карпа",
                Size = new Size(300, 300)
            };

            var resultLabel = new Label
            {
                Text = $"Максимальный поток: {maxFlow}",
                Location = new Point(10, 10),
                Size = new Size(250, 30)
            };

            resultForm.Controls.Add(resultLabel);

            int yOffset = 40; // Начальная позиция для вывода информации о потоке через вершины

            foreach (var vertex in vertices)
            {
                int flow = ek.GetFlow().ContainsKey(vertex.Key)
                    ? ek.GetFlow()[vertex.Key].Sum(kv => kv.Value) // Sum the flows for each vertex
                    : 0;

                Label vertexLabel = new Label
                {
                    Text = $"Вершина {vertex.Key} - Поток: {flow}",
                    Location = new Point(10, yOffset),
                    Size = new Size(250, 30)
                };
                resultForm.Controls.Add(vertexLabel);
                yOffset += 30; // Смещение для следующей вершины
            }

            resultForm.Controls.Add(resultLabel);
            resultForm.ShowDialog();
        }

    }
}