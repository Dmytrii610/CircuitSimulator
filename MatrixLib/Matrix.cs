using System.Text;
namespace MatrixLib
{
    public class Matrix
    {
        private const double Epsilon = 1e-12;
        private int rows;
        private int columns;
        private double[,] matrix;
        public Matrix() { }
        public Matrix(int rows, int columns)
        {
            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), "Liczba wierszy musi być większa od zera.");
            }
            if (columns <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), "Liczba kolumn musi być większa od zera.");
            }
            this.rows = rows;
            this.columns = columns;
            matrix = new double[rows, columns];
        }
        public double this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }
        public double[,] GetMatrix()
        {
            if (matrix == null)
            {
                throw new InvalidOperationException("Macierz nie została zainicjalizowana.");
            }
            return (double[,])matrix.Clone();
        }
        public int Rows
        {
            get { return rows; }
        }
        public int Columns
        {
            get { return columns; }
        }
        public bool IsSquare
        {
            get { return rows == columns; }
        }
        public void SetMatrix(double[,] newMatrix)
        {
            if (newMatrix.GetLength(0) != rows || newMatrix.GetLength(1) != columns)
            {
                throw new InvalidOperationException("New matrix dimensions do not match the current matrix dimensions.");
            }
            matrix = (double[,])newMatrix.Clone();
        }
        public void SetRows(int newRows)
        {
            if (newRows <= 0)
            {
                throw new InvalidOperationException("Number of rows must be greater than zero.");
            }
            double[,] newMatrix = new double[newRows, columns];
            for (int i = 0; i < Math.Min(rows, newRows); i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    newMatrix[i, j] = matrix[i, j];
                }
            }
            rows = newRows;
            matrix = newMatrix;
        }
        public void SetColumns(int newColumns)
        {
            if (newColumns <= 0)
            {
                throw new InvalidOperationException("Number of columns must be greater than zero.");
            }
            double[,] newMatrix = new double[rows, newColumns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < Math.Min(columns, newColumns); j++)
                {
                    newMatrix[i, j] = matrix[i, j];
                }
            }
            columns = newColumns;
            matrix = newMatrix;
        }
        public void Print()
        {
            if (matrix == null) throw new InvalidOperationException("Matrix is not initialized.");
            Console.Write(ToString());
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    sb.Append(matrix[i, j]).Append('\t');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        static public Matrix operator +(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.columns != b.columns)
                throw new InvalidOperationException("Matrices must have the same dimensions for addition.");
            double[,] newMatrix = new double[a.rows, a.columns];
            Matrix result = new Matrix(a.rows, a.columns);
            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < a.columns; j++)
                {
                    newMatrix[i, j] = a[i, j] + b[i, j];
                }
            }
            result.SetMatrix(newMatrix);
            return result;
        }
        static public Matrix operator -(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.columns != b.columns)
                throw new InvalidOperationException("Matrices must have the same dimensions for subtraction.");
            Matrix result = new Matrix(a.rows, a.columns);
            double[,] newMatrix = new double[a.rows, a.columns];
            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < a.columns; j++)
                {
                    newMatrix[i, j] = a[i, j] - b[i, j];
                }
            }
            result.SetMatrix(newMatrix);
            return result;
        }
        static public Matrix operator *(Matrix a, Matrix b)
        {
            if (a.columns != b.rows)
                throw new InvalidOperationException("Number of columns in the first matrix must equal number of rows in the second matrix for multiplication.");
            Matrix result = new Matrix(a.rows, b.columns);
            double[,] newMatrix = new double[a.rows, b.columns];
            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < b.columns; j++)
                {
                    for (int k = 0; k < a.columns; k++)
                    {
                        newMatrix[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            result.SetMatrix(newMatrix);
            return result;
        }
        static public Matrix operator *(Matrix a, double scalar)
        {
            Matrix result = new Matrix(a.rows, a.columns);
            double[,] newMatrix = new double[a.rows, a.columns];
            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < a.columns; j++)
                {
                    newMatrix[i, j] = a[i, j] * scalar;
                }
            }
            result.SetMatrix(newMatrix);
            return result;
        }
        static public Matrix operator *(double scalar, Matrix a)
        {
            return a * scalar;
        }
        static public bool operator ==(Matrix a, Matrix b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                return false;
            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < a.columns; j++)
                {
                    if (a[i, j] != b[i, j])
                        return false;
                }
            }
            return true;
        }
        static public bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }
        private Matrix Minor(int row, int col)
        {
            if (row < 0 || row >= rows)
                throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of bounds.");
            if (col < 0 || col >= columns)
                throw new ArgumentOutOfRangeException(nameof(col), "Column index is out of bounds.");
            if (!IsSquare)
                throw new InvalidOperationException("Minor can only be computed for square matrices.");
            if (rows < 2)
                throw new InvalidOperationException("Matrix must be at least 2x2 to compute a minor.");
            Matrix minorMatrix = new Matrix(rows - 1, columns - 1);
            double[,] minorData = new double[rows - 1, columns - 1];
            int mi = 0;
            for (int i = 0; i < rows; i++)
            {
                if (i == row) continue;

                int mj = 0;
                for (int j = 0; j < columns; j++)
                {
                    if (j == col) continue;

                    minorData[mi, mj] = matrix[i, j];
                    mj++;
                }
                mi++;
            }
            minorMatrix.SetMatrix(minorData);
            return minorMatrix;
        }
        public double Det()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Determinant can only be computed for square matrices.");
            if (rows == 1)
                return matrix[0, 0];
            if (rows == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            double determinant = 0;
            for (int j = 0; j < columns; j++)
            {
                Matrix minor = Minor(0, j);
                determinant += (j % 2 == 0 ? 1 : -1) * matrix[0, j] * minor.Det();
            }
            return determinant;
        }
        public double MinorDet(int row, int col)
        {
            if (row < 0 || row >= rows)
                throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of bounds.");
            if (col < 0 || col >= columns)
                throw new ArgumentOutOfRangeException(nameof(col), "Column index is out of bounds.");
            if (!IsSquare)
                throw new InvalidOperationException("Minor determinant can only be computed for square matrices.");
            if (rows < 2)
                throw new InvalidOperationException("Matrix must be at least 2x2 to compute a minor determinant.");
            Matrix minorMatrix = Minor(row, col);
            return minorMatrix.Det();
        }
        public Matrix Transpose()
        {
            Matrix transposed = new Matrix(columns, rows);
            double[,] transposedData = new double[columns, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    transposedData[j, i] = matrix[i, j];
                }
            }
            transposed.SetMatrix(transposedData);
            return transposed;
        }
        public double Trace()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Trace can only be computed for square matrices.");
            double sum = 0;
            for (int i = 0; i < rows; i++)
            {
                sum += matrix[i, i];
            }
            return sum;
        }
        public static Matrix I(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");
            }
            Matrix identity = new Matrix(size, size);
            double[,] identityData = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                identityData[i, i] = 1;
            }
            identity.SetMatrix(identityData);
            return identity;
        }
        public override bool Equals(object obj)
        {
            if (obj is Matrix other)
            {
                return this == other;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + rows.GetHashCode();
            hash = hash * 23 + columns.GetHashCode();
            foreach (var value in matrix)
            {
                hash = hash * 23 + value.GetHashCode();
            }
            return hash;
        }
        public Matrix Inverse()
        {
            return Solve(I(rows));
        }
        private Matrix Adjugate()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Adjugate can only be computed for square matrices.");
            Matrix adjugate = new Matrix(rows, columns);
            double[,] adjugateData = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    double minorDet = MinorDet(i, j);
                    adjugateData[j, i] = ((i + j) % 2 == 0 ? 1 : -1) * minorDet;
                }
            }
            adjugate.SetMatrix(adjugateData);
            return adjugate;
        }
        public (Matrix L, Matrix U, Matrix P) LU()
        {
            if (!IsSquare)
                throw new InvalidOperationException("LU decomposition can only be computed for square matrices.");

            double[,] u = new double[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    u[i, j] = this[i, j];

            double[,] l = new double[rows, columns];
            int[] perm = new int[rows];
            for (int i = 0; i < rows; i++)
                perm[i] = i;

            for (int k = 0; k < rows; k++)
            {
                int pivotRow = k;
                double maxVal = Math.Abs(u[k, k]);
                for (int i = k + 1; i < rows; i++)
                {
                    if (Math.Abs(u[i, k]) > maxVal)
                    {
                        maxVal = Math.Abs(u[i, k]);
                        pivotRow = i;
                    }
                }
                if (Math.Abs(maxVal) < Epsilon)
                    throw new InvalidOperationException("kfdjsl");
                if (pivotRow != k)
                {
                    for (int j = 0; j < columns; j++)
                        (u[k, j], u[pivotRow, j]) = (u[pivotRow, j], u[k, j]);
                    for (int j = 0; j < k; j++)
                        (l[k, j], l[pivotRow, j]) = (l[pivotRow, j], l[k, j]);
                    (perm[k], perm[pivotRow]) = (perm[pivotRow], perm[k]);
                }

                l[k, k] = 1;
                for (int i = k + 1; i < rows; i++)
                {
                    double factor = u[i, k] / u[k, k];
                    l[i, k] = factor;
                    for (int j = k; j < columns; j++)
                    {
                        u[i, j] -= factor * u[k, j];
                    }
                }
            }
            Matrix L = new Matrix(rows, columns);
            L.SetMatrix(l);
            Matrix U = new Matrix(rows, columns);
            U.SetMatrix(u);

            double[,] pData = new double[rows, columns];
            for (int i = 0; i < rows; i++)
                pData[i, perm[i]] = 1;
            Matrix P = new Matrix(rows, columns);
            P.SetMatrix(pData);

            return (L, U, P);
        }
        public Matrix Solve(Matrix bVector)
        {
            var (L, U, P) = LU();
            Matrix pb = P * bVector;
            int cols = bVector.Columns;
            Matrix solution = new Matrix(rows, cols);
            Matrix y = new Matrix(rows, cols);
            for (int col = 0; col < cols; col++)
            {
                for (int i = 0; i < rows; i++)
                {
                    double tdiff = 0;
                    for (int j = 0; j < i; j++)
                        tdiff += L[i, j] * y[j, col];
                    y[i, col] = (pb[i, col] - tdiff) / L[i, i];
                }

                for (int i = rows - 1; i >= 0; i--)
                {
                    double tdiff = 0;
                    for (int j = i + 1; j < rows; j++)
                        tdiff += U[i, j] * solution[j, col];
                    solution[i, col] = (y[i, col] - tdiff) / U[i, i];
                }
            }

            return solution;
        }
        class Program
        {
            static void Main(string[] args)
            {
                Matrix a = new Matrix(4, 4);
                a.SetMatrix(new double[,]
                {
                    {2, 0, 1,-1 },
                    {3, 1, 0, 4},
                    {1, -1, 2, 3},
                    {0, 2, 1, 1}
                });
                Matrix b = new Matrix(3, 3);
                b.SetMatrix(new double[,]
                {
                    { -1, 8, 7 },
                    { 9, 4, 3 },
                    { 3, 2, 1 }
                });
                Console.WriteLine(a.Det());
            }
        }
    }
}