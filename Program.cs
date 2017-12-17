using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab4 {

    class Program {
        static void Main(string[] args) {
            WhatsNextWindow whatsNextWindow = new WhatsNextWindow(15, 1, 6, 6);
            BuildingWindow buildingWindow = new BuildingWindow(1, 1, 10, 10);
            Overseer watcher = new Overseer();
            watcher.addWindow(whatsNextWindow);
            watcher.addWindow(buildingWindow);
            watcher.render();
        }
    }

    class Tetris {
        public static char FIGURE = '0';
        public static char EMPTY_CELL = '\0';
        public static int score = 0;
        internal static char BORDER = '*';
    }

    class Overseer {
        private MainMatrix mainMatrix = new MainMatrix();
        private List<SubWindow> windows = new List<SubWindow>();

        public void addWindow(SubWindow window) {
            windows.Add(window);
        }

        public void render() {
            while (true) {
                System.Threading.Thread.Sleep(1000);

                mainMatrix.clear();
                Console.Clear();
                windows.ForEach(window => window.draw());

                char[,] matrixCopy = mainMatrix.getMatrix();

                for (int i = 0; i <= matrixCopy.GetUpperBound(0); i++)
                    for (int j = 0; j <= matrixCopy.GetUpperBound(1); j++) {
                        Console.Write(matrixCopy[i, j]);

                        if (j == matrixCopy.GetUpperBound(1))
                            Console.WriteLine();
                    }

            }
        }
    }

    //namespace element {

    class ElementSource {
        private static Random random = new Random();
        private static Element nextElem = null;

        public Element whatsNext() {
            if (nextElem == null) {
                return getNext();
            } else
                return nextElem;
        }

        public Element getNext() {
            int next = random.Next(1);
            switch (next) {
                case 0: {
                        Element oldElem = nextElem;
                        nextElem = new ElementType1();
                        return oldElem;
                    };

                /*case 1: {
                        Element oldElem = nextElem;
                        nextElem = new ElementType2();
                        return oldElem;
                    }

                case 2: {
                        Element oldElem = nextElem;
                        nextElem = new ElementType3();
                        return oldElem;
                    }*/

                /*case 3: {
                        Element oldElem = nextElem;
                        nextElem = new ElementType4();
                        return oldElem;
                    }*/
                default:
                    return new ElementType1();
            }

        }
    }

    abstract class Element {

        private char[,] figure;

        public Element(char[,] figure) {
            this.figure = figure;
        }

        public char[,] getFigure() {
            return figure;
        }

        public void rotate() {
            char[] vector = new char[(figure.GetUpperBound(0) + 1) * (figure.GetUpperBound(1) + 1) + 1];
            int vectorIndex = 0;

            for (int j = 0; j <= figure.GetUpperBound(1); j++)
                for (int i = figure.GetUpperBound(0); i >= 0; i--) { 
                    vector[vectorIndex] = figure[i, j];
                    vectorIndex += 1;
                }

            vectorIndex = 0;
            for (int i = 0; i <= figure.GetUpperBound(0); i++)
                for (int j = 0; j <= figure.GetUpperBound(1); j++) {
                    figure[i, j] = vector[vectorIndex];
                    vectorIndex += 1;
                }
        }
    }

    sealed class ElementType1 : Element {
        public ElementType1()
        : base(new char[,] { { '0', '\0', '\0' }, { '0', '0', '0' }, { '\0', '\0', '\0' } }) {
        }
    }

    sealed class ElementType2 : Element {
        public ElementType2()
        : base(new char[,] { { Tetris.EMPTY_CELL, Tetris.EMPTY_CELL, Tetris.EMPTY_CELL}, 
            { Tetris.FIGURE, Tetris.FIGURE, Tetris.FIGURE},
            { Tetris.EMPTY_CELL, Tetris.EMPTY_CELL, Tetris.EMPTY_CELL},
            }) {
        }
    }

    sealed class ElementType3 : Element {
        public ElementType3()
        : base(new char[,] { { Tetris.EMPTY_CELL, Tetris.FIGURE, Tetris.EMPTY_CELL },
            { Tetris.FIGURE, Tetris.FIGURE, Tetris.FIGURE},
            { Tetris.EMPTY_CELL, Tetris.EMPTY_CELL, Tetris.EMPTY_CELL } }) {
        }
    }

    //rotation bugged
    /*sealed class ElementType4 : Element {
        public ElementType4()
        : base(new char[,] { { Tetris.FIGURE, Tetris.EMPTY_CELL},
            { Tetris.FIGURE, Tetris.FIGURE},
            { Tetris.EMPTY_CELL, Tetris.FIGURE } }) {
        }
    }*/

    //}


    //namespace window {

    abstract class SubWindow {
        protected static ElementSource elementSource = new ElementSource();
        protected static MainMatrix mainMatrix = new MainMatrix();

        protected int x, y, width, height;

        public SubWindow(int x, int y, int width, int height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }


        protected void drawBorder() {
            for (int i = 0; i <= width; i++)
                mainMatrix.setElement(y - 1, x + i, Tetris.BORDER);

            for (int i = 0; i <= width; i++)
                mainMatrix.setElement(y + width, x + i, Tetris.BORDER);

            for (int i = 0; i <= height + 1; i++)
                mainMatrix.setElement(y + i -1, x + width, Tetris.BORDER);

            for (int i = 0; i <= height + 1; i++)
                mainMatrix.setElement(y + i -1, x - 1, Tetris.BORDER);
        }

        abstract public void draw();

        protected void paintElement(char[,] element, int x, int y) {

            for (int i = 0; i <= element.GetUpperBound(0); i++)
                for (int j = 0; j <= element.GetUpperBound(1); j++)
                    if (element[i, j] != Tetris.EMPTY_CELL)
                        mainMatrix.setElement(i + y, j + x, element[i, j]);
        }
    }

    class WhatsNextWindow : SubWindow {

        public WhatsNextWindow(int x, int y, int width, int height) : base(x, y, width, height) { }

        public override void draw() {
            drawBorder();

            char[] nextElemString = "Next".ToCharArray();
            for (int i = 0; i < nextElemString.Length; i++)
                mainMatrix.setElement(y, i + x, nextElemString[i]);

            if (elementSource.whatsNext() == null)
                elementSource.getNext();
            char[,] nextElement = elementSource.whatsNext().getFigure();
            this.paintElement(nextElement, x, y + 2);

            char[] scoreString = ("" + Tetris.score).ToCharArray();
            for (int i = 0; i < scoreString.Length; i++)
                mainMatrix.setElement(y + 5, i + x, scoreString[i]);
        }
    }

    class BuildingWindow : SubWindow {
        private char[,] filledMatrix;
        private Element currentElement;
        private int elemX, elemY;
        private bool isEndGame = false;

        Task keyListener;

        public BuildingWindow(int x, int y, int width, int height) : base(x, y, width, height) {
            filledMatrix = new char[height, width];
            elemX = width / 2;
            elemY = 0;

            initTasks();
        }

        private void initTasks() {
            keyListener = Task.Factory.StartNew(() => {
                while (true) { 
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'a') {
                        if (elemX > 0)
                            elemX -= 1;
                    } else if (key.KeyChar == 'd') {
                        if (elemX + 3 < width)
                            elemX += 1;
                    } else if (key.KeyChar == 'r') {
                        currentElement.rotate();
                    }
                }
            });
        }

        public override void draw() {
            drawBorder();

            if (!isEndGame) {
                if (currentElement == null)
                    currentElement = elementSource.getNext();

                if (checkIfCanGoLower()) {
                    elemY += 1;
                } else {
                    if (checkEndGame())
                        isEndGame = true;

                    stopDescent();
                    verifyLines();
                }

                

                    drawFilledMatrix();
                drawCurrentElement();
            } else {
                char[] endGameText = ("Game End, Score: " + Tetris.score).ToCharArray();
                for (int i = 0; i < endGameText.Length; i++)
                    mainMatrix.setElement(height / 2, i + 2, endGameText[i]);
            }
        }


        private bool checkEndGame() {
            return elemY <= 0;
        }

        private void drawFilledMatrix() {
            for (int i = 0; i <= filledMatrix.GetUpperBound(0); i++)
                for (int j = 0; j <= filledMatrix.GetUpperBound(1); j++)
                    mainMatrix.setElement(i + y, j + x, filledMatrix[i, j]);
        }

        private void drawCurrentElement() {
            if (currentElement != null)
                paintElement(currentElement.getFigure(), x + elemX, y + elemY);
        }

        private void stopDescent() {
            char[,] figure = currentElement.getFigure();

            for (int i = 0; i <= figure.GetUpperBound(0); i++)
                for (int j = 0; j <= figure.GetUpperBound(1); j++) {
                    if (figure[i, j] != Tetris.EMPTY_CELL)
                        filledMatrix[i + elemY, j + elemX] = Tetris.FIGURE;
                }


            currentElement = elementSource.getNext();
            elemX = width / 2;
            elemY = -1;
        }

        private void verifyLines() {
            for (int i = 0; i <= filledMatrix.GetUpperBound(0); i++) {
                int lineSum = 0;

                for (int j = 0; j <= filledMatrix.GetUpperBound(1); j++)
                    if (filledMatrix[i, j] == Tetris.EMPTY_CELL)
                        break;
                    else lineSum++;

                if (lineSum == filledMatrix.GetUpperBound(1) + 1) {
                    deleteLine(i);
                    i = 0;
                }
            }
        }

        private void deleteLine(int line) {
            for (int i = line; i > 0; i--)
                for (int j = 0; j <= filledMatrix.GetUpperBound(1); j++)
                    filledMatrix[i, j] = filledMatrix[i - 1, j];
            Tetris.score += 1;
        }

        private bool checkIfCanGoLower() {
            char[,] figure = currentElement.getFigure();

            for (int j = 0; j <= figure.GetUpperBound(1); j++)
                for (int i = 0; i <= figure.GetUpperBound(0); i++)
                    if (elemY + i < 0)
                        return true;
                    else if (figure[i, j] == Tetris.FIGURE && (i + elemY + 1 == height - 1))
                        return false;
                    else if (figure[i, j] == Tetris.FIGURE && filledMatrix[i + elemY + 1, j + elemX] == Tetris.FIGURE)
                        return false;

            return true;
        }
    }


    class MainMatrix {
        private static char[,] matrix = new char[25, 40];
        public char[,] getMatrix() {
            return (char[,])matrix.Clone();
        }

        public void setElement(int y, int x, char c) {
            try {
                matrix[y, x] = c;
            } catch (IndexOutOfRangeException ex) { }
        }

        public void clear() {
            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
                for (int j = 0; j <= matrix.GetUpperBound(1); j++)
                    matrix[i, j] = Tetris.EMPTY_CELL;
        }
    }

    //}

}
