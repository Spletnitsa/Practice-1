namespace Practice_1
{
    public class Cargo //класс для разбивки CSV файла
    {
        public int[] X = new int[10];
        public int D;

        public void piece(string line)
        {
            string[] parts = line.Split(';');
            int lastIndexOfCSV = parts.Length - 1;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                X[i] = int.Parse(parts[i]);
            }
            D = int.Parse(parts[lastIndexOfCSV]);
        }



        public static List<Cargo> ReadFile(string filename)
        {
            List<Cargo> res = new List<Cargo>();
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Cargo p = new Cargo();
                    p.piece(line);
                    res.Add(p);
                }
            }

            return res;
        }
    }

    class neuron//класс реализующий классический персептрон с методом обучения Хэбба
    {
        private int[] X = new int[10];//входы
        private const int X0 = -1;//вход смещения
        private int[] W = new int[10];//веса
        private int T = 0;//вес смещения
        private int NetRes = 0;//Net сигнал, применяется для хранения суммы взвешенных входов
        private int output;//выход

        public neuron()
        {
            //задание нулевых начальных значений для весов
            for (int i = 0; i < W.Length; i++)
            {
                W[i] = 0;
            }
        }
        
        //расчет суммы взвешенных входов 
        public void Net()
        {
            NetRes = 0;

            for (int i = 0; i < X.Length; i++)
            {
                NetRes += W[i] * X[i];
            }

            NetRes += T * X0;
        }

        //Строгая ступенчатая функция активации
        public int ActivateFunction()
        {
            if (NetRes > 0)
                return 1;
            else if (NetRes <= 0)
                return -1;
            else
                return 0;
        }

        //метод обучения персептоона по правилу Хэбба
        public void LearningNeuron(int[][] X, int[] D)
        {
            int runs = 0;
            int matrixLength = X.Length;
            bool exit = true;

            Console.WriteLine("Обучение нейрона:");
            do
            {
                runs++;
                exit = true;
                Console.WriteLine($"Прогон {runs}");
                for (int i = 0; i < matrixLength; i++)
                {
                    this.X = X[i];

                    Net();
                    output = ActivateFunction();

                    Console.WriteLine("Итерация: " + (i + 1));

                    if (output != D[i])
                    {
                        for (int j = 0; j < this.X.Length; j++)
                        {
                            W[j] += this.X[j] * D[i];
                            Console.WriteLine($"Изменение весов:\n W{j + 1} = {W[j]}");
                        }
                        T -= D[i];
                        Console.WriteLine($" T = {T}");
                        exit = false;
                    }
                    else
                        Console.WriteLine("Выход совпал с эталонным значением");

                    Console.WriteLine($"Выход = {output}\nЭталонное значение = {D[i]}\n");
                }
            } while (!exit);

            Console.WriteLine("Обучение завершено!\n");
        }

        //метод решения задач, в качестве аргумента поступает список со значениями входных и эталлоных значений
        public void SolutionOfProblem(List<Cargo> CSV_Struct)
        {
            int matrixLength = CSV_Struct.Count;//колическтво строк в матрице
            float oneIterationInProcent = 100f / matrixLength;//Сколько процентов в одной строке матрицы
            int correctOutputs = 0;//количество парвильных выходов
            
            //вывод значения весов уже обученного персептрона
            for (int i = 0; i < W.Length; i++)
            {
                Console.WriteLine($"W{i} = {W[i]}");
            }

            Console.WriteLine("Решение реальных задач:");

            //цикл прохода по всей марице входных и эталонных значений 
            for (int i = 0; i < matrixLength; i++)
            {
                //присваивание вектору X входных значений строки матрицы
                for (int j = 0; j < CSV_Struct[i].X.Length; j++)
                {
                    this.X[j] = CSV_Struct[i].X[j];
                }
                Net();//расчет net сигнала
                output = ActivateFunction();//присваивание выхода активационной функции вызоду персептрона
                if (output == CSV_Struct[i].D)//сравнение выхода персептрона с эталонным значением
                {
                    correctOutputs++;//увеличение количества корректных выходов
                }
            }

            float correctOutputsInProcent = oneIterationInProcent * correctOutputs;//расчет процента правильных значений
            Console.WriteLine($"Правильных ответов: {correctOutputsInProcent}%");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            //Матрица значений для обучения
            int[][] iterations = new int[5][];
            iterations[0] = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            iterations[1] = new int[10] { -1, 1, -1, 1, -1, 1, -1, 1, -1, 1 };
            iterations[2] = new int[10] { 1, -1, -1, 1, -1, -1, 1, -1, -1, 1 };
            iterations[3] = new int[10] { -1, 1, 1, -1, -1, 1, 1, 1, 1, -1 };
            iterations[4] = new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            int[] D = new int[] { -1, -1, -1, -1, 1 };//эталонные значения для матрицы значений для обучения
            neuron firstNeuron = new neuron();//создание нейрона

            firstNeuron.LearningNeuron(iterations, D);//обучение нейрона

            //считывание CSV файла
            List<Cargo> CSV_Struct = new List<Cargo>();
            CSV_Struct = Cargo.ReadFile("D:\\university\\7 semestr\\" +
                "Design of neuroprocessors\\Practice 1\\Table.csv");

            //вызов метода решения задач
            firstNeuron.SolutionOfProblem(CSV_Struct);
        }
    }
}