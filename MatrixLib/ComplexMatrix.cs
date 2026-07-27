using System.Text;
namespace MatrixLib
{
    public enum ComplexPrintType
    {
        Cartesian,
        Trigonometric,
        Polar
    }
    public readonly struct ComplexNumber
    {
        public double RealPart { get; }
        public double ImaginaryPart { get; }
        public double Magnitude => Math.Sqrt(RealPart * RealPart + ImaginaryPart * ImaginaryPart);
        public double PhaseRad => Math.Atan2(ImaginaryPart, RealPart);
        public ComplexNumber(double realPart, double imaginaryPart)
        {
            RealPart = realPart;
            ImaginaryPart = imaginaryPart;
        }

        public static ComplexNumber FromCartesian(double real, double imaginary)
            => new ComplexNumber(real, imaginary);

        public static ComplexNumber FromPolar(double magnitude, double angleRad)
            => new ComplexNumber(
                magnitude * Math.Cos(angleRad),
                magnitude * Math.Sin(angleRad)
            );
        public string ToString(ComplexPrintType printType, string format = "G")
        {
            string re = RealPart.ToString(format);
            string im = Math.Abs(ImaginaryPart).ToString(format);
            string mag = Magnitude.ToString(format);
            string phase = PhaseRad.ToString(format);

            return printType switch
            {
                ComplexPrintType.Cartesian => ImaginaryPart < 0
                    ? $"{re} - {im}i"
                    : $"{re} + {im}i",

                ComplexPrintType.Trigonometric =>
                    $"{mag} * (cos({phase}) + i*sin({phase}))",

                ComplexPrintType.Polar =>
                    $"{mag} * e^(i*{phase})",

                _ => throw new ArgumentOutOfRangeException(nameof(printType), printType, null)
            };
        }
        public override string ToString() => ToString(ComplexPrintType.Cartesian);
        static public ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
            => new ComplexNumber(a.RealPart + b.RealPart, a.ImaginaryPart + b.ImaginaryPart);
        static public ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
            => new ComplexNumber(a.RealPart - b.RealPart, a.ImaginaryPart - b.ImaginaryPart);
        static public ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
            => new ComplexNumber(a.RealPart * b.RealPart - a.ImaginaryPart * b.ImaginaryPart
                               , a.RealPart * b.ImaginaryPart + a.ImaginaryPart * b.RealPart);
        static public ComplexNumber operator *(double scalar, ComplexNumber a)
            => new ComplexNumber(a.RealPart * scalar, a.ImaginaryPart * scalar);
        static public ComplexNumber operator *(ComplexNumber a, double scalar)
            => new ComplexNumber(a.RealPart * scalar, a.ImaginaryPart * scalar);
        static public ComplexNumber operator /(ComplexNumber a, ComplexNumber b)
            => new ComplexNumber((a.RealPart * b.RealPart + a.ImaginaryPart * b.ImaginaryPart) / (b.Magnitude * b.Magnitude),
                                (a.ImaginaryPart * b.RealPart - a.RealPart * b.ImaginaryPart) / (b.Magnitude * b.Magnitude));
        static public ComplexNumber operator /(double scalar, ComplexNumber a)
        {
            double denominator = a.Magnitude * a.Magnitude;
            return new ComplexNumber(
                (scalar * a.RealPart) / denominator,
                (-scalar * a.ImaginaryPart) / denominator
            );
        }
        public ComplexNumber Conjugate() => new ComplexNumber(RealPart, (-1) * ImaginaryPart);
        public override bool Equals(object obj)
            => obj is ComplexNumber other && RealPart == other.RealPart && ImaginaryPart == other.ImaginaryPart;
        public override int GetHashCode() => HashCode.Combine(RealPart, ImaginaryPart);
        public static bool operator ==(ComplexNumber a, ComplexNumber b) => a.Equals(b);
        public static bool operator !=(ComplexNumber a, ComplexNumber b) => !a.Equals(b);
        public static implicit operator ComplexNumber(double real) => new ComplexNumber(real, 0);

    }
    public class ComplexMatrix
    {
        private const double Epsilon = 1e-12;
        private int rows;
        private int columns;
        private ComplexNumber[,] matrix;
        public ComplexMatrix() { }
        public ComplexMatrix(int rows, int columns)
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
            matrix = new ComplexNumber[rows, columns];
        }
        public ComplexNumber this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }
        public ComplexNumber[,] GetMatrix()
        {
            if (matrix == null)
            {
                throw new InvalidOperationException("Macierz nie została zainicjalizowana.");
            }
            return (ComplexNumber[,])matrix.Clone();
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
        public void SetMatrix(ComplexNumber[,] newMatrix)
        {
            if (newMatrix.GetLength(0) != rows || newMatrix.GetLength(1) != columns)
            {
                throw new InvalidOperationException("New matrix dimensions do not match the current matrix dimensions.");
            }
            matrix = (ComplexNumber[,])newMatrix.Clone();
        }
        public void SetRows(int newRows)
        {
            if (newRows <= 0)
            {
                throw new InvalidOperationException("Number of rows must be greater than zero.");
            }
            ComplexNumber[,] newMatrix = new ComplexNumber[newRows, columns];
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
            ComplexNumber[,] newMatrix = new ComplexNumber[rows, newColumns];
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
        static public ComplexMatrix operator +(ComplexMatrix a, ComplexMatrix b)
        {
            if (a.rows != b.rows || a.columns != b.columns)
                throw new InvalidOperationException("Matrices must have the same dimensions for addition.");
            ComplexNumber[,] newMatrix = new ComplexNumber[a.rows, a.columns];
            ComplexMatrix result = new ComplexMatrix(a.rows, a.columns);
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
        static public ComplexMatrix operator -(ComplexMatrix a, ComplexMatrix b)
        {
            if (a.rows != b.rows || a.columns != b.columns)
                throw new InvalidOperationException("Matrices must have the same dimensions for subtraction.");
            ComplexMatrix result = new ComplexMatrix(a.rows, a.columns);
            ComplexNumber[,] newMatrix = new ComplexNumber[a.rows, a.columns];
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
        static public ComplexMatrix operator *(ComplexMatrix a, ComplexMatrix b)
        {
            if (a.columns != b.rows)
                throw new InvalidOperationException("Number of columns in the first matrix must equal number of rows in the second matrix for multiplication.");
            ComplexMatrix result = new ComplexMatrix(a.rows, b.columns);
            ComplexNumber[,] newMatrix = new ComplexNumber[a.rows, b.columns];
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
        static public ComplexMatrix operator *(ComplexMatrix a, double scalar)
        {
            ComplexMatrix result = new ComplexMatrix(a.rows, a.columns);
            ComplexNumber[,] newMatrix = new ComplexNumber[a.rows, a.columns];
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
        static public ComplexMatrix operator *(double scalar, ComplexMatrix a)
        {
            return a * scalar;
        }
        static public bool operator ==(ComplexMatrix a, ComplexMatrix b)
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
        static public bool operator !=(ComplexMatrix a, ComplexMatrix b)
        {
            return !(a == b);
        }
        private ComplexMatrix Minor(int row, int col)
        {
            if (row < 0 || row >= rows)
                throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of bounds.");
            if (col < 0 || col >= columns)
                throw new ArgumentOutOfRangeException(nameof(col), "Column index is out of bounds.");
            if (!IsSquare)
                throw new InvalidOperationException("Minor can only be computed for square matrices.");
            if (rows < 2)
                throw new InvalidOperationException("Matrix must be at least 2x2 to compute a minor.");
            ComplexMatrix minorMatrix = new ComplexMatrix(rows - 1, columns - 1);
            ComplexNumber[,] minorData = new ComplexNumber[rows - 1, columns - 1];
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
        public ComplexNumber Det()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Determinant can only be computed for square matrices.");
            if (rows == 1)
                return matrix[0, 0];
            if (rows == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            ComplexNumber determinant = 0;
            for (int j = 0; j < columns; j++)
            {
                ComplexMatrix minor = Minor(0, j);
                determinant += (j % 2 == 0 ? 1 : -1) * matrix[0, j] * minor.Det();
            }
            return determinant;
        }
        public ComplexNumber MinorDet(int row, int col)
        {
            if (row < 0 || row >= rows)
                throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of bounds.");
            if (col < 0 || col >= columns)
                throw new ArgumentOutOfRangeException(nameof(col), "Column index is out of bounds.");
            if (!IsSquare)
                throw new InvalidOperationException("Minor determinant can only be computed for square matrices.");
            if (rows < 2)
                throw new InvalidOperationException("Matrix must be at least 2x2 to compute a minor determinant.");
            ComplexMatrix minorMatrix = Minor(row, col);
            return minorMatrix.Det();
        }
        public ComplexMatrix Transpose()
        {
            ComplexMatrix transposed = new ComplexMatrix(columns, rows);
            ComplexNumber[,] transposedData = new ComplexNumber[columns, rows];
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
        public ComplexNumber Trace()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Trace can only be computed for square matrices.");
            ComplexNumber sum = 0;
            for (int i = 0; i < rows; i++)
            {
                sum += matrix[i, i];
            }
            return sum;
        }
        public static ComplexMatrix I(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");
            }
            ComplexMatrix identity = new ComplexMatrix(size, size);
            ComplexNumber[,] identityData = new ComplexNumber[size, size];
            for (int i = 0; i < size; i++)
            {
                identityData[i, i] = 1;
            }
            identity.SetMatrix(identityData);
            return identity;
        }
        public override bool Equals(object obj)
        {
            if (obj is ComplexMatrix other)
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
        public ComplexMatrix Inverse()
        {
            return Solve(I(rows));
        }
        private ComplexMatrix Adjugate()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Adjugate can only be computed for square matrices.");
            ComplexMatrix adjugate = new ComplexMatrix(rows, columns);
            ComplexNumber[,] adjugateData = new ComplexNumber[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var minorDet = MinorDet(i, j);
                    adjugateData[j, i] = ((i + j) % 2 == 0 ? 1 : -1) * minorDet;
                }
            }
            adjugate.SetMatrix(adjugateData);
            return adjugate;
        }
        public (ComplexMatrix L, ComplexMatrix U, ComplexMatrix P) LU()
        {
            if (!IsSquare)
                throw new InvalidOperationException("LU decomposition can only be computed for square matrices.");

            ComplexNumber[,] u = new ComplexNumber[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    u[i, j] = this[i, j];

            ComplexNumber[,] l = new ComplexNumber[rows, columns];
            int[] perm = new int[rows];
            for (int i = 0; i < rows; i++)
                perm[i] = i;

            for (int k = 0; k < rows; k++)
            {
                int pivotRow = k;
                var maxMag = u[k, k].Magnitude;
                for (int i = k + 1; i < rows; i++)
                {
                    if (u[i, k].Magnitude > maxMag)
                    {
                        maxMag = u[i, k].Magnitude;
                        pivotRow = i;
                    }
                }
                if (maxMag < Epsilon)
                    throw new InvalidOperationException("Macierz jest osobliwa.");
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
                    var factor = u[i, k] / u[k, k];
                    l[i, k] = factor;
                    for (int j = k; j < columns; j++)
                    {
                        u[i, j] -= factor * u[k, j];
                    }
                }
            }
            ComplexMatrix L = new ComplexMatrix(rows, columns);
            L.SetMatrix(l);
            ComplexMatrix U = new ComplexMatrix(rows, columns);
            U.SetMatrix(u);

            ComplexNumber[,] pData = new ComplexNumber[rows, columns];
            for (int i = 0; i < rows; i++)
                pData[i, perm[i]] = 1;
            ComplexMatrix P = new ComplexMatrix(rows, columns);
            P.SetMatrix(pData);

            return (L, U, P);
        }
        public ComplexMatrix Solve(ComplexMatrix bVector)
        {
            var (L, U, P) = LU();
            ComplexMatrix pb = P * bVector;
            int cols = bVector.Columns;
            ComplexMatrix solution = new ComplexMatrix(rows, cols);
            ComplexMatrix y = new ComplexMatrix(rows, cols);
            for (int col = 0; col < cols; col++)
            {
                for (int i = 0; i < rows; i++)
                {
                    ComplexNumber tdiff = 0;
                    for (int j = 0; j < i; j++)
                        tdiff += L[i, j] * y[j, col];
                    y[i, col] = (pb[i, col] - tdiff) / L[i, i];
                }

                for (int i = rows - 1; i >= 0; i--)
                {
                    ComplexNumber tdiff = 0;
                    for (int j = i + 1; j < rows; j++)
                        tdiff += U[i, j] * solution[j, col];
                    solution[i, col] = (y[i, col] - tdiff) / U[i, i];
                }
            }

            return solution;
        }
    }
}