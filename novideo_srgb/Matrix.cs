﻿using System;

namespace novideo_srgb
{
    public class Matrix
    {
        private double[,] _values;

        private Matrix()
        {
        }

        public double this[int x, int y]
        {
            get => _values[x, y];
            set => _values[x, y] = value;
        }

        public int Rows => _values.GetLength(0);
        public int Cols => _values.GetLength(1);

        public static Matrix FromValues(double[,] array)
        {
            if (array.GetLength(0) != 3 || !(array.GetLength(1) == 3 || array.GetLength(1) == 1))
            {
                throw new ArgumentException("Array must be 3x3 or 3x1");
            }

            var result = new Matrix
            {
                _values = array
            };
            return result;
        }

        public static Matrix Zero3x3()
        {
            return FromValues(new double[3, 3]);
        }

        public static Matrix Zero3x1()
        {
            return FromValues(new double[3, 1]);
        }

        public static Matrix One3x1()
        {
            return FromValues(new double[,] { { 1 }, { 1 }, { 1 } });
        }

        public static Matrix FromDiagonal(double[] array)
        {
            if (array.Length != 3)
            {
                throw new ArgumentException("Array must have length 3");
            }

            var result = new Matrix
            {
                _values = new double[3, 3]
            };
            for (var i = 0; i < 3; i++)
            {
                result._values[i, i] = array[i];
            }

            return result;
        }

        public static Matrix FromDiagonal(Matrix column)
        {
            if (column.Cols != 1)
            {
                throw new ArgumentException("Matrix must be 3x1");
            }

            return FromDiagonal(new[] { column[0, 0], column[1, 0], column[2, 0] });
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Cols != 3)
            {
                throw new ArgumentException("Left side must be 3x3");
            }

            var result = b.Cols == 3 ? Zero3x3() : Zero3x1();

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < result.Cols; j++)
                {
                    for (var k = 0; k < 3; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return result;
        }

        public static Matrix operator *(double a, Matrix b)
        {
            var result = b.Cols == 1 ? Zero3x1() : Zero3x3();

            for (var i = 0; i < result.Rows; i++)
            {
                for (var j = 0; j < result.Cols; j++)
                {
                    result[i, j] = a * b[i, j];
                }
            }

            return result;
        }

        public Matrix Inverse()
        {
            if (Cols != 3)
            {
                throw new ArgumentException("Matrix must be 3x3");
            }

            var a = this[0, 0];
            var b = this[0, 1];
            var c = this[0, 2];
            var d = this[1, 0];
            var e = this[1, 1];
            var f = this[1, 2];
            var g = this[2, 0];
            var h = this[2, 1];
            var i = this[2, 2];

            var denom = a * e * i - a * f * h - b * d * i + b * f * g + c * d * h - c * e * g;
            return 1 / denom * FromValues(new[,]
            {
                { e * i - f * h, -b * i + c * h, b * f - c * e },
                { -d * i + f * g, a * i - c * g, -a * f + c * d },
                { d * h - e * g, -a * h + b * g, a * e - b * d }
            });
        }
    }
}