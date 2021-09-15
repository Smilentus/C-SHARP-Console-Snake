using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {
        static Random rnd = new Random();

        enum directions
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        // Текущее направление движения. По умолчанию вправо
        static directions curDirection;
        static directions newDirection;

        // Длина змеи
        static int snakeLength = 0;
        static int snakeSpeed = 100;
        static int eatedStars = 0;
        // Текущее положение ЗМЯИ (головы)
        static int x, y;
        // ОЛДЫ на месте
        static int ox, oy, obx, oby;
        // Позиции туловища ЗМЯИ
        static List<int> xSnake = new List<int>();
        static List<int> ySnake = new List<int>();
        // Позиции заспавненной звёздочки
        static int starX, starY;

        // Ограничения уровня
        static int xLimit, yLimit;

        // РИСУНАЧКИ
        const string snakeImage = "#";
        const string starImage = "*";

        // ТОЧКА ВХОДА
        static void Main(string[] args)
        {
            // Изменение названия консольного приложения
            Console.Title = "Игра про маленькую змейку!";

            Console.SetWindowSize(50, 25);

            // Таймер отрисовки змеёныша
            System.Timers.Timer drawTimer = new System.Timers.Timer();
            drawTimer.Elapsed += new System.Timers.ElapsedEventHandler(Draw);
            drawTimer.Interval = snakeSpeed;
            drawTimer.Enabled = true;

            // Начало игры
            StartGame();
        }

        // Спавним звезду
        public static void SpawnStar()
        {
            // Выбираем новые позиции для ЗВЯЗДЫ
            starX = rnd.Next(1, xLimit - 1);
            starY = rnd.Next(1, yLimit + 1);
            Console.SetCursorPosition(starX, starY);
            Console.Write(starImage);
        }

        // УНИЧТОЖАЕМ (кушаем) ЗВЁЗДОЧКУ
        public static void AssimilateStar()
        {
            // Добавляем новую часть
            if (snakeLength == 0)
            {
                xSnake.Add(x);
                ySnake.Add(y);
            }
            else
            {
                xSnake.Add(xSnake[xSnake.Count - 1]);
                ySnake.Add(ySnake[ySnake.Count - 1]);
            }

            // Удлиняем наш член
            snakeLength++;
            eatedStars++;

            DrawSnakeInfo();

            // Спавним волшебную звезду
            SpawnStar();
        }

        // Рисуем весь уровень
        public static void Draw(object sender, System.Timers.ElapsedEventArgs e)
        {
            curDirection = newDirection;

            // Если наехали на звезду, ПОЖИРАЕМ ЕЁ
            if (starX == x && starY == y)
            {
                AssimilateStar();
            }

            // Границы уровня
            if (x < 0)
                x = xLimit;
            if (x > xLimit)
                x = 0;
            if (y > yLimit)
                y = 0;
            if (y < 0)
                y = yLimit;

            // ЭТО ТЕЛО ЗМЯЙКИ (тело)
            if (snakeLength > 0)
            {
                MoveSnakeBody();

                // Рисуем каждую часть
                for (int i = 0; i < snakeLength; i++)
                {
                    Console.SetCursorPosition(xSnake[i], ySnake[i]);
                    Console.Write(snakeImage);
                }
            }

            // Перерисовываем ЗМЕЯКУ
            OptimizeSnakeDrawing();

            DrawLimit();

            // ЭТО ЗМЕЙКА (голова)
            Console.SetCursorPosition(x, y);
            Console.Write(snakeImage);
            CalculateNewPos();

            Console.SetCursorPosition(starX, starY);
            Console.Write(starImage);

            // УМИРАЕМ, ЕСЛИ НАТКНУЛИСЬ НА СЕБЯ
            CheckCollision();
        }

        public static void OptimizeSnakeDrawing()
        {
            if(snakeLength > 0)
            {
                Console.SetCursorPosition(obx, oby);
                Console.Write(" ");
            }
            else
            {
                Console.SetCursorPosition(ox, oy);
                Console.Write(" ");
            }
        }

        // Запуск игры и перезапуск
        public static void StartGame()
        {
            Console.Clear();

            // Начальное направление движение
            curDirection = directions.RIGHT;

            // Позиции начала головы
            x = 10;
            y = 10;

            // Длина змеи (изначально 0(только голова))
            snakeLength = 0;
            eatedStars = 0;

            // Лимит уровня
            xLimit = 20;
            yLimit = 20;

            // Спавним первую звезду
            SpawnStar();

            // ЭТО ГРАНИЦЫ УРОВНЯ
            DrawLimit();

            DrawSnakeInfo();

            // Двигаемся
            Move();
        }

        // Проверка столкновения с самим собой
        public static void CheckCollision()
        {
            for (int i = 0; i < snakeLength; i++)
            {
                if (x == xSnake[i] && y == ySnake[i])
                {
                    StartGame();
                    break;
                }
            }
        }

        // Рисуем информацию о змейке
        public static void DrawSnakeInfo()
        {
            Console.SetCursorPosition(30, 5);
            Console.Write("Информация о змейке: ");
            Console.SetCursorPosition(30, 6);
            Console.Write("Длина: " + (snakeLength + 1));
            Console.SetCursorPosition(30, 7);
            Console.Write("Съедено яблок: " + eatedStars);

        }

        // Рисуем границы уровня
        public static void DrawLimit()
        {
            // Горизонтальная стена
            for (int _x = 0; _x < xLimit + 1; _x++)
            {
                Console.SetCursorPosition(_x, yLimit + 1);
                Console.Write("═");
            }

            // Вертикальная стена
            for (int _y = 0; _y < yLimit + 1; _y++)
            {
                Console.SetCursorPosition(xLimit + 1, _y);
                Console.Write("║");
            }

            Console.SetCursorPosition(xLimit + 1, yLimit + 1);
            Console.Write("╝");
        }

        // Расчёт новой позиции змейки
        public static void CalculateNewPos()
        {
            ox = x;
            oy = y;

            curDirection = newDirection;

            // Сдвигаем змейку
            switch (curDirection)
            {
                case directions.UP:
                    y--;
                    break;
                case directions.DOWN:
                    y++;
                    break;
                case directions.RIGHT:
                    x++;
                    break;
                case directions.LEFT:
                    x--;
                    break;
            }
        }

        // MOVE YOUR BODY EVERYBODY
        public static void MoveSnakeBody()
        {
            if (snakeLength > 0)
            {
                obx = xSnake[snakeLength - 1];
                oby = ySnake[snakeLength - 1];

                for (int i = snakeLength - 1; i > 0; i--)
                {
                    xSnake[i] = xSnake[i - 1];
                    ySnake[i] = ySnake[i - 1];
                }

                xSnake[0] = ox;
                ySnake[0] = oy;
            }
        }

        // Задаём НАПРАВЛЕНИЕ 
        public static void Move()
        {
            while (true)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey(true);

                // Переводим в верхний регистр во избежание багов
                string lastKey = pressedKey.KeyChar.ToString().ToUpper();

                if (lastKey == "W")
                {
                    newDirection = directions.UP;
                }
                if (lastKey == "S")
                {
                    newDirection = directions.DOWN;
                }
                if (lastKey == "A")
                {
                    newDirection = directions.LEFT;
                }
                if (lastKey == "D")
                {
                    newDirection = directions.RIGHT;
                }
                if (lastKey == "Ц")
                {
                    newDirection = directions.UP;
                }
                if (lastKey == "Ы")
                {
                    newDirection = directions.DOWN;
                }
                if (lastKey == "Ф")
                {
                    newDirection = directions.LEFT;
                }
                if (lastKey == "В")
                {
                    newDirection = directions.RIGHT;
                }

            }
        }
    }
}
