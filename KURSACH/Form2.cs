﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KURSACH
{
    public partial class Form2 : Form
    {
        private Dictionary<int, Point> vertices; // Словник для зберігання координат вершин
        private List<Tuple<int, int, int>> edges; // Список для зберігання рёбер (вершина1, вершина2, вага)
        private string folderPath = "C:\\Users\\Owner\\Documents\\Visual Studio 2022\\Saves\\KURSACH\\"; // Шлях до файлів
        private Button runAlgorithmButton; // Кнопка для запуску алгоритму Форда-Фалкерсона
        private Button runEdmondsKarpButton; // Кнопка для запуску алгоритму Едмондса-Карпа
        private Button runDeliveryButton; // Кнопка для запуску обчислення доставки

        public Form2()
        {
            InitializeComponent();
            vertices = new Dictionary<int, Point>(); // Ініціалізація словника вершин
            edges = new List<Tuple<int, int, int>>(); // Ініціалізація списку рёбер

            // Створюємо ComboBox для вибору файлу
            ComboBox fileComboBox = new ComboBox();
            fileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            fileComboBox.Location = new Point(10, 10);
            fileComboBox.Size = new Size(100, 30);
            fileComboBox.SelectedIndexChanged += FileComboBox_SelectedIndexChanged;

            // Додаємо доступні файли в ComboBox
            string[] files = { "graf1.txt", "graf2.txt", "graf3.txt" };
            fileComboBox.Items.AddRange(files);
            this.Controls.Add(fileComboBox);

            // Завантажуємо дані з першого файлу за замовчуванням
            if (files.Length > 0)
            {
                LoadCoordinatesFromFile(Path.Combine(folderPath, files[0]));
                CreateButtons(); // Створюємо кнопки для вершин при ініціалізації форми
            }

            // Додаємо кнопку для запуску алгоритму Форда-Фалкерсона
            runAlgorithmButton = new Button
            {
                Text = "Запуск алгоритма Форда-Фалкерсона",
                Location = new Point(10, 50),
                Size = new Size(250, 30)
            };
            runAlgorithmButton.Click += RunFordFulkersonButton_Click;
            this.Controls.Add(runAlgorithmButton);

            // Додаємо кнопку для запуску алгоритму Едмондса-Карпа
            runEdmondsKarpButton = new Button
            {
                Text = "Запуск алгоритма Eдмондса-Карпа",
                Location = new Point(10, 90),
                Size = new Size(250, 30)
            };
            runEdmondsKarpButton.Click += RunEdmondsKarpButton_Click;
            this.Controls.Add(runEdmondsKarpButton);

            // Додаємо кнопку для запуску обчислення доставки
            runDeliveryButton = new Button
            {
                Text = "Розрахувати доставку",
                Location = new Point(10, 130),
                Size = new Size(250, 30)
            };
            runDeliveryButton.Click += runDeliveryButton_Click;
            this.Controls.Add(runDeliveryButton);
        }

        // Метод для завантаження координат вершин і рёбер з файлу
        private void LoadCoordinatesFromFile(string filePath)
        {
            vertices.Clear();
            edges.Clear();

            if (File.Exists(filePath))
            {
                try
                {
                    var lines = File.ReadLines(filePath).ToList(); // Читаємо всі рядки з файлу
                    int vertexId = 1; // Ідентифікатор для вершин
                    bool isGraphSection = false; // Флаг для того, щоб почати обробку "Матриці графа"

                    foreach (var line in lines)
                    {
                        var parts = line.Trim();
                        if (string.IsNullOrEmpty(parts))
                            continue;

                        // Якщо рядок містить координати точки (в тому числі назву вершини)
                        if (!isGraphSection && parts.Contains(","))
                        {
                            var coordinates = parts.Split(',').Select(c => c.Trim()).ToArray();
                            if (coordinates.Length == 2)
                            {
                                if (int.TryParse(coordinates[0], out int x) && int.TryParse(coordinates[1], out int y))
                                {
                                    vertices[vertexId] = new Point(x, y); // Додаємо точку в словник
                                    vertexId++;
                                }
                                else
                                {
                                    MessageBox.Show($"Невірний формат координат у рядку: {line}");
                                }
                            }
                        }

                        // Якщо рядок містить інформацію про з'єднання (Матриця графа)
                        else if (parts.StartsWith("//Матрица графа"))
                        {
                            isGraphSection = true;
                        }

                        // Якщо рядок містить інформацію про рёбра між вершинами
                        else if (parts.Contains(","))
                        {
                            var edgeInfo = parts.Split(',').Select(c => c.Trim()).ToArray();
                            if (edgeInfo.Length == 3)
                            {
                                if (int.TryParse(edgeInfo[0], out int vertex1) &&
                                    int.TryParse(edgeInfo[1], out int vertex2) &&
                                    int.TryParse(edgeInfo[2], out int weight))
                                {
                                    edges.Add(Tuple.Create(vertex1, vertex2, weight)); // Додаємо ребра в список
                                }
                                else
                                {
                                    MessageBox.Show($"Неправильний формат ребер у рядку: {line}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка під час читання файлу: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Файл не знайдено: " + filePath);
            }
        }

        // Створення кнопок для кожної вершини на формі
        private void CreateButtons()
        {
            // Видаляємо старі кнопки (не створюємо їх повторно)
            this.Controls.OfType<Button>().Where(b => b != runAlgorithmButton && b != runEdmondsKarpButton && b != runDeliveryButton).ToList().ForEach(button => button.Dispose());

            // Створюємо нові кнопки для кожної вершини
            foreach (var vertex in vertices)
            {
                Button button = new Button
                {
                    Width = 30,
                    Height = 30,
                    Location = new Point(vertex.Value.X - 15, vertex.Value.Y - 15), // Центруємо кнопку на координатах
                    BackColor = Color.Red,
                    FlatStyle = FlatStyle.Flat,
                    Text = vertex.Key.ToString(), // Номер вершини
                    FlatAppearance = { BorderSize = 0 }
                };
                this.Controls.Add(button);
            }

            this.Invalidate(); // Оновлюємо форму
        }

        // Обробник для зміни вибору файлу в ComboBox
        private void FileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedFile = (string)comboBox.SelectedItem;
            LoadCoordinatesFromFile(Path.Combine(folderPath, selectedFile)); // Завантажуємо нові дані з файлу
            CreateButtons();  // Створюємо кнопки заново для вибраного файлу
        }

        // Метод для малювання графа (вершини і рёбра)
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
                    Point vertex1 = vertices[edge.Item1]; // Отримуємо координати першої вершини
                    Point vertex2 = vertices[edge.Item2]; // Отримуємо координати другої вершини
                    int weight = edge.Item3; // Вага ребра

                    g.DrawLine(Pens.Black, vertex1, vertex2); // Малюємо лінію між вершинами
                    float midX = (vertex1.X + vertex2.X) / 2f;
                    float midY = (vertex1.Y + vertex2.Y) / 2f;

                    float offsetX = 0;
                    float offsetY = -15;

                    g.DrawString(weight.ToString(), font, brush, midX + offsetX, midY + offsetY); // Виводимо вагу ребра
                }
            }
        }

        //Гершиков Вадим КНТ-133

        // Алгоритм Форда-Фалкерсона
        public class FordFulkerson
        {
            private Dictionary<int, Point> vertices;  // Словник для зберігання вершин графа
            private List<Tuple<int, int, int>> edges; // Список рєбр графа (початкова вершина, кінцева вершина, пропускна здатність)
            private Dictionary<int, Dictionary<int, int>> capacity;  // Словник для зберігання пропускних здатностей рєбр
            private Dictionary<int, int> flowThroughVertex;  // Словник для зберігання потоку через кожну вершину
            private Dictionary<int, Dictionary<int, int>> graph;  // Граф у вигляді списку суміжності

            // Конструктор для ініціалізації графа і пропускних здатностей
            public FordFulkerson(Dictionary<int, Point> vertices, List<Tuple<int, int, int>> edges)
            {
                this.vertices = vertices;
                this.edges = edges;
                capacity = new Dictionary<int, Dictionary<int, int>>();
                flowThroughVertex = new Dictionary<int, int>();
                graph = new Dictionary<int, Dictionary<int, int>>();

                foreach (var edge in edges)
                {
                    // Ініціалізуємо граф для зберігання пропускних здатностей
                    if (!graph.ContainsKey(edge.Item1))
                        graph[edge.Item1] = new Dictionary<int, int>();
                    if (!graph.ContainsKey(edge.Item2))
                        graph[edge.Item2] = new Dictionary<int, int>();

                    graph[edge.Item1][edge.Item2] = edge.Item3;  // Встановлюємо пропускну здатність для рєбра
                    if (!capacity.ContainsKey(edge.Item1))
                        capacity[edge.Item1] = new Dictionary<int, int>();
                    capacity[edge.Item1][edge.Item2] = edge.Item3;  // Записуємо пропускну здатність рєбра
                }
            }

            // Основний метод для знаходження максимального потоку
            public int FindMaxFlow(int source, int sink)
            {
                int maxFlow = 0;
                flowThroughVertex.Clear();

                // Поки можемо знайти шлях збільшення потоку, продовжуємо
                while (true)
                {
                    var path = FindAugmentingPath(source, sink);
                    if (path == null)
                        break;

                    // Знаходимо мінімальну пропускну здатність на шляху
                    int pathFlow = int.MaxValue;
                    foreach (var edge in path)
                    {
                        pathFlow = Math.Min(pathFlow, capacity[edge.Item1][edge.Item2]);
                    }

                    // Оновлюємо потоки на рєбрах та пропускні здатності
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

                    // Додаємо потік до максимального потоку
                    maxFlow += pathFlow;
                }

                return maxFlow;  // Повертаємо максимальний потік
            }

            // Метод для знаходження шляху збільшення потоку
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

                    // Шукаємо сусідів вершини u для шляху збільшення потоку
                    foreach (var v in graph[u].Keys)
                    {
                        if (!visited.Contains(v) && capacity[u].ContainsKey(v) && capacity[u][v] > 0)
                        {
                            queue.Enqueue(v);
                            visited.Add(v);
                            parent[v] = u;  // Записуємо попередню вершину для відновлення шляху
                        }
                    }
                }

                // Якщо шляху немає, повертаємо null
                if (!visited.Contains(sink))
                    return null;

                var path = new List<Tuple<int, int>>();
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    path.Insert(0, Tuple.Create(u, v));  // Відновлюємо шлях
                }

                return path;
            }

            // Метод для отримання потоку через вершини
            public Dictionary<int, int> GetFlowThroughVertex()
            {
                return flowThroughVertex;
            }
        }

        // Обробник натискання кнопки для запуску алгоритму Форда-Фалкерсона
        private void RunFordFulkersonButton_Click(object sender, EventArgs e)
        {
            FordFulkerson ff = new FordFulkerson(vertices, edges);

            int source = 1;  // Початкова вершина (джерело)
            int sink = vertices.Count;  // Кінцева вершина (стік)

            int maxFlow = ff.FindMaxFlow(source, sink);

            // Створюємо форму для відображення результатів
            var resultForm = new Form
            {
                Text = "Результаты алгоритма Форда-Фалкерсона",
                AutoSize = true
            };

            // Додаємо мітку для максимального потоку
            var resultLabel = new Label
            {
                Text = $"Максимальный поток: {maxFlow}",
                Location = new Point(10, 10),
                AutoSize = true
            };

            resultForm.Controls.Add(resultLabel);

            // Додаємо інформацію про потік через кожну вершину
            int yOffset = 40; // Початкова позиція для вивода інформації о потоках через вершини

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
                yOffset += 30; // Зміщення для наступної вершини
            }

            resultForm.Controls.Add(resultLabel);

            // Відображаємо форму результатів
            resultForm.ShowDialog();
        }

        // Алгоритм Едмондса-Карпа
        public class EdmondsKarp
        {
            private Dictionary<int, Point> vertices; // Словник для зберігання вершин графа
            private List<Tuple<int, int, int>> edges; // Список для зберігання ребер графа та їхніх пропускних спроможностей
            private Dictionary<int, Dictionary<int, int>> capacity; // Словник для зберігання пропускних спроможностей між парами вершин
            private Dictionary<int, Dictionary<int, int>> flow; // Словник для зберігання потоку між парами вершин
            private Dictionary<int, Dictionary<int, int>> graph; // Словник для зберігання графа та його ребер

            // Конструктор ініціалізує граф, пропускні спроможності і потоки
            public EdmondsKarp(Dictionary<int, Point> vertices, List<Tuple<int, int, int>> edges)
            {
                this.vertices = vertices;
                this.edges = edges;
                capacity = new Dictionary<int, Dictionary<int, int>>();
                flow = new Dictionary<int, Dictionary<int, int>>(); // Словник для потоку між вершинами
                graph = new Dictionary<int, Dictionary<int, int>>();

                // Ініціалізуємо граф та пропускні спроможності для кожного ребра
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

                    // Ініціалізуємо потоки для кожного ребра як 0
                    if (!flow.ContainsKey(edge.Item1))
                        flow[edge.Item1] = new Dictionary<int, int>();
                    if (!flow[edge.Item1].ContainsKey(edge.Item2))
                        flow[edge.Item1][edge.Item2] = 0;
                }
            }

            // Метод для знаходження максимального потоку між джерелом та стоком
            public int FindMaxFlow(int source, int sink)
            {
                int maxFlow = 0;
                flow.Clear(); // Очищаємо потоки перед новим обчисленням

                // Шукаємо шлях до потоку поки існують можливі шляхи для збільшення потоку
                while (true)
                {
                    var path = FindAugmentingPath(source, sink);
                    if (path == null)
                        break;

                    // Знаходимо мінімальний потік на шляху
                    int pathFlow = int.MaxValue;
                    foreach (var edge in path)
                    {
                        pathFlow = Math.Min(pathFlow, capacity[edge.Item1][edge.Item2] - flow[edge.Item1][edge.Item2]);
                    }

                    // Оновлюємо потоки та пропускні спроможності
                    foreach (var edge in path)
                    {
                        capacity[edge.Item1][edge.Item2] -= pathFlow;
                        if (!capacity.ContainsKey(edge.Item2))
                            capacity[edge.Item2] = new Dictionary<int, int>();
                        if (!capacity[edge.Item2].ContainsKey(edge.Item1))
                            capacity[edge.Item2][edge.Item1] = 0;
                        capacity[edge.Item2][edge.Item1] += pathFlow;

                        flow[edge.Item1][edge.Item2] += pathFlow; // Збільшуємо потік по ребру
                    }

                    maxFlow += pathFlow; // Додаємо до максимального потоку
                }

                return maxFlow; // Повертаємо максимальний потік
            }

            // Метод для знаходження шляху з джерела до стоку
            private List<Tuple<int, int>> FindAugmentingPath(int source, int sink)
            {
                var parent = new Dictionary<int, int>(); // Для збереження шляхів
                var visited = new HashSet<int>(); // Для відслідковування відвіданих вершин
                var queue = new Queue<int>(); // Черга для BFS
                queue.Enqueue(source); // Починаємо з джерела
                visited.Add(source);

                // BFS для пошуку шляху з джерела до стоку
                while (queue.Count > 0)
                {
                    int u = queue.Dequeue();
                    if (u == sink)
                        break;

                    foreach (var v in graph[u].Keys)
                    {
                        // Ініціалізація потоку між вершинами, якщо ще не існує
                        if (!flow.ContainsKey(u))
                            flow[u] = new Dictionary<int, int>();
                        if (!flow[u].ContainsKey(v))
                            flow[u][v] = 0;

                        // Перевірка, чи є можливість збільшити потік по ребру
                        if (!visited.Contains(v) && capacity[u].ContainsKey(v) && flow[u][v] < capacity[u][v])
                        {
                            queue.Enqueue(v);
                            visited.Add(v);
                            parent[v] = u; // Зберігаємо батьківську вершину для побудови шляху
                        }
                    }
                }

                // Якщо не знайдено шлях, повертаємо null
                if (!visited.Contains(sink))
                    return null;

                // Відновлюємо шлях з джерела до стоку
                var path = new List<Tuple<int, int>>();
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    path.Insert(0, Tuple.Create(u, v)); // Додаємо ребра в шлях
                }

                return path;
            }

            // Метод для отримання потоку між вершинами
            public Dictionary<int, Dictionary<int, int>> GetFlow()
            {
                return flow;
            }
        }

        // Обработчик кнопки алгоритма Эдмондса-Карпа
        private void RunEdmondsKarpButton_Click(object sender, EventArgs e)
        {
            // Створюємо об'єкт алгоритму Едмондса-Карпа з вершин і ребер
            EdmondsKarp ek = new EdmondsKarp(vertices, edges);

            int source = 1; // Джерело
            int sink = vertices.Count; // Сток

            // Обчислюємо максимальний потік
            int maxFlow = ek.FindMaxFlow(source, sink);

            // Створюємо форму для відображення результатів
            var resultForm = new Form
            {
                Text = "Результаты алгоритма Эдмондса-Карпа",
                AutoSize = true,
            };

            // Мітка для відображення максимального потоку
            var resultLabel = new Label
            {
                Text = $"Максимальный поток: {maxFlow}",
                Location = new Point(10, 10),
                AutoSize = true,
            };

            resultForm.Controls.Add(resultLabel);

            int yOffset = 40; // Початкове розташування для виведення інформації про потік

            // Виведення потоку для кожної вершини
            foreach (var vertex in vertices)
            {
                int flow = ek.GetFlow().ContainsKey(vertex.Key)
                    ? ek.GetFlow()[vertex.Key].Sum(kv => kv.Value) // Сума потоків для кожної вершини
                    : 0;

                Label vertexLabel = new Label
                {
                    Text = $"Вершина {vertex.Key} - Поток: {flow}",
                    Location = new Point(10, yOffset),
                    Size = new Size(250, 30)
                };
                resultForm.Controls.Add(vertexLabel);
                yOffset += 30; // Зміщуємо вниз для наступного рядка
            }

            resultForm.Controls.Add(resultLabel);

            // Відображаємо форму результатів
            resultForm.ShowDialog();
        }

        private void runDeliveryButton_Click(object sender, EventArgs e)
        {
            // Створюємо об'єкт для алгоритму Едмондса-Карпа для пошуку максимального потоку
            EdmondsKarp ek = new EdmondsKarp(vertices, edges);

            int source = 1; // Джерело в графі
            int sink = vertices.Count; // Сток в графі

            // Знаходимо максимальний потік за допомогою алгоритму Едмондса-Карпа
            int maxFlow = ek.FindMaxFlow(source, sink);

            // Створюємо об'єкт для алгоритму Форда-Фалкерсона для пошуку максимального потоку
            FordFulkerson ff = new FordFulkerson(vertices, edges);

            // Задаємо джерело та сток для другого алгоритму
            int source1 = 1;
            int sink1 = vertices.Count;

            // Знаходимо максимальний потік за допомогою алгоритму Форда-Фалкерсона
            int maxFlow1 = ff.FindMaxFlow(source1, sink1);

            // Створюємо нову форму для відображення калькулятора доставки
            var resultForm = new Form
            {
                Text = "Калькулятор доставки", // Назва форми
                AutoSize = false, // Автоматичний розмір форми
                Size = new System.Drawing.Size(350, 550),
                AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F),
                AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font,
            };

            // Створюємо лейбл для виведення тексту "Назва товару"
            var label2 = new Label
            {
                AutoSize = true,
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                Location = new System.Drawing.Point(12, 9), // Позиція лейбла
                Name = "label2",
                Size = new System.Drawing.Size(122, 23),
                TabIndex = 2,
                Text = "Назва товару:\r\n", // Текст лейбла
            };
            resultForm.Controls.Add(label2); // Додаємо лейбл на форму

            // Створюємо лейбл для введення кількості товару
            var label3 = new Label
            {
                AutoSize = true,
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                Location = new System.Drawing.Point(12, 80), // Позиція лейбла
                Name = "label3",
                Size = new System.Drawing.Size(88, 23),
                TabIndex = 3,
                Text = "Кількість:", // Текст лейбла
            };
            resultForm.Controls.Add(label3); // Додаємо лейбл на форму

            // Створюємо поле для вводу кількості товару (NumericUpDown)
            var numericUpDownQuantity = new NumericUpDown
            {
                Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                Location = new System.Drawing.Point(106, 80), // Позиція поля вводу
                Maximum = new decimal(new int[] { 1000000, 0, 0, 0 }), // Максимальне значення
                Name = "numericUpDownQuantity",
                Size = new System.Drawing.Size(120, 29),
                TabIndex = 6,
            };

            resultForm.Controls.Add(numericUpDownQuantity); // Додаємо поле на форму

            // Створюємо кнопку для розрахунку доставки
            var buttonCalculate = new Button
            {
                //BackColor = System.Drawing.SystemColors.InactiveCaption,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                Location = new System.Drawing.Point(1, 120), // Позиція кнопки
                Name = "buttonCalculate",
                Size = new System.Drawing.Size(140, 52),
                TabIndex = 7,
                Text = "Розрахувати дані посилки", // Текст на кнопці
                UseVisualStyleBackColor = false,
            };
            resultForm.Controls.Add(buttonCalculate); // Додаємо кнопку на форму
            var buttonFind = new Button
            {
                //BackColor = System.Drawing.SystemColors.InactiveCaption,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                Location = new System.Drawing.Point(1, 190), // Позиція кнопки
                Name = "buttonCalculate",
                Size = new System.Drawing.Size(140, 52),
                TabIndex = 7,
                Text = "Пошук замовлення", // Текст на кнопці
                UseVisualStyleBackColor = false,
            };
            resultForm.Controls.Add(buttonFind); // Додаємо кнопку на форму

            var textBoxOrderNumber = new TextBox
            {
                Font = new System.Drawing.Font("Arial Narrow", 14.25F),
                Location = new System.Drawing.Point(150, 190),
                Size = new System.Drawing.Size(100, 29),

            };
            resultForm.Controls.Add(textBoxOrderNumber);

            // Лейбл для відображення результатів
            var labelResult = new Label
            {
                AutoSize = true,
                BackColor = System.Drawing.Color.Transparent,
                Dock = System.Windows.Forms.DockStyle.Top,
                Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                Location = new System.Drawing.Point(0, 0),
                Name = "labelResult",
                Size = new System.Drawing.Size(0, 23),
                TabIndex = 8
            };
            resultForm.Controls.Add(labelResult); // Додаємо лейбл на форму

            // Створюємо випадаючий список для вибору товару
            var comboBoxProduct = new ComboBox
            {
                Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                FormattingEnabled = true,
                Location = new System.Drawing.Point(140, 9), // Позиція комбобоксу
                Name = "comboBoxProduct",
                Size = new System.Drawing.Size(121, 31),
                TabIndex = 10,
            };
            resultForm.Controls.Add(comboBoxProduct); // Додаємо комбобокс на форму

            comboBoxProduct.Items.AddRange(new object[] {
                "хліб 5", // Товар 1
                "молоко 10", // Товар 2
                "яйця 8" // Товар 3
            });

            // Оголошуємо словник для цін на товари
            Dictionary<string, int> productPrices = new Dictionary<string, int>
            {
                { "хліб 5", 5 }, // Ціна хліба
                { "молоко 10", 10 }, // Ціна молока
                { "яйця 8", 8 } // Ціна яєць
            };

            // Обробник натискання кнопки для розрахунку даних посилки
            buttonCalculate.Click += (object clickSender, EventArgs clickEvent) =>
            {
                // Перевіряє, чи comboBoxProduct ініціалізовано та містить вибраний елемент
                if (comboBoxProduct == null || comboBoxProduct.SelectedItem == null)
                {
                    MessageBox.Show("Виберіть продукт");
                    return;
                }
                // Отримуємо обраний товар та його ціну
                string product = comboBoxProduct.SelectedItem.ToString();

                int pricePerUnit = productPrices[product];
                int quantity = (int)numericUpDownQuantity.Value;

                int deliveryTimeff = 0;
                int deliveryTimeed = 0;

                // Розрахунок часу доставки за допомогою алгоритмів
                if (quantity <= maxFlow)
                {
                    deliveryTimeff = 1;
                    deliveryTimeed = 1;
                }
                else
                {
                    deliveryTimeff = (int)Math.Floor((double)quantity / maxFlow) + 1;
                    deliveryTimeed = (int)Math.Floor((double)quantity / maxFlow1) + 1;
                }

                // Додаткові витрати на доставку, якщо доставка займає 1 день
                int deliverySurcharge = deliveryTimeff <= 3 ? 20 : 0;

                // Загальна вартість товару та доставки
                int totalCost = quantity * pricePerUnit + deliverySurcharge;

                // Виведення результатів на форму
                labelResult.Text = $"Замовлення:\n" +
                                   $"Товар: {product}\n" +
                                   $"Кількість: {quantity}\n" +
                                   $"Час доставки(Форда-Фалкерсона): {deliveryTimeff} дн.\n" +
                                   $"Час доставки(Едмондса-Карпа): {deliveryTimeed} дн.\n" +
                                   $"Загальна вартість: {totalCost} грн.";

                string textToSave = $"Замовлення:\n" +
                                    $"Товар: {product}\n" +
                                    $"Кількість: {quantity}\n" +
                                    $"Час доставки (Форда-Фалкерсона): {deliveryTimeff} дн.\n" +
                                    $"Час доставки (Едмондса-Карпа): {deliveryTimeed} дн.\n" +
                                    $"Загальна вартість: {totalCost} грн.\n\n";

                //Метод для нумерації замовлень
                // Вказуємо шлях до вхідного файлу, який містить дані для обробки
                string inputFilePath = "order.txt";  // Путь к исходному файлу
                // Вказуємо шлях до вихідного файлу, куди буде збережений результат
                string outputFilePath = "order.txt";  // Путь к выходному файлу

                try
                {
                    // Читаємо всі строки з вхідного файлу
                    string[] lines = File.ReadAllLines(inputFilePath);
                    // Знаходимо максимальний номер замовлення в файлі та визначаємо наступний номер
                    int currentOrderNumber = GetMaxOrderNumber(lines) + 1;  // Определяем следующий номер для добавления

                    // Відкриваємо вихідний файл для запису змінених даних
                    using (StreamWriter writer = new StreamWriter(outputFilePath))
                    {
                        // Перебираємо кожну строку з файлу
                        foreach (var line in lines)
                        {
                            // Перевіряємо, чи містить строка слово "Замовлення:"
                            if (line.Contains("Замовлення:"))
                            {
                                // Перевіряємо, чи є вже номер замовлення на початку рядка
                                if (!Regex.IsMatch(line, @"^\d+\.\s*Замовлення:"))
                                {
                                    // Якщо номера немає, додаємо новий номер перед текстом "Замовлення:"
                                    writer.WriteLine($"{currentOrderNumber}. Замовлення:");
                                    currentOrderNumber++;  // Збільшуємо номер для наступного замовлення
                                }
                                else
                                {
                                    // Якщо номер є, просто записуємо рядок без змін
                                    writer.WriteLine(line);
                                }
                            }
                            else
                            {
                                // Якщо в рядку немає слова "Замовлення:", просто записуємо його
                                writer.WriteLine(line);
                            }
                        }
                    }

                    // Виводимо повідомлення про успішну обробку документа
                    Console.WriteLine("Документ обработан и сохранен как 'order_numbered.txt'.");
                }
                catch (Exception ex)
                {
                    // Обробляємо помилки, якщо вони виникли під час виконання
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }

                // Метод для отримання максимального номера замовлення з файлу
                int GetMaxOrderNumber(string[] lines)
                {
                    int maxOrderNumber = 0;

                    // Перебираємо кожен рядок файлу
                    foreach (var line in lines)
                    {
                        // Шукаємо рядки, які починаються з числа та точки, а потім слідує "Замовлення:"
                        Match match = Regex.Match(line, @"^(\d+)\.\s*Замовлення:");
                        if (match.Success)
                        {
                            // Якщо знайдено номер, перетворюємо його на число та перевіряємо, чи є він максимальним
                            int currentOrderNumber = int.Parse(match.Groups[1].Value);
                            if (currentOrderNumber > maxOrderNumber)
                            {
                                maxOrderNumber = currentOrderNumber;  // Оновлюємо максимальний номер
                            }
                        }
                    }

                    // Повертаємо максимальний знайдений номер
                    return maxOrderNumber;
                }

                try
                {
                    // Записываем текст в файл (перезапишет файл, если он существует)
                    File.AppendAllText("order.txt", textToSave);
                }
                catch (Exception ex)
                {
                    // Если произошла ошибка, выводим сообщение
                    MessageBox.Show($"Помилка при збереженні файлу: {ex.Message}");
                }
            };

            buttonFind.Click += (object clickSender, EventArgs clickEvent) =>
            {
                // Отримуємо номер замовлення з текстового поля
                if (int.TryParse(textBoxOrderNumber.Text, out int orderNumberToFind))
                {
                    try
                    {
                        // Читаємо всі замовлення з файлу
                        string[] orders = File.ReadAllLines("order.txt");
                        string orderDetails = string.Empty;

                        // Пошук замовлення за номером
                        foreach (var line in orders)
                        {
                            if (line.StartsWith($"{orderNumberToFind}. Замовлення:"))
                            {
                                orderDetails += line + Environment.NewLine;
                                int index = Array.IndexOf(orders, line);

                                // Зчитуємо рядки замовлення, поки не зустрінемо наступний номер або кінець файлу
                                while (++index < orders.Length && !orders[index].StartsWith("Номер замовлення:"))
                                {
                                    orderDetails += orders[index] + Environment.NewLine;
                                }
                                break;
                            }
                        }

                        // Виведення результатів пошуку
                        if (!string.IsNullOrEmpty(orderDetails))
                        {
                            labelResult.Text = $"Замовлення знайдено:\n{orderDetails}";
                        }
                        else
                        {
                            labelResult.Text = "Замовлення з таким номером не знайдено.";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Обробка помилок читання файлу
                        MessageBox.Show($"Помилка при читанні файлу: {ex.Message}", "Помилка");
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, введіть коректний номер замовлення.", "Помилка вводу");
                }

            };

            // Створюємо панель для розміщення результатів
            var panel1 = new Panel
            {
                //BackColor = System.Drawing.SystemColors.InactiveCaption,
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D,
                Location = new System.Drawing.Point(1, 250), // Позиція панелі
                Name = "panel1",
                Size = new System.Drawing.Size(350, 200), // Розміри панелі
                TabIndex = 9,
            };
            resultForm.Controls.Add(panel1); // Додаємо панель на форму

            panel1.Controls.Add(labelResult);

            // Створюємо кнопку для перегляду всіх замовлень
            var buttonOrderView = new Button
            {
                Location = new Point(170, 120), // Позиція кнопки
                Name = "buttonOrderView",
                Size = new Size(140, 52),
                Text = "Замовлення", // Текст на кнопці
                UseVisualStyleBackColor = false,
            };
            resultForm.Controls.Add(buttonOrderView); // Додаємо кнопку на форму

            // Обробник натискання кнопки
            buttonOrderView.Click += (object clickSender, EventArgs clickEvent) =>
            {

                // Вказуємо шлях до вхідного файлу, який містить дані для обробки
                string inputFilePath = "order.txt";  // Путь к исходному файлу
                                                     // Вказуємо шлях до вихідного файлу, куди буде збережений результат
                string outputFilePath = "order.txt";  // Путь к выходному файлу

                try
                {
                    // Читаємо всі строки з вхідного файлу
                    string[] lines = File.ReadAllLines(inputFilePath);
                    // Знаходимо максимальний номер замовлення в файлі та визначаємо наступний номер
                    int currentOrderNumber = GetMaxOrderNumber(lines) + 1;  // Определяем следующий номер для добавления

                    // Відкриваємо вихідний файл для запису змінених даних
                    using (StreamWriter writer = new StreamWriter(outputFilePath))
                    {
                        // Перебираємо кожну строку з файлу
                        foreach (var line in lines)
                        {
                            // Перевіряємо, чи містить строка слово "Замовлення:"
                            if (line.Contains("Замовлення:"))
                            {
                                // Перевіряємо, чи є вже номер замовлення на початку рядка
                                if (!Regex.IsMatch(line, @"^\d+\.\s*Замовлення:"))
                                {
                                    // Якщо номера немає, додаємо новий номер перед текстом "Замовлення:"
                                    writer.WriteLine($"{currentOrderNumber}. Замовлення:");
                                    currentOrderNumber++;  // Збільшуємо номер для наступного замовлення
                                }
                                else
                                {
                                    // Якщо номер є, просто записуємо рядок без змін
                                    writer.WriteLine(line);
                                }
                            }
                            else
                            {
                                // Якщо в рядку немає слова "Замовлення:", просто записуємо його
                                writer.WriteLine(line);
                            }
                        }
                    }

                    // Виводимо повідомлення про успішну обробку документа
                    Console.WriteLine("Документ обработан и сохранен как 'order_numbered.txt'.");
                }
                catch (Exception ex)
                {
                    // Обробляємо помилки, якщо вони виникли під час виконання
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }

                // Метод для отримання максимального номера замовлення з файлу
                int GetMaxOrderNumber(string[] lines)
                {
                    int maxOrderNumber = 0;

                    // Перебираємо кожен рядок файлу
                    foreach (var line in lines)
                    {
                        // Шукаємо рядки, які починаються з числа та точки, а потім слідує "Замовлення:"
                        Match match = Regex.Match(line, @"^(\d+)\.\s*Замовлення:");
                        if (match.Success)
                        {
                            // Якщо знайдено номер, перетворюємо його на число та перевіряємо, чи є він максимальним
                            int currentOrderNumber = int.Parse(match.Groups[1].Value);
                            if (currentOrderNumber > maxOrderNumber)
                            {
                                maxOrderNumber = currentOrderNumber;  // Оновлюємо максимальний номер
                            }
                        }
                    }

                    // Повертаємо максимальний знайдений номер
                    return maxOrderNumber;
                }

                // Створюємо нову форму
                var Form = new Form
                {
                    Text = "Замовлення",
                    Size = new Size(360, 460), //Розмір форми
                };

                //Читаемо із файлу
                string fileContent = File.ReadAllText("order.txt");

                //Для виводу тексту
                var resultLabel = new Label
                {
                    Text = fileContent, //Текст із файлу
                    AutoSize = true, //Розмір: автоматичний
                    Location = new Point(0, 0), // Позиція
                    Name = "resultLabel",
                    Font = new Font("Arial", 10, FontStyle.Regular)

                };
                Form.Controls.Add(resultLabel); // Додаємо на форму

                // Створюємо панель
                var panel = new Panel
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    Location = new Point(0, 0), // Позиція панелі
                    Name = "panel",
                    Size = new Size(350, 420), // Розміри панелі
                    AutoScroll = true,

                };
                Form.Controls.Add(panel); // Додаємо панель на форму

                panel.Controls.Add(resultLabel); // Додаємо текст на панель

                // Відображаємо форму з калькулятором доставки

                Form.ShowDialog();
            };

            // Відображаємо форму з калькулятором доставки
            resultForm.ShowDialog();
        }

    }

}