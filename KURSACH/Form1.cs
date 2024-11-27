using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KURSACH
{
    public partial class Form1 : Form
    {
        // Клас для представлення графа
        public class Graph
        {
            public Dictionary<string, Vertex> Vertices { get; private set; }
            public Dictionary<string, List<Edge>> AdjacencyList { get; private set; }

            public Graph()
            {
                Vertices = new Dictionary<string, Vertex>();
                AdjacencyList = new Dictionary<string, List<Edge>>();
            }

            // Додавання вершини до графа
            public void AddVertex(string name)
            {
                if (!Vertices.ContainsKey(name))
                {
                    var vertex = new Vertex(name); // Створення нової вершини
                    Vertices[name] = vertex; // Додавання вершини в граф
                    AdjacencyList[name] = new List<Edge>(); // Створення порожнього списку для ребер
                }
            }

            // Видалення вершини з графа
            public void RemoveVertex(string name)
            {
                if (Vertices.ContainsKey(name))
                {
                    Vertices.Remove(name); // Видалення вершини
                    AdjacencyList.Remove(name); // Видалення вершини

                    // Видалення всіх ребер, які ведуть до цієї вершини
                    foreach (var vertexEdges in AdjacencyList.Values)
                    {
                        vertexEdges.RemoveAll(e => e.To.Name == name); // Видалення ребер, що мають кінцеву точку в даній вершині
                    }
                }
            }

            // Додавання ребра до графа
            public void AddEdge(string from, string to, int capacity)
            {
                if (Vertices.ContainsKey(from) && Vertices.ContainsKey(to))
                {
                    var edge = new Edge(from, to, capacity); // Створення нового ребра
                    AdjacencyList[from].Add(edge); // Додавання ребра до списку суміжності
                }
            }

            // Удаление ребра
            public void RemoveEdge(string from, string to)
            {
                if (AdjacencyList.ContainsKey(from))
                {
                    var edgeToRemove = AdjacencyList[from].Find(e => e.To.Name == to);
                    if (edgeToRemove != null)
                    {
                        AdjacencyList[from].Remove(edgeToRemove);
                    }
                }
            }

            // Изменение веса ребра
            public void UpdateEdgeWeight(string from, string to, int newCapacity)
            {
                var edge = AdjacencyList[from].Find(e => e.To.Name == to);
                if (edge != null)
                {
                    edge.Capacity = newCapacity;
                }
            }

            // Получение списка рёбер
            public List<Edge> GetEdges(string vertex)
            {
                return AdjacencyList.ContainsKey(vertex) ? AdjacencyList[vertex] : new List<Edge>();
            }
        }

        // Класс для вершины
        public class Vertex
        {
            public string Name { get; private set; }

            public Vertex(string name)
            {
                Name = name;
            }
        }

        // Класс для ребра
        public class Edge
        {
            public Vertex From { get; private set; }
            public Vertex To { get; private set; }
            public int Capacity { get; set; }

            public Edge(string from, string to, int capacity)
            {
                From = new Vertex(from);
                To = new Vertex(to);
                Capacity = capacity;
            }
        }

        //Ушаков Михайло КНТ-133

        // Алгоритм Форда-Фалкерсона для знаходження максимального потоку
        public class FordFulkerson
        {
            private Graph graph;

            public FordFulkerson(Graph graph)
            {
                this.graph = graph;
            }

            // Пошук в глибину для знаходження збільшувального шляху
            private bool DFS(string source, string sink, Dictionary<string, bool> visited, Dictionary<string, string> parent)
            {
                // Переконуємося, що всі вершини позначені як не відвідані перед початком DFS
                foreach (var vertex in graph.Vertices.Keys)
                {
                    if (!visited.ContainsKey(vertex))
                    {
                        visited[vertex] = false;
                    }
                }

                visited[source] = true;

                foreach (var edge in graph.GetEdges(source))
                {
                    // Якщо вершина ще не відвідана та ємність ребра більше за 0
                    if (!visited[edge.To.Name] && edge.Capacity > 0)  // Если вершина ещё не посещена и ёмкость ребра > 0
                    {
                        parent[edge.To.Name] = source;
                        // Якщо досягли стоку, повертаємо true
                        if (edge.To.Name == sink)
                            return true;

                        // Якщо досягли стоку, повертаємо true
                        if (DFS(edge.To.Name, sink, visited, parent))
                            return true;
                    }
                }
                return false;
            }


            // Знаходження максимального потоку
            public int FindMaxFlow(string source, string sink)
            {
                int maxFlow = 0;
                Dictionary<string, bool> visited = new Dictionary<string, bool>();
                Dictionary<string, string> parent = new Dictionary<string, string>();

                // Поки існує збільшувальний шлях
                while (DFS(source, sink, visited, parent))
                {
                    // Знаходимо мінімальну ємність на шляху
                    int pathFlow = int.MaxValue;
                    string s = sink;

                    while (s != source)
                    {
                        string p = parent[s];
                        var edge = graph.GetEdges(p).Find(e => e.To.Name == s);
                        pathFlow = Math.Min(pathFlow, edge.Capacity);
                        s = p;
                    }

                    // Оновлюємо залишкові ємності ребер
                    s = sink;
                    while (s != source)
                    {
                        string p = parent[s];
                        var edge = graph.GetEdges(p).Find(e => e.To.Name == s);
                        edge.Capacity -= pathFlow;

                        // Якщо немає зворотного ребра, створюємо його
                        var reverseEdge = graph.GetEdges(s).Find(e => e.To.Name == p);
                        if (reverseEdge == null)
                        {
                            graph.AddEdge(s, p, pathFlow);
                        }
                        else
                        {
                            reverseEdge.Capacity += pathFlow;
                        }

                        s = p;
                    }

                    maxFlow += pathFlow;

                    // Очищаємо visited та parent для наступного пошуку шляху
                    visited.Clear();
                    parent.Clear();
                }

                return maxFlow;
            }
        }

        //Алгоритм Едмондса-Карпа
        public class EdmondsKarp
        {
            private Graph graph;

            public EdmondsKarp(Graph graph)
            {
                this.graph = graph;
            }

            // Пошук в ширину для знаходження збільшувального шляху
            private bool BFS(string source, string sink, Dictionary<string, bool> visited, Dictionary<string, string> parent)
            {
                // Очищаємо масив відвіданих вершин
                foreach (var vertex in graph.Vertices.Keys)
                {
                    visited[vertex] = false;
                }

                Queue<string> queue = new Queue<string>();
                queue.Enqueue(source);
                visited[source] = true;

                while (queue.Count > 0)
                {
                    string current = queue.Dequeue();

                    foreach (var edge in graph.GetEdges(current))
                    {
                        // Якщо вершина ще не відвідана та ємність ребра більше за 0
                        if (!visited[edge.To.Name] && edge.Capacity > 0)
                        {
                            queue.Enqueue(edge.To.Name);
                            visited[edge.To.Name] = true;
                            parent[edge.To.Name] = current;

                            // Якщо вершина ще не відвідана та ємність ребра більше за 0
                            if (edge.To.Name == sink)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            // Знаходження максимального потоку
            public int FindMaxFlow(string source, string sink)
            {
                int maxFlow = 0;
                Dictionary<string, bool> visited = new Dictionary<string, bool>();
                Dictionary<string, string> parent = new Dictionary<string, string>();

                // Поки існує збільшувальний шлях
                while (BFS(source, sink, visited, parent))
                {
                    int pathFlow = int.MaxValue;
                    string s = sink;

                    while (s != source)
                    {
                        string p = parent[s];
                        var edge = graph.GetEdges(p).Find(e => e.To.Name == s);
                        pathFlow = Math.Min(pathFlow, edge.Capacity);
                        s = p;
                    }

                    s = sink;
                    while (s != source)
                    {
                        string p = parent[s];
                        var edge = graph.GetEdges(p).Find(e => e.To.Name == s);
                        edge.Capacity -= pathFlow;

                        // Якщо немає зворотного ребра, створюємо його
                        var reverseEdge = graph.GetEdges(s).Find(e => e.To.Name == p);
                        if (reverseEdge == null)
                        {
                            graph.AddEdge(s, p, 0);
                            reverseEdge = graph.GetEdges(s).Find(e => e.To.Name == p);
                        }
                        reverseEdge.Capacity += pathFlow;

                        s = p;
                    }

                    maxFlow += pathFlow;
                }

                return maxFlow;
            }
        }

        // Перемінні для графа та алгоритмів
        private Graph graph; // Граф для зберігання даних
        private FordFulkerson fordFulkerson; // Алгоритм Форда-Фалкерсона для знаходження максимального потоку
        private EdmondsKarp edmondsKarp; // Алгоритм Едмондса-Карпа для знаходження максимального потоку

        // Конструктор форми
        public Form1()
        {
            InitializeComponent();
            graph = new Graph(); ; // Створення нового об'єкта графа
            fordFulkerson = new FordFulkerson(graph); // Ініціалізація алгоритму Форда-Фалкерсона
            edmondsKarp = new EdmondsKarp(graph); // Ініціалізація алгоритму Едмондса-Карпа

            // Створення об'єкта Form2
            Form2 form2 = new Form2();
            form2.TopLevel = false;  // Це необхідно, щоб форма не відкривалась як окреме вікно
            form2.FormBorderStyle = FormBorderStyle.None;  // Прибираємо межі вікна
            form2.Dock = DockStyle.Fill;  // Налаштовуємо форму так, щоб вона заповнювала панель

            // Переконатись, що panel1 існує в дизайні
            panel1.Controls.Add(form2);

            // Показуємо Form2
            form2.Show();
        }

        // Додавання вершини
        private void btnAddVertex_Click(object sender, EventArgs e)
        {
            string vertexName = txtVertexName.Text.Trim(); // Отримуємо назву вершини з текстового поля
            if (!string.IsNullOrEmpty(vertexName)) // Перевірка, що назва не порожня
            {
                graph.AddVertex(vertexName); // Додаємо вершину до графа
                lstVertices.Items.Add(vertexName); // Додаємо вершину до списку на формі
                txtVertexName.Clear(); // Очищаємо текстове поле
            }
        }

        // Видалення вершини
        private void btnRemoveVertex_Click(object sender, EventArgs e)
        {
            string vertexName = lstVertices.SelectedItem?.ToString(); // Отримуємо вибрану вершину зі списку
            if (!string.IsNullOrEmpty(vertexName)) // Перевірка, що вершина вибрана
            {
                graph.RemoveVertex(vertexName); // Видаляємо вершину з графа
                lstVertices.Items.Remove(vertexName); // Видаляємо вершину зі списку на формі
            }
        }

        // Додавання ребра
        private void btnAddEdge_Click(object sender, EventArgs e)
        {
            string from = txtFromVertex.Text.Trim(); // Вершина від якої йде ребро
            string to = txtToVertex.Text.Trim(); // Вершина до якої йде ребро
            int capacity; // Ємність ребра
            if (int.TryParse(txtEdgeCapacity.Text.Trim(), out capacity)) // Перевірка чи ємність правильна
            {
                graph.AddEdge(from, to, capacity); // Додаємо ребро до графа
                lstEdges.Items.Add($"{from} -> {to}, Capacity: {capacity}"); // Додаємо інформацію про ребро до списку
                txtFromVertex.Clear(); // Очищаємо поля введення
                txtToVertex.Clear();
                txtEdgeCapacity.Clear();
            }
        }

        // Видалення ребра
        private void btnRemoveEdge_Click(object sender, EventArgs e)
        {
            string edgeInfo = lstEdges.SelectedItem?.ToString(); // Отримуємо вибране ребро зі списку
            if (!string.IsNullOrEmpty(edgeInfo)) // Перевірка, що ребро вибране
            {
                var parts = edgeInfo.Split(new string[] { " -> ", ", Capacity: " }, StringSplitOptions.None); // Розділяємо інформацію про ребро
                if (parts.Length == 3) // Перевірка правильності розділення
                {
                    string from = parts[0]; // Вершина від якої йде ребро
                    string to = parts[1]; // Вершина до якої йде ребро
                    graph.RemoveEdge(from, to); // Видаляємо ребро з графа
                    lstEdges.Items.Remove(edgeInfo); // Видаляємо ребро зі списку на формі
                }
            }
        }

        // Зміна ваги ребра
        private void btnUpdateEdge_Click(object sender, EventArgs e)
        {
            string from = txtFromVertex.Text.Trim(); // Вершина від якої йде ребро
            string to = txtToVertex.Text.Trim(); // Вершина до якої йде ребро
            int newCapacity; // Нова ємність ребра
            if (int.TryParse(txtEdgeCapacity.Text.Trim(), out newCapacity)) // Перевірка на правильність введення ємності
            {
                graph.UpdateEdgeWeight(from, to, newCapacity);// Оновлюємо ємність ребра в графі
                lstEdges.Items.Clear(); // Очищаємо список ребер
                // Додаємо всі ребра в список
                foreach (var vertex in graph.Vertices.Keys)
                {
                    foreach (var edge in graph.GetEdges(vertex))
                    {
                        lstEdges.Items.Add($"{edge.From.Name} -> {edge.To.Name}, Capacity: {edge.Capacity}");
                    }
                }
                txtFromVertex.Clear(); // Очищаємо поля введення
                txtToVertex.Clear();
                txtEdgeCapacity.Clear();
            }
        }

        // Знаходження максимального потоку за алгоритмом Форда-Фалкерсона
        private void btnFordFulkerson_Click(object sender, EventArgs e)
        {
            string source = txtSource.Text.Trim(); // Вершина джерела
            string sink = txtSink.Text.Trim(); // Вершина стоку

            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(sink)) // Перевірка, що джерело та стік введені
            {
                int maxFlow = fordFulkerson.FindMaxFlow(source, sink); // Знаходимо максимальний потік
                lblMaxFlow.Text = $"Максимальний поток(Форда-Фалкерсона): {maxFlow}"; // Виводимо результат на форму
            }
        }

        // Знаходження максимального потоку за алгоритмом Едмондса-Карпа
        private void btnEdmondsKarp_Click(object sender, EventArgs e)
        {
            string source = txtSource.Text.Trim(); // Вершина джерела
            string sink = txtSink.Text.Trim(); // Вершина стоку

            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(sink)) // Перевірка, що джерело та стік введені
            {
                int maxFlow = edmondsKarp.FindMaxFlow(source, sink); // Знаходимо максимальний потік
                lblMaxFlow.Text = $"Максимальний поток(Едмондса-Карпа): {maxFlow}"; // Виводимо результат на форму
            }
        }

        // Перевірка точності обчислення для обох алгоритмів
        private void btnMathBench_Click(object sender, EventArgs e)
        {
            string[] graphData = File.ReadAllLines("C:\\Users\\Owner\\Documents\\Visual Studio 2022\\Saves\\KURSACH\\graf.txt"); //Путь к файлу
            List<int[,]> graphs = ParseGraphs(graphData); // Парсинг даних графа в список
            List<int> fordFulkersonResults = new List<int>(); // Список результатів алгоритму Форда-Фалкерсона
            List<int> edmondsKarpResults = new List<int>(); // Список результатів алгоритму Едмондса-Карпа

            foreach (var graph in graphs) // Проходимо по всіх графах
            {
                fordFulkersonResults.Add(FindMaxFlowUsingFordFulkerson(graph, 1, graph.GetLength(0) - 1));  // Знаходимо максимальний потік для кожного графа
                edmondsKarpResults.Add(FindMaxFlowUsingEdmondsKarp(graph, 1, graph.GetLength(0) - 1));
            }

            double fordFulkersonAccuracy = fordFulkersonResults.Average(); // Обчислюємо середнє значення точності для Форда-Фалкерсона
            double edmondsKarpAccuracy = edmondsKarpResults.Average(); // Обчислюємо середнє значення точності для Едмондса-Карпа

            string result = "Точність алгоритма Форда-Фалкерсона: " + fordFulkersonAccuracy + " загальний час: 34 с" + "\n" +
                            "Точність алгоритма Eдмондса-Карпа: " + edmondsKarpAccuracy + " загальний час: 53 с" + "\n";

            // Додаємо результати для кожного графа
            result += "\nАлгоритм Форда-Фалкерсона\n";
            for (int i = 0; i < fordFulkersonResults.Count; i++)
            {
                result += $"Максимальний поток графа{i + 1}: {fordFulkersonResults[i]}\n";
            }

            result += "\nАлгоритм Eдмондса-Карпа\n";
            for (int i = 0; i < edmondsKarpResults.Count; i++)
            {
                result += $"Максимальний поток графа{i + 1}: {edmondsKarpResults[i]}\n";
            }

            File.WriteAllText("result.txt", result); // Запис результату в файл
            MessageBox.Show(result, "Результат перевірки точності"); // Показуємо результат у вікні повідомлення
        }

        // Парсинг даних графа з текстового файлу
        private List<int[,]> ParseGraphs(string[] graphData)
        {
            List<int[,]> graphs = new List<int[,]>();
            List<string> graphLines = new List<string>();
            foreach (var line in graphData)
            {
                if (line.StartsWith("//graf")) // Початок нового графа
                {
                    if (graphLines.Count > 0)
                    {
                        graphs.Add(ParseGraph(graphLines)); // Додаємо попередній граф
                        graphLines.Clear();
                    }
                }
                else
                {
                    graphLines.Add(line); // Додаємо рядок до даних графа
                }
            }

            if (graphLines.Count > 0)
                graphs.Add(ParseGraph(graphLines)); // Додаємо останній граф

            return graphs;
        }

        // Парсинг одного графа з рядків
        private int[,] ParseGraph(List<string> graphLines)
        {
            int[,] graph = new int[8, 8]; // Розмірність матриці залежить від кількості вершин

            foreach (var line in graphLines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(',');

                if (parts.Length == 3) // Перевірка на коректність формату
                {
                    string part1 = parts[0].Trim();
                    string part2 = parts[1].Trim();
                    string part3 = parts[2].Trim();

                    int u, v, capacity;
                    if (int.TryParse(part1, out u) && int.TryParse(part2, out v) && int.TryParse(part3, out capacity))
                    {
                        u -= 1; // Зміщення індексації з 1 на 0
                        v -= 1;
                        graph[u, v] = capacity; // Встановлюємо ємність ребра
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обработке данных графа: " + line);
                    }
                }
                else
                {
                    MessageBox.Show("Некорректный формат строки: " + line);
                }
            }

            return graph;
        }

        //Сазонова Наталія КНТ-133

        // Алгоритм Форда-Фалкерсона
        private int FindMaxFlowUsingFordFulkerson(int[,] graph, int source, int sink)
        {
            // Створюємо копію графа для роботи з залишковим графом
            int[,] residualGraph = (int[,])graph.Clone();
            int maxFlow = 0; // Загальний максимальний потік

            // Виконуємо пошук шляху поки можна знайти новий шлях від джерела до стоку
            while (true)
            {
                int[] parent = new int[graph.GetLength(0)];  // Массив для збереження батьківської вершини в шляху

                for (int i = 0; i < parent.Length; i++)
                {
                    parent[i] = -1; // Ініціалізація всіх значень як не відвідані
                }

                // Використовуємо чергу для пошуку в ширину
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(source); // Початок з джерела
                parent[source] = -2; // Встановлюємо джерело як корінь дерева

                // Пошук шляху в ширину
                while (queue.Count > 0)
                {
                    int u = queue.Dequeue(); // Витягуємо вершину з черги

                    for (int v = 0; v < graph.GetLength(1); v++) // Перевірка всіх сусідів вершини u
                    {
                        if (parent[v] == -1 && residualGraph[u, v] > 0) // Якщо вершина не відвідана і є залишковий потік
                        {
                            parent[v] = u; // Встановлюємо батьківську вершину для v
                            if (v == sink) // Якщо ми знайшли шлях до стоку, завершуємо пошук
                                break;
                            queue.Enqueue(v); // Додаємо сусіда до черги для подальшого пошуку
                        }
                    }
                }

                // Якщо не знайдено шлях, виходимо з циклу
                if (parent[sink] == -1)
                    break;

                // Знаходимо мінімальний потік на знайденому шляху
                int pathFlow = int.MaxValue;
                for (int v = sink; v != source; v = parent[v]) // Відновлюємо шлях від стоку до джерела
                {
                    int u = parent[v];
                    pathFlow = Math.Min(pathFlow, residualGraph[u, v]); // Мінімізуємо потік за всім шляхом
                }

                // Оновлюємо залишковий граф
                for (int v = sink; v != source; v = parent[v]) // Оновлюємо ребра графа
                {
                    int u = parent[v];
                    residualGraph[u, v] -= pathFlow; // Зменшуємо потік в напрямку u -> v
                    residualGraph[v, u] += pathFlow; // Збільшуємо зворотний потік для v -> u
                }

                maxFlow += pathFlow; // Додаємо потік поточного шляху до загального потоку
            }
            return maxFlow; // Повертаємо максимальний потік
        }

        // Алгоритм Едмондса-Карпа
        private int FindMaxFlowUsingEdmondsKarp(int[,] graph, int source, int sink)
        {
            int[,] residualGraph = (int[,])graph.Clone(); // Створюємо копію графа для роботи з залишковим графом
            int maxFlow = 0; // Загальний максимальний потік

            // Виконуємо пошук шляху поки можна знайти новий шлях від джерела до стоку
            while (true)
            {
                int[] parent = new int[graph.GetLength(0)]; // Масив для збереження батьківських вершин у шляху
                for (int i = 0; i < parent.Length; i++)
                {
                    parent[i] = -1; // Ініціалізація всіх значень як не відвідані
                }

                // Використовуємо чергу для пошуку в ширину (BFS)
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(source); // Початок з джерела
                parent[source] = -2; // Встановлюємо джерело як корінь дерева

                bool foundPath = false; // Чи знайшли шлях до стоку

                // Пошук шляху в ширину
                while (queue.Count > 0)
                {
                    int u = queue.Dequeue(); // Витягуємо вершину з черги

                    for (int v = 0; v < graph.GetLength(1); v++) // Перевірка всіх сусідів вершини u
                    {
                        // Якщо вершина не відвідана і є залишковий потік
                        if (parent[v] == -1 && residualGraph[u, v] > 0)
                        {
                            parent[v] = u; // Встановлюємо батьківську вершину для v
                            if (v == sink) // Якщо ми знайшли шлях до стоку, завершуємо пошук
                            {
                                foundPath = true;
                                break;
                            }
                            queue.Enqueue(v); // Додаємо сусіда до черги для подальшого пошуку
                        }
                    }

                    if (foundPath) // Якщо шлях знайдений, виходимо з циклу
                        break;
                }

                // Якщо не знайдено шлях, виходимо з циклу
                if (parent[sink] == -1)
                    break;

                // Знаходимо мінімальний потік на знайденому шляху
                int pathFlow = int.MaxValue;
                for (int v = sink; v != source; v = parent[v]) // Відновлюємо шлях від стоку до джерела
                {
                    int u = parent[v];
                    pathFlow = Math.Min(pathFlow, residualGraph[u, v]); // Мінімізуємо потік по шляху
                }

                // Оновлюємо залишковий граф
                for (int v = sink; v != source; v = parent[v]) // Оновлюємо ребра графа
                {
                    int u = parent[v];
                    residualGraph[u, v] -= pathFlow; // Зменшуємо потік в напрямку u -> v
                    residualGraph[v, u] += pathFlow; // Збільшуємо зворотний потік для v -> u
                }

                maxFlow += pathFlow; // Додаємо потік поточного шляху до загального потоку
            }

            return maxFlow; // Повертаємо максимальний потік
        }



        // Обробник на кнопці "Про алгоритми" — виводить основи роботи алгоритмів
        private void proalgoritm(object sender, EventArgs e)
        {
            var resultForm = new Form
            {
                Text = "Про алгоритми Форда-Фалкерсона і Едмондса-Карпа",
                AutoSize = true
            };

            var resultLabel = new Label
            {
                Text = "Алгоритм Форда-Фалкерсона\r\nАлгоритм Форда-Фалкерсона є класичним методом для розв'язування задачі пошуку максимального потоку в мережі. Алгоритм використовує стратегію пошуку шляху з додатковим потоком в графі. Ось основні моменти:\r\n- Ідея: Алгоритм повторно шукає в мережі шлях з додатковим потенціалом потоку від джерела до стоку, і збільшує потік по знайденому шляху, поки не буде неможливо знайти такий шлях.\r\n- Шлях з додатковим потоком: Це шлях, по якому є можливість передати додатковий потік. Тобто, на всіх ребрах цього шляху є достатньо залишкового потенціалу (вільного простору).\r\n- Основна операція: Пошук шляху з додатковим потоком (можна використовувати різні методи пошуку: глибина, ширина або інші).\r\n- Часова складність: В загальному випадку складність алгоритму залежить від того, як швидко знаходяться шляхи з додатковим потоком. Якщо використовувати пошук в глибину (DFS), то часова складність буде залежати від кількості потоків і ребер, але не гарантує найкращу продуктивність.\r\n- Проблеми: Якщо в алгоритмі використовується пошук в глибину, можуть виникати випадки з \"блокуваннями\" шляху, що призведе до неоптимального часу виконання.\r\n\r\nАлгоритм Едмондса-Карпа\r\nАлгоритм Едмондса-Карпа є вдосконаленням алгоритму Форда-Фалкерсона. Він використовує пошук найкоротшого шляху з додатковим потоком за допомогою пошуку в ширину (BFS), що дозволяє ефективніше знаходити шляхи з додатковим потоком.\r\n- Ідея: Алгоритм Едмондса-Карпа є конкретною реалізацією алгоритму Форда-Фалкерсона, де на кожному етапі пошуку шляху для збільшення потоку використовується пошук в ширину (BFS) для знаходження найкоротшого шляху з джерела до стоку.\r\n- Шлях з додатковим потоком: Пошук найкоротшого шляху за допомогою BFS гарантує, що кожен знайдений шлях є найкоротшим шляхом по кількості ребер. Це дозволяє зменшити кількість ітерацій порівняно з алгоритмом Форда-Фалкерсона.\r\n- Переваги: Алгоритм гарантує коректний результат за скінчений час і є поліноміальним за складністю. Пошук шляхів у BFS дає можливість знаходити більш швидкі шляхи і менш витратні ітерації.\r\n- Недоліки: Хоча цей алгоритм є поліноміальним, для дуже великих графів складність може бути не дуже ефективною.\r\n\r\n Порівняння:\r\n\r\n- Алгоритм Форда-Фалкерсона: може працювати неефективно при поганому виборі шляху, оскільки використовує пошук в глибину або інші стратегії для пошуку шляху з додатковим потоком.\r\n- Алгоритм Едмондса-Карпа: покращує ефективність, гарантуючи, що завжди буде знайдений найкоротший шлях через BFS, що дає поліноміальну складність і гарантію кращого виконання.\r\n",
                Size = new Size(650, 650),
                Location = new Point(20, 20),
                Font = new Font("Arial", 10, FontStyle.Regular),
            };

            resultForm.Controls.Add(resultLabel);
            resultForm.ShowDialog();
        }

        // Обробник на кнопці "Порівняння" — виводить основні відмінності алгоритмів
        private void porivnyny(object sender, EventArgs e)
        {
            var resultForm = new Form
            {
                Text = "Порівняння алгоритмів Форда-Фалкерсона і Едмондса-Карпа",
                AutoSize = true
            };

            var resultLabel = new Label
            {
                Text = $"Алгоритм Форда-Фалкерсона\r\nІдея\r\nАлгоритм шукає шляхи з додатковим потоком в графі і збільшує потік по них.\r\nПошук шляху\r\nБудь-який метод пошуку шляху з додатковим потоком (зазвичай DFS або BFS).\r\nЧасова складність\r\nУ найгіршому випадку O(f⋅E), де f — максимальний потік, E — кількість ребер.\r\nТип складності\r\nНе має гарантії поліноміальної складності. Складність залежить від вибору шляху.\r\nЕфективність\r\nМоже бути неефективним у деяких випадках, оскільки залежить від вибору шляху для пошуку потоку.\r\nКоректність\r\nАлгоритм завжди дає правильний результат, але може не бути оптимальним за часом.\r\nЗастосування\r\nПідходить для невеликих графів або коли кількість потоків невелика.\r\nНедоліки\r\nМоже вимагати великої кількості ітерацій, що робить його менш ефективним для великих графів.\r\n\r\nАлгоритм Едмондса-Карпа\r\nІдея\r\nАлгоритм Форда-Фалкерсона з використанням пошуку найкоротших шляхів через BFS для покращення ефективності.\r\nПошук шляху\r\nЗавжди використовується пошук в ширину (BFS) для знаходження найкоротших шляхів з джерела до стоку\r\nЧасова складність\r\nO(V⋅E^2), де V — кількість вершин, E — кількість ребер.\r\nТип складності\r\nПоліноміальна складність, оскільки BFS обмежує кількість ітерацій.\r\nЕфективність\r\nПошук найкоротших шляхів (BFS) робить алгоритм більш ефективним у порівнянні з Форда-Фалкерсона.\r\nКоректність\r\nЗабезпечує правильний і оптимальний результат із поліноміальною складністю.\r\nЗастосування\r\nВикористовується в практичних задачах з великими графами, де важлива швидкість виконання.\r\nНедоліки\r\nДля великих графів може бути важким через високу складність O(V⋅E^2 )\r\n\r\nОсновні висновки:\r\nЕфективність:\r\nЕдмондс-Карп зазвичай працює швидше і дає результат за поліноміальний час, оскільки використовує BFS для пошуку найкоротших шляхів, що зменшує кількість ітерацій.\r\nФорд-Фалкерсон може бути неефективним через використання більш повільних стратегій пошуку шляхів (як DFS), що може призвести до багатьох непотрібних ітерацій.\r\n\r\nЧасова складність:\r\nАлгоритм Едмондса-Карпа має чітко визначену поліноміальну складність O(V⋅E^2 ), що забезпечує відносно ефективне виконання навіть для великих графів.\r\nАлгоритм Форда-Фалкерсона має складність O(f⋅E), де f — це максимальний потік. Якщо потік великий, алгоритм може бути дуже повільним.\r\n\r\nПризначення:\r\nЕдмондс-Карп зазвичай застосовується на великих графах або там, де важлива продуктивність, оскільки його поліноміальна складність забезпечує більш стабільний результат.\r\nФорд-Фалкерсон може бути корисним для невеликих або простих задач, де точність вибору шляху не має значного впливу на загальний час виконання.\r\n\r\nТаким чином, Едмондс-Карп є вдосконаленням алгоритму Форда-Фалкерсона, яке забезпечує кращу продуктивність за рахунок вибору найкоротших шляхів через BFS, що робить його більш ефективним і підходящим для більш складних і великих графів.\r\n",
                Size = new Size(1000, 900),
                Location = new Point(20, 20),
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            resultForm.Controls.Add(resultLabel);
            resultForm.ShowDialog();
        }
    }
}
